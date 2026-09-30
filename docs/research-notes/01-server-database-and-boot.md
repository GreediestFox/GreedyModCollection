# 01 - Server database seeding, boot order and upkeep

## Idea (GreedyFox)
Run a modded LiF:YO dedicated server on a private machine: keep all custom objects and recipes alive across restarts, move the server to another drive, and start a fresh world without losing any modification.

## What was done / tried
* Custom object types inserted by hand with SQL disappeared at the next restart.
* Custom building rows referenced by world data made the server refuse to boot.
* A Steam "move" of the server folder validated the install and silently restored modded depot files.
* The world was wiped and recreated on request after an unexplained client FPS problem (the real cause was a client setting, see note 06).

## Output taken into the server
* Every new object type is registered in a mod pack's `dbChanges()` or baked into `art/dump.sql`.
* 65 mod-only object types are baked into `art/dump.sql` (separate `INSERT IGNORE` block after the Jorvik rows) so the boot foreign-key check passes.
* The start script (`Start_LiF_Server.bat`) clears stale construction-phase links of buildings that are already complete before starting the server.
* Depot files that mods edit were rebuilt by three-way merge after the Steam restore and are backed up; the rebuilt server booted clean (0 invalid foreign keys).

## Technical notes (what to know when doing the same)
* **Seeding order at every boot:** the server wipes `objects_types` / `recipe` (etc.), reseeds them from `art/dump.sql` (copied over `sql/dump.sql`, so edit `art/dump.sql`), then runs `sp_checkForeignKeys`, and only then runs each mod's `dbChanges()` callbacks. A row that is already in the dump wins over a mod's `INSERT IGNORE` of the same id, so once a type is baked, edit the dump, not the mod.
* `recipe_requirement` rows in the dump use NULL ids and the AUTO_INCREMENT counter is not reset between boots: custom requirement ids renumber upward at every boot. Any `unmovable_objects_requirements` row that points at a custom requirement (a custom building still under construction) will dangle after the next restart and abort the boot. Finish or delete such constructions before restarting. Check with a `LEFT JOIN` of `unmovable_objects_requirements` against `recipe_requirement`.
* A leftover test item or world row that references a removed type also trips the FK check.
* PowerShell `Set-Content -Encoding UTF8` adds a BOM, which breaks SQL/XML files; write UTF-8 without BOM.
* `mysql -B` doubles backslashes in output, and `mysql --execute="source ..."` splits at spaces; pipe a file into `mysql` instead.
* Some game XML files declare `encoding="utf8"`, which .NET `XmlDocument.Load` rejects (use `LoadXml` on the text); Python's strict XML parser rejects one existing token in the client XMLs, so validate with .NET.
* Server start: always via the start script (it clears the Steam AppID environment variables); never via Steam's Play button.
* **Steam move/verify restores stock versions of every modded depot file** (`data/skill_types.xml`, `cm_objects.xml`, `cm_messages.xml`, `cm_equipTypes.xml`, `item_effects.xml`, `cm_spawn_patterns.xml`, the character `male.dts`/`female.dts`, some `.cs.dso`). Files outside the depot (mods, config, dump.sql, DLLs) survive. After any Steam operation diff those files against backups before booting. The server's `skill_types.xml` genuinely differs from the client's (shorter durations): never copy one over the other.
* Restoring a wiped world: take a full `mysqldump --routines --triggers --events` first; dropping the database and restarting rebuilds it from `sql/new.sql` + patch + `art/dump.sql` + mods. GM accounts are matched by account id, so re-creating the same Steam account as id 2 keeps the GM whitelist valid.
* The Windows feature *Smart App Control* blocks any newly built unsigned hook DLL ("Bad Image" 0xc0e90002) and the server then boots without the hooks. It has to be switched off by the machine owner (self-signed certificates do not help).
