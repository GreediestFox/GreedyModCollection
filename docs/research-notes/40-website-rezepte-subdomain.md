# 40 Website: Forum button became "Rezepte", recipe book moved to a subdomain

Date: 2026-11-09. The website source is now in the repo as `website/` (source only, see its README).

## What changed
- The "Forum" button (it only opened Discord) was renamed **Rezepte** and points to `https://rezepte.wulpischelande.de` on every page (`data-link="rezepte"`, key `rezepte` in `public/data/links.json`). Discord has its own buttons.
- `rezepte.wulpischelande.de` is a placeholder Worker (`rezepte-publish/`, `noindex`) until the real recipe site exists. Subdomains are created by the `domains` entry in `cloudflare.config.ts` (same pattern as the wiki).
- The old on-page recipe book was removed: section "Rezeptbuch" with search, its menu link, the recipe code in `js/main.js`, the 882 generated recipe pages and their sitemap entries. Sections were renumbered. Old `/rezepte/...` addresses are redirected (301) to the subdomain via `public/_redirects`.
- The recipe data (853 recipes, German + English names) was kept in an archive folder outside the published site, ready for the future subdomain.

## Deploy order
`Rezepte-veroeffentlichen.bat` first (creates the subdomain), then `Website-veroeffentlichen.bat`.

## Notes
- The recipe generator (`tools/recipes`) still writes into `public/`; point it at the subdomain project before using it again.
- The repo is public: the copy here leaves out pictures, the legal pages with the operator's address and team data on purpose.
- Typing multi-line Python through `python -c` in PowerShell breaks on quotes; use a script file.
