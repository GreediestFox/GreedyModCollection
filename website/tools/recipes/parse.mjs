// Parses tab 0 of the sheet: repeating blocks of  [recipe header, recipe rows..., blank, requirement header, requirement rows...]
import { readFileSync, writeFileSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
const dir = fileURLToPath(new URL('./work/', import.meta.url));
function parseCsv(t) {
  const rows = []; let row = [], f = '', q = false;
  for (let i = 0; i < t.length; i++) {
    const c = t[i];
    if (q) { if (c === '"') { if (t[i + 1] === '"') { f += '"'; i++; } else q = false; } else f += c; }
    else if (c === '"') q = true;
    else if (c === ',') { row.push(f); f = ''; }
    else if (c === '\n') { row.push(f); rows.push(row); row = []; f = ''; }
    else if (c !== '\r') f += c;
  }
  if (f || row.length) { row.push(f); rows.push(row); }
  return rows;
}
const clean = (s) => (s || '').trim().replace(/^[`'"]+|[`'"]+$/g, '').trim();
const rows = parseCsv(readFileSync(dir + 'tab0.csv', 'utf8').replace(/^\uFEFF/, ''));
let mode = null; const recipes = new Map(); const reqs = []; const comments = [];
for (const r of rows) {
  const h = r.map(clean);
  if (h[0] === 'ID' && h[1] === 'Name') { mode = 'recipe'; continue; }
  if (h[0] === 'ID' && h[1] === 'RecipeID') { mode = 'req'; continue; }
  if (!/^\d+$/.test(h[0])) { if (h.slice(0, 12).every((x) => !x)) { /* blank */ } continue; }
  if (mode === 'recipe') {
    recipes.set(+h[0], { id: +h[0], name: h[1], desc: h[2], tool: +h[3], skill: +h[4], lvl: +h[5], result: +h[6], dep: +h[7], qty: +h[8], auto: +h[9], bp: +h[10], reqs: [] });
  } else if (mode === 'req') {
    const o = { id: +h[0], recipe: +h[1], mat: +h[2], quality: +h[3], infl: +h[4], qty: +h[5], region: +h[6], note: (r[8] || '').trim() };
    reqs.push(o); if (o.note) comments.push(o.note);
  }
}
let orphan = 0; for (const q of reqs) { const rc = recipes.get(q.recipe); if (rc) rc.reqs.push(q); else orphan++; }
const list = [...recipes.values()];
console.log('recipes:', list.length, '| requirement rows:', reqs.length, '| requirements without a recipe in this tab:', orphan);
console.log('recipes without ingredients:', list.filter((x) => !x.reqs.length).length);
const dup = list.length - new Set(list.map((x) => x.id)).size; console.log('duplicate recipe ids overwritten:', dup);
console.log('distinct skills:', [...new Set(list.map((x) => x.skill))].sort((a, b) => a - b).join(','));
console.log('distinct material ids:', new Set(reqs.map((x) => x.mat)).size, '| distinct result ids:', new Set(list.map((x) => x.result)).size);
console.log('blueprint recipes:', list.filter((x) => x.bp).length, '| names with stray quote:', list.filter((x) => /['`"]/.test(x.name)).length);
console.log('sample recipes:'); list.slice(0, 3).concat(list.slice(-3)).forEach((x) => console.log(' ', JSON.stringify(x)));
console.log('sample comments:', comments.slice(0, 12).join(' || '));
writeFileSync(dir + 'parsed.json', JSON.stringify(list));
