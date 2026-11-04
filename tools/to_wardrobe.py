"""
Turns the skin packs into REAL skins (wardrobe / skin menu) instead of standalone items.
1. removes the standalone items (server: restores the pre-skin backups; client F: rebuilds from the clean 10-31 backup)
2. adds the armor skins (<skin> blocks from the MMO data) to the base armor types' <skins> in client + server cm_equipTypes.xml
   (tool and weapon skins are already defined by the YO equip data)
3. makes sure every skin's name/description message and icon exist
4. writes all numeric skin ids (-> wardrobe table of the client exe) to skin_ids.json
"""
import re, os, json, shutil, sys

SRV = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
SRC = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
CLI = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own'
BKC = r'E:\ClaudeScratch\backups\client_full_20261031\Life is Feudal Your Own'
B = r'E:\ClaudeScratch\backups'
MMOD = r'F:\Lif Datein\MMO Zip\staticData_extracted'
MMOEN = r'E:\ClaudeScratch\mmo_effects\staticData_dlpack\cm_messages.xml'
MMODE = r'E:\ClaudeScratch\mmo_effects\loc_cmpack\de\data\cm_messages.xml'
ICONROOT = r'E:\ClaudeScratch\qbms\out'
SK = r'E:\ClaudeScratch\project_archive\Session_2026-11-01\SkinPort'
APPLY = '--apply' in sys.argv


def rd(p):
    return open(p, encoding='utf-8', errors='replace', newline='').read()


def wr(p, s):
    os.makedirs(os.path.dirname(p), exist_ok=True)
    open(p, 'w', encoding='utf-8', newline='').write(s)


tools = json.load(open(B + r'\toolskins_20261101\tool_skins.json'))
armor = json.load(open(B + r'\armorskins_20261101\armor_skins.json'))
weap = json.load(open(B + r'\weaponskins_20261101\weapon_skins.json'))
skin_objs = {s['obj'] for g in (tools, armor, weap) for s in g}
print(len(skin_objs), 'standalone items to remove')

# ---------------------------------------------------------------- 1. restore originals
srv_restore = {
    r'\art\dump.sql': B + r'\toolskins_20261101\srv_dump.sql',
    r'\data\cm_equipTypes.xml': B + r'\toolskins_20261101\srv_cm_equipTypes.xml',
    r'\art\datablocks\weapons\LiFWeapons.cs': B + r'\toolskins_20261101\srv_LiFWeapons.cs',
    r'\art\datablocks\weapons\LifRanged.cs': B + r'\weaponskins_20261101\srv_LifRanged.cs',
    r'\data\item_effects.xml': B + r'\armorskins_20261101\srv_item_effects.xml',
    r'\data\skill_types.xml': B + r'\toolskins_20261101\srv_skill_types.xml',
}
for rel, bk in srv_restore.items():
    assert os.path.exists(bk), bk
    print('server restore', rel)
    if APPLY:
        shutil.copy2(bk, SRV + rel)

# client: clean files from the 10-31 backup
cli_files = [r'\data\objects_types.xml', r'\data\recipe.xml', r'\data\recipe_requirement.xml', r'\data\cm_equipTypes.xml',
             r'\data\loc\de\data\objects_types_Name.xml', r'\data\loc\de\data\recipe_Name.xml',
             r'\art\datablocks\weapons\LiFWeapons.cs', r'\art\datablocks\weapons\LifRanged.cs']
for rel in cli_files:
    assert os.path.exists(BKC + rel), rel
    if APPLY:
        shutil.copy2(BKC + rel, SRC + rel)
for d in ('ToolSkinsMod', 'ArmorSkinsMod', 'WeaponSkinsMod'):
    p = SRC + r'\mod\%s' % d
    if APPLY and os.path.exists(p):
        shutil.rmtree(p)


def strip_skill(path):
    t = rd(path)

    def fix(m):
        ids = [i for i in m.group(2).split() if not (i.isdigit() and int(i) in skin_objs)]
        return m.group(1) + ' '.join(ids) + m.group(3)
    return re.sub(r'(<ent_req type="object_type_id">)([^<]*)(</ent_req>)', fix, t)


if APPLY:
    wr(SRC + r'\data\skill_types.xml', strip_skill(SRC + r'\data\skill_types.xml'))

# ---------------------------------------------------------------- 2. skins into the equip types
mmo_eq = rd(MMOD + r'\cm_equipTypes.xml')


def skin_block(tid, sid, nid):
    b = re.search(r'<object id="%d">(.*?)</object>' % tid, mmo_eq, re.S).group(1)
    m = re.search(r'<skin id="%s" name="%d">.*?</skin>' % (re.escape(sid), nid), b, re.S)
    return m.group(0)


add_by_base = {}
for s in armor:
    add_by_base.setdefault(s['base'], []).append(skin_block(s['base'], s['sid'], s['nid']))


def add_skins(eq):
    n_added = 0
    for tid, blocks in add_by_base.items():
        m = re.search(r'<object id="%d">(.*?)</object>' % tid, eq, re.S)
        body = m.group(1)
        have = set(re.findall(r'<skin id="([^"]*)"', body))
        new = [b for b in blocks if re.match(r'<skin id="([^"]*)"', b).group(1) not in have]
        if not new:
            continue
        n_added += len(new)
        text = '\n'.join(new) + '\n'
        if '</skins>' in body:
            body2 = body.replace('</skins>', text + '</skins>', 1)
        else:
            blk = '<skins>\n' + text + '</skins>\n'
            if '<armor>' in body:
                body2 = body.replace('<armor>', blk + '<armor>', 1)
            else:
                body2 = body + blk
        eq = eq[:m.start(1)] + body2 + eq[m.end(1):]
    return eq, n_added


cli_eq = rd(BKC + r'\data\cm_equipTypes.xml')
cli_eq, n1 = add_skins(cli_eq)
srv_eq = rd(B + r'\toolskins_20261101\srv_cm_equipTypes.xml')
srv_eq, n2 = add_skins(srv_eq)
print('armor skins added to client/server equip types:', n1, n2)

# ---------------------------------------------------------------- 3. messages + icons for every skin
ids = set(); msgs = set(); icons = set()
for m in re.finditer(r'<skin id="([^"]*)" name="(\d+)">\s*<info description="(\d+)" icon="([^"]*)"', cli_eq):
    sid, nid, did, icon = m.groups()
    if sid.isdigit():
        ids.add(int(sid))
    msgs.add(int(nid)); msgs.add(int(did)); icons.add(icon)
print(len(ids), 'numeric skin ids,', len(msgs), 'messages,', len(icons), 'icons')


def messages(path):
    return {int(m.group(1)): m.group(2) for m in re.finditer(r'<string id="(\d+)"[^>]*>([^<]*)</string>', rd(path))}


have_en = messages(BKC + r'\data\cm_messages.xml') if os.path.exists(BKC + r'\data\cm_messages.xml') else messages(SRC + r'\data\cm_messages.xml')
src_en_path = SRC + r'\data\cm_messages.xml'
have_en = messages(src_en_path)
mmo_en = messages(MMOEN); mmo_de = messages(MMODE)
missing = sorted(m for m in msgs if m not in have_en and m in mmo_en)
print('messages missing in YO:', len(missing), 'of which unknown to MMO too:', len([m for m in msgs if m not in have_en and m not in mmo_en]))

missing_icons = []
for ic in icons:
    rel = ic.replace('/', '\\')
    if not (os.path.exists(SRC + '\\' + rel) or os.path.exists(CLI + '\\' + rel)):
        missing_icons.append(ic)
print('icons missing in the client:', len(missing_icons))
fixed = 0
for ic in missing_icons:
    base = os.path.basename(ic)
    src = None
    for dp, dn, fn in os.walk(ICONROOT):
        if base in fn:
            src = os.path.join(dp, base); break
    if src:
        fixed += 1
        if APPLY:
            dst = SRC + '\\' + ic.replace('/', '\\')
            os.makedirs(os.path.dirname(dst), exist_ok=True)
            shutil.copy2(src, dst)
print('icons copied from the MMO extraction:', fixed, 'still missing:', len(missing_icons) - fixed)

if not APPLY:
    sys.exit(0)

wr(SRC + r'\data\cm_equipTypes.xml', cli_eq)
wr(SRV + r'\data\cm_equipTypes.xml', srv_eq)
json.dump(sorted(ids), open(SK + r'\skin_ids.json', 'w'))


def add_messages(path, texts):
    t = rd(path)
    add = ''.join('<string id="%d">%s</string>\n' % (i, texts[i]) for i in missing if i in texts)
    if '</strings>' in t:
        t = t.replace('</strings>', add + '</strings>', 1)
    wr(path, t)


add_messages(SRC + r'\data\cm_messages.xml', mmo_en)
if os.path.exists(SRV + r'\data\cm_messages.xml'):
    add_messages(SRV + r'\data\cm_messages.xml', mmo_en)
de_path = SRC + r'\data\loc\de\data\cm_messages.xml'
de_base = rd(CLI + r'\data\loc\de\data\cm_messages.xml')
de_have = messages(CLI + r'\data\loc\de\data\cm_messages.xml')
miss_de = [i for i in sorted(msgs) if i not in de_have and i in mmo_de]
wr(de_path, de_base.replace('</strings>', ''.join('<string id="%d">%s</string>\n' % (i, mmo_de[i]) for i in miss_de) + '</strings>', 1))
print('done; skin ids:', len(ids), '; de messages added:', len(miss_de))
