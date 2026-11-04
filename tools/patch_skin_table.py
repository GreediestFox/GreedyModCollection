"""
Makes every skin of the game "owned" in the client's wardrobe: moves the wardrobe's extra-skin table (used by our .skin cave,
see DtsTools/patch_client_skins.py) into a new data section and fills it with ALL skin ids.
Usage: python patch_skin_table.py <in.exe> <out.exe> <skin_ids.json>
"""
import sys, struct, json, hashlib

src, dst, idsf = sys.argv[1:4]
ids = sorted(set(json.load(open(idsf))))
d = bytearray(open(src, 'rb').read())
lf = struct.unpack_from('<I', d, 0x3c)[0]
nsec, = struct.unpack_from('<H', d, lf + 6)
optsz, = struct.unpack_from('<H', d, lf + 20)
opt = lf + 24
sect_align, file_align = struct.unpack_from('<II', d, opt + 32)
size_of_headers, = struct.unpack_from('<I', d, opt + 60)
size_of_image, = struct.unpack_from('<I', d, opt + 56)
sec0 = opt + optsz
secs = {}
for i in range(nsec):
    o = sec0 + 40 * i
    name = bytes(d[o:o + 8]).rstrip(b'\0').decode()
    vs, va, rs, rp = struct.unpack_from('<IIII', d, o + 8)
    secs[name] = (va, vs, rp, rs, o)
if '.sknt' in secs:
    raise SystemExit('already patched')
va, vs, rp, rs, _ = secs['.skin']
sec = bytes(d[rp:rp + vs])
# the old table: the single id 990101 (0x000F1B95)
pos = sec.index(struct.pack('<i', 990101))
t_start = va + pos
t_end = t_start + 4
print('old table at RVA %x' % t_start)
# find the RIP-relative lea instructions pointing at T_START / T_END inside the cave
sites = []
for off in range(0, pos):
    pre = sec[off:off + 3]
    if pre in (b'\x48\x8d\x0d', b'\x48\x8d\x05', b'\x48\x8d\x35', b'\x4c\x8d\x3d'):
        disp, = struct.unpack_from('<i', sec, off + 3)
        target = va + off + 7 + disp
        if target in (t_start, t_end):
            sites.append((off + 3, target == t_start))
print('lea sites:', [(hex(o), 'start' if s else 'end') for o, s in sites])
if len(sites) != 6:
    raise SystemExit('expected 6 lea sites, found %d' % len(sites))
# new section (code + table): a NEW first-pass check so Steam-listed skins that are in the table are skipped there (the second pass lists them as owned - no duplicates, no shop button)
nva = (size_of_image + sect_align - 1) // sect_align * sect_align
table = b''.join(struct.pack('<i', i) for i in ids)
NEXT, OWNED, ORIG_CONT, S3 = 0xa68077, 0xa6801d, 0xa67fc8, 0xa67fc0
# code first, then the table (4-aligned); every rel32 is computed once the layout is known
code = bytearray()
fix = []          # (position of rel32, target RVA or label)
labels = {}
def emit(h): code.extend(bytes.fromhex(h.replace(' ', '')))
def rel(target): fix.append((len(code), target)); code.extend(b'\0\0\0\0')
def label(n): labels[n] = len(code)
label('CHK')
emit('84 C0'); emit('0F 84'); rel(NEXT)                     # test al,al ; je NEXT
emit('48 8D 0D'); rel('T_START'); emit('48 39 CE'); emit('0F 82'); rel('FIRST')    # lea rcx,T ; cmp rsi,rcx ; jb FIRST
emit('48 8D 0D'); rel('T_END'); emit('48 39 CE'); emit('0F 83'); rel('FIRST')      # lea rcx,Tend ; cmp rsi,rcx ; jae FIRST
emit('E9'); rel(OWNED)                                     # second pass (rsi inside the table): owned
label('FIRST')                                             # first pass over the Steam list: rax/rcx/rdx are dead here
emit('8B 16')                                              # mov edx,[rsi]
emit('48 8D 05'); rel('T_START'); emit('48 8D 0D'); rel('T_END')   # lea rax,T ; lea rcx,Tend
label('LOOP')
emit('48 39 C8'); emit('0F 83'); rel('ORIG')               # cmp rax,rcx ; jae ORIG
emit('39 10'); emit('0F 84'); rel(NEXT)                    # cmp [rax],edx ; je NEXT  (id is in the table: the second pass lists it)
emit('48 83 C0 04'); emit('E9'); rel('LOOP')               # add rax,4 ; jmp LOOP
label('ORIG'); emit('E9'); rel(ORIG_CONT)
while len(code) % 4: emit('90')
label('T_START')
tbl_off = len(code)
code.extend(table)
label('T_END')
table_total = bytes(code)
raw_size = (len(table_total) + file_align - 1) // file_align * file_align
last_end = max(rp_ + rs_ for (_, _, rp_, rs_, _) in secs.values())
assert last_end == len(d), (last_end, len(d))
hdr_pos = sec0 + 40 * nsec
assert bytes(d[hdr_pos:hdr_pos + 40]) == b'\0' * 40 and hdr_pos + 40 <= size_of_headers
for pos, tgt in fix:
    t_rva = nva + labels[tgt] if isinstance(tgt, str) else tgt
    struct.pack_into('<i', code, pos, t_rva - (nva + pos + 4))
table_total = bytes(code)
d += table_total + b'\0' * (raw_size - len(table_total))
d[hdr_pos:hdr_pos + 40] = b'.sknt\0\0\0' + struct.pack('<IIIIIIHHI', len(table_total), nva, raw_size, last_end, 0, 0, 0, 0, 0x60000020)
struct.pack_into('<H', d, lf + 6, nsec + 1)
struct.pack_into('<I', d, opt + 56, nva + (len(table_total) + sect_align - 1) // sect_align * sect_align)
struct.pack_into('<I', d, opt + 64, 0)    # checksum
n_start = nva + labels['T_START']
n_end = nva + labels['T_END']
for off, is_start in sites:
    fpos = rp + off
    nxt = va + off + 4
    struct.pack_into('<i', d, fpos, (n_start if is_start else n_end) - nxt)
# hook site S3: its jmp now goes to the new CHK
s3o = None
for name, (sva, svs, srp, srs, _) in secs.items():
    if sva <= S3 < sva + max(svs, srs):
        s3o = srp + (S3 - sva)
assert d[s3o] == 0xE9, hex(d[s3o])
old_tgt = S3 + 5 + struct.unpack_from('<i', d, s3o + 1)[0]
assert va <= old_tgt < va + vs, hex(old_tgt)
struct.pack_into('<i', d, s3o + 1, (nva + labels['CHK']) - (S3 + 5))
open(dst, 'wb').write(d)
print('wrote', dst, len(d), 'bytes; table of', len(ids), 'skins at RVA %x' % nva)
print('sha256', hashlib.sha256(d).hexdigest())
