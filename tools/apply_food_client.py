"""Food spoilage client part (2026-10-04): Daniel's tooltipManager.cs (shelf-life row) with the rate table regenerated
from OUR objects_types (same ParentID rules as f_foodPerishRate), and messages 5195-5198 (EN client+server, DE)."""
import os, re, shutil, subprocess
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
SRC = r'E:\ClaudeScratch\daniel\food spoilage system\Client\gui\scripts\tooltipManager.cs'
BK = r'E:\ClaudeScratch\backups\daniel_food_20261004'
M = r'C:\Program Files\MariaDB 12.0\bin\mysql.exe'
os.makedirs(BK, exist_ok=True)

rows = subprocess.run([M, '-uroot', '-p1234', 'lif_1', '-N', '-B', '-e', 'select ID,ParentID from objects_types'],
                      capture_output=True, text=True).stdout.split()
P = {int(rows[i]): (int(rows[i + 1]) if rows[i + 1] != 'NULL' else None) for i in range(0, len(rows), 2)}


def rate(i):
    if i == 376: return None
    cur = i
    for _ in range(32):
        p = P.get(cur)
        if p is None: return None
        if p in (250, 279): return 2
        if p in (227, 228, 229, 230, 231): return 4
        if p == 248: return 8
        cur = p
    return None


lists = {r: ' '.join(str(i) for i in sorted(i for i in P if rate(i) == r)) for r in (2, 4, 8)}
t = open(SRC, 'rb').read().decode('utf-8-sig')
n = 0
for r in (2, 4, 8):
    t, k = re.subn(r'FoodPerish_registerRateList\("[0-9 ]*", %d\);' % r, 'FoodPerish_registerRateList("%s", %d);' % (lists[r], r), t)
    n += k
assert n == 3, n
t = t.replace('function FoodPerish_initRateTable()', '// LiFx 2026-10-04: id lists regenerated from our objects_types (same rules as f_foodPerishRate in FoodSpoilagePack).\nfunction FoodPerish_initRateTable()', 1)
dst = C + r'\gui\scripts\tooltipManager.cs'
shutil.copy2(dst, BK + r'\tooltipManager.cs')
open(dst, 'wb').write(t.encode('utf-8'))
dso = dst + '.dso'
if os.path.exists(dso):
    shutil.copy2(dso, BK + r'\tooltipManager.cs.dso'); os.rename(dso, dso + '.disabled_20261004')
print('tooltip written; counts', {r: len(lists[r].split()) for r in lists})

EN = {5195: 'Remaining shelf life', 5196: ' months', 5197: ' weeks', 5198: ' days'}
DE = {5195: 'Verbleibende Haltbarkeit', 5196: ' Monate', 5197: ' Wochen', 5198: ' Tage'}


def setmsgs(p, tag, vals):
    shutil.copy2(p, BK + '\\' + tag)
    raw = open(p, 'rb').read(); bom = raw.startswith(b'\xef\xbb\xbf'); t = raw.decode('utf-8-sig'); nl = '\r\n' if '\r\n' in t else '\n'
    for i, v in vals.items():
        if re.search(r'<string id="%d"' % i, t):
            t = re.sub(r'(<string id="%d"[^>]*>)[^<]*(</string>)' % i, lambda m: m.group(1) + v + m.group(2), t)
        else:
            last = t.rfind('</string>') + len('</string>')
            t = t[:last] + nl + '<string id="%d">%s</string>' % (i, v) + t[last:]
    open(p, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))
    print('messages set in', tag)


setmsgs(C + r'\data\cm_messages.xml', 'client_cm_messages.xml', EN)
setmsgs(S + r'\data\cm_messages.xml', 'server_cm_messages.xml', EN)
setmsgs(C + r'\data\loc\de\data\cm_messages.xml', 'de_cm_messages.xml', DE)
