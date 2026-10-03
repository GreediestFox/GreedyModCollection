"""2026-10-03: separate the craft outfits from their skins.
Geselle = normal outfit, Meister = former skin as its own item (harder recipe, Tailor's Workshop 2836 only).
Each outfit gets ONE merged skill effect (one cm_effects entry with several <parameter type="SKILL"> lines).

Player effect slots (cm_effects.xml, server == client):
  reused as-is : 26 Forging, 28 Carpentry, 35 Procuration, 36 Cooking
  repurposed   : 27, 29, 31, 32, 34 (Skill-Raised effects no item/DB row applied) + free MMO-only slots 77, 78, 80, 81, 85
DB `effects` rows 46-56 point items (item_effects.xml) at those player effects.
New Meister items 3925-3929 (look = former SkinA meshes), recipes 6483-6487; 3653/3654 (Fancy Stonecutter/Breeder)
become the Meister for 3591/3592, their recipes 6397/6398 move to the workshop with the same harder costs."""
import os, re, shutil
S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
BK = r'E:\ClaudeScratch\backups\outfit_tiers_20261003'
MMO_ICONS = r'E:\ClaudeScratch\qbms\out\art_2D\Effects'
os.makedirs(BK, exist_ok=True)

SK = {2: ('Mining', 'Bergbau'), 4: ('Forging', 'Schmieden'), 5: ('Armorsmithing', 'Rüstungsschmieden'),
      7: ('Building Maintain', 'Gebäudewartung'), 8: ('Carpentry', 'Schreinern'), 9: ('Bowcraft', 'Bogenbau'),
      10: ('Warfare engineering', 'Belagerungsgerätebau'), 12: ('Herbalism', 'Kräuterkunde'), 13: ('Brewing', 'Brauen'),
      14: ('Healing', 'Heilen'), 15: ('Alchemy', 'Alchemie'), 17: ('Materials Preparation', 'Baustoffvorbereitung'),
      18: ('Construction', 'Zimmern'), 19: ('Masonry', 'Maurerhandwerk'), 20: ('Architecture', 'Architektur'),
      23: ('Procuration', 'Tierverarbeitung'), 24: ('Cooking', 'Kochen'), 25: ('Tailoring', 'Schneidern'),
      26: ('Warhorse training', 'Schlachtrosstraining')}

# player effect id -> (skills, icon); only the slots that are (re)written
PLAYER_FX = {
    31: ([15, 12], 'Skill_Raised_Alchemy.png'),                 # Geselle Alchemist
    77: ([18, 7], 'Skill_Raised_Construction.png'),              # Geselle Engineer
    78: ([17], 'Skill_Raised_Materials_Processing.png'),         # Geselle Stonecutter
    29: ([8, 9, 10], 'Skill_Raised_Carpentry.png'),              # Meister Carpenter
    27: ([4, 5], 'Skill_Raised_Armorsmithing.png'),              # Meister Blacksmith
    80: ([24, 13], 'Skill_Raised_Cooking.png'),                  # Meister Cook
    32: ([15, 12, 14], 'Skill_Raised_Healing.png'),              # Meister Alchemist
    34: ([18, 7, 19, 20], 'Skill_Raised_Architecture.png'),      # Meister Engineer
    81: ([23, 25, 26], 'Skill_Raised_Procuration.png'),          # Meister Breeder
    85: ([17, 2], 'Skill_Raised_Mining.png'),                    # Meister Stonecutter
}
# DB effects rows (item -> player effect)
DB_FX = [(46, 31), (47, 77), (48, 35), (49, 78), (50, 29), (51, 27), (52, 80), (53, 32), (54, 34), (55, 81), (56, 85)]
FX_NAME = lambda pid: 'Skill Raised: ' + ', '.join(SK[s][0] for s in PLAYER_FX[pid][0])
FX_NAME_DE = lambda pid: 'Fertigkeit erhöht: ' + ', '.join(SK[s][1] for s in PLAYER_FX[pid][0])
DB_NAME = {35: 'Skill Raised: Procuration'}

# item -> DB effect id (all "1 20" like the vanilla outfits)
ITEM_FX = {303: 21, 304: 22, 305: 46, 306: 47, 307: 25, 3591: 49, 3592: 48,
           3925: 51, 3926: 50, 3927: 53, 3928: 54, 3929: 52, 3653: 56, 3654: 55}

# Meister items: id, base, en name, skin key, skin desc msg, de name, de desc
MEISTER = [
    (3925, 303, "Master Blacksmith's Outfit", 'Blacksmith', 4324, 'Schmiedekleidung (Meister)',
     'Die prunkvolle Kleidung eines Schmiedemeisters: schwarzes Hemd, Lederschürze mit karmesinroten Mustern und ein Gürtel, an dem Zangen und Hämmer in strenger Ordnung hängen.'),
    (3926, 304, "Master Carpenter's Outfit", 'Carpenter', 4325, 'Schreinerkleidung (Meister)',
     'Die stattliche Tracht städtischer Schreinermeister, mit praktischen Taschen an Schürze und Jacke, robuster Hose aus gekochtem Leder und Schlaufen für Werkzeuge.'),
    (3927, 305, "Master Alchemist's Outfit", 'Alchemist', 4326, 'Alchemistenkleidung (Meister)',
     'Ein verzierter Mantel aus magentafarbenem Leinen, leicht und weit, mit einem Gürtel voller bauchiger Fläschchen aus Rauchglas.'),
    (3928, 306, "Master Engineer's Outfit", 'Engineer', 4327, 'Ingenieurkleidung (Meister)',
     'Ein karmesinrotes Hemd mit mattgoldenen Mustern und eine Lederjacke, in die Maße und Winkel für Baupläne eingenäht sind.'),
    (3929, 307, "Master Cook's Outfit", 'Cook', 4328, 'Kochkleidung (Meister)',
     'Eine blaue Weste, ein helles Leinenhemd und eine breite weiße Schürze mit Taschen für Salz und Gewürze und Schlaufen für Messer und Kellen.'),
]
GOLD, SILVER = 3049, 3052
# recipe id, result, base outfit, leather id, (linen, wool, leather) counts, leaf
MEISTER_RECIPES = [
    (6483, 3925, 303, 424, (4, 4, 4), GOLD), (6484, 3926, 304, 424, (4, 4, 3), SILVER),
    (6485, 3927, 305, 425, (5, 3, 3), SILVER), (6486, 3928, 306, 424, (4, 4, 3), GOLD),
    (6487, 3929, 307, 424, (5, 3, 3), SILVER),
    (6397, 3653, 3591, 3554, (4, 4, 4), GOLD), (6398, 3654, 3592, 3556, (5, 3, 3), SILVER)]
M_LVL, M_DEP = 90, 60
REC_IMG = {3925: 'art/2D/equipIcons/Outfits/Male_Craft_Blacksmith_SkinA.png', 3926: 'art/2D/equipIcons/Outfits/Male_Craft_Carpenter_SkinA.png',
           3927: 'art/2D/equipIcons/Outfits/Male_Craft_Alchemist_SkinA.png', 3928: 'art/2D/equipIcons/Outfits/Male_Craft_Engineer_SkinA.png',
           3929: 'art/2D/equipIcons/Outfits/Male_Craft_Cook_SkinA.png', 3653: 'mod/HornSkinMod/art/2D/Items/miner_skinA.png',
           3654: 'mod/HornSkinMod/art/2D/Items/breeder_skinA.png'}
EN_NAME = {3925: "Master Blacksmith's Outfit", 3926: "Master Carpenter's Outfit", 3927: "Master Alchemist's Outfit",
           3928: "Master Engineer's Outfit", 3929: "Master Cook's Outfit", 3653: "Fancy Stonecutter's Outfit", 3654: "Fancy Breeder's Outfit"}


def reqs(base, leather, cnt, leaf):  # (material, quality, influence, quantity)
    return [(base, 0, 20, 1), (261, 0, 10, cnt[0]), (266, 0, 10, cnt[1]), (leather, 0, 10, cnt[2]), (leaf, 0, 20, 5), (2836, 0, 20, 40)]


def rd(p): return open(p, 'rb').read().decode('utf-8')
def wr(p, t): open(p, 'wb').write(t.encode('utf-8'))
def bk(p, name):
    if not os.path.exists(os.path.join(BK, name)): shutil.copy2(p, os.path.join(BK, name))


def msgs(t, mid):
    return re.search(r'<string id="%d"[^>]*>([^<]*)</string>' % mid, t).group(1)


srv_msgs = rd(S + r'\data\cm_messages.xml')

# ---------------- cm_effects.xml (server + client) ----------------
def fx_block(t, pid):
    skills, icon = PLAYER_FX[pid]
    flags = re.search(r'<effect id="26" name="[^"]*">\s*(<flags[^>]*/>)', t).group(1)
    ps = ''.join('        <parameter type="SKILL" skill="%d" applytype="INCREASE" />\r\n' % s for s in skills)
    return ('    <effect id="%d" name="%s">\r\n        %s\r\n        <description>2615</description>\r\n%s'
            '        <icon>art/2D/Effects/%s</icon>\r\n    </effect>') % (pid, FX_NAME(pid), flags, ps, icon)


def fix_effects(t):
    for pid in PLAYER_FX:
        m = re.search(r'[ \t]*<effect id="%d" name="[^"]*">.*?</effect>' % pid, t, re.S)
        t = t[:m.start()] + fx_block(t, pid) + t[m.end():]
    return t


for root, tag in ((S, 'server'), (C, 'client')):
    p = root + r'\data\cm_effects.xml'; bk(p, tag + '_cm_effects.xml'); wr(p, fix_effects(rd(p)))
print('cm_effects.xml: %d effects rewritten (server+client)' % len(PLAYER_FX))
for pid, (sk, icon) in PLAYER_FX.items():
    dst = C + r'\art\2D\Effects\\' + icon
    if not os.path.exists(dst):
        shutil.copy2(os.path.join(MMO_ICONS, icon), dst); print('icon copied', icon)

# ---------------- cm_equipTypes.xml (server + client): drop skins of 303-307, add 3925-3929 ----------------
def fix_equip(t):
    for oid in (303, 304, 305, 306, 307):
        m = re.search(r'<object id="%d">.*?</object>' % oid, t, re.S)
        seg = re.sub(r'\s*<skins>.*?</skins>', '', m.group(0), flags=re.S)
        t = t[:m.start()] + seg + t[m.end():]
    add = ''
    for oid, base, en, key, _, _, _ in MEISTER:
        if '<object id="%d">' % oid in t: continue
        orig = re.search(r'<object id="%d">.*?</object>' % base, rd(os.path.join(BK, 'server_cm_equipTypes.xml')), re.S).group(0)
        skin_meshes = re.search(r'<skin id="\d+"[^>]*>.*?(<meshes>.*?</meshes>)', orig, re.S).group(1)
        blk = re.sub(r'\s*<skins>.*?</skins>', '', orig, flags=re.S)
        blk = re.sub(r'<meshes>.*?</meshes>', lambda _: skin_meshes, blk, count=1, flags=re.S)
        blk = blk.replace('<object id="%d">' % base, '<object id="%d">' % oid)
        blk = re.sub(r'<!--name = [^>]*-->', '<!--name = %s (LiFx 2026-10-03: former skin as own item) -->' % en.replace("'", '&apos;'), blk)
        add += blk + '\r\n'
    i = t.rindex('</equipment_types>')
    return t[:i] + add + t[i:]


for root, tag in ((S, 'server'), (C, 'client')):
    p = root + r'\data\cm_equipTypes.xml'; bk(p, tag + '_cm_equipTypes.xml'); wr(p, fix_equip(rd(p)))
print('cm_equipTypes.xml: skins removed from 303-307, Meister 3925-3929 added (server+client)')

# ---------------- item_effects.xml (server) ----------------
p = S + r'\data\item_effects.xml'; bk(p, 'server_item_effects.xml'); t = rd(p)
for iid, eid in ITEM_FX.items():
    m = re.search(r'<item\s+id="%d">.*?</item>' % iid, t, re.S)
    if m:
        seg = re.sub(r'<effect id="\d+">[^<]*</effect>', '<effect id="%d">1 20</effect>' % eid, m.group(0), count=1)
        t = t[:m.start()] + seg + t[m.end():]
    else:
        i = t.rindex('</items>')
        t = t[:i] + '    <item id="%d">\r\n        <effect id="%d">1 20</effect>\r\n    </item>\r\n' % (iid, eid) + t[i:]
wr(p, t); print('item_effects.xml: %d outfits set' % len(ITEM_FX))

# ---------------- dump.sql (art = real seed; sql kept in sync): effects rows + Meister objects ----------------
def esc(s): return s.replace("'", "''")


def obj_row(oid, en, key, desc):
    return ("(%d,225,'%s',0,0,0,0,0,0,0,0,4,1,1200,'',0,0,0,0,0,0,'art/2D/equipIcons/Outfits/Male_Craft_%s_SkinA.png','%s',100000,NULL,1,1)"
            % (oid, esc(en), key, esc(desc)))


DESC = {oid: msgs(srv_msgs, dmsg) for oid, _, _, _, dmsg, _, _ in MEISTER}
FX_ROWS = ["(%d,'%s',NULL,NULL,%d)" % (i, DB_NAME.get(pid) or FX_NAME(pid), pid) for i, pid in DB_FX]
for sub in ('art', 'sql'):
    p = S + '\\' + sub + r'\dump.sql'; bk(p, sub + '_dump.sql'); t = rd(p)
    if "(46,'Skill Raised" not in t:
        old = "(45,'Drink: Shaky Hands',NULL,NULL,79);"
        assert old in t, sub
        t = t.replace(old, "(45,'Drink: Shaky Hands',NULL,NULL,79),\n" + ',\n'.join(FX_ROWS) + ';', 1)
    if '(3925,225,' not in t:
        i = t.index('(3924,213,'); j = t.index('\n', i) + 1
        assert t[i:j].rstrip().endswith('),'), 'row after 3924 must continue the VALUES list'
        t = t[:j] + ''.join(obj_row(oid, en, key, DESC[oid]) + ',\n' for oid, _, en, key, _, _, _ in MEISTER) + t[j:]
    wr(p, t); print('%s\\dump.sql updated' % sub)

# ---------------- OutfitsPack mod.cs: effects rows, Meister objects + recipes ----------------
def rec_lines(rid, res):
    r = [x for x in MEISTER_RECIPES if x[0] == rid][0]
    out = ['        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (%d,\'%s\',\'\',2836,25,%d,%d,%d,1,0,0,\'%s\')");'
           % (rid, esc(EN_NAME[res]), M_LVL, res, M_DEP, REC_IMG[res])]
    for mat, q, inf, n in reqs(r[2], r[3], r[4], r[5]):
        out.append('        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,%d,%d,%d,%d,%d,0)");' % (rid, mat, q, inf, n))
    return out


p = S + r'\mods\LiFx\OutfitsPack\mod.cs'; bk(p, 'OutfitsPack_mod.cs'); t = rd(p)
if '3925' not in t:
    L = ['        //////////////// Geselle/Meister tiers (2026-10-03) ////////////////',
         '        // DB effects 46-56 -> merged Skill-Raised player effects (cm_effects.xml); item mapping in data\\item_effects.xml.']
    L += ['        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES %s");' % r for r in FX_ROWS]
    L.append('        // Meister outfits 3925-3929 = former wardrobe skins of 303-307 as own items (Tailor\'s Workshop only, Tailoring 90).')
    L += ['        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES %s");' % obj_row(oid, en, key, DESC[oid]) for oid, _, en, key, _, _, _ in MEISTER]
    for rid, res, *_ in MEISTER_RECIPES:
        if rid < 6400: continue  # 6397/6398 live in HornSkinPack
        L += rec_lines(rid, res)
    t = t.replace('    function LiFxOutfitsPack::dbChanges() {\r\n', '    function LiFxOutfitsPack::dbChanges() {\r\n' + '\r\n'.join(L) + '\r\n', 1) \
        if '\r\n' in t else t.replace('    function LiFxOutfitsPack::dbChanges() {\n', '    function LiFxOutfitsPack::dbChanges() {\n' + '\n'.join(L) + '\n', 1)
    assert '3925' in t
    t = t.replace('Objects 3591-3592, recipes 6346-6347.', 'Objects 3591-3592, recipes 6346-6347. 2026-10-03: Meister tiers 3925-3929 (recipes 6483-6487) + DB effects 46-56.', 1)
    wr(p, t); print('OutfitsPack mod.cs updated')

# ---------------- HornSkinPack mod.cs: 6397/6398 -> workshop, Meister cost ----------------
p = S + r'\mods\LiFx\HornSkinPack\mod.cs'; bk(p, 'HornSkinPack_mod.cs'); t = rd(p)
nl = '\r\n' if '\r\n' in t else '\n'
for rid, res in ((6397, 3653), (6398, 3654)):
    t = re.sub(r'[ \t]*dbi\.Update\("INSERT IGNORE INTO `recipe_requirement` VALUES \(NULL,%d,[^\n]*\n' % rid, '', t)
    lines = rec_lines(rid, res)
    t = re.sub(r'[ \t]*dbi\.Update\("INSERT IGNORE INTO `recipe` VALUES \(%d,[^\n]*\n' % rid, lambda _: nl.join(lines) + nl, t)
wr(p, t); print('HornSkinPack mod.cs: 6397/6398 moved to the workshop')

# ---------------- client objects_types.xml ----------------
p = C + r'\data\objects_types.xml'; bk(p, 'client_objects_types.xml'); t = rd(p)
m = re.search(r'[ \t]*<row>\s*<ID>3924</ID>.*?</row>\r?\n', t, re.S)
tmpl = re.search(r'[ \t]*<row>\s*<ID>303</ID>.*?</row>\r?\n', t, re.S).group(0)
add = ''
for oid, base, en, key, _, _, _ in MEISTER:
    if '<ID>%d</ID>' % oid in t: continue
    r = tmpl.replace('<ID>303</ID>', '<ID>%d</ID>' % oid)
    r = re.sub(r'<Name>.*?</Name>', lambda _: '<Name>%s</Name>' % en, r)
    r = re.sub(r'<FaceImage>.*?</FaceImage>', lambda _: '<FaceImage>art\\2D\\equipIcons\\Outfits\\Male_Craft_%s_SkinA.png</FaceImage>' % key, r)
    r = re.sub(r'<Description>.*?</Description>|<Description\s*/>', lambda _: '<Description>%s</Description>' % DESC[oid], r)
    add += r
t = t[:m.end()] + add + t[m.end():]
wr(p, t); print('client objects_types.xml: %d rows' % add.count('<row>'))

# ---------------- client recipe.xml + recipe_requirement.xml ----------------
p = C + r'\data\recipe.xml'; bk(p, 'client_recipe.xml'); t = rd(p)
tmpl = re.search(r'[ \t]*<row>\s*<ID>5104</ID>.*?</row>\r?\n', t, re.S).group(0)
add = ''
for rid, res, *_ in MEISTER_RECIPES:
    r = tmpl.replace('<ID>5104</ID>', '<ID>%d</ID>' % rid)
    r = re.sub(r'<Name>.*?</Name>', lambda _: '<Name>%s</Name>' % EN_NAME[res].replace("'", '&apos;'), r)
    r = re.sub(r'<SkillLvl>\d+</SkillLvl>', '<SkillLvl>%d</SkillLvl>' % M_LVL, r)
    r = re.sub(r'<ResultObjectTypeID>\d+</ResultObjectTypeID>', '<ResultObjectTypeID>%d</ResultObjectTypeID>' % res, r)
    r = re.sub(r'<SkillDepends>\d+</SkillDepends>', '<SkillDepends>%d</SkillDepends>' % M_DEP, r)
    r = re.sub(r'<ImagePath>.*?</ImagePath>', lambda _: '<ImagePath>%s</ImagePath>' % REC_IMG[res].replace('/', '\\'), r)
    old = re.search(r'[ \t]*<row>\s*<ID>%d</ID>.*?</row>\r?\n' % rid, t, re.S)
    if old: t = t[:old.start()] + r + t[old.end():]
    else: add += r
m = re.search(r'[ \t]*<row>\s*<ID>5108</ID>.*?</row>\r?\n', t, re.S)
t = t[:m.end()] + add + t[m.end():]
wr(p, t); print('client recipe.xml updated')

p = C + r'\data\recipe_requirement.xml'; bk(p, 'client_recipe_requirement.xml'); t = rd(p)
rids = [r[0] for r in MEISTER_RECIPES]
t = re.sub(r'[ \t]*<row>\s*<ID>\d+</ID>\s*<RecipeID>(%s)</RecipeID>.*?</row>\r?\n' % '|'.join(map(str, rids)), '', t, flags=re.S)
nid = max(int(x) for x in re.findall(r'<ID>(\d+)</ID>', t)) + 1
rows = ''
for rid, res, base, leather, cnt, leaf in MEISTER_RECIPES:
    for mat, q, inf, n in reqs(base, leather, cnt, leaf):
        rows += ('\t<row><ID>%d</ID><RecipeID>%d</RecipeID><MaterialObjectTypeID>%d</MaterialObjectTypeID><Quality>%d</Quality>'
                 '<Influence>%d</Influence><Quantity>%d</Quantity><IsRegionItemRequired>0</IsRegionItemRequired></row>\r\n') % (nid, rid, mat, q, inf, n)
        nid += 1
last = list(re.finditer(r'[ \t]*<row><ID>\d+</ID><RecipeID>5108</RecipeID>.*?</row>\r?\n', t))[-1]
t = t[:last.end()] + rows + t[last.end():]
wr(p, t); print('client recipe_requirement.xml: %d rows' % rows.count('<row>'))

# ---------------- German loc ----------------
LD = C + r'\data\loc\de\data'
def set_str(fname, items):
    p = LD + '\\' + fname; bk(p, 'de_' + fname); raw = open(p, 'rb').read(); bom = raw.startswith(b'\xef\xbb\xbf')
    t = raw.decode('utf-8-sig'); nl = '\r\n' if '\r\n' in t else '\n'
    for i, v in items:
        if re.search(r'<string id="%d">' % i, t):
            t = re.sub(r'<string id="%d">[^<]*</string>' % i, lambda _: '<string id="%d">%s</string>' % (i, v), t)
        else:
            last = t.rfind('</string>') + len('</string>')
            t = t[:last] + nl + '<string id="%d">%s</string>' % (i, v) + t[last:]
    open(p, 'wb').write((b'\xef\xbb\xbf' if bom else b'') + t.encode('utf-8'))


GES = {303: 'Schmiedekleidung', 304: 'Schreinerkleidung', 305: 'Alchemistenkleidung', 306: 'Ingenieurkleidung',
       307: 'Kochkleidung', 3591: 'Steinmetzkleidung', 3592: 'Züchterkleidung'}
names = [(i, n + ' (Geselle)') for i, n in GES.items()] + [(oid, de) for oid, _, _, _, _, de, _ in MEISTER]
names += [(3653, 'Steinmetzkleidung (Meister)'), (3654, 'Züchterkleidung (Meister)')]
set_str('objects_types_Name.xml', names)
set_str('objects_types_Description.xml', [(oid, dd) for oid, _, _, _, _, _, dd in MEISTER])
rec_de = [(222, 303), (5104, 303), (223, 304), (5105, 304), (224, 305), (5106, 305), (225, 306), (5107, 306),
          (226, 307), (5108, 307), (6346, 3591), (6347, 3592)]
rn = [(rid, GES[o] + ' (Geselle)') for rid, o in rec_de]
rn += [(rid, dict(names)[res]) for rid, res, *_ in MEISTER_RECIPES]
set_str('recipe_Name.xml', rn)
set_str('cm_effects_name.xml', [(pid, FX_NAME_DE(pid)) for pid in PLAYER_FX])
print('German names set; backups in', BK)
