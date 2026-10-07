# 15 - Engine reference: player stance, object states, effects, soft stamina, movement, action animations

## Idea (GreedyFox)
A climbing mechanic (auto-climb on 46-75° slopes, as in LiF MMO) is to be ported. The reverse engineering for it produced a set of engine facts that are useful well beyond climbing, so they are collected here as a reference.

## What was done / tried
* The facts come from the reverse engineering of the climbing mod (Ghidra headless, x64dbg, live logs). Most are confirmed in game.
* Approaches that failed are listed under "Pitfalls", so they are not repeated.

## Output taken into the server
None yet: reference only. The climbing port is pending.

## Technical notes
RVAs refer to server `ddctd_cm_yo_server.exe` (SHA-256 `acd99bd1…`, build yo_1.4.4.5) and the **stock** client `yo_cm_client.exe` (SHA-256 `1d3f7fd3…`). S = server, C = client.

### Character parameters and stance
| | Server | Client |
|---|---|---|
| Character parameters (cp) | player + 0xAA8 | player + 0x12B8 |
| Stance (0 normal, 1 swimming, 2, 3; 4 = new climb stance) | cp + 0x550 | cp + 0x568 |
| Combat flag (combat is NOT a stance value, it is a flag at stance 0) | cp + 0x50C | cp + 0x524 |
| Stance → pose mapper | 0x94110 | 0x14CB70 |
| Stance setter | 0x93610 | 0x14C140 |

### Object state: how the engine gives effects for the duration of a stance
* Chain on the server: `setStance 0x93610` → cp vtable +0x190 (`0x96F80`) → `0x94020` (stance → object state; return site 0x96F97) → `ApplyState 0x4DF9B0` (only caller 0x9702D) → `0x4DF900`. The last step removes the effects of the old state, then sets those of the new one.
* State table 0x140AE72E8, stride 0x38, up to 4 × (effect id, float). Swimming = state 10 = {Limited Movement, Disabled}; carrying = state 12 = {Disabled}; climbing (stance 4) is unknown to the engine → state 0.
* To give a new stance an effect set: detour 0x94020, and only for calls from 0x96F97 with stance 4 return an existing state (e.g. 12). The engine then adds and removes the effect itself, and the effect replicates to the client HUD without client code.

### Effects
* Definitions: 94 × 0x40 bytes (S 0x140BE1510, C 0x14170DA90), CanOperate at +9.
* Active effects: 0x18-byte entries (+0 expiry, 0xFFFFFFFF = unlimited) in the container at cp + 0x730; the array is at container + 0x60 (S) / + 0x90 (C).
* "Operating blocked?": S 0x4DE1C0, C 0x935370, `u64 f(container, actionId)`. The exception list (10 ids) is at S 0x89CBC0 / C 0x112A0B8.

### Soft stamina (fixed point 1e6, 100 units = 1e8)
* Server: soft = [cp+0x288] − [cp+0x268] + [cp+0x270]; hard is in the same object (+0x188/+0x190/+0x1A8). Client: all offsets +0x20.
* `CharacterVitalParameters::Process_tick` = S 0x97BC0. Per tick `rate = vfn+0x90` (units/s); `delta = rate·dt·1e6`.
* The rate function is S 0x94F60 / C 0x14D8D0 (13-byte prolog `48 89 5C 24 08 57 48 83 EC 40 48 8B D9`); base regeneration is 25/s.
* To make an activity drain stamina: hook the rate function and return a negative rate. Clamping, broadcast and the exhausted logic then stay native.
* **The client simulates stamina itself.** Hook both sides, otherwise the HUD and checksums diverge.

### Movement
| | Server | Client |
|---|---|---|
| mDataBlock / contacted / contactNormal.z | +0x2040 / +0x2158 / +0x2170 | +0x4288 / +0x4320 / +0x4338 |
| PlayerData runSurfaceAngle / jumpSurfaceAngle / runSurfaceCos | +0x36C / +0x3A0 / +0x8430 | +0x400 / +0x434 / +0x86A0 |
| `findContact` *run compare (`comiss …,[rax+runSurfaceCos]`; `seta al`) | ~0xF6D96 | ~0x203E16 |
| `Player::updateMove` (12-byte prolog `48 8B C4 55 56 57 41 54 41 55 41 56`) | 0x106040 | 0x213BC0 |
| Target ground speed (cp vtable slot) | +0x118 → 0x9E310 | +0x110 → 0x154EE0 |

* Forcing `*run = 1` in findContact makes a steeper slope walkable for one player, without touching the datablock (a datablock swap re-initialises all animation threads and resyncs).
* Move axes: Move+0x18 = x (−1 left, +1 right), Move+0x1C = y (> 0 forward).
* Ground speed is **not** mass-based. Target speed = `max(|x|·side[state], |y|·forward[state]) · aux · cp[+0x590] · cp[+0x598]`. Mass only enters air/vertical movement.
  * cp vtables: S 0x738A80/0x75CC78/0x7E2F58, C 0xEA6998/0xEE15A8/0xF7AF78.

### Action animations
* `Player::setActionThread(this, pick, forward, hold, wait, forceSet)`: S 0x100F80, C 0x20E6A0. pick+0x08 = action index; forceSet = 1 is safe every tick.
* `pickActionAnimation` (C 0x20A6F0) re-picks every tick with forceSet = 0, so a one-shot action is overwritten next tick. The working approach is to override pick+8 inside setActionThread.
* The action index is **not** networked: every client derives other players' animations from velocity plus the replicated stance (4-bit field).
* Native action table: entries of 0x18 bytes (name pointer, direction x/y/z, speed), 541 entries, S 0xAC7BD0 / C 0x15338A0. It is filled by a static initializer; names are resolved in `PlayerData::preload`.

### Pitfalls
* Action 12 (`Climb`) has corrupt keyframes. Re-triggering it during a control-object resync freezes the client (TSShape "Target keyframe num −2147483648" flood). Slots 13/14 (`idle2`/`Idle3`) are unused and can be renamed to new sequences.
* `ApplyVitalDelta` (S 0x934D0) needs the vital object itself, which has a vtable. Passing a field pointer (cp+0xE0) crashes at 0x935B9, and the function changes hard stamina only.
* Writing the stamina fields directly does nothing visible: the next `Process_tick` refills them.
* The MMO climb sequences `Climbing_forward`, `_left` and `_right` exist only in the MMO `male/female.dts`, not in the YO models.
