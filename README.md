# LIF-PROJECT: Life is Feudal: Your Own - server + client modding work

A complete, documented record of a modded **Life is Feudal: Your Own (LiF:YO)** dedicated server and client: ~500 custom objects and ~500 recipes (buildings, furniture, decorations, items, workshops, crops, outfits, siege), the engine hooks that make them work, the client patches, and the tools used to build and maintain it all.

It builds on the **LiFx / Plus** framework: <https://github.com/LiF-x/Plus>. The engine-hook source code lives there (see "Plus hooks" below); this repo contains everything around it.

> **No game art is included.** Models (`.dts`), textures (`.dds`), icons (`.png`) and sounds that come from the MMO build, Arden, EKR, Jorvik or the base game are copyrighted by their owners and are not distributed here. The mod packs reference them by path; you must supply your own copies (see "Art").

> **Third-party mod packs are not included.** The Jorvik mod, the Knool pack and the EKR buildings pack belong to their authors; their `mod.cs` files and data rows are left out of this repository (the docs and reports still list what the ids were used for). Install those packs from their authors, then use the notes here to merge them. All other packs here are our own code that registers content ported from the MMO/Arden builds; the art is not included.

## Earlier material in this repository

* `tools/pack-merge/` - PowerShell scripts used to renumber and merge a third-party mod pack's data into an existing build (object/recipe id remapping, skill_types.xml edits, mod.cs generation).
* `docs/sound-editing.md` - how client-side sound editing works and what to watch out for.
* `docs/id-ranges.md` - id ranges used for custom content.

## What is in here

```
server/mods/LiFx/<Pack>/mod.cs   32 server mod packs (objects, recipes, conversions, requirements; INSERT IGNORE in dbChanges())
server/config/lifxpluss.example.xml   config for every Plus hook used (DB password removed)
server/db/mod_rows_from_dump.sql      mod rows that live in the seed dump (art/dump.sql)
client/data-additions/*.xml       rows/blocks to add to the client's data\*.xml (objects_types, recipe, recipe_requirement, cm_objects)
client/scripts/                   client-side Torque scripts we wrote (GM panel helper, lay-down pose, mesh hiding)
tools/                            Python tools (client exe patchers, DTS tools, icon porting, item generator, report generators)
docs/reports/                     per-ID lists of every mod object/recipe with German translation status
docs/research-notes/              reverse-engineering notes and lessons learned (RVAs, mechanics, pitfalls)
```

## Requirements

* LiF:YO **dedicated server** (Steam) with the LiFx/Plus loader installed (<https://github.com/LiF-x/Plus>), MariaDB running (the server start script does this).
* LiF:YO **client** (Steam).
* Python 3 with Pillow for the tools.
* Your own copies of the art assets (see below).

## Install on a server (short version)

1. **Back up** the server folder and the client folder.
2. Copy `server/mods/LiFx/*` into `<server>/mods/LiFx/`. Packs are independent; take only the ones you want. A pack's first comment line says what it contains and its id range. Some packs are intentionally dormant (all lines commented): uncomment to enable.
3. Merge `server/config/lifxpluss.example.xml` into your `<server>/config/lifxpluss.xml` (only the hook elements you need, and **set your own DB password**).
4. Client side, for every pack you installed: append the matching rows from `client/data-additions/*.rows.xml` to the client `data\objects_types.xml`, `recipe.xml`, `recipe_requirement.xml` (before the closing `</table>`), and the blocks in `cm_objects.blocks.xml` to BOTH the client's and the server's `data\cm_objects.xml` (inside `<object_types>`). Ids must be identical on both sides.
5. Copy the art into `<client>/mod/<PackName>/art/...` exactly at the paths the rows use (`FaceImage`, `shapeName` fields), and, for the server-side models, into `<server>/mod/...`.
6. Materials: models use named materials that must be defined in the client `art\materials.cs` (one `singleton Material(...)` per material name with `mapTo`, `diffuseMap[0..2]`, `materialTag0 = "LiF"`). If a model renders orange/untextured a material definition is missing. Delete any stale `materials.cs.dso` afterwards (the client prefers `.dso` over `.cs`).
7. Start the server with its start script, watch the boot log, then start the client (data files are read at startup only).

### Rules that cost us days (read these)

* The server wipes and reseeds `objects_types` / `recipe` from `art/dump.sql` at **every boot**, then runs each mod's `dbChanges()` with `INSERT IGNORE`. A row already in the seed dump **wins** over the mod's row, and a manual SQL INSERT is gone after the next restart. Edit `art/dump.sql` (not `sql/dump.sql`, which is a copy) or the mod.
* `sp_checkForeignKeys` runs **before** mod `dbChanges()`: a dangling reference (objects_patch, ent_req, item list) to a mod-only type is a fatal boot error.
* Every new **movable** object needs: a base type, a carried twin (`ParentID 1902`) and an `objects_conversions` row. The default datablock limit (~220 movable types) needs the Plus `datablockRange` hook plus the matching client exe patch.
* Steam "verify files" / updates silently restore stock server files and the client exe: re-apply mods and patches afterwards.
* Client `data\prefs.cs` is rewritten on exit; edit it only while the client is closed, and delete a stale `prefs.cs.dso`.
* Translations: client `data\loc\<lang>\data\objects_types_Name.xml` (id = object id) and `recipe_Name.xml` (id = recipe id), plus the `_Description` files. Missing entries fall back to English.

## Plus hooks (engine extensions)

The C++ hooks (extra crops and wild gatherables, datablock range, stable/greenhouse aliases, workshop quality buff, well water, cart capacity, herb-garden time gate) are contributed to the Plus repository, with documentation: <https://github.com/LiF-x/Plus> (`docs/YO_SERVER_HOOKS.md`).

## Client exe patches

`tools/patch_client_*.py` patch a **copy of your own** `yo_cm_client.exe` (they verify the original bytes first and refuse otherwise, and write a backup):

| Tool | Purpose |
|---|---|
| `patch_dbrange_client.py` | widen the datablock id range (pairs with Plus `datablockRange`) |
| `patch_client_crops.py`, `patch_client_substance_alias.py` | client half of the custom crops |
| `patch_client_skins.py` | adds custom skins to the wardrobe list |
| `patch_client_textedit_cut.py` | fixes an engine text-edit crash (`StringBuffer::cut`) |

Gatherable limit (wild plants, 218 -> 224): client needs the `cmp` immediate patched at RVAs 0x4F13E8, 0x4F1468, 0x4F14D8, 0x4F1608 and 0x4F1830 (see `docs/research-notes/03-crops-and-wild-gathering.md`).

## Other tools

* `dts_clone.py`, `verify_clone.py`: clone a character/object `.dts` with a different material name (recolors and skins without new meshes).
* `make_dxt1.py`: write valid DXT1 DDS with mip chain (Direct3D needs compressed DDS with mips here).
* `port_icons.py`: copy icon PNGs from zips into a client `art\2D` with a backup and manifest.
* `items_gen.py`: generates a plain-item mod pack + matching client rows from a list (name, icon, category).
* `loc_report.py`, `loc_report2.py`: generate the per-ID translation/recipe status reports in `docs/reports`.
* `minidump_stack.py`, `x64dis.py`, `findcave.py`, `exe_strings_near_rva.py`: crash-dump and exe analysis helpers used for the reverse engineering.

The tools contain absolute Windows paths at the top (server, client, scratch folders): edit them first.

## Art

Packs reference their art under `mod/<Name>/art/...` (client) and the same relative path on the server. Expected folders: `AltarsIdolsMod`, `BuildingsMod`, `ChestsMod`, `CloakFlagMod`, `DisplaysMod`, `EKRBuildingsMod`, `FoodDrinksMod`, `FurnitureMod`, `HornSkinMod`, `JorvikMod`, `KingOutfitsMod`, `MaterialsChainMod`, `OutfitsMod`, `SiegeMod`, `SignsPillarsMod`, `StatuesMod`, `TierGearMod`. Source builds: the LiF MMO client, Arden, the EKR modpack and the Jorvik mod; obtain those from their authors.

### Packs, id ranges and recipe status

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


Legend: ORIGINAL = ported recipe, ingredients untouched. EDITED = only skill/level (and category) changed. Full per-ID lists: `docs/reports/`.

## Status / known gaps

* ~400 of the active objects and ~280 of the active recipes have no German name yet (`docs/reports/` lists exactly which).
* The 114 `MmoItemsPack` items are plain (name + icon): no recipes and no food effects.
* Crop 3D models still reuse their alias target's model; the outfit materials King, Tatters, Surcoat, FlagOfficer and Merchant are not mapped in the YO client.

## Legal

Unofficial fan modding for a game we do not own or represent. Not affiliated with the game's developers. Game assets are the property of their respective owners and are intentionally not included. 

## License

This repository is licensed under the **GNU General Public License v3.0** (see [`LICENSE`](LICENSE)), the same license as the LiFx/Plus framework it builds on (<https://github.com/LiF-x/Plus/blob/main/LICENSE>). Game assets are not covered by this license and are not included.
