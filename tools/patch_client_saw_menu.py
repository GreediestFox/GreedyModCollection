#!/usr/bin/env python3
"""
patch_client_saw_menu.py - client twin of Daniel's hook_saw_menu (tree felling, 2026-10-04).

The client offers the "Saw out a Billet/Board/Building Log" interaction category only when the targeted movable object
is ObjectTypeID 626 (Hardwood Log) or 653 (Softwood Log); this is a hard-coded compare at RVA 0x505862:

    cmp eax, 0x272 ; je 0x505875      (626 -> matched)
    cmp eax, 0x28D ; je 0x505875      (653 -> matched)
    xor sil, sil   ; jmp 0x505887     (no match)

The 19 bytes are replaced by a jmp into a new executable section ".saw" (+14 NOPs). The cave repeats the compare for
626, 653 and every extra id given on the command line, then jumps to the same two targets. Must match the server-side
lists (skill_types abilities 37-39 and the Plus abilityEntityCheck section).

Usage:  python patch_client_saw_menu.py <yo_cm_client.exe> <id> [<id> ...]      e.g. 3930 3932
        python patch_client_saw_menu.py <yo_cm_client.exe> --restore
"""
import sys, os, struct, shutil

SITE = 0x505862
MATCHED = 0x505875
NOMATCH = 0x505887
ORIG = bytes.fromhex('3d72020000740c3d8d0200007405' '4032f6eb12')
BKDIR = r'E:\ClaudeScratch\backups\treefelling_20261004'


def main():
    if len(sys.argv) < 3:
        print(__doc__); sys.exit(1)
    exe = sys.argv[1]
    os.makedirs(BKDIR, exist_ok=True)
    orig = os.path.join(BKDIR, 'yo_cm_client.exe.before_sawmenu')
    if sys.argv[2] == '--restore':
        shutil.copy2(orig, exe); print('restored', orig); return
    ids = [int(x) for x in sys.argv[2:]]
    if not os.path.exists(orig):
        shutil.copy2(exe, orig); print('backup created:', orig)

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
    if any(s[0] == '.saw' for s in secs):
        raise SystemExit('a .saw section already exists - restore the backup first')

    def r2o(rva):
        for name, va, vs, rp, rs, o in secs:
            if va <= rva < va + max(vs, rs):
                return rp + (rva - va)
        raise SystemExit('rva %x not mapped' % rva)

    got = bytes(d[r2o(SITE):r2o(SITE) + len(ORIG)])
    if got != ORIG:
        raise SystemExit('site has unexpected bytes %s - different client build?' % got.hex())

    # The section table is full, so the cave goes into the unused tail of our own last section (.subal, added by
    # patch_client_substance_alias.py): its raw data is file-aligned and longer than its VirtualSize.
    name, va, vs, rp, rs, hoff = secs[-1]
    if name != '.subal':
        raise SystemExit('expected .subal as last section, found %s' % name)
    cave_off = (vs + 15) // 16 * 16
    new_va = va + cave_off

    code = bytearray()

    def rel_to(target):
        nxt = new_va + len(code) + 4
        code.extend(struct.pack('<i', target - nxt))

    for oid in [626, 653] + ids:
        code.extend(b'\x3d' + struct.pack('<I', oid))     # cmp eax, imm32
        code.extend(b'\x0f\x84'); rel_to(MATCHED)          # je matched
    code.extend(b'\x40\x32\xf6')                           # xor sil, sil
    code.extend(b'\xe9'); rel_to(NOMATCH)                  # jmp no-match convergence
    if cave_off + len(code) > rs:
        raise SystemExit('not enough room in .subal (%d free, %d needed)' % (rs - cave_off, len(code)))
    if bytes(d[rp + cave_off:rp + cave_off + len(code)]) != b'\0' * len(code):
        raise SystemExit('.subal tail is not empty - already patched?')
    d[rp + cave_off:rp + cave_off + len(code)] = code
    struct.pack_into('<I', d, hoff + 8, cave_off + len(code))   # extend .subal VirtualSize over the cave
    if va + cave_off + len(code) > size_of_image:
        raise SystemExit('cave would exceed SizeOfImage')
    struct.pack_into('<I', d, opt + 64, 0)                 # checksum

    jmp = b'\xe9' + struct.pack('<i', new_va - (SITE + 5)) + b'\x90' * (len(ORIG) - 5)
    o = r2o(SITE); d[o:o + len(jmp)] = jmp
    open(exe, 'wb').write(d)
    print('patched %s: cave in .subal at RVA %x (%d bytes), extra log ids %s' % (exe, new_va, len(code), ids))


if __name__ == '__main__':
    main()
