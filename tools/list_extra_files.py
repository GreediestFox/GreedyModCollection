import os, sys, json, re, collections, time
sys.path.insert(0, r'E:\ClaudeScratch\GMC\tools')
from steam_manifest import read_manifest
CLI = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own'
DC = r'G:\progam (launcher)\depotcache'
DEPOTS = [('290082', '6520715282330536987'), ('290083', '7495867738960878322'), ('290084', '3874211234009931359')]
m = json.load(open(r'E:\ClaudeScratch\GMC\release\1.0.125\upload\manifest.json', encoding='utf-8'))
ship = {f['path'].lower() for f in m['files']}
stock = set()
for dep, man in DEPOTS:
    for e in read_manifest(os.path.join(DC, '%s_%s.manifest' % (dep, man))):
        stock.add(e['name'].lower().replace('\\', '/'))
SKIP = re.compile(r'''^(logs|screenshots|daemon|shaders/cache|core/fonts|art/intro\.backup|scripts/run_once|mod/Books/cache|scripts/lifmod/Books/cache|mod/GMHelper/economy|data/(terrains|presets|hotbars|inventory))/
  |^data/(prefs\.cs|sf_tree\.txt)$|^scripts/client/(postfx\.settings|developers\.bind)\.cs$|\.log$|\.gmc_stock$|^\.mis\.decals|gmc-hashcache|patcher\.log|\.gmc-download$|\.lock$|\.lnk$|\.dso$|^crashrpt|^debug\.log|^console\.log|^cache/|^prefs''', re.I | re.X)
extras = []
for dp, dn, fn in os.walk(CLI):
    for f in fn:
        p = os.path.join(dp, f)
        rel = os.path.relpath(p, CLI).replace('\\', '/')
        k = rel.lower()
        if k in stock or k in ship or SKIP.search(rel): continue
        extras.append((rel, os.path.getsize(p), (time.time() - os.path.getmtime(p)) / 86400))
print(len(extras), 'extra files (not stock, not shipped, not per-player)')
grp = collections.defaultdict(lambda: [0, 0])
for rel, size, age in extras:
    parts = rel.split('/')
    key = '/'.join(parts[:4]) if len(parts) > 4 else '/'.join(parts[:-1]) or parts[0]
    grp[key][0] += 1; grp[key][1] += size
for k, (n, s) in sorted(grp.items(), key=lambda kv: -kv[1][1])[:45]:
    print('%5d files %10.1f MB  %s' % (n, s / 1048576, k))
json.dump(extras, open('g_extras.json', 'w'))
