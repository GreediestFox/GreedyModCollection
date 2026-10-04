"""From the newer EKR modpack (2026-10-04):
 A) real recipe/building images for 8 already-ported EKR buildings (were vanilla placeholders);
 B) EKR's cosmetic fur pieces of the Viking helmets, worn alone in a RING slot (7/8), as new items 3936-3938
    (EKR ids 2933/2939/2940 are not reused), recipes 6488-6490 (EKR: Tailoring 90, Weaver's Toolkit), no armor values."""
import os, re, shutil
S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
K = r'E:\ClaudeScratch\ekr2\EKRModpack'
BK = r'E:\ClaudeScratch\backups\ekr_art_furs_20261004'
os.makedirs(BK, exist_ok=True)


def rd(p):
    raw = open(p, 'rb').read(); return raw.startswith(b'\xef\xbb\xbf'), raw.decode('utf-8-sig')


def wr(p, bom, t): open(p, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))


def bk(p, tag):
    d = os.path.join(BK, tag)
    if not os.path.exists(d): shutil.copy2(p, d)


# ---------------- A) building images ----------------
IMG = {3673: 'lehmhuette.png', 3674: 'Einfacher-Unterstand.png', 3675: 'Lehmhaus.png', 3676: 'Kleines-Lehmhaus.png',
       3679: 'stabile-Lehmhuette.png', 3737: '3113.png', 3739: '3124.png', 3741: '3130.png'}
DST = C + r'\mod\EKRBuildingsMod\art\2D\Recipes'
os.makedirs(DST, exist_ok=True)
for f in set(IMG.values()):
    shutil.copy2(os.path.join(K, r'art\2D\Recipes', f), os.path.join(DST, f))
path = {oid: 'mod/EKRBuildingsMod/art/2D/Recipes/' + f for oid, f in IMG.items()}

for sub in ('art', 'sql'):
    p = S + '\\' + sub + r'\dump.sql'; bom, t = rd(p); bk(p, sub + '_dump.sql')
    for oid, img in path.items():
        m = re.search(r"^\(%d,\d+,'[^']*',[^\r\n]*" % oid, t, re.M)
        row = m.group(0)
        if img in row: continue
        new = re.sub(r",'(art/2D/[^']*)','([^']*)',(\d+|NULL),(\d+|NULL),(\d),(\d)\)", lambda mm: ",'%s','%s',%s,%s,%s,%s)" % (img, mm.group(2), mm.group(3), mm.group(4), mm.group(5), mm.group(6)), row, count=1)
        assert new != row, (sub, oid, row)
        t = t[:m.start()] + new + t[m.end():]
    wr(p, bom, t)
print('dump.sql: FaceImage of 8 buildings')

p = S + r'\mods\LiFx\EkrBuildingsPack\mod.cs'; bom, t = rd(p); bk(p, 'EkrBuildingsPack_mod.cs')
for oid, img in path.items():
    m = re.search(r"INSERT IGNORE INTO `objects_types` VALUES \(%d,[^\r\n]*" % oid, t)
    row = m.group(0)
    new = re.sub(r",'(art/2D/[^']*)','([^']*)',(\d+|NULL),(\d+|NULL),(\d),(\d)\)", lambda mm: ",'%s','%s',%s,%s,%s,%s)" % (img, mm.group(2), mm.group(3), mm.group(4), mm.group(5), mm.group(6)), row, count=1)
    assert new != row, oid
    t = t.replace(row, new, 1)
    m = re.search(r"INSERT IGNORE INTO `recipe` VALUES \((\d+),'[^']*','[^']*',\w+,\d+,\d+,%d,[^\r\n]*" % oid, t)
    row = m.group(0)
    new = re.sub(r",'[^']*'\)\"\);$", ",'%s')\");" % img, row)
    assert new != row, ('recipe', oid)
    t = t.replace(row, new, 1)
wr(p, bom, t); print('EkrBuildingsPack mod.cs: FaceImage + recipe image of 8 buildings')

p = C + r'\data\objects_types.xml'; bom, t = rd(p); bk(p, 'client_objects_types.xml')
for oid, img in path.items():
    m = re.search(r'<row>\s*<ID>%d</ID>.*?</row>' % oid, t, re.S)
    seg = re.sub(r'<FaceImage>.*?</FaceImage>', lambda _: '<FaceImage>%s</FaceImage>' % img.replace('/', '\\'), m.group(0))
    t = t[:m.start()] + seg + t[m.end():]
wr(p, bom, t)
p = C + r'\data\recipe.xml'; bom, t = rd(p); bk(p, 'client_recipe.xml')
for oid, img in path.items():
    m = re.search(r'<row>(?:(?!</row>).)*<ResultObjectTypeID>%d</ResultObjectTypeID>.*?</row>' % oid, t, re.S)
    seg = re.sub(r'<ImagePath>.*?</ImagePath>', lambda _: '<ImagePath>%s</ImagePath>' % img.replace('/', '\\'), m.group(0))
    t = t[:m.start()] + seg + t[m.end():]
wr(p, bom, t); print('client objects_types + recipe images')

# ---------------- B) fur collars ----------------
FURS = [  # id, mesh, en, de, icon, recipe id, requirements (EKR)
    (3936, 'Padded90_Vik_Helmet_Add_Dw', 'Boar Fur Collar', 'Wildschweinfellkragen', 'boar_hide.png', 6488,
     [(429, 25, 1), (488, 25, 1), (1393, 25, 2), (1477, 25, 1)]),
    (3937, 'Chain90_Vik_Helmet_Add_Dw', 'Bear Fur Collar', 'Bärenfellkragen', 'bear_hide.png', 6489,
     [(426, 25, 1), (490, 25, 1), (1393, 25, 3), (261, 25, 2)]),
    (3938, 'Padded60_Vik_Helmet_Add_Dw', 'Small Boar Fur Collar', 'Kleiner Wildschweinfellkragen', 'boar_hide.png', 6490,
     [(429, 25, 1), (489, 25, 1), (1393, 25, 2), (261, 25, 1)]),
]
DESC_EN = 'A fur collar from a Viking helmet, worn on its own. Cosmetic, takes a ring slot.'
DESC_DE = 'Ein Fellkragen eines Wikingerhelms, einzeln getragen. Rein kosmetisch, belegt einen Ringplatz.'


def orow(oid, en, icon):
    return "(%d,478,'%s',0,0,0,0,0,0,0,0,3,1,500,'',0,0,0,0,0,0,'art/2D/Items/%s','%s',2000,NULL,1,1)" % (oid, en, icon, DESC_EN)


for sub in ('art', 'sql'):
    p = S + '\\' + sub + r'\dump.sql'; bom, t = rd(p)
    if '(3936,478,' not in t:
        i = t.index('(3935,234,'); j = t.index('\n', i) + 1
        assert t[i:j].rstrip().endswith('),')
        t = t[:j] + ''.join(orow(o, en, ic) + ',\n' for o, _, en, _, ic, _, _ in FURS) + t[j:]
        wr(p, bom, t)
print('dump.sql: fur items')

pk = S + r'\mods\LiFx\FurCollarsPack'; os.makedirs(pk, exist_ok=True)
L = ["// Fur collars (from the EKR modpack, 2026-10-04): the fur parts of the Viking helmets (Padded90/Chain90/Padded60_Vik_Helmet_Add_Dw)",
     "// worn alone in a ring slot (7/8), cosmetic only. Objects 3936-3938, recipes 6488-6490 (Tailoring 90, Weaver's Toolkit).", '',
     'if (!isObject(LiFxFurCollarsPack))', '{', '    new ScriptObject(LiFxFurCollarsPack)', '    {', '    };', '}', '',
     'package LiFxFurCollarsPack', '{', '    function LiFxFurCollarsPack::setup() {',
     '        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxFurCollarsPack);', '    }',
     '    function LiFxFurCollarsPack::version() {', '        return "1.0.0";', '    }', '    function LiFxFurCollarsPack::dbChanges() {']
for oid, mesh, en, de, icon, rid, reqs in FURS:
    L.append('        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES %s");' % orow(oid, en, icon))
for oid, mesh, en, de, icon, rid, reqs in FURS:
    L.append("        dbi.Update(\"INSERT IGNORE INTO `recipe` VALUES (%d,'%s','',295,25,90,%d,50,1,0,0,'art/2D/Items/%s')\");" % (rid, en, oid, icon))
    for mat, inf, q in reqs:
        L.append('        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,%d,%d,0,%d,%d,0)");' % (rid, mat, inf, q))
L += ['    }', '};', 'activatePackage(LiFxFurCollarsPack);', 'LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxFurCollarsPack);', '']
open(pk + r'\mod.cs', 'w', encoding='utf-8', newline='\r\n').write('\n'.join(L)); print('FurCollarsPack written')

# cm_equipTypes (server + client): ring slots, mesh, EKR cutoffs, no armor
for root, tag in ((S, 'server'), (C, 'client')):
    p = root + r'\data\cm_equipTypes.xml'; bom, t = rd(p); bk(p, tag + '_cm_equipTypes.xml')
    if '<object id="3936">' not in t:
        nl = '\r\n' if '\r\n' in t else '\n'
        add = ''
        for oid, mesh, en, de, icon, rid, reqs in FURS:
            add += nl.join(['<object id="%d">' % oid, '   <!--name = %s (EKR, cosmetic, ring slot) -->' % en, '   <skillID>33</skillID>', '   <skillAmount>0</skillAmount>',
                            '   <slots>', '      <slot>7</slot>', '      <slot>8</slot>', '   </slots>', '   <meshes>', '      <mesh>%s</mesh>' % mesh, '   </meshes>',
                            '   <cutoffs>', '      <cutoff type="ears" action="on" />', '      <cutoff type="hair" action="on" />',
                            '      <cutoff type="beard" action="on" />', '      <cutoff type="head" action="on" />', '   </cutoffs>', '</object>']) + nl
        i = t.rindex('</equipment_types>'); t = t[:i] + add + t[i:]
        wr(p, bom, t)
    print(tag, 'cm_equipTypes ok')

# client objects_types / recipe / recipe_requirement
p = C + r'\data\objects_types.xml'; bom, t = rd(p)
if '<ID>3936</ID>' not in t:
    tmpl = re.search(r'[ \t]*<row>\s*<ID>479</ID>.*?</row>\r?\n', t, re.S).group(0)
    add = ''
    for oid, mesh, en, de, icon, rid, reqs in FURS:
        r = tmpl.replace('<ID>479</ID>', '<ID>%d</ID>' % oid, 1)
        r = re.sub(r'<Name>.*?</Name>', lambda _: '<Name>%s</Name>' % en, r)
        r = re.sub(r'<UnitWeight>\d+</UnitWeight>', '<UnitWeight>500</UnitWeight>', r)
        r = re.sub(r'<FaceImage>.*?</FaceImage>', lambda _: '<FaceImage>art\\2D\\Items\\%s</FaceImage>' % icon, r)
        r = re.sub(r'<Description>.*?</Description>|<Description\s*/>', lambda _: '<Description>%s</Description>' % DESC_EN, r)
        add += r
    m = re.search(r'[ \t]*<row>\s*<ID>3935</ID>.*?</row>\r?\n', t, re.S); t = t[:m.end()] + add + t[m.end():]
    wr(p, bom, t)
p = C + r'\data\recipe.xml'; bom, t = rd(p)
if '<ID>6488</ID>' not in t:
    tmpl = re.search(r'[ \t]*<row>\s*<ID>222</ID>.*?</row>\r?\n', t, re.S).group(0)   # Blacksmith's outfit (Weaver's Toolkit, Tailoring)
    add = ''
    for oid, mesh, en, de, icon, rid, reqs in FURS:
        r = tmpl.replace('<ID>222</ID>', '<ID>%d</ID>' % rid, 1)
        r = re.sub(r'<Name>.*?</Name>', lambda _: '<Name>%s</Name>' % en, r)
        r = re.sub(r'<StartingToolsID>\d+</StartingToolsID>', '<StartingToolsID>295</StartingToolsID>', r)
        r = re.sub(r'<SkillLvl>\d+</SkillLvl>', '<SkillLvl>90</SkillLvl>', r)
        r = re.sub(r'<ResultObjectTypeID>\d+</ResultObjectTypeID>', '<ResultObjectTypeID>%d</ResultObjectTypeID>' % oid, r)
        r = re.sub(r'<ImagePath>.*?</ImagePath>', lambda _: '<ImagePath>art\\2D\\Items\\%s</ImagePath>' % icon, r)
        add += r
    m = re.search(r'[ \t]*<row>\s*<ID>6487</ID>.*?</row>\r?\n', t, re.S); t = t[:m.end()] + add + t[m.end():]
    wr(p, bom, t)
p = C + r'\data\recipe_requirement.xml'; bom, t = rd(p)
if '<RecipeID>6488</RecipeID>' not in t:
    nid = max(int(x) for x in re.findall(r'<ID>(\d+)</ID>', t)) + 1
    rows = ''
    for oid, mesh, en, de, icon, rid, reqs in FURS:
        for mat, inf, q in reqs:
            rows += ('\t<row><ID>%d</ID><RecipeID>%d</RecipeID><MaterialObjectTypeID>%d</MaterialObjectTypeID><Quality>0</Quality>'
                     '<Influence>%d</Influence><Quantity>%d</Quantity><IsRegionItemRequired>0</IsRegionItemRequired></row>\r\n') % (nid, rid, mat, inf, q)
            nid += 1
    i = t.rindex('</table>'); t = t[:i] + rows + t[i:]
    wr(p, bom, t)
print('client rows ok')

LD = C + r'\data\loc\de\data'
def set_str(f, items):
    p = LD + '\\' + f; bom, t = rd(p); bk(p, 'de_' + f); nl = '\r\n' if '\r\n' in t else '\n'
    for i, v in items:
        if re.search(r'<string id="%d">' % i, t):
            t = re.sub(r'<string id="%d">[^<]*</string>' % i, lambda _: '<string id="%d">%s</string>' % (i, v), t)
        else:
            last = t.rfind('</string>') + len('</string>'); t = t[:last] + nl + '<string id="%d">%s</string>' % (i, v) + t[last:]
    wr(p, bom, t)
set_str('objects_types_Name.xml', [(o, de) for o, _, _, de, _, _, _ in FURS])
set_str('objects_types_Description.xml', [(o, DESC_DE) for o, _, _, _, _, _, _ in FURS])
set_str('recipe_Name.xml', [(rid, de) for _, _, _, de, _, rid, _ in FURS])
print('German names ok; backups in', BK)
