# BeveragesPack (test): Thin Beer 3920 / Strong Beer 3921

Test of the beverage brief with two drinks. Each drink applies one combined custom effect (100 / 101: `SPEED` and `ATTACK_SPEED` `MULTIPLY` with fixed values and `ignore_magnitude="1"`), linked through a row in the database table `effects` (`ResultPotionID` -> `PlayerEffectID`) the way the cocktails are.

| Item | Parent | Effect | Speed | Attack speed |
|---|---|---|---|---|
| 3920 Thin Beer (Duennbier) | Beer 1117 | 100 | x1.15 | x0.85 |
| 3921 Strong Beer (Starkbier) | Beer 1117 | 101 | x0.85 | x1.15 |

Install: copy `server/mods/LiFx/BeveragesPack` into the server's `mods/LiFx/`; add the effect fragment to `data/cm_effects.xml` and fill the messages from `client/data-additions/beverages/messages.txt` on client AND server; append the item rows to the client `objects_types.xml`; bake the two `objects_types` rows into `art/dump.sql` (boot foreign-key check). Restart the server, relaunch the client, spawn the items with the GM panel and drink them.

What to observe (see `docs/beverage-discovery.md`): is the effect applied for a non-cocktail drink, are the multipliers above 1 honoured, how long does it last, how much blur, does the third drink apply "Full", does Thin Beer + Strong Beer together behave sensibly.
