// Builds public/data/recipes.json (German) and public/data/i18n-recipes-en.json (English names) from the sheet export.
import { readFileSync, writeFileSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
const D = fileURLToPath(new URL('./work/', import.meta.url));
const OUT = fileURLToPath(new URL('../../public/data/', import.meta.url));
const LOC = 'F:/SteamLibrary/steamapps/common/Life is Feudal Your Own/data/loc/de/data/';
const unxml = (s) => s.replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&quot;/g, '"').replace(/&apos;/g, "'").replace(/&amp;/g, '&');
function loc(file) { const m = new Map(); for (const x of readFileSync(LOC + file, 'utf8').matchAll(/<string id="(\d+)">([\s\S]*?)<\/string>/g)) if (x[2].trim()) m.set(+x[1], unxml(x[2]).replace(/\s+/g, ' ').trim()); return m; }
function tsv(file) { const m = new Map(); for (const l of readFileSync(D + file, 'utf8').replace(/^\uFEFF/, '').split(/\r?\n/)) { const i = l.indexOf('\t'); if (i > 0 && /^\d+$/.test(l.slice(0, i))) m.set(+l.slice(0, i), l.slice(i + 1).trim()); } return m; }
const recipeDe = loc('recipe_Name.xml'), objDe = loc('objects_types_Name.xml'), skillDe = loc('skill_types.xml');
const recipeEn = tsv('db_recipe_names.tsv'), objEn = tsv('db_object_names.tsv'), skillEn = tsv('db_skill_names.tsv');
const list = JSON.parse(readFileSync(D + 'parsed.json', 'utf8'));
const clean = (s) => String(s || '').replace(/^[`'"]+|[`'"]+$/g, '').replace(/\s+/g, ' ').trim();

function line(q) {
  const n = String(q.note || '').replace(/^\/\*\s*|\s*\*\/$/g, '').replace(/\s+/g, ' ').trim();
  let m;
  const fixExtra = (s) => { s = s.trim(); if (/^regio notwenig$/i.test(s)) s = 'Regio notwendig'; return /^[\/\-\s]*$/.test(s) ? '' : s; };
  if ((m = n.match(/^(.+?)\s+H[a-z]*barkeit\s+(\d+)$/i))) return m[1] + ' (Haltbarkeit ' + m[2] + ')';     // also fixes "Halterbarkeit"
  if ((m = n.match(/^(\S+)\s+\1\s+(\d+)$/i))) return m[1] + ' (Haltbarkeit ' + m[2] + ')';                // "Windmühle Windmühle 10"
  if ((m = n.match(/^(.+?)\s+x\s*(\d+)\s*(.*)$/i))) { const ex = fixExtra(m[3]); return m[2] + ' × ' + m[1] + (ex ? ' (' + ex + ')' : ''); }
  if ((m = n.match(/^(.+?)\s+(\d+)\s*x$/i))) return m[2] + ' × ' + m[1];
  if ((m = n.match(/^(.+?)\s+(\d+)$/))) return m[2] + ' × ' + m[1];                                          // "Wolle 4"
  const byId = objDe.get(q.mat);
  if (byId && (/x\d+$/i.test(n) || !/\d/.test(n))) return q.qty + ' × ' + byId;                              // "...Helmx1", "Dickes Leder": use the game's own name and amount
  if (n) return n;
  return q.qty + ' × ' + (byId || objEn.get(q.mat) || ('Gegenstand ' + q.mat));
}

const out = []; const en = {}; const extras = new Map(); let noGerman = [];
for (const r of list) {
  let name = recipeDe.get(r.id);
  if (!name) { name = clean(r.name); noGerman.push(r.id + ': ' + name); }
  const skill = skillDe.get(r.skill) || ('Fertigkeit ' + r.skill);
  const resName = objDe.get(r.result) || clean(objEn.get(r.result)) || ('Gegenstand ' + r.result);
  const output = r.qty > 1 ? r.qty + ' × ' + resName : resName;
  const ingredients = r.reqs.map(line);
  const rec = { name, skill, output, ingredients };
  if (r.lvl > 0) rec.level = r.lvl;
  out.push(rec);
  // English names (by id, from your database)
  if (recipeEn.get(r.id) && recipeEn.get(r.id) !== name) en[name] = clean(recipeEn.get(r.id));
  if (objEn.get(r.result) && objDe.get(r.result)) en[resName] = clean(objEn.get(r.result));
  if (skillEn.get(r.skill) && skillDe.get(r.skill)) en[skill] = clean(skillEn.get(r.skill));
  for (const q of r.reqs) {
    if (q.mat != null && objDe.get(q.mat) && objEn.get(q.mat)) en[objDe.get(q.mat)] = clean(objEn.get(q.mat));
    // the sheet author's own wording of the item name (may differ from the game's German name) -> English name of the same item id
    const nm = line(q).match(/^\d+ × (.+?)(?: \(.+\))?$/) || line(q).match(/^(.+?) \(Haltbarkeit \d+\)$/);
    if (nm && q.mat != null && objEn.get(q.mat) && !(nm[1] in en)) en[nm[1]] = clean(objEn.get(q.mat));
    const mm = line(q).match(/^\d+ × .+? \((.+)\)$/); if (mm && !/^Haltbarkeit/.test(mm[1])) extras.set(mm[1], (extras.get(mm[1]) || 0) + 1);
  }
}
out.sort((a, b) => a.skill.localeCompare(b.skill, 'de') || a.name.localeCompare(b.name, 'de'));
// drop English entries that equal the German text
for (const k of Object.keys(en)) if (en[k] === k) delete en[k];
writeFileSync(OUT + 'recipes.json', JSON.stringify(out, null, 0).replace(/\},\{/g, '},\n{').replace(/^\[/, '[\n').replace(/\]$/, '\n]') + '\n', 'utf8');
writeFileSync(OUT + 'i18n-recipes-en.json', JSON.stringify({
  _help: 'English names for the recipe book (loaded only in English mode). German name -> English name (from the game database).',
  texts: Object.assign(en, { 'Regio notwendig': 'region required', 'Regio nötig': 'region required' }),
  patterns: [{ match: '^Fertigkeitsstufe: (\\d+)$', replace: 'Skill level: $1' }],
}, null, 1) + '\n', 'utf8');
console.log('recipes written:', out.length, '| skills:', new Set(out.map((x) => x.skill)).size, '| English entries:', Object.keys(en).length);
console.log('recipes without a German name in your game files (' + noGerman.length + '):', noGerman.slice(0, 12).join(' ; '));
console.log('extras in brackets (' + extras.size + ' kinds):', [...extras.entries()].sort((a, b) => b[1] - a[1]).slice(0, 15).map(([k, v]) => k + '×' + v).join(' ; '));
console.log('sample:', JSON.stringify(out[0]), '\n       ', JSON.stringify(out[400]));
const sizeKB = (readFileSync(OUT + 'recipes.json').length / 1024).toFixed(0), enKB = (readFileSync(OUT + 'i18n-recipes-en.json').length / 1024).toFixed(0);
console.log('file sizes: recipes.json', sizeKB, 'KB | i18n-recipes-en.json', enKB, 'KB');
