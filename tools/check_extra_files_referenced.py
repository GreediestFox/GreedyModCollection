import os, json, re, collections
FR = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
SRC = 'g_extras.json'
extras = json.load(open(SRC))
man = json.load(open(r'E:\ClaudeScratch\GMC\release\1.0.125\upload\manifest.json', encoding='utf-8'))
blob = []
for f in man['files']:
    p = f['path']
    if p.lower().endswith(('.xml', '.cs', '.gui', '.txt', '.ini')) and f['size'] < 40_000_000:
        fp = os.path.join(FR, p.replace('/', os.sep))
        if os.path.exists(fp):
            try: blob.append(open(fp, 'rb').read().decode('utf-8', 'replace').lower().replace('\\', '/'))
            except Exception: pass
blob = '\n'.join(blob)
print('searchable text %.1f MB' % (len(blob) / 1048576))
ref = collections.defaultdict(list); unref = collections.defaultdict(list)
for rel, size, age in extras:
    low = rel.lower()
    noext = low.rsplit('.', 1)[0]
    base = os.path.basename(noext)
    hit = low in blob or noext in blob
    if not hit and low.endswith(('.dts', '.dds', '.png', '.stbin', '.jpg')):
        hit = ('/' + os.path.basename(low)) in blob or (os.path.basename(low) in blob and len(base) > 6)
    parts = rel.split('/')
    g = '/'.join(parts[:4]) if len(parts) > 4 else '/'.join(parts[:-1])
    (ref if hit else unref)[g].append((rel, size))
print('=== REFERENCED by shipped data (a tester is missing these)')
tot = 0
for g, v in sorted(ref.items(), key=lambda kv: -sum(s for _, s in kv[1])):
    print('%5d files %9.1f MB  %s' % (len(v), sum(s for _, s in v) / 1048576, g)); tot += len(v)
print('total referenced extras:', tot)
print('=== NOT referenced (leftovers/duplicates?)')
for g, v in sorted(unref.items(), key=lambda kv: -sum(s for _, s in kv[1]))[:12]:
    print('%5d files %9.1f MB  %s' % (len(v), sum(s for _, s in v) / 1048576, g))
json.dump({g: v for g, v in ref.items()}, open(SRC.replace('g_extras', 'g_extras_ref'), 'w'))
