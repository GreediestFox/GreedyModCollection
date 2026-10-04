# 14 - Equipment slot 17: a dedicated slot for cosmetic fur collars

## Idea
EKR's cosmetic fur pieces (the fur of the Viking helmets, worn alone) should get their own equipment slot instead of taking a ring slot.

## What was done / tried
* First try: slot 15. It looked free in `cm_equipTypes`, but slots 15 and 16 hold the invisible fists (Hand of Boris / Ilyas, items 1120/1122). The collar pushed a fist into the inventory, where the engine destroyed it. The character was repaired in the DB.
* Client (`isValidSlot`: `slot-1 < 0x11`) and server already accept slots 1..17. Slot 17 is unused, and only the DB rows were missing.
* A neck-slot variant (amulets moved to the ring slots) was offered as the cheap alternative; the real slot was chosen.

## Output taken into the server (confirmed in game)
* `FurCollarsPack`: items 3936-3938, Tailoring recipes 6488-6490. It re-creates `p_allocate_equipment_slots` with 17 slots and adds the slot-17 row for existing characters.
* Furs use `<slot>17</slot>` only.
* Client exe patch `tools/patch_client_equip_slot17.py` plus a `SlotFurPnl` panel in `equipmentWindow.gui`.

## Technical notes
* `CmEquipmentControl` (client):
  * Panel records: a 15-element array of 0x30 bytes at `this+0x310`, index = slot, followed by members from `0x5E0`.
  * State bytes at `0x618+slot`, 18 bytes, so 17 already fits.
  * Object size 0x640 is used in 4 places: two allocations, a sized delete, and the engine type info at RVA 0xF2609.
* The patch grows the object to 0x680 and places the slot-17 record at `this+0x640` (exactly `0x310 + 17*0x30`). It constructs and destroys that record in the ctor and both dtors.
* Setup (name table `SlotHead..SlotTabard`, loop of 14) gets an extra `SlotFur` pass. The name pointer is written to `this+0x670` at runtime, because the exe is ASLR, so no absolute addresses.
* The panel reset gets an extra pass; 4 slot loops jump 14 -> 17 and 10 `slot < 15` guards admit 17.
* The caves fill the unused tails of the earlier patch sections `.skin/.crop/.subal`; the section table has no room for another section header.
