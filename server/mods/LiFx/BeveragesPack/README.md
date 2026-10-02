# BeveragesPack: six drinks with one buff and one drawback each

Implements the beverage brief with **existing effects only** (Plan A), no engine hook.

## How the engine applies a drink (found by decompiling the server)

`AbilityImp::Drink::_onDoPerform` (ability 205) calls `CharacterSaveableEffects::applyPotionItemEffect` (RVA 0x1C4A40). It reads the effect list of the **item instance**:

* database: `items.FeatureID` -> `features` row with `has_effects = 1`, plus rows in `item_effects (ItemID, EffectID, Magnitude)`;
* each `EffectID` is a row id of the table `effects`, whose `PlayerEffectID` is the effect in `cm_effects.xml`; `Magnitude / 1000` is the strength; the duration comes from the item quality (about 666 s at quality 100);
* after the listed effects the engine adds **Full** (25). An item without a list gets only Full, which is what every earlier attempt produced.

The GM give and the engine's item creation use `has_effects = 0`, and the in-memory item has no list until the inventory is reloaded (relog), so database triggers only help after a relog. The working solution is the **Plus hook `drinkEffects`** (see the Plus repository, `docs/YO_SERVER_HOOKS.md`): it wraps `applyPotionItemEffect` and, for an item type configured in `lifxpluss.xml` that has no list of its own, applies the configured player effects first and lets the engine add Full. `mod.cs` registers the item types and drops the old triggers at boot; the item type rows are baked into `art/dump.sql` (the boot foreign-key check runs before mods).

## Drinks

| Item | Buff | Drawback | Tier |
|---|---|---|---|
| 3920 Thin Beer (Duennbier) | Accelerated | Clumsiness | 1 |
| 3922 Wheat Beer (Weissbier) | Stronger | Shaky Hands | 2 |
| Wine 962 (vanilla, "Wein") | Clever | Slowed | 2 |
| Mead 1119 (vanilla, "Met") | Harder (Willpower) | Shaky Hands | 2 |
| 3921 Strong Beer (Starkbier) | Swiftness | Slowed | 3 |
| 3923 Strong Spirits | Tougher | Clumsiness | 3 |

Magnitudes (in the `<drinkEffects>` section of `lifxpluss.xml`, see `server/config/lifxpluss.example.xml`): speed effects 0.10 / 0.15 / 0.20, attribute buffs 3.0 / 5.0, Shaky Hands 0.15. They are first guesses to be tuned in game. Duration cannot be set per drink (it follows the quality).

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

Confirmed in game (2026-10-01/02) with the Plus hook `drinkEffects`: each drink applies its two effects plus Full right away, also for items from the GM give or crafting (no relog needed); the drunk blur appears and the third drink in a row is refused like vanilla (Full accumulates). Magnitudes are first guesses and will be tuned after PvP tests.

## Hook configuration (server/config/lifxpluss.example.xml)

```xml
<drinkEffects enabled="1" verbose="1" dump="0">
    <db host="127.0.0.1" port="3306" user="root" password="CHANGE_ME" name="lif_1" />
    <drink objectTypeId="3920"> <effect id="6" magnitude="0.10" /> <effect id="7" magnitude="0.10" /> </drink>
    ...
</drinkEffects>
```

`effect id` is the PLAYER effect id of `cm_effects.xml` (1..93). The hook looks the item type up in the database by item id. Install the Plus DLL built from the `yo-engine-hooks` branch, add the section, restart the server.

## Bug found on the way

The third argument of `applyPotionItemEffect` is a **float** (the magnitude of Full, passed in xmm2). Declared as an integer in a hook it arrives as 0: Full is then added with magnitude 0, the drunk blur disappears and drinks stop counting towards the drink limit.
