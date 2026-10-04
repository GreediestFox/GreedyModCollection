"""'s icon images (2026-10-04) -> game item icons for the 4 new beverages and the 3 fur collars.
Background: transparent already, black, or a painted 'transparency checkerboard' -> removed by flood fill from the edges.
Icons: tight crop, scaled like the vanilla ones (drinks ~68 px high, collars ~100 px wide), plus a 1.35x brighter _h variant."""
import os, re, shutil
from collections import deque
from PIL import Image, ImageEnhance

SRC = r'source_images'   # folder with the source images 45.webp..51.webp
S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
OUT = C + r'\art\2D\Items'
BK = r'E:\ClaudeScratch\backups\icons_20261004'
os.makedirs(BK, exist_ok=True)

# item id -> (source image, icon file name, max box (w, h))
ICONS = {
    3920: (45, 'thin_beer', (64, 68)),        # wooden tankard with lid
    3921: (51, 'strong_beer', (52, 68)),      # dark red bottle
    3922: (47, 'wheat_beer', (64, 68)),       # foaming mug
    3923: (46, 'strong_spirits', (60, 68)),   # green bottle with rope and wax seal
    3936: (48, 'boar_fur_collar', (69, 56)),
    3937: (49, 'bear_fur_collar', (69, 56)),
    3938: (50, 'small_boar_fur_collar', (63, 51)),
}


def is_bg(px, mode):
    r, g, b, a = px
    if a < 16: return True
    if mode == 'black': return r < 28 and g < 28 and b < 28
    # checkerboard / white: light and nearly grey
    return min(r, g, b) > 196 and max(r, g, b) - min(r, g, b) < 14


def cut(im):
    im = im.convert('RGBA'); w, h = im.size; px = im.load()
    corner = px[2, 2]
    if corner[3] < 16 and px[w - 3, h - 3][3] < 16:
        mode = 'alpha'
    elif max(corner[:3]) < 28:
        mode = 'black'
    else:
        mode = 'light'
    if mode != 'alpha':
        seen = bytearray(w * h); q = deque()
        for x in range(w):
            q.append((x, 0)); q.append((x, h - 1))
        for y in range(h):
            q.append((0, y)); q.append((w - 1, y))
        while q:
            x, y = q.popleft(); i = y * w + x
            if seen[i]: continue
            seen[i] = 1
            if not is_bg(px[x, y], mode): continue
            px[x, y] = (0, 0, 0, 0)
            if x > 0: q.append((x - 1, y))
            if x < w - 1: q.append((x + 1, y))
            if y > 0: q.append((x, y - 1))
            if y < h - 1: q.append((x, y + 1))
    if mode == 'light':   # defringe: light grey/white pixels touching transparency are checkerboard leftovers
        for _ in range(4):
            kill = []
            for y in range(h):
                for x in range(w):
                    r, g, b, a = px[x, y]
                    if a and min(r, g, b) > 150 and max(r, g, b) - min(r, g, b) < 24:
                        if any(0 <= x + dx < w and 0 <= y + dy < h and px[x + dx, y + dy][3] == 0 for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1))):
                            kill.append((x, y))
            for x, y in kill: px[x, y] = (0, 0, 0, 0)
        # soften: semi-transparent edge for remaining pixels next to transparency
        edge = [(x, y) for y in range(h) for x in range(w) if px[x, y][3] and any(0 <= x + dx < w and 0 <= y + dy < h and px[x + dx, y + dy][3] == 0 for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)))]
        for x, y in edge: r, g, b, a = px[x, y]; px[x, y] = (r, g, b, 140)
        # drop stray strips (checkerboard edge lines): keep only the largest connected blob
        lab = [0] * (w * h); best, best_id, cur = 0, 0, 0
        for sy in range(h):
            for sx in range(w):
                i0 = sy * w + sx
                if lab[i0] or px[sx, sy][3] == 0: continue
                cur += 1; cnt = 0; q = deque([(sx, sy)]); lab[i0] = cur
                while q:
                    x, y = q.popleft(); cnt += 1
                    for nx, ny in ((x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1)):
                        if 0 <= nx < w and 0 <= ny < h:
                            j = ny * w + nx
                            if not lab[j] and px[nx, ny][3]:
                                lab[j] = cur; q.append((nx, ny))
                if cnt > best: best, best_id = cnt, cur
        for y in range(h):
            for x in range(w):
                if px[x, y][3] and lab[y * w + x] != best_id: px[x, y] = (0, 0, 0, 0)
    bbox = im.getchannel('A').point(lambda a: 255 if a > 24 else 0).getbbox()
    return im.crop(bbox), mode


made = {}
for oid, (n, name, box) in ICONS.items():
    im, mode = cut(Image.open(os.path.join(SRC, '%d.webp' % n)))
    im.thumbnail(box, Image.LANCZOS)
    for suffix, img in (('', im), ('_h', ImageEnhance.Brightness(im).enhance(1.35))):
        p = os.path.join(OUT, name + suffix + '.png')
        if os.path.exists(p) and not os.path.exists(os.path.join(BK, name + suffix + '.png')): shutil.copy2(p, BK)
        img.save(p)
    made[oid] = 'art/2D/Items/%s.png' % name
    print(oid, name, im.size, 'bg:', mode)


def rd(p):
    raw = open(p, 'rb').read(); return raw.startswith(b'\xef\xbb\xbf'), raw.decode('utf-8-sig')


def wr(p, bom, t): open(p, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))


def bk(p, tag):
    d = os.path.join(BK, tag)
    if not os.path.exists(d): shutil.copy2(p, d)


ROWFACE = re.compile(r"(\((\d+),\d+,'(?:[^'\\]|\\.|'')*',(?:[^,]*,){18})'([^']*)'")


def fix_rows(t, dump_style):
    def rep(m):
        oid = int(m.group(2))
        if oid not in made: return m.group(0)
        path = made[oid].replace('/', '\\\\') if dump_style else made[oid]
        return m.group(1) + "'" + path + "'"
    return ROWFACE.sub(rep, t)


for sub in ('art', 'sql'):
    p = S + '\\' + sub + r'\dump.sql'; bom, t = rd(p); bk(p, sub + '_dump.sql'); t2 = fix_rows(t, True)
    print(sub, 'dump.sql rows changed:', sum(1 for oid in made if made[oid].replace('/', '\\\\') in t2) ); wr(p, bom, t2)
for pack in ('BeveragesPack', 'FurCollarsPack'):
    p = S + r'\mods\LiFx\%s\mod.cs' % pack; bom, t = rd(p); bk(p, pack + '_mod.cs'); t = fix_rows(t, False)
    for oid, rid in ((3936, 6488), (3937, 6489), (3938, 6490)):   # collar recipe images
        t = re.sub(r"(INSERT IGNORE INTO `recipe` VALUES \(%d,[^\n]*?,)'art/2D/Items/[^']*'\)" % rid, lambda m: m.group(1) + "'" + made[oid] + "')", t)
    wr(p, bom, t); print(pack, 'updated')
p = C + r'\data\objects_types.xml'; bom, t = rd(p); bk(p, 'client_objects_types.xml')
for oid, path in made.items():
    m = re.search(r'<row>\s*<ID>%d</ID>.*?</row>' % oid, t, re.S)
    seg = re.sub(r'<FaceImage>.*?</FaceImage>', lambda _: '<FaceImage>%s</FaceImage>' % path.replace('/', '\\'), m.group(0))
    t = t[:m.start()] + seg + t[m.end():]
wr(p, bom, t)
p = C + r'\data\recipe.xml'; bom, t = rd(p); bk(p, 'client_recipe.xml')
for oid in (3936, 3937, 3938):
    m = re.search(r'<row>(?:(?!</row>).)*<ResultObjectTypeID>%d</ResultObjectTypeID>.*?</row>' % oid, t, re.S)
    seg = re.sub(r'<ImagePath>.*?</ImagePath>', lambda _: '<ImagePath>%s</ImagePath>' % made[oid].replace('/', '\\'), m.group(0))
    t = t[:m.start()] + seg + t[m.end():]
wr(p, bom, t)
print('client rows updated; backups in', BK)
