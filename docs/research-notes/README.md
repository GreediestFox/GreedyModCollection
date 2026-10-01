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

Offsets (RVAs) refer to one specific Steam build of the YO dedicated server and client and must be re-verified after any game update.
