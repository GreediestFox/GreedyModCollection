# Add-ons in this project (merged 2026-10-15)

Folder layout
- `public/`  = the website (this is what `cf.cmd deploy` publishes)
- `tools/`   = owner tools, run on your PC with Node. NOT published (they sit next to `public/`, not inside)
- `uploads/` = pictures you upload with the admin tool (created on first use, NOT published)
- `wiki/`    = the wiki (Astro Starlight), a separate project with its own build and its own deploy

## 1. Forum + Discord buttons  (`public/data/links.json`)
Every button with `data-link="discord" | "forum" | "wiki"` takes its address from `links.json`.
Change the invite there once; it updates the hero buttons, the menu, the join cards and the footer.
`"wiki": "https://..."` makes the Wiki menu entry and the two wiki cards appear (they are hidden until it is set).
"Forum" simply opens Discord in a new tab.

## 2. Team + Thanks pages  (`team.html`, `thanks.html`, `css/pages.css`, `js/pages.js`)
- `data/team.json`: `members` (name, role, bio, image, links), `recruiting` (true/false), `openRoles`, `recruitIntro`, `noRolesText`.
- `data/featured.json`: featured artists (id, name, kind, where, role, text[], quote, images[], links[]).
- `data/thanks.json`: the plain list of names (also shown on the main page).
- Pictures: `assets/img/team/member-1.jpg ...` and `assets/img/featured/<id>-1.jpg, -2.jpg, -3.jpg`.
- Both lists are EMPTY right now ("Die Teamliste folgt bald." / "Vorgestellte Danksagungen folgen bald."), because the add-on only shipped sample names.

## 3. Language switch DE | EN  (`js/i18n.js`, `data/i18n-en.json`)
The pages are written in German. The EN button translates them with the dictionary `data/i18n-en.json`
(exact German text -> English). A text that is not in the dictionary stays German.
=> Every time you write a new German text (news, recipe names, team bios ...) add one line to the dictionary.
First visit: German browsers get German, all others English. `?lang=en` / `?lang=de` forces a language.
`data-no-i18n` on an element = never translate it. (The original add-on had English as the page language; it was flipped for this site.)
Impressum and Datenschutz are German only.

## 4. Maintenance mode  (`js/maintenance.js`, `data/maintenance.json`, `tools/maint.mjs`)
Run in the PROJECT folder (the one with `tools/`):

    node tools/maint.mjs on "Wir tauschen Bilder aus, bis gleich."
    node tools/maint.mjs on "Rezept-Update" --until "2026-10-15 18:00" --title "Die Schmiede ist kalt"
    node tools/maint.mjs message "Gleich fertig."
    node tools/maint.mjs status
    node tools/maint.mjs off

This changes `public/data/maintenance.json` on your PC. It goes live when you deploy (`cf.cmd deploy`).
Visitors then see a full-screen maintenance page within ~30 s. You: open the site once with `?preview` on the end
to see the real site with a red bar (`?preview=off` to leave). It is a courtesy screen, not security.

## 5. Replace pictures from the page  (`tools/admin-server.mjs`, `tools/admin.js`)  - local only
1. `node tools/admin-server.mjs`   (serves `public/` on http://localhost:8000, 127.0.0.1 only; `--port 8001` if busy)
2. `node tools/maint.mjs on "Updating pictures"`
3. open http://localhost:8000/?preview  -> every picture slot gets a "Replace image" button
4. Upload from the dialog (goes to `uploads/`), click a picture to use it. The old one is backed up in `uploads/_replaced/`.
5. `node tools/maint.mjs off`, stop the server (Ctrl+C), then `cf.cmd deploy` to publish the new `public/assets/`.
Changes made for this project: the server serves `public/`, keeps `uploads/` outside it, and injects `admin.js` into pages
only while it runs (so `admin.js` is never part of the published site).

## 7. Team editor  (`Team-Editor-starten.bat`, `tools/editor.mjs`, `tools/editor.html`)  - local only
Double-click **Team-Editor-starten.bat** (needs Node.js). Your browser opens http://localhost:8002/__edit/ with a form for
- **Mitglieder** (name, role, short text, photo upload, links), **Offene Stellen** (recruiting on/off, roles, tags),
- **Künstler (Danksagung)** (featured artists: texts, quote, up to 4 pictures, links) and **Namensliste** (the plain thanks list).
Every German text has an optional English field. Whatever you enter there is written into `data/i18n-en.json`, so the EN button
translates the new entry automatically (no English entry = the text stays German in EN mode).
**Speichern** (or Ctrl+S) writes `team.json`, `featured.json`, `thanks.json`, `i18n-en.json` and first copies the old versions to `backups/<date>/`.
Photos are saved to `public/assets/img/team/` or `.../featured/`. "Team-Seite ansehen" shows the real page from your PC.
Afterwards run `cf.cmd deploy` to publish. The editor only listens on your own PC (127.0.0.1) and only writes those files.

## Legal pages
`public/impressum.html` and `public/datenschutz.html` contain the texts from `Doku/Impressum.docx` and `Doku/Datenschutzerklärung.docx`
(Stand 15. Oktober 2026). To change them, edit the .html files (plain text between the tags). The language switch does not translate them.

## 6. Wiki  (`wiki/`)  - not published yet
Configured already: server name, Discord invite, main-site address, fonts (Almendra Bold + IM Fell English, both OFL, self-hosted).
The pages inside are still the starter texts (English template + a small German skeleton in `src/content/docs/de/`) and need your content.

    cd wiki
    npm.cmd install
    npm.cmd run dev        # preview on http://localhost:4321
    npm.cmd run build      # creates wiki\dist
    cf.cmd deploy          # publishes it as its own site: die-wulpischen-lande-wiki (config files are in wiki\)  [untested]

Then put the wiki's address into `public/data/links.json` as `"wiki": "https://....workers.dev"` and deploy the main site:
the Wiki menu entry, "Installationsanleitung" (-> /start/install/) and "Wiki zu den Spielmechaniken" appear automatically.
Also set `site:` in `wiki/astro.config.mjs` to the wiki address before the final build.

## 8. Server status, patcher download and R2 (added 2026-10-15)
- **Status updater** (`Status-Updater-starten.bat` -> `tools/status-updater.ps1`): leave the window open while the game server runs.
  Every 60 s it reads the server log (read-only) and uploads `status.json` {online, players, version, updated} to the R2 bucket `gmc`
  (public address in `public/data/links.json` -> "status"). "online" = server process running AND log says "ready to accept connections";
  "players" = joins minus leaves in that log; "version" = modpack version from the public manifest. Test without upload:
  load the script as a script block with `-Once -NoUpload` (see the file header).
  On the website: fresh data = Online/Offline with players; data older than 5 minutes = Offline (updater not running);
  nothing readable = "Status unbekannt". It needs the R2 CORS rule (GET/HEAD from the website address, already set on bucket gmc).
  When you connect your own domain, ADD that domain to the CORS rule: `cf.cmd r2 buckets cors update gmc --rules @rules.json`.
- **Patcher download**: `downloads/GMC-Patcher.zip` (GmcPatcher.exe 1.0.2 + gmc-patcher.json + LIES-MICH.txt) is uploaded to
  `https://pub-23d5a23dd23640eaa704c9caa9ad4cf1.r2.dev/download/GMC-Patcher.zip`; the join section shows the download block when
  `links.json` has "patcher". To publish a new patcher: rebuild the zip from `_zip-inhalt/` and upload it again with rclone
  (`rclone copyto GMC-Patcher.zip r2:gmc/download/GMC-Patcher.zip`).
- The patcher is NOT code-signed yet (SmartScreen / antivirus may warn on first start; the page and LIES-MICH.txt say so).

## 9. Recipe book and gallery (added 2026-10-15)
- `public/data/recipes.json` = the 848 reworked recipes from the Google sheet "Rezepte Stand 2026" (first tab), German names from the game's own
  loc files, ingredient lines in the sheet author's wording. `public/data/i18n-recipes-en.json` = English names (from the database); it is loaded only
  in English mode. How to update everything when the sheet changes: `tools/recipes/README.md`.
- The recipe list shows 120 recipes at a time with a "Mehr anzeigen" button; search also finds English words in English mode.
- Gallery: `public/assets/img/gallery-1..6.webp`, descriptions in `public/data/gallery.json` (+ English in i18n-en.json).
