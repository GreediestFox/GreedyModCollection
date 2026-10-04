"""List EKR modpack content we do not have yet (by name), grouped by EKR category; writes a German markdown list."""
import re, os, subprocess, collections
K = r'E:\ClaudeScratch\ekr2\EKRModpack'
M = r'C:\Program Files\MariaDB 12.0\bin\mysql.exe'
OUT = 'EKR_fehlende_Inhalte.md'


def rows(path, keys):
    t = open(path, encoding='utf-8-sig', errors='replace').read()
    return [{k: (re.search(r'<%s>([^<]*)</%s>' % (k, k), m.group(1)) or [None, ''])[1] for k in keys} for m in re.finditer(r'<row>(.*?)</row>', t, re.S)]


kot = rows(K + r'\data\objects_types.xml', ['ID', 'ParentID', 'Name', 'FaceImage', 'IsMovableObject', 'IsUnmovableobject'])
kid = {r['ID']: r for r in kot}
krec = rows(K + r'\data\recipe.xml', ['ID', 'Name', 'ResultObjectTypeID', 'SkillTypeID', 'SkillLvl'])
rec_by_res = collections.defaultdict(list)
for r in krec: rec_by_res[r['ResultObjectTypeID']].append(r)
cmo = open(K + r'\data\cm_objects.xml', encoding='utf-8-sig', errors='replace').read()
cm_ids = set(re.findall(r'<object id="(\d+)"', cmo))
eq_ids = set(re.findall(r'<object id="(\d+)"', open(K + r'\data\cm_equipTypes.xml', encoding='utf-8-sig', errors='replace').read()))

out = subprocess.run([M, '-uroot', '-p1234', 'lif_1', '-N', '-B', '-e', 'select ID,Name from objects_types'], capture_output=True, text=True, encoding='utf-8').stdout
ours_id = {}; ours_names = set()
for l in out.strip().split('\n'):
    i, n = l.split('\t'); ours_id[i] = n; ours_names.add(n.strip().lower())
skills = dict(l.split('\t') for l in subprocess.run([M, '-uroot', '-p1234', 'lif_1', '-N', '-B', '-e', 'select ID,Name from skill_type'], capture_output=True, text=True, encoding='utf-8').stdout.strip().split('\n'))


def exists(p):
    return bool(p) and os.path.exists(os.path.join(K, p.replace('/', '\\')))


def cat(r):
    # walk up to the first parent that is a "category" name we can show
    chain, cur = [], r['ParentID']
    for _ in range(4):
        p = kid.get(cur)
        if not p: break
        chain.append(p['Name']); cur = p['ParentID']
    return chain[0] if chain else '?'


groups = collections.defaultdict(list)
for r in kot:
    name = r['Name'].strip()
    if not name or r['ParentID'] in ('1902',) or name.lower().startswith('mounted'): continue
    if name.lower() in ours_names: continue
    if r['ID'] in ours_id and ours_id[r['ID']].strip().lower() == name.lower(): continue
    is_cat = any(x['ParentID'] == r['ID'] for x in kot[:0])  # placeholder (categories filtered below)
    recs = rec_by_res.get(r['ID'], [])
    if not recs and r['ID'] not in cm_ids and r['ID'] not in eq_ids and not exists(r['FaceImage']):
        continue  # pure category/placeholder rows without anything behind them
    kind = 'Gebäude/Objekt' if r['IsUnmovableobject'] == '1' else ('beweglich' if r['IsMovableObject'] == '1' else 'Gegenstand')
    model = 'ja' if (r['ID'] in cm_ids or r['ID'] in eq_ids) else '-'
    icon = 'ja' if exists(r['FaceImage']) else '-'
    rec = ', '.join('%s %s' % (skills.get(x['SkillTypeID'], x['SkillTypeID']), x['SkillLvl']) for x in recs) or '-'
    groups[cat(r)].append((int(r['ID']), name, kind, model, icon, rec))

lines = ['# EKR-Modpack: Inhalte, die wir noch nicht haben (Stand 2026-10-04)', '',
         'Quelle: `EKRModpack-20261004T142513Z-1-001.zip` (entpackt in `E:\\ClaudeScratch\\ekr2`). Abgeglichen per Name gegen unseren Server; '
         'bereits portierte EKR-Gebäude, Jorvik- und Knool-Inhalte fehlen hier daher.', '',
         '- **Modell**: Eintrag in `cm_objects`/`cm_equipTypes` des Packs (3D-Modell oder Ausrüstungs-Mesh vorhanden)',
         '- **Icon**: Bilddatei existiert im Pack', '- **Rezept**: Fertigkeit und Stufe aus dem EKR-Rezept', '']
total = 0
for g in sorted(groups, key=lambda g: (-len(groups[g]), g)):
    L = sorted(groups[g]); total += len(L)
    lines += ['## %s (%d)' % (g, len(L)), '', '| EKR-ID | Name | Art | Modell | Icon | Rezept |', '|---|---|---|---|---|---|']
    lines += ['| %d | %s | %s | %s | %s | %s |' % x for x in L]
    lines.append('')
lines.insert(2, '**%d Einträge** in %d Kategorien.' % (total, len(groups)))
open(OUT, 'w', encoding='utf-8').write('\n'.join(lines))
print(total, 'entries,', len(groups), 'groups ->', OUT)
for g in sorted(groups, key=lambda g: -len(groups[g])): print(' ', len(groups[g]), g)
