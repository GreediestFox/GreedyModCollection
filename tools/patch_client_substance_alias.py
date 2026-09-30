import os
#!/usr/bin/env python3
"""
patch_client_substance_alias.py - make the CLIENT's Geo::SubstanceManager ter2id lookup treat custom
crop substance ids as aliases of Wheat's own, so ability XML requirements referencing them (e.g. the
shared "Harvest Crops" ability, id 73) pass Substance_EntityRequirement::_init()'s validation instead
of aborting client startup with "Can't startup: failed to load skills/abilities."

Mirrors the server-side Plus DLL hook (hook_crop_types.cpp, OnSubstanceLookupByTer2Id) exactly, but as
a direct .exe file patch since the client has no DLL-injection framework. The lookup function
(RVA 0xa6bc10) is byte-identical to the server's (RVA 0x572de0) - both walk a byte-keyed hash bucket:

    mov r9, [rcx+0x10]      ; RVA 0xa6bc10, overwritten by a 5-byte jmp + 3 NOPs
    movzx r8d, dl
    test r9, r9
    jz ...

The cave: for each configured alias base B (a crop's fertileSmallSubstance) and its alias target T
(default 101=Wheat; pass base:target to pick a different vanilla crop, e.g. 161=Grape), if dl is in
[B, B+6) it is remapped to T+(dl-B) before falling through to the two replicated original instructions
and jumping back to RVA 0xa6bc18 (right after them). Terrain data itself is untouched - this only
affects what this one lookup reports substances 191..196 etc. "look like".

Usage:  python patch_client_substance_alias.py <yo_cm_client.exe> <base1>[:<target1>] [<base2>[:<target2>] ...]
        (re-run with a different base list to re-patch; requires --restore first if a .subal section
        already exists. Builds on top of the CURRENT exe, so run this AFTER patch_client_crops.py.)
        python patch_client_substance_alias.py <yo_cm_client.exe> --restore
"""
import sys, os, struct, shutil
import pefile

RVA = {
    'TARGET': 0xa6bc10,   # Geo::SubstanceManager's byte-ter2id lookup (client's own copy)
    'RESUME': 0xa6bc18,   # first instruction after the 2 we overwrite (`test r9,r9`)
}
ORIG_BYTES = bytes.fromhex('4c8b4910' '440fb6c2')  # mov r9,[rcx+0x10] ; movzx r8d,dl
WHEAT_BASE = 101


class Asm:
    def __init__(self, base):
        self.base = base
        self.b = bytearray()
        self.labels = {}
        self.fix = []

    def here(self):
        return self.base + len(self.b)

    def label(self, name):
        self.labels[name] = self.here()

    def raw(self, hexstr):
        self.b += bytes.fromhex(hexstr.replace(' ', ''))

    def rel32(self, target):
        self.fix.append((len(self.b), target))
        self.b += b'\0\0\0\0'

    def jmp(self, target):
        self.raw('E9')
        self.rel32(target)

    def jcc(self, cc_byte_hex, target):
        self.raw('0F ' + cc_byte_hex)
        self.rel32(target)

    def resolve(self):
        for pos, target in self.fix:
            tgt = self.labels[target] if isinstance(target, str) else target
            nxt = self.base + pos + 4
            self.b[pos:pos + 4] = struct.pack('<i', tgt - nxt)
        return bytes(self.b)


def build_cave(base, alias_pairs):
    a = Asm(base)
    a.label('start')
    for i, (b, t) in enumerate(alias_pairs):
        a.raw('80 FA'); a.b += struct.pack('<B', b)          # cmp dl, b
        a.jcc('82', 'next_%d' % i)                            # jb next_i (dl < b: not this crop)
        a.raw('80 FA'); a.b += struct.pack('<B', (b + 6) & 0xFF)  # cmp dl, b+6
        a.jcc('83', 'next_%d' % i)                            # jae next_i (dl >= b+6: not this crop)
        a.raw('80 EA'); a.b += struct.pack('<B', b)           # sub dl, b
        a.raw('80 C2'); a.b += struct.pack('<B', t)           # add dl, t
        a.jmp('resume_replicated')
        a.label('next_%d' % i)
    a.label('resume_replicated')
    a.raw('4C 8B 49 10')   # mov r9, [rcx+0x10]
    a.raw('44 0F B6 C2')   # movzx r8d, dl
    a.jmp(RVA['RESUME'])
    return a


def main():
    if len(sys.argv) < 3:
        print(__doc__)
        sys.exit(1)
    exe = sys.argv[1]
    bkdir = r'E:\ClaudeScratch\backups\croptypes_20260927'
    os.makedirs(bkdir, exist_ok=True)
    orig = os.path.join(bkdir, 'yo_cm_client.exe.before_substancealias')
    if not os.path.exists(orig):
        shutil.copy2(exe, orig)
        print('backup created:', orig)
    if sys.argv[2] == '--restore':
        shutil.copy2(orig, exe)
        print('restored original exe')
        return
    alias_pairs = []
    for spec in sys.argv[2:]:
        if ':' in spec:
            b_s, t_s = spec.split(':')
            b, t = int(b_s), int(t_s)
        else:
            b, t = int(spec), WHEAT_BASE
        if not (0 <= b <= 249) or not (0 <= t <= 249):
            raise SystemExit('alias base/target %s out of byte range' % spec)
        alias_pairs.append((b, t))

    d = bytearray(open(exe, 'rb').read())
    lf = struct.unpack_from('<I', d, 0x3c)[0]
    nsec, = struct.unpack_from('<H', d, lf + 6)
    optsz, = struct.unpack_from('<H', d, lf + 20)
    opt = lf + 24
    if struct.unpack_from('<H', d, opt)[0] != 0x20b:
        raise SystemExit('not PE32+')
    sect_align, file_align = struct.unpack_from('<II', d, opt + 32)
    size_of_headers, = struct.unpack_from('<I', d, opt + 60)
    size_of_image, = struct.unpack_from('<I', d, opt + 56)
    sec0 = opt + optsz
    secs = []
    for i in range(nsec):
        o = sec0 + 40 * i
        name = bytes(d[o:o + 8]).rstrip(b'\0').decode(errors='replace')
        vs, va, rs, rp = struct.unpack_from('<IIII', d, o + 8)
        secs.append((name, va, vs, rp, rs, o))
    if any(s[0] == '.subal' for s in secs):
        raise SystemExit('a .subal section already exists - restore the backup first')

    def r2o(rva):
        for name, va, vs, rp, rs, o in secs:
            if va <= rva < va + max(vs, rs):
                return rp + (rva - va)
        raise SystemExit('rva %x not mapped' % rva)

    got = bytes(d[r2o(RVA['TARGET']):r2o(RVA['TARGET']) + len(ORIG_BYTES)])
    if got != ORIG_BYTES:
        raise SystemExit('target site has unexpected bytes %s (expected %s) - different client build?'
                          % (got.hex(), ORIG_BYTES.hex()))

    last_end = max(s[3] + s[4] for s in secs)
    sec_hdr_pos = sec0 + 40 * nsec
    if bytes(d[sec_hdr_pos:sec_hdr_pos + 40]) != b'\0' * 40 or sec_hdr_pos + 40 > size_of_headers:
        raise SystemExit('no room for a new section header')
    new_va = (size_of_image + sect_align - 1) // sect_align * sect_align

    cave = build_cave(new_va, alias_pairs)
    code = cave.resolve()
    raw_size = (len(code) + file_align - 1) // file_align * file_align

    d = d[:last_end] + bytearray(b'\0' * 0)
    d += code + b'\0' * (raw_size - len(code))
    hdr = b'.subal\0\0' + struct.pack('<IIIIIIHHI', len(code), new_va, raw_size, last_end,
                                      0, 0, 0, 0, 0x60000020)
    d[sec_hdr_pos:sec_hdr_pos + 40] = hdr
    struct.pack_into('<H', d, lf + 6, nsec + 1)
    struct.pack_into('<I', d, opt + 56, new_va + (len(code) + sect_align - 1) // sect_align * sect_align)
    struct.pack_into('<II', d, opt + 112 + 8 * 4, 0, 0)
    struct.pack_into('<I', d, opt + 64, 0)

    def w(rva, blob):
        o = r2o(rva)
        d[o:o + len(blob)] = blob

    def rel(frm_next, tgt):
        return struct.pack('<i', tgt - frm_next)

    jmp_and_nops = b'\xe9' + rel(RVA['TARGET'] + 5, cave.labels['start']) + b'\x90\x90\x90'
    w(RVA['TARGET'], jmp_and_nops)

    open(exe, 'wb').write(d)
    print('patched %s: new section .subal at RVA %x (%d bytes), alias pairs (base:target) %s'
          % (exe, new_va, len(code), alias_pairs))


if __name__ == '__main__':
    main()
