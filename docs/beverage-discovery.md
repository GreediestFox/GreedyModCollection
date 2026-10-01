# Beverages: discovery note (step 0, nothing changed)

Read-only findings from the server install (`G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server`), its database `lif_1`, the client scripts and the Plus docs.

## 1. How a consumable is linked to an effect: found, but it is one effect per potion

* DB table **`effects`** (40 rows): `ID`, `Effect_name`, `ResultPreparationID`, `ResultPotionID`, `PlayerEffectID`. `PlayerEffectID` is the id in `data/cm_effects.xml`.
  Example: `effects.ID 8 "Raise Agi (temp)"`: preparation 793, **potion 784 (Swift Limbs Cocktail)**, player effect **20 (Faster)**. The nine cocktails are rows 1, 2, 5-11; the poisons are rows 13-18.
* One potion maps to exactly **one** player effect. So "one buff + one drawback" cannot come from two links; it needs either Plan B (one combined custom effect per drink) or another mechanism.
* `effects_sets` (EffectID1..3) is the alchemy mixing table (which effect triples a preparation combo gives); it is not an application table.
* `data/item_effects.xml` is the *equipment* buff table (item id -> `<effect id="21">1 20</effect>`, the id being a row of the DB `effects` table); the DB table `item_effects` is empty. Not usable for drinks.
* The `effects` rows come from the seed dump (`art/dump.sql` line ~6803, also `sql/new.sql`); mods can add rows with `INSERT IGNORE` in `dbChanges()` (FK to `objects_types`, so register the items first). `ID` and `PlayerEffectID` are `tinyint unsigned` (max 255), fine for custom ids from 100.

## 2. What marks an item as a drink: the "Drink" ability list

Ability **205 "Drink"** in `skill_types.xml` (server and client): `ent_req object_type_id parent="1"` = **`1091 962 1117 1118 1119 1532`**: Cocktails, Wine, Beer, Cider, Mead and Khtynka (the `parent="1"` form also matches descendants). Wine/Beer/Cider/Mead are children of 37 "Alcohol" and have no effect row and no description: they are drink-only items. Ability 206 "Eat the Preparation" covers parent 212.
**The dormant White Beer 3186 / Oat Stout 3187 / Red Ale 3189 are parent 228 "4 ingredients", which is eaten with ability 43 "Eat" (parents 227-232), not drunk.** They must not be reused as they are.
New drinks therefore need a parent from the 205 list (Beer 1117, Wine 962, Mead 1119, Cider 1118) so that "Drink" works and the blur mechanic can see them.

## 3. Blur and "Full"

* Blur is **client side**: `initDrinkPostEffect(%distQ, %refrac1, %refrac2)` (core/scripts/client/postFx.cs) enables `DrunkPostFX`, called by the engine; the three numbers could be per drink if the engine passes them, which cannot be read from data.
* "Full" is effect **25**, parameter `DRINK_IMMUNE:INCREASE`, German text "Du bist betrunken und kannst **keine Cocktails** mehr trinken." So it blocks cocktails; whether wine/beer/mead also count towards the three drinks that trigger it is engine logic I cannot see in data. It must be tested.
* Which blur strength each item gets, and how long it lasts, is not in any data file I found. The decision "blur lasts as long as the buff" therefore cannot be implemented from data; only observed.

## 4. Effects needed (names from `cm_effects.xml`)

| Wanted | Effect id | Parameter |
|---|---|---|
| Accelerated | 6 | `SPEED:INCREASE` |
| Slowed | 5 | `SPEED:DECREASE` |
| Clumsiness | 7 | `ATTACK_SPEED:DECREASE` |
| Swiftness | 8 | `ATTACK_SPEED:INCREASE` |
| Stronger / Faster / Harder (Will) / Clever / Tougher | 19 / 20 / 21 / 22 / 23 | `STR` / `AGI` / `WILL` / `INT` / `CON` `:INCREASE` |
| Shaky Hands | 79 | **no parameter** (text "Risiko zu taumeln, wenn du den Feind angreifst") |
| Full | 25 | `DRINK_IMMUNE:INCREASE` |

"Hardier" in the brief is effect 21 **Harder** (Willpower).

## 5. What that means for the six drinks

* A custom combined effect (Plan B, ids 100+) can use the same parameter tokens. Only `SPEED`/`ATTACK_SPEED` have a proven explicit magnitude form (Resurrected uses `SPEED:MULTIPLY=0.7`, `ATTACK_SPEED:MULTIPLY=0.5`). So **Dünnbier** (speed up, attack speed down) and **Starkbier** (attack speed up, speed down) are buildable from data as written.
* **Wein** (Clever + Slowed), **Strong spirits** (Tougher + Clumsiness): the attribute buff has no known magnitude form in data (the cocktails take magnitude from the potion). Needs a test whether `INT:INCREASE` / `CON:INCREASE` with a `value=` is honoured.
* **Weißbier** and **Met** need **Shaky Hands**, which is an effect without parameters: its behaviour is engine code, and it cannot be copied into a combined effect. Only how the engine applies effect 79 (it is not referenced by any skill or item in the data files) would tell whether it can be given to a drink.
* Whether the engine applies an `effects` row (`ResultPotionID` -> `PlayerEffectID`) when a non-cocktail (a child of Beer/Wine/Mead) is drunk is untested: today that mapping only exists for children of 1091.

## 6. Proposed next steps (need your decision)

1. **Smallest safe test:** add Dünnbier and Starkbier only (parents Beer 1117, items in a new `BeveragesPack` via `dbChanges`, custom effects 100/101 with `MULTIPLY` values, `effects` rows with `ResultPotionID`), spawn them by GM, drink them on the test server, and report: effect applied, blur, "Full" on the third, effect duration. This answers the three open engine questions in one go.
2. If that works, extend to Wein/spirits with an attribute test, and decide for Weißbier/Met whether to replace Shaky Hands by an effect that exists as data (for example Clumsiness) or to find the engine hook for effect 79.
3. Add descriptions, translations and client rows as in the brief only after the test.

Nothing was edited, no branch was created and the server was not restarted.


---

# Test results (2026-10-01, live server, items 3920 Thin Beer / 3921 Strong Beer)

Everything below was observed in the server log (`INSERT INTO character_effects ...`) after drinking the items.

| Attempt | Result |
|---|---|
| Items as children of Beer (1117) | Boot refused: "ObjectTypeID=1117 is parent type and has non-zero weight" (a type with children must have weight 0). |
| Gems with weight 0 | Boot refused: "ObjectTypeID=3830 has zero weight". All item types need a non-zero weight. |
| Custom effect ids 94, 95, 100, 101 in `cm_effects.xml` | "CmObjEffectsInit - bad id" for each; the engine accepts only ids up to the vanilla range. Only **92** is free. (The Plus doc says ids from 100 up are free: not true for the effect parser.) |
| Items under Alcohol (37), listed in Drink ability 205 | Drinking inserts only effect **25 (Full, 666 s, magnitude 1000000)**. The `effects` table link is ignored. |
| Items under Cocktails (1091), `effects` row with `ResultPotionID` | Same: only Full. |
| `effects` row with `ResultPreparationID` = `ResultPotionID` = item id | Same: only Full. |
| Random event on ability 205 with two `addeffect` elements (new event id 598) | Never fired. |
| The same event stored under an existing id (597) | Never fired. |

Conclusion: with data alone, the engine gives every one of these drinks only "Full". Per-drink buff and drawback needs engine work (a hook on the function that writes `character_effects`, which the Plus docs say has not been located) or a control test with a vanilla cocktail to see what the cocktail path does differently.


---

# Resolution (2026-10-01): it works, with data only

Decompiling `Drink::_onDoPerform` showed that the engine reads the effect list of the **item instance** (`features.has_effects` + `item_effects` rows) and adds Full afterwards; an item without a list gets only Full. Items created with that list applied both effects and Full in the first test (effects 92 and 19 for a Thin Beer, 8 and 20 for a Strong Beer, magnitude = row value x 1000, duration 666 s at quality 100). The pack therefore uses two database triggers that attach the list to every new item of the six drink types. See `server/mods/LiFx/BeveragesPack/README.md`.

Corrections to the earlier sections: the `effects` table link and random events are not used for drinks; custom effect ids are limited to the vanilla range; the brief's Plan B (custom combined effects) is not needed.
