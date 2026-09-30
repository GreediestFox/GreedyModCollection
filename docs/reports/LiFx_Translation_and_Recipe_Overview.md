# LiFx mod objects and recipes: German translation and recipe overview

Generated 2026-09-30. Full per-ID lists (only active, in-DB entries matter; dormant ones are marked "commented"): `LiFx_ModObjects_Translation.csv` and `LiFx_ModRecipes_Translation.csv` (semicolon separated, open in Excel).

## 1. Where to translate (German client)

Folder: `F:\SteamLibrary\steamapps\common\Life is Feudal Your Own\data\loc\de\data\` (UTF-8 files, line format `<string id="ID">Text</string>`, keep `</strings>` at the end).

| What | File | string id = |
|---|---|---|
| Item / building / furniture / decoration NAME | `objects_types_Name.xml` | object ID |
| Item / building description | `objects_types_Description.xml` | object ID |
| Build-window / crafting recipe NAME | `recipe_Name.xml` | recipe ID |
| Recipe description | `recipe_Description.xml` | recipe ID |
| Ability names (e.g. Hinlegen, Gather) | `skill_types_ability_name.xml` | ability ID |
| Skill names | `skill_types.xml` | skill ID |

Missing entries fall back to the English name from `data\objects_types.xml` / `data\recipe.xml`. The client reads loc files at startup only, so relaunch after editing. There is no server-side translation; the server sends IDs only.

## 2. Translation status per pack (active entries only)

Object = item/building/furniture/decoration. "German missing" counts entries that have no German name yet.

| Pack | Object IDs | Objects | German missing | Recipe IDs | Recipes | German missing | Recipe edit status |
|---|---|---|---|---|---|---|---|
| AltarsIdolsPack | 3100-3105 | 6 | 6 | 5900-5905 | 6 | 6 | ORIGINAL MMO recipes |
| BigDryingFrame | 2829 | 1 | 0 | 1618, 5024 | 2 | 1 | own design |
| BigTanningTub | 2017 | 1 | 0 | 1189, 5021, 5600-5601 | 4 | 0 | own design |
| BuildingsPack | 3602-3603 | 2 | 2 | 6353-6354 | 2 | 2 | ORIGINAL |
| ChestsPack | 3537-3544 | 8 | 8 | 6294-6297 | 4 | 4 | EDITED (skill/levels) |
| CloakFlagPack | 3626-3642 | 17 | 17 | 6370-6386 | 17 | 17 | ORIGINAL MMO recipes |
| DisplaysPack | 3111-3152 | 42 | 42 | 5911-5940 | 30 | 30 | EDITED (skill/levels, decorations moved to Arts) |
| EkrBuildingsPack | 3655-3669, 3673-3676, 3679-3680, 3682-3685, 3688-3713, 3716-3734, 3737-3739, 3741-3750, 3753 | 84 | 84 | 6404-6414, 6418-6421, 6424-6425, 6427-6430, 6433-6454, 6457-6466, 6469-6470, 6472-6482 | 66 | 66 | mostly ORIGINAL ports (removed: 12 >20k-poly buildings, Tribe decos, Engelsstatue, Primitive Bed) |
| FoodDrinksPack | 3175-3193, 3751-3752 | 21 | 20 | 5952-5959 | 8 | 8 | ORIGINAL MMO recipes (no food effects for new MMO dishes) |
| FreshWaterPack | - | 0 | 0 | 6399-6403 | 5 | 5 | ORIGINAL |
| FurniturePack | 3153-3159, 3161-3167 | 14 | 14 | 5941-5947 | 7 | 7 | EDITED (skill -> Carpentry, levels) |
| HornSkinPack | 3643-3654 | 12 | 12 | 6387-6398 | 12 | 12 | ORIGINAL MMO recipes |
| JewelersWorkshop | 2894 | 1 | 0 | 1704, 5025-5038 | 15 | 0 | own design |
| JorvikModPack | 3001-3053 | 53 | 0 | 1088-1097, 1122, 1132-1146, 1148-1174 | 53 | 0 | EDITED (levels/skills changed) |
| KingOutfitsPack | 3611-3625 | 15 | 15 | 6355-6369 | 15 | 15 | ORIGINAL MMO recipes |
| KnoolMod-Pack | 2461-2466 | 6 | 0 | - | 0 | 0 | weapon recipes commented out, items still spawnable |
| LargeHerbalGarden | 2835 | 1 | 0 | 1624, 5200-5265, 5299-5365 | 134 | 66 | own design |
| MaterialsChainPack | 3545-3590 | 46 | 46 | 6314-6333 | 20 | 20 | metal bar/ingot/lump recipes commented out (28); remaining recipes ORIGINAL |
| MmoItemsPack | 3800-3913 | 114 | 114 | - | 0 | 0 | items only - no recipes |
| OilPressRabbitCage | 2834, 2873, 2895 | 3 | 0 | 1623, 1686, 1703 | 3 | 0 | own design |
| OreWasher | 2021 | 1 | 0 | 1180, 5610-5613 | 5 | 0 | own design |
| OutfitsPack | 3591-3592 | 2 | 2 | 6346-6347 | 2 | 2 | ORIGINAL MMO recipes |
| SiegePack | 3593-3601 | 9 | 9 | 6348-6352 | 5 | 5 | ORIGINAL MMO recipes |
| SignsPillarsPack | 3106-3110 | 5 | 5 | 5906-5910 | 5 | 5 | EDITED (skill/levels, pillars/lights moved to Arts) |
| SlavicFortifications | 2900-2923 | 24 | 0 | 5500-5523 | 24 | 0 | cloned from vanilla wall recipes (ingredients same as vanilla) |
| StatuesPack | 3169-3174 | 6 | 6 | 5949-5951 | 3 | 3 | ORIGINAL MMO recipes |
| StonemasonsWorkplace | 2020 | 1 | 0 | 1179, 5020, 5400-5405 | 8 | 0 | own design |
| StorageBuildings | 2400, 2830-2833 | 5 | 0 | 1619-1622, 5720 | 5 | 0 | own design |
| TailorsWorkshop | 2836 | 1 | 0 | 1625, 5022, 5100-5164 | 67 | 3 | own design (65 recipes cloned from vanilla tailoring) |
| Woodshed | 2480 | 1 | 0 | 1280 | 1 | 0 | own design |

Totals: 502 active objects (402 without German name), 528 active recipes (277 without German name).

## 3. Objects still needing a German NAME (file `objects_types_Name.xml`)

- **AltarsIdolsPack** (6): 3100-3105
- **BuildingsPack** (2): 3602-3603
- **ChestsPack** (8): 3537-3544
- **CloakFlagPack** (17): 3626-3642
- **DisplaysPack** (42): 3111-3152
- **EkrBuildingsPack** (84): 3655-3669, 3673-3676, 3679-3680, 3682-3685, 3688-3713, 3716-3734, 3737-3739, 3741-3750, 3753
- **FoodDrinksPack** (20): 3175-3179, 3181-3193, 3751-3752
- **FurniturePack** (14): 3153-3159, 3161-3167
- **HornSkinPack** (12): 3643-3654
- **KingOutfitsPack** (15): 3611-3625
- **MaterialsChainPack** (46): 3545-3590
- **MmoItemsPack** (114): 3800-3913
- **OutfitsPack** (2): 3591-3592
- **SiegePack** (9): 3593-3601
- **SignsPillarsPack** (5): 3106-3110
- **StatuesPack** (6): 3169-3174

## 4. Recipes still needing a German NAME (file `recipe_Name.xml`)

- **AltarsIdolsPack** (6): 5900-5905
- **BigDryingFrame** (1): 5024
- **BuildingsPack** (2): 6353-6354
- **ChestsPack** (4): 6294-6297
- **CloakFlagPack** (17): 6370-6386
- **DisplaysPack** (30): 5911-5940
- **EkrBuildingsPack** (66): 6404-6414, 6418-6421, 6424-6425, 6427-6430, 6433-6454, 6457-6466, 6469-6470, 6472-6482
- **FoodDrinksPack** (8): 5952-5959
- **FreshWaterPack** (5): 6399-6403
- **FurniturePack** (7): 5941-5947
- **HornSkinPack** (12): 6387-6398
- **KingOutfitsPack** (15): 6355-6369
- **LargeHerbalGarden** (66): 5300-5365
- **MaterialsChainPack** (20): 6314-6333
- **OutfitsPack** (2): 6346-6347
- **SiegePack** (5): 6348-6352
- **SignsPillarsPack** (5): 5906-5910
- **StatuesPack** (3): 5949-5951
- **TailorsWorkshop** (3): 5022, 5158, 5164

## 5. Kinds (what is a building / furniture / decoration)

`Kind` column in the objects CSV: Item, Building (unmovable), Movable (furniture, decorations, siege), Device/workshop. Decoration-type objects are the Movable ones in the packs DisplaysPack (mannequins, shields, banners, lights), SignsPillarsPack, AltarsIdolsPack, StatuesPack, CloakFlagPack, FurniturePack and JorvikModPack.

| Pack | Kind | Count |
|---|---|---|
| AltarsIdolsPack | Building | 6 |
| BigDryingFrame | Device/workshop | 1 |
| BigTanningTub | Device/workshop | 1 |
| BuildingsPack | Building | 2 |
| ChestsPack | Carried twin of a movable | 4 |
| ChestsPack | Movable (furniture/decoration/siege) | 4 |
| CloakFlagPack | Item | 17 |
| DisplaysPack | Building | 18 |
| DisplaysPack | Carried twin of a movable | 12 |
| DisplaysPack | Movable (furniture/decoration/siege) | 12 |
| EkrBuildingsPack | Building | 52 |
| EkrBuildingsPack | Carried twin of a movable | 17 |
| EkrBuildingsPack | Movable (furniture/decoration/siege) | 15 |
| FoodDrinksPack | Item | 21 |
| FurniturePack | Carried twin of a movable | 7 |
| FurniturePack | Movable (furniture/decoration/siege) | 7 |
| HornSkinPack | Item | 12 |
| JewelersWorkshop | Device/workshop | 1 |
| JorvikModPack | Building | 32 |
| JorvikModPack | Carried twin of a movable | 3 |
| JorvikModPack | Item | 13 |
| JorvikModPack | Movable (furniture/decoration/siege) | 5 |
| KingOutfitsPack | Item | 15 |
| KnoolMod-Pack | Item | 6 |
| LargeHerbalGarden | Device/workshop | 1 |
| MaterialsChainPack | Item | 46 |
| MmoItemsPack | Item | 114 |
| OilPressRabbitCage | Building | 1 |
| OilPressRabbitCage | Device/workshop | 1 |
| OilPressRabbitCage | Item | 1 |
| OreWasher | Device/workshop | 1 |
| OutfitsPack | Item | 2 |
| SiegePack | Carried twin of a movable | 4 |
| SiegePack | Item | 1 |
| SiegePack | Movable (furniture/decoration/siege) | 4 |
| SignsPillarsPack | Building | 5 |
| SlavicFortifications | Building | 24 |
| StatuesPack | Carried twin of a movable | 3 |
| StatuesPack | Movable (furniture/decoration/siege) | 3 |
| StonemasonsWorkplace | Device/workshop | 1 |
| StorageBuildings | Building | 5 |
| TailorsWorkshop | Device/workshop | 1 |
| Woodshed | Building | 1 |

## 6. Where the recipes live (to edit skill, level or ingredients)

- Server mod packs: `G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server\mods\LiFx\<Pack>\mod.cs` (`INSERT IGNORE INTO recipe` / `recipe_requirement` / `objects_types` lines).
- Baked rows that WIN over mod.cs (edit here, never in `sql\dump.sql`): `...\Dedicated Server\art\dump.sql`. This holds all vanilla recipes plus 65 mod types and the Slavic pack (objects 2900-2923, recipes 5500-5523, requirements 91000-91113).
- Client mirror (must match the server, needs client relaunch): `data\objects_types.xml`, `data\recipe.xml`, `data\recipe_requirement.xml`, `data\cm_objects.xml` (3D model per building/furniture) in the client folder.

## 7. How "unedited" was determined

I cannot diff against the MMO/Arden/Jorvik originals, so "ORIGINAL" means: ported recipe whose ingredients and quantities I never changed. "EDITED" means only skill/level (and the Arts/Carpentry category move) was changed by us, ingredients are still the original. Vanilla recipes touched: 1026-1030 and 1036-1040 (moved to Arts), 2, 5, 8, 1032, 1033 (level 90).