# 10 - Movement/checksum research, light activation, RE methodology

## Idea (GreedyFox)
* Find out why ploughing (and to a lesser degree pulling carts) stutters and why the server logs "packetDataChecksum disagree!".
* Light torches and fires through an ability and build a dusk sweep that lights them automatically.

## What was done / tried
* A multi-week investigation of the client/server `Move` checksum, instrumented with breakpoint logs on both sides and correlated by `GetTickCount()` (valid because client and server ran on one machine).
* Light activation was traced to the ability class behind "Light the Fire" with observation-only hooks (`hook_register_perform`, `hook_light_working_object`, `hook_resolve_light_object`).

## Output taken into the server
* **Plough speed fixed:** the server special-cased the plough (object 53) to a flat 0.5. Moving its `ParentID` from 15 to 77 in the seed dump routes it through the same load/willpower speed formula as the wheelbarrow and carts (167/168/169); the "Lift Object" exclusion list (ability 96) must still list all seven members (53, 167-169 and the horse carts 1437/1461/1496/1497).
* **Stutter:** the mechanism was found (below) but the judder itself is not fully removed.
* **Light activation:** the state toggle of `AbilityImp::LightOn` was confirmed working in game; the automatic dusk sweep is still to be built.

## Technical notes
* The server prints one MISMATCH per packet, not per move, so raw counts undercount by the client's bundling factor. While ploughing, essentially every Move send cycle mismatches; the raw input is clean, so the divergence is in processing or in an unlogged field.
* **Mechanism:** the real checksum fields drift across integer boundaries before an identical `cvttss2si` truncation on both sides, so the two sides truncate different integers for nearly equal floats.
* Raising the checksum grace window (existing Plus hook, `graceDepth`) rescues only ~0.2% of mismatches. A handcart has a ten times lower collision rate than the plough for the same movement.
* False leads, all falsified by live tests: the plough-ability chain (`_checkPlowAbility`, ability 143 disabled with no change), `applyCorrection`, the catch-up backlog counter, the hard-coded plough compare, and the `mount0`/`mount1` node geometry (swapping the plough's shape for the cart's mesh changed nothing).
* **RE methodology that worked:** name functions from the diagnostic strings the binary embeds; use RTTI lookups in the live process to identify classes (for example `PlowAuto_Ability` exists only on the server); decompile with headless Ghidra scripts (decompile at RVA, callers-of, find scalar immediates, disassemble a range) on the **live** executable, not a decoy copy of another build; validate every hook's prologue bytes before detouring and skip with a log line on mismatch; hook in memory only.
* The ability factory (`AbilityManager` constructor, ids 0x40 UsePotterWheel, 0x13C QuestForge, 0x148 QuestCook, ...) registers each ability with `_registerAbility(this, id, new X_Ability)`; the registry stamps the id onto the object, which is why new ability ids can reuse an existing class.
* Observation hooks never touch return values or engine state, only log, so they are safe to leave in.
