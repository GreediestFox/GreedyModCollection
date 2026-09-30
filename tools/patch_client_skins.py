import os
#!/usr/bin/env python3
"""
patch_client_skins.py - make the LiF:YO client list custom (non-Steam) clothing skins as owned in the wardrobe window.

The client builds the wardrobe list in FUN_140a67db0 (RVA 0xa67db0): it takes the skin ids from Steam's item
definitions, keeps the ones that are skins of the equipped item, and marks them owned/not owned from the Steam
inventory. This patch adds a second pass over a small table of extra skin ids (kept in a new code section) that are
appended as OWNED whenever the equipped item type really has that skin in cm_equipTypes.xml.

Usage:  python patch_client_skins.py <yo_cm_client.exe> <skinId> [<skinId> ...]
        (re-run with a different id list any time; it always starts from the original exe backup)
        python patch_client_skins.py <yo_cm_client.exe> --restore
"""
import sys, os, struct, shutil

RVA = {'S1': 0xa67f9f, 'S2': 0xa68084, 'S3': 0xa67fc0,
       'LOOP_BODY': 0xa67fb0, 'NEXT': 0xa68077, 'OWNED': 0xa6801d, 'ORIG_CONT': 0xa67fc8, 'RESULTS': 0xa6808c}
ORIG = {'S1': bytes.fromhex('0f84e3000000'), 'S2': bytes.fromhex('4c8b6567488b45c7'),
        'S3': bytes.fromhex('84c00f84af000000')}


class Asm:
    def __init__(self, base):
        self.base = base; self.b = bytearray(); self.labels = {}; self.fix = []

    def here(self): return self.base + len(self.b)
    def label(self, n): self.labels[n] = self.here()
    def raw(self, hexs): self.b += bytes.fromhex(hexs.replace(' ', ''))
    def rel32(self, target):          # placeholder disp32 to a label or an absolute RVA
        self.fix.append((len(self.b), target)); self.b += b'\0\0\0\0'
    def jmp(self, t): self.raw('E9'); self.rel32(t)
    def jcc(self, op, t): self.raw('0F ' + op); self.rel32(t)
    def lea(self, opcode3, t): self.raw(opcode3); self.rel32(t)
    def resolve(self):
        for pos, t in self.fix:
            tgt = self.labels[t] if isinstance(t, str) else t
            nxt = self.base + pos + 4
            self.b[pos:pos + 4] = struct.pack('<i', tgt - nxt)
        return bytes(self.b)


def build_cave(base, ids):
    a = Asm(base)
    # CHK: replaces "test al,al / je NEXT" at S3
    a.label('CHK')
    a.raw('84 C0'); a.jcc('84', RVA['NEXT'])                   # test al,al ; je NEXT
    a.lea('48 8D 0D', 'T_START'); a.raw('48 39 CE'); a.jcc('82', 'ORIG')   # lea rcx,T ; cmp rsi,rcx ; jb ORIG
    a.lea('48 8D 0D', 'T_END'); a.raw('48 39 CE'); a.jcc('83', 'ORIG')     # lea rcx,Tend ; cmp rsi,rcx ; jae ORIG
    a.jmp(RVA['OWNED'])                                        # custom id -> force the "owned" path
    a.label('ORIG'); a.jmp(RVA['ORIG_CONT'])
    # C0: entered when the Steam pass is finished (or was empty)
    a.label('C0')
    a.lea('48 8D 05', 'T_START'); a.raw('48 39 C6'); a.jcc('82', 'S2')     # lea rax,T ; cmp rsi,rax ; jb S2
    a.lea('48 8D 05', 'T_END'); a.raw('48 39 C6'); a.jcc('87', 'S2')       # lea rax,Tend ; cmp rsi,rax ; ja S2
    a.jmp('DONE')                                              # rsi inside table => second pass finished
    a.label('S2')
    a.lea('48 8D 35', 'T_START'); a.lea('4C 8D 3D', 'T_END')   # lea rsi,T ; lea r15,Tend
    a.raw('4C 8B 65 6F')                                       # mov r12,[rbp+0x6f]  (owned vector)
    a.raw('4C 39 FE'); a.jcc('84', 'DONE')                     # cmp rsi,r15 ; je DONE
    a.jmp(RVA['LOOP_BODY'])
    a.label('DONE')
    a.raw('4C 8B 65 67'); a.raw('48 8B 45 C7')                 # the two instructions displaced from S2
    a.jmp(RVA['RESULTS'])
    while len(a.b) % 4: a.raw('90')
    a.label('T_START')
    for i in ids: a.b += struct.pack('<i', i)
    a.label('T_END')
    return a


def main():
    if len(sys.argv) < 3: print(__doc__); sys.exit(1)
    exe = sys.argv[1]
    bkdir = r'E:\ClaudeScratch\backups\client_exe'; os.makedirs(bkdir, exist_ok=True)
    orig = os.path.join(bkdir, 'yo_cm_client.exe.orig')
    if not os.path.exists(orig):
        shutil.copy2(exe, orig); print('backup created:', orig)
    if sys.argv[2] == '--restore':
        shutil.copy2(orig, exe); print('restored original exe'); return
    ids = [int(x) for x in sys.argv[2:]]
    d = bytearray(open(orig, 'rb').read())
    lf = struct.unpack_from('<I', d, 0x3c)[0]
    nsec, = struct.unpack_from('<H', d, lf + 6)
    optsz, = struct.unpack_from('<H', d, lf + 20)
    opt = lf + 24
    if struct.unpack_from('<H', d, opt)[0] != 0x20b: raise SystemExit('not PE32+')
    sect_align, file_align = struct.unpack_from('<II', d, opt + 32)
    size_of_headers, = struct.unpack_from('<I', d, opt + 60)
    size_of_image, = struct.unpack_from('<I', d, opt + 56)
    sec0 = opt + optsz
    secs = []
    for i in range(nsec):
        o = sec0 + 40 * i
        name = bytes(d[o:o + 8]).rstrip(b'\0').decode()
        vs, va, rs, rp = struct.unpack_from('<IIII', d, o + 8)
        secs.append((name, va, vs, rp, rs, o))
    if any(s[0] == '.skin' for s in secs): raise SystemExit('original exe already has a .skin section?!')
    def r2o(rva):
        for name, va, vs, rp, rs, o in secs:
            if va <= rva < va + max(vs, rs): return rp + (rva - va)
        raise SystemExit('rva %x not mapped' % rva)
    # verify original bytes at the three patch sites
    for k in ('S1', 'S2', 'S3'):
        got = bytes(d[r2o(RVA[k]):r2o(RVA[k]) + len(ORIG[k])])
        if got != ORIG[k]: raise SystemExit('patch site %s has unexpected bytes %s (expected %s)' % (k, got.hex(), ORIG[k].hex()))
    # new section
    last_end = max(s[3] + s[4] for s in secs)
    sec_hdr_pos = sec0 + 40 * nsec
    if bytes(d[sec_hdr_pos:sec_hdr_pos + 40]) != b'\0' * 40 or sec_hdr_pos + 40 > size_of_headers:
        raise SystemExit('no room for a new section header')
    new_va = (size_of_image + sect_align - 1) // sect_align * sect_align
    cave = build_cave(new_va, ids)
    # resolve needs labels; run once to compute layout, then final
    code = cave.resolve()
    raw_size = (len(code) + file_align - 1) // file_align * file_align
    # drop the authenticode overlay (signature is invalid after any patch anyway)
    d = d[:last_end] + bytearray(b'\0' * 0)
    d += code + b'\0' * (raw_size - len(code))
    hdr = b'.skin\0\0\0' + struct.pack('<IIIIIIHHI', len(code), new_va, raw_size, last_end, 0, 0, 0, 0, 0x60000020)
    d[sec_hdr_pos:sec_hdr_pos + 40] = hdr
    struct.pack_into('<H', d, lf + 6, nsec + 1)
    struct.pack_into('<I', d, opt + 56, new_va + (len(code) + sect_align - 1) // sect_align * sect_align)
    # clear certificate table + checksum
    struct.pack_into('<II', d, opt + 112 + 8 * 4, 0, 0)
    struct.pack_into('<I', d, opt + 64, 0)
    # patch sites
    def w(rva, blob): o = r2o(rva); d[o:o + len(blob)] = blob
    def rel(frm_next, tgt): return struct.pack('<i', tgt - frm_next)
    C0 = cave.labels['C0']; CHK = cave.labels['CHK']
    w(RVA['S1'], bytes.fromhex('0f84') + rel(RVA['S1'] + 6, C0))
    w(RVA['S2'], b'\xe9' + rel(RVA['S2'] + 5, C0) + b'\x90\x90\x90')
    w(RVA['S3'], b'\xe9' + rel(RVA['S3'] + 5, CHK) + b'\x90\x90\x90')
    open(exe, 'wb').write(d)
    print('patched %s: new section .skin at RVA %x (%d bytes), skin ids %s' % (exe, new_va, len(code), ids))


if __name__ == '__main__':
    main()
