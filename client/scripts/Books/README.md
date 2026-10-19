# Books client mod (GMC)

`books.cs` is exec'd at the end of the client `art/materials.cs` (patcher-managed pack). It registers the tooltip for the document types, the Ctrl+P read/write shortcut, the right-click "Schreiben"/"Lesen" entry, the automatic write flow and the extra craft-window tab for the Metal Scissors. See `docs/research-notes/21-...`.

Data changes that go with it (client, mirrored on the server where noted):
- `gui/scripts/tooltipManager.cs`: `addTooltipTemplate("tooltipInventory", 3960|3961|3962, "booksTooltip");`
- `skill_types.xml` (client `data`, client `server/data`, dedicated server `data`): ability 336 "Open Gift" -> `object_type_id 3960 3961 3962`; ability 217 "Craft" -> `471 1477 3047`.
- `data/loc/de/data/skill_types_ability_name.xml`: id 336 = "Schreiben".
- `data/objects_types.xml`, `recipe.xml`, `recipe_requirement.xml`: rows for 3960-3962, 3984/3993 and recipes 6600-6602 (client lists are separate from the server DB: without them the recipe does not show).
- Icons: `art/2D/Items/yo_scissors.png` + `yo_scissors_i.png` (own drawings; the tab shows the inactive copy when the tool is not in the inventory).