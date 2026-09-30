import sys, struct, shutil, pefile, os
exe=sys.argv[1]; mode=sys.argv[2] if len(sys.argv)>2 else 'apply'
BK=r'E:\ClaudeScratch\backups\dbexpand_20260926\yo_cm_client.exe.before_dbexpand'
NEWLAST=0x1002; NEWDYN=0x1043; NEWSTART=0x403
# (rva of the imm32, old, new, description)
sites=[(0x1c7796,0x320,NEWSTART,'TypeStorage counter start')]
for r in (0x2a361d,0x2a51e9,0xa12618,0xa12718,0xa12818,0xa12918,0xa12a18):
    sites.append((r+2,0x402,NEWLAST,'readRangedU32 max @%x'%r))
sites.append((0x5c960a,0x443,NEWDYN,'Sim dynamic id start'))
for r in (0x23e5d9,0x2ac2fa,0x2ac35b):
    sites.append((r,10,12,'readInt(10)+3 datablock id (mounted images etc) @%x'%r))
if mode=='restore':
    shutil.copy2(BK,exe); print('restored'); raise SystemExit
pe=pefile.PE(exe); d=bytearray(open(exe,'rb').read())
def r2o(rva):
    for s in pe.sections:
        if s.VirtualAddress<=rva<s.VirtualAddress+max(s.Misc_VirtualSize,s.SizeOfRawData): return s.PointerToRawData+rva-s.VirtualAddress
    raise SystemExit('unmapped %x'%rva)
# verify preceding opcode context for the 5+2 read sites (mov edx,3 within 8 bytes before) and all old values
for rva,old,new,desc in sites:
    o=r2o(rva); cur=struct.unpack_from('<I',d,o)[0]
    ctx=bytes(d[o-8:o])
    ok = cur in (old,new)
    print('%-34s rva %x cur %#x expect %#x %s ctx %s'%(desc,rva,cur,old,'OK' if ok else 'MISMATCH',ctx.hex()))
    if not ok: raise SystemExit('abort: unexpected bytes')
if mode=='check': raise SystemExit
for rva,old,new,desc in sites:
    struct.pack_into('<I',d,r2o(rva),new)
pe.close()
with open(exe,'wb') as f: f.write(d)
print('patched',len(sites),'sites')
