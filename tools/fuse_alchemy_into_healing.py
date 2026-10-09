"""2026-10-09: fuse Alchemy (skill 15) into Healing (skill 14).
- skill_types.xml (server + client): abilities 59 "Mix a Cocktail" (apothecary: objects 109/140/519, lvl 0) and
  60 "Transmute Into Gold!" (lvl 100) move into Healing; the Alchemy row is removed (client skill tab no longer shows it).
- skill_type row 15 removed from art\\dump.sql + sql\\dump.sql (no recipes / character skills / child skills use it).
- skill-raise effects 31/32/33 (cm_effects, server + client): Alchemy -> Healing (32 already had Healing: Alchemy dropped).
- Alchemist's outfits 305 + 3927 (cm_equipTypes, server + client): required skill 15 -> 14.
- client gui\\scripts\\skills.cs: no Alchemy node position, no Healing->Alchemy connector line.
- German display: skill 14 "Heilen & Alchemie", effect names 31-33."""
import os, re, shutil

S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
BK = r'E:\ClaudeScratch\backups\fuse_alchemy_20261009'
os.makedirs(BK, exist_ok=True)


def rd(p):
    raw = open(p, 'rb').read(); return raw.startswith(b'\xef\xbb\xbf'), raw.decode('utf-8-sig')


def wr(p, bom, t): open(p, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))


def bk(p):
    tag = ('S_' if p.startswith(S) else 'C_') + os.path.relpath(p, S if p.startswith(S) else C).replace('\\', '_')
    if not os.path.exists(os.path.join(BK, tag)): shutil.copy2(p, os.path.join(BK, tag))


def edit(p, fn):
    bk(p); bom, t = rd(p); t2 = fn(t)
    if t2 != t: wr(p, bom, t2); print('changed', p)
    else: print('UNCHANGED', p)


# 1) skill_types.xml: move abilities of row 15 to row 14, drop row 15
def fuse_skill_types(t):
    r15 = re.search(r'([ \t]*)<row>\s*<ID>15</ID>.*?</row>[ \t]*\r?\n', t, re.S)
    if not r15: return t
    abil = re.search(r'<abilities>(.*?)[ \t]*</abilities>', r15.group(0), re.S).group(1).strip('\r\n')
    t = t[:r15.start()] + t[r15.end():]
    r14 = re.search(r'<row>\s*<ID>14</ID>.*?</row>', t, re.S)
    blk = r14.group(0)
    i = blk.rfind('</abilities>'); ls = blk.rfind('\n', 0, i) + 1
    nl = '\r\n' if '\r\n' in t else '\n'
    new = blk[:ls] + abil.replace('\r\n', '\n').replace('\n', nl) + nl + blk[ls:]
    return t[:r14.start()] + new + t[r14.end():]


for p in (S + r'\data\skill_types.xml', C + r'\data\skill_types.xml'):
    edit(p, fuse_skill_types)

# 2) dump.sql: drop skill_type row 15
for p in (S + r'\art\dump.sql', S + r'\sql\dump.sql'):
    edit(p, lambda t: re.sub(r"^\(15,'Alchemy','',14,1,'Int','Agi',518,519\),\r?\n", '', t, flags=re.M))


# 3) cm_effects.xml: effects 31/32/33
def fix_effects(t):
    def eff(m):
        e = m.group(0); eid = m.group(1)
        if eid == '32':    # had Alchemy + Herbalism + Healing -> drop the Alchemy parameter
            e = re.sub(r'[ \t]*<parameter type="SKILL" skill="15" applytype="INCREASE" />\r?\n', '', e)
            e = e.replace('Skill Raised: Alchemy, Herbalism, Healing', 'Skill Raised: Herbalism, Healing')
        else:
            e = e.replace('skill="15"', 'skill="14"')
            e = e.replace('Skill Raised: Alchemy, Herbalism', 'Skill Raised: Healing, Herbalism').replace('Skill Raised: Alchemy"', 'Skill Raised: Healing"')
        return e
    return re.sub(r'<effect id="(31|32|33)".*?</effect>', eff, t, flags=re.S)


for p in (S + r'\data\cm_effects.xml', C + r'\data\cm_effects.xml'):
    edit(p, fix_effects)

# 4) cm_equipTypes.xml: outfits 305 / 3927 require Healing
for p in (S + r'\data\cm_equipTypes.xml', C + r'\data\cm_equipTypes.xml'):
    edit(p, lambda t: re.sub(r'(<object id="(?:305|3927)">.*?<skillID>)15(</skillID>)', r'\g<1>14\2', t, flags=re.S))

# 5) client skill tab
def fix_skills_cs(t):
    t = re.sub(r'[ \t]*else if \(%skill_type_id == 15\) //Alchemy\r?\n[ \t]*return "[^"]*";\r?\n', '', t)
    # Healing no longer draws the connector line to the (removed) Alchemy node
    t = t.replace('|| %skillTypeId == 12 || %skillTypeId == 14 || %skillTypeId == 22', '|| %skillTypeId == 12 || %skillTypeId == 22')
    return t


edit(C + r'\gui\scripts\skills.cs', fix_skills_cs)


# 6) German display names
def de_names(t):
    t = re.sub(r'<string id="31">[^<]*</string>', '<string id="31">Fertigkeit erhÃ¶ht: Heilen &amp; Alchemie, KrÃ¤uterkunde</string>', t)
    t = re.sub(r'<string id="32">[^<]*</string>', '<string id="32">Fertigkeit erhÃ¶ht: KrÃ¤uterkunde, Heilen &amp; Alchemie</string>', t)
    t = re.sub(r'<string id="33">[^<]*</string>', '<string id="33">Fertigkeit erhÃ¶ht: Heilen &amp; Alchemie</string>', t)
    return t


edit(C + r'\data\loc\de\data\cm_effects_name.xml', de_names)
edit(C + r'\data\loc\de\data\skill_types.xml', lambda t: re.sub(r'<string id="14">[^<]*</string>', '<string id="14">Heilen &amp; Alchemie</string>', t))

# 7) OutfitsPack DB effect names (cosmetic, keeps DB names in line with cm_effects)
edit(S + r'\mods\LiFx\OutfitsPack\mod.cs', lambda t: t.replace("'Skill Raised: Alchemy, Herbalism, Healing'", "'Skill Raised: Herbalism, Healing'")
     .replace("'Skill Raised: Alchemy, Herbalism'", "'Skill Raised: Healing, Herbalism'"))
print('backups in', BK)


# ---- 8) skill descriptions: Healing texts 178-183 also describe the former Alchemy (184-189) ----
NL = '&#xA;'
PAIRS = [(179, 185), (180, 186), (181, 187), (182, 188), (183, 189)]   # healing level text <- alchemy level text
PASSIVE = {
    'en': 'Maximum %% of Hard HP that can be healed, and maximum quality of produced cocktails' + NL + NL,
    'de': 'Die Punktzahl bestimmt den maximalen Anteil in %% von Lebenspunkten, der geheilt werden kann, und die maximale QualitÃ¤t der hergestellten Cocktails' + NL + NL,
}


def rd(p):
    raw = open(p, 'rb').read(); return raw.startswith(b'\xef\xbb\xbf'), raw.decode('utf-8-sig')


def fix(p, lang):
    tag = ('S_' if p.startswith(S) else 'C_') + os.path.relpath(p, S if p.startswith(S) else C).replace('\\', '_')
    if not os.path.exists(os.path.join(BK, tag)): shutil.copy2(p, os.path.join(BK, tag))
    bom, t = rd(p)

    def get(i): return re.search(r'<string id="%d"[^>]*>(.*?)</string>' % i, t, re.S).group(1)

    def put(i, text):
        nonlocal t
        t = re.sub(r'(<string id="%d"[^>]*>).*?(</string>)' % i, lambda m: m.group(1) + text + m.group(2), t, count=1, flags=re.S)

    for heal, alch in PAIRS:
        h, a = get(heal), get(alch)
        if a in h: continue                       # already merged
        put(heal, h.rstrip() + NL + a)
    put(178, PASSIVE[lang])
    open(p, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))
    print('merged', p)


fix(S + r'\data\cm_messages.xml', 'en')
fix(C + r'\data\cm_messages.xml', 'en')
fix(C + r'\data\loc\de\data\cm_messages.xml', 'de')


# ---- 9) blank line between the Healing and the Alchemy part ----
FILES = [r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server\data\cm_messages.xml',
         r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\data\cm_messages.xml',
         r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\data\loc\de\data\cm_messages.xml']
for p in FILES:
    raw = open(p, 'rb').read(); bom = raw.startswith(b'\xef\xbb\xbf'); t = raw.decode('utf-8-sig'); n = 0
    for i in range(179, 184):
        m = re.search(r'(<string id="%d"[^>]*>)(.*?)(</string>)' % i, t, re.S)
        body = m.group(2)
        if '&#xA;&#xA;' in body or '&#xA;' not in body: continue
        body = body.replace('&#xA;', '&#xA;&#xA;', 1); n += 1
        t = t[:m.start()] + m.group(1) + body + m.group(3) + t[m.end():]
    open(p, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))
    print(n, p)
