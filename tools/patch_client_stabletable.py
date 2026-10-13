"""Client exe patch #3 for the Rabbit Cage (2834): give the stable window a table row for it.

yo_cm_client.exe keeps the animal-building parameters in a table at RVA 0x153B9B0 (.data): 11 dwords per record
    [building type, 4 allowed animal item types, max animals, 3 numbers, flag, product item]
for 144 Coop, 143 Barn, 517 Wooden barn, 133 Small stable, 134 Large stable, then a -1 terminator record; ~25 small getters scan it
(RVA 0x329240..0x32A5FF). The table cannot grow in place (another table follows directly), so the patch
  * appends a copy of the table with one more record to the LAST section (.subal, grown by 0x200 bytes) and
  * points every RIP-relative reader of the old table (only those, targets 0x153B9B0..0x153BAB7) at the copy.
The new record (2834): rabbits only (item 1053), max 30 animals, the Coop's numbers, product 338 (the server decides what is harvested).
Idempotent. Run with the client closed. Use --dry to only print the plan."""
import os, shutil, struct, sys, time
import capstone
from capstone import x86_const as X

EXE = r"F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\yo_cm_client.exe"
BACKUP = r"E:\ClaudeScratch\backups\stabletable_exe_" + time.strftime("%Y%m%d_%H%M")
T_RVA = 0x153B9B0
T_OFF = 0x1539DB0
T_END = T_RVA + 0x108              # the 6 records (5 + terminator); the next table starts here
NEW_REC = [2834, 1053, 0, 0, 0, 30, 6, 5, 3, 0, 338]
CAVE_DELTA = 0x180                 # offset of the new table inside .subal
GROW = 0x200


def main(dry):
    d = bytearray(open(EXE, "rb").read())
    pe = struct.unpack_from("<I", d, 0x3C)[0]
    nsec = struct.unpack_from("<H", d, pe + 6)[0]
    opt = struct.unpack_from("<H", d, pe + 20)[0]
    sh = pe + 24 + opt
    secs = []
    for i in range(nsec):
        name = bytes(d[sh + 40 * i:sh + 40 * i + 8]).rstrip(b"\0").decode()
        vsize, va, rsize, raw = struct.unpack_from("<IIII", d, sh + 40 * i + 8)
        secs.append((name, va, vsize, raw, rsize, sh + 40 * i))
    sub = [s for s in secs if s[0] == ".subal"][0]
    _, sva, svs, sraw, srs, shdr = sub
    new_rva = sva + CAVE_DELTA
    new_off = sraw + CAVE_DELTA

    if svs >= 0x400 and struct.unpack_from("<I", d, new_off)[0] == 144:
        print("already patched (new table present at rva %x)" % new_rva); return 0
    if sraw + srs != len(d) or svs != 0x160 or srs != 0x200:
        print("unexpected .subal layout: raw %x size %x vsize %x file %x" % (sraw, srs, svs, len(d))); return 1
    old = bytes(d[T_OFF:T_OFF + 0x108])
    recs = [struct.unpack_from("<11i", old, 44 * i) for i in range(6)]
    if [r[0] for r in recs] != [144, 143, 517, 133, 134, -1]:
        print("unexpected table content:", [r[0] for r in recs]); return 1

    # --- readers of the old table
    text = [s for s in secs if s[0] == ".text"][0]
    _, tva, tvs, traw, trs, _ = text
    md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
    md.detail = True
    plan = []
    seen = set()
    for o in range(traw, traw + trs - 4):
        if d[o - 1] & 0xC7 != 0x05:
            continue
        disp = struct.unpack_from("<i", d, o)[0]
        rva_disp = tva + (o - traw)
        if not any(T_RVA <= rva_disp + n + disp < T_END for n in (4, 5, 8)):
            continue
        for back in (3, 2, 4, 5):
            s = o - back
            ins = next(md.disasm(bytes(d[s:s + 15]), 0x140000000 + tva + (s - traw)), None)
            if ins is None or ins.size < back + 4:
                continue
            rip = [op for op in ins.operands if op.type == X.X86_OP_MEM and op.mem.base == X.X86_REG_RIP]
            if not rip or ins.disp_offset != back:
                continue
            tgt = ins.address - 0x140000000 + ins.size + ins.disp
            if T_RVA <= tgt < T_END and s not in seen:
                seen.add(s)
                plan.append((o, s, ins, tgt))
            break
    print("readers of the stable table to retarget: %d" % len(plan))
    funcs = sorted({(p[2].address - 0x140000000) & ~0x1f for p in plan})
    print("  in code blocks around:", [hex(f) for f in funcs[:40]])
    if len(plan) != 16:
        print("unexpected number of readers - aborting"); return 1
    for o, s, ins, tgt in plan[:6]:
        print("   e.g. %x: %s %s -> table+0x%x" % (ins.address - 0x140000000, ins.mnemonic, ins.op_str, tgt - T_RVA))
    if dry:
        print("dry run: nothing written"); return 0

    os.makedirs(BACKUP, exist_ok=True)
    shutil.copy2(EXE, os.path.join(BACKUP, "yo_cm_client.exe"))
    # --- new table = 5 records + new record + terminator
    table = b"".join(struct.pack("<11i", *r) for r in recs[:5]) + struct.pack("<11i", *NEW_REC) + struct.pack("<11i", *recs[5])
    assert len(table) == 7 * 44 and CAVE_DELTA + len(table) <= 0x400
    d += b"\0" * GROW
    d[new_off:new_off + len(table)] = table
    struct.pack_into("<I", d, shdr + 8, 0x400)      # VirtualSize
    struct.pack_into("<I", d, shdr + 16, 0x400)     # SizeOfRawData
    for o, s, ins, tgt in plan:
        end_rva = ins.address - 0x140000000 + ins.size
        new_target = new_rva + (tgt - T_RVA)
        struct.pack_into("<i", d, o, new_target - end_rva)
    open(EXE, "wb").write(d)
    print("patched: new table at rva %x (file %x), %d readers retargeted; backup in %s" % (new_rva, new_off, len(plan), BACKUP))
    return 0


if __name__ == "__main__":
    sys.exit(main("--dry" in sys.argv))
