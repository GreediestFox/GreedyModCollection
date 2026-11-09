/* Maintenance screen.
 * Reads data/maintenance.json and covers the page with a "under maintenance" screen while enabled.
 * Switch it with:  node tools/maint.mjs on "message"   /   node tools/maint.mjs off
 *
 * NOTE: this is a courtesy screen, not security. Anyone technical can hide it in their browser.
 * Owner preview: open the site once with ?preview in the address to see the real site while maintenance is on
 * (stored in this browser only). Use ?preview=off to go back to the visitor view.
 */
(function () {
  'use strict';
  var URL_JSON = 'data/maintenance.json';
  var POLL_MS = 30000;
  var STORE = 'maint_preview';
  var overlay = null;

  function store(get, val) {
    try { if (get) return localStorage.getItem(STORE); if (val === null) localStorage.removeItem(STORE); else localStorage.setItem(STORE, val); } catch (e) {}
    return null;
  }
  var q = new URLSearchParams(location.search);
  if (q.has('preview')) store(false, q.get('preview') === 'off' ? null : '1');
  function isPreview() { return store(true) === '1'; }

  function css() {
    if (document.getElementById('maint-css')) return;
    var s = document.createElement('style');
    s.id = 'maint-css';
    s.textContent =
      '#maint{position:fixed;inset:0;z-index:2000;background:radial-gradient(ellipse at 50% 35%,#2a0a10 0%,#050304 70%);color:#e9e2d4;display:flex;flex-direction:column;align-items:center;justify-content:center;text-align:center;padding:2rem;font-family:"IM Fell English",Georgia,serif;overflow:auto}' +
      '#maint img{width:min(40vw,150px);height:auto;filter:drop-shadow(0 0 22px rgba(232,140,40,.55));animation:maintspin 24s linear infinite}' +
      '#maint h1{font-family:var(--head,"IM Fell English",serif);font-weight:900;letter-spacing:.04em;font-size:clamp(2.2rem,8vw,4.2rem);margin:1.4rem 0 .6rem;line-height:1.05}' +
      '#maint p{max-width:34rem;margin:.4rem auto;font-size:1.2rem;color:#d4cbbb}' +
      '#maint .until{font-family:"IM Fell English SC",Georgia,serif;letter-spacing:.12em;color:#e8902a;margin-top:1.2rem}' +
      '#maint a{color:#e8902a}' +
      '@keyframes maintspin{to{transform:rotate(360deg)}}' +
      '@media(prefers-reduced-motion:reduce){#maint img{animation:none}}' +
      '#maint-bar{position:fixed;left:0;right:0;bottom:0;z-index:1999;background:#b3162a;color:#fff;font:600 14px/1.3 system-ui,sans-serif;text-align:center;padding:.5rem 1rem}' +
      '#maint-bar a{color:#fff}' +
      'html.maint-on,html.maint-on body{overflow:hidden!important}';
    document.head.appendChild(s);
  }

  function fmtUntil(v) {
    if (!v) return '';
    var d = new Date(v);
    if (isNaN(d)) return String(v);
    return d.toLocaleString(document.documentElement.lang === 'en' ? 'en-GB' : 'de-AT', { dateStyle: 'medium', timeStyle: 'short' });
  }

  function showOverlay(cfg) {
    css();
    if (!overlay) {
      overlay = document.createElement('div');
      overlay.id = 'maint';
      overlay.setAttribute('role', 'alert');
      document.body.appendChild(overlay);
    }
    var until = fmtUntil(cfg.until);
    overlay.innerHTML = '';
    var img = document.createElement('img');
    img.src = 'assets/symbols/sun-original.png'; img.alt = ''; img.width = 150; img.height = 150;
    var h = document.createElement('h1'); h.textContent = cfg.title || 'Wartungsarbeiten';
    var p = document.createElement('p'); p.textContent = cfg.message || 'Wir arbeiten an der Seite und sind gleich wieder da.';
    overlay.appendChild(img); overlay.appendChild(h); overlay.appendChild(p);
    if (until) { var u = document.createElement('p'); u.className = 'until'; u.textContent = 'Voraussichtlich zurück: ' + until; overlay.appendChild(u); }
    document.documentElement.classList.add('maint-on');
  }
  function hideOverlay() {
    if (overlay) { overlay.remove(); overlay = null; }
    document.documentElement.classList.remove('maint-on');
  }
  function showBar(on) {
    var bar = document.getElementById('maint-bar');
    if (on && !bar) {
      css();
      bar = document.createElement('div'); bar.id = 'maint-bar';
      bar.innerHTML = 'Wartungsmodus ist AN — Besucher sehen die Wartungsseite. Du bist in der Besitzer-Vorschau. <a href="?preview=off">Vorschau verlassen</a>';
      document.body.appendChild(bar);
    } else if (!on && bar) bar.remove();
  }

  function apply(cfg) {
    var on = !!(cfg && cfg.enabled);
    if (on && cfg.until) { /* an expired end time does not switch it off by itself; the owner does that */ }
    if (on && !isPreview()) { showBar(false); showOverlay(cfg); }
    else { hideOverlay(); showBar(on && isPreview()); }
  }

  function check() {
    fetch(URL_JSON + '?t=' + Date.now(), { cache: 'no-store' })
      .then(function (r) { if (!r.ok) throw new Error(r.status); return r.json(); })
      .then(apply)
      .catch(function () { /* file missing or unreadable: leave the site as it is */ });
  }

  function start() { check(); setInterval(check, POLL_MS); document.addEventListener('visibilitychange', function () { if (!document.hidden) check(); }); }
  if (document.body) start(); else document.addEventListener('DOMContentLoaded', start);
})();
