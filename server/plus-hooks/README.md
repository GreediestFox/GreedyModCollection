Reference copy of the drinkEffects hook source. The maintained version is in the Plus repository (branch `yo-engine-hooks`, file `source/server/hooks/engine/`).

hook_craft_effects_range.cpp/.h: raises the item_effects.xml effect id limit from 1..38 to 1..127 (config <craftEffectsRange enabled="1"/>), needed for the Geselle/Meister outfit effects (see docs/research-notes/12).

Hooks ported from Daniel's mods (2026-10-04), wired into Plus like the others, logging to logs/agriculture.log via agri_log.h:
- hook_plow_cart_parity: removes the plough exception from the cart tuning branch (server RVA 0x9E34D); needs the client twin (tools/patch_client_plow_parity.py).
- hook_harvest_soil: harvested tiles go back to settled soil (RVA 0x3A9759).
- hook_plow_area: plowing loosens a 3x3 area (RVA 0x3A5FD1).
- hook_log_description, hook_saw_output, hook_ability_entity_check: let extra log types be examined and sawn (RVAs 0x39633C, 0x39E761, 0x39EE12); the client saw menu needs tools/patch_client_saw_menu.py.

hook_tunnel_inspect.cpp/.h (2026-10-09): inspecting a tunnel tile with timber (boards/column) shows message 5213 "supported, will not collapse" instead of the day count (call site RVA 0x397AF5, config <tunnelInspect enabled="1" messageId="5213"/>). Wired like the others: ConfigureTunnelInspect(root) next to ConfigureLogDescription, AttachTunnelInspectHook() next to AttachLogDescriptionHook in cm_server.cpp. Message text: client/data-additions/tunnel_supports. Pairs with server/mods/LiFx/TunnelSupportsPack (timber decay 0), see docs/research-notes/16.
