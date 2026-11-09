/* Shared script for team.html and thanks.html.
   Menu, colour shift, picture slots, links.json, and the data-driven lists. */
(function () {
  'use strict';

  var $ = function (s, r) { return (r || document).querySelector(s); };
  var $$ = function (s, r) { return Array.prototype.slice.call((r || document).querySelectorAll(s)); };
  function esc(t) {
    return String(t == null ? '' : t).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
  }
  function safeUrl(u) { return (typeof u === 'string' && /^https?:\/\//i.test(u)) ? u : null; }
  function slug(s) { return String(s || '').toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '') || 'entry'; }
  function getJSON(url) {
    return fetch(url, { cache: 'no-store' }).then(function (r) { if (!r.ok) throw new Error(url + ' ' + r.status); return r.json(); });
  }

  /* ---------- picture slots: the picture appears as soon as its file exists ---------- */
  function slotHTML(src, label, cls) {
    return '<div class="slot' + (cls ? ' ' + cls : '') + '" data-img="' + esc(src) + '" data-label="' + esc(label) + '"></div>';
  }
  function fillSlot(slot, src) {
    var img = new Image();
    img.onload = function () { slot.classList.add('has-img'); img.alt = slot.getAttribute('data-alt') || ''; slot.appendChild(img); };
    img.src = src;
  }
  function fillAll(root) {
    $$('.slot[data-img]', root).forEach(function (s) {
      var src = s.getAttribute('data-img');
      if (src && !s.querySelector('img')) fillSlot(s, src);
    });
  }

  /* ---------- colour shift: crimson at the top, orange at the bottom ---------- */
  var C0 = { a: [179, 22, 42], b: [224, 67, 47] }, C1 = { a: [232, 116, 26], b: [247, 170, 56] };
  function mix(p, q, t) { return p.map(function (v, i) { return Math.round(v + (q[i] - v) * t); }); }
  function onScroll() {
    var max = document.documentElement.scrollHeight - window.innerHeight;
    var t = max > 0 ? Math.min(1, Math.max(0, window.scrollY / max)) : 0;
    var a = mix(C0.a, C1.a, t), b = mix(C0.b, C1.b, t), root = document.documentElement;
    root.style.setProperty('--accent', 'rgb(' + a.join(',') + ')');
    root.style.setProperty('--accent-2', 'rgb(' + b.join(',') + ')');
    root.style.setProperty('--glow', 'rgba(' + a.join(',') + ',.45)');
  }
  window.addEventListener('scroll', onScroll, { passive: true });
  onScroll();

  /* ---------- mobile menu ---------- */
  var toggle = $('#navToggle'), links = $('#navLinks');
  if (toggle && links) {
    toggle.addEventListener('click', function () {
      var open = links.classList.toggle('open');
      toggle.setAttribute('aria-expanded', open ? 'true' : 'false');
    });
  }

  /* ---------- reveal on scroll ---------- */
  var io = ('IntersectionObserver' in window) ? new IntersectionObserver(function (entries) {
    entries.forEach(function (e) { if (e.isIntersecting) { e.target.classList.add('in'); io.unobserve(e.target); } });
  }, { threshold: 0.08 }) : null;
  function reveal(root) {
    $$('.member, .role-card, .feature, .more-thanks, .apply, .recruit-intro', root).forEach(function (el) {
      el.classList.add('reveal');
      if (io) io.observe(el); else el.classList.add('in');
    });
  }

  /* ---------- links and server address (data/links.json) ---------- */
  getJSON('data/links.json').then(function (l) {
    $$('[data-link]').forEach(function (a) {
      var k = a.getAttribute('data-link');
      var u = safeUrl(l[k]) || (k === 'apply' ? safeUrl(l.discord) : null);
      if (u) { a.setAttribute('href', u.replace(/\/$/, '') + (a.getAttribute('data-path') || (u.slice(-1) === '/' ? '/' : ''))); a.hidden = false; }
      else if (k === 'wiki') a.hidden = true;   // no wiki address in links.json yet: keep the wiki links hidden
    });
    $$('[data-text]').forEach(function (e) {
      var t = l[e.getAttribute('data-text')];
      if (typeof t === 'string' && t) e.textContent = t;
    });
  }).catch(function () {});

  /* A text typed on several lines (Enter in the editor) becomes one block per line, so lines never run together.
     Single-line texts stay a normal paragraph. data-ml tells js/i18n.js to translate the lines as one unit. */
  function para(text) {
    var lines = String(text == null ? '' : text).split(/\n+/).map(function (s) { return s.trim(); }).filter(Boolean);
    if (lines.length < 2) return '<p>' + esc(lines[0] || '') + '</p>';
    return '<p data-ml>' + lines.map(function (l) { return '<span class="ln">' + esc(l) + '</span>'; }).join('') + '</p>';
  }

  function chipLinks(list) {
    return (list || []).map(function (l) {
      var u = safeUrl(l && l.url);
      return u ? '<a class="chip" href="' + esc(u) + '" target="_blank" rel="noopener">' + esc(l.label || u) + '</a>' : '';
    }).join('');
  }

  /* ---------- team page (data/team.json) ---------- */
  var teamGrid = $('#teamGrid');
  if (teamGrid) {
    getJSON('data/team.json').then(function (t) {
      var recruiting = !!t.recruiting;
      var top = $('#recruitBtn');
      if (top) top.textContent = recruiting ? 'Wir suchen Verstärkung' : 'Tritt dem Team bei';

      teamGrid.innerHTML = (t.members || []).map(function (m, i) {
        var img = m.image || ('assets/img/team/member-' + (i + 1) + '.jpg');
        var ch = chipLinks(m.links);
        return '<article class="member"><div class="member-photo">' + slotHTML(img, 'Foto · 800×1000') + '</div>' +
          '<div class="member-body"><h3>' + esc(m.name) + '</h3><p class="role">' + esc(m.role) + '</p>' +
          (m.bio ? para(m.bio) : '') + (ch ? '<div class="chips">' + ch + '</div>' : '') + '</div></article>';
      }).join('') || '<p class="muted center">Die Teamliste folgt bald.</p>';

      var intro = $('#recruitIntro');
      if (intro) intro.textContent = t.recruitIntro || '';
      var roles = $('#roleList');
      if (roles) {
        var list = recruiting ? (t.openRoles || []) : [];
        roles.innerHTML = list.map(function (r) {
          var tags = (r.tags || []).map(function (x) { return '<span class="chip">' + esc(x) + '</span>'; }).join('');
          return '<article class="role-card"><h3>' + esc(r.title) + '</h3>' + para(r.description) + (tags ? '<div class="chips">' + tags + '</div>' : '') + '</article>';
        }).join('');
        var none = $('#noRoles');
        if (none) { none.hidden = list.length > 0; none.textContent = t.noRolesText || 'Derzeit gibt es keine offenen Stellen, aber sag uns auf Discord Bescheid, wenn du helfen möchtest.'; }
      }
      fillAll(document);
      reveal(document);
    }).catch(function () { teamGrid.innerHTML = '<p class="muted center">Die Teamliste konnte nicht geladen werden.</p>'; });
  }

  /* ---------- featured thanks (data/featured.json) and the plain list (data/thanks.json) ---------- */
  var featureList = $('#featureList');
  if (featureList) {
    getJSON('data/featured.json').then(function (list) {
      featureList.innerHTML = list.map(function (f) {
        var id = f.id || slug(f.name);
        var imgs = (f.images && f.images.length) ? f.images : [];
        var n = Math.max(3, imgs.length), slots = [];
        for (var k = 0; k < n; k++) slots.push(imgs[k] || ('assets/img/featured/' + id + '-' + (k + 1) + '.jpg'));
        var thumbs = slots.slice(1).map(function (s, j) { return slotHTML(s, 'Bild ' + (j + 2), 'feature-thumb'); }).join('');
        var paras = (f.text || []).map(function (p) { return para(p); }).join('');
        var ch = chipLinks(f.links);
        var eyebrow = [f.kind || 'Besonderer Dank', f.where].filter(Boolean).map(esc).join(' · ');
        return '<article class="feature" id="' + esc(id) + '">' +
          '<div class="feature-media">' + slotHTML(slots[0], 'Hauptbild · 1200×900', 'feature-main') + '<div class="feature-thumbs">' + thumbs + '</div></div>' +
          '<div class="feature-body"><p class="eyebrow">' + eyebrow + '</p><h3>' + esc(f.name) + '</h3>' +
          (f.role ? '<p class="role">' + esc(f.role) + '</p>' : '') + paras +
          (f.quote ? '<blockquote>' + esc(f.quote) + '</blockquote>' : '') +
          (ch ? '<div class="chips">' + ch + '</div>' : '') + '</div></article>';
      }).join('') || '<p class="muted center">Vorgestellte Danksagungen folgen bald.</p>';
      fillAll(featureList);
      reveal(document);
    }).catch(function () { featureList.innerHTML = '<p class="muted center">Konnte nicht geladen werden.</p>'; });
  }
  var thanksList = $('#thanksList');
  if (thanksList) {
    getJSON('data/thanks.json').then(function (list) {
      thanksList.innerHTML = list.map(function (p) { return '<li><b>' + esc(p.name) + '</b><span>' + esc(p.role) + '</span></li>'; }).join('');
    }).catch(function () {});
  }
})();
