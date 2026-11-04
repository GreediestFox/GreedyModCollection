import os, sys, json
sys.path.insert(0, r'E:\ClaudeScratch\GMC\tools')
from steam_manifest import read_manifest, sha1
CLI = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own'
DC = r'G:\progam (launcher)\depotcache'
DEPOTS = [('290082', '6520715282330536987'), ('290083', '7495867738960878322'), ('290084', '3874211234009931359')]
m = json.load(open(r'E:\ClaudeScratch\GMC\release\1.0.122\upload\manifest.json', encoding='utf-8'))
ship = {f['path'].lower(): f for f in m['files']}
stock = {}
for dep, man in DEPOTS:
    for e in read_manifest(os.path.join(DC, '%s_%s.manifest' % (dep, man))):
        if e.get('flags', 0) & 64 or 'sha' not in e: continue
        stock[e['name'].lower()] = e
print(len(stock), 'stock files')
mod = []
for k, e in stock.items():
    p = os.path.join(CLI, e['name'])
    if not os.path.exists(p): continue
    if os.path.getsize(p) != e['size'] or sha1(p) != e['sha']:
        mod.append(e['name'])
print('modified stock files on G:', len(mod))
for n in sorted(mod):
    s = ship.get(n.lower().replace('\\', '/'))
    p = os.path.join(CLI, n)
    status = 'NOT SHIPPED'
    if s:
        from hashlib import sha256
        status = 'shipped-identical' if sha256(open(p, 'rb').read()).hexdigest() == s['sha256'] else 'shipped-but-DIFFERENT'
    print('%-22s %9d  %s' % (status, os.path.getsize(p), n))
