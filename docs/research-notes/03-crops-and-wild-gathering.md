# 03 - Extra farmable crops and wild gathering

## Idea (GreedyFox)
Add new farmable plants (three grain types, hops, sugar beet) that can be sown, grow and be harvested like the vanilla crops; later also make the grains findable with the "search for wild plants" action, and let fishing drop the new fish.

## What was done / tried
* Reverse engineering of the shared sow / grow / harvest code on server and client, then a server hook plus a client patch that register extra crops at runtime.
* Several dead ends on the way (a substance id collision, a second client-side ability-to-seed lookup, the wrong map key); each was found from logs and fixed.
* A planned "swap wheat for a new grain in the wild-gather call" never fired in practice and was dropped; wild gathering was done data-driven instead (see below).

## Output taken into the server
* Five crops (Rye, Barley, Oat, Hops, Sugar Beet) confirmed end to end in game: sow, grow, harvest.
* Rye, Barley and Oat (and later the others) are wild-gatherable via the gatherables data (types 220-224).
* Hook source: `hook_crop_types.cpp/.h` (contributed to the Plus repository); client half: `tools/patch_client_crops.py` and `tools/patch_client_substance_alias.py`.
* Item rows live in `mods/LiFx/FoodDrinksPack/mod.cs` (ids 3175-3177, 3751, 3752). Crop visuals still reuse their alias target's look (wheat, grape, carrot).

## Technical notes
* All eight vanilla crops (abilities 133-140) share **one** native function, `AbilityImp::BasePlantImp::_onDoPerform`. Per-crop data (seed item, the two tilled-soil substances) lives in a runtime hash map keyed by ability id; the ability object's compiled code never hardcodes its own id, so a new ability id can reuse an existing crop's vtable and behave as an independent crop. The hook wraps the routine that registers all built-in abilities and, after the real registration, registers the configured extra crops and inserts their map rows through the engine's own insert function.
* Growth is arithmetic on the terrain substance id: a crop substance is one within `[101, 101+W)` (vanilla W = 75, eight inlined range checks that are widened in memory), advancing by +2 per stage (odd = fertile, even = non-fertile, six stages per 10-wide block). New crops need their own unused 10-wide block (191, 201, 211, 221, 231 used here).
* Validation of ability requirements uses a compiled-in substance registry which does not know the new ids (boot abort "invalid substance ter2ID"); the hook remaps lookups of the crops' stage substances to the alias base crop's stage ids (Wheat by default, `aliasBase` to use another).
* Harvesting does not use the crop map: it looks up the current substance byte in a separate map to learn the harvested item; the hook inserts the crop's seed item under its two Big-stage substances.
* Client needs the same: a Sow ability in `skill_types.xml` mirroring Wheat's block under the new id, matching substance/material pairs per stage, a second client-side "ability -> seed item" lookup (the map key had to be the ability id), and a client exe patch for the range checks.
* **Four places** need wiring per crop: server registration, server growth range, substance lookup alias, client ability/seed table (plus client range checks).
* Wild gathering: `gatherables.xml` (server and client identical) rows (type, shape `.stbin`, `objectTypeId`, quality range, `abilityId` 334, description); ability 68 has a `gatherable_type` requirement listing description words; the engine limit of 218 gatherable types was raised to 224 (server: four `cmp` sites patched in memory by the `maxGatherableType` config attribute; client: five sites patched on disk, see the README).
* Lesson: a one-off manual `INSERT INTO objects_types` is wiped at the next restart; register new items in a mod `dbChanges()` (see note 01).
