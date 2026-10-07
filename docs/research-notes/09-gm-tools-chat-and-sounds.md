# 09 - GM tools, client quality-of-life mods, chat colours, sounds

## Idea (GreedyFox)
A working GM panel (spawn items, teleport, commands) in the client, a quality-of-life mod window (SF Mods), colour-coded local chat depending on nearby players, and custom sounds for workshops and animals.

## What was done / tried
* A pre-built GM panel package was found to call an API that does not exist in this server; one real native database bridge was kept and everything else was rebuilt as client scripts (`client/scripts/GMHelper`) and server commands (`server/mods/LiFx/GMCommands`).
* SF Mods v1.20 (third-party client mod, not included here) was integrated using its original compiled release: a version rebuilt from decompiled source crashed the client during start-up in the first `exec`s and was never bisected.
* Local chat colours and sounds were added as client-only scripts/data.

## Output taken into the server
* GM login and whitelist by account id, `!give`/spawn panel with search, teleports and helper commands.
* A server bug fixed on the way: the `serverCmdLocalChatMessage` wrapper passed three arguments to an engine handler that takes two, which silently rejected all typed local chat.
* Sound profiles for the slave (Jorvik 3026), beehives and the Big Tanning Tub, and a wash loop for the Ore Washer.

## Technical notes
* The chat box cannot carry GM commands (own chat protocol), so the client scripts send them through console functions.
* **GM password, only from the server files:**
  * The admin writes it (6+ characters) into `mods/LiFx/GMCommands/gm_setpass.txt`. The server turns it into a salted hash in `gm_password.cs` and empties the txt.
  * Both files are read again at boot and before every `!login`, so a new password works without a restart. The in-game `!setpass` is disabled.
  * `gm_password.cs` is parsed line by line instead of `exec`'d. Avoid `strrchr` there: cutting the quoted value with two `strpos` calls is safe.
  * Keep `gm_password.cs` in server backups: a clean reinstall loses it, and then nobody can log in until a new password is set.
* **Chat colours:** the chat only honours a colour tag when the text starts with `=`, i.e. `=<color:RRGGBB>text`; without `=` the `<` is eaten. A package override of `clientCmdLocalChatMessage(sender, pos, text)` counts other Player objects in the client's server group within a radius and picks a colour per level.
* **Client sound mechanics:** audio is FMOD. Profiles are in `art/datablocks/audioProfiles.cs` (`SFXProfile`, filename, description); an object's sound is a `<soundEmitters><emitter><profile>` inside a state in `data/cm_objects.xml` (client and server copies kept in sync). `AudioObjectsCloseLoop3D` is the objects channel (reference distance 3 m, max 30 m). Mono 44.1 kHz **wav** works; ogg made with libsndfile and `SFXPlayList` did not play for object emitters. Random behaviour = one long pre-mixed wav with clips and silent gaps, looped. Edit from the original source file to avoid cumulative loss.
* **Ability sounds are client-driven:** the ability's `<results><sound>` comes from the *client's* `skill_types.xml`, so a per-building sound needs its own ability id on the client. The server's ability table (`AbilityManager` registration, id -> class) shows all 15 device-craft ids in use; unused quest abilities are free slots (note 04).
* The server has no sound profiles; `CmSoundsManager` is an empty stub.
* Third-party SF Mods start-up: its last line in `art/materials.cs` schedules `exec("mod/sf-mods/init.cs")`; its update check was replaced by an empty stub.
* Translation workflow used: a generated "to-do" sheet of untranslated strings grouped by workstation, then a checker for the translator's edits (it caught two swapped names and an overwritten vanilla line).
