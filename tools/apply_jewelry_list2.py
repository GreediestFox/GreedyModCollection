"""Jewelry buff list update 2026-11-04 (Michael's list): Int removed everywhere; 496 Str+Will, 498 Str+Will+Con, 491 Str+Will+Agi. Byte-exact edits of the server item_effects.xml."""
import shutil, os

S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server\data\item_effects.xml'
BK = r'E:\ClaudeScratch\backups\jewelry_list2_20261104'
os.makedirs(BK, exist_ok=True)
if not os.path.exists(BK + r'\item_effects.xml'):
    shutil.copy2(S, BK + r'\item_effects.xml')
d = open(S, 'rb').read()
q = b'\r\n' if b'\r\n' in d else b'\n'
ind = b'        '


def block(lines):
    return q.join(ind + l for l in lines)


edits = [
    (b'<item id="496"> <!-- Gold Sapphire Ring: Inspired Work (was Int) -->' + q + block([b'<effect id="58">1 1</effect>']),
     b'<item id="496"> <!-- Gold Sapphire Ring -->' + q + block([b'<effect id="7">0.5 1.5</effect>', b'<effect id="9">0.5 1.5</effect>'])),
    (b'<item id="498"> <!-- Exceptional Gold and Silver Ring -->' + q + block([b'<effect id="10">4 15</effect>', b'<effect id="9">1 3</effect>', b'<effect id="11">1 5</effect>']),
     b'<item id="498"> <!-- Exceptional Gold and Silver Ring -->' + q + block([b'<effect id="7">1 3</effect>', b'<effect id="9">1 4</effect>', b'<effect id="11">1 5</effect>'])),
    (b'<item id="491"> <!-- Exceptional Gold and Silver Amulet -->' + q + block([b'<effect id="10">5 20</effect>', b'<effect id="9">2 5</effect>', b'<effect id="8">2 7</effect>']),
     b'<item id="491"> <!-- Exceptional Gold and Silver Amulet -->' + q + block([b'<effect id="7">2 5</effect>', b'<effect id="9">2 6</effect>', b'<effect id="8">2 7</effect>'])),
]
for old, new in edits:
    assert d.count(old) == 1, old[:60]
    d = d.replace(old, new)
open(S, 'wb').write(d)
print('item_effects.xml updated:', len(edits), 'items')
