"""Client twin of the Plus plowCartParity hook (2026-10-04): yo_cm_client.exe RVA 0x154F1D,
je +7 (74 07) -> nop nop, so the plough takes the same cart tuning branch as on the server. --restore undoes it."""
import os, shutil, sys, pefile
EXE = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\yo_cm_client.exe'
BK = r'E:\ClaudeScratch\backups\agri_20261004\yo_cm_client.exe.before_plowparity'
pe = pefile.PE(EXE, fast_load=True); off = pe.get_offset_from_rva(0x154F1D); pe.close()
b = bytearray(open(EXE, 'rb').read())
assert bytes(b[off - 6:off]) == bytes.fromhex('bbd82a000035'), b[off - 6:off].hex()
want, new = (b'\x90\x90', b'\x74\x07') if '--restore' in sys.argv else (b'\x74\x07', b'\x90\x90')
if bytes(b[off:off + 2]) == new:
    print('already', 'restored' if '--restore' in sys.argv else 'patched'); sys.exit()
assert bytes(b[off:off + 2]) == want, b[off:off + 2].hex()
if not os.path.exists(BK): shutil.copy2(EXE, BK)
b[off:off + 2] = new
open(EXE, 'wb').write(b)
print('done at file offset', hex(off), '->', new.hex())
