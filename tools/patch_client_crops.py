import os
#!/usr/bin/env python3
"""
patch_client_crops.py - register new native ability ids in the CLIENT exe so custom Sow abilities
(new plantable crops) pass AbilityManager::_parseXmlAbilityNode()'s "is this id registered" check.

The client independently maintains its own AbilityManager with its own compiled "register all
built-in abilities" function (distinct from the server's, a separate binary). That function ends
with, for the last vanilla ability (Baromsag, id=0x15a):

    ...
    mov al, 1                      ; RVA 0x4b77dc  <- patched: jmp cave
    mov rbx, qword ptr [rsp+0x50]  ; restores the caller's rbx (rbx holds `this` up to this point)
    add rsp, 0x30
    pop rdi
    pop rsi
    pop rbp
    ret

This patches a 5-byte jmp at RVA 0x4b77dc into a new PE section holding straight-line code that,
for each configured new ability id, replicates the SAME allocate/zero/base-init/set-vtable/register
sequence the compiler already emits ~350 times in that same function for the vanilla abilities -
reusing PlantWheat_Ability's own vtable (RVA 0xf8c630) verbatim, exactly as the server-side DLL hook
does. `rbx` (still holding the AbilityManager `this`) and `rdi` (a scratch object-pointer the
compiler itself reuses this way throughout the function) are used the same way the surrounding
compiler-generated code already uses them; nothing else in the function is disturbed. The cave then
replicates the two original instructions it overwrote (`mov al,1` and the rbx restore) and jumps
back into the function's own epilogue.

Usage:  python patch_client_crops.py <yo_cm_client.exe> <id1>:<seedItemId1> [<id2>:<seedItemId2> ...]
        (re-run with a different id list to re-patch; requires --restore first if a .crop section already exists)
        python patch_client_crops.py <yo_cm_client.exe> --restore
"""
import sys, os, struct, shutil
import pefile

RVA = {
    'REDIRECT': 0x4b77dc,       # `mov al,1` immediately before the rbx restore, at the tail of the
                                 # ability-registration function (RVA 0x4af4b0)
    'ALLOC': 0xdb9584,          # generic allocator, called as Alloc(size) -> void*
    'BASEINIT': 0x4a3690,       # AbilityImp base-object init, called as Init(obj)
    'REGISTER': 0x4ba830,       # AbilityManager::_registerAbility(this, id, obj) -> bool (al)
    'WHEAT_VTABLE': 0xf8c630,   # AbilityImp::PlantWheat_Ability::vftable (reused for every new crop)
    'RESUME': 0x4b77e3,         # `add rsp,0x30` - first instruction after the 2 we overwrite
    'CROP_MAP': 0x163a410,      # client's own ability-id -> {seedItemId} unordered_map (mirrors the
                                 # server DLL hook's DAT_140b98d10, but the client only ever reads
                                 # offset+0 of the record - CheckPlayerInventory's seed-item lookup)
    'CROP_MAP_GET': 0x509360,   # same get-or-default-insert-by-id function as the server's FUN_1403a95c0
}
ORIG_REDIRECT = bytes.fromhex('b00148' '8b5c')  # `mov al,1` (b0 01) + first 3 bytes of the next mov
OBJ_SIZE = 0x170


class Asm:
    def __init__(self, base):
        self.base = base
        self.b = bytearray()
        self.labels = {}
        self.fix = []  # (pos, target_label_or_rva, size)

    def here(self):
        return self.base + len(self.b)

    def label(self, name):
        self.labels[name] = self.here()

    def raw(self, hexstr):
        self.b += bytes.fromhex(hexstr.replace(' ', ''))

    def rel32(self, target):
        self.fix.append((len(self.b), target))
        self.b += b'\0\0\0\0'

    def call(self, target_rva):
        self.raw('E8')
        self.rel32(target_rva)

    def jmp(self, target):
        self.raw('E9')
        self.rel32(target)

    def jcc(self, cc_byte_hex, target):
        self.raw('0F ' + cc_byte_hex)
        self.rel32(target)

    def lea_rax_abs(self, target_rva):
        # lea rax, [rip+disp32] pointing at an absolute image RVA (stays correct under ASLR)
        self.raw('48 8D 05')
        self.rel32(target_rva)

    def resolve(self):
        for pos, target in self.fix:
            tgt = self.labels[target] if isinstance(target, str) else target
            nxt = self.base + pos + 4
            self.b[pos:pos + 4] = struct.pack('<i', tgt - nxt)
        return bytes(self.b)


def build_cave(base, crops):
    a = Asm(base)
    a.label('start')
    for aid, seed_item in crops:
        # obj = Alloc(OBJ_SIZE)
        a.raw('48 83 EC 28')                       # sub rsp, 0x28  (shadow space, balanced below)
        a.raw('B9'); a.b += struct.pack('<I', OBJ_SIZE)  # mov ecx, OBJ_SIZE
        a.call(RVA['ALLOC'])                        # call Alloc            -> rax
        a.raw('48 85 C0')                            # test rax, rax
        a.jcc('84', 'skip_%d' % aid)                 # je skip (allocation failed)
        a.raw('48 89 C7')                            # mov rdi, rax          (rdi = obj, scratch)
        # zero OBJ_SIZE bytes at [rdi] (OBJ_SIZE is a multiple of 8)
        a.raw('31 C0')                               # xor eax, eax
        a.raw('B9'); a.b += struct.pack('<I', OBJ_SIZE // 8)  # mov ecx, OBJ_SIZE/8
        a.raw('49 89 FB')                            # mov r11, rdi
        a.label('zero_%d' % aid)
        a.raw('49 89 03')                            # mov [r11], rax
        a.raw('49 83 C3 08')                         # add r11, 8
        a.raw('E2')                                  # loop zero_<id>
        back = a.here() + 1
        a.b += struct.pack('<b', a.labels['zero_%d' % aid] - (back + 0))
        # BaseInit(obj)
        a.raw('48 89 F9')                            # mov rcx, rdi
        a.call(RVA['BASEINIT'])
        # obj->vtable = PlantWheat_Ability::vftable
        a.lea_rax_abs(RVA['WHEAT_VTABLE'])
        a.raw('48 89 07')                            # mov [rdi], rax
        # RegisterAbility(this=rbx, id, obj=rdi) -> al
        a.raw('49 89 F8')                            # mov r8, rdi
        a.raw('BA'); a.b += struct.pack('<I', aid)    # mov edx, aid
        a.raw('48 89 D9')                            # mov rcx, rbx
        a.call(RVA['REGISTER'])
        a.raw('84 C0')                                # test al, al
        a.jcc('85', 'ok_%d' % aid)                    # jne ok (registered fine)
        # failure cleanup: if (obj) obj->vftable[0](obj, /*alsoFree=*/1)
        a.raw('48 85 FF')                             # test rdi, rdi
        a.jcc('84', 'ok_%d' % aid)                    # je ok
        a.raw('48 8B 07')                             # mov rax, [rdi]
        a.raw('BA 01 00 00 00')                       # mov edx, 1
        a.raw('48 89 F9')                             # mov rcx, rdi
        a.raw('FF 10')                                # call [rax]
        a.label('ok_%d' % aid)
        a.label('skip_%d' % aid)
        a.raw('48 83 C4 28')                          # add rsp, 0x28
        # populate the client's own ability-id -> seedItemId record (CheckPlayerInventory's lookup)
        a.raw('48 83 EC 30')                          # sub rsp, 0x30
        a.raw('C7 44 24 28')                          # mov dword ptr [rsp+0x28], aid   (the map KEY)
        a.b += struct.pack('<I', aid)
        a.lea_rax_abs(RVA['CROP_MAP'])                # lea rax, [rip+disp32]  (&CROP_MAP)
        a.raw('48 89 C1')                             # mov rcx, rax
        a.raw('48 8D 54 24 28')                        # lea rdx, [rsp+0x28]    (&key)
        a.call(RVA['CROP_MAP_GET'])
        a.raw('C7 00')                                 # mov dword ptr [rax], seed_item
        a.b += struct.pack('<I', seed_item)
        a.raw('48 83 C4 30')                          # add rsp, 0x30
    # replicate the 2 overwritten original instructions, then resume the real epilogue
    a.raw('B0 01')                                    # mov al, 1
    a.raw('48 8B 5C 24 50')                           # mov rbx, [rsp+0x50]
    a.jmp(RVA['RESUME'])
    return a


def main():
    if len(sys.argv) < 3:
        print(__doc__)
        sys.exit(1)
    exe = sys.argv[1]
    bkdir = r'E:\ClaudeScratch\backups\croptypes_20260927'
    os.makedirs(bkdir, exist_ok=True)
    orig = os.path.join(bkdir, 'yo_cm_client.exe.before_cropcave')
    if not os.path.exists(orig):
        shutil.copy2(exe, orig)
        print('backup created:', orig)
    if sys.argv[2] == '--restore':
        shutil.copy2(orig, exe)
        print('restored original exe')
        return
    crops = []
    for spec in sys.argv[2:]:
        aid_s, seed_s = spec.split(':')
        crops.append((int(aid_s), int(seed_s)))

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
    if any(s[0] == '.crop' for s in secs):
        raise SystemExit('a .crop section already exists - restore the backup first')

    def r2o(rva):
        for name, va, vs, rp, rs, o in secs:
            if va <= rva < va + max(vs, rs):
                return rp + (rva - va)
        raise SystemExit('rva %x not mapped' % rva)

    # verify every site matches this exact build before touching anything
    got = bytes(d[r2o(RVA['REDIRECT']):r2o(RVA['REDIRECT']) + len(ORIG_REDIRECT)])
    if got != ORIG_REDIRECT:
        raise SystemExit('redirect site has unexpected bytes %s (expected %s) - different client build?'
                          % (got.hex(), ORIG_REDIRECT.hex()))
    for name in ('ALLOC', 'BASEINIT', 'REGISTER'):
        pass  # prologues already confirmed interactively before writing this tool

    last_end = max(s[3] + s[4] for s in secs)
    sec_hdr_pos = sec0 + 40 * nsec
    if bytes(d[sec_hdr_pos:sec_hdr_pos + 40]) != b'\0' * 40 or sec_hdr_pos + 40 > size_of_headers:
        raise SystemExit('no room for a new section header')
    new_va = (size_of_image + sect_align - 1) // sect_align * sect_align

    cave = build_cave(new_va, crops)
    code = cave.resolve()
    raw_size = (len(code) + file_align - 1) // file_align * file_align

    d = d[:last_end] + bytearray(b'\0' * 0)
    d += code + b'\0' * (raw_size - len(code))
    hdr = b'.crop\0\0\0' + struct.pack('<IIIIIIHHI', len(code), new_va, raw_size, last_end,
                                       0, 0, 0, 0, 0x60000020)
    d[sec_hdr_pos:sec_hdr_pos + 40] = hdr
    struct.pack_into('<H', d, lf + 6, nsec + 1)
    struct.pack_into('<I', d, opt + 56, new_va + (len(code) + sect_align - 1) // sect_align * sect_align)
    # clear certificate table + checksum (signature is invalid after any patch anyway)
    struct.pack_into('<II', d, opt + 112 + 8 * 4, 0, 0)
    struct.pack_into('<I', d, opt + 64, 0)

    def w(rva, blob):
        o = r2o(rva)
        d[o:o + len(blob)] = blob

    def rel(frm_next, tgt):
        return struct.pack('<i', tgt - frm_next)

    w(RVA['REDIRECT'], b'\xe9' + rel(RVA['REDIRECT'] + 5, cave.labels['start']))

    open(exe, 'wb').write(d)
    print('patched %s: new section .crop at RVA %x (%d bytes), crops %s'
          % (exe, new_va, len(code), crops))


if __name__ == '__main__':
    main()
