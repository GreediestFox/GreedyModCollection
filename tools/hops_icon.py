import os
import re, os, shutil
from PIL import Image, ImageEnhance
S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
BK = r'E:\ClaudeScratch\backups\mmo_items_20260930'
src = Image.open(r'path/to/your/input.webp').convert('RGBA')
bb = src.getbbox()
src = src.crop(bb)
w = 72
h = round(src.height * w / src.width)
ic = src.resize((w, h), Image.LANCZOS)
d = C + r'\art\2D\Items'
ic.save(d + r'\Hops.png')
hv = ic.copy()
r, g, b, a = hv.split()
rgb = ImageEnhance.Brightness(Image.merge('RGB', (r, g, b))).enhance(1.25)
r, g, b = rgb.split()
Image.merge('RGBA', (r, g, b, a)).save(d + r'\Hops_h.png')
print('icon', ic.size)

cx = C + r'\data\objects_types.xml'
shutil.copy2(cx, os.path.join(BK, 'client_objects_types_before_hops.xml'))
t = open(cx, encoding='utf-8', newline='').read()
t, c = re.subn(r'(<ID>3751</ID>.*?<FaceImage>)[^<]*(</FaceImage>)', lambda m: m.group(1) + r'art\2D\Items\Hops.png' + m.group(2), t, count=1, flags=re.S)
assert c == 1
open(cx, 'w', encoding='utf-8', newline='').write(t)

mp = S + r'\mods\LiFx\FoodDrinksPack\mod.cs'
m = open(mp, encoding='utf-8', newline='').read()
m, c = re.subn(r"(VALUES \(3751,[^\n]*?,')[^']*\.png(')", lambda x: x.group(1) + 'art/2D/Items/Hops.png' + x.group(2), m, count=1)
assert c == 1
open(mp, 'w', encoding='utf-8', newline='').write(m)
print('done')
