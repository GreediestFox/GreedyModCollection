# Alchemy fused into Healing

Alchemy (skill 15) no longer exists as its own skill. Healing (14) carries all of its abilities, and the skill tab no longer shows an Alchemy node. Apply with `tools/fuse_alchemy_into_healing.py`; it is safe to run more than once.

What changes (server + client unless noted):
- `data/skill_types.xml`: abilities 59 "Mix a Cocktail" (apothecary: objects 109 / 140 / 519, level 0) and 60 "Transmute Into Gold!" (level 100) move into the Healing row; the Alchemy row is removed.
- Server `art/dump.sql` and `sql/dump.sql`: the `skill_type` row 15 is removed.
- `data/cm_effects.xml`: the skill-raise effects 31 / 33 point to Healing; effect 32 drops its Alchemy parameter (it already raises Healing).
- `data/cm_equipTypes.xml`: Alchemist's Outfit 305 and Master Alchemist's Outfit 3927 require Healing 60.
- Client `gui/scripts/skills.cs` (`skills.cs.patch`): no Alchemy node position, and Healing no longer draws the connector line.
- `data/cm_messages.xml` (EN) and `loc/de/data/cm_messages.xml` (DE): the Healing passive (178) and level texts (179-183) also describe the former Alchemy (184-189), separated by a blank line.
- `loc/de`: skill 14 is shown as "Heilen & Alchemie"; the effect names 31-33 are adjusted.

No recipe and no child skill used Alchemy. Characters that already have Alchemy points would need their value moved to Healing before the row is removed.