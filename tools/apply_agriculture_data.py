"""Port of Daniel's Agriculture_mod data parts (2026-10-04), WITHOUT the client exe patch (see patch_client_plow_parity.py).
 1. Plough 53 -> ParentID 77 (cart family), IsContainer 1, MaxContSize 300000 (art + sql dump.sql).
 2. skill_types.xml (server + client): ability 108 excludes the plough; ability 96 lists the category-77 carts
    individually instead of '77', so the plough stays liftable.
 3. lifxpluss.xml: <harvestSoil> and <plowArea> enabled; <plowCartParity> present but disabled until the client
    side is patched too (one-sided parity makes mismatches worse)."""
import os, re, shutil
S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
BK = r'E:\ClaudeScratch\backups\agri_20261004'
os.makedirs(BK, exist_ok=True)


def bk(p, tag):
    d = os.path.join(BK, tag)
    if not os.path.exists(d): shutil.copy2(p, d)


def rd(p): return open(p, 'rb').read().decode('utf-8')
def wr(p, t): open(p, 'wb').write(t.encode('utf-8'))


OLD = "(53,15,'Plough',0,1,0,0,0,0,0,0,5,"
NEW = "(53,77,'Plough',1,1,0,0,0,0,0,300000,5,"
for sub in ('art', 'sql'):
    p = S + '\\' + sub + r'\dump.sql'; bk(p, sub + '_dump.sql'); t = rd(p)
    if NEW not in t:
        assert t.count(OLD) == 1, sub
        wr(p, t.replace(OLD, NEW))
    print(sub, 'dump.sql plough ok')

CARTS = '167 168 169 1437 1461 1496 1497 3030'
for root, tag in ((S, 'server'), (C, 'client')):
    p = root + r'\data\skill_types.xml'; bk(p, tag + '_skill_types.xml'); t = rd(p)
    m = re.search(r'<ability [^>]*id="96">.*?</ability>', t, re.S)
    seg = m.group(0)
    if 'not_object_type_id">77 ' in seg:
        t = t[:m.start()] + seg.replace('not_object_type_id">77 ', 'not_object_type_id">%s ' % CARTS, 1) + t[m.end():]
    m = re.search(r'<ability [^>]*id="108">.*?</ability>', t, re.S)
    seg = m.group(0)
    if 'not_object_type_id">53<' not in seg:
        seg2 = re.sub(r'(<ent_req type="state">complete working damaged</ent_req>)(\s*)',
                      lambda mm: mm.group(1) + mm.group(2) + '<ent_req type="not_object_type_id">53</ent_req>' + mm.group(2), seg, count=1)
        assert seg2 != seg, tag
        t = t[:m.start()] + seg2 + t[m.end():]
    wr(p, t); print(tag, 'skill_types ok')

p = S + r'\config\lifxpluss.xml'; bk(p, 'lifxpluss.xml'); t = rd(p)
if 'plowCartParity' not in t:
    add = ('\n    <!-- Ported from Daniel\'s Agriculture_mod (2026-10-04). plowCartParity must only be enabled together with the'
           ' matching client exe patch (RVA 0x154F1D, 74 07 -> 90 90); harvestSoil: harvested tiles go back to settled soil;'
           ' plowArea: plowing loosens 3x3 tiles. -->'
           '\n    <plowCartParity enabled="0" />\n    <harvestSoil enabled="1" />\n    <plowArea enabled="1" />')
    t = t.replace('<craftEffectsRange enabled="1" />', '<craftEffectsRange enabled="1" />' + add, 1)
    assert 'plowCartParity' in t
    wr(p, t)
print('config ok')
