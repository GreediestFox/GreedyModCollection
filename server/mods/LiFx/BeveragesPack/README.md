# BeveragesPack (test): Thin Beer 3920 / Strong Beer 3921

Test of the beverage brief with two drinks. Each drink applies one combined custom effect (94 / 95: `SPEED` and `ATTACK_SPEED` `MULTIPLY` with fixed values and `ignore_magnitude="1"`), linked through a row in the database table `effects` (`ResultPotionID` -> `PlayerEffectID`) the way the cocktails are.

| Item | Parent | Effect | Speed | Attack speed |
|---|---|---|---|---|
| 3920 Thin Beer (Duennbier) | Cocktails 1091 | 94 | x1.15 | x0.85 |
| 3921 Strong Beer (Starkbier) | Cocktails 1091 | 95 | x0.85 | x1.15 |

Install: copy `server/mods/LiFx/BeveragesPack` into the server's `mods/LiFx/`; add the effect fragment to `data/cm_effects.xml` and fill the messages from `client/data-additions/beverages/messages.txt` on client AND server; append the item rows to the client `objects_types.xml`; bake the two `objects_types` rows into `art/dump.sql` (boot foreign-key check). Restart the server, relaunch the client, spawn the items with the GM panel and drink them.

What to observe (see `docs/beverage-discovery.md`): is the effect applied for a non-cocktail drink, are the multipliers above 1 honoured, how long does it last, how much blur, does the third drink apply "Full", does Thin Beer + Strong Beer together behave sensibly.

Engine rules learned on the first boot: an item type must have a non-zero weight, and a type that has children must have weight 0 (so Beer 1117 cannot be a parent; the new beers are siblings under 37).

Second finding on the first boot: the engine rejects custom player effect ids of 100 and above ("CmObjEffectsInit - bad id" for each); ids 94 and 95 (unused gap in 1-93) are used instead. The Plus note that ids from 100 up are free is wrong for the effect parser.

Third finding (first in-game test): items under Alcohol (37) are drunk by the engine as plain alcohol: it ignores the `effects` link and inserts only effect 25 (Full, 666 s, magnitude 1000000) into `character_effects`, for both beers. Only items in the Cocktails category (1091) use the `effects` link, so the two drinks now sit under 1091.

Status: the items load and can be drunk, but the engine applies only Full for them; see `docs/beverage-discovery.md` for every attempt and its result.
