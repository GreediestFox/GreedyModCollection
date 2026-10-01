# BeveragesPack: six drinks with one buff and one drawback each

Implements the beverage brief with **existing effects only** (Plan A), no engine hook.

## How the engine applies a drink (found by decompiling the server)

`AbilityImp::Drink::_onDoPerform` (ability 205) calls `CharacterSaveableEffects::applyPotionItemEffect` (RVA 0x1C4A40). It reads the effect list of the **item instance**:

* database: `items.FeatureID` -> `features` row with `has_effects = 1`, plus rows in `item_effects (ItemID, EffectID, Magnitude)`;
* each `EffectID` is a row id of the table `effects`, whose `PlayerEffectID` is the effect in `cm_effects.xml`; `Magnitude / 1000` is the strength; the duration comes from the item quality (about 666 s at quality 100);
* after the listed effects the engine adds **Full** (25). An item without a list gets only Full, which is what every earlier attempt produced.

The GM give and the engine's item creation use `has_effects = 0`, so two database triggers on `items` attach the list to every new item of the listed types, however it was created (GM give, crafting, loot). The triggers and the `effects` mapping rows are created by `mod.cs` at every boot; the item type rows and the effect rows are also baked into `art/dump.sql` (the boot foreign-key check runs before mods).

## Drinks

| Item | Buff | Drawback | Tier |
|---|---|---|---|
| 3920 Thin Beer (Duennbier) | Accelerated | Clumsiness | 1 |
| 3922 Wheat Beer (Weissbier) | Stronger | Shaky Hands | 2 |
| Wine 962 (vanilla, "Wein") | Clever | Slowed | 2 |
| Mead 1119 (vanilla, "Met") | Harder (Willpower) | Shaky Hands | 2 |
| 3921 Strong Beer (Starkbier) | Swiftness | Slowed | 3 |
| 3923 Strong Spirits | Tougher | Clumsiness | 3 |

Magnitudes (in the trigger): speed effects 0.10 / 0.15 / 0.20, attribute buffs 3.0 / 5.0, Shaky Hands 0.15. They are first guesses to be tuned in game. Duration cannot be set per drink (it follows the quality).

Effect mapping rows (`effects.ID` -> player effect): 41 -> 6 Accelerated, 42 -> 5 Slowed, 43 -> 7 Clumsiness, 44 -> 8 Swiftness, 45 -> 79 Shaky Hands; vanilla rows 7 -> 19 Stronger, 9 -> 21 Harder, 10 -> 22 Clever, 11 -> 23 Tougher.

## Install

1. Copy `server/mods/LiFx/BeveragesPack` into the server's `mods/LiFx/`.
2. Bake the four `objects_types` rows (3920-3923, parent 37) and the five `effects` rows into `art/dump.sql` (see `mod.cs` for the exact values).
3. Add `3920 3921 3922 3923` to the Drink ability's `object_type_id` list in `skill_types.xml` (client AND server).
4. Client: append `client/data-additions/beverages/objects_types.rows.xml` to `data/objects_types.xml` and add the German strings from `loc_de.txt`.
5. Restart the server and relaunch the client; spawn the items with the GM panel and drink them.

## Things learned on the way

* A type with children must have weight 0, and every item type needs a non-zero weight, or the boot is refused.
* The engine rejects player effect ids above the vanilla range ("CmObjEffectsInit - bad id"); only 92 is free. The Plus doc's "ids from 100 up are free" does not hold.
* The `effects` table link `ResultPotionID`, random events on the Drink ability and parent categories do not give a drink its effects; only the per-item list does.
* Items of the same type with and without a list do not stack; items created before the triggers existed (or by GM before a restart) only give Full.

## Status

Confirmed in game on 2026-10-01 (after a relog, so the inventory reloads from the database): each drink applied exactly its two effects plus Full in the server log. Open: items created in memory by the GM give or by crafting only give Full until the inventory is reloaded; a Plus hook on the apply function (RVA 0x1C4A40) that applies configured effects by item type would remove that gap. Magnitudes are first guesses.
