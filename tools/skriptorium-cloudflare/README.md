# Skriptorium on Cloudflare (Worker + D1)

Port of `BookWebsite` (Express + SQLite). Same pages (`public/`), same API paths and answers; the game mod (`BooksPack`) works unchanged against it.

- `src/index.js` - the Worker (router, sanitizer, cipher, PBKDF2 password hashing). No dependencies.
- `migrations/0001_init.sql` - D1 schema (includes the `ticket` column).
- `wrangler.jsonc` - Worker + static assets (`public/`) + D1 binding `DB`. Only `/api/*`, `/admin/*`, `/open` go through the Worker first.
- `test_api.py` - end-to-end API test (`python -I test_api.py http://127.0.0.1:8799`): 35 checks, all pass locally.

Differences to the Express version
- Book passwords use PBKDF2-SHA256 (100k, WebCrypto) instead of bcrypt: bcrypt is too slow for Worker CPU limits. A fresh database, so no old hashes to migrate.
- HTML cleaning uses `HTMLRewriter` with the same whitelist as `sanitize-html` before (tags, span color/font-size, img http(s) src + width).
- The legacy `/uploads/` folder is gone (no image upload since long; images are links).
- Old local documents are not copied over (test data only).

## Local test
```
npm install
echo TABLET_ADMIN_KEY=localtestkey123 > .dev.vars
npx wrangler d1 migrations apply skriptorium --local
npx wrangler dev --local --port 8799
```

## Publish (not done yet)
```
npx wrangler login
npx wrangler d1 create skriptorium            # copy the database_id into wrangler.jsonc ("database_id")
npx wrangler d1 migrations apply skriptorium --remote
npx wrangler secret put TABLET_ADMIN_KEY      # long random value
npx wrangler deploy
```
Then: custom domain (e.g. `skriptorium.wulpischelande.de`, Workers -> Settings -> Domains) and in the game server `mods/LiFx/BooksPack/config.cs`
`$BooksPack::baseUrl = "https://<address>"` (no trailing slash) and `$BooksPack::tabletAdminKey` = the same key as the secret; restart the game server.
(`cf deploy` does not read `wrangler.jsonc`; deploy with wrangler.)


Note: public/ (the static pages) is not part of this folder; copy it from the original BookWebsite (public/) before running or deploying. Account-specific values in wrangler.jsonc (database_id, domain) must be replaced for another account.
