"""Client exe patch #2 for the Rabbit Cage (2834): open the stable window for it.

CmInventory::ShowComplexObjectInventory (yo_cm_client.exe RVA ~0x410300..0x4107FF) picks the window by building type (edi):
types 107..163 through a jump table (133/134/143/144 all go to handler 0x41076A = stable window), 517/682/1110/1353 through a
compare chain, anything else -> "This device have no interface attached".

RVA 0x4105B2 holds a redundant repeat of the test `mov rcx,r12 ; call 0x1401C01F0 ; test al,al ; je ..` (16 bytes): 0x1C01F0 is the pure
getter `cmp dword [rcx+428h],0 ; setne al`, already tested 0x20 bytes earlier with the same register, so the second `je` can never be taken.
Those 16 bytes become
    cmp edi,0B12h ; jne +5 ; mov edi,90h ; nop nop nop
i.e. type 2834 is dispatched exactly like type 144 (Coop). Idempotent. Run with the client closed.
"""
import os, shutil, sys, time

EXE = r"F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\yo_cm_client.exe"
BACKUP = r"E:\ClaudeScratch\backups\stablewindow_exe_" + time.strftime("%Y%m%d_%H%M")
OFF = 0x4105B2 - 0xC00        # .text raw 0x400 / va 0x1000
OLD = bytes.fromhex("498bcc" "e836fcdaff" "84c0" "0f84c6010000")
NEW = bytes.fromhex("81ff120b0000" "7505" "bf90000000" "909090")
assert len(OLD) == len(NEW) == 16


def main(dry=False):
    d = bytearray(open(EXE, "rb").read())
    cur = bytes(d[OFF:OFF + 16])
    if cur == NEW:
        print("already patched"); return 0
    if cur != OLD:
        print("UNEXPECTED bytes at 0x%X: %s" % (OFF, cur.hex(" "))); return 1
    # the earlier, identical test (RVA 0x410592) must still be there, otherwise the second one is not redundant
    first = bytes(d[0x410592 - 0xC00:0x410592 - 0xC00 + 8])
    if first != bytes.fromhex("498bcce856fcdaff"):
        print("first test not as expected:", first.hex(" ")); return 1
    print("bytes verified" + (" (dry run, nothing written)" if dry else ""))
    if dry:
        return 0
    os.makedirs(BACKUP, exist_ok=True)
    shutil.copy2(EXE, os.path.join(BACKUP, "yo_cm_client.exe"))
    d[OFF:OFF + 16] = NEW
    open(EXE, "wb").write(d)
    print("patched; backup in", BACKUP)
    return 0


if __name__ == "__main__":
    sys.exit(main(dry="--dry" in sys.argv))
