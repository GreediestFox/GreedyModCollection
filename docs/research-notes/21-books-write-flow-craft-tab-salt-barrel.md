# 21 Books in game: right-click, automatic write flow, scissors craft tab, Salt Barrel

Follow-up to note 18. The book registry now works end to end (letters 3960, books 3961, stone tablets 3962; key = item Durability, table `book_registry`). This note records what was needed on top of it, and the engine facts learned on the way.

## Right-click "Schreiben" / "Lesen"

- The game cannot take new abilities, so ability **336 "Open Gift"** is repurposed: `ent_req object_type_id 3960 3961 3962`, German name "Schreiben". Server and client `skill_types.xml` and the client loc string change.
- **A repurposed ability must live in a skill the character has a row for** (table `skills`), otherwise the menu never offers it. Abilities 42 (Bowcraft), 271 (Archer) failed for exactly that reason.
- `client/scripts/Books/books.cs` wraps the menu button builders (`createSingle/Default/addInGroupAbilityWidgetButton`) in a package: it renames the entry to "Lesen" for registered documents and lays a `GuiMouseEventCtrl` over it so the real ability never runs. The default entry's text has a leading space (`trim()` before comparing).
- The document key is learned from the item tooltip, which appears after `hovertime` (500 ms). A quick right-click found no key, so `GuiInventoryItem::onMouseEnter` sets `hovertime = 30` for the three document icons, and the key is held 0.6 s after the pointer leaves.

## Automatic write flow (ticket)

"Schreiben" on a blank item opens the website (`/letter.html?new=1&ticket=<random>`). The page stores the ticket with the new document; `GET /api/ticket/<ticket>` returns `<code>\n<title>` once it is finalised (`server/mods/LiFx/BooksPack/website/ticket.js`). The client polls it with `SimDownloadHelper` (no HTTP object in this engine) and sends `BookApply(code, title)`; the server replaces one blank item of that type in the player's inventory with the registered one (`LiFxBooksPack::register`). Stone tablets use the admin page and are limited to GMs who are logged in to the GM panel; the key is only sent to them.

Registry keys start at 10000 (`AUTO_INCREMENT`) because freshly crafted blanks carry a default durability up to 5000.

## Craft window tab for a tool (Metal Scissors)

- The tabs of the inventory craft window are created by the engine for a fixed set of tools (`GuiCraftWindows::addInventoryToolButton(typeId, image, active, selected)`; inactive tabs use `<icon>_i.png`). A tab for another tool has to be appended by script (package override, after the last native tab), and the click command must be prefixed to remember the tool.
- The recipes of a tab are found through a real craft ability: scissors (3047) had to be added to ability 217 "Craft" (types `471 1477 3047`). The list of that ability then also contains the plant-fibre recipes, so `addRecipeListItem` is filtered: recipe 6600 only on the scissors tab, nothing else on it.
- Tools are never shown as slots in the craft window (vanilla behaviour); put them in the recipe description.

## Building recipes must exist before the recipe manager reads the table

`CmRecipesManager` loads the recipe table in `prepareManagers` about two seconds after the boot starts. Statements queued by mods whose `dbChanges` run later (a mod with a very large batch such as ZZ_RecipeRevisionPack) are not in it. Crafting does not care, **building/placement** (`AbilityBaseBuildingImp::CouldPerform`) logs "recipe not found" and plays the error sound. Put such recipes in an early mod. Also: never put your own recipe ids inside a range another mod deletes (ZZ deletes 6510-6599).

## Opening a fixed container (engine facts, not changed)

`AbilityBaseOpenImp::openIt` (server RVA ~0x386300, compare block at 0x3863EC) only opens an unmovable object's inventory if it is an incomplete site, a device, type 131/132/516, a windmill (137/1093) or a descendant of 1397 (Ruins). Devices get no "Look in Inventory" entry; they open through a type specific "Use X" ability. The client opens non-device containers with a generic window. A movable container (like the Barrel 106) avoids all of this.

## Salt Barrel (Salzfass)

Movable container exactly like the Barrel: object 3984 (parent 63, `IsContainer=1 IsMovable=1`), carried twin 3993 (parent 1902), `objects_conversions (3984,3993)`, craft recipe 6602 (Saw, Carpentry 30), shape = the game's own `crate_quest.dts`. `FoodSpoilagePack` pauses spoilage only for containers of type 3984 (the normal Barrel no longer protects).

Two traps found:
- Changing a type from unmovable to movable while built instances exist crashes the boot (`ComplexObject::_makeFromStaticData() - inconsistant object type`, "Fatal: Can't init terrain list"). Delete the `unmovable_objects` rows (and requirements, claims, `working_containers`, root container) with the server stopped.
- **Client and server must create the same movable ImageData ids.** `FreeMovableSlots` turns 186 dev types non-movable with a late `dbChanges` UPDATE; on about 40 % of the boots it landed after the server had built its list (368 instead of 361 `ShapeBaseImageData` lines in the log, types 1544-1550 present), every later id was shifted by seven and carrying any later type made the client disconnect with "Invalid packet (mounted images)". The UPDATE is now also baked at the end of `art/dump.sql`, which runs before everything else. Check the count in the server log after a boot.