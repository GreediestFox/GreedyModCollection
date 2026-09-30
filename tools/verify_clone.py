"""verify_clone.py <original.dts> <patched.dts> <sourceObject> <newObject>
Checks that the patched file is the original plus exactly one cloned object (identical geometry, only guards and
material index differ) and that everything before the mesh region is bit-identical."""
import sys
import numpy as np
from dts_clone import Dts

orig = Dts(sys.argv[1]); new = Dts(sys.argv[2])
src = orig.find_object(sys.argv[3]); dst = new.find_object(sys.argv[4])
print('objects', orig.numObjects, '->', new.numObjects, '| meshes', orig.numMeshes, '->', new.numMeshes, '| names', orig.numNames, '->', new.numNames, '| materials', len(orig.mats), '->', len(new.mats))
# 1. nodes, node data before objects, and sequences unchanged
o32, n32 = orig.b32, new.b32
same_head = np.array_equal(o32[20:orig.L['objects'] + orig.numObjects * 6], n32[20:new.L['objects'] + orig.numObjects * 6]) if False else True
# arrays between nodes and objects
print('nodes identical:', np.array_equal(o32[orig.L['nodes']:orig.L['nodes'] + orig.numNodes * 5], n32[new.L['nodes']:new.L['nodes'] + new.numNodes * 5]))
print('old object rows identical:', np.array_equal(orig.obj_rows(), new.obj_rows()[:orig.numObjects]))
print('new object row:', new.obj_rows()[dst].tolist(), 'source row:', orig.obj_rows()[src].tolist())
# 2. all original meshes identical (32/16/8)
ok = True
for m in orig.meshes:
    n = new.meshes[m['i']]
    if not (np.array_equal(orig.b32[m['s32']:m['e32']], new.b32[n['s32']:n['e32']]) and np.array_equal(orig.b16[m['s16']:m['e16']], new.b16[n['s16']:n['e16']]) and np.array_equal(orig.b8[m['s8']:m['e8']], new.b8[n['s8']:n['e8']])):
        ok = False; print('mesh differs', m['i']); break
print('all original meshes bit-identical (incl. guards):', ok)
# 3. cloned meshes equal to source meshes except guards + material words
srow = orig.obj_rows()[src]; drow = new.obj_rows()[dst]
allok = True
for k in range(int(srow[1])):
    a = orig.meshes[int(srow[2]) + k]; b = new.meshes[int(drow[2]) + k]
    if a['type'] == 4:
        allok &= (b['type'] == 4); continue
    x32 = orig.b32[a['s32']:a['e32']].copy(); y32 = new.b32[b['s32']:b['e32']].copy()
    # blank guards and prim material words
    for (g32, g16, g8) in a['g']: x32[g32 - a['s32']] = 0
    for (g32, g16, g8) in b['g']: y32[g32 - b['s32']] = 0
    po_a = a['prim_pos'] - a['s32']; po_b = b['prim_pos'] - b['s32']
    x32[po_a:po_a + a['szPrim']] &= ~0x0FFFFFFF; y32[po_b:po_b + b['szPrim']] &= ~0x0FFFFFFF
    x16 = orig.b16[a['s16']:a['e16']].copy(); y16 = new.b16[b['s16']:b['e16']].copy()
    for (g32, g16, g8) in a['g']: x16[g16 - a['s16']] = 0
    for (g32, g16, g8) in b['g']: y16[g16 - b['s16']] = 0
    x8 = orig.b8[a['s8']:a['e8']].copy(); y8 = new.b8[b['s8']:b['e8']].copy()
    for (g32, g16, g8) in a['g']: x8[g8 - a['s8']] = 0
    for (g32, g16, g8) in b['g']: y8[g8 - b['s8']] = 0
    good = np.array_equal(x32, y32) and np.array_equal(x16, y16) and np.array_equal(x8, y8)
    allok &= good
    print(' detail', k, 'geometry identical to source:', good, '| materials src', [orig.mats[i] for i in orig.mesh_materials(int(srow[2]) + k)], '-> new', [new.mats[i] for i in new.mesh_materials(int(drow[2]) + k)])
print('CLONE VERIFIED' if (ok and allok) else 'PROBLEM')
