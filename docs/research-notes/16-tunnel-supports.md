# 16 - Permanent mine supports (tunnel timber)

## Idea (GreedyFox)
Mine supports (wall boards and support columns) should not rot away, and inspecting a supported tunnel should say so.

## What was done / tried
* First suspected the wooden platform pillars (`Pillars`, ids 1361-1366) and the per-object `noDecay="1"` flag in `cm_objects.xml`. Wrong objects: mine supports are not objects at all.
* Mine supports are **tunnel timber** inside the terrain data:
  * light timber = boards on the walls, ability 15 "Reinforce Tunnel"
  * heavy timber = columns, ability 16 "Construct Supporting Column"
  * both on the `tun_side` entity, placed by `AbilityImp::TimberingBaseImp`.
* No ability in the game removes timber; it only disappears through decay. A "destroy" option would need a new ability plus a native hook (not done).

## Output taken into the server
* **`TunnelSupportsPack`** sets `$cm_config::Geo::LightTimberDecayValue = 0` and `$cm_config::Geo::HeavyTimberDecayValue = 0`.
  * It does this at setup and again in `onServerCreatedCallbacks`, because `scripts/server/cm_config.cs` runs later and resets them to 1.
  * Unsupported tunnels still decay (`TunnelDecayValue` stays 1).
  * Verified in the server log: `GEO_DECAY heavy timber at [...] from 60 to 60`, while open trenches went 7 → 6.
* **Plus hook `hook_tunnel_inspect`** (`<tunnelInspect enabled="1" messageId="5213"/>`): inspecting a tunnel tile that has timber shows message 5213 "It is supported and will not collapse" instead of message 2555 with a day count. Message 5213 goes into the server and client `cm_messages.xml` (+ loc).

## Technical notes
* **Config variables** are engine-bound (registered in `FUN_140156940`, live). The exe strings read `Geo::TunnelDecayValue` etc., but the script names carry the `$cm_config::` prefix; `$Geo::…` is just an empty script variable.
  * Defaults: light timber resource 10, unsupported tunnel resource 8, all decay values 1 per decay iteration.
  * The decay iteration is the in-game morning clean-up.
* **`TerrainDeformer::_decayTimber`** (server 0x573AA0):
  * Reads the tunnel level qword: bits 0-15 altitude, byte 2 "not air", byte 3 flags (0x20 light timber, 0x40 heavy timber), bits 32-47 the decay resource.
  * If decay < resource, the resource is lowered. Otherwise the timber is removed (heavy turns into light with the default resource).
* **Inspect:**
  * `AbilityImp::Inspect::_inspectEntity` (0x397550) calls `InspectHelper::calculateTunnelDecay` (0x57CB50), which returns `resource / TunnelDecayValue + 1`, ignoring timber.
  * Level lookup: `FUN_1404AEDD0(column, &qword, altitude, int* index)`, read-only.
  * The hook patches +0x397AF5..+0x397B07 (`lea rcx` / `call` / store days / `mov edx,2555`) with a jump into a trampoline that calls a C++ helper and continues at +0x397B08.
* **Vanilla "Destroy"** (ability 203, `AbilityImp::DestroyObject`) handles stationary and movable objects. The client check `DestroyObject_Ability::checkEntity` requires a claim with destroy rights. It does not apply to tunnel timber.
