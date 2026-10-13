"""Client exe patch: widen the hard-coded animal-feed bit mask so item types 370, 371, 372 count as feed.

The client (like the server) decides what may go into a stable's feed trough with
    lea eax,[rcx-160h]; cmp eax,17h; ja ..; mov edx,80055Dh; bt edx,eax; jc yes
i.e. item type ids 352..375 tested against a 24 bit mask. 80055Dh -> 9C055Dh sets bits 18,19,20 = ids 370,371,372.
Two sites in yo_cm_client.exe (found by the byte pattern). Idempotent. Run with the client closed.
"""
import os, shutil, sys, time

EXE = r"F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\yo_cm_client.exe"
BACKUP = r"E:\ClaudeScratch\backups\feedmask_exe_" + time.strftime("%Y%m%d_%H%M")
SITES = [0x32992A, 0x329961]   # file offset of the third mask byte (0x80 -> 0x9C)
CTX = {0x32992A: bytes.fromhex("8da1fffeffff83f817770aba5d05"), 0x329961: None}


def main():
    with open(EXE, "rb") as f:
        d = bytearray(f.read())
    todo = []
    for off in SITES:
        # context: ... ba 5d 05 <80|9c> 00 0f a3 c2  (mov edx,imm32; bt edx,eax)
        if d[off - 3:off] != bytes.fromhex("ba5d05") or d[off + 1] != 0x00 or d[off + 2:off + 5] != bytes.fromhex("0fa3c2"):
            print("UNEXPECTED bytes at 0x%X: %s" % (off, d[off - 3:off + 5].hex(" ")))
            return 1
        if d[off] == 0x9C:
            print("0x%X already patched" % off)
        elif d[off] == 0x80:
            todo.append(off)
        else:
            print("UNEXPECTED mask byte %02X at 0x%X" % (d[off], off))
            return 1
    if not todo:
        print("nothing to do")
        return 0
    os.makedirs(BACKUP, exist_ok=True)
    shutil.copy2(EXE, os.path.join(BACKUP, "yo_cm_client.exe"))
    for off in todo:
        d[off] = 0x9C
    with open(EXE, "wb") as f:
        f.write(d)
    print("patched %d site(s); backup in %s" % (len(todo), BACKUP))
    return 0


if __name__ == "__main__":
    sys.exit(main())
