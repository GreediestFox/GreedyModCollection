import os
import zipfile, os, json, hashlib, shutil

C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
ART = C + r'\art\2D'
BK = r'E:\ClaudeScratch\backups\mmo_icons_20260930'
Z = r'F:\Lif Mods'
ZIPS = ['equipIcons_extracted.zip', 'Items_extracted.zip', 'Objects_extracted.zip', 'Recipes_extracted.zip']
os.makedirs(BK + r'\replaced', exist_ok=True)

# case-insensitive index of existing files
idx = {}
for dp, _, fs in os.walk(ART):
    for f in fs:
        p = os.path.join(dp, f)
        idx[os.path.relpath(p, ART).replace('\\', '/').lower()] = p

manifest = {'added': [], 'replaced': [], 'identical': 0}
total_new_bytes = 0
for zn in ZIPS:
    zf = zipfile.ZipFile(os.path.join(Z, zn))
    for i in zf.infolist():
        if i.is_dir():
            continue
        rel = i.filename
        data = zf.read(i)
        key = rel.lower()
        dst = idx.get(key) or os.path.join(ART, rel.replace('/', '\\'))
        if key in idx:
            cur = open(idx[key], 'rb').read()
            if hashlib.md5(cur).digest() == hashlib.md5(data).digest():
                manifest['identical'] += 1
                continue
            bk = os.path.join(BK, 'replaced', rel.replace('/', '\\'))
            os.makedirs(os.path.dirname(bk), exist_ok=True)
            open(bk, 'wb').write(cur)
            open(dst, 'wb').write(data)
            manifest['replaced'].append(rel)
        else:
            os.makedirs(os.path.dirname(dst), exist_ok=True)
            open(dst, 'wb').write(data)
            manifest['added'].append(rel)
            total_new_bytes += len(data)
            idx[key] = dst
json.dump(manifest, open(BK + r'\manifest.json', 'w'), indent=1)
print('added %d files (%.1f MB), replaced %d (originals backed up), identical skipped %d' % (
    len(manifest['added']), total_new_bytes / 1e6, len(manifest['replaced']), manifest['identical']))
