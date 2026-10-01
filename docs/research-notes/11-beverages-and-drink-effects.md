# 11 - Beverages: per-drink buff and drawback

## Idea (GreedyFox)
Six alcoholic drinks (Dünnbier, Weißbier, Wein, Met, Starkbier, strong spirits), each with one buff and one drawback built from existing effects, taking part in the game's drunk mechanic (blur, then "Full").

## What was done / tried
* Items under Beer, Alcohol and Cocktails, a link through the `effects` table, random events on the Drink ability, and custom effect ids: none gave a drink its own effects; every drink only gave Full.
* Decompiling `AbilityImp::Drink::_onDoPerform` and `CharacterSaveableEffects::applyPotionItemEffect` showed that the effects come from the effect list of the item instance; without one the engine adds only Full.
* First data-only version: database triggers that attach the list to new items (works after a relog).
* Final version: a Plus hook that applies configured effects by item type at the moment of drinking.

## Output taken into the server
* Items 3920-3923 plus vanilla Wine 962 and Mead 1119; `drinkEffects` section in `lifxpluss.xml`; confirmed in game (each drink applies its two effects without a relog).

## Technical notes
* Engine rules met on the first boots: a type with children must have weight 0; an item type needs a non-zero weight; player effect ids above the vanilla range are rejected ("bad id"), only 92 is free.
* `applyPotionItemEffect` RVA 0x1C4A40 (fx, item, **float** mult); core add-effect RVA 0x1C3AA0 (fx, effect id, float value, int ms, 0); item effect feature lookup 0x27C040; duration = q*q*1000/15 ms; the writer of the effect row is 0x1C3350. Full (effect 25) accumulates per drink; the client turns it into the blur (`initDrinkPostEffect`, called from the effect panel handler when effect 0x19 changes).
* A hook that declares the float argument as an integer makes Full's magnitude 0 (no blur, no drink limit).
* Database: `features.has_effects`, `item_effects (ItemID, EffectID, Magnitude)`, `effects (ID, PlayerEffectID)` mapping; `f_deleteItem` deletes the item's own features row, so rows must not be shared between items.
