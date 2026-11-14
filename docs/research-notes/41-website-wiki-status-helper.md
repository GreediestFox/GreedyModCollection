# 41 Website and wiki: new documents, live server status, vote button

Date: 2026-11-14. Follows [40](40-website-rezepte-subdomain.md). The source copy is in `website/` (source only, see its README).

## Wiki content
- The wiki pages Serverkonzept, Serverregeln and Server-Lore were replaced by the operator's Word documents (German original plus English translation). The old Discord rules (age limit, house rules, voice chat, sanctions) are not part of the new Regelwerk and no longer appear on the wiki.
- The PvP rules keep their numbered outline (1., 4.2.1, 5.8.2) because the text refers to points such as "Punkt 4" and "5.8.2". Word's numbering is not stored in the text, so it was recovered from `numbering.xml`; the wiki renders it with CSS counters (`ol.legal`).
- The Lore page carries the seven pictures of the document (not in the repository: third-party art).
- The start page "Willkommen im Wiki" became "Server-Informationen". Later edits of the documents are compared against the previous conversion and only the difference is applied (example: one removed bullet in the Konzept).

## Main site
- Menu button "Wiki" became a two-line button SERVER / INFOS (one line in the phone menu).
- "Die Insel": the three boxes are equal portrait boxes side by side (one column below 900 px); new map picture.
- Server-Status card: address, state (green / red), players, version and update time, with a compact layout (address wide, state and players large, version and time small), a vote button and a credit link to the listing.

## Live status without an API key
The server listing on life-is-feudal.org offers a JSON interface, but it needs the owner's secret server key. Instead the Worker in `status-worker/` (`status.wulpischelande.de`) reads the public listing page (table rows Address / Status / Players / Version), caches it for 3 minutes at the edge and returns `{online, players, max, version, address, updated}`, the shape the page already used. It answers 502 ("unknown" on the page) when the layout is not recognised or the listing's own check is older than an hour, and sends CORS headers only for the project's own sites. If the key is ever available, only the Worker has to change (store the key as a Worker secret, never in the page).

## Traps
- A Worker with code is deployed with `wrangler deploy` (it reads `wrangler.jsonc`); `cf deploy` only knows the static-assets configuration.
- A new `.bat` file written by a script can be unreadable for a while; write it with CRLF line endings in ASCII and read it back before asking someone to run it.
- When testing against a local server, the browser may serve old cached CSS/JS; force a reload of the files before judging a change.
