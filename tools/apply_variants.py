"""Clothing colour variants (Rags / North Rags / Steppe Rags / Simple Clothes) as standalone items.
Idempotent-ish: run once on a clean state (backups are written first). See README in this folder."""
import os, re, shutil, subprocess, glob, sys

CLIENT = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
SERVER = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
SRC = r'C:\Users\MJZej\OneDrive\Desktop\Clothing'
WORK = r'E:\ClaudeScratch\project_archive\Session_2026-10-25\ClothingVariants'
BK = r'E:\ClaudeScratch\backups\clothvar_20261025'
MYSQL = r'C:\Program Files\MariaDB 12.0\bin\mysql.exe'
os.makedirs(BK, exist_ok=True)
DRY = '--dry' in sys.argv

# id, base item, German name, English name, male mesh base, male suffix, female mesh base, female suffix, kind
# suffix None = that gender wears the plain base mesh
ITEMS = [
    (3995, 297, 'Lumpen Dunkelgrün', 'Rags Dunkelgruen', 'Male_Tatters_Eur', 'Dunkelgruen', 'Female_Tatters_Eur', None, 'weaver'),
    (3996, 297, 'Lumpen Edel', 'Rags Edel', 'Male_Tatters_Eur', 'Edel', 'Female_Tatters_Eur', 'Edel', 'workshop'),
    (3997, 1137, 'Wikingerlumpen Grün', 'North Rags Gruen', 'Male_Tatters_Vik', 'Gruen', 'Female_Tatters_Vik', None, 'weaver'),
    (3998, 1137, 'Wikingerlumpen Blau', 'North Rags Blau', 'Male_Tatters_Vik', None, 'Female_Tatters_Vik', 'Blau', 'weaver'),
    (3999, 1137, 'Wikingerlumpen Edel', 'North Rags Edel', 'Male_Tatters_Vik', 'Edel', 'Female_Tatters_Vik', 'Edel', 'workshop'),
    (4000, 1138, 'Steppenlumpen Dunkelbraun', 'Steppe Rags Dunkelbraun', 'Male_Tatters_Mon', 'Dunkelbraun', 'Female_Tatters_Mon', None, 'weaver'),
    (4001, 1138, 'Steppenlumpen Orange', 'Steppe Rags Orange', 'Male_Tatters_Mon', None, 'Female_Tatters_Mon', 'Orange', 'weaver'),
    (4002, 1138, 'Steppenlumpen Edel', 'Steppe Rags Edel', 'Male_Tatters_Mon', 'Edel', 'Female_Tatters_Mon', 'Edel', 'workshop'),
    (4003, 299, 'Einfache Kleidung Blau', 'Simple Clothes Blau', 'Male_Peasant', 'Blau', 'Female_Peasant', None, 'weaver'),
]
RECIPE0 = 6700
# source file token per (mesh base, suffix): regex that the file name must match (umlauts differ in encoding)
TOKEN = {'Dunkelgruen': r'Dunkelgr', 'Gruen': r'_Gr', 'Dunkelbraun': r'Dunk(el|le)braun', 'Blau': r'Blau', 'Orange': r'Orange', 'Edel': r'Edel'}
# base ingredients (material, quality, influence, quantity) of the normal recipes
BASE_REQ = {297: [(1477, 0, 40, 6)], 1137: [(1477, 0, 40, 6)], 1138: [(1477, 0, 40, 6)],
            299: [(261, 0, 15, 5), (260, 0, 15, 6), (425, 0, 10, 3)]}
BASE_ICON = {297: 'rags_eur.png', 1137: 'rags_vik.png', 1138: 'rags_mon.png', 299: 'simple_clothes.png'}
DYE = 3982


def rd(p):
    return open(p, 'rb').read().decode('utf-8-sig')


def wr(p, t):
    if DRY:
        return
    open(p, 'wb').write(t.encode('utf-8'))


def bak(p):
    d = os.path.join(BK, p.replace(':', '').replace('\\', '_'))
    if not os.path.exists(d):
        shutil.copyfile(p, d)


def nl_of(t):
    return '\r\n' if '\r\n' in t else '\n'


def meshname(base, suf):
    return base + '_' + suf if suf else base


# ------------------------------------------------------------------ 1) textures
tex_dir = os.path.join(CLIENT, 'art', 'Textures', 'CharacterTextures', 'Outfits', 'Variants')
os.makedirs(tex_dir, exist_ok=True)
variants = []   # (gender, base mesh, suffix)
for it in ITEMS:
    if it[5]:
        variants.append(('male', it[4], it[5]))
    if it[7]:
        variants.append(('female', it[6], it[7]))
for g, base, suf in variants:
    folder = os.path.join(SRC, 'Female' if g == 'female' else 'Male')
    got = {}
    for f in os.listdir(folder):
        if not f.startswith(base) or not re.search(TOKEN[suf], f):
            continue
        for kind in ('DIFFUSE', 'NORMALMAP', 'SPECULAR'):
            if kind in f:
                got[kind] = f
    if set(got) != {'DIFFUSE', 'NORMALMAP', 'SPECULAR'}:
        raise SystemExit('missing maps for %s %s: %s' % (base, suf, got))
    for kind, f in got.items():
        dst = os.path.join(tex_dir, '%s_%s.dds' % (meshname(base, suf), kind))
        if not DRY:
            shutil.copyfile(os.path.join(folder, f), dst)
    print('textures', meshname(base, suf), '<-', got)

# ------------------------------------------------------------------ 2) materials (own file, exec'd from materials.cs)
mp = os.path.join(CLIENT, 'art', 'materials.cs')
fp = os.path.join(CLIENT, 'art', 'female_materials.cs')
mt, ft = rd(mp), rd(fp)


def template(text, basemesh):
    m = re.search(r'Material\(\w+\)[^\r\n]*\r?\n\{\s*(?:skinned = true;\s*)?mapTo = "%s_DIFFUSE";.*?\n\};' % re.escape(basemesh), text, re.S)
    if not m:
        m = re.search(r'singleton Material\(\w+\)\r?\n\{\r?\n   mapTo = "%s_DIFFUSE";.*?\n\};' % re.escape(basemesh), text, re.S)
    if not m:
        raise SystemExit('no material template for ' + basemesh)
    return m.group(0)


blocks = ['// Clothing colour variants (2026-10-25). One material per cloned mesh object (see DtsTools spec).']
for g, base, suf in variants:
    mesh = meshname(base, suf)
    t = template(ft if g == 'female' else mt, base)
    body = re.sub(r'Material\(\w+\)[^\r\n]*', 'Material(LiFx_Variant_%s)' % mesh, t, count=1)
    body = re.sub(r'mapTo = "[^"]*";', 'mapTo = "%s_DIFFUSE";' % mesh, body)
    body = re.sub(r'diffuseMap\[0\] = "[^"]*";', 'diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/Variants/%s_DIFFUSE.dds";' % mesh, body)
    body = re.sub(r'diffuseMap\[1\] = "[^"]*";', 'diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/Variants/%s_NORMALMAP.dds";' % mesh, body)
    body = re.sub(r'diffuseMap\[2\] = "[^"]*";', 'diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/Variants/%s_SPECULAR.dds";' % mesh, body)
    if not body.lstrip().startswith('singleton'):
        body = 'singleton ' + body.lstrip()
    blocks.append(body)
vp = os.path.join(CLIENT, 'art', 'materials_variants.cs')
wr(vp, '\r\n'.join(blocks) + '\r\n')
print('materials_variants.cs:', len(blocks) - 1, 'materials')

# materials.cs: exec the new file, drop the old skin test material
bak(mp)
nl = nl_of(mt)
if 'materials_variants.cs' not in mt:
    mt = mt.replace('exec("art/female_materials.cs");', 'exec("art/female_materials.cs");' + nl + 'exec("art/materials_variants.cs");', 1)
mt2 = re.sub(r'\r?\nsingleton Material\(LiFx_Recolor_Male_Peasant_SkinB\)\r?\n\{.*?\n\};[ \t]*', '', mt, flags=re.S)
print('old test material removed:', mt2 != mt)
wr(mp, mt2)

# ------------------------------------------------------------------ 3) cm_equipTypes (client + dedicated server)
def object_block(text, oid):
    m = re.search(r'<object id="%d">.*?</object>' % oid, text, re.S)
    return m.group(0)


for path in (os.path.join(CLIENT, 'data', 'cm_equipTypes.xml'), os.path.join(SERVER, 'data', 'cm_equipTypes.xml')):
    t = rd(path)
    bak(path)
    nl = nl_of(t)
    if '<object id="3995">' in t:
        print('equipTypes already patched:', path)
        continue
    # remove the old test skin (skin 990101 = Simple Clothes blue) from item 299
    t = re.sub(r'\r?\n[ \t]*<skin id="990101".*?</skin>', '', t, count=1, flags=re.S)
    new = []
    for iid, base, de, en, mb, ms, fb, fs, kind in ITEMS:
        blk = object_block(t, base)
        blk = re.sub(r'<object id="%d">' % base, '<object id="%d">' % iid, blk, count=1)
        blk = re.sub(r'<!--name = [^>]*-->', '<!--name = %s -->' % en, blk, count=1)
        blk = re.sub(r'\r?\n[ \t]*<skins>.*?</skins>', '', blk, flags=re.S)
        blk = blk.replace('<mesh>%s</mesh>' % mb, '<mesh>%s</mesh>' % meshname(mb, ms), 1)
        blk = blk.replace('<mesh>%s</mesh>' % fb, '<mesh>%s</mesh>' % meshname(fb, fs), 1)
        new.append(blk)
    t = t.replace('</equipment_types>', nl.join(new) + nl + '</equipment_types>')
    wr(path, t)
    print('equipTypes patched:', path, '(+%d objects, test skin removed)' % len(new))

# ------------------------------------------------------------------ 4) database rows (baked into art\dump.sql tail) + client xml
def mysql(sql):
    r = subprocess.run([MYSQL, '-uroot', '-p'+os.environ['LIF_DB_PW'], '-D', 'lif_1', '-N', '-B', '-e', sql], capture_output=True, text=True, encoding='utf-8')
    return [l.split('\t') for l in r.stdout.splitlines()]


cols = mysql("select column_name,data_type from information_schema.columns where table_schema='lif_1' and table_name='objects_types' order by ordinal_position")
rows = {b: mysql('select * from objects_types where ID=%d' % b)[0] for b in (297, 299, 1137, 1138)}


def sqlval(v, dt):
    if v == 'NULL':
        return 'NULL'
    if dt in ('int', 'tinyint', 'smallint'):
        return v
    return "'" + v.replace('\\\\', '/').replace("'", "''") + "'"


sql = ['', '-- Clothing colour variants (2026-10-25): items 3995-4003, recipes 6700-6708', ]
items_sql, rec_sql, req_sql = [], [], []
for n, (iid, base, de, en, mb, ms, fb, fs, kind) in enumerate(ITEMS):
    r = list(rows[base]); r[0] = str(iid); r[2] = en
    items_sql.append('INSERT IGNORE INTO `objects_types` VALUES (%s);' % ','.join(sqlval(v, c[1]) for v, c in zip(r, cols)))
    rid = RECIPE0 + n
    tool = 295 if kind == 'weaver' else 2836
    rec_sql.append("INSERT IGNORE INTO `recipe` VALUES (%d,'%s','',%d,25,60,%d,50,1,0,0,'art/2D/Items/%s');" % (rid, en, tool, iid, BASE_ICON[base]))
    reqs = list(BASE_REQ[base]) + [(DYE, 0, 0, 1 if kind == 'weaver' else 3)]
    reqs.append((295, 0, 10, 30) if kind == 'weaver' else (2836, 0, 20, 40))
    for mat, q, infl, qty in reqs:
        req_sql.append('INSERT INTO `recipe_requirement` VALUES (NULL,%d,%d,%d,%d,%d,0);' % (rid, mat, q, infl, qty))
rid_list = ','.join(str(RECIPE0 + i) for i in range(len(ITEMS)))
sql += items_sql + rec_sql + ['DELETE FROM `recipe_requirement` WHERE RecipeID IN (%s);' % rid_list] + req_sql
dp = os.path.join(SERVER, 'art', 'dump.sql')
d = rd(dp)
if 'Clothing colour variants' in d:
    print('dump.sql already has the variants')
else:
    bak(dp)
    nl = nl_of(d)
    wr(dp, d.rstrip() + nl + nl.join(sql) + nl)
    print('dump.sql: +%d statements' % (len(sql) - 2))

# client xml: objects_types, recipe, recipe_requirement
op = os.path.join(CLIENT, 'data', 'objects_types.xml')
o = rd(op); bak(op)
if '<ID>3995</ID>' not in o:
    nl = nl_of(o); add = []
    for iid, base, de, en, *_ in ITEMS:
        m = re.search(r'<row>\s*<ID>%d</ID>.*?</row>' % base, o, re.S).group(0)
        m = m.replace('<ID>%d</ID>' % base, '<ID>%d</ID>' % iid, 1)
        m = re.sub(r'<Name>[^<]*</Name>', '<Name>%s</Name>' % en, m, count=1)
        add.append('\t' + m)
    o = o.replace('</table>', nl.join(add) + nl + '</table>') if '</table>' in o else o
    wr(op, o)
    print('client objects_types.xml +%d rows' % len(add))

rp = os.path.join(CLIENT, 'data', 'recipe.xml')
r = rd(rp); bak(rp)
if '<ID>6700</ID>' not in r:
    nl = nl_of(r); add = []
    for n, (iid, base, de, en, mb, ms, fb, fs, kind) in enumerate(ITEMS):
        tool = 295 if kind == 'weaver' else 2836
        add.append('\t<row>\n\t\t<ID>%d</ID>\n\t\t<Name>%s</Name>\n\t\t<Description></Description>\n\t\t<StartingToolsID>%d</StartingToolsID>\n\t\t<SkillTypeID>25</SkillTypeID>\n\t\t<SkillLvl>60</SkillLvl>\n\t\t<ResultObjectTypeID>%d</ResultObjectTypeID>\n\t\t<SkillDepends>50</SkillDepends>\n\t\t<Quantity>1</Quantity>\n\t\t<Autorepeat>0</Autorepeat>\n\t\t<IsBlueprint>0</IsBlueprint>\n\t\t<ImagePath>art\\2D\\Items\\%s</ImagePath>\n\t</row>'.replace('\n', nl) % (RECIPE0 + n, en, tool, iid, BASE_ICON[base]))
    r = r.replace('</table>', nl.join(add) + nl + '</table>')
    wr(rp, r)
    print('client recipe.xml +%d rows' % len(add))

qp = os.path.join(CLIENT, 'data', 'recipe_requirement.xml')
q = rd(qp); bak(qp)
if '<RecipeID>6700</RecipeID>' not in q:
    nl = nl_of(q); add = []; nid = 300920
    for n, (iid, base, de, en, mb, ms, fb, fs, kind) in enumerate(ITEMS):
        reqs = list(BASE_REQ[base]) + [(DYE, 0, 0, 1 if kind == 'weaver' else 3)]
        reqs.append((295, 0, 10, 30) if kind == 'weaver' else (2836, 0, 20, 40))
        for mat, qq, infl, qty in reqs:
            add.append('\t<row><ID>%d</ID><RecipeID>%d</RecipeID><MaterialObjectTypeID>%d</MaterialObjectTypeID><Quality>%d</Quality><Influence>%d</Influence><Quantity>%d</Quantity><IsRegionItemRequired>0</IsRegionItemRequired></row>' % (nid, RECIPE0 + n, mat, qq, infl, qty)); nid += 1
    q = q.replace('</table>', nl.join(add) + nl + '</table>')
    wr(qp, q)
    print('client recipe_requirement.xml +%d rows' % len(add))

# ------------------------------------------------------------------ 5) German names + remove the old test texts
loc = os.path.join(CLIENT, 'data', 'loc', 'de', 'data')
for fn, idsnames in (('objects_types_Name.xml', [(i[0], i[2]) for i in ITEMS]), ('recipe_Name.xml', [(RECIPE0 + n, i[2]) for n, i in enumerate(ITEMS)])):
    p = os.path.join(loc, fn); t = rd(p); bak(p)
    nl = nl_of(t)
    add = ''.join('<string id="%d">%s</string>%s' % (i, n, nl) for i, n in idsnames if '<string id="%d">' % i not in t)
    if add:
        t = t.replace('</strings>', add + '</strings>')
        wr(p, t)
        print(fn, '+', add.count('<string'))
n = 0
for p in glob.glob(os.path.join(CLIENT, 'data', 'loc', '*', 'data', 'cm_messages.xml')):
    t = rd(p)
    t2 = re.sub(r'<string id="99000[12]">.*?</string>\r?\n?', '', t, flags=re.S)
    if t2 != t:
        bak(p); wr(p, t2); n += 1
print('test skin texts removed from', n, 'loc files')

# old test files
for f in (r'art\2D\equipIcons\Outfits\Male_Peasant_SkinB.png', r'art\2D\equipIcons\Outfits\Male_Peasant_SkinB_h.png',
          r'art\Textures\CharacterTextures\Outfits\Simple\Male_Peasant_Blue_DIFFUSE.dds', r'art\Textures\CharacterTextures\Outfits\Simple\Male_Peasant_Blue_NORMALMAP.dds',
          r'art\Textures\CharacterTextures\Outfits\Simple\Male_Peasant_Blue_SPECULAR.dds'):
    p = os.path.join(CLIENT, f)
    if os.path.exists(p):
        bak(p)
        if not DRY:
            os.remove(p)
        print('removed test file', f)
print('DONE' + (' (dry run)' if DRY else ''))
