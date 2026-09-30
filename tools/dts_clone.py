#!/usr/bin/env python3
"""
dts_clone.py - clone objects (meshes) inside a Torque TSShape v24 .dts and give the copy its own material.

Why: a texture is bound to a mesh's material, so two clothing items that need different textures need
two mesh objects. This tool appends a copy of an existing object (all detail levels, skinned meshes
included) under a new name and re-points its primitives to a new material name.

Usage
  python dts_clone.py info  <in.dts> <name-substring>            list matching objects + their materials
  python dts_clone.py build <in.dts> <out.dts> <spec.json>       apply all clones from the spec to a pristine dts
  python dts_clone.py check <file.dts>                           structural validation only

spec.json
  {"clones": [ {"from": "Male_Peasant", "name": "Male_Peasant_Blue",
                "materials": {"Male_Peasant_DIFFUSE": "Male_Peasant_Blue_DIFFUSE"}} ]}
  "materials" maps old material name -> new material name (new names that do not exist yet are appended to
  the shape's material list with the old material's flags). Omit "materials" to clone without recolouring.

Validated on LiF/MMO male.dts + female.dts (version 24). Guard values are renumbered, sequences are untouched.
"""
import sys, json, struct
import numpy as np


def s32v(v):
    v &= 0xFFFFFFFF
    return v - (1 << 32) if v >= (1 << 31) else v


def s16v(v):
    v &= 0xFFFF
    return v - 0x10000 if v >= 0x8000 else v


class Dts:
    def __init__(self, path):
        d = open(path, 'rb').read()
        ver, size, s16, s8 = struct.unpack('<4i', d[:16])
        if (ver & 0xFFFF) != 24:
            raise SystemExit('only DTS version 24 supported, got %d' % (ver & 0xFFFF))
        self.version = ver
        buf = np.frombuffer(d[16:16 + size * 4], dtype='<i4')
        self.b32 = buf[:s16].copy()
        self.b16 = np.frombuffer(d[16 + s16 * 4:16 + s8 * 4], dtype='<i2').copy()
        self.b8 = np.frombuffer(d[16 + s8 * 4:16 + size * 4], dtype='u1').copy()
        self.tail = d[16 + size * 4:]
        w = self.b32
        (self.numNodes, self.numObjects, self.numDecals, self.numSubShapes, self.numIfl, self.numNodeRots,
         self.numNodeTrans, self.numUniform, self.numAligned, self.numArbitrary, self.numGround,
         self.numObjStates, self.numDecalStates, self.numTriggers, self.numDetails, self.numMeshes,
         self.numNames) = [int(x) for x in w[:17]]
        self.L = self._layout()
        self.meshes, self.ends = self._walk()
        self.names = self._names()
        self.mat_start, self.mats, self.mat_arrays = self._materials()

    # ---- 32-bit layout of assembleShape (guards are single words)
    def _layout(self):
        L = {}; p = 19 + 1
        L['radius'] = p; p += 11 + 1
        L['nodes'] = p; p += self.numNodes * 5 + 1
        L['objects'] = p; p += self.numObjects * 6 + 1
        p += self.numDecals * 5 + 1
        p += self.numIfl * 5 + 1
        p += self.numSubShapes * 3 + 1
        L['subNumNodes'] = p; L['subNumObjects'] = p + self.numSubShapes
        p += self.numSubShapes * 3 + 1
        p += self.numNodes * 3 + self.numNodeTrans * 3 + 1
        p += self.numUniform + self.numAligned * 3 + self.numArbitrary * 3 + 1
        p += self.numGround * 3 + 1
        L['objStates'] = p; p += self.numObjStates * 3 + 1
        p += self.numDecalStates + 1
        p += self.numTriggers * 2 + 1
        p += self.numDetails * 7 + 1
        L['meshes'] = p
        return L

    def _walk(self):
        w = self.b32; p = self.L['meshes']
        p16 = self.numNodes * 4 + self.numNodeRots * 4 + 15
        p8 = 15
        guard = 15
        out = []
        for i in range(self.numMeshes):
            m = dict(i=i, s32=p, s16=p16, s8=p8, g=[])
            t = int(w[p]); p += 1
            typ = t & 7; m['type'] = typ
            if typ == 4:
                m.update(e32=p, e16=p16, e8=p8); out.append(m); continue
            if typ not in (0, 1):
                raise SystemExit('mesh %d: unsupported mesh type %d' % (i, typ))

            def G():
                nonlocal p, p16, p8, guard
                if int(w[p]) != guard:
                    raise SystemExit('guard mismatch in mesh %d (%d != %d)' % (i, int(w[p]), guard))
                m['g'].append((p, p16, p8)); p += 1; p16 += 1; p8 += 1; guard += 1
            G()
            nf, nmf, parent = [int(x) for x in w[p:p + 3]]; p += 3
            p += 10
            nv = int(w[p]); p += 1
            m['parent'] = parent; m['nv'] = nv
            if parent < 0: p += 3 * nv
            ntv = int(w[p]); p += 1
            if parent < 0: p += 2 * ntv
            if parent < 0: p += 3 * nv; p8 += nv
            szPrim = int(w[p]); p += 1
            m['prim_pos'] = p; m['szPrim'] = szPrim
            p += szPrim; p16 += szPrim * 2
            szInd = int(w[p]); p += 1; p16 += szInd
            szMerge = int(w[p]); p += 1; p16 += szMerge
            p += 2
            G()
            if typ == 1:
                sz = int(w[p]); p += 1
                if parent < 0: p += 6 * sz; p8 += sz
                szT = int(w[p]); p += 1
                if parent < 0: p += 16 * szT
                szV = int(w[p]); p += 1
                if parent < 0: p += 3 * szV
                szN = int(w[p]); p += 1
                if parent < 0: p += szN
                G()
            m.update(e32=p, e16=p16, e8=p8)
            out.append(m)
        if len(self.b32) - p != 2:
            raise SystemExit('unexpected 32-bit remainder %d' % (len(self.b32) - p))
        return out, (p, p16, p8, guard)

    def _names(self):
        p8 = self.ends[2]
        region = bytes(self.b8[p8 + 1:]); names = []; pos = 0
        for _ in range(self.numNames):
            j = region.index(b'\x00', pos); names.append(region[pos:j].decode('latin1')); pos = j + 1
        if not (0 <= len(region) - (pos + 1) <= 3) or any(region[pos + 1:]):
            raise SystemExit('name table does not end where expected (%d vs %d)' % (pos + 1, len(region)))
        self.names_bytes = region[:pos]
        return names

    def _materials(self):
        t = self.tail
        # material list is the last block: [U8 version][S32 count][names: U8 len + chars]*count[6 arrays of S32/F32]
        # locate by scanning backwards from a known-good anchor: try every position, keep the parse that ends exactly at EOF
        best = None
        i = t.rfind(b'DIFFUSE')
        # walk back to candidate starts (version byte 1 followed by plausible count)
        for start in range(max(0, i - 40000), i):
            if t[start] != 1: continue
            cnt = struct.unpack('<i', t[start + 1:start + 5])[0]
            if not (1 <= cnt <= 4000): continue
            p = start + 5; ok = True
            for _ in range(cnt):
                if p >= len(t): ok = False; break
                L = t[p]; p += 1
                if L == 0 or p + L > len(t): ok = False; break
                p += L
            if ok and len(t) - p == cnt * 4 * 6:
                best = (start, cnt, p); break
        if not best:
            raise SystemExit('material list not found')
        start, cnt, p = best
        names = []; q = start + 5
        for _ in range(cnt):
            L = t[q]; q += 1; names.append(t[q:q + L].decode('latin1')); q += L
        arrs = [np.frombuffer(t[p + a * cnt * 4:p + (a + 1) * cnt * 4], dtype='<i4').copy() for a in range(6)]
        return start, names, arrs

    # ---- queries
    def obj_rows(self):
        w = self.b32; o = self.L['objects']
        return w[o:o + self.numObjects * 6].reshape(-1, 6)

    def find_object(self, name):
        rows = self.obj_rows()
        for i in range(len(rows)):
            if self.names[rows[i][0]] == name: return i
        raise SystemExit('object %r not found' % name)

    def mesh_materials(self, meshidx):
        m = self.meshes[meshidx]
        if m['type'] == 4: return []
        ws = self.b32[m['prim_pos']:m['prim_pos'] + m['szPrim']]
        return sorted(set(int(x) & 0x0FFFFFFF for x in ws if not (int(x) & 0x10000000)))

    # ---- output
    def save(self, path, b32, b16, b8, tail):
        if len(b16) & 1: b16 = np.concatenate([b16, np.zeros(1, dtype='<i2')])
        if len(b8) & 3: b8 = np.concatenate([b8, np.zeros(4 - (len(b8) & 3), dtype='u1')])
        s16 = len(b32); s8 = s16 + len(b16) // 2; size = s8 + len(b8) // 4
        with open(path, 'wb') as f:
            f.write(struct.pack('<4i', self.version, size, s16, s8))
            f.write(b32.astype('<i4').tobytes()); f.write(b16.astype('<i2').tobytes()); f.write(b8.astype('u1').tobytes())
            f.write(tail)


def build(src, out, spec):
    d = Dts(src)
    if d.numObjStates != d.numObjects:
        raise SystemExit('numObjectStates != numObjects, unsupported')
    rows = d.obj_rows()
    add_obj = []; add32 = []; add16 = []; add8 = []
    new_names = []; mats = list(d.mats); arrs = [a.copy() for a in d.mat_arrays]
    guard = d.ends[3]
    n_meshes = d.numMeshes; n_obj = d.numObjects
    log = []
    for c in spec['clones']:
        s = d.find_object(c['from']); srow = rows[s]
        name = c['name']
        if name in d.names or name in new_names:
            raise SystemExit('name %r already exists' % name)
        remap = {}
        for old, new in c.get('materials', {}).items():
            if old not in mats: raise SystemExit('material %r not in list' % old)
            oi = mats.index(old)
            if new in mats: ni = mats.index(new)
            else:
                ni = len(mats); mats.append(new)
                for a in range(6): arrs[a] = np.concatenate([arrs[a], arrs[a][oi:oi + 1]])
            remap[oi] = ni
        nm_idx = d.numNames + len(new_names); new_names.append(name)
        start_new = n_meshes
        for k in range(int(srow[1])):
            m = d.meshes[int(srow[2]) + k]
            if m['type'] == 4:
                add32.append(np.array([int(d.b32[m['s32']])], dtype='<i4')); n_meshes += 1; continue
            blk32 = d.b32[m['s32']:m['e32']].copy(); blk16 = d.b16[m['s16']:m['e16']].copy(); blk8 = d.b8[m['s8']:m['e8']].copy()
            po = m['prim_pos'] - m['s32']
            for j in range(m['szPrim']):
                v = int(blk32[po + j]) & 0xFFFFFFFF
                if (v & 0x10000000) == 0 and (v & 0x0FFFFFFF) in remap:
                    blk32[po + j] = s32v((v & 0xF0000000) | remap[v & 0x0FFFFFFF])
            for (g32, g16, g8) in m['g']:
                blk32[g32 - m['s32']] = guard
                blk16[g16 - m['s16']] = s16v(guard)
                blk8[g8 - m['s8']] = guard & 0xFF
                guard += 1
            add32.append(blk32); add16.append(blk16); add8.append(blk8); n_meshes += 1
        add_obj.append([nm_idx, int(srow[1]), start_new, int(srow[3]), -1, -1])
        n_obj += 1
        log.append('cloned %s -> %s (object %d, meshes %d..%d, materials %s)' % (c['from'], name, n_obj - 1, start_new, n_meshes - 1, {mats[k] if k < len(d.mats) else '?': mats[v] for k, v in remap.items()}))
    L = d.L
    obj_end = L['objects'] + d.numObjects * 6
    st_end = L['objStates'] + d.numObjStates * 3
    new32 = np.concatenate([d.b32[:obj_end], np.array(add_obj, dtype='<i4').reshape(-1),
                            d.b32[obj_end:st_end], np.array([0x3F800000, 0, 0] * len(add_obj), dtype='<i4'),
                            d.b32[st_end:d.ends[0]]] + add32 + [np.array([guard, guard + 1], dtype='<i4')])
    new16 = np.concatenate([d.b16[:d.ends[1]]] + add16 + [np.array([s16v(guard), s16v(guard + 1)], dtype='<i2')])
    names_bytes = d.names_bytes + b''.join(n.encode('latin1') + b'\x00' for n in new_names)
    new8 = np.concatenate([d.b8[:d.ends[2]]] + add8 + [np.array([guard & 0xFF], dtype='u1'),
                                                       np.frombuffer(names_bytes, dtype='u1'), np.array([(guard + 1) & 0xFF], dtype='u1')])
    # header counts
    new32[1] = n_obj; new32[11] = n_obj; new32[15] = n_meshes; new32[16] = d.numNames + len(new_names)
    # sub-shape object count (single sub-shape assumed)
    if d.numSubShapes != 1: raise SystemExit('only one sub-shape supported')
    new32[L['subNumObjects'] + 6 * len(add_obj)] = n_obj      # index shifts by the inserted object rows
    # material list
    blob = bytes([1]) + struct.pack('<i', len(mats))
    for n in mats: blob += bytes([len(n)]) + n.encode('latin1')
    for a in arrs: blob += a.astype('<i4').tobytes()
    tail = d.tail[:d.mat_start] + blob
    d.save(out, new32, new16, new8, tail)
    return log


def check(path, quiet=False):
    d = Dts(path)
    rows = d.obj_rows()
    # every object's mesh range must be inside the mesh list and reference valid material indexes
    bad = 0
    for i in range(len(rows)):
        s, n = int(rows[i][2]), int(rows[i][1])
        if s + n > d.numMeshes: bad += 1; continue
        for k in range(n):
            for mi in d.mesh_materials(s + k):
                if mi >= len(d.mats): bad += 1
    if not quiet:
        print('OK %s: nodes %d objects %d meshes %d names %d materials %d, bad refs %d' % (path, d.numNodes, d.numObjects, d.numMeshes, d.numNames, len(d.mats), bad))
    return bad == 0


def info(path, sub):
    d = Dts(path); rows = d.obj_rows()
    for i in range(len(rows)):
        nm = d.names[rows[i][0]]
        if sub.lower() in nm.lower():
            mats = sorted(set(d.mats[mi] for k in range(int(rows[i][1])) for mi in d.mesh_materials(int(rows[i][2]) + k) if mi < len(d.mats)))
            print('%4d %-40s meshes %d start %d materials %s' % (i, nm, rows[i][1], rows[i][2], mats))


if __name__ == '__main__':
    a = sys.argv[1:]
    if not a: print(__doc__); sys.exit(0)
    if a[0] == 'info': info(a[1], a[2])
    elif a[0] == 'check': sys.exit(0 if check(a[1]) else 1)
    elif a[0] == 'build':
        spec = json.load(open(a[3], encoding='utf-8-sig'))
        for line in build(a[1], a[2], spec): print(line)
        if not check(a[2]): sys.exit(1)
    else: print(__doc__)
