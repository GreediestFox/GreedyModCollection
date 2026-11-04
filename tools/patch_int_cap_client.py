"""
Client exe patch (display side of patch_int_cap.py), v2 = two hooks in one new section ".gmcc".

1. getMaxSkillAmount (client RVA 0x4d4d80; used by updateMaxSkillcap 0x4d5d40 / 0x4d7380) is wrapped: result += magnitude(effect 22, 1e6 fixed) * 20,
   read from the local player's own effect list (what the effect icon shows).
   Client layout: local player = FUN_1402a6830() -> conn; conn+0xf80 -> ptr -> player (flags&0x400). Effect entry of id = player+0x1A30+id*0x60
   (the effect panel row builder 0x931e90 reads it so; cObjEffects itself = player+0x19E8 = cp(0x12B8)+0x730): expiry u32 at entry+0x48 (0 = inactive),
   magnitude i64 (1e6 fixed) at entry+0x58 (the drunk code reads effect 25 at player+0x23E8 = 0x1A30+25*0x60+0x58).
   Effect 22: expiry player+0x22B8, magnitude player+0x22C8.  (v1/v2 wrongly used 0x2C60/0x2C70 = the effect-25 entry mistaken for the container.)
2. v2: the window only recomputes the cap number when the server sends its cap table (login, before the ring effect exists), so the effect-change
   notifier (RVA 0x936d50, (list, effectId)) is wrapped: after it ran for effect 22 the skills window is refreshed with the engine's own
   updateMaxSkillcap (0x4d7380) on the CmSkillsPlayer, fetched the same way EventSkillToClient::unpack does (0x4d19f0 on the global 0x161d030,
   shared-ref released like the original).
Usage: python -I patch_int_cap_client.py <in.exe> <out.exe>
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
if '.gmcc' in secs:
    raise SystemExit('already patched')


def foff(rva):
    for va, vs, rp, rs in secs.values():
        if va <= rva < va + max(vs, rs):
            return rp + rva - va
    raise SystemExit('rva not mapped %x' % rva)


GETTER = 0x4d4d80
LOCALCONN = 0x2a6830
NOTIFY = 0x936d50
GLOBAL = 0x161d030
LOOKUP = 0x4d19f0
REFRESH = 0x4d7380
SKILLUPD = 0x4d72f0      # v4: CmSkillsPlayer skill-amount update (skills, skillId): the cap number is refreshed after every skill change
og = foff(GETTER)
ou = foff(SKILLUPD)
assert bytes(d[ou:ou + 9]) == bytes.fromhex('4057' '4883ec40' '48c744'), bytes(d[ou:ou + 9]).hex()
assert bytes(d[og:og + 12]) == bytes.fromhex('40534881ec20080000' + '4c8bc9'), bytes(d[og:og + 12]).hex()
on = foff(NOTIFY)
assert bytes(d[on:on + 15]) == bytes.fromhex('48895c2408' + '48896c2410' + '4889742418'), bytes(d[on:on + 15]).hex()
# the lookup helper must still be what the unpack code calls: mov rcx,[rip+x] ; lea rdx,[rsp+0x850] ; call 0x4d19f0 at 0x4cdb55
o2 = foff(0x4cdb55)
disp, = struct.unpack_from('<i', d, o2 + 3)
assert 0x4cdb55 + 7 + disp == GLOBAL, hex(0x4cdb55 + 7 + disp)

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
  call %d
  test rax, rax
  jz done
  mov rcx, [rax + 0xf80]
  test rcx, rcx
  jz done
  mov rcx, [rcx]
  test rcx, rcx
  jz done
  test dword ptr [rcx + 0x10], 0x400
  jz done
  cmp dword ptr [rcx + 0x22B8], 0
  je done
  mov rdx, [rcx + 0x22C8]
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
  sub rsp, 0x820
  jmp %d
W2:
  push rsi
  push rdi
  sub rsp, 0x48
  mov edi, edx
  call ORIGN
  cmp edi, 22
  jne d2
  mov rcx, qword ptr [rip + 0x11223344]
  lea rdx, [rsp + 0x20]
  call %d
  mov rax, qword ptr [rsp + 0x20]
  test rax, rax
  jz d2
  mov rcx, qword ptr [rax]
  test rcx, rcx
  jz rel
  call %d
rel:
  mov rax, qword ptr [rsp + 0x20]
  mov rcx, qword ptr [rax]
  cmp dword ptr [rax + 8], 2
  jne rel2
  test rcx, rcx
  je rel2
  mov eax, dword ptr [rcx + 0x58]
  shr eax, 8
  test al, 1
  je rel2
  mov rax, qword ptr [rcx]
  call qword ptr [rax + 0x60]
rel2:
  mov rcx, qword ptr [rsp + 0x20]
  mov eax, 0xffffffff
  lock xadd dword ptr [rcx + 8], eax
d2:
  add rsp, 0x48
  pop rdi
  pop rsi
  ret
ORIGN:
  mov qword ptr [rsp + 8], rbx
  mov qword ptr [rsp + 0x10], rbp
  jmp %d
W4:
  push rsi
  push rdi
  sub rsp, 0x28
  mov rsi, rcx
  call ORIGU
  mov rcx, rsi
  call %d
  add rsp, 0x28
  pop rdi
  pop rsi
  ret
ORIGU:
  push rdi
  sub rsp, 0x40
  jmp %d
''' % (LOCALCONN, GETTER + 9, LOOKUP, REFRESH, NOTIFY + 10, REFRESH, SKILLUPD + 6)
ks = Ks(KS_ARCH_X86, KS_MODE_64)


def label_off(name):
    pre = asm.split(name + ':')[0]
    pre = pre.replace('call ORIGN', 'call %d' % nva).replace('call ORIG\n', 'call %d\n' % nva).replace('call ORIGU', 'call %d' % nva)
    return len(ks.asm(pre, nva)[0])


# keystone needs every label defined while assembling the pieces before it, so resolve the three call targets via a first pass
off_orig = label_off('ORIG')
off_w2 = label_off('W2')
off_orign = label_off('ORIGN')
off_w4 = label_off('W4')
off_origu = label_off('ORIGU')
body = (asm.replace('call ORIG\n', 'call %d\n' % (nva + off_orig)).replace('call ORIGN', 'call %d' % (nva + off_orign))
        .replace('call ORIGU', 'call %d' % (nva + off_origu)))
code, cnt = ks.asm(body, nva)
code = bytearray(code)
# patch the RIP-relative load of the global
pos = bytes(code).find(bytes.fromhex('488b0d44332211'))
assert pos > 0
struct.pack_into('<i', code, pos + 3, GLOBAL - (nva + pos + 7))
code = bytes(code)
print('cave: %d bytes at RVA %x (W2 at %x, ORIG at %x, ORIGN at %x)' % (len(code), nva, nva + off_w2, nva + off_orig, nva + off_orign))

raw_size = (len(code) + file_align - 1) // file_align * file_align
last_end = max(rp_ + rs_ for (_, _, rp_, rs_) in secs.values())
assert last_end == len(d), (last_end, len(d))
hdr_pos = sec0 + 40 * nsec
assert bytes(d[hdr_pos:hdr_pos + 40]) == b'\0' * 40 and hdr_pos + 40 <= size_of_headers
d += code + b'\0' * (raw_size - len(code))
d[hdr_pos:hdr_pos + 40] = b'.gmcc\0\0\0' + struct.pack('<IIIIIIHHI', len(code), nva, raw_size, last_end, 0, 0, 0, 0, 0x60000020)
struct.pack_into('<H', d, lf + 6, nsec + 1)
struct.pack_into('<I', d, opt + 56, nva + (len(code) + sect_align - 1) // sect_align * sect_align)
struct.pack_into('<I', d, opt + 64, 0)
d[og:og + 9] = b'\xE9' + struct.pack('<i', nva - (GETTER + 5)) + b'\xCC' * 4
d[on:on + 10] = b'\xE9' + struct.pack('<i', (nva + off_w2) - (NOTIFY + 5)) + b'\xCC' * 5
d[ou:ou + 6] = b'\xE9' + struct.pack('<i', (nva + off_w4) - (SKILLUPD + 5)) + b'\xCC'
open(dst, 'wb').write(d)
print('wrote', dst, len(d), 'bytes')
print('sha256', hashlib.sha256(d).hexdigest())
cs = Cs(CS_ARCH_X86, CS_MODE_64)
for rva, off, n in ((GETTER, og, 5), (NOTIFY, on, 5)):
    for i in cs.disasm(bytes(d[off:off + n]), rva):
        print('  %x: %s %s' % (i.address, i.mnemonic, i.op_str))
for i in cs.disasm(code, nva):
    print('  %x: %s %s' % (i.address, i.mnemonic, i.op_str))
