(function () {
  'use strict';

  /* ---------- helpers ---------- */
  var $ = function (s, r) { return (r || document).querySelector(s); };
  var $$ = function (s, r) { return Array.prototype.slice.call((r || document).querySelectorAll(s)); };
  function esc(t) { var d = document.createElement('div'); d.textContent = t == null ? '' : String(t); return d.innerHTML; }
  function getJSON(url) {
    return fetch(url, { cache: 'no-store' }).then(function (r) {
      if (!r.ok) throw new Error(url + ' ' + r.status);
      return r.json();
    });
  }

  /* ---------- image slots: show the picture if the file exists, else keep the placeholder ---------- */
  function fillSlot(slot, src) {
    var img = new Image();
    img.onload = function () {
      slot.classList.add('has-img');
      img.alt = slot.getAttribute('data-alt') || '';
      slot.appendChild(img);
    };
    img.src = src;
  }
  $$('.slot[data-img]').forEach(function (s) { fillSlot(s, s.getAttribute('data-img')); });

  /* ---------- loader ---------- */
  var pctEl = $('#loaderPct'), loader = $('#loader');
  var shown = 0, target = 0, pageReady = false, started = Date.now();
  function tick() {
    var minTime = 1800;
    var timePct = Math.min(100, ((Date.now() - started) / minTime) * 100);
    target = pageReady ? Math.min(100, timePct) : Math.min(90, timePct);
    shown += (target - shown) * 0.12;
    if (target - shown < 0.5) shown = target;
    pctEl.textContent = Math.round(shown);
    if (shown >= 100) { finishLoad(); return; }
    requestAnimationFrame(tick);
  }
  function finishLoad() {
    loader.classList.add('done');
    document.body.classList.remove('is-loading');
    document.documentElement.classList.add('pics'); // now start loading the section pictures
  }
  // the loader waits for the page text and fonts only; the big pictures keep loading in the background
  var domDone = document.readyState !== 'loading', fontsDone = !document.fonts;
  function checkReady() { if (domDone && fontsDone) pageReady = true; }
  if (!domDone) document.addEventListener('DOMContentLoaded', function () { domDone = true; checkReady(); });
  if (document.fonts) document.fonts.ready.then(function () { fontsDone = true; checkReady(); });
  checkReady();
  setTimeout(function () { pageReady = true; }, 3000); // never trap people behind the loader
  requestAnimationFrame(tick);

  /* ---------- palette: crimson at the top -> orange at the bottom ---------- */
  var C0 = { a: [179, 22, 42], b: [224, 67, 47] };   // crimson
  var C1 = { a: [232, 116, 26], b: [247, 170, 56] }; // orange
  function mix(p, q, t) { return p.map(function (v, i) { return Math.round(v + (q[i] - v) * t); }); }
  function rgb(c) { return 'rgb(' + c.join(',') + ')'; }
  var root = document.documentElement;
  var nav = $('#nav');
  function onScroll() {
    var max = document.documentElement.scrollHeight - window.innerHeight;
    var t = max > 0 ? Math.min(1, Math.max(0, window.scrollY / max)) : 0;
    var a = mix(C0.a, C1.a, t), b = mix(C0.b, C1.b, t);
    root.style.setProperty('--accent', rgb(a));
    root.style.setProperty('--accent-2', rgb(b));
    root.style.setProperty('--glow', 'rgba(' + a.join(',') + ',.45)');
    nav.classList.toggle('solid', window.scrollY > 40);
  }
  window.addEventListener('scroll', onScroll, { passive: true });
  onScroll();

  /* ---------- mobile nav ---------- */
  var toggle = $('#navToggle'), links = $('#navLinks');
  toggle.addEventListener('click', function () {
    var open = links.classList.toggle('open');
    toggle.setAttribute('aria-expanded', open ? 'true' : 'false');
  });
  $$('#navLinks a').forEach(function (a) {
    a.addEventListener('click', function () { links.classList.remove('open'); toggle.setAttribute('aria-expanded', 'false'); });
  });

  /* ---------- reveal on scroll ---------- */
  var revealTargets = $$('.band .wrap > *, .news, .recipe, .steps li, .card');
  revealTargets.forEach(function (el) { el.classList.add('reveal'); });
  if ('IntersectionObserver' in window) {
    var io = new IntersectionObserver(function (entries) {
      entries.forEach(function (e) { if (e.isIntersecting) { e.target.classList.add('in'); io.unobserve(e.target); } });
    }, { threshold: 0.08 });
    revealTargets.forEach(function (el) { io.observe(el); });
  } else {
    revealTargets.forEach(function (el) { el.classList.add('in'); });
  }
  function revealNew(root) {
    $$('.news, .recipe', root).forEach(function (el) {
      el.classList.add('reveal');
      if ('IntersectionObserver' in window) io.observe(el); else el.classList.add('in');
    });
  }

  /* ---------- links and server address (data/links.json) ----------
     Every element with data-link="discord", "rezepte" or "wiki" gets its address from this file,
     and data-text="serverAddress" shows the server address. Change them in one place.
     Wiki links stay hidden until links.json contains a "wiki" address. */
  var linksP = getJSON('data/links.json').catch(function () { return {}; });
  linksP.then(function (l) {
    // blocks that need an address (data-needs="patcher") appear only when links.json has it
    $$('[data-needs]').forEach(function (e) {
      var v = l[e.getAttribute('data-needs')];
      e.hidden = !(typeof v === 'string' && /^https?:\/\//i.test(v));
    });
    $$('[data-link]').forEach(function (a) {
      var k = a.getAttribute('data-link'), u = l[k];
      if (typeof u === 'string' && /^https?:\/\//i.test(u)) {
        a.setAttribute('href', u.replace(/\/$/, '') + (a.getAttribute('data-path') || (u.slice(-1) === '/' ? '/' : '')));
        a.hidden = false;
      } else if (k === 'wiki' || k === 'patcher') a.hidden = true;
    });
    $$('[data-text]').forEach(function (e) {
      var t = l[e.getAttribute('data-text')];
      if (typeof t === 'string' && t) e.textContent = t;
    });
  });

  /* ---------- news (data/news.json) ---------- */
  getJSON('data/news.json').then(function (items) {
    var grid = $('#newsGrid');
    grid.innerHTML = items.map(function (n) {
      return '<article class="news"><div class="slot" data-img="' + esc(n.image || '') + '" data-label="Bild"></div>' +
        '<div class="news-body"><time>' + esc(n.date) + '</time><h3>' + esc(n.title) + '</h3><p>' + esc(n.text) + '</p></div></article>';
    }).join('');
    $$('.slot[data-img]', grid).forEach(function (s) { var src = s.getAttribute('data-img'); if (src) fillSlot(s, src); });
    revealNew(grid);
  }).catch(function () { $('#newsGrid').innerHTML = '<p class="muted">Die Neuigkeiten konnten nicht geladen werden.</p>'; });

  /* ---------- gallery (data/gallery.json: list of image paths) ---------- */
  var galleryGrid = $('#gallery-grid');
  getJSON('data/gallery.json').then(function (list) {
    galleryGrid.innerHTML = list.map(function (g) {
      return '<div class="slot" data-img="' + esc(g.src) + '" data-alt="' + esc(g.alt || '') + '" data-label="' + esc(g.alt || 'Galeriebild') + '"></div>';
    }).join('');
    $$('.slot', galleryGrid).forEach(function (s) { fillSlot(s, s.getAttribute('data-img')); });
  }).catch(function () {});
  galleryGrid.addEventListener('click', function (e) {
    var img = e.target.closest('img');
    if (!img) return;
    var box = document.createElement('div');
    box.className = 'lightbox';
    box.innerHTML = '<img src="' + esc(img.src) + '" alt="">';
    box.addEventListener('click', function () { box.remove(); });
    document.addEventListener('keydown', function onKey(ev) { if (ev.key === 'Escape') { box.remove(); document.removeEventListener('keydown', onKey); } });
    document.body.appendChild(box);
  });

  /* ---------- thanks (data/thanks.json) ---------- */
  getJSON('data/thanks.json').then(function (list) {
    $('#thanksList').innerHTML = list.map(function (p) {
      return '<li><b>' + esc(p.name) + '</b><span>' + esc(p.role) + '</span></li>';
    }).join('');
  }).catch(function () {});

  /* ---------- server status (data/status.json, written by your server) ---------- */
  var pill = $('#statusPill');
  function setStatus(s) {
    if (s === 'unknown') {   // no status data could be read at all: do not claim online or offline
      pill.className = 'status-pill';
      $('span', pill).textContent = 'Status unbekannt';
      $('#stState').textContent = 'Unbekannt'; $('#stState').className = '';
      $('#stPlayers').textContent = '—'; $('#stVersion').textContent = '—'; $('#stUpdated').textContent = '—';
      return;
    }
    var online = !!(s && s.online);
    pill.className = 'status-pill ' + (online ? 'online' : 'offline');
    $('span', pill).textContent = online ? 'Online · ' + s.players + (s.max ? '/' + s.max : '') + ' Spieler' : 'Offline';
    $('#stState').textContent = online ? 'Online' : 'Offline'; $('#stState').className = online ? 'ok' : 'bad';   // green / red
    $('#stPlayers').textContent = online ? s.players + (s.max ? ' / ' + s.max : '') : '—';
    $('#stVersion').textContent = (s && s.version) || '—';
    if (s && s.address && $('#stAddress')) $('#stAddress').textContent = s.address;   // live address from the server listing (links.json has a fixed fallback)
    $('#stUpdated').textContent = s && s.updated ? new Date(s.updated).toLocaleString(document.documentElement.lang === 'en' ? 'en-GB' : 'de-AT') : '—';
  }
  // the status file comes from links.json ("status": address of the file the status updater uploads); fallback: data/status.json
  function pollStatus() {
    linksP.then(function (l) {
      var u = (l && typeof l.status === 'string' && /^https?:\/\//i.test(l.status)) ? l.status : 'data/status.json';
      return fetch(u + (u.indexOf('?') < 0 ? '?' : '&') + 't=' + Date.now(), { cache: 'no-store' });
    }).then(function (r) {
      if (!r.ok) throw new Error(r.status);
      return r.json();
    }).then(function (s) {
      if (!s || typeof s !== 'object') throw new Error('bad status file');
      // a status file older than 5 minutes means the status updater (on the server PC) is not running: show offline
      if (s.updated && Date.now() - new Date(s.updated).getTime() > 5 * 60 * 1000) s.online = false;
      setStatus(s);
    }).catch(function () { setStatus('unknown'); });
  }
  pollStatus();
  setInterval(pollStatus, 60000);
})();
