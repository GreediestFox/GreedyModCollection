"""
Build custom crop visuals for the 5 custom crops (client only). Writes modified copies to --stage; deploy them yourself:
  stage/cm_substances.cs -> scripts/, stage/terrains_materials.cs -> art/terrains/materials.cs,
  stage/cm_environment.cs -> scripts/client/, append stage/materials_append.cs to art/materials.cs,
  rename the stale scripts/cm_substances.cs.dso, art/terrains/materials.cs.dso, scripts/client/cm_environment.cs.dso,
  copy the MMO rye/barley/oat *.stbin + CropsB*.dds (cropsspeedtree/ and its textures/ folder, and art/Textures/GroundCover/),
  and switch the client substance alias off (toggle_client_substance_alias.py).

Root cause (RE 2026-10-02): SubstanceManager::RegisterSubstance (server 0x572340, same code in client) rejects a
substance whose ter2 id, name OR terrainMaterialName is already registered ("already exists" log, misleading).
Our 30 crop substances reused Wheat/Grape/Carrot terrainMaterialName -> rejected -> the client exe alias patch
was masking that, and it also forced the wheat look. Fix: give each a unique terrainMaterialName backed by its own
TerrainMaterial, plus GroundCover blocks drawing the MMO rye/barley/oat models (atlas CropsB). Alias then OFF.
"""
import re, os, argparse

ap = argparse.ArgumentParser(description='Generate the crop-visual script changes into a staging folder.')
ap.add_argument('--client', required=True, help='Life is Feudal Your Own client folder (read only)')
ap.add_argument('--stage', required=True, help='output folder for the modified copies')
args = ap.parse_args()
CL = args.client
STAGE = args.stage
os.makedirs(STAGE, exist_ok=True)

CROPS = [  # crop key, substance-name stem in cm_substances.cs, template vanilla crop, grain (CropsB) model prefix or None
    ('Rye',       'WildRye',   'Wheat',  'rye'),
    ('Barley',    'WildBarley', 'Wheat', 'barley'),
    ('Oat',       'WildOat',   'Wheat',  'oat'),
    ('Hops',      'Hops',      'Grape',  None),
    ('SugarBeet', 'SugarBeet', 'Carrot', None),
]
STAGES = ['Small', 'Normal', 'Big']
FERT = ['Fertile', 'NonFertile']


def rd(p):
    return open(p, 'rb').read().decode('utf-8', 'surrogateescape')


def wr(name, text):
    p = os.path.join(STAGE, name)
    open(p, 'wb').write(text.encode('utf-8', 'surrogateescape'))
    print('wrote', p)


for _p in (r'\art\terrains\materials.cs', r'\scripts\client\cm_environment.cs'):
    if 'LiFx custom crops' in open(CL + _p, 'rb').read().decode('utf-8', 'replace'):
        raise SystemExit('already applied: ' + _p + ' contains the LiFx custom crops block - start from the original files')

# ---------- 1. cm_substances.cs: unique terrainMaterialName per custom substance ----------
subs = rd(CL + r'\scripts\cm_substances.cs')
n = 0
for crop, stem, tmpl, _ in CROPS:
    for f in FERT:
        for s in STAGES:
            sname = f + stem + s
            new_tm = f + crop + s
            m = re.search(r'singleton Substance\(%s\)\s*\{(.*?)\n\};' % re.escape(sname), subs, re.S)
            if not m:
                raise SystemExit('substance %s not found' % sname)
            body = m.group(1)
            body2, k = re.subn(r'terrainMaterialName = "[^"]*";', 'terrainMaterialName = "%s";' % new_tm, body, count=1)
            if k != 1:
                raise SystemExit('no terrainMaterialName in ' + sname)
            subs = subs[:m.start(1)] + body2 + subs[m.end(1):]
            n += 1
print('substances retargeted:', n)
wr('cm_substances.cs', subs)

# ---------- 2. art/terrains/materials.cs: one TerrainMaterial per new name (clone of the template's) ----------
ter = rd(CL + r'\art\terrains\materials.cs')
blocks = {}
for m in re.finditer(r'new TerrainMaterial\(\) \{.*?\n\s*\};', ter, re.S):
    nm = re.search(r'internalName = "([^"]*)"', m.group(0))
    if nm:
        blocks[nm.group(1)] = m.group(0)
gidx = [int(x) for x in re.findall(r'globalIndex = "(\d+)"', ter)]
nxt = max(gidx) + 1
start_idx = nxt
new_blocks = []
for crop, stem, tmpl, _ in CROPS:
    for f in FERT:
        for s in STAGES:
            src = blocks.get(f + tmpl + s)
            if not src:
                raise SystemExit('template terrain material %s missing' % (f + tmpl + s))
            b = src.replace('internalName = "%s"' % (f + tmpl + s), 'internalName = "%s"' % (f + crop + s))
            b, k = re.subn(r'globalIndex = "\d+"', 'globalIndex = "%d"' % nxt, b)
            if k != 1:
                raise SystemExit('globalIndex count %d in %s' % (k, f + tmpl + s))
            new_blocks.append('      ' + b.strip())
            nxt += 1
end_marker = '//--- OBJECT WRITE END ---'
pos = ter.rfind('};', 0, ter.find(end_marker))  # closing of SimGroup(TerrainMaterialList)
ter = ter[:pos] + '   // LiFx custom crops (2026-10-02): unique terrain materials for Rye/Barley/Oat/Hops/SugarBeet\n' + \
      '\n'.join(new_blocks) + '\n' + ter[pos:]
print('terrain materials added: %d (globalIndex %d-%d)' % (len(new_blocks), start_idx, nxt - 1))
wr('terrains_materials.cs', ter)

# ---------- 3. cm_environment.cs: 8 new GroundCover blocks ----------
env = rd(CL + r'\scripts\client\cm_environment.cs')
VAN = {('Fertile', 'Normal'): 'Gc_Plants_normal_fertile', ('Fertile', 'Big'): 'GC_Plants_big_fertile',
       ('NonFertile', 'Normal'): 'Gc_Plants_normal_nonfertile', ('NonFertile', 'Big'): 'GC_Plants_big_nonfertile'}
SLOT = {'Wheat': 6, 'Grape': 0, 'Carrot': 4}


def get_block(name):
    m = re.search(r'singleton GroundCover\(%s\) \{.*?\n   \};' % re.escape(name), env, re.S)
    if not m:
        raise SystemExit('groundcover %s not found' % name)
    return m.group(0)


def make_gc(newname, van_block, material, slots):
    """slots: list of (template_slot_index, layer_name, shapeFilename, shapeAltFilename)"""
    lines = van_block.split('\n')
    head = re.sub(r'GroundCover\([^)]*\)', 'GroundCover(%s)' % newname, lines[0])
    scalars, idx = [], {}
    for ln in lines[1:-1]:
        mm = re.match(r'\s*(\w+)\[(\d+)\]\s*=\s*(.*);\s*$', ln)
        if mm:
            idx.setdefault(mm.group(1), {})[int(mm.group(2))] = mm.group(3)
            continue
        ms = re.match(r'\s*(\w+)\s*=\s*(.*);\s*$', ln)
        if ms:
            if ms.group(1) == 'Material':
                scalars.append('      Material = "%s";' % material)
            else:
                scalars.append('      %s = %s;' % (ms.group(1), ms.group(2)))
    out = [head] + scalars + ['']
    for prop in idx:
        for i, (ts, layer, shp, alt) in enumerate(slots):
            if prop == 'layer':
                v = '"%s"' % layer
            elif prop == 'shapeFilename':
                v = '"%s"' % shp
            elif prop == 'shapeAltFilename':
                v = '"%s"' % alt
            else:
                v = idx[prop].get(ts)
                if v is None:
                    continue
            out.append('      %s[%d] = %s;' % (prop, i, v))
        out.append('')
    out.append('   };')
    return '\n'.join(out)


MD = 'art/models/3d/environment/cropsspeedtree/'
gcs = []
for f in FERT:
    for s in ('Normal', 'Big'):
        van = get_block(VAN[(f, s)])
        size = 'small' if s == 'Normal' else 'big'
        grain = [(SLOT['Wheat'], f + c + s, MD + '%s%s01.stbin' % (p, size), MD + '%s%s02.stbin' % (p, size))
                 for c, _, _, p in CROPS if p]
        other = []
        for c, _, t, p in CROPS:
            if p is None:
                base = t.lower()
                other.append((SLOT[t], f + c + s, MD + '%s%s01.stbin' % (base, size), MD + '%s%s02.stbin' % (base, size)))
        gcs.append(make_gc('GC_LiFx_%s_%s_grain' % (f, s), van, 'CropsB', grain))
        gcs.append(make_gc('GC_LiFx_%s_%s_other' % (f, s), van, 'CropsAtlas', other))
endpos = env.rstrip().rfind('};')  # closing of SimGroup(EnvironmentGroup)
env = env[:endpos] + '   // LiFx custom crops (2026-10-02): MMO rye/barley/oat (atlas CropsB) + hops/sugar beet\n' + \
      '\n\n'.join(gcs) + '\n\n' + env[endpos:]
print('groundcover blocks added:', len(gcs))
wr('cm_environment.cs', env)

# ---------- 4. CropsB material (appended to art/materials.cs, which has no .dso) ----------
mat = '''
// LiFx custom crops (2026-10-02): MMO grain atlas for rye/barley/oat ground cover
singleton Material(CropsB)
{
   mapTo = "CropsB";
   diffuseColor[0] = "0.8 0.8 0.8 1";
   diffuseMap[0] = "art/Textures/GroundCover/CropsB.dds";
   diffuseMap[1] = "art/Textures/GroundCover/CropsB_Normal.dds";
   specularPower[0] = "10";
   pixelSpecular[0] = "1";
   useAnisotropic[0] = "1";
   doubleSided = "1";
   alphaTest = "1";
   alphaRef = "111";
   materialTag0 = "Crops";
   useCustomColor = true;
};
'''
wr('materials_append.cs', mat)
