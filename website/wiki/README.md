# Wiki (Astro Starlight)

Static wiki for `wiki.yourdomain`. Needs Node 20+ (tested on 22).

    npm install
    npm run dev        # http://localhost:4321
    npm run build      # output in dist/ (upload this folder)

## First edits
1. `src/config.ts`: main site URL, Discord invite, server name.
2. `astro.config.mjs`: uncomment `site:` with your wiki address before deploying.
3. Fonts: copy your self-hosted font files to `public/fonts/` and fix the file names in `src/styles/fonts.css`.
4. Pages live in `src/content/docs/` as `.md` / `.mdx`. The sidebar is in `astro.config.mjs`.
5. Install-guide screenshots: `public/img/install/<name>.jpg`. Circles are set per picture, see the last section of the install page.

## Annotated screenshot
    import Annotated from '../../../components/Annotated.astro';
    <Annotated src="/img/install/01.jpg" alt="..." ratio="16/9"
      marks={[{ x: 22, y: 70, r: 9, label: 'Press Install' }]} />
x/y = circle centre in % of the picture, r = radius in % of picture width. Missing picture = placeholder.

## Deploy (Cloudflare Pages)
Build command `npm run build`, output directory `dist`, custom domain `wiki.yourdomain`.
Dark only, no external requests (fonts, scripts and search are all local), so it needs no cookie banner.
Legal links in the footer point to your main site's Impressum and Datenschutz.
