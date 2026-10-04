"""Daniel's demo quest (Wranen the Hunter, bring a branch) as quest 21 (2026-10-04).
- server data\quests.xml: quest 21 appended (Daniel's quest 1 with every <quest>1</quest> -> 21); vanilla 1-20 untouched
- quests_subjects.xml (server + client): Wranen (subject 5, type 1513) now starts quest 21 instead of tutorial quest 5
- client data\quests_tasks.xml: quest 21 task entry
- messages 5201-5212 EN (client + server) and DE"""
import os, re, shutil
S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
D = r'E:\ClaudeScratch\daniel\quests darknight'
BK = r'E:\ClaudeScratch\backups\quest21_20261004'
os.makedirs(BK, exist_ok=True)


def rd(p):
    raw = open(p, 'rb').read(); return raw.startswith(b'\xef\xbb\xbf'), raw.decode('utf-8-sig')


def wr(p, bom, t): open(p, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))


def bk(p, tag): shutil.copy2(p, os.path.join(BK, tag))


# quests.xml
_, dq = rd(D + r'\Server\data\quests.xml')
q = re.search(r'[ \t]*<quest id="1">.*?</quest>', dq, re.S).group(0)
q = q.replace('<quest id="1">', '<quest id="21">', 1).replace('<quest>1</quest>', '<quest>21</quest>')
q = q.replace('<!-- Wranen the Hunter - Branch Quest -->', '<!-- Wranen the Hunter - Branch Quest (ported 2026-10-04 from Daniel\'s demo) -->', 1)
p = S + r'\data\quests.xml'; bom, t = rd(p); bk(p, 'server_quests.xml')
if '<quest id="21">' not in t:
    i = t.rindex('</quests>'); nl = '\r\n' if '\r\n' in t else '\n'
    t = t[:i] + q.replace('\n', nl) + nl + t[i:]
    wr(p, bom, t)
print('quests.xml: quest 21 present')

# quests_subjects.xml
for root, tag in ((S, 'server'), (C, 'client')):
    p = root + r'\data\quests_subjects.xml'; bom, t = rd(p); bk(p, tag + '_quests_subjects.xml')
    m = re.search(r'<subject id="5" entity="complex_obj" typeID="1513">.*?</subject>', t, re.S)
    seg = m.group(0).replace('<init quest="5" conversation="1" />', '<init quest="21" conversation="1" />')
    t = t[:m.start()] + seg + t[m.end():]
    wr(p, bom, t); print(tag, 'subject 5 ->', re.search(r'<init quest="(\d+)"', seg).group(1))

# quests_tasks.xml (client)
_, dt = rd(D + r'\Client\data\quests_tasks.xml')
qt = re.search(r'[ \t]*<quest id="1">.*?</quest>', dt, re.S).group(0).replace('<quest id="1">', '<quest id="21">', 1)
p = C + r'\data\quests_tasks.xml'; bom, t = rd(p); bk(p, 'client_quests_tasks.xml')
if '<quest id="21">' not in t:
    root_close = re.findall(r'</(\w+)>\s*$', t)[0]
    i = t.rindex('</%s>' % root_close); nl = '\r\n' if '\r\n' in t else '\n'
    t = t[:i] + qt.replace('\n', nl) + nl + t[i:]
    wr(p, bom, t)
print('quests_tasks.xml: quest 21 present')

# messages
def msgs(path):
    _, t = rd(path)
    return {int(a): (b, c) for a, b, c in re.findall(r'<string id="(\d+)"([^>]*)>([^<]*)</string>', t) if 5201 <= int(a) <= 5212}


EN = msgs(D + r'\Client\data\cm_messages.xml')
DE = msgs(D + r'\Client\data\loc\de\data\cm_messages.xml')
assert len(EN) == 12 and len(DE) == 12


def setmsgs(p, tag, vals):
    bom, t = rd(p); bk(p, tag); nl = '\r\n' if '\r\n' in t else '\n'
    for i in sorted(vals):
        attrs, v = vals[i]
        if re.search(r'<string id="%d"' % i, t):
            t = re.sub(r'(<string id="%d"[^>]*>)[^<]*(</string>)' % i, lambda m: m.group(1) + v + m.group(2), t)
        else:
            last = t.rfind('</string>') + len('</string>')
            t = t[:last] + nl + '<string id="%d"%s>%s</string>' % (i, attrs, v) + t[last:]
    wr(p, bom, t); print('messages in', tag)


setmsgs(C + r'\data\cm_messages.xml', 'client_cm_messages.xml', EN)
setmsgs(S + r'\data\cm_messages.xml', 'server_cm_messages.xml', EN)
setmsgs(C + r'\data\loc\de\data\cm_messages.xml', 'de_cm_messages.xml', DE)
