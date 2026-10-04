"""Quest 21 clashed with the client's vanilla quest_tasks entry 21 ("Mining"), so the tracker showed Mining texts.
Move Daniel's branch quest to id 30 (free on server and client): server quests.xml, quests_subjects (server+client),
client quests_tasks.xml (insert ours as 30; vanilla 21 untouched). DB progress is moved by the caller."""
import re, shutil, os
S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
D = r'E:\ClaudeScratch\daniel\quests darknight'
BK = r'E:\ClaudeScratch\backups\quest21_20261004\renumber30'
os.makedirs(BK, exist_ok=True)


def rd(p):
    raw = open(p, 'rb').read(); return raw.startswith(b'\xef\xbb\xbf'), raw.decode('utf-8-sig')


def wr(p, bom, t): open(p, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))


# server quests.xml: our quest block is the LAST quest and carries the Daniel comment
p = S + r'\data\quests.xml'; bom, t = rd(p); shutil.copy2(p, BK + r'\server_quests.xml')
a = t.index('<quest id="21">')
assert "ported 2026-10-04 from Daniel" in t[a:a + 200]
b = t.index('</quests>')
blk = t[a:b]
assert '<quest id="30">' not in t
blk2 = blk.replace('<quest id="21">', '<quest id="30">', 1).replace('<quest>21</quest>', '<quest>30</quest>')
t = t[:a] + blk2 + t[b:]
wr(p, bom, t); print('server quests.xml: 21 -> 30 (%d inner refs)' % blk2.count('<quest>30</quest>'))

for root, tag in ((S, 'server'), (C, 'client')):
    p = root + r'\data\quests_subjects.xml'; bom, t = rd(p); shutil.copy2(p, BK + '\\' + tag + '_quests_subjects.xml')
    n = t.count('<init quest="21" conversation="1" />')
    assert n == 1, (tag, n)
    t = t.replace('<init quest="21" conversation="1" />', '<init quest="30" conversation="1" />')
    wr(p, bom, t); print(tag, 'subject -> 30')

_, dt = rd(D + r'\Client\data\quests_tasks.xml')
qt = re.search(r'[ \t]*<quest id="1">.*?</quest>', dt, re.S).group(0).replace('<quest id="1">', '<quest id="30">', 1)
qt = qt.replace('<!-- Wranen the Hunter - Branch Quest -->', "<!-- Wranen the Hunter - Branch Quest (ported 2026-10-04 from Daniel's demo) -->", 1)
p = C + r'\data\quests_tasks.xml'; bom, t = rd(p); shutil.copy2(p, BK + r'\client_quests_tasks.xml')
assert '<quest id="30">' not in t
i = t.rindex('</quests>'); nl = '\r\n' if '\r\n' in t else '\n'
t = t[:i] + '\t' + qt.strip().replace('\r\n', '\n').replace('\n', nl) + nl + t[i:]
wr(p, bom, t); print('client quests_tasks: quest 30 added')
