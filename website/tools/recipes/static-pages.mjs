// Generates crawlable STATIC recipe pages for search engines from public/data/recipes.json:
//   /rezepte/                       overview (all skills)
//   /rezepte/<skill>/               all recipes of one skill (plain HTML lists)
//   /rezepte/<skill>/<recipe>/      one page per recipe
// plus the matching entries in sitemap.xml and the English texts in data/i18n-en.json.
// Run after build.mjs:  node tools\recipes\static-pages.mjs      (then deploy)
import { readFileSync, writeFileSync, mkdirSync, rmSync, existsSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
const P = fileURLToPath(new URL('../../public/', import.meta.url));
const SITE = 'https://wulpischelande.de';
const recipes = JSON.parse(readFileSync(P + 'data/recipes.json', 'utf8'));

const esc = (s) => String(s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
const slugify = (s) => s.toLowerCase().replace(/ä/g, 'ae').replace(/ö/g, 'oe').replace(/ü/g, 'ue').replace(/ß/g, 'ss').replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '') || 'x';

// ---- slugs (unique)
const skills = new Map();       // skill -> { slug, recipes: [] }
const usedSkill = new Set();
for (const r of recipes) {
  if (!skills.has(r.skill)) { let s = slugify(r.skill), n = 2; while (usedSkill.has(s)) s = slugify(r.skill) + '-' + n++; usedSkill.add(s); skills.set(r.skill, { slug: s, recipes: [], used: new Set() }); }
  const sk = skills.get(r.skill);
  let s = slugify(r.name), n = 2; while (sk.used.has(s)) s = slugify(r.name) + '-' + n++; sk.used.add(s);
  r._slug = s; r._skillSlug = sk.slug; sk.recipes.push(r);
}

const NAV = `<header class="nav solid" id="nav">
  <a class="nav-brand" href="/">Die wulpischen Lande</a>
  <button class="nav-toggle" id="navToggle" aria-label="Menü" aria-expanded="false"><span></span><span></span><span></span></button>
  <nav id="navLinks">
    <a href="/#world">Insel</a>
    <a href="/#gallery">Galerie</a>
    <a href="/#recipes" aria-current="page">Rezepte</a>
    <a href="/#join">Beitreten</a>
    <a href="#" class="nav-btn" data-link="wiki" target="_blank" rel="noopener" hidden>Wiki</a>
    <a href="/team" class="nav-btn">Team</a>
    <a href="/thanks" class="nav-btn">Dank</a>
    <a href="https://rezepte.wulpischelande.de" class="nav-cta" data-link="rezepte" target="_blank" rel="noopener">Rezepte</a>
  </nav>
</header>`;
const FOOT = `<footer class="footer">
  <p>Die wulpischen Lande ist ein unabhängiges Community-Projekt für Life is Feudal: Your Own. Es steht in keiner Verbindung zu den Entwicklern, Publishern oder Rechteinhabern des Spiels und wird von diesen weder unterstützt noch gesponsert. Zum Spielen ist eine rechtmäßig erworbene Kopie des Spiels erforderlich. Alle Marken und spielbezogenen Rechte gehören ihren jeweiligen Inhabern.</p>
  <p><a href="/impressum">Impressum</a> · <a href="/datenschutz">Datenschutz</a> · <a href="/urheberrecht">Urheberrecht</a> · <a href="https://discord.gg/E6PrsCYV4x" data-link="discord" target="_blank" rel="noopener">Discord</a></p>
</footer>`;

function page({ path, title, desc, h1, body, crumbs }) {
  const url = SITE + path;
  const ld = { '@context': 'https://schema.org', '@type': 'BreadcrumbList', itemListElement: crumbs.map((c, i) => ({ '@type': 'ListItem', position: i + 1, name: c[0], item: SITE + c[1] })) };
  return `<!doctype html>
<html lang="de">
<head>
<meta charset="utf-8">
<base href="/">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>${esc(title)}</title>
<meta name="description" content="${esc(desc)}">
<meta name="theme-color" content="#0a0506">
<link rel="canonical" href="${url}">
<link rel="icon" href="/favicon.ico" sizes="any">
<link rel="icon" type="image/png" href="/favicon-192.png" sizes="192x192">
<link rel="apple-touch-icon" href="/apple-touch-icon.png">
<meta property="og:type" content="website">
<meta property="og:site_name" content="Die wulpischen Lande">
<meta property="og:locale" content="de_DE">
<meta property="og:title" content="${esc(title)}">
<meta property="og:description" content="${esc(desc)}">
<meta property="og:url" content="${url}">
<meta property="og:image" content="${SITE}/assets/img/og-image.jpg">
<meta property="og:image:width" content="1200"><meta property="og:image:height" content="630">
<meta name="twitter:card" content="summary_large_image">
<script type="application/ld+json">${JSON.stringify(ld)}</script>
<link rel="stylesheet" href="css/fonts.css">
<link rel="stylesheet" href="css/style.css">
<link rel="stylesheet" href="css/pages.css">
</head>
<body>

${NAV}

<main class="band band-dark legal-page">
  <div class="wrap" id="recipeList">
${body}
  </div>
</main>

${FOOT}

<script src="js/pages.js"></script>
<script src="js/i18n.js"></script>
<script src="js/maintenance.js"></script>
</body>
</html>
`;
}
const ingList = (r) => '<ul>' + r.ingredients.map((i) => '<li>' + esc(i) + '</li>').join('') + '</ul>';
const write = (path, html) => { const d = P + path.replace(/^\//, ''); mkdirSync(d, { recursive: true }); writeFileSync(d + '/index.html', html, 'utf8'); };

if (existsSync(P + 'rezepte')) rmSync(P + 'rezepte', { recursive: true, force: true });
const urls = [];
// ---- overview
{
  const list = [...skills.entries()].sort((a, b) => a[0].localeCompare(b[0], 'de'));
  const body = `    <h1>Rezeptbuch: alle Rezepte</h1>
    <p class="stand">Life is Feudal: Your Own · Die wulpischen Lande</p>
    <p>Hier findest du alle ${recipes.length} Rezepte unseres Life-is-Feudal-Your-Own-Servers auf der Insel Myralon, geordnet nach Fertigkeit. Zu jedem Rezept stehen Ergebnis, Fertigkeitsstufe, Werkzeug und alle Zutaten.</p>
    <p>Die Rezepte gelten für unseren Server mit dem GMC-Modpack. Eine Suche mit Filter findest du im <a href="/#recipes">interaktiven Rezeptbuch</a>.</p>
    <h3>Fertigkeiten</h3>
    <ul>${list.map(([name, s]) => `<li><a href="rezepte/${s.slug}/">${esc(name)}</a> (${s.recipes.length} Rezepte)</li>`).join('')}</ul>`;
  write('/rezepte', page({ path: '/rezepte/', title: 'Rezeptbuch: alle Rezepte – Life is Feudal: Your Own | Die wulpischen Lande', desc: `Alle ${recipes.length} Rezepte des Life-is-Feudal-Your-Own-Servers „Die wulpischen Lande“ nach Fertigkeit: Zutaten, Werkzeug und Fertigkeitsstufe.`, h1: 'Rezeptbuch', body, crumbs: [['Die wulpischen Lande', '/'], ['Rezeptbuch', '/rezepte/']] }));
  urls.push('/rezepte/');
}
// ---- skill pages + recipe pages
for (const [skill, s] of skills) {
  const sorted = [...s.recipes].sort((a, b) => (a.level || 0) - (b.level || 0) || a.name.localeCompare(b.name, 'de'));
  const body = `    <h1>Rezepte: <span>${esc(skill)}</span></h1>
    <p class="stand"><span>${s.recipes.length}</span> Rezepte · Life is Feudal: Your Own · Die wulpischen Lande</p>
    <p>Alle Rezepte der Fertigkeit <span>${esc(skill)}</span> auf unserem Server, sortiert nach Fertigkeitsstufe. <a href="rezepte/">Zurück zu allen Fertigkeiten</a> · <a href="/#recipes">interaktives Rezeptbuch</a></p>
${sorted.map((r) => `    <h3><a href="rezepte/${s.slug}/${r._slug}/">${esc(r.name)}</a></h3>
    <p>Ergebnis: ${esc(r.output)}</p>${r.level ? `\n    <p>Fertigkeitsstufe: ${r.level}</p>` : ''}
    ${ingList(r)}`).join('\n')}`;
  write('/rezepte/' + s.slug, page({ path: `/rezepte/${s.slug}/`, title: `Rezepte ${skill} – Life is Feudal: Your Own | Die wulpischen Lande`, desc: `Alle ${s.recipes.length} Rezepte der Fertigkeit ${skill} in Life is Feudal: Your Own (Die wulpischen Lande): Zutaten, Werkzeug und Stufe.`, h1: skill, body, crumbs: [['Die wulpischen Lande', '/'], ['Rezeptbuch', '/rezepte/'], [skill, `/rezepte/${s.slug}/`]] }));
  urls.push(`/rezepte/${s.slug}/`);
  for (const r of s.recipes) {
    const ing = r.ingredients.map((i) => i.replace(/ \(Haltbarkeit \d+\)$/, '')).slice(0, 4).join(', ');
    const body2 = `    <h1>${esc(r.name)}</h1>
    <p class="stand">Rezept · Fertigkeit <span>${esc(skill)}</span>${r.level ? ' · Stufe <span>' + r.level + '</span>' : ''}</p>
    <h3>Ergebnis</h3>
    <p>${esc(r.output)}</p>
    <h3>Zutaten und Werkzeug</h3>
    ${ingList(r)}
    <p>${esc(r.name)} ist ein Rezept aus Life is Feudal: Your Own auf dem Community-Server „Die wulpischen Lande“ (Insel Myralon). Die Angaben gelten für unseren Server mit dem GMC-Modpack.</p>
    <p><a href="rezepte/${s.slug}/">Alle Rezepte: ${esc(skill)}</a> · <a href="rezepte/">Rezeptbuch</a> · <a href="/#recipes">interaktives Rezeptbuch mit Suche</a></p>`;
    write(`/rezepte/${s.slug}/${r._slug}`, page({ path: `/rezepte/${s.slug}/${r._slug}/`, title: `${r.name} – Rezept (${skill}) | Life is Feudal: Your Own | Die wulpischen Lande`,
      desc: `Rezept ${r.name} in Life is Feudal: Your Own: Fertigkeit ${skill}${r.level ? ', Stufe ' + r.level : ''}. Zutaten: ${ing}.`.slice(0, 300), h1: r.name, body: body2, crumbs: [['Die wulpischen Lande', '/'], ['Rezeptbuch', '/rezepte/'], [skill, `/rezepte/${s.slug}/`], [r.name, `/rezepte/${s.slug}/${r._slug}/`]] }));
    urls.push(`/rezepte/${s.slug}/${r._slug}/`);
  }
}
// ---- sitemap: keep the normal pages, replace the /rezepte entries
const sm = P + 'sitemap.xml';
const base = [...readFileSync(sm, 'utf8').matchAll(/<loc>([^<]+)<\/loc>/g)].map((m) => m[1].replace(SITE, '')).filter((u) => !u.startsWith('/rezepte'));
const today = new Date().toISOString().slice(0, 10);
writeFileSync(sm, '<?xml version="1.0" encoding="UTF-8"?>\n<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">\n' + [...base, ...urls].map((u) => `  <url><loc>${SITE}${u}</loc><lastmod>${today}</lastmod></url>`).join('\n') + '\n</urlset>\n', 'utf8');
// ---- English texts of the page frame (names/ingredients come from i18n-recipes-en.json, loaded because the container has id="recipeList")
const dp = P + 'data/i18n-en.json'; const d = JSON.parse(readFileSync(dp, 'utf8'));
Object.assign(d.texts, {
  'Rezeptbuch: alle Rezepte': 'Recipe book: all recipes',
  'Fertigkeiten': 'Skills',
  'Ergebnis': 'Result',
  'Zutaten und Werkzeug': 'Ingredients and tool',
  'Zurück zu allen Fertigkeiten': 'Back to all skills',
  'interaktives Rezeptbuch': 'interactive recipe book',
  'interaktives Rezeptbuch mit Suche': 'interactive recipe book with search',
  'Rezeptbuch': 'Recipe book',
  'Die Rezepte gelten für unseren Server mit dem GMC-Modpack. Eine Suche mit Filter findest du im': 'The recipes apply to our server with the GMC modpack. You can find a search with filters in the',
  'Life is Feudal: Your Own · Die wulpischen Lande': 'Life is Feudal: Your Own · Die wulpischen Lande',
  'Rezept · Fertigkeit': 'Recipe · Skill',
  '· Stufe': '· Level',
  'Rezepte:': 'Recipes:',
  'Alle Rezepte der Fertigkeit': 'All recipes of the skill',
  'auf unserem Server, sortiert nach Fertigkeitsstufe.': 'on our server, sorted by skill level.',
  'Rezepte · Life is Feudal: Your Own · Die wulpischen Lande': 'recipes · Life is Feudal: Your Own · Die wulpischen Lande',
});
d.patterns = d.patterns || [];
const addPat = (match, replace) => { if (!d.patterns.some((p) => p.match === match)) d.patterns.push({ match, replace }); };
addPat('^(\\d+) Rezepte · Life is Feudal: Your Own · Die wulpischen Lande$', '$1 recipes · Life is Feudal: Your Own · Die wulpischen Lande');
addPat('^Rezepte: (.+)$', 'Recipes: $1');
addPat('^Rezept · Fertigkeit (.+?)(?: · Stufe (\\d+))?$', 'Recipe · Skill $1 · Level $2');
addPat('^Ergebnis: (.+?)(?: · Fertigkeitsstufe: (\\d+))?$', 'Makes: $1 · Skill level: $2');
addPat('^Alle Rezepte: (.+)$', 'All recipes: $1');
addPat('^(\\d+) Rezepte$', '$1 recipes');
addPat('^\\((\\d+) Rezepte\\)$', '($1 recipes)');
addPat('^(.+) ist ein Rezept aus Life is Feudal: Your Own auf dem Community-Server „Die wulpischen Lande“ \\(Insel Myralon\\)\\. Die Angaben gelten für unseren Server mit dem GMC-Modpack\\.$', '$1 is a recipe from Life is Feudal: Your Own on the community server "Die wulpischen Lande" (island of Myralon). The details apply to our server with the GMC modpack.');
addPat('^Hier findest du alle (\\d+) Rezepte unseres Life-is-Feudal-Your-Own-Servers auf der Insel Myralon, geordnet nach Fertigkeit\\. Zu jedem Rezept stehen Ergebnis, Fertigkeitsstufe, Werkzeug und alle Zutaten\\.$', 'Here you find all $1 recipes of our Life is Feudal: Your Own server on the island of Myralon, sorted by skill. Every recipe lists the result, skill level, tool and all ingredients.');
writeFileSync(dp, JSON.stringify(d, null, 1), 'utf8');
console.log('static pages:', urls.length, '(skills:', skills.size, ', recipes:', recipes.length, ') | sitemap urls:', base.length + urls.length);
