# 19 - Animal fodder: quality bonus for newborn animals, no quality gain from ordinary feed

## Idea (GreedyFox)
* Ordinary feed must no longer raise the quality of animals kept in stables, barns and coops.
* Three new fodder items, one per building type, give every **newborn** animal +2 quality, but only while the fodder is in that building.
* Slaughtering gives +5 quality on the products, but only in the in-game months October to February.

## Items and recipes (`server/mods/LiFx/AnimalFodderPack`)
| Item | Id | Building | Recipe | Ingredients |
|---|---|---|---|---|
| Grosstierfutter (Large Animal Feed) | 370 | stables (horses, moose) | 6500 | 10 straw, 5 oat, 5 carrot, 10 rye |
| Nutztierfutter (Livestock Feed) | 371 | barn (cows, pigs, sheep) | 6501 | 10 straw, 5 peas, 5 apples, 10 rye |
| Kleintierfutter (Small Animal Feed) | 372 | coop (chickens, rabbits) | 6502 | 10 nuts, 5 barley, 5 carrot, 10 wheat |

* Crafted at the Quern-stone (116) with Advanced Farming (32), level 30, 10 items per craft. Weight: 1500 (like straw) for 370/371, 230 (like a carrot) for 372.
* The ids 370-372 are not a free choice, see "Feed list" below.

## Engine facts (server ddctd_cm_yo_server.exe, RVAs for the 2026-10 Steam build)
* **Feed list:** "is this item animal feed" is not data driven. Two tiny predicates test the item type id against a hard-coded bit mask `0x80055D` over the ids 0x160..0x177 (352..375: wheat, peas, carrot, straw, flour, ...), plus 0x402, 0x403, 0x406:
  * `0x1B0E70 isFeed(mgr, typeRecord*)` (record id at +8): used by the feeding tick and `checkContainerLimits`
  * `0x1B0E10 isFeed(mgr, itemTypeId)`: used by `checkContainerLimits` (feed weight cap)
  * The **client has its own copy** of the mask (two sites, `mov edx,80055Dh`), and the trough window refuses anything outside it. A new feed item therefore needs an id inside 352..375 and a client exe patch (`tools/patch_client_feedmask.py`: `0x80055D` -> `0x9C055D`, which turns on the free ids 370, 371, 372).
* **Feeding tick** `0x1B29D0`: animals eat by item weight; the stable's FoodQuality (+0x38) is the weighted average of the eaten items' quality.
* **Vanilla quality gain:** the per-animal tick `0x1B2FA0` raises an animal's quality by 1 (`INC R8B`, bytes `41 FE C0`, at `0x1B33AC` and `0x1B33FC`) when FoodQuality is high enough and the animals are fed and clean. The hook replaces both with a 3-byte NOP (decay when starving or dirty is untouched).
* **Newborns:** the breeding tick `0x1B21E0` computes the quality from the two parents (capped at 100) and calls `CmStable::bornAnimal(stable, inventory, animalTypeId, quality)` at `0x1AC2E0`.
  * Inventory wrapper +0x48 -> map (+0x30 buckets, +0x38 count); node [2] = item; item +0x18 = type record (+8 = id), +0x24 = quantity.
* **Slaughter:** every product goes through `_slaughterAddProduct(mgr, inventory, itemType, quantity, quality, ptr)` at `0x1ABD00`, called from `0x1ADBC0`, which is reached from the stable slaughter (`0x1B37B0`) and from `SlaughterAnimalsInv::_onDoPerform` (`0x356CF0`). So the bonus also applies to animals slaughtered from a player inventory.
* **In-game date:** the body of `getGameTime()` is at `0x468760` (no arguments, returns `"DD-MM-YYYY HH:MM:SS"`); the month is characters 3-4.

## The Plus hook `hook_animal_fodder` (`server/plus-hooks`)
```xml
<animalFodder enabled="1" verbose="1" disableFeedGrowth="1" newbornBonus="2" slaughterBonus="5" slaughterMonths="10 11 12 1 2">
    <fodder itemTypeId="370" animals="1039 1040 1042 1043 1044 1045 1140 1685 1663" />
    <fodder itemTypeId="371" animals="1046 1047 1048 1049 1050 1051" />
    <fodder itemTypeId="372" animals="1052 1053" />
</animalFodder>
```
* detours both feed predicates (fodder counts as feed), `bornAnimal` (+newbornBonus when the species' fodder is in the building) and `_slaughterAddProduct` (+slaughterBonus inside the month window); optionally NOPs the two `INC R8B` sites.
* Every target is byte-checked before attaching; a different server build logs an error and skips the hook.
* Log: `logs/animal_fodder.log` (every newborn says whether it found fodder).
* Wired with `ConfigureAnimalFodder(root)` after `ConfigureStableAlias` and `AttachAnimalFodderHook()` after `AttachStableAliasHook` in `cm_server.cpp`.

## Traps found on the way (worth remembering)
* **Foreign keys are validated before mods run `dbChanges()`.** If a player inventory holds an item of a mod-only type, the boot dies with `invalid key(s) of items.ObjectTypeID`. Every mod item type that can end up in an inventory must be baked into `art/dump.sql` (the real DB seed). Renumbering an item id that exists in `items` needs `SET FOREIGN_KEY_CHECKS=0; UPDATE items ...` before the next boot.
* **The client finds types and names by binary search.** New rows in `objects_types.xml` and the loc files must be inserted at their sorted place. Appended at the end they broke lookups of other items (animals showed up as other objects).
* **A patcher-managed client reverts every local data edit on the next Steam start.** For tests, start `yo_cm_client.exe` directly with `SteamAppId=SteamGameId=SteamOverlayGameId=290080`; for good, release a new pack version.
* An exe patched by an older patch build is not "stock" for the patcher: it needs an upgrade path from the kept `.gmc_stock` copy (done in GmcPatcher 1.0.1).

## Not verified in game yet
* Animals eating the fodder, the +2 on a newborn and the +5 at slaughter (all depend on offsets derived from decompiled code). Recipes, crafting, the trough and the weights are confirmed.
* Ordinary breeding inheritance is unchanged: only the in-stable growth from feeding is removed.
