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
* Item rows live in `mods/LiFx/FoodDrinksPack/mod.cs` (ids 3175-3177, 3751, 3752).
* Own crop visuals (2026-10-02, confirmed in game): Rye, Barley and Oat use the real grain plants from the MMO client, Hops a grape vine with green cones, Sugar Beet a carrot plant with a white root; inspecting a field names the crop itself. Client tools: `tools/build_crop_visuals.py`, `tools/make_hops_beet.py`, `tools/png_to_dds.py`, `tools/toggle_client_substance_alias.py` (the client substance alias is switched OFF again once the visuals are in).

## Technical notes
* All eight vanilla crops (abilities 133-140) share **one** native function, `AbilityImp::BasePlantImp::_onDoPerform`. Per-crop data (seed item, the two tilled-soil substances) lives in a runtime hash map keyed by ability id; the ability object's compiled code never hardcodes its own id, so a new ability id can reuse an existing crop's vtable and behave as an independent crop. The hook wraps the routine that registers all built-in abilities and, after the real registration, registers the configured extra crops and inserts their map rows through the engine's own insert function.
* Growth is arithmetic on the terrain substance id: a crop substance is one within `[101, 101+W)` (vanilla W = 75, eight inlined range checks that are widened in memory), advancing by +2 per stage (odd = fertile, even = non-fertile, six stages per 10-wide block). New crops need their own unused 10-wide block (191, 201, 211, 221, 231 used here).
* Validation of ability requirements uses a compiled-in substance registry which does not know the new ids (boot abort "invalid substance ter2ID"); the hook remaps lookups of the crops' stage substances to the alias base crop's stage ids (Wheat by default, `aliasBase` to use another).
* Harvesting does not use the crop map: it looks up the current substance byte in a separate map to learn the harvested item; the hook inserts the crop's seed item under its two Big-stage substances.
* Client needs the same: a Sow ability in `skill_types.xml` mirroring Wheat's block under the new id, matching substance/material pairs per stage, a second client-side "ability -> seed item" lookup (the map key had to be the ability id), and a client exe patch for the range checks.
* **Four places** need wiring per crop: server registration, server growth range, substance lookup alias, client ability/seed table (plus client range checks).
* Wild gathering: `gatherables.xml` (server and client identical) rows (type, shape `.stbin`, `objectTypeId`, quality range, `abilityId` 334, description); ability 68 has a `gatherable_type` requirement listing description words; the engine limit of 218 gatherable types was raised to 224 (server: four `cmp` sites patched in memory by the `maxGatherableType` config attribute; client: five sites patched on disk, see the README).
* Lesson: a one-off manual `INSERT INTO objects_types` is wiped at the next restart; register new items in a mod `dbChanges()` (see note 01).

## Crop visuals (2026-10-02)
* Symptom: every custom crop looked like its alias crop. Changing the crop substances' `terrainMaterialName` in `scripts/cm_substances.cs` changed nothing, and switching the client alias off made the client refuse to start ("failed to load skills/abilities", invalid substance ter2ID).
* Cause, from `SubstanceManager::RegisterSubstance` (server `0x572340`, same code in the client): a substance is rejected if its ter2 id, its **name** or its **terrainMaterialName** is already registered (three hash maps; the log says "substance already exists (id, name)" in all three cases, which hides the real reason). Our crop substances reused the vanilla crops' terrain material names, so all 30 were rejected, and the client alias patch only masked that.
* The client also loads a stale `*.cs.dso` instead of an edited `.cs` when both exist (`scripts/cm_substances.cs`, `art/terrains/materials.cs`, `scripts/client/cm_environment.cs`): rename the `.dso` or edits have no effect.
* Fix (client only): a unique `terrainMaterialName` per crop stage, a matching `TerrainMaterial` (clone of the template crop's, new `internalName`, next free `globalIndex`; vanilla uses 0-88, we use 89-118), and `GroundCover` blocks inside `EnvironmentGroup` whose `layer[N]` is that terrain material name. A GroundCover holds at most 8 layers and one `Material`; the four vanilla crop blocks are full, so the custom crops get their own blocks (grains with the MMO atlas `CropsB`, hops/beet with `CropsAtlas`). With the substances registered for real the client alias is no longer needed; the server keeps its alias (it has no script substances).
* SpeedTree crop models (`*.stbin`) carry their texture names inside the file (`CropsAtlas.tga`, `_Normal`, `_Specular`); the engine loads `<name>.dds` from the model's own `textures/` folder and only streams `.dds` (a `.png` is ignored, the plant is then invisible). Recolouring a crop without touching the vanilla one: copy the model, replace the texture name with a same-length name, and ship a recoloured atlas under that name.
* Inspect ("You see planted ...") on the server looks the cell up through the aliased substance and then asks `0x3A9700` (only caller `AbilityImp::Inspect::_inspectEntity` `0x395AC0`) for the seed item of the alias crop's block, so a custom grain was reported as Wheat. The Plus hook remembers the real crop on the alias call and swaps the block back. The German message 2689 used `%1` instead of `%(o1)` and printed the item id.
