# 18 Books registry and the FeudalTools Tradepost

## Idea

The server owner wanted books, letters and stone tablets for the book website mod (YOscriptorium, a successor to "Books for the People"), with the following properties:
- Every written item carries a key that survives storage, including the trading post.
- The game creates and names the item itself.
- Players cannot rename it.

The client should resolve the key to the title, link and icon. Work started 2026-10-10. This note holds the test results and the decisions so far; the implementation is still open.

## How the book mod works today (client only)

- **Naming:** a player names a **Pouch (461)** with the ability **"Name the Bag" (184)**. The ability is an `inventory_item` ability for types 458 Backpack, 459 Sack, 460 Bag and 461 Pouch. The name is the website tag (`_ltr…`, `_bok…`, `_tbl…`).
- **Display:** every second, the client mod walks all `GuiInventoryItem`s and reads the tag from `getText()`. It fetches the title from `<baseUrl>/api/title/<tag>`, caches it, and sets the text and icon.
- **Tooltip:** the mouse tooltip of the pouch is registered for type 461 in a patched `gui/scripts/tooltipManager.cs`.

## Where item and container names live (server DB)

- **Names are server-side.** The `items` table has no name column. Both `items.FeatureID` and `containers.FeatureID` point to `features.CustomtextID`, which points to `custom_texts.Custom_text` (varchar 255).
  - Containers are renamed by the proc `f_renameContainer(id, text)`; an empty text deletes the feature.
  - `f_insertNewItemInventory(..., inFeatureText, ...)` can create an item that already has a name.
  - The script function `Player::inventoryAddItem(type, qty, quality, durability, createdDurability)` has no name parameter.
- **A pouch is not an `items` row:** it is a `containers` row whose `ParentID` is the container holding it.
- **Ability 312 "Rename" is the wrong place to block renaming.** It covers placed objects and buildings (`complex_obj`), not inventory items.

**Test 2026-10-10:**
- A pouch named `_bok_test_1` could be renamed again to `_bok_test2`. The old `custom_texts` row was removed, so naming is NOT one-time only.
- The name survived a round trip through a chest.

## FeudalTools Tradepost (third-party mod, compiled scripts only)

The "trading post" players use is not vanilla. It is the FeudalTools Tradepost 1.6 together with its base mod TTmod 1.6:
- **Server:** the folders `TTmod/` and `Tradepost/` in the server root, plus `exec("TTmod/init.cs"); exec("Tradepost/init.cs");` at the end of `main.cs`.
- **Client:** the mod folder `tradepost/` (`cmod.cs.dso`, `tpclient.cs.dso`). The shipped `cmod.cs` expects a client LiFx framework; without it, a small loader can exec `tpclient.cs` directly (see `client/scripts/Tradepost/`).
- **Config:** `Tradepost/config.cs` ships with `$FTTP::ServerID = {SERVER_ID};`. That placeholder is a TorqueScript syntax error and must be replaced with the ID from the FeudalTools Server Manager.

**What it needs:**
- A FeudalTools account. The Tradeposts mod needs Premium.
- The server registered with its public IP and database credentials, with the database reachable from the internet.
- A dedicated DB user instead of root (see `server/db/feudaltools_grants.sql`).

**How it moves items** (read from the string tables of `transfer.cs.dso` and `updater.cs.dso`):
- The trade window is a web page on `tradepost.feudal.tools`, opened with `openWeb`. The page writes the actions into `nyu_tradepost_*` tables.
- The server polls `nyu_tradepost_transfers`:
  - `remove_item` deletes the item from the inventory (`inventoryRemoveItem`).
  - `storage_take` / `market_buy` create a NEW item with `inventoryAddItem(type, qty, quality, durability, createdDurability)`.
- Offers store only: ObjectTypeID, RegionID, Quality, Quantity, Durability, CreatedDurability, Effects and Blueprint.
- **The name and the item ID are lost.** The mod reads only the `items` table, so a pouch (a `containers` row) probably cannot be offered at all.
- On boot it scans for `trade_post.dts` models and registers a trigger radius of 10 around each one.

## Decision so far

- **The key goes into Durability / CreatedDurability**, not the name or the item ID:
  - Both values survive the tradepost.
  - The client can already read them: the stock tooltip asks for `items.Durability` / `items.CreatedDurability`.
- **The book items must not stack** (MaxStackSize 1), or two books would merge. Health Book 3029 is such a type; Book Paper 3028 stacks to 1000.
- **The written items become plain non-stacking item types instead of pouches.**
  - Plain items cannot be renamed by players at all, which solves the renaming problem.
  - Bag names stay possible for real bags.

Still to test in game:
- A book with durability 12345 through a chest, the ground, relog and the tradepost.
- Whether durability ever changes on an unused item.
