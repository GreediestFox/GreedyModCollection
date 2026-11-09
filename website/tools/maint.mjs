#!/usr/bin/env node
// Maintenance mode switch.
//   node tools/maint.mjs on ["message"] [--until "2026-10-15 18:00"] [--title "Title"]
//   node tools/maint.mjs off
//   node tools/maint.mjs status
//   node tools/maint.mjs message "new text"      (change text while it is on)
// Optional: --file <path> to point at another maintenance.json

import { readFile, writeFile } from 'node:fs/promises';
import { fileURLToPath } from 'node:url';
import path from 'node:path';

const here = path.dirname(fileURLToPath(import.meta.url));
const args = process.argv.slice(2);

function take(flag) {
  const i = args.indexOf(flag);
  if (i === -1) return null;
  const v = args[i + 1];
  args.splice(i, v === undefined ? 1 : 2);
  return v ?? null;
}
const fileArg = take('--file');
const untilArg = take('--until');
const titleArg = take('--title');
const file = fileArg ? path.resolve(fileArg) : path.join(here, '..', 'public', 'data', 'maintenance.json');
const cmd = (args.shift() || 'status').toLowerCase();
const text = args.join(' ').trim();

async function load() {
  try { return JSON.parse(await readFile(file, 'utf8')); }
  catch { return { enabled: false, title: 'Under maintenance', message: 'We are working on the site and will be back shortly.', until: null, updated: null }; }
}
async function save(cfg) {
  cfg.updated = new Date().toISOString();
  await writeFile(file, JSON.stringify(cfg, null, 2) + '\n', 'utf8');
}
function parseUntil(s) {
  if (!s) return null;
  const d = new Date(s.replace(' ', 'T'));
  if (isNaN(d)) { console.error(`Cannot read the time "${s}". Use e.g. "2026-10-15 18:00".`); process.exit(1); }
  return d.toISOString();
}
function show(cfg) {
  console.log(`Maintenance mode: ${cfg.enabled ? 'ON' : 'off'}`);
  if (cfg.enabled) {
    console.log(`  title:   ${cfg.title}`);
    console.log(`  message: ${cfg.message}`);
    if (cfg.until) console.log(`  until:   ${new Date(cfg.until).toLocaleString()}`);
  }
  if (cfg.updated) console.log(`  last changed: ${new Date(cfg.updated).toLocaleString()}`);
}

const cfg = await load();
switch (cmd) {
  case 'on':
    cfg.enabled = true;
    if (text) cfg.message = text;
    if (titleArg) cfg.title = titleArg;
    cfg.until = untilArg ? parseUntil(untilArg) : null;
    await save(cfg); show(cfg);
    console.log('Visitors get the maintenance screen within ~30 seconds. Add ?preview to your own URL to keep seeing the real site.');
    break;
  case 'off':
    cfg.enabled = false;
    await save(cfg); show(cfg);
    break;
  case 'message':
    if (!text) { console.error('Give the new message text.'); process.exit(1); }
    cfg.message = text;
    if (titleArg) cfg.title = titleArg;
    if (untilArg) cfg.until = parseUntil(untilArg);
    await save(cfg); show(cfg);
    break;
  case 'status':
    show(cfg);
    break;
  default:
    console.error('Commands: on [message] [--until "YYYY-MM-DD HH:MM"] [--title "..."] | off | status | message "text"');
    process.exit(1);
}
