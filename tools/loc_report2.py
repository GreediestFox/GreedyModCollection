import os
import csv, re, collections, subprocess
exec(open(os.path.join(os.path.dirname(os.path.abspath(__file__)), 'loc_report.py'), encoding='utf-8').read().split("w = lambda fn")[0])

# add Slavic (baked in art\dump.sql, no INSERTs in mod.cs)
for i in range(2900, 2924):
    d = db_obj.get(i)
    if d:
        de = de_on.get(i)
        out_o.append([i, d[1], 'SlavicFortifications', kind(dict(id=i, mov='0', unmov='0', dev='0')), root_cat(int(d[2])), 'yes', '', 'OK' if de and de != d[1] else ('MISSING' if not de else 'same-as-English'), de or '', 'yes' if i in de_od else 'no'])
for i in range(5500, 5524):
    d = db_rec.get(i)
    if d:
        de = de_rn.get(i)
        res = int(d[4])
        out_r.append([i, d[1], 'SlavicFortifications', res, (db_obj.get(res) or ['', '?'])[1], int(d[2]), skills.get(int(d[2]), ''), d[3], 'yes', '', 'OK' if de and de != d[1] else ('MISSING' if not de else 'same-as-English'), de or ''])
out_o.sort(key=lambda r: r[0])
out_r.sort(key=lambda r: r[0])

OUT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'docs', 'reports')
wr = lambda fn, hdr, rows: (lambda f: (lambda cw: (cw.writerow(hdr), cw.writerows(rows)))(csv.writer(f, delimiter=';')))(open(fn, 'w', encoding='utf-8-sig', newline=''))
wr(OUT + r'\LiFx_ModObjects_Translation.csv', ['ObjectID', 'Name (EN)', 'Pack', 'Kind', 'Category', 'In DB', 'Note', 'German name status', 'German name now', 'German description present'], out_o)
wr(OUT + r'\LiFx_ModRecipes_Translation.csv', ['RecipeID', 'Name (EN)', 'Pack', 'ResultID', 'Result name', 'SkillID', 'Skill', 'Level', 'In DB', 'Note', 'German name status', 'German name now'], out_r)


def ranges(ids):
    ids = sorted(ids)
    out, s, p = [], None, None
    for x in ids:
        if s is None:
            s = p = x
        elif x == p + 1:
            p = x
        else:
            out.append((s, p))
            s = p = x
    if s is not None:
        out.append((s, p))
    return ', '.join(str(a) if a == b else '%d-%d' % (a, b) for a, b in out)


# active = in DB (or MmoItemsPack, which goes live at next restart) and not commented
def active_o(r):
    return (r[5] == 'yes' or r[2] == 'MmoItemsPack') and r[6] != 'commented'


def active_r(r):
    return r[8] == 'yes' and r[9] != 'commented'


ao = [r for r in out_o if active_o(r)]
ar = [r for r in out_r if active_r(r)]
packs = sorted(set(r[2] for r in ao) | set(r[2] for r in ar))
ED = {
    'FurniturePack': 'EDITED (skill -> Carpentry, levels)', 'ChestsPack': 'EDITED (skill/levels)', 'DisplaysPack': 'EDITED (skill/levels, decorations moved to Arts)',
    'SignsPillarsPack': 'EDITED (skill/levels, pillars/lights moved to Arts)', 'JorvikModPack': 'EDITED (levels/skills changed)',
    'EkrBuildingsPack': 'mostly ORIGINAL ports (removed: 12 >20k-poly buildings, Tribe decos, Engelsstatue, Primitive Bed)',
    'AltarsIdolsPack': 'ORIGINAL MMO recipes', 'StatuesPack': 'ORIGINAL MMO recipes', 'BuildingsPack': 'ORIGINAL', 'CloakFlagPack': 'ORIGINAL MMO recipes',
    'HornSkinPack': 'ORIGINAL MMO recipes', 'KingOutfitsPack': 'ORIGINAL MMO recipes', 'OutfitsPack': 'ORIGINAL MMO recipes', 'SiegePack': 'ORIGINAL MMO recipes',
    'SlavicFortifications': 'cloned from vanilla wall recipes (ingredients same as vanilla)', 'FoodDrinksPack': 'ORIGINAL MMO recipes (no food effects for new MMO dishes)',
    'FreshWaterPack': 'ORIGINAL', 'MmoItemsPack': 'items only - no recipes', 'KnoolMod-Pack': 'weapon recipes commented out, items still spawnable',
    'MaterialsChainPack': 'metal bar/ingot/lump recipes commented out (28); remaining recipes ORIGINAL',
    'TierGearPack': 'DORMANT (all commented)', 'TribeCampDecorations': 'DORMANT (all commented)',
    'OreWasher': 'own design', 'LargeHerbalGarden': 'own design', 'TailorsWorkshop': 'own design (65 recipes cloned from vanilla tailoring)', 'JewelersWorkshop': 'own design',
    'StonemasonsWorkplace': 'own design', 'BigDryingFrame': 'own design', 'BigTanningTub': 'own design', 'OilPressRabbitCage': 'own design', 'StorageBuildings': 'own design',
    'Woodshed': 'own design',
}
L = []
L.append('# LiFx mod objects and recipes: German translation and recipe overview\n')
L.append('Generated 2026-09-30. Full per-ID lists (only active, in-DB entries matter; dormant ones are marked "commented"): `LiFx_ModObjects_Translation.csv` and `LiFx_ModRecipes_Translation.csv` (semicolon separated, open in Excel).\n')
L.append('## 1. Where to translate (German client)\n')
L.append('Folder: `F:\\SteamLibrary\\steamapps\\common\\Life is Feudal Your Own\\data\\loc\\de\\data\\` (UTF-8 files, line format `<string id="ID">Text</string>`, keep `</strings>` at the end).\n')
L.append('| What | File | string id = |\n|---|---|---|')
L.append('| Item / building / furniture / decoration NAME | `objects_types_Name.xml` | object ID |')
L.append('| Item / building description | `objects_types_Description.xml` | object ID |')
L.append('| Build-window / crafting recipe NAME | `recipe_Name.xml` | recipe ID |')
L.append('| Recipe description | `recipe_Description.xml` | recipe ID |')
L.append('| Ability names (e.g. Hinlegen, Gather) | `skill_types_ability_name.xml` | ability ID |')
L.append('| Skill names | `skill_types.xml` | skill ID |')
L.append('\nMissing entries fall back to the English name from `data\\objects_types.xml` / `data\\recipe.xml`. The client reads loc files at startup only, so relaunch after editing. There is no server-side translation; the server sends IDs only.\n')
L.append('## 2. Translation status per pack (active entries only)\n')
L.append('Object = item/building/furniture/decoration. "German missing" counts entries that have no German name yet.\n')
L.append('| Pack | Object IDs | Objects | German missing | Recipe IDs | Recipes | German missing | Recipe edit status |\n|---|---|---|---|---|---|---|---|')
for p in packs:
    o = [r for r in ao if r[2] == p]
    r_ = [r for r in ar if r[2] == p]
    om = sum(1 for r in o if r[7] != 'OK')
    rm = sum(1 for r in r_ if r[10] != 'OK')
    L.append('| %s | %s | %d | %d | %s | %d | %d | %s |' % (p, ranges([r[0] for r in o]) or '-', len(o), om, ranges([r[0] for r in r_]) or '-', len(r_), rm, ED.get(p, '')))
tot_o, tot_r = len(ao), len(ar)
L.append('\nTotals: %d active objects (%d without German name), %d active recipes (%d without German name).\n' % (tot_o, sum(1 for r in ao if r[7] != 'OK'), tot_r, sum(1 for r in ar if r[10] != 'OK')))
L.append('## 3. Objects still needing a German NAME (file `objects_types_Name.xml`)\n')
for p in packs:
    o = [r for r in ao if r[2] == p and r[7] != 'OK']
    if o:
        L.append('- **%s** (%d): %s' % (p, len(o), ranges([r[0] for r in o])))
L.append('\n## 4. Recipes still needing a German NAME (file `recipe_Name.xml`)\n')
for p in packs:
    o = [r for r in ar if r[2] == p and r[10] != 'OK']
    if o:
        L.append('- **%s** (%d): %s' % (p, len(o), ranges([r[0] for r in o])))
L.append('\n## 5. Kinds (what is a building / furniture / decoration)\n')
L.append('`Kind` column in the objects CSV: Item, Building (unmovable), Movable (furniture, decorations, siege), Device/workshop. Decoration-type objects are the Movable ones in the packs DisplaysPack (mannequins, shields, banners, lights), SignsPillarsPack, AltarsIdolsPack, StatuesPack, CloakFlagPack, FurniturePack and JorvikModPack.\n')
kc = collections.Counter((r[2], r[3]) for r in ao)
L.append('| Pack | Kind | Count |\n|---|---|---|')
for (p, k), n in sorted(kc.items()):
    L.append('| %s | %s | %d |' % (p, k, n))
L.append('\n## 6. Where the recipes live (to edit skill, level or ingredients)\n')
L.append('- Server mod packs: `G:\\Progam (Launcher)\\steamapps\\common\\Life is Feudal Your Own Dedicated Server\\mods\\LiFx\\<Pack>\\mod.cs` (`INSERT IGNORE INTO recipe` / `recipe_requirement` / `objects_types` lines).')
L.append('- Baked rows that WIN over mod.cs (edit here, never in `sql\\dump.sql`): `...\\Dedicated Server\\art\\dump.sql`. This holds all vanilla recipes plus 65 mod types and the Slavic pack (objects 2900-2923, recipes 5500-5523, requirements 91000-91113).')
L.append('- Client mirror (must match the server, needs client relaunch): `data\\objects_types.xml`, `data\\recipe.xml`, `data\\recipe_requirement.xml`, `data\\cm_objects.xml` (3D model per building/furniture) in the client folder.')
L.append('\n## 7. How "unedited" was determined\n')
L.append('I cannot diff against the MMO/Arden/Jorvik originals, so "ORIGINAL" means: ported recipe whose ingredients and quantities I never changed. "EDITED" means only skill/level (and the Arts/Carpentry category move) was changed by us, ingredients are still the original. Vanilla recipes touched: 1026-1030 and 1036-1040 (moved to Arts), 2, 5, 8, 1032, 1033 (level 90).')
open(OUT + r'\LiFx_Translation_and_Recipe_Overview.md', 'w', encoding='utf-8').write('\n'.join(L))
print('\n'.join(L))
