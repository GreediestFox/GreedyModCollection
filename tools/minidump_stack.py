import struct,sys
p=r'path/to/CrashDumps/yo_cm_client.exe.dmp'
d=open(p,'rb').read()
sig,ver,nstreams,dirrva=struct.unpack_from('<IIII',d,0)
streams={}
for i in range(nstreams):
    t,sz,rva=struct.unpack_from('<III',d,dirrva+i*12); streams[t]=(sz,rva)
# modules
sz,rva=streams[4]; n=struct.unpack_from('<I',d,rva)[0]; mods=[]
for i in range(n):
    o=rva+4+i*108; base,size=struct.unpack_from('<QI',d,o); namerva=struct.unpack_from('<I',d,o+20)[0]
    ln=struct.unpack_from('<I',d,namerva)[0]; name=d[namerva+4:namerva+4+ln].decode('utf-16le'); mods.append((base,size,name))
def mod(a):
    for b,s,nm in mods:
        if b<=a<b+s: return nm.split('\\')[-1],a-b
    return None,None
# exception
sz,rva=streams[6]; tid=struct.unpack_from('<I',d,rva)[0]; code,flags,rec,addr,nparam=struct.unpack_from('<IIQQI',d,rva+8)
ctxsz,ctxrva=struct.unpack_from('<II',d,rva+8+152)
print('exception code %08X at %016X (%s+0x%X) thread %d'%(code,addr,*mod(addr),tid))
rip=struct.unpack_from('<Q',d,ctxrva+0xF8)[0]; rsp=struct.unpack_from('<Q',d,ctxrva+0x98)[0]
print('RIP %s+0x%X  RSP %016X'%(*mod(rip),rsp))
# memory lookup: Memory64ListStream (9) or MemoryListStream (5)
regions=[]
if 9 in streams:
    sz,rva=streams[9]; cnt,base=struct.unpack_from('<QQ',d,rva); off=base
    for i in range(cnt):
        st,ds=struct.unpack_from('<QQ',d,rva+16+i*16); regions.append((st,ds,off)); off+=ds
elif 5 in streams:
    sz,rva=streams[5]; cnt=struct.unpack_from('<I',d,rva)[0]
    for i in range(cnt):
        st,ds,r=struct.unpack_from('<QII',d,rva+4+i*16); regions.append((st,ds,r))
def read(a,n):
    for st,ds,off in regions:
        if st<=a<st+ds: return d[off+(a-st):off+(a-st)+min(n,st+ds-a)]
    return b''
stack=read(rsp,0x40000); print('stack bytes read:',len(stack))
seen=[]
for i in range(0,len(stack)-8,8):
    v=struct.unpack_from('<Q',stack,i)[0]; m,o=mod(v)
    if m:
        # check it is preceded by a call (heuristic): look at bytes before
        pre=read(v-7,7)
        iscall= len(pre)==7 and (pre[2]==0xE8 or pre[5]==0xFF or pre[4]==0xFF or pre[1]==0xFF or pre[0]==0xFF)
        seen.append((i,m,o,iscall))
for i,m,o,c in seen[:70]: print('  +%05X  %s+0x%X %s'%(i,m,o,'' if c else '(?)'))
# look for embedded EXCEPTION_RECORDs on the stack (code, flags, record, address, nparams)
known={0xC0000005:'ACCESS_VIOLATION',0xC0000374:'HEAP_CORRUPTION',0xE06D7363:'C++ exception',0xC0000409:'STACK_BUFFER_OVERRUN',0x80000003:'BREAKPOINT',0xC000001D:'ILLEGAL_INSTRUCTION',0xC00000FD:'STACK_OVERFLOW',0xC0000094:'INT_DIV_ZERO',0x887A0005:'DXGI_DEVICE_REMOVED',0x8007000E:'E_OUTOFMEMORY'}
print('--- exception records found on stack:')
for i in range(0,len(stack)-0x98,8):
    code,flags=struct.unpack_from('<II',stack,i)
    if code in known and flags in (0,1,0x81,0x21):
        rec,addr,npar=struct.unpack_from('<QQI',stack,i+8); m,o=mod(addr)
        if m and npar<=15:
            params=struct.unpack_from('<%dQ'%npar,stack,i+32)
            print('  +%05X %s at %s+0x%X params=%s'%(i,known[code],m,o,[hex(x) for x in params[:4]]))
print('--- full module list of stack pointers (deduped, in order):')
last=None; k=0
for i,m,o,c in seen:
    key=m
    if m.lower() not in ('ntdll.dll',) and (m,o)!=last:
        print('  +%05X  %s+0x%X'%(i,m,o)); k+=1; last=(m,o)
    if k>110: break
print('--- frames after the AV record:')
k=0
for i,m,o,c in seen:
    if i>0x2D40+0x98 and m.lower() in ('yo_cm_client.exe',):
        print('  +%05X  %s+0x%X'%(i,m,o)); k+=1
    if k>28: break

print('=== AV context decode')
ctxoff=0x2D40-0x4F0
def reg(o): return struct.unpack_from('<Q',stack,ctxoff+o)[0]
flags=struct.unpack_from('<I',stack,ctxoff+0x30)[0]
R={n:reg(o) for n,o in (('rax',0x78),('rcx',0x80),('rdx',0x88),('rbx',0x90),('rsp',0x98),('rbp',0xA0),('rsi',0xA8),('rdi',0xB0),('r8',0xB8),('r9',0xC0),('r10',0xC8),('r14',0xE8),('r15',0xF0),('rip',0xF8))}
print('ContextFlags %X'%flags, {k:hex(v) for k,v in R.items()})
def q(a):
    b=read(a,8); return struct.unpack('<Q',b)[0] if len(b)==8 else None
sb=R['rbx']; size=q(sb); ptr=q(sb+8)
print('StringBuffer this=%X size=%s ptr=%s  cut index rdx=%d len r9=%d'%(sb,size,hex(ptr) if ptr else None,R['rdx'],R['r9']))
if ptr and size is not None:
    raw=read(ptr,min(max(size & 0xFFFFFFFF,1),200)*2); print('text in buffer: %r'%raw.decode('utf-16le',errors='replace'))
ctrl=q(R['rsp']+0x20); print('saved rbx (GuiTextEditCtrl this?) = %s'%(hex(ctrl) if ctrl else None))
if ctrl:
    for nm,o in (('blockStart',0x858),('blockEnd',0x860),('cursorPos',0x868),('undoBlockStart',0x840),('undoBlockEnd',0x848)):
        print('  %s = %s'%(nm,q(ctrl+o)))
    us=q(ctrl+0x7f8); up=q(ctrl+0x800)
    if up:
        raw=read(up,min((us or 0)&0xFFFF,200)*2); print('  undo text (text before this keystroke): %r (len %s)'%(raw.decode('utf-16le',errors='replace'),us))
    # try to find the control's internal name: scan first 0x120 bytes for pointers to ASCII strings
    for o in range(0,0x120,8):
        p=q(ctrl+o)
        if p:
            s=read(p,40)
            if s and all(32<=c<127 for c in s.split(b'\0')[0]) and 3<=len(s.split(b'\0')[0])<=39: print('  +%03X -> %r'%(o,s.split(b'\0')[0].decode()))