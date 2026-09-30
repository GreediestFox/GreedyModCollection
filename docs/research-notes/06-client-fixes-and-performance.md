# 06 - Client fixes and performance findings

## Idea (GreedyFox)
* Find out why the client frame rate dropped to ~30 in some areas and why texture loading looked doubled after spawning.
* Fix a client crash when typing into a text box of the GM panel.
* Start the client maximized at 2560x1440, Ultra, without V-Sync, auto quality and colour correction.

## What was done / tried
* Suspected mod textures, character meshes and world objects first; deleted buildings and even wiped the world. None of it helped.
* Read the client's `prefs.cs` and logs, then a crash dump from `%LOCALAPPDATA%\CrashDumps`.
* Halved 18 oversize mod textures on the client (kept originals).

## Output taken into the server / client
* The FPS problem was the client option **Auto Adjust Quality**: it had pushed the tree LOD to 9.58 and object LOD to 4.11, and V-Sync at 60 Hz clamped the result to exactly 30. Switched off, FPS returned to normal (about 96 at 2560x1440 Ultra).
* The real reason the settings never stuck: a stale compiled `data/prefs.cs.dso` (older than the `.cs`) is preferred by the client over the `.cs` beside it. It was renamed; the prefs were then applied with the client closed.
* The double texture loading is a **season feature**: the client starts with the Spring set and reloads every texture when the server reports another season (logged as "Changing season to"). It is not caused by mods (two experiments ruled out the custom skin materials and the horn-helmet entries). Left as is.
* The GM-panel crash was an engine bug (below) fixed with a 2-byte client patch: `tools/patch_client_textedit_cut.py`.

## Technical notes
* `data/prefs.cs` is rewritten when the client exits: edit it only while the client is closed. After editing any `.cs` that has a `.cs.dso` sibling, disable the `.dso`.
* Check first for any FPS complaint: `$pref::TS::autoAdjustQuality`, `detailAdjust`, `detailAdjustForest` versus the preset files in `gui/scripts/preset_*.cs`, and the V-Sync flag.
* Seasons come from the in-game date; with `dayCycle` = 3 real hours per game day the meteorological seasons rotate about every 11 days of real time (Summer = game June-August). Every Spring session registers at most ~218 repeated textures, every non-Spring session ~2,650 (a full reload, 26-41 s).
* About 128 mod textures have no mip chain (shimmer at a distance); 18 DXT1 textures above 2048 px were halved (the six with mips lose only the top mip; the twelve without mips were re-encoded with a full chain). Direct3D in this client needs DXT-compressed DDS files with mip chains; uncompressed DDS renders flat grey.
* **Crash root cause (`StringBuffer::cut`):** the Windows error said heap corruption in `ntdll`, but that was the game's own crash reporter; the original exception was an access violation read at client RVA 0x60A718 in `StringBuffer::cut(start, len)` called from `GuiTextEditCtrl::handleCharInput`. On a keystroke with a selection it cuts `mBlockStart..mBlockEnd` without a bounds check, and the copy loop compares `start < size - len` as unsigned, so a selection longer than the text (left over after `setText`) wraps and reads until an unmapped page. Patch: `jae` -> `jge` at RVA 0x60A705 and `jb` -> `jl` at 0x60A72E (the function's tail already sets size = 0 when len >= size).
* **Reading a client crash dump:** `tools/minidump_stack.py` parses the dump (modules, exception, stack scan, embedded EXCEPTION_RECORDs); the top-level exception is often the crash reporter, so look for the embedded record. `exe_strings_near_rva.py` names a function by the strings its code references; `x64dis.py` disassembles. Heap memory is not in these dumps.
* On the server an `AuthTicketCanceled` / `CR_STEAM_AUTH_REJECTED` log line only means the client process died (Steam cancelled the ticket); it is not a kick.
* A server started without the Plus DLL hooks (wrong start method) dies with "invalid substance ter2ID" / "Can't load skills/abilities" because the custom crop substances are unknown to it.
* Patched client exes are overwritten by Steam updates and file verification; re-run the patchers afterwards.
