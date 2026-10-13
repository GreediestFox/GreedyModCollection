"""Linear sweep of yo_cm_client.exe .text with capstone: find windows that mention several of the vanilla stable type ids
(133 134 143 144 517) as immediates / lea displacements, i.e. hard-coded 'is this a stable' checks."""
import struct, sys, collections
import capstone
from capstone import x86_const as X

import os
EXE = os.environ.get("SCAN_EXE", r"F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\yo_cm_client.exe")
d = open(EXE, "rb").read()
pe = struct.unpack_from("<I", d, 0x3C)[0]
nsec = struct.unpack_from("<H", d, pe + 6)[0]
opt = struct.unpack_from("<H", d, pe + 20)[0]
off = pe + 24 + opt
secs = {}
for i in range(nsec):
    name = d[off:off + 8].rstrip(b"\0").decode()
    vsize, va, rsize, raw = struct.unpack_from("<IIII", d, off + 8)
    secs[name] = (va, vsize, raw, rsize)
    off += 40
va, vs, raw, rs = secs[".text"]
code = d[raw:raw + rs]
TYPES = {133: "133", 134: "134", 143: "143", 144: "144", 517: "517"}
md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
md.detail = True
events = []    # (rva, typeid, text)
pos = 0
n = 0
while pos < len(code):
    got = False
    for ins in md.disasm(code[pos:pos + 0x200000], 0x140000000 + va + pos):
        got = True
        n += 1
        pos = ins.address - 0x140000000 - va + ins.size
        for op in ins.operands:
            val = None
            if op.type == X.X86_OP_IMM:
                val = op.imm
            elif op.type == X.X86_OP_MEM and op.mem.base != 0 and op.mem.base != X.X86_REG_RIP and op.mem.disp < 0 and ins.mnemonic == "lea":
                val = -op.mem.disp
            if val in TYPES:
                events.append((ins.address - 0x140000000, val, "%s %s" % (ins.mnemonic, ins.op_str)))
    if not got:
        pos += 1
    else:
        pass
print("instructions:", n, "events:", len(events))
# windows: >=3 distinct stable ids within 320 bytes
events.sort()
found = []
j = 0
for i in range(len(events)):
    k = i
    ids = set()
    while k < len(events) and events[k][0] - events[i][0] <= 320:
        ids.add(events[k][1]); k += 1
    if len(ids) >= 3:
        found.append((events[i][0], sorted(ids), [e[2] for e in events[i:k]]))
last = -1
for rva, ids, txt in found:
    if rva - last < 320:
        continue
    last = rva
    print("window at rva %x ids %s" % (rva, ids))
    for t in txt[:8]:
        print("     ", t)
