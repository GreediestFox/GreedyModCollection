import bcrypt from "bcryptjs";

// Skriptorium (Schriftstuecke) as a Cloudflare Worker: static pages from ./public, API on D1.
// Port of the Express + SQLite version; the API (paths, request and response shapes) is unchanged.

const MAX_BODY = 2 * 1024 * 1024;
const TICKET_RE = /^[A-Za-z0-9]{8,24}$/;
const MAX_PAGES = 200;
const MAX_BONUS_PER_REQUEST = 10;

// ---------------------------------------------------------------- helpers

const json = (data, status = 200) =>
  new Response(JSON.stringify(data), { status, headers: { "content-type": "application/json; charset=utf-8" } });
const err = (status, error) => json({ error }, status);
const text = (body, status = 200) =>
  new Response(body, { status, headers: { "content-type": "text/plain; charset=utf-8" } });

async function readBody(request) {
  const len = Number(request.headers.get("content-length") || 0);
  if (len > MAX_BODY) return null;
  const type = request.headers.get("content-type") || "";
  try {
    if (type.includes("application/json")) {
      const raw = await request.text();
      if (raw.length > MAX_BODY) return null;
      const v = JSON.parse(raw || "{}");
      return v && typeof v === "object" ? v : {};
    }
    if (type.includes("application/x-www-form-urlencoded")) {
      return Object.fromEntries(new URLSearchParams(await request.text()));
    }
  } catch (e) {
    return {};
  }
  return {};
}

const PREFIX = { letter: "_ltr", book: "_bok", tablet: "_tbl" };
const ID_ALPHABET = "23456789abcdefghjkmnpqrstuvwxyz"; // no look-alike characters

function newId(type) {
  let out = "";
  while (out.length < 10) {
    const bytes = crypto.getRandomValues(new Uint8Array(32));
    for (const b of bytes) {
      if (b < 248 && out.length < 10) out += ID_ALPHABET[b % 31]; // 248 = 31 * 8, no modulo bias
    }
  }
  return PREFIX[type] + out;
}

function typeFromId(id) {
  for (const [type, prefix] of Object.entries(PREFIX)) if (id.startsWith(prefix)) return type;
  return null;
}

const str = (v, max) => (v === undefined || v === null ? "" : String(v)).slice(0, max);
const ticketOf = (body) => (TICKET_RE.test(body.ticket || "") ? body.ticket : null);

// ---------------------------------------------------------------- passwords (PBKDF2, native WebCrypto: bcrypt is too slow for Worker CPU limits)

const PBKDF2_ITER = 100000;
const b64 = (buf) => btoa(String.fromCharCode(...new Uint8Array(buf)));
const unb64 = (s) => Uint8Array.from(atob(s), (c) => c.charCodeAt(0));

async function derive(password, salt, iter) {
  const key = await crypto.subtle.importKey("raw", new TextEncoder().encode(password), "PBKDF2", false, ["deriveBits"]);
  return crypto.subtle.deriveBits({ name: "PBKDF2", salt, iterations: iter, hash: "SHA-256" }, key, 256);
}

async function hashPassword(password) {
  const salt = crypto.getRandomValues(new Uint8Array(16));
  return `pbkdf2$${PBKDF2_ITER}$${b64(salt)}$${b64(await derive(password, salt, PBKDF2_ITER))}`;
}

async function verifyPassword(password, stored) {
  // legacy hashes from the old Express/SQLite site (bcrypt)
  if ((stored || "").startsWith("$2")) return bcrypt.compareSync(password, stored);
  const p = (stored || "").split("$");
  if (p.length !== 4 || p[0] !== "pbkdf2") return false;
  const got = new Uint8Array(await derive(password, unb64(p[2]), Number(p[1])));
  const want = unb64(p[3]);
  if (got.length !== want.length) return false;
  let diff = 0;
  for (let i = 0; i < got.length; i++) diff |= got[i] ^ want[i];
  return diff === 0;
}

// ---------------------------------------------------------------- HTML sanitizer (HTMLRewriter whitelist, replaces sanitize-html)

const SIMPLE_TAGS = new Set(["b", "strong", "i", "em", "u", "br", "p", "ul", "ol", "li"]);
const BOOK_TAGS = new Set([...SIMPLE_TAGS, "span", "img"]);
const DROP_WITH_CONTENT = new Set(["script", "style", "textarea", "option", "noscript", "template", "iframe", "object", "embed", "title", "head"]);
const SPAN_STYLES = {
  color: [/^#[0-9a-fA-F]{3,6}$/, /^rgb\(/],
  "font-size": [/^\d+(\.\d+)?(px|pt|em)$/]
};
const IMG_STYLES = { width: [/^\d+(\.\d+)?(px|%)$/] };

function cleanStyle(value, allowed) {
  const out = [];
  for (const decl of (value || "").split(";")) {
    const i = decl.indexOf(":");
    if (i < 0) continue;
    const prop = decl.slice(0, i).trim().toLowerCase();
    const val = decl.slice(i + 1).trim();
    if (allowed[prop] && allowed[prop].some((re) => re.test(val))) out.push(`${prop}:${val}`);
  }
  return out.join(";");
}

// Whitelist sanitizer: unknown tags are unwrapped, script-like tags dropped with content, attributes limited to a safe set
async function clean(html, mode) {
  if (!html || typeof html !== "string") return "";
  const allowedTags = mode === "book" ? BOOK_TAGS : SIMPLE_TAGS;
  const rewriter = new HTMLRewriter()
    .on("*", {
      element(el) {
        let tag = el.tagName.toLowerCase();
        if (DROP_WITH_CONTENT.has(tag)) return el.remove();
        if (tag === "div") {
          el.tagName = "p";
          tag = "p";
        }
        if (!allowedTags.has(tag)) return el.removeAndKeepContent();
        for (const [name] of [...el.attributes]) {
          const lower = name.toLowerCase();
          const value = el.getAttribute(name) || "";
          let keep = null;
          if (tag === "span" && lower === "style") keep = cleanStyle(value, SPAN_STYLES) || null;
          else if (tag === "img" && lower === "style") keep = cleanStyle(value, IMG_STYLES) || null;
          else if (tag === "img" && lower === "alt") keep = value;
          else if (tag === "img" && lower === "src") keep = /^https?:\/\//i.test(value.trim()) ? value.trim() : null;
          if (keep === null) el.removeAttribute(name);
          else el.setAttribute(name, keep);
        }
      },
      comments(c) {
        c.remove();
      }
    })
    .onDocument({
      comments(c) {
        c.remove();
      }
    });
  return rewriter.transform(new Response(html, { headers: { "content-type": "text/html; charset=utf-8" } })).text();
}
const sanitizeSimple = (html) => clean(html, "simple");
const sanitizeBook = (html) => clean(html, "book");

// ---------------------------------------------------------------- stone tablet cipher

const ALPHABET = "abcdefghijklmnopqrstuvwxyz".split("");
const RUNES = [];
for (let cp = 0x16a0; cp <= 0x16f0 && RUNES.length < 26; cp++) RUNES.push(String.fromCodePoint(cp));

function shuffle(arr) {
  const a = arr.slice();
  for (let i = a.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [a[i], a[j]] = [a[j], a[i]];
  }
  return a;
}

function buildCipherMap() {
  const runes = shuffle(RUNES);
  const map = {};
  ALPHABET.forEach((letter, i) => (map[letter] = runes[i]));
  return map;
}

function encryptText(plain, map) {
  return plain
    .split("")
    .map((ch) => map[ch.toLowerCase()] || ch)
    .join("");
}

function symbolCounts(encrypted, map) {
  const used = new Set(Object.values(map));
  const counts = {};
  for (const ch of encrypted) if (used.has(ch)) counts[ch] = (counts[ch] || 0) + 1;
  return Object.entries(counts)
    .map(([symbol, count]) => ({ symbol, count }))
    .sort((a, b) => b.count - a.count);
}

function decodeWithMap(encrypted, map) {
  const reverse = {};
  Object.entries(map).forEach(([letter, symbol]) => (reverse[symbol] = letter));
  return encrypted
    .split("")
    .map((ch) => reverse[ch] || ch)
    .join("");
}

// ---------------------------------------------------------------- letters

async function letters(request, env, parts, body) {
  const db = env.DB;
  const m = request.method;
  const load = (id) => db.prepare("SELECT * FROM documents WHERE id = ? AND type = 'letter'").bind(id).first();
  const blocks = async (id) =>
    (await db.prepare("SELECT seq, html, created_at FROM blocks WHERE document_id = ? ORDER BY seq ASC").bind(id).all()).results;
  const nextSeq = async (id) => {
    const r = await db.prepare("SELECT MAX(seq) AS maxSeq FROM blocks WHERE document_id = ?").bind(id).first();
    return (r && r.maxSeq !== null && r.maxSeq !== undefined ? r.maxSeq : -1) + 1;
  };

  if (parts.length === 0 && m === "POST") {
    const id = newId("letter");
    await db.prepare("INSERT INTO documents (id, type, title, ticket) VALUES (?, 'letter', ?, ?)").bind(id, str(body.title, 200), ticketOf(body)).run();
    return json({ id });
  }

  const id = parts[0];
  const doc = id ? await load(id) : null;
  if (!doc) return err(404, "not_found");
  const action = parts[1];

  if (!action && m === "GET") {
    const base = { id: doc.id, title: doc.title, finalized: !!doc.finalized, sealed: !!doc.sealed };
    if (doc.sealed) return json({ ...base, blocks: null });
    return json({ ...base, blocks: await blocks(doc.id) });
  }

  if (action === "draft" && m === "PUT") {
    if (doc.finalized) return err(409, "already_finalized");
    const html = await sanitizeSimple(body.html);
    const existing = await db.prepare("SELECT id FROM blocks WHERE document_id = ? AND seq = 0").bind(doc.id).first();
    const stmts = [];
    if (existing) stmts.push(db.prepare("UPDATE blocks SET html = ? WHERE id = ?").bind(html, existing.id));
    else stmts.push(db.prepare("INSERT INTO blocks (document_id, seq, html) VALUES (?, 0, ?)").bind(doc.id, html));
    if (body.title !== undefined) stmts.push(db.prepare("UPDATE documents SET title = ? WHERE id = ?").bind(str(body.title, 200), doc.id));
    await db.batch(stmts);
    return json({ ok: true });
  }

  if (action === "finalize" && m === "POST") {
    if (doc.finalized) return err(409, "already_finalized");
    if (!doc.title || !doc.title.trim()) return err(400, "title_required");
    await db.prepare("UPDATE documents SET finalized = 1, sealed = ?, updated_at = datetime('now') WHERE id = ?").bind(body.sealed ? 1 : 0, doc.id).run();
    return json({ ok: true });
  }

  if (action === "append" && m === "POST") {
    if (!doc.finalized) return err(409, "not_finalized_yet");
    if (doc.sealed) return err(409, "sealed");
    const html = await sanitizeSimple(body.html);
    if (!html || !html.trim()) return err(400, "empty");
    const seq = await nextSeq(doc.id);
    await db.batch([
      db.prepare("INSERT INTO blocks (document_id, seq, html) VALUES (?, ?, ?)").bind(doc.id, seq, html),
      db.prepare("UPDATE documents SET updated_at = datetime('now') WHERE id = ?").bind(doc.id)
    ]);
    return json({ ok: true, seq });
  }

  if (action === "break-seal" && m === "POST") {
    if (!doc.sealed) return err(409, "not_sealed");
    const seq = await nextSeq(doc.id);
    await db.batch([
      db.prepare("UPDATE documents SET sealed = 0, updated_at = datetime('now') WHERE id = ?").bind(doc.id),
      db.prepare("INSERT INTO blocks (document_id, seq, html) VALUES (?, ?, ?)").bind(doc.id, seq, "<p><em>[Siegel gebrochen]</em></p>")
    ]);
    return json({ ok: true, blocks: await blocks(doc.id) });
  }

  if (action === "seal" && m === "POST") {
    if (!doc.finalized) return err(409, "not_finalized_yet");
    if (doc.sealed) return json({ ok: true, alreadySealed: true });
    await db.prepare("UPDATE documents SET sealed = 1, updated_at = datetime('now') WHERE id = ?").bind(doc.id).run();
    return json({ ok: true });
  }

  return err(404, "not_found");
}

// ---------------------------------------------------------------- books

async function books(request, env, parts, body) {
  const db = env.DB;
  const m = request.method;
  const load = (id) => db.prepare("SELECT * FROM documents WHERE id = ? AND type = 'book'").bind(id).first();
  const loadPages = async (id) => {
    const rows = (await db.prepare("SELECT html FROM blocks WHERE document_id = ? ORDER BY seq ASC").bind(id).all()).results;
    return rows.length ? rows.map((r) => r.html) : [""];
  };
  const sanitizePages = async (raw) => {
    const arr = (Array.isArray(raw) ? raw : []).slice(0, MAX_PAGES);
    const out = await Promise.all(arr.map((h) => sanitizeBook(typeof h === "string" ? h : "")));
    return out.length ? out : [""];
  };
  const hex = (v) => (/^#[0-9a-fA-F]{6}$/.test((v || "").toString().trim()) ? v.toString().trim() : null);
  const replacePages = (id, pages) => [
    db.prepare("DELETE FROM blocks WHERE document_id = ?").bind(id),
    ...pages.map((html, i) => db.prepare("INSERT INTO blocks (document_id, seq, html) VALUES (?, ?, ?)").bind(id, i, html))
  ];
  const coverStmt = (id) =>
    db
      .prepare("UPDATE documents SET cover_author = ?, cover_color = ?, cover_text_color = ? WHERE id = ?")
      .bind(((body.author || "").toString().trim().slice(0, 100)) || null, hex(body.coverColor), hex(body.coverTextColor), id);

  if (parts.length === 0 && m === "POST") {
    const id = newId("book");
    await db.batch([
      db.prepare("INSERT INTO documents (id, type, title, ticket) VALUES (?, 'book', ?, ?)").bind(id, str(body.title, 200), ticketOf(body)),
      db.prepare("INSERT INTO blocks (document_id, seq, html) VALUES (?, 0, '')").bind(id)
    ]);
    return json({ id });
  }

  const id = parts[0];
  const doc = id ? await load(id) : null;
  if (!doc) return err(404, "not_found");
  const action = parts[1];

  if (!action && m === "GET") {
    return json({
      id: doc.id,
      title: doc.title,
      finalized: !!doc.finalized,
      author: doc.cover_author || "",
      coverColor: doc.cover_color || "",
      coverTextColor: doc.cover_text_color || "",
      pages: await loadPages(doc.id)
    });
  }

  if (action === "draft" && m === "PUT") {
    if (doc.finalized) return err(409, "already_finalized");
    const stmts = replacePages(doc.id, await sanitizePages(body.pages));
    if (body.title !== undefined) stmts.push(db.prepare("UPDATE documents SET title = ? WHERE id = ?").bind(str(body.title, 200), doc.id));
    stmts.push(coverStmt(doc.id));
    await db.batch(stmts);
    return json({ ok: true });
  }

  if (action === "finalize" && m === "POST") {
    if (doc.finalized) return err(409, "already_finalized");
    if (!doc.title || !doc.title.trim()) return err(400, "title_required");
    const password = (body.password || "").toString();
    if (password.length < 4) return err(400, "password_too_short");
    const hash = await hashPassword(password);
    await db.prepare("UPDATE documents SET finalized = 1, password_hash = ?, updated_at = datetime('now') WHERE id = ?").bind(hash, doc.id).run();
    return json({ ok: true });
  }

  if (!action && m === "PUT") {
    if (!doc.finalized) return err(409, "not_finalized_yet");
    if (!(await verifyPassword((body.password || "").toString(), doc.password_hash))) return err(403, "wrong_password");
    const stmts = replacePages(doc.id, await sanitizePages(body.pages));
    if (body.title !== undefined) stmts.push(db.prepare("UPDATE documents SET title = ? WHERE id = ?").bind(str(body.title, 200), doc.id));
    stmts.push(coverStmt(doc.id));
    stmts.push(db.prepare("UPDATE documents SET updated_at = datetime('now') WHERE id = ?").bind(doc.id));
    await db.batch(stmts);
    return json({ ok: true });
  }

  return err(404, "not_found");
}

// ---------------------------------------------------------------- stone tablets

async function tablets(request, env, parts, body, url) {
  const db = env.DB;
  const m = request.method;
  const load = (id) => db.prepare("SELECT * FROM documents WHERE id = ? AND type = 'tablet'").bind(id).first();

  if (parts.length === 0 && m === "POST") {
    const key = (body.key || url.searchParams.get("key") || "").toString();
    if (!env.TABLET_ADMIN_KEY || key !== env.TABLET_ADMIN_KEY) return err(403, "forbidden");
    const title = str(body.title, 200);
    if (!title.trim()) return err(400, "title_required");
    const plain = str(body.plainText, 20000);
    if (!plain.trim()) return err(400, "empty_text");
    const id = newId("tablet");
    const map = buildCipherMap();
    await db
      .prepare("INSERT INTO documents (id, type, title, finalized, cipher_map, encrypted_text, ticket) VALUES (?, 'tablet', ?, 1, ?, ?, ?)")
      .bind(id, title, JSON.stringify(map), encryptText(plain, map), ticketOf(body))
      .run();
    return json({ id, url: `/tablet.html?id=${id}` });
  }

  const id = parts[0];
  const doc = id ? await load(id) : null;
  if (!doc) return err(404, "not_found");
  const action = parts[1];
  const map = JSON.parse(doc.cipher_map);

  if (!action && m === "GET") {
    return json({ id: doc.id, title: doc.title, encryptedText: doc.encrypted_text, symbols: symbolCounts(doc.encrypted_text, map) });
  }

  if (action === "guess" && m === "POST") {
    const symbol = (body.symbol || "").toString();
    const letter = (body.letter || "").toString().toLowerCase();
    const correct = map[letter] === symbol;
    return json({ correct, symbol, letter: correct ? letter : undefined });
  }

  if (action === "bonus-reveal" && m === "POST") {
    const exclude = new Set(Array.isArray(body.excludeSymbols) ? body.excludeSymbols.map(String) : []);
    const used = new Set(symbolCounts(doc.encrypted_text, map).map((s) => s.symbol));
    let count = parseInt(body.count, 10);
    if (!Number.isFinite(count) || count < 0) count = 0;
    count = Math.min(count, MAX_BONUS_PER_REQUEST);
    const remaining = shuffle(Object.entries(map).filter(([, symbol]) => used.has(symbol) && !exclude.has(symbol)));
    return json({ pairs: remaining.slice(0, count).map(([letter, symbol]) => ({ symbol, letter })) });
  }

  if (action === "reveal" && m === "POST") {
    const guesses = body.mapping || {};
    const all = Object.entries(map).every(([letter, symbol]) => guesses[symbol] === letter);
    if (!all) return err(403, "incomplete_or_wrong");
    return json({ plainText: decodeWithMap(doc.encrypted_text, map) });
  }

  return err(404, "not_found");
}

// ---------------------------------------------------------------- router

export default {
  async fetch(request, env) {
    const url = new URL(request.url);
    const path = url.pathname;
    const parts = path.replace(/^\/+|\/+$/g, "").split("/").map(decodeURIComponent);

    if (parts[0] === "api") {
      const resource = parts[1];
      const rest = parts.slice(2);

      if (request.method === "GET" && resource === "title" && rest.length === 1) {
        const tag = rest[0];
        if (!typeFromId(tag)) return text("", 404);
        const doc = await env.DB.prepare("SELECT title FROM documents WHERE id = ?").bind(tag).first();
        if (!doc) return text("", 404);
        return text((doc.title || "").split("\n")[0].slice(0, 100));
      }

      if (request.method === "GET" && resource === "ticket" && rest.length === 1) {
        const t = rest[0];
        if (!TICKET_RE.test(t)) return text("", 404);
        const doc = await env.DB
          .prepare("SELECT id, title FROM documents WHERE ticket = ? AND finalized = 1 ORDER BY created_at DESC LIMIT 1")
          .bind(t)
          .first();
        if (!doc) return text("", 404);
        return text(doc.id + "\n" + (doc.title || "").split("\n")[0].slice(0, 100));
      }

      const handler = { letters, books, tablets }[resource];
      if (!handler) return err(404, "not_found");
      const body = request.method === "GET" ? {} : await readBody(request);
      if (body === null) return err(413, "too_large");
      try {
        return await handler(request, env, rest, body, url);
      } catch (e) {
        console.error(e && e.stack ? e.stack : String(e));
        return err(500, "server_error");
      }
    }

    // Stone tablet creation page: only with the secret key, not linked anywhere
    if (path === "/admin/tablet") {
      const key = url.searchParams.get("key") || "";
      if (!env.TABLET_ADMIN_KEY || key !== env.TABLET_ADMIN_KEY) return text("Zugriff verweigert / Access denied.", 403);
      return env.ASSETS.fetch(new Request(new URL("/admin-tablet.html", url), request));
    }

    // Open a document by its tag
    if (path === "/open") {
      const tag = (url.searchParams.get("tag") || "").trim();
      const type = typeFromId(tag);
      const lang = url.searchParams.get("lang") ? `&lang=${encodeURIComponent(url.searchParams.get("lang"))}` : "";
      const page = { letter: "letter", book: "book", tablet: "tablet" }[type];
      if (!page) return text("Unbekannter Tag / unknown tag.", 404);
      return Response.redirect(`${url.origin}/${page}.html?id=${tag}${lang}`, 302);
    }

    return env.ASSETS.fetch(request);
  }
};
