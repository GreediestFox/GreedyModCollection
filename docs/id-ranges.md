# Id ranges for custom content

| Range | Use |
|---|---|
| 2359-2362, 2399 | animal carcass objects (custom tribal NPCs) |
| 2394-2398 | tribal NPC objects |
| 2461-2466 | custom weapon objects |
| 3000-3053 | objects merged from a second mod pack, renumbered to avoid collisions |
| 3920-3921 | beverage test items (Thin Beer, Strong Beer); effects 100-101; `effects` rows 41-42; messages 5170-5171 |
| 5500-5523, 5830-5835 | recipes for custom buildings and weapons |

When merging a pack, remap any ids that collide with used ids to open ones, then update every cross reference (recipes, requirements, skill_types.xml object lists, tool ids).