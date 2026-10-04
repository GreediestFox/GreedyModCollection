"""Tree felling (Daniel's prototype made real, 2026-10-04): pine/spruce drop an Amberwood Log, birch/aspen a
Whitewood Log. Sawing gives Amberwood/Whitewood Billets (new) and the existing MMO boards 3800/3802; Building Log stays 233.
Objects: 3930 Amberwood Log (+3931 carried), 3932 Whitewood Log (+3933 carried), 3934 Amberwood Billet, 3935 Whitewood Billet.
Server hooks in Plus: treeDrops (existing), logDescription, sawOutput, abilityEntityCheck (ported from Daniel).
The client saw menu needs a separate client exe patch (not done here)."""
import os, re, shutil
S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
D = r'E:\ClaudeScratch\daniel\tree felling\Client\art\2D\Items'
BK = r'E:\ClaudeScratch\backups\treefelling_20261004'
os.makedirs(BK, exist_ok=True)


def rd(p):
    raw = open(p, 'rb').read(); return raw.startswith(b'\xef\xbb\xbf'), raw.decode('utf-8-sig')


def wr(p, bom, t): open(p, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))


def bk(p, tag):
    d = os.path.join(BK, tag)
    if not os.path.exists(d): shutil.copy2(p, d)


ROWS = [  # dump.sql rows (same column layout as 653 / 1945 / 325)
    "(3930,622,'Amberwood Log',0,1,0,0,0,0,0,0,2,0,5,'',0,0,0,0,0,0,'art\\\\2D\\\\Items\\\\soft_log.png','A log of amberwood, felled from a pine or spruce.',0,150,0,0)",
    "(3931,1902,'Amberwood Log',0,0,0,0,0,0,0,0,2,1,5000,'',0,0,0,0,0,0,'art\\\\2D\\\\Objects\\\\softwood_log.png','',NULL,NULL,0,0)",
    "(3932,622,'Whitewood Log',0,1,0,0,0,0,0,0,2,0,5,'',0,0,0,0,0,0,'art\\\\2D\\\\Items\\\\hard_log.png','A log of whitewood, felled from a birch or aspen.',0,150,0,0)",
    "(3933,1902,'Whitewood Log',0,0,0,0,0,0,0,0,2,1,5000,'',0,0,0,0,0,0,'art\\\\2D\\\\Objects\\\\hardwood_log.png','',NULL,NULL,0,0)",
    "(3934,234,'Amberwood Billet',0,0,0,0,0,0,0,0,3,10000,10000,'',0,0,0,0,0,0,'art/2D/Items/Amberwood_billet.png','Sawed off of amberwood logs. Can be used as fuel.',400,NULL,0,0)",
    "(3935,234,'Whitewood Billet',0,0,0,0,0,0,0,0,3,10000,10000,'',0,0,0,0,0,0,'art/2D/Items/Whitewood_billet.png','Sawed off of whitewood logs. Can be used as fuel.',400,NULL,0,0)",
]
CONV = ["(2284,3930,3931)", "(2285,3932,3933)"]

# ---- dump.sql (art = seed, sql kept in sync) ----
for sub in ('art', 'sql'):
    p = S + '\\' + sub + r'\dump.sql'; bom, t = rd(p); bk(p, sub + '_dump.sql')
    if '(3930,622,' not in t:
        i = t.index('(3929,225,'); j = t.index('\n', i) + 1
        assert t[i:j].rstrip().endswith('),')
        t = t[:j] + ''.join(r + ',\n' for r in ROWS) + t[j:]
    if '(2284,3930,3931)' not in t:
        assert t.count('(43,653,1945),') == 1
        t = t.replace('(43,653,1945),', '(43,653,1945),\n' + ',\n'.join(CONV) + ',', 1)
    wr(p, bom, t); print(sub, 'dump.sql ok')

# ---- server mod pack (keeps the rows if dump.sql is ever replaced) ----
pk = S + r'\mods\LiFx\TreeFellingPack'; os.makedirs(pk, exist_ok=True)
L = ['// Tree felling (ported 2026-10-04 from Daniel\'s prototype): Amberwood (pine/spruce) and Whitewood (birch/aspen) logs.',
     '// Objects 3930-3935 (logs + carried twins + billets); boards are the MMO items 3800/3802. Plus hooks: treeDrops,',
     '// logDescription, sawOutput, abilityEntityCheck (lifxpluss.xml); client saw menu needs the client exe patch.', '',
     'if (!isObject(LiFxTreeFellingPack))', '{', '    new ScriptObject(LiFxTreeFellingPack)', '    {', '    };', '}', '',
     'package LiFxTreeFellingPack', '{', '    function LiFxTreeFellingPack::setup() {',
     '        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxTreeFellingPack);', '    }',
     '    function LiFxTreeFellingPack::version() {', '        return "1.0.0";', '    }', '    function LiFxTreeFellingPack::dbChanges() {']
for r in ROWS:
    L.append('        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES %s");' % r.replace("\\\\", "\\\\"))
for c in CONV:
    L.append('        dbi.Update("INSERT IGNORE INTO `objects_conversions` VALUES %s");' % c)
L += ['    }', '};', 'activatePackage(LiFxTreeFellingPack);', 'LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxTreeFellingPack);', '']
open(pk + r'\mod.cs', 'w', encoding='utf-8', newline='\r\n').write('\n'.join(L)); print('TreeFellingPack mod.cs written')

# ---- client objects_types.xml ----
p = C + r'\data\objects_types.xml'; bom, t = rd(p); bk(p, 'client_objects_types.xml')
def crow(t, src, i, name, face, desc):
    r = re.search(r'[ \t]*<row>\s*<ID>%d</ID>.*?</row>\r?\n' % src, t, re.S).group(0)
    r = r.replace('<ID>%d</ID>' % src, '<ID>%d</ID>' % i, 1)
    r = re.sub(r'<Name>.*?</Name>', lambda _: '<Name>%s</Name>' % name, r)
    r = re.sub(r'<FaceImage>.*?</FaceImage>', lambda _: '<FaceImage>%s</FaceImage>' % face, r)
    r = re.sub(r'<Description>.*?</Description>|<Description\s*/>', lambda _: '<Description>%s</Description>' % desc, r)
    return r
if '<ID>3930</ID>' not in t:
    add = (crow(t, 653, 3930, 'Amberwood Log', r'art\2D\Items\soft_log.png', 'A log of amberwood, felled from a pine or spruce.') +
           crow(t, 1945, 3931, 'Amberwood Log', r'art\2D\Objects\softwood_log.png', '') +
           crow(t, 653, 3932, 'Whitewood Log', r'art\2D\Items\hard_log.png', 'A log of whitewood, felled from a birch or aspen.') +
           crow(t, 1945, 3933, 'Whitewood Log', r'art\2D\Objects\hardwood_log.png', '') +
           crow(t, 325, 3934, 'Amberwood Billet', r'art\2D\Items\Amberwood_billet.png', 'Sawed off of amberwood logs. Can be used as fuel.') +
           crow(t, 325, 3935, 'Whitewood Billet', r'art\2D\Items\Whitewood_billet.png', 'Sawed off of whitewood logs. Can be used as fuel.'))
    m = re.search(r'[ \t]*<row>\s*<ID>3929</ID>.*?</row>\r?\n', t, re.S)
    t = t[:m.end()] + add + t[m.end():]
    wr(p, bom, t)
print('client objects_types ok')

# ---- cm_objects.xml (server + client): logs use the vanilla log shape ----
for root, tag in ((S, 'server'), (C, 'client')):
    p = root + r'\data\cm_objects.xml'; bom, t = rd(p); bk(p, tag + '_cm_objects.xml')
    if '<object id="3930"' not in t:
        src = re.search(r'([ \t]*)<object id="653"[^>]*>.*?</object>', t, re.S)
        ind = src.group(1); blk = src.group(0)
        a = blk.replace('<object id="653"', '<object id="3930"', 1).replace('<!--name = Softwood tree log-->', '<!--name = Amberwood Log (LiFx 2026-10-04)-->')
        w = blk.replace('<object id="653"', '<object id="3932"', 1).replace('<!--name = Softwood tree log-->', '<!--name = Whitewood Log (LiFx 2026-10-04)-->')
        nl = '\r\n' if '\r\n' in t else '\n'
        t = t[:src.end()] + nl + a + nl + w + t[src.end():]
        wr(p, bom, t)
    print(tag, 'cm_objects ok')

# ---- skill_types.xml (server + client): saw abilities 37/38/39 accept the new logs ----
for root, tag in ((S, 'server'), (C, 'client')):
    p = root + r'\data\skill_types.xml'; bom, t = rd(p); bk(p, tag + '_skill_types.xml')
    n = 0
    for aid in (37, 38, 39):
        m = re.search(r'<ability [^>]*id="%d">.*?</ability>' % aid, t, re.S)
        seg = m.group(0)
        if '626 653 3930 3932' not in seg:
            seg2 = seg.replace('<ent_req type="object_type_id">626 653</ent_req>', '<ent_req type="object_type_id">626 653 3930 3932</ent_req>', 1)
            assert seg2 != seg, (tag, aid)
            t = t[:m.start()] + seg2 + t[m.end():]; n += 1
    wr(p, bom, t); print(tag, 'skill_types abilities updated', n)

# ---- lifxpluss.xml ----
p = S + r'\config\lifxpluss.xml'; bom, t = rd(p); bk(p, 'lifxpluss.xml')
t = re.sub(r'<treeDrops enabled="0">\s*<!-- <drop tree="oak" itemId="626" /> -->\s*</treeDrops>',
           '<treeDrops enabled="1">\n    <!-- Tree felling (2026-10-04): pine/spruce -> Amberwood Log 3930, birch/aspen -> Whitewood Log 3932 -->\n'
           '    <drop tree="pine" itemId="3930" />\n    <drop tree="spruce" itemId="3930" />\n'
           '    <drop tree="birch" itemId="3932" />\n    <drop tree="aspen" itemId="3932" />\n</treeDrops>', t, count=1)
assert '<drop tree="pine" itemId="3930" />' in t, 'treeDrops block not found'
if '<logDescription' not in t:
    t = t.replace('<plowArea enabled="1" />', '<plowArea enabled="1" />\n'
        '    <!-- Tree felling hooks ported from Daniel (2026-10-04): new logs get the log examine text, can be sawn, and saw into their own billet/board. -->\n'
        '    <logDescription enabled="1">\n        <objectTypeId>3930</objectTypeId>\n        <objectTypeId>3932</objectTypeId>\n    </logDescription>\n'
        '    <sawOutput enabled="1">\n        <mapping objectTypeId="3930" billetObjectTypeId="3934" boardObjectTypeId="3800" />\n'
        '        <mapping objectTypeId="3932" billetObjectTypeId="3935" boardObjectTypeId="3802" />\n    </sawOutput>\n'
        '    <abilityEntityCheck enabled="1">\n        <objectTypeId>3930</objectTypeId>\n        <objectTypeId>3932</objectTypeId>\n    </abilityEntityCheck>', 1)
    assert '<logDescription' in t
wr(p, bom, t); print('config ok')

# ---- icons ----
for n in ('Amberwood_billet.png', 'Amberwood_billet_h.png', 'Whitewood_billet.png', 'Whitewood_billet_h.png'):
    dst = C + r'\art\2D\Items\\' + n
    if not os.path.exists(dst): shutil.copy2(os.path.join(D, n), dst); print('icon', n)

# ---- German names / descriptions ----
LD = C + r'\data\loc\de\data'
def set_str(f, items):
    p = LD + '\\' + f; bom, t = rd(p); bk(p, 'de_' + f); nl = '\r\n' if '\r\n' in t else '\n'
    for i, v in items:
        if re.search(r'<string id="%d">' % i, t):
            t = re.sub(r'<string id="%d">[^<]*</string>' % i, lambda _: '<string id="%d">%s</string>' % (i, v), t)
        else:
            last = t.rfind('</string>') + len('</string>'); t = t[:last] + nl + '<string id="%d">%s</string>' % (i, v) + t[last:]
    wr(p, bom, t)
set_str('objects_types_Name.xml', [(3930, 'Bernsteinholzstamm'), (3931, 'Bernsteinholzstamm'), (3932, 'Weißholzstamm'), (3933, 'Weißholzstamm'),
                                   (3934, 'Bernsteinholzscheit'), (3935, 'Weißholzscheit'), (3800, 'Bernsteinholzbrett'),
                                   (3801, 'Bernsteinholz-Baublock'), (3802, 'Weißholzbrett'), (3803, 'Weißholz-Baublock')])
set_str('objects_types_Description.xml', [(3930, 'Ein Stamm aus Bernsteinholz, gefällt von einer Kiefer oder Fichte.'),
                                          (3932, 'Ein Stamm aus Weißholz, gefällt von einer Birke oder Espe.'),
                                          (3934, 'Aus Bernsteinholzstämmen gesägt. Kann als Brennstoff verwendet werden.'),
                                          (3935, 'Aus Weißholzstämmen gesägt. Kann als Brennstoff verwendet werden.')])
print('German names ok; backups in', BK)
