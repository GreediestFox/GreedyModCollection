"""Female underwear as craftable items; female characters spawn without underwear (body mesh + Female_Body_v1 texture only).
Items 4004-4007, recipes 6709-6712. Uses the same pipeline as apply_variants.py."""
import os, re, shutil, subprocess, json, glob, sys

CLIENT = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
SERVER = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
SRC = r'C:\Users\MJZej\OneDrive\Desktop\Clothing\Female'
WORK = r'E:\ClaudeScratch\project_archive\Session_2026-10-25\ClothingVariants'
BK = r'E:\ClaudeScratch\backups\underwear_20261025'
MYSQL = r'C:\Program Files\MariaDB 12.0\bin\mysql.exe'
DTS = r'E:\ClaudeScratch\project_archive\DtsTools\dts_clone.py'
PRISTINE = r'E:\SteamLibrary\steamapps\common\Life is Feudal MMO\game\eu\art\models\3d\mobiles\characters'
os.makedirs(BK, exist_ok=True)

# id, German name, English name, body mesh, base item row to copy, recipe dye amount
ITEMS = [
    (4004, 'Damenunterwäsche Gottlung', 'Female Underwear Gottlung', 'Female_Underwear_Eur_Body', 0),
    (4005, 'Damenunterwäsche Wikinger', 'Female Underwear Vikings', 'Female_Underwear_Vik_Body', 0),
    (4006, 'Damenunterwäsche Steppe', 'Female Underwear Steppe', 'Female_Underwear_Mon_Body', 0),
    (4007, 'Damenunterwäsche Wikinger Grün', 'Female Underwear Vikings Gruen', 'Female_Underwear_Vik_Gruen_Body', 1),
]
RECIPE0 = 6709
DYE = 3982


def rd(p):
    return open(p, 'rb').read().decode('utf-8-sig')


def wr(p, t):
    open(p, 'wb').write(t.encode('utf-8'))


def bak(p):
    d = os.path.join(BK, p.replace(':', '').replace('\\', '_'))
    if not os.path.exists(d):
        shutil.copyfile(p, d)


def nl_of(t):
    return '\r\n' if '\r\n' in t else '\n'


# ---------------------------------------------------------------- 1) dts clone (green Vik underwear) -> female.dts in client art + dedicated server art
spec_p = os.path.join(WORK, 'spec_female.json')
spec = json.load(open(spec_p))
if not any(c['name'] == 'Female_Underwear_Vik_Gruen_Body' for c in spec['clones']):
    spec['clones'].append({'from': 'Female_Underwear_Vik_Body', 'name': 'Female_Underwear_Vik_Gruen_Body',
                           'materials': {'Female_Underwear_Vik_DIFFUSE': 'Female_Underwear_Vik_Gruen_DIFFUSE'}})
    json.dump(spec, open(spec_p, 'w'), indent=1)
out = os.path.join(WORK, 'female_new2.dts')
r = subprocess.run([sys.executable, '-I', DTS, 'build', os.path.join(PRISTINE, 'female.dts'), out, spec_p], capture_output=True, text=True)
print('dts build:', r.stdout.strip()[-200:], r.stderr.strip()[-200:])
chk = subprocess.run([sys.executable, '-I', DTS, 'check', out], capture_output=True, text=True)
print(chk.stdout.strip()[:300], chk.stderr.strip()[:300])
assert chk.returncode == 0 and os.path.getsize(out) > 1000
for root in (os.path.join(CLIENT, 'art', 'models', '3d', 'mobiles', 'characters'), os.path.join(SERVER, 'art', 'models', '3d', 'mobiles', 'characters')):
    dst = os.path.join(root, 'female.dts')
    bak(dst)
    shutil.copyfile(out, dst)
    print('deployed', dst, os.path.getsize(dst))

# ---------------------------------------------------------------- 2) textures + material
tex_dir = os.path.join(CLIENT, 'art', 'Textures', 'CharacterTextures', 'Outfits', 'Variants')
for kind in ('DIFFUSE', 'NORMALMAP', 'SPECULAR'):
    f = [x for x in os.listdir(SRC) if x.startswith('Female_Underwear_Vik') and kind in x][0]
    shutil.copyfile(os.path.join(SRC, f), os.path.join(tex_dir, 'Female_Underwear_Vik_Gruen_%s.dds' % kind))
vp = os.path.join(CLIENT, 'art', 'materials_variants.cs')
vt = rd(vp); bak(vp)
if 'Female_Underwear_Vik_Gruen' not in vt:
    blk = '''singleton Material(LiFx_Variant_Female_Underwear_Vik_Gruen)
{
   skinned = true;
   mapTo = "Female_Underwear_Vik_Gruen_DIFFUSE";
   diffuseMap[0] = "art/Textures/CharacterTextures/Outfits/Variants/Female_Underwear_Vik_Gruen_DIFFUSE.dds";
   diffuseMap[1] = "art/Textures/CharacterTextures/Outfits/Variants/Female_Underwear_Vik_Gruen_NORMALMAP.dds";
   diffuseMap[2] = "art/Textures/CharacterTextures/Outfits/Variants/Female_Underwear_Vik_Gruen_SPECULAR.dds";
   normal3DC="1";
   materialTag0 = "LiF";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "180";
   mipLODBias = -1.2;
   useAnisotropic[0] = "1";
};
'''.replace('\n', '\r\n')
    wr(vp, vt.rstrip() + '\r\n' + blk)
    print('material added')

# ---------------------------------------------------------------- 3) cm_equipTypes: the 4 underwear items (client + dedicated server)
BLOCK = '''<object id="%(id)d">
   <!--name = %(en)s -->
   <skillID>62</skillID>
   <skillAmount>0</skillAmount>
   <slots>
      <slot>2</slot>
   </slots>
   <extraSlotsBlock>
      <slot>5</slot>
   </extraSlotsBlock>
   <meshes>
      <mesh>%(mesh)s</mesh>
   </meshes>
   <armor>
      <multiplier type="slash" value="0.02" />
      <multiplier type="pierce" value="0.03" />
      <multiplier type="chop" value="0.02" />
      <multiplier type="blunt" value="0.03" />
      <multiplier type="siege" value="0.04" />
   </armor>
</object>'''
for path in (os.path.join(CLIENT, 'data', 'cm_equipTypes.xml'), os.path.join(SERVER, 'data', 'cm_equipTypes.xml')):
    t = rd(path); bak(path); nl = nl_of(t)
    if '<object id="4004">' in t:
        print('equipTypes already patched', path); continue
    add = nl.join((BLOCK % dict(id=i, en=en, mesh=mesh)).replace('\n', nl) for i, de, en, mesh, dye in ITEMS)
    wr(path, t.replace('</equipment_types>', add + nl + '</equipment_types>'))
    print('equipTypes +4:', path)

# ---------------------------------------------------------------- 4) database (dump.sql tail) + client xml
def mysql(sql):
    r = subprocess.run([MYSQL, '-uroot', '-p'+os.environ['LIF_DB_PW'], '-D', 'lif_1', '-N', '-B', '-e', sql], capture_output=True, text=True, encoding='utf-8')
    return [l.split('\t') for l in r.stdout.splitlines()]


cols = mysql("select column_name,data_type from information_schema.columns where table_schema='lif_1' and table_name='objects_types' order by ordinal_position")
tmpl = mysql('select * from objects_types where ID=299')[0]


def sqlval(v, dt):
    if v == 'NULL':
        return 'NULL'
    if dt in ('int', 'tinyint', 'smallint'):
        return v
    return "'" + v.replace('\\\\', '/').replace("'", "''") + "'"


sql = ['', '-- Female underwear items 4004-4007, recipes 6709-6712 (2026-10-25)']
req_lines = []
for n, (iid, de, en, mesh, dye) in enumerate(ITEMS):
    r = list(tmpl); r[0] = str(iid); r[2] = en
    sql.append('INSERT IGNORE INTO `objects_types` VALUES (%s);' % ','.join(sqlval(v, c[1]) for v, c in zip(r, cols)))
rids = ','.join(str(RECIPE0 + i) for i in range(len(ITEMS)))
for n, (iid, de, en, mesh, dye) in enumerate(ITEMS):
    sql.append("INSERT IGNORE INTO `recipe` VALUES (%d,'%s','',295,25,0,%d,25,1,0,0,'art/2D/Items/simple_clothes.png');" % (RECIPE0 + n, en, iid))
sql.append('DELETE FROM `recipe_requirement` WHERE RecipeID IN (%s);' % rids)
REQ = {}
for n, (iid, de, en, mesh, dye) in enumerate(ITEMS):
    reqs = [(1477, 0, 40, 3)] + ([(DYE, 0, 0, dye)] if dye else []) + [(295, 0, 10, 30)]
    REQ[RECIPE0 + n] = reqs
    for mat, q, infl, qty in reqs:
        sql.append('INSERT INTO `recipe_requirement` VALUES (NULL,%d,%d,%d,%d,%d,0);' % (RECIPE0 + n, mat, q, infl, qty))
dp = os.path.join(SERVER, 'art', 'dump.sql'); d = rd(dp)
if 'Female underwear items' not in d:
    bak(dp); nl = nl_of(d)
    wr(dp, d.rstrip() + nl + nl.join(sql) + nl)
    print('dump.sql +%d statements' % (len(sql) - 2))

op = os.path.join(CLIENT, 'data', 'objects_types.xml'); o = rd(op); bak(op)
if '<ID>4004</ID>' not in o:
    nl = nl_of(o); base = re.search(r'<row>\s*<ID>299</ID>.*?</row>', o, re.S).group(0); add = []
    for iid, de, en, mesh, dye in ITEMS:
        m = base.replace('<ID>299</ID>', '<ID>%d</ID>' % iid, 1)
        m = re.sub(r'<Name>[^<]*</Name>', '<Name>%s</Name>' % en, m, count=1)
        add.append('\t' + m)
    wr(op, o.replace('</table>', nl.join(add) + nl + '</table>'))
rp = os.path.join(CLIENT, 'data', 'recipe.xml'); r_ = rd(rp); bak(rp)
if '<ID>6709</ID>' not in r_:
    nl = nl_of(r_); add = []
    for n, (iid, de, en, mesh, dye) in enumerate(ITEMS):
        add.append(('\t<row>\n\t\t<ID>%d</ID>\n\t\t<Name>%s</Name>\n\t\t<Description></Description>\n\t\t<StartingToolsID>295</StartingToolsID>\n\t\t<SkillTypeID>25</SkillTypeID>\n\t\t<SkillLvl>0</SkillLvl>\n\t\t<ResultObjectTypeID>%d</ResultObjectTypeID>\n\t\t<SkillDepends>25</SkillDepends>\n\t\t<Quantity>1</Quantity>\n\t\t<Autorepeat>0</Autorepeat>\n\t\t<IsBlueprint>0</IsBlueprint>\n\t\t<ImagePath>art\\2D\\Items\\simple_clothes.png</ImagePath>\n\t</row>' % (RECIPE0 + n, en, iid)).replace('\n', nl))
    wr(rp, r_.replace('</table>', nl.join(add) + nl + '</table>'))
qp = os.path.join(CLIENT, 'data', 'recipe_requirement.xml'); q = rd(qp); bak(qp)
if '<RecipeID>6709</RecipeID>' not in q:
    nl = nl_of(q); nid = max(int(x) for x in re.findall(r'<ID>(\d+)</ID>', q)) + 1; add = []
    for rid, reqs in REQ.items():
        for mat, qq, infl, qty in reqs:
            add.append('\t<row><ID>%d</ID><RecipeID>%d</RecipeID><MaterialObjectTypeID>%d</MaterialObjectTypeID><Quality>%d</Quality><Influence>%d</Influence><Quantity>%d</Quantity><IsRegionItemRequired>0</IsRegionItemRequired></row>' % (nid, rid, mat, qq, infl, qty)); nid += 1
    wr(qp, q.replace('</table>', nl.join(add) + nl + '</table>'))
loc = os.path.join(CLIENT, 'data', 'loc', 'de', 'data')
for fn, pairs in (('objects_types_Name.xml', [(i[0], i[1]) for i in ITEMS]), ('recipe_Name.xml', [(RECIPE0 + n, i[1]) for n, i in enumerate(ITEMS)])):
    p = os.path.join(loc, fn); t = rd(p); bak(p); nl = nl_of(t)
    add = ''.join('<string id="%d">%s</string>%s' % (i, n, nl) for i, n in pairs if '<string id="%d">' % i not in t)
    if add:
        wr(p, t.replace('</strings>', add + '</strings>'))
print('client xml + loc done')

# ---------------------------------------------------------------- 5) customisation: female characters spawn without underwear (client + server copy)
for path in (os.path.join(CLIENT, 'data', 'cm_customisation.xml'), os.path.join(SERVER, 'data', 'cm_customisation.xml')):
    t = rd(path); bak(path)
    n0 = len(re.findall(r'Female_Underwear', t))
    t2 = re.sub(r'[ \t]*<(?:helmet_)?mesh>Female_Underwear_[A-Za-z_]+</(?:helmet_)?mesh>[ \t]*\r?\n', '', t)
    wr(path, t2)
    print('customisation: underwear references %d -> %d in %s' % (n0, len(re.findall(r'Female_Underwear', t2)), path))
print('DONE')
