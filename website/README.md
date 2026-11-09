# Website "Die wulpischen Lande" (source only)

Source of https://wulpischelande.de (static site on a Cloudflare Worker with static assets), the wiki (https://wiki.wulpischelande.de, Astro Starlight) and the placeholder of https://rezepte.wulpischelande.de.

This is a **source-only copy**. Deliberately not included (public repository):
- pictures (`public/assets`, press-kit/background images are third-party material), fonts, wiki screenshots, the fox picture of the placeholder page
- the legal pages `impressum.html`, `datenschutz.html`, `urheberrecht.html` (they contain the operator's name and address; menus in this copy link to them, so those links are dead here) and `team.json` / `thanks.json`
- generated output (`wiki-publish/public`, `dist`, `node_modules`, `.cloudflare`), backups, the recipe archive (853 recipes, 882 generated pages)

## Layout
- `public/` - the main site (index, team, thanks, `css/`, `js/`, `data/*.json`, `_redirects`, `sitemap.xml`)
- `tools/` - local helpers: team editor, admin overlay, maintenance switch, server status updater, recipe generator (`tools/recipes`)
- `wiki/` - the wiki sources (German root + `/en/`); built by `Wiki-veroeffentlichen.bat` into `wiki-publish/public`
- `wiki-publish/`, `rezepte-publish/` - deploy folders with their `cloudflare.config.ts`
- `*.bat` - publish helpers (`cf.cmd deploy`); run `Rezepte-veroeffentlichen.bat` first, then `Website-veroeffentlichen.bat`

## Recipes
The recipe book moved to the subdomain `rezepte.wulpischelande.de` (placeholder for now). `public/_redirects` sends the old `/rezepte/...` addresses there.
