"""List every recipe that still needs a Decorator's Kit (1634-1636 premium, 3042-3044 Jorvik), with German names and
the exact file:line of each requirement row (server mod.cs / dump.sql, client recipe_requirement.xml)."""
import re, glob, os, collections
S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
L = C + r'\data\loc\de\data'
KITS = {1634: 'Dekorationsset Anfänger', 1635: 'Dekorationsset Lehrling', 1636: 'Dekorationsset Meister',
        3042: 'Jorvik-Kit Lehrling', 3043: 'Jorvik-Kit Geselle', 3044: 'Jorvik-Kit Meister'}


def load(f):
    return dict((int(a), b) for a, b in re.findall(r'<string id="(\d+)">([^<]*)</string>', open(L + '\\' + f, encoding='utf-8-sig').read()))


rn, on, sn = load('recipe_Name.xml'), load('objects_types_Name.xml'), {}
for k in KITS:
    if k in on: KITS[k] = on[k]
rows = [l.rstrip('\n').split('\t') for l in open(r'E:\ClaudeScratch\outfits\kit_recipes.tsv', encoding='utf-8-sig') if l.strip()]
recipes = collections.OrderedDict()
for rid, name, res, sk, skname, lvl, st, mat, matname, qty, parent in rows:
    r = recipes.setdefault(int(rid), dict(name=name, res=int(res), sk=int(sk), skname=skname, lvl=lvl, parent=int(parent), kits=[]))
    r['kits'].append((int(mat), int(qty)))

# where the requirement rows live
srcs = glob.glob(S + r'\mods\**\*.cs', recursive=True) + [S + r'\art\dump.sql']
hits = collections.defaultdict(list)
for p in srcs:
    try:
        lines = open(p, encoding='utf-8', errors='replace').read().split('\n')
    except Exception:
        continue
    for n, line in enumerate(lines, 1):
        for m in re.finditer(r'\((?:NULL|\d+),\s*(\d+),\s*(1634|1635|1636|3042|3043|3044),', line):
            hits[int(m.group(1))].append((p, n))
cl = open(C + r'\data\recipe_requirement.xml', encoding='utf-8').read().split('\n')
chits = collections.defaultdict(list)
for n, line in enumerate(cl, 1):
    m = re.search(r'<RecipeID>(\d+)</RecipeID><MaterialObjectTypeID>(1634|1635|1636|3042|3043|3044)<', line)
    if m: chits[int(m.group(1))].append(n)
# multi-line client rows
for n, line in enumerate(cl, 1):
    m = re.search(r'<MaterialObjectTypeID>(1634|1635|1636|3042|3043|3044)</MaterialObjectTypeID>', line)
    if m and '<RecipeID>' not in line:
        r = re.search(r'<RecipeID>(\d+)</RecipeID>', cl[n - 2])
        if r: chits[int(r.group(1))].append(n)


def rel(p):
    return p.replace(S + '\\', 'Server\\')


GROUPS = [('Schreinern (Carpentry)', lambda r: r['sk'] == 8),
          ('Dekorationen / Kunst (Arts)', lambda r: r['sk'] == 53),
          ('Allgemeine Aktionen (Galgen, Glockentürme, Altäre ...)', lambda r: r['sk'] not in (8, 53))]
out = ['# Rezepte mit Dekorations-Kits (Stand 2026-10-03)', '',
       'Kits: ' + ', '.join('%d = %s' % (k, v) for k, v in KITS.items()), '',
       'Dateien: **Server** = `%s`, **Client** = `%s\\data\\recipe_requirement.xml`.' % (S, C),
       'Zeile = Zeile der Kit-Anforderung (`recipe_requirement`). Änderungen an `mod.cs` brauchen einen Server-Neustart, die Client-XML muss gleich bleiben.', '']
total = 0
for title, pred in GROUPS:
    sel = [(rid, r) for rid, r in recipes.items() if pred(r)]
    if not sel: continue
    out += ['## %s (%d Rezepte)' % (title, len(sel)), '',
            '| Rezept | Name (DE) | Ergebnis | Kits | Server-Datei:Zeile | Client-Zeile |', '|---|---|---|---|---|---|']
    for rid, r in sel:
        total += 1
        kits = ', '.join('%dx %d' % (q, k) for k, q in r['kits'])
        de = rn.get(rid) or on.get(r['res']) or r['name'] + ' (EN)'
        srv = '<br>'.join('`%s:%d`' % (rel(p), n) for p, n in hits.get(rid, [])) or '-'
        cli = ', '.join(str(n) for n in chits.get(rid, [])) or '-'
        out.append('| %d | %s | %d | %s | %s | %s |' % (rid, de, r['res'], kits, srv, cli))
    out.append('')
out.insert(2, '**%d Rezepte** brauchen noch mindestens ein Kit.' % total)
open('Rezepte_mit_Dekorations-Kits.md', 'w', encoding='utf-8').write('\n'.join(out))
print(total, 'recipes;', sum(len(v) for v in hits.values()), 'server rows;', sum(len(v) for v in chits.values()), 'client rows')
