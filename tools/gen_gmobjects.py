"""Regenerate mod/GMHelper/gmobjects.cs from the CURRENT client data (objects_types.xml + cm_objects.xml).
Rules (same as the 2026-09-25 file): buildings = IsUnmovableobject with a 3D model, objects = IsMovableObject with a 3D model,
items = leaf object types without a 3D placeable model that have an inventory icon. Commented-out rows (<!-- -->) never appear.
Entries that were in the old file and still exist keep their (English) name; hidden ranges stay hidden."""
import re, sys, os, html

C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
OLD = C + r'\mod\GMHelper\gmobjects.cs'
OUT = sys.argv[1] if len(sys.argv) > 1 else OLD

# hidden on purpose (removed recipes/content), from the old header
HIDE = set(range(2461, 2467)) | set(range(2359, 2363)) | {2399} | set(range(2940, 2962)) | set(range(3562, 3574))

t = open(C + r'\data\objects_types.xml', encoding='utf-8').read()
t = re.sub(r'<!--.*?-->', '', t, flags=re.S)
types = {}
for m in re.finditer(r'<row>(.*?)</row>', t, re.S):
    b = m.group(1)
    def g(tag):
        r = re.search(r'<%s>(.*?)</%s>' % (tag, tag), b, re.S)
        return r.group(1).strip() if r else ''
    i = int(g('ID'))
    types[i] = dict(parent=int(g('ParentID') or 0), name=html.unescape(g('Name')), mov=g('IsMovableObject') == '1',
                    unm=g('IsUnmovableobject') == '1', face=g('FaceImage'))
parents = {v['parent'] for v in types.values()}

cm = open(C + r'\data\cm_objects.xml', encoding='utf-8').read()
cm = re.sub(r'<!--.*?-->', '', cm, flags=re.S)
with_model = set(int(x) for x in re.findall(r'<object id="(\d+)"[^>]*>', cm))

# old entries: keep their names
old = {}
if os.path.exists(OLD):
    for m in re.finditer(r'\$GMObj::([bmi])\[\d+\] = "(\d+) (.*)";', open(OLD, encoding='utf-8').read()):
        old[(m.group(1), int(m.group(2)))] = m.group(3)

b, m_, it = [], [], []
for i, v in types.items():
    if i in HIDE or not v['name']:
        continue
    if v['unm'] and i in with_model:
        b.append((i, v['name']))
    elif v['mov'] and i in with_model:
        m_.append((i, v['name']))
    elif i not in with_model and i not in parents and v['face'] and not v['mov'] and not v['unm']:
        it.append((i, v['name']))


def sortkey(e):
    return (e[1].lower(), e[0])


out = []
out.append('// Generated 2026-10-24 from the client\'s cm_objects.xml + objects_types.xml (commented-out rows excluded): every object type that has a 3D model.')
out.append('// Buildings = IsUnmovableobject, objects = IsMovableObject. Entry format: <id> <name>')
out.append('// Items = leaf object types without a 3D placeable model that have an inventory icon (what /ADD can give). Entry format: <id> <name>')
out.append('// Hidden on purpose (removed recipes/content): Knool weapons 2461-2466, Knool carcasses 2359-2362 2399, Tribe Camp decorations 2940-2961, case-hardened/toughened bars+ingots 3562-3573 (gen_gmobjects.py).')
for kind, lst in (('b', b), ('m', m_), ('i', it)):
    lst.sort(key=sortkey)
    out.append('$GMObj::%sCount = %d;' % (kind, len(lst)))
    for n, (i, name) in enumerate(lst):
        name = old.get((kind, i), name).replace('"', "'")
        out.append('$GMObj::%s[%d] = "%d %s";' % (kind, n, i, name))
out.append('$GMObj::bCount = %d;' % len(b))
out.append('$GMObj::mCount = %d;' % len(m_))
out.append('$GMObj::iCount = %d;' % len(it))
open(OUT, 'w', encoding='utf-8', newline='\r\n').write('\n'.join(out) + '\n')
print('buildings', len(b), 'objects', len(m_), 'items', len(it), '->', OUT)
if os.path.exists(OLD):
    oldb = {i for (k, i) in old if k == 'b'}; oldm = {i for (k, i) in old if k == 'm'}; oldi = {i for (k, i) in old if k == 'i'}
    nb = {i for i, _ in b}; nm = {i for i, _ in m_}; ni = {i for i, _ in it}
    print('buildings: +%d -%d | objects: +%d -%d | items: +%d -%d' % (len(nb - oldb), len(oldb - nb), len(nm - oldm), len(oldm - nm), len(ni - oldi), len(oldi - ni)))
    print('removed items sample:', sorted(oldi - ni)[:15])
    print('added items sample:', sorted(ni - oldi)[:15])
