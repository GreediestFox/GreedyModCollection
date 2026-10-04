# 13 - Daniel's mods: agriculture, food spoilage, tree felling, client extras, demo quest

## Idea
A friend's mod bundle (8 mods) ported onto this server. His mods were built against a near-vanilla install with their own DLL-injection launchers and full replacement copies of core files.

## What was done / tried
* Nothing was copied wholesale. His `dump.sql`, `cm_objects.xml`, `objects_types.xml`, `skill_types.xml` and `cm_messages.xml` are old full copies and would have wiped this server's content; only his actual changes were merged.
* His C++ hooks were moved into Plus (`server/plus-hooks/`, logging to `logs/agriculture.log`); the launchers and DLLs were not used.
* Skipped: the server namespace fix for the 32 "namespace parent linkage" boot errors (cosmetic, needs reconstructed scripts over the vanilla `.dso`, and would change which script callbacks shields/animals/NPCs get). YOscriptorium (needs its own web server) is not ported yet.

## Output taken into the server (all confirmed in game)
* **Agriculture:**
  * The plough (53) moves to cart category 77 (`ParentID 77`, container 300000), so it uses the wheelbarrow speed formula.
  * `skill_types`: ability 108 excludes 53; ability 96 lists the carts individually instead of `77`.
  * Hooks `plowCartParity` (judder fix; client twin `tools/patch_client_plow_parity.py`), `harvestSoil` (harvested tiles back to settled soil), `plowArea` (3x3 plowing).
* **Client extras:**
  * Texture preload at client start.
  * The craft window shows the required minimum quality per ingredient (message 797 extended).
  * Lamp posts switch light/fire off in daylight (own emitter `Lamppost_FireEmitter` on object 166, client and server `cm_objects`).
  * Script diffs in `client/scripts/DanielClientMods/`.
* **Food spoilage** (`FoodSpoilagePack`):
  * Once per >= 24 h, at server start only, food loses 2/4/8 quality by category; quality 0 turns into Dung 1032.
  * Barrels (106) pause it.
  * Bookkeeping lives in its own tables (`food_perish_*`), not in extra `items` columns.
  * The tooltip shows the remaining shelf life (messages 5195-5198).
* **Tree felling** (`TreeFellingPack`):
  * Pine/spruce drop an Amberwood Log 3930 and birch/aspen a Whitewood Log 3932 (carried twins 3931/3933), via the existing `treeDrops` hook.
  * They saw into billets 3934/3935 and boards 3800/3802 via the ported `logDescription`/`sawOutput`/`abilityEntityCheck` hooks.
  * The client saw menu is patched with `tools/patch_client_saw_menu.py`.
* **Demo quest:** Wranen the Hunter (subject 5, type 1513) gives quest 30 (bring a Branch, get a copper coin). Snippets are in `client/data-additions/quest30/`.

## Technical notes
* Quest ids must be free on BOTH sides: the server `quests.xml` only had dialog quests 1-20, but the client `quests_tasks.xml` already used 21 ("Mining"). A clash shows the wrong text in the quest tracker.
* The client exe section table is full; new client code caves go into unused space at the end of an existing patch section (`.subal`).
* Changing item quality in the DB while the server runs is unsafe (loaded items stay in memory); spoilage is therefore applied before the world loads.
