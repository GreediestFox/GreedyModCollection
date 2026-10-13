"""Client exe patch: let the Rabbit Cage (2834) pass the compiled-in stable gate like a Coop (144).

yo_cm_client.exe RVA 0x4DD0B0.. is a yes/no check "may this character use this animal building?" (it gates the
Manage / Harvest / Clean / Slaughter abilities 77-80). It reads the character's level in ebx and the building type in eax:
    133 -> level >= 60,  134 -> >= 90,  143 -> >= 30,  517 -> >= 30,  144 (Coop) -> always,  anything else -> NO
The last block (Coop) is 18 bytes (RVA 0x4DD233..0x4DD244):
    cmp eax,90h ; jne fail ; mov bl,1 ; jmp done
It becomes
    cmp eax,90h ; je  0x4DD224 (the shared 'mov bl,1 ; jmp done' of the neighbouring block) ; cmp eax,0B12h (2834) ; jne fail
so type 2834 is accepted without a level requirement, exactly like the Coop. Idempotent; run with the client closed.
"""
import os, shutil, sys, time

EXE = r"F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\yo_cm_client.exe"
BACKUP = r"E:\ClaudeScratch\backups\stablegate_exe_" + time.strftime("%Y%m%d_%H%M")
OFF = 0x4DC633     # file offset of RVA 0x4DD233 (.text: raw 0x400 / va 0x1000)
OLD = bytes.fromhex("3d90000000" "0f8532ffffff" "b301" "e92dffffff")
NEW = bytes.fromhex("3d90000000" "74ea" "3d120b0000" "0f852bffffff")
assert len(OLD) == len(NEW) == 18


def main():
    d = bytearray(open(EXE, "rb").read())
    cur = bytes(d[OFF:OFF + 18])
    if cur == NEW:
        print("already patched"); return 0
    if cur != OLD:
        print("UNEXPECTED bytes at 0x%X: %s" % (OFF, cur.hex(" "))); return 1
    # sanity: the shared success exit the new 'je' jumps to must be 'mov bl,1 ; jmp 4dd172' at RVA 0x4DD224
    if bytes(d[0x4DC624:0x4DC62B]) != bytes.fromhex("b301e947ffffff"):
        print("shared success block not as expected: %s" % bytes(d[0x4DC624:0x4DC62B]).hex(" ")); return 1
    os.makedirs(BACKUP, exist_ok=True)
    shutil.copy2(EXE, os.path.join(BACKUP, "yo_cm_client.exe"))
    d[OFF:OFF + 18] = NEW
    open(EXE, "wb").write(d)
    print("patched; backup in", BACKUP)
    return 0


if __name__ == "__main__":
    sys.exit(main())
