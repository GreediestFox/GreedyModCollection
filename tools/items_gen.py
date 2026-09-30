import os, re, shutil, sys

S = r'G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server'
C = r'F:\SteamLibrary\steamapps\common\Life is Feudal Your Own'
BK = r'E:\ClaudeScratch\backups\mmo_items_20260930'
APPLY = len(sys.argv) > 1 and sys.argv[1] == 'apply'

# (icon base, display name, parent, stack, unit weight, length, description)
SEED, VEG, MEAT, DISH, GEM, HONEY, BOARD, KIT, ANI, MISC, TOOLS, FOOD = 251, 232, 335, 228, 480, 232, 235, 1132, 210, 7, 224, 228
items = [
    ('Amberwood_board', 'Amberwood Board', BOARD, 10000, 20000, 4, 'A board of amberwood.'),
    ('Amberwood_building_block', 'Amberwood Building Block', BOARD, 10000, 20000, 4, 'A building block of amberwood.'),
    ('Whitewood_board', 'Whitewood Board', BOARD, 10000, 20000, 4, 'A board of whitewood.'),
    ('Whitewood_building_block', 'Whitewood Building Block', BOARD, 10000, 20000, 4, 'A building block of whitewood.'),
    ('Big_Potato', 'Big Potato', VEG, 10000, 500, 2, 'A large potato.'),
    ('Fat_Wheat', 'Fat Wheat', 250, 10000, 120, 2, 'Plump wheat grains.'),
    ('Green_Onion', 'Green Onion', VEG, 10000, 100, 2, 'A crop that can be grown by Farmers.'),
    ('Green_Peas', 'Green Peas', VEG, 10000, 80, 2, 'A crop that can be grown by Farmers.'),
    ('White_Cabbage', 'White Cabbage', VEG, 10000, 600, 2, 'A crop that can be grown by Farmers.'),
    ('Sugar_Carrot', 'Sugar Carrot', VEG, 10000, 230, 2, 'A sweet carrot.'),
    ('onion_big', 'Big Onion', VEG, 10000, 150, 2, 'A large onion.'),
    ('onion_l', 'Large Onion', VEG, 10000, 130, 2, 'An onion.'),
    ('Cabbage_Seeds', 'Cabbage Seeds', SEED, 10000, 100, 2, 'Seeds for sowing.'),
    ('Carrot_Seeds', 'Carrot Seeds', SEED, 10000, 100, 2, 'Seeds for sowing.'),
    ('Onion_Seeds', 'Onion Seeds', SEED, 10000, 100, 2, 'Seeds for sowing.'),
    ('Rich_Flax_Seeds', 'Rich Flax Seeds', SEED, 10000, 100, 2, 'Seeds for sowing.'),
    ('Sugar_Carrot_Seeds', 'Sugar Carrot Seeds', SEED, 10000, 100, 2, 'Seeds for sowing.'),
    ('White_Cabbage_Seeds', 'White Cabbage Seeds', SEED, 10000, 100, 2, 'Seeds for sowing.'),
    ('Wine_Sprout', 'Wine Sprout', 643, 10000, 100, 2, 'A cutting of a wine grapevine.'),
    ('Wild_Wine_Sprout', 'Wild Wine Sprout', 643, 10000, 100, 2, 'A cutting of a wild grapevine.'),
    ('Fatty_pork', 'Fatty Pork', MEAT, 10000, 1000, 2, 'Fatty pork meat.'),
    ('Marbled_beef', 'Marbled Beef', MEAT, 10000, 1000, 2, 'Marbled beef.'),
    ('Lamb', 'Lamb', MEAT, 10000, 1000, 2, 'Lamb meat.'),
    ('Rough_thik_leather', 'Rough Thick Leather', 373, 10000, 3000, 3, 'Rough thick leather.'),
    ('Soft_thik_leather', 'Soft Thick Leather', 373, 10000, 3000, 3, 'Soft thick leather.'),
    ('Rocksalt_trap', 'Rock Salt Trap', 205, 10000, 1000, 2, 'A trap baited with rock salt.'),
    ('dark_honey', 'Dark Honey', HONEY, 10000, 300, 1, 'Dark honey.'),
    ('white_honey', 'White Honey', HONEY, 10000, 300, 1, 'White honey.'),
    ('yellow_honey', 'Yellow Honey', HONEY, 10000, 300, 1, 'Yellow honey.'),
    ('raw_gem', 'Raw Gem', GEM, 10000, 0, 2, 'An uncut gem.'),
    ('rough_gem', 'Rough Gem', GEM, 10000, 0, 2, 'A roughly cut gem.'),
    ('cracked_gem', 'Cracked Gem', GEM, 10000, 0, 2, 'A cracked gem.'),
    ('aquamarine', 'Aquamarine', GEM, 10000, 0, 2, 'A gem.'),
    ('carnelian', 'Carnelian', GEM, 10000, 0, 2, 'A gem.'),
    ('jasper', 'Jasper', GEM, 10000, 0, 2, 'A gem.'),
    ('onyx', 'Onyx', GEM, 10000, 0, 2, 'A gem.'),
    ('topaz', 'Topaz', GEM, 10000, 0, 2, 'A gem.'),
    ('case_hardened_toughened_metal_band', 'Case-hardened Toughened Metal Band', 199, 10000, 1000, 2, 'A metal band.'),
    ('amulet_bone', 'Bone Amulet', MISC, 1, 100, 2, 'An amulet.'),
    ('amulet_copper', 'Copper Amulet', MISC, 1, 100, 2, 'An amulet.'),
    ('amulet_oobs', 'Amulet of the Blind Spirit', MISC, 1, 100, 2, 'An amulet.'),
    ('amulet_osb', 'Amulet of the Spirit Bear', MISC, 1, 100, 2, 'An amulet.'),
    ('amulet_osc', 'Amulet of the Spirit Crow', MISC, 1, 100, 2, 'An amulet.'),
    ('amulet_otl', 'Amulet of the Lynx', MISC, 1, 100, 2, 'An amulet.'),
    ('amulet_otwh', 'Amulet of the White Hunt', MISC, 1, 100, 2, 'An amulet.'),
    ('pionners_amulet', "Pioneer's Amulet", MISC, 1, 100, 2, 'An amulet.'),
    ('raven_amulet', 'Raven Amulet', MISC, 1, 100, 2, 'An amulet.'),
    ('ring_bone', 'Bone Ring', MISC, 1, 100, 2, 'A ring.'),
    ('ring_copper', 'Copper Ring', MISC, 1, 100, 2, 'A ring.'),
    ('ring_othis', 'Ring of the Hissing Snake', MISC, 1, 100, 2, 'A ring.'),
    ('ring_otrh', 'Ring of the Red Hunt', MISC, 1, 100, 2, 'A ring.'),
    ('ring_otsl', 'Ring of the Silver Lynx', MISC, 1, 100, 2, 'A ring.'),
    ('ring_otsw', 'Ring of the Swift Wolf', MISC, 1, 100, 2, 'A ring.'),
    ('ring_otwh', 'Ring of the White Hunt', MISC, 1, 100, 2, 'A ring.'),
    ('adjaruli', 'Adjaruli Khachapuri', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('ashlayamfu', 'Ashlyamfu', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('b_rolls_w_stuffing', 'Stuffed Cabbage Rolls', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('bigos', 'Bigos', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('blood_sausage', 'Blood Sausage', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('carbonara', 'Carbonara', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('cheesecake', 'Cheesecake', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('chicken_broth', 'Chicken Broth', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('chicken_w_potatoes', 'Chicken with Potatoes', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('cream_cake', 'Cream Cake', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('dumplings', 'Dumplings', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('hunters_sausage', "Hunter's Sausage", DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('noodles', 'Noodles', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('olivier_salad', 'Olivier Salad', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('pork_goulash', 'Pork Goulash', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('potato_pancake', 'Potato Pancake', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('r_cabbage_soup', 'Red Cabbage Soup', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('s_cabbage_rolls', 'Sour Cabbage Rolls', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('s_with_potatoes', 'Sausages with Potatoes', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('sour_rye_soup', 'Sour Rye Soup', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('w_mushroom_soup', 'Wild Mushroom Soup', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('basic_work_ration', 'Basic Work Ration', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('medium_work_ration', 'Medium Work Ration', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('large_work_ration', 'Large Work Ration', DISH, 10000, 700, 2, 'Food (4 ingredients)'),
    ('blacksmiths_prec_tools', "Blacksmith's Precision Tools", TOOLS, 1, 3000, 2, 'A set of precision tools.'),
    ('carpenters_prec_measuring_set', "Carpenter's Precision Measuring Set", TOOLS, 1, 3000, 2, 'A set of precision tools.'),
    ('cooks_prof_kitchenware', "Cook's Professional Kitchenware", TOOLS, 1, 3000, 2, 'A set of professional tools.'),
    ('engineers_prec_blueprint_tools', "Engineer's Precision Blueprint Tools", TOOLS, 1, 3000, 2, 'A set of precision tools.'),
    ('herbalists_distillation_set', "Herbalist's Distillation Set", TOOLS, 1, 3000, 2, 'A set of tools.'),
    ('kitchen_seasoning_set', 'Kitchen Seasoning Set', TOOLS, 1, 1500, 2, 'A set of seasonings.'),
    ('primitive_sewing_tools', 'Primitive Sewing Tools', TOOLS, 1, 1500, 2, 'A set of sewing tools.'),
    ('stonecut_prec_chiseling_set', 'Stonecutter Precision Chiseling Set', TOOLS, 1, 3000, 2, 'A set of precision tools.'),
    ('armor_fitting_stand_kit', 'Armor Fitting Stand Kit', KIT, 10000, 5000, 3, 'A kit for building an armor fitting stand.'),
    ('breeders_animal_husbandry_kit', "Breeder's Animal Husbandry Kit", KIT, 10000, 5000, 3, 'A kit.'),
    ('gem_engrav_polishing_kit', 'Gem Engraving and Polishing Kit', KIT, 10000, 5000, 3, 'A kit.'),
    ('herb_drying_preserv_rack_kit', 'Herb Drying and Preserving Rack Kit', KIT, 10000, 5000, 3, 'A kit.'),
    ('master_smithing_tool_rack_kit', 'Master Smithing Tool Rack Kit', KIT, 10000, 5000, 3, 'A kit.'),
    ('pattern_cut_table_prec_kit', 'Pattern Cutting Table Precision Kit', KIT, 10000, 5000, 3, 'A kit.'),
    ('prec_woodworking_frame_kit', 'Precision Woodworking Frame Kit', KIT, 10000, 5000, 3, 'A kit.'),
    ('siege_mech_assembly_rig_kit', 'Siege Mechanism Assembly Rig Kit', KIT, 10000, 5000, 3, 'A kit.'),
    ('animal_skull', 'Animal Skull', ANI, 10000, 1000, 2, 'A trophy.'),
    ('inhuman_skull', 'Inhuman Skull', ANI, 10000, 1000, 2, 'A trophy.'),
    ('witch_skull', 'Witch Skull', ANI, 10000, 1000, 2, 'A trophy.'),
    ('shrunken_head', 'Shrunken Head', ANI, 10000, 500, 2, 'A trophy.'),
    ('dead_fetus', 'Dead Fetus', ANI, 10000, 500, 2, 'A grisly ingredient.'),
    ('clawed_paw', 'Clawed Paw', ANI, 10000, 500, 2, 'A trophy.'),
    ('handlace', 'Hand Lace', ANI, 10000, 100, 2, 'A trinket.'),
    ('witch_braid', 'Witch Braid', ANI, 10000, 100, 2, 'A trinket.'),
    ('savage_trophy', 'Savage Trophy', ANI, 10000, 1000, 2, 'A trophy.'),
    ('chieftains_helm', "Chieftain's Helm", MISC, 1, 2000, 3, 'A trophy.'),
    ('chieftains_knee_pad', "Chieftain's Knee Pad", MISC, 1, 500, 2, 'A trophy.'),
    ('impious_chieftains_symbol', "Impious Chieftain's Symbol", MISC, 1, 200, 2, 'A symbol.'),
    ('hunter_mask', 'Hunter Mask', MISC, 1, 500, 2, 'A mask.'),
    ('skull_mask', 'Skull Mask', MISC, 1, 500, 2, 'A mask.'),
    ('crow_charm', 'Crow Charm', MISC, 1, 100, 2, 'A charm.'),
    ('gods_coins', "Gods' Coins", MISC, 10000, 50, 2, 'Sacred coins.'),
    ('horse_badge', 'Horse Badge', MISC, 1, 100, 2, 'A badge.'),
    ('homecomingtotem', 'Homecoming Totem', MISC, 1, 1000, 2, 'A totem.'),
    ('teleportationtotem', 'Teleportation Totem', MISC, 1, 1000, 2, 'A totem.'),
    ('chest_pendulum', 'Chest Pendulum', MISC, 1, 100, 2, 'A pendulum.'),
    ('pike', None, 0, 0, 0, 0, ''),
]
items = [i for i in items if i[1]]
assert len(set(i[1] for i in items)) == len(items)
START = 3800

# icon paths: verify they exist
rows = []
missing = []
for n, (icon, name, parent, stack, wt, ln, desc) in enumerate(items):
    p = os.path.join(C, 'art', '2D', 'Items', icon + '.png')
    if not os.path.exists(p):
        missing.append(icon)
    rows.append((START + n, parent, name, stack, wt, ln, desc, 'art/2D/Items/%s.png' % icon))
print('items', len(rows), 'id range', rows[0][0], rows[-1][0], 'missing icons', missing)
if missing:
    sys.exit(1)


def sql_esc(s):
    return s.replace("'", "''")


cs = ['// MMO items ported as plain items (name + icon, no recipes / food effects). Ids %d-%d.' % (rows[0][0], rows[-1][0]),
      'if (!isObject(LiFxMmoItemsPack)) { new ScriptObject(LiFxMmoItemsPack) { }; }', '',
      'package LiFxMmoItemsPack', '{',
      '    function LiFxMmoItemsPack::setup() {',
      '        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxMmoItemsPack);',
      '    }', '    function LiFxMmoItemsPack::version() {', '        return "1.0.0";', '    }',
      '    function LiFxMmoItemsPack::dbChanges() {']
for (i, parent, name, stack, wt, ln, desc, icon) in rows:
    price = 3000 if parent == 228 else 50
    exp = 1 if parent == 228 else 0
    cs.append('        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (%d,%d,\'%s\',0,0,0,0,0,0,0,0,%d,%d,%d,\'\',0,0,0,0,0,0,\'%s\',\'%s\',%d,NULL,%d,%d)");'
              % (i, parent, sql_esc(name), ln, stack, wt, icon, sql_esc(desc), price, exp, exp))
cs += ['    }', '};', 'activatePackage(LiFxMmoItemsPack);', 'LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxMmoItemsPack);', '']
# mimic tail of SiegePack
tail = open(S + r'\mods\LiFx\SiegePack\mod.cs', encoding='utf-8', errors='replace').read()[-200:]
print('SiegePack tail:', repr(tail))


def esc(x):
    return x.replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;')


xml = ''
for (i, parent, name, stack, wt, ln, desc, icon) in rows:
    f = [('ID', i), ('ParentID', parent), ('Name', name), ('IsContainer', 0), ('IsMovableObject', 0), ('IsUnmovableobject', 0),
         ('IsTool', 0), ('IsDevice', 0), ('IsDoor', 0), ('IsPremium', 0), ('MaxContSize', 0), ('Length', ln), ('MaxStackSize', stack),
         ('UnitWeight', wt), ('BackgndImage', ''), ('FaceImage', icon.replace('/', '\\')), ('Description', desc)]
    xml += '<row>\r\n' + ''.join('\t\t<%s>%s</%s>\r\n' % (k, esc(str(v)), k) for k, v in f) + '\t</row>\r\n'

open(os.path.join(os.environ['TEMP'], 'claude', 'MmoItems_mod.cs'), 'w', encoding='utf-8', newline='').write('\r\n'.join(cs))
open(os.path.join(os.environ['TEMP'], 'claude', 'MmoItems_rows.xml'), 'w', encoding='utf-8', newline='').write(xml)
if APPLY:
    os.makedirs(BK, exist_ok=True)
    cp = C + r'\data\objects_types.xml'
    shutil.copy2(cp, os.path.join(BK, 'client_objects_types.xml'))
    t = open(cp, encoding='utf-8', newline='').read()
    assert '<ID>3800</ID>' not in t
    k = t.rindex('</table>')
    open(cp, 'w', encoding='utf-8', newline='').write(t[:k] + xml + t[k:])
    d = S + r'\mods\LiFx\MmoItemsPack'
    os.makedirs(d, exist_ok=True)
    shutil.copy(os.path.join(os.environ['TEMP'], 'claude', 'MmoItems_mod.cs'), d + r'\mod.cs')
    print('applied')
