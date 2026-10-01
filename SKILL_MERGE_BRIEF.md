# Brief: merge the Alchemy and Healing skills

You are working on a Life is Feudal: Your Own (LiF:YO) server with the LiFx mod framework. This brief is a handoff from an earlier chat. The earlier chat had no access to the real skill data, so this is a plan plus a discovery job, not a finished design. Read it fully, do step 0 first, and show the owner your findings before changing anything. This is a separate project from `BEVERAGE_IMPLEMENTATION_BRIEF.md`; do not mix the two.

## 1. Goal (the owner's words, tidied)

Merge the Alchemy skill and the Healing skill into one big skill. The merged skill keeps the same level spacing and unlocks (every ability and recipe unlock keeps its required level). The Alchemy skill, which sits last in the skill tree, becomes an empty placeholder.

Working assumption: **Healing is the surviving skill** and Alchemy is the placeholder. Confirm this with the owner, and ask whether the surviving skill should be renamed (for example "Healing & Alchemy").

## 2. What is known

From the owner's repo (`GreediestFox/LIF-PROJECT`, mod comments and `tools/pack-merge`):
- Skills, abilities and their requirements live in `data/skill_types.xml`. The server and the client each have a copy and the two must match. Each `<ability id="...">` block holds `ent_req` and `req` entries (object type ids, tool ids). Skills are referenced by numeric skill row id. Known examples from the mods: Herbalism 12, Brewing 13, Sculpt skill row 17, Construction 18, Masonry 19, Procuration 23, Tailoring 25. YO and the original MMO use the same skill ids. Healing and Alchemy ids are not recorded in the repo; look them up by name.
- Recipes carry a skill id and a skill level (columns used by the mods: `recipe.skill` and `SkillLvl`; for example recipes 5200 to 5265 use skill 12 at level 0). Check the real column names in the live schema before relying on them.
- Adding a **new ability id** to `skill_types.xml` crashed the server ("using of unregistered ability"). Abilities are compiled classes, one per id. So only existing abilities may be moved or edited, never new ones added.
- Editing existing ability blocks in `skill_types.xml` works (the mods extend object lists that way). The existing script `tools/pack-merge/merge_skill_types.ps1` is a good model: it edits both the client and the server copy, edits ability blocks by id with a regex, and throws if the expected old text is missing. `revert_skill_types.ps1` and `find_skill_refs.ps1` in the same folder are also useful.
- **Steam "verify integrity of game files" on the dedicated server silently reverts `data/skill_types.xml` (and `data/cm_objects.xml`) to vanilla.** The mods record a `.bak_pre_<name>` backup beside each change. Do the same, and tell the owner to keep the changed files outside Steam's verify.
- Player skill levels are stored per character in the database. The table and column names are not in the repo.

## 3. Step 0: discovery (read only)

Ask the owner for the server install and client install folders (or use connected folders). Then find and report:

1. The Healing and Alchemy skill ids and names, and where the skill names are stored for display (messages file, client text).
2. For each of the two skills, the full list of abilities, with the required skill level for each unlock, and any other requirement (tools, objects). Put them side by side by level.
3. Every recipe that uses Alchemy and every recipe that uses Healing, with their skill levels. Include the cocktails (779 to 787), poisons (940 to 950), preparations and bandages. Count them.
4. Where the skill tree layout is defined (order, position, icons), and whether the client reads it from `skill_types.xml` or from another file. Report what an empty skill would look like and whether the layout allows a skill row with no abilities.
5. The database tables and columns that store each character's skill levels and skill progress, and any other table that references a skill id.
6. Whether any mod, config (`lifxpluss.xml`) or script refers to either skill id.
7. Whether skill gain is data-driven (grow rates, caps) and whether it differs between the two skills.

Write the findings into a short note and show it to the owner. If something here is not data-driven, or the tree cannot show an empty skill, stop and say so.

## 4. Step 1: design (confirm with the owner before building)

Propose, using the real data from step 0:
- **Merged ladder.** List every unlock from both skills in one table by level. Where both skills unlock something at the same level, that is fine; just show it. Keep every required level unchanged unless the owner agrees to change it.
- **Progress speed.** One skill now rises from both healing and alchemy work, so it levels faster. Show the owner the grow rates and ask if anything should change. Do not change them without approval.
- **Existing characters.** For each character, propose how the two saved levels become one. The default suggestion is to take the higher of the two. Show how many characters have both skills and how many would gain or lose unlocks.
- **The placeholder.** The Alchemy row stays, with no abilities and no recipes, renamed to a placeholder name the owner picks. Do not delete the skill row: skill ids are referenced widely and removal is not known to be safe.
- **Drinks.** If the beverage project is also being built, its recipes must not use the Alchemy skill.

## 5. Step 2: implementation

Work on a new git branch. Back up every file before changing it (`.bak_pre_skillmerge`), both client and server copies. Rules:
- Move existing abilities between skills by editing existing blocks. Never add a new ability id.
- Use a script modeled on `merge_skill_types.ps1`: edit by ability id, fail loudly if the old text is not found, apply the same edit to the client and the server copy, and write a matching revert script.
- Re-point the Alchemy recipes to the surviving skill with the agreed levels. Put recipe and database changes in a mod `dbChanges` with `INSERT IGNORE` or `UPDATE` only if that is how the repo handles it for existing rows; otherwise write a reviewed SQL file and show it to the owner. Do not run anything against the live database yourself.
- Write the character-level merge as a SQL script that the owner runs on a dev copy first. Include a query that shows each affected character's levels before and after.
- Update `docs/` with what was changed and how to revert.

## 6. Test checklist (dev server and a dev client only)

- [ ] Server and client boot with no ability-parsing errors
- [ ] The skill tree shows the merged skill with all unlocks at the original levels
- [ ] The Alchemy placeholder shows (or is hidden) as expected and nothing crashes when it is opened
- [ ] Every Alchemy recipe is craftable at its level under the surviving skill
- [ ] Healing abilities still work, and the cocktails, poisons and preparations are craftable
- [ ] A test character with only Alchemy, only Healing, and both, ends up at the agreed merged level
- [ ] Skill gain from healing and from alchemy work both raise the merged skill
- [ ] After a Steam "verify integrity", the files are restored from the backups and the server still boots
- [ ] The revert script restores the original skill tree

## 7. Rules

- Work on a branch, back up first, show a diff and wait for approval before any change that touches live data or the database.
- Do not guess ids, table names or column names. Copy from the real files, or stop and ask.
- Do not add ability ids. Do not delete skill rows.
- Tell the owner plainly what could not be done.
