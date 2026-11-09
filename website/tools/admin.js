/* Owner tools: "Replace image" buttons under the site's pictures.
 * They only appear when ALL of these are true:
 *   1. maintenance mode is ON (data/maintenance.json)
 *   2. this browser is in owner preview (open the site once with ?preview)
 *   3. the site is served by tools/admin-server.mjs on your own computer
 * Visitors never see them. Do not upload this file or tools/ to the public server.
 */
(function () {
  'use strict';
  var API = '/__admin';
  var CHECK_MS = 30000;
  var token = null, layer = null, dlg = null, active = false;
  var buttons = new Map();
  var frame = 0;

  function previewOn() { try { return localStorage.getItem('maint_preview') === '1'; } catch (e) { return false; } }

  function css() {
    if (document.getElementById('adm-css')) return;
    var s = document.createElement('style');
    s.id = 'adm-css';
    s.textContent =
      '#adm-layer{position:fixed;inset:0;z-index:1500;pointer-events:none}' +
      '.adm-btn{position:fixed;transform:translateX(-50%);pointer-events:auto;cursor:pointer;font:600 13px/1 system-ui,sans-serif;letter-spacing:.04em;color:#fff;background:#b3162a;border:1px solid #fff6;border-radius:3px;padding:8px 14px;box-shadow:0 2px 12px #000a}' +
      '.adm-btn:hover{background:#d4283d}' +
      '.adm-back{position:fixed;inset:0;z-index:1600;background:#000b;display:flex;align-items:center;justify-content:center;padding:1rem;font:15px/1.4 system-ui,sans-serif}' +
      '.adm-dlg{width:min(780px,100%);max-height:90vh;overflow:auto;background:#140a0c;color:#eee;border:1px solid #b3162a;border-radius:6px;padding:1.2rem 1.4rem;box-shadow:0 10px 60px #000}' +
      '.adm-dlg h3{margin:0 0 .2rem;font-size:1.2rem}' +
      '.adm-dlg code{color:#f0a060;word-break:break-all}' +
      '.adm-row{display:flex;gap:.7rem;align-items:center;flex-wrap:wrap;margin:1rem 0}' +
      '.adm-dlg button{font:inherit;cursor:pointer;color:#fff;background:#b3162a;border:0;border-radius:3px;padding:.5rem .9rem}' +
      '.adm-dlg button.alt{background:#333}' +
      '.adm-msg{min-height:1.3em;color:#f0a060}' +
      '.adm-grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(130px,1fr));gap:.7rem}' +
      '.adm-grid button{background:#1d1214;border:1px solid #ffffff22;padding:.3rem;text-align:center;font-size:12px;color:#ccc;word-break:break-all}' +
      '.adm-grid button:hover{border-color:#b3162a;background:#2a1519}' +
      '.adm-grid img{width:100%;height:90px;object-fit:cover;display:block;margin-bottom:.3rem;background:#000}' +
      '.adm-empty{color:#999}';
    document.head.appendChild(s);
  }

  function api(path, opts) {
    opts = opts || {};
    opts.headers = Object.assign({ 'X-Admin-Token': token }, opts.headers || {});
    opts.cache = 'no-store';
    return fetch(API + path, opts).then(function (r) {
      return r.json().catch(function () { return {}; }).then(function (j) {
        if (!r.ok) throw new Error(j.error || ('Error ' + r.status));
        return j;
      });
    });
  }

  function targetOf(slot) {
    var t = slot.getAttribute('data-img');
    if (!t) return null;
    try {
      var u = new URL(t, location.href);
      if (u.origin !== location.origin) return null;
      return decodeURIComponent(u.pathname).replace(/^\//, '');
    } catch (e) { return null; }
  }

  /* ---------- buttons ---------- */
  function ensureLayer() {
    if (layer) return;
    css();
    layer = document.createElement('div');
    layer.id = 'adm-layer';
    document.body.appendChild(layer);
  }
  function destroy() {
    if (layer) { layer.remove(); layer = null; }
    buttons.clear();
    closeDlg();
  }
  function render() {
    if (!active || !layer) return;
    if (document.body.classList.contains('is-loading')) { layer.style.display = 'none'; return; }
    layer.style.display = '';
    var vh = window.innerHeight, seen = new Set();
    Array.prototype.forEach.call(document.querySelectorAll('.slot[data-img]'), function (slot) {
      var target = targetOf(slot);
      if (!target) return;
      var r = slot.getBoundingClientRect();
      if (r.width < 60 || r.height < 40 || r.bottom < 0 || r.top > vh) return;
      seen.add(slot);
      var b = buttons.get(slot);
      if (!b) {
        b = document.createElement('button');
        b.type = 'button';
        b.className = 'adm-btn';
        b.textContent = 'Replace image';
        b.setAttribute('data-target', target);
        b.addEventListener('click', function (e) { e.preventDefault(); e.stopPropagation(); openDlg(slot); });
        layer.appendChild(b);
        buttons.set(slot, b);
      }
      var h = b.offsetHeight || 30;
      var top = Math.min(r.bottom - h - 8, vh - h - 48); // 48 keeps clear of the red preview bar
      top = Math.max(top, Math.max(r.top + 8, 8));
      b.style.left = (r.left + r.width / 2) + 'px';
      b.style.top = top + 'px';
    });
    buttons.forEach(function (b, slot) { if (!seen.has(slot)) { b.remove(); buttons.delete(slot); } });
  }
  function schedule() { if (!frame) frame = requestAnimationFrame(function () { frame = 0; render(); }); }

  /* ---------- dialog ---------- */
  function el(tag, cls, text) { var e = document.createElement(tag); if (cls) e.className = cls; if (text != null) e.textContent = text; return e; }
  function closeDlg() { if (dlg) { dlg.remove(); dlg = null; } }

  function openDlg(slot) {
    closeDlg();
    var target = targetOf(slot);
    var back = el('div', 'adm-back');
    var box = el('div', 'adm-dlg');
    back.appendChild(box);
    back.addEventListener('mousedown', function (e) { if (e.target === back) closeDlg(); });

    box.appendChild(el('h3', null, 'Replace picture'));
    var p = el('div'); p.appendChild(document.createTextNode('Slot: ')); p.appendChild(el('code', null, target)); box.appendChild(p);

    var row = el('div', 'adm-row');
    var up = el('button', null, 'Upload from computer…');
    var file = document.createElement('input');
    file.type = 'file'; file.accept = 'image/jpeg,image/png,image/webp,image/gif,image/avif'; file.multiple = true; file.hidden = true;
    var close = el('button', 'alt', 'Close');
    row.appendChild(up); row.appendChild(file); row.appendChild(close);
    box.appendChild(row);
    var msg = el('div', 'adm-msg');
    box.appendChild(msg);
    box.appendChild(el('p', 'adm-empty', 'Uploads go into the uploads/ folder. Click a picture below to use it for this slot (the old one is backed up).'));
    var grid = el('div', 'adm-grid');
    box.appendChild(grid);

    function say(t) { msg.textContent = t || ''; }
    function list() {
      api('/uploads').then(function (items) {
        grid.innerHTML = '';
        if (!items.length) { grid.appendChild(el('div', 'adm-empty', 'No pictures in uploads/ yet.')); return; }
        items.forEach(function (it) {
          var b = el('button');
          b.type = 'button';
          var img = document.createElement('img');
          img.src = 'uploads/' + encodeURIComponent(it.name) + '?v=' + Math.round(it.mtime);
          img.alt = '';
          b.appendChild(img);
          b.appendChild(document.createTextNode(it.name));
          b.addEventListener('click', function () { use(it.name); });
          grid.appendChild(b);
        });
      }).catch(function (e) { say(e.message); });
    }
    function use(name) {
      say('Replacing …');
      api('/replace', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ target: target, source: name }) })
        .then(function () { refreshSlot(slot); closeDlg(); })
        .catch(function (e) { say(e.message); });
    }
    function uploadAll(files) {
      var chain = Promise.resolve(), ok = 0;
      Array.prototype.forEach.call(files, function (f) {
        chain = chain.then(function () {
          say('Uploading ' + f.name + ' …');
          return api('/upload?name=' + encodeURIComponent(f.name), { method: 'POST', headers: { 'Content-Type': 'application/octet-stream' }, body: f })
            .then(function () { ok++; })
            .catch(function (e) { say(f.name + ': ' + e.message); throw e; });
        });
      });
      chain.then(function () { say(ok + ' uploaded. Click one to use it.'); list(); }).catch(function () { list(); });
    }
    up.addEventListener('click', function () { file.click(); });
    file.addEventListener('change', function () { if (file.files.length) uploadAll(file.files); file.value = ''; });
    close.addEventListener('click', closeDlg);

    document.body.appendChild(back);
    dlg = back;
    list();
  }

  document.addEventListener('keydown', function (e) { if (e.key === 'Escape') closeDlg(); });

  function refreshSlot(slot) {
    var base = slot.getAttribute('data-img').split('?')[0];
    var img = new Image();
    img.onload = function () {
      Array.prototype.forEach.call(slot.querySelectorAll('img'), function (old) { old.remove(); });
      img.alt = slot.getAttribute('data-alt') || '';
      slot.appendChild(img);
      slot.classList.add('has-img');
      schedule();
    };
    img.src = base + '?v=' + Date.now();
  }

  /* ---------- on/off ---------- */
  function check() {
    fetch('data/maintenance.json?t=' + Date.now(), { cache: 'no-store' })
      .then(function (r) { return r.json(); })
      .then(function (cfg) {
        if (!cfg.enabled || !previewOn()) throw new Error('off');
        return fetch(API + '/ping', { cache: 'no-store' });
      })
      .then(function (r) { if (!r.ok) throw new Error('no helper'); return r.json(); })
      .then(function (j) { token = j.token; active = true; ensureLayer(); render(); })
      .catch(function () { active = false; destroy(); });
  }

  function start() {
    check();
    setInterval(check, CHECK_MS);
    setInterval(render, 600);
    window.addEventListener('scroll', schedule, { passive: true });
    window.addEventListener('resize', schedule);
    document.addEventListener('visibilitychange', function () { if (!document.hidden) check(); });
  }
  if (document.body) start(); else document.addEventListener('DOMContentLoaded', start);
})();
