/* Deutsch <-> English switch (no external service, nothing leaves your server).
   The pages are written in German. English comes from data/i18n-en.json: exact German text -> English text.
   A text that is not listed simply stays German, so you can add entries whenever you write new texts.
   "patterns" are for texts with numbers or names in them ($1, $2).
   (Adapted from the original add-on, which had English as the page language. The logic is the same.) */
(function () {
  'use strict';
  var KEY = 'lang';
  var dict = null, patterns = [], lang = 'de', busy = false, timer = null;
  var origText = new WeakMap();   // text node -> German original
  var origAttr = new WeakMap();   // element -> { attr: German original }
  var ATTRS = ['placeholder', 'aria-label', 'title', 'alt'];
  var SKIP = { SCRIPT: 1, STYLE: 1, TEXTAREA: 1, CODE: 1, NOSCRIPT: 1 };

  function store(v) { try { if (v) localStorage.setItem(KEY, v); return localStorage.getItem(KEY); } catch (e) { return null; } }
  function initial() {
    var q = /[?&]lang=(en|de)\b/.exec(location.search);
    if (q) { store(q[1]); return q[1]; }
    var s = store();
    if (s === 'en' || s === 'de') return s;
    return /^de\b/i.test(navigator.language || 'de') ? 'de' : 'en';   // German browsers get German, everyone else English
  }

  function norm(s) { return s.replace(/\s+/g, ' ').trim(); }
  function has(k) { return Object.prototype.hasOwnProperty.call(dict, k); }
  function tr(de) {
    var k = norm(de);
    if (!k || !dict) return null;
    if (has(k)) return dict[k];
    var mk = k.match(/^Ergebnis: (.+)$/);   // recipe card: "Ergebnis: <item>" -> "Makes: <item in English>"
    if (mk) { var inner = tr(mk[1]); return 'Makes: ' + (inner !== null ? inner : mk[1]); }
    for (var i = 0; i < patterns.length; i++) {
      if (patterns[i].re.test(k)) return k.replace(patterns[i].re, patterns[i].to);
    }
    // recipe ingredient lines: "4 × Holzbretter", "4 × Metallplatte (Regio notwendig)", "Säge (Haltbarkeit 10)"
    var m = k.match(/^(\d+) × (.+?)(?: \((.+)\))?$/);
    if (m && has(m[2])) return m[1] + ' × ' + dict[m[2]] + (m[3] ? ' (' + (has(m[3]) ? dict[m[3]] : m[3]) + ')' : '');
    m = k.match(/^(.+?) \(Haltbarkeit (\d+)\)$/);
    if (m) return (has(m[1]) ? dict[m[1]] : m[1]) + ' (durability ' + m[2] + ')';
    return null;
  }
  window.gmcI18n = { lang: function () { return lang; }, tr: function (s) { return tr(s); } };

  // the English names of the recipe book are a separate, larger file: loaded only in English mode and only where the recipe list exists
  var extraState = 0;   // 0 = not loaded, 1 = loading, 2 = loaded
  function loadExtra() {
    if (extraState || lang !== 'en' || !dict || !document.getElementById('recipeList')) return;
    extraState = 1;
    fetch('data/i18n-recipes-en.json', { cache: 'no-cache' }).then(function (r) { return r.json(); }).then(function (j) {
      Object.keys(j.texts || {}).forEach(function (k) { dict[norm(k)] = j.texts[k]; });
      (j.patterns || []).forEach(function (p) { patterns.push({ re: new RegExp(p.match), to: p.replace }); });
      extraState = 2;
      if (lang === 'en') all();
    }).catch(function () { extraState = 0; });
  }
  function keepSpace(src, out) {
    return (/^\s/.test(src) ? ' ' : '') + out + (/\s$/.test(src) ? ' ' : '');
  }

  function textNode(n) {
    var de = origText.has(n) ? origText.get(n) : n.nodeValue;
    if (lang === 'en') {
      var out = tr(de);
      if (out !== null) {
        if (!origText.has(n)) origText.set(n, de);
        var v = keepSpace(de, out);
        if (n.nodeValue !== v) n.nodeValue = v;
      }
    } else if (origText.has(n)) {
      n.nodeValue = origText.get(n); origText.delete(n);
    }
  }
  function attrs(el) {
    for (var i = 0; i < ATTRS.length; i++) {
      var a = ATTRS[i];
      if (!el.hasAttribute(a)) continue;
      var rec = origAttr.get(el) || {};
      var de = (a in rec) ? rec[a] : el.getAttribute(a);
      if (lang === 'en') {
        var out = tr(de);
        if (out !== null) { rec[a] = de; origAttr.set(el, rec); el.setAttribute(a, out); }
      } else if (a in rec) { el.setAttribute(a, rec[a]); delete rec[a]; }
    }
    if (el.tagName === 'META' && el.getAttribute('name') === 'description') {
      var r2 = origAttr.get(el) || {};
      var c = ('content' in r2) ? r2.content : el.getAttribute('content');
      if (lang === 'en') { var o = tr(c); if (o !== null) { r2.content = c; origAttr.set(el, r2); el.setAttribute('content', o); } }
      else if ('content' in r2) { el.setAttribute('content', r2.content); delete r2.content; }
    }
  }
  /* multi-line texts (<p data-ml> with one <span class="ln"> per line): translated as one unit.
     The English text may be written on several lines too, then it is shown line by line. */
  var mlOrig = new WeakMap();
  function mlRender(el, arr) {
    el.textContent = '';
    arr.forEach(function (t) { var s = document.createElement('span'); s.className = 'ln'; s.textContent = t; el.appendChild(s); });
  }
  function multiline(el) {
    var lines = mlOrig.has(el) ? mlOrig.get(el) : [].map.call(el.children, function (c) { return c.textContent; });
    if (lang === 'en') {
      var out = tr(lines.join(' '));
      if (out !== null) {
        if (!mlOrig.has(el)) mlOrig.set(el, lines);
        var parts = out.split(/\n+/).map(function (s) { return s.trim(); }).filter(Boolean);
        if (parts.join('\n') !== [].map.call(el.children, function (c) { return c.textContent; }).join('\n')) mlRender(el, parts);
      }
    } else if (mlOrig.has(el)) { mlRender(el, mlOrig.get(el)); mlOrig.delete(el); }
  }
  function walk(root) {
    if (!root || (root.nodeType === 1 && (SKIP[root.tagName] || root.closest && root.closest('#adm-layer, [data-no-i18n]')))) return;
    if (root.nodeType === 3) { textNode(root); return; }
    if (root.nodeType !== 1 && root.nodeType !== 9) return;
    if (root.nodeType === 1 && root.hasAttribute('data-ml')) { multiline(root); return; }
    if (root.nodeType === 1) attrs(root);
    for (var c = root.firstChild; c; c = c.nextSibling) {
      if (c.nodeType === 1 && (SKIP[c.tagName] || c.hasAttribute('data-no-i18n') || c.id === 'adm-layer')) continue;
      walk(c);   // (a <p data-ml> is handled at the top of walk)
    }
  }
  function all() {
    busy = true;
    walk(document.documentElement);
    document.documentElement.setAttribute('lang', lang);
    busy = false;
  }

  var mo = new MutationObserver(function (recs) {
    if (busy) return;
    clearTimeout(timer);
    timer = setTimeout(function () {
      busy = true;
      recs.forEach(function (r) {
        if (r.type === 'childList') r.addedNodes.forEach(walk);
        else if (r.type === 'characterData') textNode(r.target);
      });
      busy = false;
    }, 30);
  });

  var btn;
  function paint() {
    if (!btn) return;
    var en = lang === 'en';
    btn.setAttribute('aria-pressed', en ? 'true' : 'false');
    btn.innerHTML = '<span class="' + (en ? 'on' : '') + '">EN</span><i></i><span class="' + (en ? '' : 'on') + '">DE</span>';
    btn.setAttribute('aria-label', en ? 'Language: English. Switch to German' : 'Sprache: Deutsch. Zu Englisch wechseln');
    btn.setAttribute('data-no-i18n', '');
  }
  function set(l) {
    lang = l; store(l); all(); paint(); loadExtra();
  }

  function css() {
    var s = document.createElement('style');
    s.textContent =
      '.lang-btn{display:inline-flex;align-items:center;gap:.45em;margin-left:.6rem;padding:.35em .7em;background:transparent;border:1px solid var(--accent-2,#e8761a);color:inherit;font:inherit;font-size:.82em;letter-spacing:.12em;cursor:pointer;line-height:1}' +
      '.lang-btn span{opacity:.5}.lang-btn span.on{opacity:1;color:var(--accent-2,#e8761a);font-weight:700}' +
      '.lang-btn i{width:1px;height:1em;background:currentColor;opacity:.35}' +
      '.lang-btn:hover,.lang-btn:focus-visible{background:rgba(232,118,26,.12)}' +
      '@media(max-width:820px){.lang-btn{margin:.8rem 0 0;align-self:flex-start}}';
    document.head.appendChild(s);
  }

  function start() {
    lang = initial();
    css();
    var nav = document.getElementById('navLinks') || document.querySelector('header nav');
    if (nav) {
      btn = document.createElement('button');
      btn.type = 'button'; btn.className = 'lang-btn';
      btn.addEventListener('click', function () { set(lang === 'en' ? 'de' : 'en'); });
      nav.appendChild(btn); paint();
    }
    document.documentElement.setAttribute('lang', lang);
    fetch('data/i18n-en.json', { cache: 'no-cache' }).then(function (r) { return r.json(); }).then(function (j) {
      dict = {};
      Object.keys(j.texts || {}).forEach(function (k) { dict[norm(k)] = j.texts[k]; });
      patterns = (j.patterns || []).map(function (p) { return { re: new RegExp(p.match), to: p.replace }; });
      if (lang === 'en') { all(); loadExtra(); }
      mo.observe(document.documentElement, { childList: true, subtree: true, characterData: true });
    }).catch(function () { /* no dictionary: stay German */ });
  }
  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', start); else start();
})();
