import re, os, shutil
Z=r'C:\Users\MJZej\AppData\Local\Temp\claude\C--Users-MJZej-Downloads\f1949999-9ecf-4b06-b671-196e1f79e05e\scratchpad\claude_zip\CLAUDE\Modpack_FIN\Charcreat\Client'
ZS=r'C:\Users\MJZej\AppData\Local\Temp\claude\C--Users-MJZej-Downloads\f1949999-9ecf-4b06-b671-196e1f79e05e\scratchpad\claude_zip\CLAUDE\Modpack_FIN\Charcreat\Server'
G=r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own'
F=r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
S=r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
BK=r'E:\ClaudeScratch\backups\charcreat_20261101'; os.makedirs(BK,exist_ok=True)
def rd(p): return open(p,'rb').read().decode('utf-8')
def wr(p,t):
    os.makedirs(os.path.dirname(p),exist_ok=True); open(p,'wb').write(t.encode('utf-8'))
# 1) the two screen files
for rel in (r'gui\forms\createCharacterWindow.gui', r'gui\scripts\createCharacterWindow.cs'):
    os.makedirs(os.path.dirname(os.path.join(F,rel)),exist_ok=True); shutil.copyfile(os.path.join(Z,rel), os.path.join(F,rel))
# 2) customisation: Daniel's file, our female underwear removal re-applied
src=rd(os.path.join(Z,r'data\cm_customisation.xml'))
assert src==rd(os.path.join(ZS,r'data\cm_customisation.xml')), 'client/server customisation of Daniel differ'
n0=len(re.findall(r'Female_Underwear',src))
out=re.sub(r'[ \t]*<(?:helmet_)?mesh>Female_Underwear_[A-Za-z_]+</(?:helmet_)?mesh>[ \t]*\r?\n','',src)
print('underwear refs %d -> %d'%(n0,len(re.findall(r'Female_Underwear',out))))
for p in (os.path.join(F,r'data\cm_customisation.xml'), os.path.join(S,r'data\cm_customisation.xml')):
    if os.path.exists(p): shutil.copyfile(p, os.path.join(BK,p.replace(':','').replace('\\','_')))
    wr(p,out)
wr(os.path.join(BK,'cm_customisation_for_VPS.xml'),out)
# 3) messages: ids of Daniel's changes on top of our files
IDS=[691,692,693,694,695,696,1021,1257,1258,1259]
def elem(t,i):
    m=re.search(r'<string id="%d">.*?</string>'%i,t,re.S); return m
def merge(ours_p,dan_p,out_p):
    o=rd(ours_p); d=rd(dan_p); ch=0
    for i in IDS:
        mo=elem(o,i); md=elem(d,i)
        if mo and md and mo.group(0)!=md.group(0):
            o=o[:mo.start()]+md.group(0)+o[mo.end():]; ch+=1
    wr(out_p,o); return ch
files=['data\\cm_messages.xml']+['data\\loc\\%s\\data\\cm_messages.xml'%l for l in ('cs','de','es','fr','hu','it','ko','pl','pt-br','ru','zh-cn','zh-tw')]
tot=0
for f in files:
    tot+=merge(os.path.join(G,f), os.path.join(Z,f), os.path.join(F,f))
print('client message strings replaced:',tot)
sp=os.path.join(S,r'data\cm_messages.xml')
print('server messages replaced:', merge(sp, os.path.join(ZS,r'data\cm_messages.xml'), sp))
