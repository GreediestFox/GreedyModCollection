import struct,re
p=r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\yo_cm_client.exe'
d=open(p,'rb').read()
pe=struct.unpack_from('<I',d,0x3C)[0]; nsec=struct.unpack_from('<H',d,pe+6)[0]; opt=struct.unpack_from('<H',d,pe+20)[0]; so=pe+24+opt
secs=[]
for i in range(nsec):
    o=so+i*40; name=d[o:o+8].rstrip(b'\0').decode(); vs,va,rs,ro=struct.unpack_from('<IIII',d,o+8); secs.append((name,va,max(vs,rs),ro,rs))
def off(rva):
    for n,va,vs,ro,rs in secs:
        if va<=rva<va+vs and rva-va<rs: return ro+(rva-va)
    return None
def cstr(rva):
    o=off(rva)
    if o is None: return None
    e=d.find(b'\0',o,o+200)
    s=d[o:e]
    if len(s)>=4 and all(32<=c<127 for c in s): return s.decode()
    return None
def strings_near(rva,span=0x1800):
    out=[]
    a=off(rva-span); b=off(rva+span)
    code=d[a:b]; base=rva-span
    for m in re.finditer(rb'[\x48\x4C]\x8D[\x05\x0D\x15\x1D\x25\x2D\x35\x3D]',code):
        i=m.start()
        if i+7>len(code): continue
        rel=struct.unpack_from('<i',code,i+3)[0]; tgt=base+i+7+rel
        s=cstr(tgt)
        if s and s not in out: out.append(s)
    return out
for rva,span in ((0x9567D2,0x300),(0x60AB8B,0x300),(0x7FABAA,0x500),(0x7B25EE,0x400),(0x7FBA11,0x600),(0x82298D,0x500),(0xA2E16F,0x300),(0x81B512,0x500),(0xB85590,0x300),(0xB8949B,0x300),(0x970E3D,0x300),(0x639FC8,0x300),(0x30F8C2,0x300)):
    print('%X +-%X:'%(rva,span), strings_near(rva,span)[:10])