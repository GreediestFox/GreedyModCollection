# Changes to the BookWebsite for the game integration (GMC BooksPack 0.3.0)

When the site is hosted publicly, these files must be included (they differ from the original YOscriptorium version):

- `server/db.js` – new column `documents.ticket` (added automatically at start).
- `server/routes/ticket.js` (new) + 2 lines in `server/index.js` – `GET /api/ticket/<ticket>` returns `<code>\n<title>` as plain text once the document with that ticket is finalized, otherwise 404.
- `server/routes/letters.js`, `server/routes/books.js` – the create route stores an optional `ticket` (8-24 letters/digits).
- `public/js/letter.js`, `public/js/book.js` – the create call sends `ticket` from the URL (`?new=1&ticket=...`).

After hosting:

1. The site must be reachable by the game clients (HTTP or HTTPS) and `/api/title/<code>` and `/api/ticket/<ticket>` must work without login.
2. Set `$BooksPack::baseUrl` in `mods/LiFx/BooksPack/config.cs` (game server) to the public address, without a trailing slash.
3. Restart the game server. Clients learn the address from the server.

- Stone tablets: routes/tablets.js stores the ticket too; public/js/admin-tablet.js sends it. Use a long random value for the admin key (TABLET_ADMIN_KEY in .env) when hosting publicly, and put the same value into BooksPack config.cs as $BooksPack::tabletAdminKey (the server only gives it to GMs).
