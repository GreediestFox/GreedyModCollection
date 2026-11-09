#!/usr/bin/env node
// Local dev server for the site + image replacement helper.
//   node tools/admin-server.mjs [--port 8000] [--root <site folder>]
//
// Serves the site on http://localhost:<port>/ (127.0.0.1 only) and adds a few /__admin/ endpoints
// that js/admin.js uses to upload pictures and replace the ones the site shows.
// Use it INSTEAD of your normal dev server while you edit. Never run it on a public machine.

import http from 'node:http';
import { createReadStream } from 'node:fs';
import { readFile, stat, readdir, mkdir, copyFile, writeFile } from 'node:fs/promises';
import path from 'node:path';
import crypto from 'node:crypto';
import { fileURLToPath } from 'node:url';

const here = path.dirname(fileURLToPath(import.meta.url));
const args = process.argv.slice(2);
function opt(flag, dflt) { const i = args.indexOf(flag); return i !== -1 && args[i + 1] ? args[i + 1] : dflt; }
const PORT = Number(opt('--port', 8000));
// Layout of this project: tools/ and uploads/ sit NEXT TO public/ (the folder that gets deployed), never inside it.
const ROOT = path.resolve(opt('--root', path.join(here, '..', 'public')));
const UPLOADS = path.resolve(path.join(here, '..', 'uploads'));
const ADMIN_JS = path.join(here, 'admin.js');   // served only by this local server, injected into HTML pages below
const ASSETS = path.join(ROOT, 'assets');
const MAX_BYTES = 30 * 1024 * 1024;
const IMG_EXT = new Set(['.jpg', '.jpeg', '.png', '.webp', '.gif', '.avif']);
const token = crypto.randomBytes(24).toString('hex');

const MIME = {
  '.html': 'text/html; charset=utf-8', '.css': 'text/css; charset=utf-8', '.js': 'text/javascript; charset=utf-8',
  '.mjs': 'text/javascript; charset=utf-8', '.json': 'application/json; charset=utf-8', '.txt': 'text/plain; charset=utf-8',
  '.md': 'text/plain; charset=utf-8', '.png': 'image/png', '.jpg': 'image/jpeg', '.jpeg': 'image/jpeg', '.webp': 'image/webp',
  '.gif': 'image/gif', '.avif': 'image/avif', '.svg': 'image/svg+xml', '.ico': 'image/x-icon', '.woff': 'font/woff', '.woff2': 'font/woff2',
};

/* ---------- guards: only the page served by this very server may use the admin API ---------- */
const hostOk = (req) => /^(localhost|127\.0\.0\.1|\[::1\])(:\d+)?$/i.test(req.headers.host || '');
const siteOk = (req) => { const s = req.headers['sec-fetch-site']; return !s || s === 'same-origin' || s === 'none'; };
const originOk = (req) => { const o = req.headers.origin; if (!o) return true; try { return new URL(o).host === req.headers.host; } catch { return false; } };
function tokenOk(req) {
  const t = String(req.headers['x-admin-token'] || '');
  return t.length === token.length && crypto.timingSafeEqual(Buffer.from(t), Buffer.from(token));
}
const inside = (base, p) => { const r = path.relative(base, p); return !!r && !r.startsWith('..') && !path.isAbsolute(r); };

/* ---------- helpers ---------- */
function send(res, code, body, headers = {}) {
  const isObj = body !== null && typeof body === 'object' && !Buffer.isBuffer(body);
  res.writeHead(code, { 'Cache-Control': 'no-store', ...(isObj ? { 'Content-Type': 'application/json; charset=utf-8' } : {}), ...headers });
  res.end(isObj ? JSON.stringify(body) : body);
}
function readBody(req, limit) {
  return new Promise((resolve, reject) => {
    const chunks = []; let n = 0;
    req.on('data', (c) => { n += c.length; if (n > limit) { reject(Object.assign(new Error('too large'), { code: 413 })); req.destroy(); } else chunks.push(c); });
    req.on('end', () => resolve(Buffer.concat(chunks)));
    req.on('error', reject);
  });
}
function sniff(b) {
  if (b.length < 12) return null;
  if (b[0] === 0xff && b[1] === 0xd8 && b[2] === 0xff) return 'jpg';
  if (b.subarray(0, 8).equals(Buffer.from([0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a]))) return 'png';
  if (b.subarray(0, 4).toString('latin1') === 'RIFF' && b.subarray(8, 12).toString('latin1') === 'WEBP') return 'webp';
  if (b.subarray(0, 4).toString('latin1') === 'GIF8') return 'gif';
  if (b.subarray(4, 8).toString('latin1') === 'ftyp' && /avi[fs]/.test(b.subarray(8, 16).toString('latin1'))) return 'avif';
  return null;
}
function cleanName(name) {
  const base = path.basename(String(name || 'image')).replace(/\.[^.]*$/, '');
  return (base.replace(/[^A-Za-z0-9._-]+/g, '_').replace(/^\.+/, '').slice(0, 80)) || 'image';
}
async function exists(p) { try { await stat(p); return true; } catch { return false; } }
function resolveTarget(t) {
  if (typeof t !== 'string' || t.includes('\0') || t.includes('\\')) return null;
  const norm = path.posix.normalize(t);
  if (norm.startsWith('/') || norm.startsWith('..') || !norm.startsWith('assets/')) return null;
  const abs = path.join(ROOT, ...norm.split('/'));
  if (!inside(ASSETS, abs) || !IMG_EXT.has(path.extname(abs).toLowerCase())) return null;
  return { rel: norm, abs };
}

/* ---------- admin API ---------- */
async function admin(req, res, url) {
  if (!hostOk(req) || !originOk(req)) return send(res, 403, { error: 'forbidden' });
  const route = url.pathname.replace('/__admin', '') || '/';

  if (req.method === 'GET' && route === '/admin.js') {
    return send(res, 200, await readFile(ADMIN_JS), { 'Content-Type': MIME['.js'] });
  }
  if (req.method === 'GET' && route === '/ping') {
    if (!siteOk(req)) return send(res, 403, { error: 'forbidden' });
    return send(res, 200, { ok: true, token });
  }
  if (!tokenOk(req)) return send(res, 403, { error: 'bad token (reload the page)' });

  if (req.method === 'GET' && route === '/uploads') {
    await mkdir(UPLOADS, { recursive: true });
    const out = [];
    for (const d of await readdir(UPLOADS, { withFileTypes: true })) {
      if (!d.isFile() || !IMG_EXT.has(path.extname(d.name).toLowerCase())) continue;
      const s = await stat(path.join(UPLOADS, d.name));
      out.push({ name: d.name, size: s.size, mtime: s.mtimeMs });
    }
    out.sort((a, b) => b.mtime - a.mtime);
    return send(res, 200, out);
  }

  if (req.method === 'POST' && route === '/upload') {
    let body;
    try { body = await readBody(req, MAX_BYTES); } catch (e) { return send(res, e.code || 400, { error: 'file too large (max 30 MB)' }); }
    const ext = sniff(body);
    if (!ext) return send(res, 415, { error: 'not a jpg, png, webp, gif or avif picture' });
    await mkdir(UPLOADS, { recursive: true });
    const base = cleanName(url.searchParams.get('name'));
    let name = `${base}.${ext}`, n = 1;
    while (await exists(path.join(UPLOADS, name))) name = `${base}-${n++}.${ext}`;
    await writeFile(path.join(UPLOADS, name), body);
    return send(res, 200, { ok: true, name });
  }

  if (req.method === 'POST' && route === '/replace') {
    let data;
    try { data = JSON.parse((await readBody(req, 4096)).toString('utf8')); } catch { return send(res, 400, { error: 'bad request' }); }
    const target = resolveTarget(data.target);
    if (!target) return send(res, 400, { error: 'that picture cannot be replaced (must be an image under assets/)' });
    const srcName = path.basename(String(data.source || ''));
    const srcAbs = path.join(UPLOADS, srcName);
    if (!srcName || srcName !== data.source || !inside(UPLOADS, srcAbs) || !IMG_EXT.has(path.extname(srcName).toLowerCase()) || !(await exists(srcAbs)))
      return send(res, 400, { error: 'source file not found in uploads/' });
    if (!sniff(await readFile(srcAbs))) return send(res, 415, { error: 'source is not a valid picture' });
    if (await exists(target.abs)) {
      const bdir = path.join(UPLOADS, '_replaced');
      await mkdir(bdir, { recursive: true });
      const stamp = new Date().toISOString().replace(/[:.]/g, '-');
      await copyFile(target.abs, path.join(bdir, `${stamp}__${target.rel.replace(/\//g, '_')}`));
    }
    await mkdir(path.dirname(target.abs), { recursive: true });
    await copyFile(srcAbs, target.abs);
    return send(res, 200, { ok: true, target: target.rel });
  }
  return send(res, 404, { error: 'not found' });
}

/* ---------- static files ---------- */
async function serveStatic(req, res, url) {
  if (req.method !== 'GET' && req.method !== 'HEAD') return send(res, 405, 'Method not allowed');
  let p;
  try { p = decodeURIComponent(url.pathname); } catch { return send(res, 400, 'Bad request'); }
  if (p.includes('\0') || p.split('/').some((s) => s.startsWith('.'))) return send(res, 404, 'Not found');
  // /uploads/... comes from the folder next to public/ (thumbnails in the "Replace image" dialog)
  const fromUploads = p.startsWith('/uploads/');
  const base = fromUploads ? UPLOADS : ROOT;
  let abs = path.resolve(path.join(base, fromUploads ? p.slice('/uploads/'.length) : p));
  if (abs !== base && !inside(base, abs)) return send(res, 403, 'Forbidden');
  try {
    let s = await stat(abs);
    if (s.isDirectory()) { abs = path.join(abs, 'index.html'); s = await stat(abs); }
    const ext = path.extname(abs).toLowerCase();
    if (ext === '.html' && !fromUploads) {
      // add the owner tools to every page while this local server runs (they are not part of the deployed site)
      let html = await readFile(abs, 'utf8');
      html = html.replace('</body>', '<script src="/__admin/admin.js"></script></body>');
      const buf = Buffer.from(html, 'utf8');
      res.writeHead(200, { 'Content-Type': MIME['.html'], 'Content-Length': buf.length, 'Cache-Control': 'no-store' });
      return req.method === 'HEAD' ? res.end() : res.end(buf);
    }
    res.writeHead(200, { 'Content-Type': MIME[ext] || 'application/octet-stream', 'Content-Length': s.size, 'Cache-Control': 'no-store' });
    if (req.method === 'HEAD') return res.end();
    createReadStream(abs).pipe(res);
  } catch { send(res, 404, 'Not found'); }
}

const server = http.createServer(async (req, res) => {
  try {
    const url = new URL(req.url, 'http://localhost');
    if (url.pathname.startsWith('/__admin/')) return await admin(req, res, url);
    await serveStatic(req, res, url);
  } catch (e) {
    console.error(e);
    if (!res.headersSent) send(res, 500, { error: 'server error' });
  }
});
server.on('error', (e) => {
  console.error(e.code === 'EADDRINUSE' ? `Port ${PORT} is already in use. Stop your other dev server or use --port 8001.` : e);
  process.exit(1);
});
server.listen(PORT, '127.0.0.1', () => {
  console.log(`Site:   http://localhost:${PORT}/`);
  console.log(`Edit:   node tools/maint.mjs on "message"   then open   http://localhost:${PORT}/?preview`);
  console.log(`Folder: ${UPLOADS}  (new pictures land here; replaced ones are backed up in uploads/_replaced/)`);
});
