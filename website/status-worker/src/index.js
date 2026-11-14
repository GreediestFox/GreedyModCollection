// Status helper for https://wulpischelande.de (runs at https://status.wulpischelande.de/)
// Reads the public server page on life-is-feudal.org and returns address, online state, players and version as JSON,
// in the same shape the website's status pill already understands: { online, players, max, version, address, updated }.
// The listing is fetched at most every 3 minutes per Cloudflare location (edge cache), so it is not hammered.
// Nothing secret is used: it only reads the public page. (The listing's JSON API would need the owner's server key.)

const SOURCE = 'https://life-is-feudal.org/server/25594/';
const CACHE_SECONDS = 180;
const MAX_CHECK_AGE_MIN = 60;   // if the listing's own last check is older than this, say "unknown" instead of guessing
const ALLOWED_ORIGINS = new Set(['https://wulpischelande.de', 'https://www.wulpischelande.de', 'https://wiki.wulpischelande.de']);

function clean(s) { return s.replace(/&amp;/g, '&').replace(/\s+/g, ' ').trim(); }

/** Reads the "Server Information" table of the listing page. Returns null if the layout is not recognised. */
export function parseListing(html) {
  const addr = html.match(/<strong>Address<\/strong>[\s\S]*?<\/button>\s*<strong>([^<]+)<\/strong>/);
  const stat = html.match(/<strong>Status<\/strong>\s*<\/td>\s*<td>\s*<button[^>]*>\s*([^<]+?)\s*<\/button>\s*<strong><small>([\s\S]*?)<\/small>/);
  const play = html.match(/<strong>Players<\/strong>\s*<\/td>\s*<td>\s*<strong>\s*(\d+)\s*\/\s*(\d+)/);
  const vers = html.match(/<strong>Version<\/strong>\s*<\/td>\s*<td>\s*<a[^>]*>\s*([^<]+?)\s*<\/a>/);
  if (!addr || !stat || !play) return null;
  const word = clean(stat[1]).toLowerCase();
  if (word !== 'online' && word !== 'offline') return null;
  // "Checked <1 minute ago", "Checked 4 minutes ago", "Checked 2 hours ago"
  const ago = clean(stat[2]);
  let minutes = 0;
  const m = ago.match(/(\d+)\s*(second|minute|hour|day)/i);
  if (m) minutes = Number(m[1]) * ({ second: 1 / 60, minute: 1, hour: 60, day: 1440 }[m[2].toLowerCase()]);
  return {
    online: word === 'online',
    players: Number(play[1]),
    max: Number(play[2]),
    version: vers ? clean(vers[1]) : '',
    address: clean(addr[1]),
    checkedMinutesAgo: Math.round(minutes),
  };
}

function corsHeaders(request) {
  const origin = request.headers.get('Origin') || '';
  const ok = ALLOWED_ORIGINS.has(origin) || /^http:\/\/(localhost|127\.0\.0\.1)(:\d+)?$/.test(origin);
  return ok ? { 'Access-Control-Allow-Origin': origin, 'Vary': 'Origin' } : { 'Vary': 'Origin' };
}

function json(request, body, status = 200) {
  return new Response(JSON.stringify(body), {
    status,
    headers: {
      'Content-Type': 'application/json; charset=utf-8',
      'Cache-Control': status === 200 ? 'public, max-age=60' : 'no-store',
      ...corsHeaders(request),
    },
  });
}

export default {
  async fetch(request) {
    const url = new URL(request.url);
    if (request.method === 'OPTIONS') return new Response(null, { status: 204, headers: { ...corsHeaders(request), 'Access-Control-Allow-Methods': 'GET, HEAD, OPTIONS', 'Access-Control-Max-Age': '86400' } });
    if (request.method !== 'GET' && request.method !== 'HEAD') return json(request, { error: 'method not allowed' }, 405);
    if (url.pathname !== '/' && url.pathname !== '/status.json') return json(request, { error: 'not found' }, 404);

    let html;
    try {
      const r = await fetch(SOURCE, {
        headers: { 'User-Agent': 'WulpischeLandeStatus/1.0 (+https://wulpischelande.de; status widget for our own listing, cached 3 min)', 'Accept': 'text/html' },
        cf: { cacheTtl: CACHE_SECONDS, cacheEverything: true },
      });
      if (!r.ok) return json(request, { error: 'listing answered ' + r.status }, 502);
      html = await r.text();
    } catch (e) {
      return json(request, { error: 'listing not reachable' }, 502);
    }

    const info = parseListing(html);
    if (!info) return json(request, { error: 'listing layout not recognised' }, 502);
    if (info.checkedMinutesAgo > MAX_CHECK_AGE_MIN) return json(request, { error: 'listing check is outdated', checkedMinutesAgo: info.checkedMinutesAgo }, 502);

    return json(request, { ...info, updated: new Date().toISOString(), source: 'life-is-feudal.org' });
  },
};
