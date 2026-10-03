# 12 - Player effect slots, MMO effect ports and Geselle/Meister outfits

## Idea (GreedyFox)
More player effects without crashing on the fixed effect limit; and craft outfits split into a normal "Geselle" tier and a "Meister" tier (the former wardrobe skin as its own item, made only at the Tailor's Workshop), each giving ONE effect that raises all of its skills.

## What was done / tried
* RE of the effect limit: player effects are fixed 94-slot arrays (ids 0..93) on server and client, sent with 7 bits. A full expansion is possible but large; instead a census found the slots no data or engine code uses.
* Ported from the MMO into free slots: 61 Exhausted, 84 Increased XP Gain, 64 Shackled Feet (name + icon), 92 Slightly Stronger (clone of 19). 61/64/84 are cancelable (`/EFFECT id 1` removes them).
* Outfits: skins removed from the five vanilla craft outfits (303-307); Meister items 3925-3929 use the skin meshes; 3653/3654 are the Meister for the Stonecutter/Breeder outfits (3591/3592).

## Output taken into the server
* `cm_effects.xml` (server and client identical): merged skill effects in slots 27, 29, 31, 32, 34, 77, 78, 80, 81, 85; slots 86 and 87 are still free.
* DB `effects` rows 46-56 (OutfitsPack), `data/item_effects.xml` item -> DB effect, Meister recipes 6483-6487 and 6397/6398 (Tailor's Workshop, Tailoring 90, Geselle outfit + cloth + leather + 5 gold/silver leaf).
* Plus hook `craftEffectsRange` (copy in `server/plus-hooks/`). All 14 outfits confirmed in game; quality test: 52 without, 61 with the outfit.
* Scripts: `tools/outfit_tiers.py`, `tools/port_mmo_effects.py`, `tools/add_effect92.py`, `tools/kit_report.py` (lists recipes that still need decorator kits).

## Technical notes
* "Skill Raised" effects are data, not code: `<parameter type="SKILL" skill="N" applytype="INCREASE"/>`. One effect may carry several parameters (vanilla 47 Resurrected has 9), so a merged effect is one entry with several SKILL lines; the magnitude applies to each.
* Chain: `item_effects.xml` `<effect id=E>a b</effect>` -> DB `effects.ID = E` -> `PlayerEffectID` -> `cm_effects.xml`. The craft effect is stamped on the item when it is created; older items keep what they had.
* `CraftItemEffectsManager::init` (RVA 0x4DA990) accepts only E = 1..38 (`cmp ecx,25h` at 0x4DAC33, `cmp dil,26h` at 0x4DAC3F) and logs "bad item effect id" otherwise. The DB effects table is a byte-keyed map, so only these two checks needed raising (hook: 1..127, in memory).
* The server deletes and reseeds the `effects` table at boot, like `objects_types` and `recipe`.
* German effect names live in `loc/de/data/cm_effects_name.xml`; it contains MMO leftovers for many ids, so overwrite an id when repurposing its slot.
