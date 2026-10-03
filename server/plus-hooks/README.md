Reference copy of the drinkEffects hook source. The maintained version is in the Plus repository (branch `yo-engine-hooks`, file `source/server/hooks/engine/`).

hook_craft_effects_range.cpp/.h: raises the item_effects.xml effect id limit from 1..38 to 1..127 (config <craftEffectsRange enabled="1"/>), needed for the Geselle/Meister outfit effects (see docs/research-notes/12).
