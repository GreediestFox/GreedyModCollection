# Brief: alcoholic beverages for the LiF:YO server

You are working on a Life is Feudal: Your Own (LiF:YO) server with the LiFx mod framework. This brief is a handoff from an earlier chat. Read it fully before touching anything. Do not start from zero: the research is done, the open questions are listed, and the first job is to find the files the earlier chat could not see.

## 1. Goal

Add six alcoholic drinks. Each gives one buff and one drawback. The game's existing drunk mechanic (blur per drink, then "Full" after three) should apply to them. All of this should be done with data changes, because the owner has no scripting access beyond the LiFx `mod.cs` mods in the repo.

| Drink (German name) | Buff | Drawback | Strength tier |
|---|---|---|---|
| Dünnbier | Accelerated | Clumsiness | 1 (lightest) |
| Weißbier | Stronger | Shaky Hands | 2 |
| Wein | Clever | Slowed | 2 |
| Met | Hardier | Shaky Hands | 2 |
| Starkbier | Swiftness | Slowed | 3 |
| Strong spirits | Tougher | Clumsiness | 3 (strongest) |

Notes on the table:
- Met was changed from Stronger to Hardier (Willpower) because Weißbier already used Stronger + Shaky Hands. The owner may still change this. If they prefer to keep Stronger on Met, give Met the Slowed drawback instead.
- Effect names are the ones shown in the game's effect list. Look up their ids by name in `cm_effects.xml`; do not guess ids.
- Tier is a proposal for duration and strength. Use it only if the data allows setting strength or duration per item.

Hangover: **the game already has this mechanic.** Each drink makes the player's vision blurrier, and after three drinks the player gets the debuff "Full", which blocks further drinking for a while. So do not build a separate hangover or a drink counter. The job is to make the new drinks take part in that existing mechanic (see section 5).

## 2. What is known

Sources: the owner's repo (`GreediestFox/LIF-PROJECT`) and the LiFx Plus repo (`LiF-x/Plus`, files `docs/effects_and_abilities.md` and `ghdocs/docs/effects.md`). Read those two Plus files yourself; they are short.

**Effects are data-driven.**
- `data/cm_effects.xml` defines effects. Each has an `id`, a `name`, `flags` (CanMove, CanRun, CanJump, CanRotate, CanRotateHead, CanOperate, CanBeCanceled, RemovedOnDeath), a `description` (a string id), one or more `parameter type=... applytype=...` entries, and an `icon`.
- `data/cm_messages.xml` holds the tooltip strings the descriptions point to.
- `data/item_effects.xml` links items to effects. The Plus docs describe it as an equipment buff table. It is not confirmed that it also covers consumables.
- New effect ids can be added in data only. Plus says ids 1 to 91 and 93 are used, so 100 and up is free. Examples from Plus: 45 Invulnerable, 46 Criminal, 47 Resurrected, 66 Barefooted, 90 Luck: God's Love.
- Parameter types and apply types are fixed in the server exe. A new one is silently ignored. Known tokens include: ATTACK_SPEED, BAREFOOTED_SPEED, BIND_TO_EFFECT_LIFETIME, CAST_DURATION, CONSUME, DECREASE, DECREASE_COEFF, DEFENCE, DRINK_IMMUNE, HARD_HP, HARD_HP_MAX, HARD_HP_REGEN_SPEED, HARD_STAMINA, HARD_STAMINA_REGEN_SPEED, INCREASE, INCREASE_COEFF, MULTIPLY, POISON_IMMUNE, RANGED_ATTACK_SPEED, REGENERATE, SKILL, SKILL_GROW_BONUS_MULT, SOFT_HP, SPEED, TITLE (plus the stat names such as LUCK).
- Server scripts can only clear or extend an effect that is already active on a player (`lifxClearEffect`, `lifxExtendEffect`). There is no known call that applies an effect from script, and the engine code that applies effects at runtime has not been located.

**Existing potions (base game).** Cocktails are base-game objects: 779 Adrenaline, 780 Refreshing, 781 Double Blood (Antidote), 782 Aquila Wings, 783 Bull's Strength, 784 Swift Limbs, 785 Iron Will, 786 Swift Mind, 787 Toughness. Poisons are 940 to 950. Each cocktail's outcome is one effect, for example Swift Limbs = Faster = raises Agility. The cocktail-to-effect link lives in the base game's data or database, which is not in either repo.

**Owner's repo layout.**
- `server/mods/LiFx/<Pack>/mod.cs` are the mods. They register `dbChanges` via `LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, ...)` and use `dbi.Update("INSERT IGNORE INTO ...")`.
- `server/db/mod_rows_from_dump.sql` holds rows that also live in the seed dump `art/dump.sql` (not in the repo).
- `client/data-additions/*.xml` are rows to append to the client's data files (`objects_types`, `recipe`, `recipe_requirement`).
- `docs/id-ranges.md` lists used id ranges. Custom objects 3100 to 3913, recipes 5500 to 5959. Keep new ids out of used ranges.
- `server/mods/LiFx/FoodDrinksPack/mod.cs` is **dormant** (every line is commented out). It already defines White Beer 3186, Oat Stout 3187 and Red Ale 3189 as "Food (4 ingredients)" items with recipes 5952 to 5959 and no effects. The header says it is enabled by running `enable_fooddrinks.ps1` from the owner's archive, which also bakes `dump.sql`.
- `tools/pack-merge/*.ps1` and `docs/reports/*Translation*.csv` show how objects are renumbered and translated. German names matter: add the new items to the translation reports the same way the existing ones are listed.

## 3. Step 0: discovery (do this before editing anything)

Ask the owner for the server install folder, or use the connected folders if you already have them. Then find and read, without changing:

1. `cm_effects.xml`, `cm_messages.xml`, `item_effects.xml`.
2. How a consumable (a cocktail, a food or a poison item) is linked to its effect when it is used. Search the server `data` folder and `art/dump.sql` for object ids 779 to 787 and 940 to 950 and for the name "Swift Limbs". Look at the `objects_types` columns of those rows, any food or effect table in the dump, and any XML that mentions them.
3. Whether one consumable can carry two effects, and whether strength or duration can be set per item.
   Also find what marks an item as a drink for the blur and "Full" mechanic (see section 5).
4. Which existing White Beer, Oat Stout and Red Ale rows (3186, 3187, 3189) the owner wants to reuse.

Write what you found into a short note and show it to the owner before step 1. If the consumable-to-effect link is not found in data, stop and say so. Do not invent a table layout or insert guessed columns into the database.

## 4. Step 1: implementation (drinks only)

Work on a new git branch. Back up every data file you change first. Never edit `art/dump.sql` in place; add rows through a `mod.cs` `dbChanges` with `INSERT IGNORE`, the way the existing packs do.

**Plan A: the link allows two effects per consumable.**
Each drink uses its existing buff effect and existing drawback effect from the table in section 1.

**Plan B: the link allows one effect per consumable, or the clashing effects are a problem.**
Create one combined custom effect per drink, ids from 100 up, each with a buff `parameter` and a drawback `parameter` built from the tokens above (for example SPEED up and ATTACK_SPEED down for Dünnbier). Add the description strings to `cm_messages.xml`. This also avoids one drink's Accelerated fighting another drink's Slowed. Copy the closest existing effect as the template and keep its flags. Add icons only if the owner provides them; otherwise reuse existing icon paths.

In both plans:
- Create the six items in the dormant `FoodDrinksPack` style, with new ids in a free range (check `docs/id-ranges.md`) or by reusing 3186, 3187 and 3189 where the owner agrees. Add recipes and requirements for any new item.
- Rewrite descriptions where editable. The poison effects say "You have been poisoned" and a beer must not say that.
- Add client rows to `client/data-additions/*.xml` for new items so the client shows names and icons.
- Update `docs/id-ranges.md` and the translation reports.

## 5. Step 2: plug the drinks into the existing drunk mechanic

Confirmed by the owner: each drink adds blur, and the third drink applies "Full" (the effect that says you can not drink any more potions; it probably uses the DRINK_IMMUNE parameter). Both the blur and the cap come from the engine, so the data work is only to make the six drinks count as drinks in that mechanic.

1. **Find what marks an item as a drink.** Look at how Swift Limbs Cocktail (784), Refreshing Cocktail (780) and plain water or similar drinkables are defined: their `objects_types` rows (parent id, flags, description), any food or drink table in the dump, and anything in the data XML that mentions them. Whatever makes those count toward blur and "Full" is what the new drinks need. Reuse the same parent id and flags; do not invent them.
2. **Check what the cocktails and the new items share.** The existing dormant items (White Beer 3186, Oat Stout 3187, Red Ale 3189) are described as "Food (4 ingredients)". Check whether that makes them eat-type items instead of drink-type items. If so, the new drinks must copy the drink-type definition, not the food one.
3. **Test the existing behavior first.** On a dev server, drink one cocktail at a time and note: how much blur each adds, how long it lasts, whether "Full" appears on the third drink, how long "Full" lasts, and whether those numbers are tuned per item or are global.
4. **Then test a new drink.** It should add blur and count toward "Full" exactly like a cocktail. If it does not, the item is not marked as a drink; go back to step 1.
5. **Per-drink strength.** If the blur per drink or its length can be set per item in data, use the tiers from section 1 (Dünnbier light, Starkbier and strong spirits heavy). If it is global, say so and keep all six the same.

Things that no longer apply: a custom hangover effect, a counter, chaining effects, and any claim that blur is impossible. Do not add them.

Open points to confirm with the owner:
- Should the buff and drawback of a drink apply together with the blur, and should they stay after "Full" ends, or end earlier?
- Should an Antidote (Double Blood) cocktail clear the blur? (Earlier recommendation: no.)
- Does the "Full" cap also block normal cocktails after three alcoholic drinks? That is probably intended, but ask.

## 6. Test checklist (dev server only)

- [ ] Server boots, and the parser log shows no error between "parser entry" and "parser exit"
- [ ] Each of the six drinks can be crafted and drunk, and applies its buff and drawback with the right icon and text
- [ ] Accelerated and Slowed on one player: which one wins
- [ ] Two attribute buffs from different drinks: do they stack
- [ ] Each new drink adds blur like a cocktail, and the third drink applies "Full"
- [ ] "Full" ends after its normal time and the blur fades
- [ ] Does an Antidote cocktail clear the blur
- [ ] Effects survive relog or death as intended
- [ ] The existing cocktails and poisons still work

## 7. Rules

- Work on a branch and show the owner a diff before merging anything.
- Back up data files before editing. Do not delete anything.
- Do not guess table layouts, ids or column meanings. Copy from existing rows, or stop and ask.
- Restart the dev server after data changes and report what the log says.
- Tell the owner plainly what could not be done.
