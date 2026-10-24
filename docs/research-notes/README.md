# Research notes

Condensed write-ups of the reverse engineering and porting work, one topic per file. Each has the same shape: the idea (what the server owner wanted), what was done or tried, what ended up in the server, and the technical notes that are worth keeping.

| File | Topic |
|---|---|
| [01-server-database-and-boot.md](01-server-database-and-boot.md) | DB seeding order, foreign-key trap, Steam move/verify, world wipe |
| [02-datablock-range-movables.md](02-datablock-range-movables.md) | Datablock id limit and movable objects |
| [03-crops-and-wild-gathering.md](03-crops-and-wild-gathering.md) | Extra crops, gatherables |
| [04-workshops-and-crafting.md](04-workshops-and-crafting.md) | Workshops, quality buff, herb-garden time gate |
| [05-carts-wells-and-aliases.md](05-carts-wells-and-aliases.md) | Cart capacity, well water, stable/greenhouse aliases |
| [06-client-fixes-and-performance.md](06-client-fixes-and-performance.md) | FPS, prefs, seasons, text-edit crash |
| [07-art-porting-and-tools.md](07-art-porting-and-tools.md) | Models, textures, DDS, DTS tools, skins, icons |
| [08-content-ports.md](08-content-ports.md) | Content packs and the decisions made about them |
| [09-gm-tools-chat-and-sounds.md](09-gm-tools-chat-and-sounds.md) | GM tools, chat colours, sounds |
| [10-movement-and-ability-research.md](10-movement-and-ability-research.md) | Checksum/plough research, light activation, RE methodology |
| [11-beverages-and-drink-effects.md](11-beverages-and-drink-effects.md) | Drinks with buff and drawback, drink effect list, the Plus hook |
| [12-effects-and-outfit-tiers.md](12-effects-and-outfit-tiers.md) | Free effect slots, MMO effect ports, Geselle/Meister outfits, craft effect id limit |
| [13-daniels-mods.md](13-daniels-mods.md) | Daniel's mods: agriculture, food spoilage, tree felling, client extras, demo quest |
| [14-equipment-slot-17-fur-collars.md](14-equipment-slot-17-fur-collars.md) | Equipment slot 17 for cosmetic fur collars (client exe patch) |
| [15-engine-reference-player-stance-stamina.md](15-engine-reference-player-stance-stamina.md) | Engine reference: stance, object states, effects, soft stamina, movement, action animations |
| [16-tunnel-supports.md](16-tunnel-supports.md) | Permanent mine supports (tunnel timber decay) and the "will not collapse" inspect text |
| [17-alchemy-fused-into-healing.md](17-alchemy-fused-into-healing.md) | Alchemy merged into Healing: abilities, skill tab, effects, descriptions |
| [18-books-registry-and-tradepost.md](18-books-registry-and-tradepost.md) | Where item names live, the FeudalTools Tradepost (install, how it moves items), book key design |
| [19-animal-fodder-and-quality.md](19-animal-fodder-and-quality.md) | Animal fodder: +2 quality for newborns, no quality gain from ordinary feed, slaughter bonus in a month window |
| [20-rabbit-cage-as-coop.md](20-rabbit-cage-as-coop.md) | Rabbit Cage as a real Coop: the compiled-in building-type gates (client + server), three client exe patches |
| [21-books-write-flow-craft-tab-salt-barrel.md](21-books-write-flow-craft-tab-salt-barrel.md) | Books in game: right-click Schreiben/Lesen, ticket write flow, craft tab for a tool, building recipes vs the recipe manager, Salt Barrel, datablock id race |

Offsets (RVAs) refer to one specific Steam build of the YO dedicated server and client and must be re-verified after any game update.
- [22 Recipe update 2, comment-outs, icon size standard](22-recipe-update2-comment-outs-icons.md)
- [23 Running the server on a Windows VPS](23-running-on-a-windows-vps.md)
- [24 Skriptorium on Cloudflare, icon variants](24-skriptorium-cloudflare-icon-variants.md)
- [25 VPS operation: auto-start, Steam AppID, GM login, GM spawn lists](25-vps-operation-gm-lists.md)
- [26 VIP slots](26-vip-slots.md)
- [27 AmIAlive watchdog and QuickJoin button](27-amialive-quickjoin.md)
