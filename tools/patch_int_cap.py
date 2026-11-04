"""
Server exe patch (Daniel's VPS build / our build; same code at the used RVAs), v4 = hooks in one new section ".gmcs".

1. getMaxSkillAmount (RVA 0x347650) is wrapped: result += magnitude(effect 22 "Clever", 1e6 fixed) * 20  (= the engine's per-Int-point formula
   in _calcSkillCap 0x344cb0).  Every cap check (raise 0x346200, degrade 0x346610, 0x344640, 0x393e30) uses this getter.
   Effect entry (cObjEffects at player+0x11D8, entry = +0x60 + id*0x18): +0 expiry (0 = inactive, 0xFFFFFFFF = unlimited), +0x10 i64 magnitude.
   Effect 22: expiry player+0x1448, magnitude player+0x1458.  Player = FUN_140134850(charId, 0).
2. When the Int bonus is REMOVED the engine only subtracts the magnitude (cObjEffects::ChangeEffectMagnitude 0x4dc9b0, mode param 1) or disables the
   effect (DisableEffect 0x4dcbb0) and nothing re-checks the skill sums.  Both functions are wrapped: after they ran for effect 22 the engine's own
   CmSkillsPlayer::checkMaxSumSkillAmount (0x346610, same call as onStatChange makes when a cap drops) is run for the owner, so skills over the new cap
   are lowered (engine rule: only skills with the down arrow, split evenly).  Only while the character is online (FUN_140134850 non-null).
   Skills object of the owner: vfn+8 of the owner = charId, FUN_14028c6c0(DAT_140b53908 value, &holder, charId, 0) -> *holder (same as Player::onEffectChanged 0x95de0).
3. v3: the login-time check (CmCharacterInfo init 0x1bf760 calls checkMaxSumSkillAmount at 0x1bf925 BEFORE the equipment effects exist) is switched off.
4. v4: [IntCap] lines in the server log (engine log function 0x405090) for every stage, to see what runs.
Usage: python -I patch_int_cap.py <in.exe> <out.exe>
"""
import sys, struct, hashlib
from keystone import Ks, KS_ARCH_X86, KS_MODE_64
from capstone import Cs, CS_ARCH_X86, CS_MODE_64

src, dst = sys.argv[1:3]
d = bytearray(open(src, 'rb').read())
lf = struct.unpack_from('<I', d, 0x3c)[0]
nsec, = struct.unpack_from('<H', d, lf + 6)
optsz, = struct.unpack_from('<H', d, lf + 20)
opt = lf + 24
sect_align, file_align = struct.unpack_from('<II', d, opt + 32)
size_of_image, = struct.unpack_from('<I', d, opt + 56)
size_of_headers, = struct.unpack_from('<I', d, opt + 60)
sec0 = opt + optsz
secs = {}
for i in range(nsec):
    o = sec0 + 40 * i
    name = bytes(d[o:o + 8]).rstrip(b'\0').decode()
    vs, va, rs, rp = struct.unpack_from('<IIII', d, o + 8)
    secs[name] = (va, vs, rp, rs)
if '.gmcs' in secs:
    raise SystemExit('already patched')


def foff(rva):
    for va, vs, rp, rs in secs.values():
        if va <= rva < va + max(vs, rs):
            return rp + rva - va
    raise SystemExit('rva not mapped %x' % rva)


GETTER = 0x347650
GETPLAYER = 0x134850     # (charId, 0) -> player object (0x134950 returns the NET CONNECTION, not the player)
CHANGEFX = 0x4dc9b0
DISABLEFX = 0x4dcbb0
CHECKSUM = 0x346610      # CmSkillsPlayer::checkMaxSumSkillAmount(this, float stat, int type=3, bool=1)
LOOKUP = 0x28c6c0        # (mgr, &holder, charId, 0)
MGR = 0xb53908           # DAT_140b53908 (char info manager, value passed as 1st arg)
ONEMILLION = 0x73ab80    # DAT_14073ab80 = 1e6 (double)
INITTRIM = 0x1bf925      # call checkMaxSumSkillAmount inside the character-info init (login)
LOGON = 0x405040         # bool f()   (log enabled)
LOGMSG = 0x405090        # f(int level, int 0, char* msg)
SNPRINTF = 0x85d90       # snprintf(buf, n, fmt, ...) (the engine's own wrapper, called the same way in getMaxSkillAmount)
o = foff(GETTER)
pro = bytes(d[o:o + 12])
assert pro[:7] == bytes.fromhex('4053b820180000') and pro[7] == 0xE8, pro.hex()
ALLOCA = GETTER + 12 + struct.unpack_from('<i', pro, 8)[0]
assert bytes(d[foff(GETTER + 12):foff(GETTER + 12) + 3]) == bytes.fromhex('482be0')   # sub rsp,rax
oc = foff(CHANGEFX)
assert bytes(d[oc:oc + 9]) == bytes.fromhex('89542410' + '53' + '4154' + '4155'), bytes(d[oc:oc + 9]).hex()   # mov [rsp+10h],edx; push rbx; push r12; push r13
od = foff(DISABLEFX)
assert bytes(d[od:od + 10]) == bytes.fromhex('4883ec38' + '4c8bc1' + '83fa5e'), bytes(d[od:od + 10]).hex()      # sub rsp,38h; mov r8,rcx; cmp edx,5Eh
o95 = foff(0x95e15)
disp, = struct.unpack_from('<i', d, o95 + 3)
assert 0x95e15 + 7 + disp == MGR, hex(0x95e15 + 7 + disp)
print('alloca_probe at RVA %x' % ALLOCA)

nva = (size_of_image + sect_align - 1) // sect_align * sect_align
asm = '''
W:
  push rsi
  push rdi
  sub rsp, 0x28
  mov rsi, rcx
  call ORIG
  mov rdi, rax
  test rdi, rdi
  jz done
  mov rcx, [rsi + 0x188]
  test rcx, rcx
  jz done
  mov rax, [rcx]
  call qword ptr [rax + 0x30]
  mov ecx, eax
  xor edx, edx
  call %(GETPLAYER)d
  test rax, rax
  jz done
  cmp dword ptr [rax + 0x1448], 0
  je done
  mov rdx, [rax + 0x1458]
  test rdx, rdx
  jle done
  imul rdx, rdx, 20
  add rdi, rdx
done:
  mov rax, rdi
  add rsp, 0x28
  pop rdi
  pop rsi
  ret
ORIG:
  push rbx
  mov eax, 0x1820
  call %(ALLOCA)d
  jmp %(GETTERBODY)d
LOG:
  sub rsp, 0x28
  mov qword ptr [rsp + 0x20], r8
  call %(LOGON)d
  test al, al
  jz lg
  mov r8, qword ptr [rsp + 0x20]
  xor edx, edx
  mov ecx, 2
  call %(LOGMSG)d
lg:
  add rsp, 0x28
  ret
TRIM:
  sub rsp, 0xE8
  lea r8, [rip + 0x11111111]
  call LOG
  mov rcx, qword ptr [rbx + 0x58]
  test rcx, rcx
  jz tdone
  mov qword ptr [rsp + 0x40], rcx
  mov rax, qword ptr [rcx]
  call qword ptr [rax + 8]
  mov dword ptr [rsp + 0x58], eax
  mov ecx, eax
  xor edx, edx
  call %(GETPLAYER)d
  mov qword ptr [rsp + 0x28], rax
  mov eax, dword ptr [rsp + 0x58]
  mov dword ptr [rsp + 0x20], eax
  lea rcx, [rsp + 0x60]
  mov edx, 0x80
  lea r8, [rip + 0x99999999]
  mov r9, qword ptr [rsp + 0x40]
  call %(SNPRINTF)d
  lea r8, [rsp + 0x60]
  call LOG
  mov rax, qword ptr [rsp + 0x28]
  test rax, rax
  jz offline
  mov r8d, dword ptr [rsp + 0x58]
  xor r9d, r9d
  lea rdx, [rsp + 0x50]
  mov rcx, qword ptr [rip + 0x44444444]
  call %(LOOKUP)d
  mov rax, qword ptr [rsp + 0x50]
  test rax, rax
  jz tdone
  mov rcx, qword ptr [rax]
  test rcx, rcx
  jz rel3
  mov qword ptr [rsp + 0x48], rcx
  lea r8, [rip + 0x22222222]
  call LOG
  mov rcx, qword ptr [rsp + 0x48]
  mov rax, qword ptr [rcx + 0x188]
  test rax, rax
  jz rel3
  mov rax, qword ptr [rax + 0x310]
  test rax, rax
  jz rel3
  mov rax, qword ptr [rax + 0x48]
  pxor xmm1, xmm1
  cvtsi2sd xmm1, rax
  divsd xmm1, qword ptr [rip + 0x55667788]
  cvtsd2ss xmm1, xmm1
  mov r8d, 3
  mov r9d, 1
  call %(CHECKSUM)d
  lea r8, [rip + 0x33333333]
  call LOG
rel3:
  mov rax, qword ptr [rsp + 0x50]
  mov rcx, qword ptr [rax]
  cmp dword ptr [rax + 8], 2
  jne rel4
  test rcx, rcx
  je rel4
  mov eax, dword ptr [rcx + 0x58]
  shr eax, 8
  test al, 1
  je rel4
  mov rax, qword ptr [rcx]
  call qword ptr [rax + 0x60]
rel4:
  mov rcx, qword ptr [rsp + 0x50]
  mov eax, 0xffffffff
  lock xadd dword ptr [rcx + 8], eax
  jmp tdone
offline:
  lea r8, [rip + 0x66666666]
  call LOG
tdone:
  add rsp, 0xE8
  ret
W3:
  push rbx
  push rsi
  push rdi
  sub rsp, 0x40
  mov rbx, rcx
  mov esi, edx
  mov edi, r9d
  mov eax, dword ptr [rsp + 0x80]
  mov dword ptr [rsp + 0x20], eax
  call ORIGC
  cmp esi, 22
  jne out3
  lea r8, [rip + 0x77777777]
  call LOG
  cmp edi, 1
  jne out3
  call TRIM
out3:
  add rsp, 0x40
  pop rdi
  pop rsi
  pop rbx
  ret
ORIGC:
  mov dword ptr [rsp + 0x10], edx
  push rbx
  jmp %(CHANGEBODY)d
W5:
  push rbx
  push rsi
  sub rsp, 0x38
  mov rbx, rcx
  mov esi, edx
  call ORIGD
  cmp esi, 22
  jne out5
  lea r8, [rip + 0x88888888]
  call LOG
  call TRIM
out5:
  add rsp, 0x38
  pop rsi
  pop rbx
  ret
ORIGD:
  sub rsp, 0x38
  mov r8, rcx
  jmp %(DISABLEBODY)d
''' % dict(GETPLAYER=GETPLAYER, ALLOCA=ALLOCA, GETTERBODY=GETTER + 12, LOOKUP=LOOKUP, CHECKSUM=CHECKSUM, CHANGEBODY=CHANGEFX + 5,
           DISABLEBODY=DISABLEFX + 7, LOGON=LOGON, LOGMSG=LOGMSG, SNPRINTF=SNPRINTF)
ks = Ks(KS_ARCH_X86, KS_MODE_64)
code, cnt = ks.asm(asm, nva)
code = bytearray(code)
off_w3 = len(ks.asm(asm.split('W3:')[0], nva)[0])
off_w5 = len(ks.asm(asm.split('W5:')[0], nva)[0])
# RIP-relative placeholders: MGR value, 1e6 constant, and the log strings (appended after the code)
p1 = bytes(code).find(bytes.fromhex('488b0d44444444'))
assert p1 > 0
struct.pack_into('<i', code, p1 + 3, MGR - (nva + p1 + 7))
p2 = bytes(code).find(bytes.fromhex('f20f5e0d88776655'))
assert p2 > 0
struct.pack_into('<i', code, p2 + 4, ONEMILLION - (nva + p2 + 8))
strings = {
    0x11111111: b'[IntCap] Klug effect removed - checking skill caps',
    0x22222222: b'[IntCap] skills object found - running cap check',
    0x33333333: b'[IntCap] cap check done',
    0x66666666: b'[IntCap] character not online - cap check skipped',
    0x77777777: b'[IntCap] Klug effect changed (add or remove)',
    0x88888888: b'[IntCap] Klug effect disabled',
    0x99999999: b'[IntCap] debug owner=%llx charId=%u player=%llx',
}
while len(code) % 4:
    code.append(0xCC)
for ph, text in strings.items():
    pat = bytes.fromhex('4c8d05') + struct.pack('<I', ph)
    pos = bytes(code).find(pat)
    assert pos > 0, hex(ph)
    spos = len(code)
    code += text + b'\0'
    struct.pack_into('<i', code, pos + 3, (nva + spos) - (nva + pos + 7))
code = bytes(code)
print('cave: %d bytes at RVA %x, W3 at %x, W5 at %x' % (len(code), nva, nva + off_w3, nva + off_w5))

raw_size = (len(code) + file_align - 1) // file_align * file_align
last_end = max(rp_ + rs_ for (_, _, rp_, rs_) in secs.values())
assert last_end == len(d), (last_end, len(d))
hdr_pos = sec0 + 40 * nsec
assert bytes(d[hdr_pos:hdr_pos + 40]) == b'\0' * 40 and hdr_pos + 40 <= size_of_headers
d += code + b'\0' * (raw_size - len(code))
d[hdr_pos:hdr_pos + 40] = b'.gmcs\0\0\0' + struct.pack('<IIIIIIHHI', len(code), nva, raw_size, last_end, 0, 0, 0, 0, 0x60000020)
struct.pack_into('<H', d, lf + 6, nsec + 1)
struct.pack_into('<I', d, opt + 56, nva + (len(code) + sect_align - 1) // sect_align * sect_align)
struct.pack_into('<I', d, opt + 64, 0)    # checksum
d[o:o + 12] = b'\xE9' + struct.pack('<i', nva - (GETTER + 5)) + b'\xCC' * 7
d[oc:oc + 5] = b'\xE9' + struct.pack('<i', (nva + off_w3) - (CHANGEFX + 5))
d[od:od + 7] = b'\xE9' + struct.pack('<i', (nva + off_w5) - (DISABLEFX + 5)) + b'\xCC' * 2
oi = foff(INITTRIM)
assert d[oi] == 0xE8 and INITTRIM + 5 + struct.unpack_from('<i', d, oi + 1)[0] == CHECKSUM, bytes(d[oi:oi + 5]).hex()
d[oi:oi + 5] = b'\x90' * 5
open(dst, 'wb').write(d)
print('wrote', dst, len(d), 'bytes')
print('sha256', hashlib.sha256(d).hexdigest())

cs = Cs(CS_ARCH_X86, CS_MODE_64)
for rva, off, n in ((GETTER, o, 5), (CHANGEFX, oc, 5), (DISABLEFX, od, 5)):
    for i in cs.disasm(bytes(d[off:off + n]), rva):
        print('  %x: %s %s' % (i.address, i.mnemonic, i.op_str))
for i in cs.disasm(code[:len(code) - sum(len(t) + 1 for t in strings.values()) - 3], nva):
    print('  %x: %s %s' % (i.address, i.mnemonic, i.op_str))
