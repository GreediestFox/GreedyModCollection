import os, zipfile
b=r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server\mods\LiFx'
def rd(p): return open(p,'rb').read().decode('utf-8')
def must(s,old,new):
    assert old in s, old[:80]; return s.replace(old,new,1)
nl='\r\n'
# GM: no LiFx framework on the VPS -> create the tables directly and call setup() at the end of the file
gm=rd(b+r'\GMCommands\mod.cs')
n=nl if nl in gm else '\n'
gm=must(gm,'LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxGMCommands);','LiFxGMCommands::dbChanges();   // lifmod server: no LiFx framework, create the tables right away')
gm=gm.rstrip()+n+n+'// lifmod server: no LiFx loader calls setup(), so do it here'+n+'LiFxGMCommands::setup();'+n
gm=gm.replace('mods/LiFx/GMCommands/','scripts/lifmod/gm/')
vip=rd(b+r'\VipSlotsPack\mod.cs')
vip=must(vip,'LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxVipSlots);','LiFxVipSlots::setup();   // lifmod server: no LiFx framework, start right away')
vip=vip.replace('mods/LiFx/VipSlotsPack/','scripts/lifmod/vip/')
files={'scripts/lifmod/gm/gm_commands.cs':gm,'scripts/lifmod/gm/steamids.cs':rd(b+r'\GMCommands\steamids.cs'),
       'scripts/lifmod/vip/vip_slots.cs':vip,'scripts/lifmod/vip/config.cs':rd(b+r'\VipSlotsPack\config.cs')}
for k,v in files.items(): print(k,len(v),'registerCallback left:',v.lower().count('registercallback'))
z=r'C:\Users\MJZej\Downloads\VPS_update_steamid_lifmod.zip'
if os.path.exists(z): os.remove(z)
with zipfile.ZipFile(z,'w',zipfile.ZIP_DEFLATED) as zf:
    for k,v in files.items(): zf.writestr(k,v.encode('utf-8'))
print(os.path.getsize(z))
