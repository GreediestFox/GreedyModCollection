#!/usr/bin/env python3
"""
patch_client_equip_slot17.py - give the client's equipment window (CmEquipmentControl) a panel for equipment slot 17.

Background (2026-10-04): client and server already accept equipment slots 1..17 (isValidSlot = slot-1 < 0x11), slots 15/16
are the invisible fists, 17 is unused. The window only knows 14 panels: CmEquipmentControl holds a 15-element array of
0x30-byte panel records at this+0x310 (index = slot, 1..14), immediately followed by other members at 0x5E0, and every
slot loop runs `slot < 15`. This patch:
  * enlarges the object 0x640 -> 0x680 (4 sites): the record for slot 17 lives at this+0x640 (= 0x310 + 17*0x30),
    this+0x670 is a scratch qword for the "SlotFur" name pointer;
  * constructs / destroys that record (element ctor 0x870800 / dtor 0x870A00) in the ctor and both dtors;
  * setup 0x8723D0: one extra pass after slot 14 that binds the panels named "SlotFurPnl"/"SlotFurIcon"... to slot 17;
  * panel reset 0x8716B0: one extra pass for the slot-17 record;
  * the 4 slot loops continue 14 -> 17 (skipping the fists 15/16) and the 10 `slot < 15` guards also admit 17.
No absolute addresses are used (the exe is ASLR). Code caves go into the unused tails of our own sections .skin/.crop/.subal.

Usage:  python patch_client_equip_slot17.py <yo_cm_client.exe>
        python patch_client_equip_slot17.py <yo_cm_client.exe> --restore
"""
import os, shutil, struct, sys
import pefile
from capstone import Cs, CS_ARCH_X86, CS_MODE_64

BK = r'E:\ClaudeScratch\backups\furslot17_20261004\yo_cm_client.exe.before_slot17'
OLD_SIZE, NEW_SIZE = 0x640, 0x680
REC17, SCRATCH = 0x640, 0x670

SIZE_SITES = [(0xF2609, 'c744242040060000', 4), (0x8702DD, 'b940060000', 1), (0x87211F, 'b940060000', 1), (0x870B91, 'ba40060000', 1)]
# guards: cmp <r8>,0xf ; jnc skip
GUARDS = [0x871448, 0x871561, 0x8715FE, 0x8717F1, 0x8718F8, 0x871930, 0x871DA1, 0x871E3E, 0x871F41, 0x872BE2]
# loop ends: cmp <r8>,0xf ; jc top
LOOPS = [0x8713BB, 0x8714EE, 0x871588, 0x873CCD]
REG = {'bl': ('80fb', 'b3'), 'dl': ('80fa', 'b2'), 'sil': ('4080fe', '40b6'), 'dil': ('4080ff', '40b7')}


class Cave:
    def __init__(self, base):
        self.base, self.b, self.fix, self.lab = base, bytearray(), [], {}

    def here(self): return self.base + len(self.b)
    def raw(self, h): self.b += bytes.fromhex(h)
    def label(self, n): self.lab[n] = self.here()

    def rel(self, tgt):  # rel32 to absolute RVA or label
        self.fix.append((len(self.b), tgt)); self.b += b'\0\0\0\0'

    def jmp(self, t): self.raw('e9'); self.rel(t)
    def jcc(self, cc, t): self.raw('0f' + cc); self.rel(t)
    def call(self, t): self.raw('e8'); self.rel(t)

    def lea_rip(self, prefix_modrm, t):  # e.g. '488d05' lea rax,[rip+x]
        self.raw(prefix_modrm); self.rel(t)

    def done(self):
        for pos, t in self.fix:
            tgt = self.lab[t] if isinstance(t, str) else t
            self.b[pos:pos + 4] = struct.pack('<i', tgt - (self.base + pos + 4))
        return bytes(self.b)


def main():
    exe = sys.argv[1]
    if len(sys.argv) > 2 and sys.argv[2] == '--restore':
        shutil.copy2(BK, exe); print('restored', BK); return
    os.makedirs(os.path.dirname(BK), exist_ok=True)
    if not os.path.exists(BK): shutil.copy2(exe, BK); print('backup:', BK)
    d = bytearray(open(exe, 'rb').read())
    pe = pefile.PE(data=bytes(d), fast_load=True)
    md = Cs(CS_ARCH_X86, CS_MODE_64)

    def off(rva): return pe.get_offset_from_rva(rva)
    def get(rva, n): return bytes(d[off(rva):off(rva) + n])
    def put(rva, blob): d[off(rva):off(rva) + len(blob)] = blob
    def ins(rva, n=2): return list(md.disasm(get(rva, 32), rva))[:n]

    # ---------- verify everything first ----------
    for rva, hx, _ in SIZE_SITES:
        assert get(rva, len(hx) // 2).hex() == hx, ('size site', hex(rva), get(rva, 8).hex())
    plans = []
    for kind, sites in (('guard', GUARDS), ('loop', LOOPS)):
        for rva in sites:
            a, b = ins(rva)
            assert a.mnemonic == 'cmp' and a.op_str.endswith('0xf'), (hex(rva), a.mnemonic, a.op_str)
            reg = a.op_str.split(',')[0].strip()
            assert reg in REG, (hex(rva), reg)
            want = 'jae' if kind == 'guard' else 'jb'
            assert b.mnemonic == want, (hex(rva), b.mnemonic)
            plans.append((kind, rva, reg, int(b.op_str, 16), a.size + b.size))
    assert get(0x87073C, 9).hex() == '33c0488983e0050000'
    assert get(0x870B81, 9).hex() == '90488bcfe8a6edfaff'
    assert get(0x8709DA, 9).hex() == '90488bcf488b5c2440'
    assert get(0x87280E, 10).hex() == '4983ec010f859ffcffff'
    assert get(0x8716C1, 7).hex() == '488d9960030000'
    assert get(0x87177A, 10).hex() == '4883ee010f855cffffff'
    assert get(0x8724A7, 7).hex() == '488dbe48030000'   # RDI = this(RSI)+0x348 in setup
    print('all %d sites verified' % (len(plans) + 10))

    # ---------- cave space: tails of .skin / .crop / .subal ----------
    secs = {s.Name.rstrip(b'\0').decode(): s for s in pe.sections}
    pools = []
    for n in ('.skin', '.crop', '.subal'):
        s = secs[n]; start = (s.Misc_VirtualSize + 15) // 16 * 16
        pools.append([n, s, s.VirtualAddress + start, s.VirtualAddress + s.SizeOfRawData])
    used = {}

    def alloc(build):
        for p in pools:
            c = Cave(p[2]); build(c); code = c.done()
            if p[2] + len(code) <= p[3]:
                put(p[2], code)
                p[2] = (p[2] + len(code) + 15) // 16 * 16
                used[p[0]] = p[2] - p[1].VirtualAddress
                return c.base
        raise SystemExit('out of cave space')

    def site_jmp(rva, length, cave):
        put(rva, b'\xe9' + struct.pack('<i', cave - (rva + 5)) + b'\x90' * (length - 5))

    # sizes
    for rva, hx, immoff in SIZE_SITES:
        put(rva + immoff, struct.pack('<I', NEW_SIZE))

    # guards / loops
    for kind, rva, reg, tgt, length in plans:
        cmp_, movi = REG[reg]
        after = rva + length
        if kind == 'guard':
            def b(c, cmp_=cmp_, tgt=tgt, after=after):
                c.raw(cmp_ + '0f'); c.jcc('82', after)     # slot < 15 -> continue as before
                c.raw(cmp_ + '11'); c.jcc('84', after)     # slot == 17 -> continue too
                c.jmp(tgt)                                 # otherwise the original skip
        else:
            def b(c, cmp_=cmp_, movi=movi, tgt=tgt, after=after):
                c.raw(cmp_ + '0f'); c.jcc('82', tgt)       # < 15 -> next slot
                c.jcc('85', after)                         # > 15 (i.e. 18, after slot 17) -> leave loop
                c.raw(movi + '11'); c.jmp(tgt)             # == 15 -> jump to slot 17
        site_jmp(rva, length, alloc(b))

    # ctor: construct record 17
    site_jmp(0x87073C, 9, alloc(lambda c: (c.raw('488d8b' + struct.pack('<I', REC17).hex()), c.call(0x870800),
                                           c.raw('33c0488983e0050000'), c.jmp(0x870745))))
    # dtors: destroy record 17
    site_jmp(0x870B81, 9, alloc(lambda c: (c.raw('488d8f' + struct.pack('<I', REC17).hex()), c.call(0x870A00),
                                           c.raw('488bcf'), c.call(0x81F930), c.jmp(0x870B8A))))
    site_jmp(0x8709DA, 9, alloc(lambda c: (c.raw('488d8f' + struct.pack('<I', REC17).hex()), c.call(0x870A00),
                                           c.raw('488bcf488b5c2440'), c.jmp(0x8709E3))))

    # setup: extra pass for slot 17 ("SlotFur")
    def setup(c):
        c.raw('4983ec01'); c.jcc('85', 0x8724B7)                       # sub r12,1 ; jnz top
        c.raw('488d86' + struct.pack('<I', SCRATCH + 8).hex())        # lea rax,[rsi+SCRATCH+8]
        c.raw('4c39f0'); c.jcc('84', 0x872818)                         # cmp rax,r14 ; je done (extra pass finished)
        c.lea_rip('488d05', 'name')                                    # lea rax,[rip+"SlotFur"]
        c.raw('488986' + struct.pack('<I', SCRATCH).hex())            # mov [rsi+SCRATCH],rax
        c.raw('4c8db6' + struct.pack('<I', SCRATCH).hex())            # lea r14,[rsi+SCRATCH]
        c.raw('488dbe' + struct.pack('<I', REC17 + 8).hex())          # lea rdi,[rsi+0x648]
        c.raw('41bc01000000'); c.jmp(0x8724B7)                         # mov r12d,1 ; jmp top
        c.label('name'); c.b += b'SlotFur\0'
    site_jmp(0x87280E, 10, alloc(setup))

    # panel reset 0x8716B0: keep this in RDI, then one extra pass on record 17 (+0x20)
    site_jmp(0x8716C1, 7, alloc(lambda c: (c.raw('488d9960030000'), c.raw('488bf9'), c.jmp(0x8716C8))))
    def reset_end(c):
        c.raw('4883ee01'); c.jcc('85', 0x8716E0)                       # sub rsi,1 ; jnz top
        c.raw('488d87' + struct.pack('<I', REC17 + 0x20 + 0x30).hex())  # lea rax,[rdi+0x690]
        c.raw('4839c3'); c.jcc('84', 0x871784)                         # cmp rbx,rax ; je done
        c.raw('488d9f' + struct.pack('<I', REC17 + 0x20).hex())       # lea rbx,[rdi+0x660]
        c.raw('be01000000'); c.jmp(0x8716E0)                           # mov esi,1 ; jmp top
    site_jmp(0x87177A, 10, alloc(reset_end))

    # extend VirtualSize of the used sections
    for p in pools:
        if p[0] in used:
            hoff = p[1].get_file_offset()
            struct.pack_into('<I', d, hoff + 8, used[p[0]])
    lf = struct.unpack_from('<I', d, 0x3c)[0]
    struct.pack_into('<I', d, lf + 24 + 64, 0)   # checksum
    open(exe, 'wb').write(d)
    print('patched; cave bytes used per section:', {k: hex(v) for k, v in used.items()})


if __name__ == '__main__':
    main()
