import os
import pefile,shutil,os,hashlib
exe=r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\yo_cm_client.exe'
bk=r'E:\ClaudeScratch\backups\client_textedit_cut_fix_20260930'; os.makedirs(bk,exist_ok=True)
dst=os.path.join(bk,'yo_cm_client.exe.before_cutfix'); shutil.copy2(exe,dst); assert os.path.getsize(dst)==os.path.getsize(exe)
pe=pefile.PE(exe,fast_load=True)
patches=[(0x60A705,bytes([0x73,0x29]),bytes([0x7D,0x29])),(0x60A72E,bytes([0x72,0xE0]),bytes([0x7C,0xE0]))]
offs=[(pe.get_offset_from_rva(r),o,n,r) for r,o,n in patches]; pe.close()
d=bytearray(open(exe,'rb').read())
for off,o,n,r in offs:
    cur=bytes(d[off:off+2]); print('RVA %X file offset %X: %s'%(r,off,cur.hex()))
    assert cur==o, 'unexpected bytes'
    d[off:off+2]=n
open(exe,'wb').write(d)
v=open(exe,'rb').read()
for off,o,n,r in offs: assert v[off:off+2]==n
print('patched OK; backup:',dst)