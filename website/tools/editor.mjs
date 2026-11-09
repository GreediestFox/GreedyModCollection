#!/usr/bin/env node
// Team editor: a small form for public/data/team.json, featured.json, thanks.json (+ English texts in i18n-en.json).
//   node tools/editor.mjs [--port 8002]
// Opens nothing by itself: start it, then open http://localhost:8002/__edit/ in your browser. Stop it with Ctrl+C.
// Listens on 127.0.0.1 only (your own PC). Writes only the files named above and pictures under public/assets/img/team|featured.
// Every save first copies the old files to backups/<date-time>/ next to public/.

import http from 'node:http';
import { createReadStream } from 'node:fs';
import { readFile, writeFile, stat, mkdir, copyFile } from 'node:fs/promises';
import path from 'node:path';
import crypto from 'node:crypto';
import { fileURLToPath } from 'node:url';

const here = path.dirname(fileURLToPath(import.meta.url));
const args = process.argv.slice(2);
const opt = (f, d) => { const i = args.indexOf(f); return i !== -1 && args[i + 1] ? args[i + 1] : d; };
const PORT = Number(opt('--port', 8002));
const ROOT = path.resolve(here, '..', 'public');
const DATA = path.join(ROOT, 'data');
const BACKUPS = path.resolve(here, '..', 'backups');
const token = crypto.randomBytes(24).toString('hex');

const FILES = { team: 'team.json', featured: 'featured.json', thanks: 'thanks.json', i18n: 'i18n-en.json' };
const DEFAULTS = {
  team: { recruiting: false, recruitIntro: '', noRolesText: '', members: [], openRoles: [] },
  featured: [], thanks: [], i18n: { texts: {}, patterns: [] },
};
const MIME = {
  '.html': 'text/html; charset=utf-8', '.css': 'text/css; charset=utf-8', '.js': 'text/javascript; charset=utf-8', '.mjs': 'text/javascript; charset=utf-8',
  '.json': 'application/json; charset=utf-8', '.png': 'image/png', '.jpg': 'image/jpeg', '.jpeg': 'image/jpeg', '.webp': 'image/webp',
  '.gif': 'image/gif', '.avif': 'image/avif', '.svg': 'image/svg+xml', '.ico': 'image/x-icon', '.woff': 'font/woff', '.woff2': 'font/woff2', '.ttf': 'font/ttf', '.txt': 'text/plain; charset=utf-8',
};

/* ---------- guards: only the page served by this very server may use the API ---------- */
const hostOk = (req) => /^(localhost|127\.0\.0\.1|\[::1\])(:\d+)?$/i.test(req.headers.host || '');
const siteOk = (req) => { const s = req.headers['sec-fetch-site']; return !s || s === 'same-origin' || s === 'none'; };
const originOk = (req) => { const o = req.headers.origin; if (!o) return true; try { return new URL(o).host === req.headers.host; } catch { return false; } };
function tokenOk(req) {
  const t = String(req.headers['x-editor-token'] || '');
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
async function readJson(name) {
  try { return JSON.parse((await readFile(path.join(DATA, FILES[name]), 'utf8')).replace(/^﻿/, '')); }
  catch { return structuredClone(DEFAULTS[name]); }
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
const cleanName = (n) => (path.basename(String(n || 'bild')).replace(/\.[^.]*$/, '').replace(/[^A-Za-z0-9._-]+/g, '_').replace(/^\.+/, '').slice(0, 60)) || 'bild';
async function exists(p) { try { await stat(p); return true; } catch { return false; } }

/* ---------- cleaning what the form sends (never trust the browser) ---------- */
const S = (v, max = 4000) => String(v == null ? '' : v).replace(/\r\n?/g, '\n').slice(0, max);
const URLs = (v) => { const s = S(v, 500).trim(); return /^https?:\/\//i.test(s) ? s : ''; };
const IMG = (v) => { const s = S(v, 300).trim(); return /^assets\/img\/[A-Za-z0-9_\-\/.]+$/.test(s) && !s.includes('..') ? s : ''; };
const arr = (v, max) => (Array.isArray(v) ? v.slice(0, max) : []);
const links = (v) => arr(v, 12).map((l) => ({ label: S(l && l.label, 80), url: URLs(l && l.url) })).filter((l) => l.url);
function cleanTeam(t) {
  t = t && typeof t === 'object' ? t : {};
  return {
    recruiting: !!t.recruiting,
    recruitIntro: S(t.recruitIntro),
    noRolesText: S(t.noRolesText),
    members: arr(t.members, 60).map((m) => {
      const o = { name: S(m && m.name, 120), role: S(m && m.role, 200), bio: S(m && m.bio) };
      const img = IMG(m && m.image); if (img) o.image = img;
      o.links = links(m && m.links);
      return o;
    }).filter((m) => m.name),
    openRoles: arr(t.openRoles, 30).map((r) => ({
      title: S(r && r.title, 120), description: S(r && r.description),
      tags: arr(r && r.tags, 8).map((x) => S(x, 40)).filter(Boolean),
    })).filter((r) => r.title),
  };
}
function cleanFeatured(f) {
  return arr(f, 30).map((x) => {
    const name = S(x && x.name, 120);
    const id = S(x && x.id, 60).toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '') || name.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '') || 'eintrag';
    return {
      id, name, kind: S(x && x.kind, 80), where: S(x && x.where, 120), role: S(x && x.role, 200),
      text: arr(x && x.text, 12).map((p) => S(p)).filter(Boolean),
      quote: S(x && x.quote, 600),
      images: arr(x && x.images, 8).map(IMG).filter(Boolean),
      links: links(x && x.links),
    };
  }).filter((x) => x.name);
}
const cleanThanks = (t) => arr(t, 300).map((p) => ({ name: S(p && p.name, 120), role: S(p && p.role, 120) })).filter((p) => p.name);

/* ---------- API ---------- */
async function api(req, res, url) {
  if (!hostOk(req) || !originOk(req)) return send(res, 403, { error: 'forbidden' });
  const route = url.pathname.replace('/__edit/api', '') || '/';

  if (req.method === 'GET' && route === '/state') {
    if (!siteOk(req)) return send(res, 403, { error: 'forbidden' });
    const i18n = await readJson('i18n');
    return send(res, 200, {
      token, team: await readJson('team'), featured: await readJson('featured'), thanks: await readJson('thanks'),
      dict: (i18n && i18n.texts) || {},
    });
  }
  if (!tokenOk(req)) return send(res, 403, { error: 'Ungültiges Token – Seite neu laden.' });

  if (req.method === 'POST' && route === '/upload') {
    const kind = url.searchParams.get('kind') === 'featured' ? 'featured' : 'team';
    let body;
    try { body = await readBody(req, 15 * 1024 * 1024); } catch (e) { return send(res, e.code || 400, { error: 'Bild zu groß (max. 15 MB).' }); }
    const ext = sniff(body);
    if (!ext) return send(res, 415, { error: 'Das ist kein jpg-, png-, webp-, gif- oder avif-Bild.' });
    const dir = path.join(ROOT, 'assets', 'img', kind);
    await mkdir(dir, { recursive: true });
    const name = `${cleanName(url.searchParams.get('name'))}-${crypto.randomBytes(2).toString('hex')}.${ext}`;
    await writeFile(path.join(dir, name), body);
    return send(res, 200, { ok: true, path: `assets/img/${kind}/${name}` });
  }

  if (req.method === 'POST' && route === '/save') {
    let data;
    try { data = JSON.parse((await readBody(req, 2 * 1024 * 1024)).toString('utf8')); } catch { return send(res, 400, { error: 'Ungültige Daten.' }); }
    const out = { team: cleanTeam(data.team), featured: cleanFeatured(data.featured), thanks: cleanThanks(data.thanks) };
    // English texts -> i18n-en.json (only adds / updates entries, never deletes any)
    const i18n = await readJson('i18n');
    i18n.texts = i18n.texts || {};
    let added = 0;
    for (const [k, v] of Object.entries(data.en && typeof data.en === 'object' ? data.en : {}).slice(0, 3000)) {
      const de = S(k, 3000).replace(/\s+/g, ' ').trim(), en = S(v, 3000).trim();
      if (de && en && i18n.texts[de] !== en) { i18n.texts[de] = en; added++; }
    }
    // backup, then write
    const stamp = new Date().toISOString().replace(/[:.]/g, '-');
    const bdir = path.join(BACKUPS, stamp);
    await mkdir(bdir, { recursive: true });
    for (const f of Object.values(FILES)) if (await exists(path.join(DATA, f))) await copyFile(path.join(DATA, f), path.join(bdir, f));
    const w = (name, obj, indent) => writeFile(path.join(DATA, FILES[name]), JSON.stringify(obj, null, indent) + '\n', 'utf8');
    await w('team', out.team, 2); await w('featured', out.featured, 2); await w('thanks', out.thanks, 2); await w('i18n', i18n, 1);
    return send(res, 200, { ok: true, backup: path.relative(path.resolve(here, '..'), bdir), englishUpdated: added });
  }
  return send(res, 404, { error: 'not found' });
}

/* ---------- static files (so "Seite ansehen" shows the real site on your PC) ---------- */
async function serveStatic(req, res, url) {
  if (req.method !== 'GET' && req.method !== 'HEAD') return send(res, 405, 'Method not allowed');
  let p;
  try { p = decodeURIComponent(url.pathname); } catch { return send(res, 400, 'Bad request'); }
  if (p.includes('\0') || p.split('/').some((s) => s.startsWith('.'))) return send(res, 404, 'Not found');
  let abs = path.resolve(path.join(ROOT, p));
  if (abs !== ROOT && !inside(ROOT, abs)) return send(res, 403, 'Forbidden');
  try {
    let s = await stat(abs);
    if (s.isDirectory()) { abs = path.join(abs, 'index.html'); s = await stat(abs); }
    res.writeHead(200, { 'Content-Type': MIME[path.extname(abs).toLowerCase()] || 'application/octet-stream', 'Content-Length': s.size, 'Cache-Control': 'no-store' });
    if (req.method === 'HEAD') return res.end();
    createReadStream(abs).pipe(res);
  } catch { send(res, 404, 'Not found'); }
}

const server = http.createServer(async (req, res) => {
  try {
    const url = new URL(req.url, 'http://localhost');
    if (url.pathname.startsWith('/__edit/api/')) return await api(req, res, url);
    if (url.pathname === '/__edit' || url.pathname === '/__edit/') {
      if (!hostOk(req)) return send(res, 403, 'forbidden');
      return send(res, 200, await readFile(path.join(here, 'editor.html')), { 'Content-Type': 'text/html; charset=utf-8' });
    }
    await serveStatic(req, res, url);
  } catch (e) {
    console.error(e);
    if (!res.headersSent) send(res, 500, { error: 'server error' });
  }
});
server.on('error', (e) => {
  console.error(e.code === 'EADDRINUSE' ? `Port ${PORT} ist schon belegt. Nimm einen anderen: node tools/editor.mjs --port 8003` : e);
  process.exit(1);
});
server.listen(PORT, '127.0.0.1', () => {
  console.log(`Team-Editor:  http://localhost:${PORT}/__edit/`);
  console.log(`Seite ansehen: http://localhost:${PORT}/team.html`);
  console.log('Beenden mit Strg+C. Nach dem Speichern: cf.cmd deploy');
});
