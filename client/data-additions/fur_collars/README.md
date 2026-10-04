# Fur collars and equipment slot 17

Cosmetic fur collars (from the EKR modpack, the fur parts of the Viking helmets) that are worn in their own equipment slot 17.

- Server: `server/mods/LiFx/FurCollarsPack` (items 3936-3938, recipes 6488-6490, slot-17 rows via `p_allocate_equipment_slots`).
- Server + client: `cm_equipTypes.blocks.xml`.
- Client: the `*.rows.xml` files, `equipmentWindow.gui.patch` (adds the `SlotFurPnl`/`SlotFurIcon` panel; rename `equipmentWindow.gui.dso` afterwards) and the exe patch `tools/patch_client_equip_slot17.py`.
- Icons are not included (art); the rows reference `art/2D/Items/*_fur_collar.png`.
