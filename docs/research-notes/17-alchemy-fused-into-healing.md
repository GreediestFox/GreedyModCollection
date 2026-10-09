# 17 - Alchemy fused into Healing

## Idea (GreedyFox)
One skill instead of two: Healing should also cover alchemy (apothecary, cocktails, transmuting), nothing may get lost, and the old Alchemy section should disappear from the skill tab.

## What was done / tried
* **Before:** Herbalism 12 → Healing 14 → Alchemy 15. Alchemy had no child skills, no recipes and only two abilities:
  * 59 "Mix a Cocktail" on the Alchemist's Table / Herbalist's Shops 109, 140, 519
  * 60 "Transmute Into Gold!" on Lump of Iron 414, level 100
* Checked that the game logic is data-driven: an ability belongs to the skill row it sits in. The server string `Skill_Alchemy` is only part of the effect name/icon table, not skill logic.

## Output taken into the server
* All changes in `client/data-additions/alchemy_fusion/README.md`, applied by `tools/fuse_alchemy_into_healing.py`:
  * abilities moved, skill row removed from the XML on both sides and from the DB seed
  * skill-raise effects and the two Alchemist outfits retargeted to Healing
  * skill tab node and connector removed
  * descriptions merged
* Confirmed in game: the tab shows Healing ("Heilen & Alchemie") with all 8 abilities and the merged level texts.

## Technical notes
* **Skill tab layout** (`gui/scripts/skills.cs`): node positions are hard-coded per skill id (`getSkillItemPosition`), and a few nodes draw their own connector line to the next node (Healing: x+74, y+40, length 60).
  * The list of skills comes from native iterators (`getFirstBaseSkill` / `getNextSecondSkill`) over the client `skill_types.xml`, so removing the row is enough for the node to disappear.
* **`skill_type` DB table:** it is cleared and reseeded from `art/dump.sql` at every boot. Foreign keys reference it from `recipe.SkillTypeID`, `skills.SkillTypeID` and `skill_type.Parent`, so a removed skill must not be used by any recipe, character skill row or child skill.
* **Level texts:** `skill_types.xml` points to message ids per row (`Passive`, `DescLvl0/30/60/90/100`). Healing uses 178-183, the old Alchemy 184-189. `&#xA;&#xA;` gives a blank line in the skill window.
