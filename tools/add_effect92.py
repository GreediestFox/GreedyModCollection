"""Effect 92 (the only unused slot) = clone of 19 'Stronger' named 'Slightly Stronger' (DE 'Etwas staerker').
Same parameter (STR/INCREASE) and icon; the potency comes from the magnitude it is applied with.
cm_effects.xml is identical on server and client and must stay identical."""
import os, re, shutil
FILES = [r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server\data\cm_effects.xml',
         r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\data\cm_effects.xml']
LOC = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\data\loc\de\data\cm_effects_name.xml'
BK = r'E:\ClaudeScratch\backups\effect92_20261003'
os.makedirs(BK, exist_ok=True)
for i, p in enumerate(FILES):
    shutil.copy2(p, os.path.join(BK, ('server_' if i == 0 else 'client_') + 'cm_effects.xml'))
    t = open(p, 'rb').read().decode('utf-8')
    if '<effect id="92"' in t:
        print('already present', p); continue
    src = re.search(r'([ \t]*)<effect id="19" name="Stronger">.*?</effect>', t, re.S)
    indent = src.group(1)
    clone = src.group(0).replace('<effect id="19" name="Stronger">', '<effect id="92" name="Slightly Stronger">')
    clone = '%s<!-- LiFx 2026-10-03: id 92 was the only unused slot; clone of 19 Stronger (potency = magnitude at apply time) -->\r\n' % indent + clone
    anchor = re.search(r'[ \t]*<effect id="93"', t)
    t = t[:anchor.start()] + clone + '\r\n' + t[anchor.start():]
    open(p, 'wb').write(t.encode('utf-8'))
    print('added effect 92 to', p)
shutil.copy2(LOC, os.path.join(BK, 'de_cm_effects_name.xml'))
raw = open(LOC, 'rb').read(); bom = raw.startswith(b'\xef\xbb\xbf'); t = raw.decode('utf-8-sig')
if '<string id="92">' not in t:
    t = t.replace('<string id="93">', '<string id="92">Etwas stärker</string>\r\n<string id="93">', 1)
    open(LOC, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))
    print('German name added')
