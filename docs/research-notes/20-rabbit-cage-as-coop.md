# 20 - Rabbit Cage as a real Coop (own model, full husbandry window)

## Idea (GreedyFox)
The Rabbit Cage (object 2834, rabbit hutch model) should work exactly like a Coop: Manage window, breeding, feeding, dung, harvest, slaughter. Earlier attempts (2026-09-21/22) only got Manage to show nothing and fell back to "the recipe builds a vanilla Coop". This note is the real fix.

## Why it failed before: three hard-coded type gates (none is in XML)
The animal buildings are recognised by hard-coded lists of the vanilla type ids 133 (Small Stable), 134 (Large Stable), 143 (Barn), 144 (Coop), 517 (Wooden Barn). Adding the type to the `object_type_id` lists of abilities 77-80 in `skill_types.xml` has no effect.

1. **Ability gate** - "may this character use this building?" (Manage / Harvest / Clean / Slaughter, abilities 77-80). A cascade: 133 needs level 60, 134 needs 90, 143 and 517 need 30, 144 needs nothing, **any other type is refused**.
   * client `yo_cm_client.exe` RVA 0x4DD0B0 (the Coop block is the 18 bytes at 0x4DD233)
   * server `ManageAnimals_Ability::checkEntity` RVA 0x354D90 (18 bytes at 0x35502A)
   * Note: when the server cannot find the character's skills the function returns "yes" (the `Can't find skills` branch). That makes a test right after a GM action look as if it worked, and fail after a relog.
   * Patch (both): `cmp eax,90h ; jne fail ; mov bl,1 ; jmp done` becomes `cmp eax,90h ; je <ok block of the neighbouring type> ; cmp eax,<type> ; jne fail`. The server side is done by the Plus hook (`PatchManageGate` in `hook_stable_alias`), for any rule with `behavesLike="144"`.
2. **Window picker** - client `CmInventory::ShowComplexObjectInventory` (RVA ~0x410300..0x4107FF) chooses the window by building type: types 107..163 through a jump table (133/134/143/144 all go to handler 0x41076A, the stable window), 517/682/1110/1353 through a compare chain, anything else logs `This device have no interface attached`.
   * Patch: 16 bytes at RVA 0x4105B2 are a redundant repeat of a pure getter test (`mov rcx,r12 ; call 0x1C01F0 ; test al,al ; je`); they become `cmp edi,<type> ; jne +5 ; mov edi,90h ; nop nop nop`, so the type is dispatched as a Coop.
3. **Window data** - the client keeps the parameters in a table at RVA 0x153B9B0 (`.data`), 11 dwords per record `[type, 4 allowed animal items, max animals, 3 numbers, flag, product item]`, read by 8 small getters (16 RIP-relative references, RVA 0x329240-0x32A5FF). Without a row the window shows `Tiere 0/0`, trough 0/0 and refuses every animal.
   * The table cannot grow in place (another table follows directly). The patch appends a copy with one more record to the last section (`.subal`, grown by 0x200 bytes, table at RVA 0x18F0180) and points the 16 readers at the copy. The cage record is `[2834, 1053, 0, 0, 0, 30, 6, 5, 3, 0, 338]`.

## Server side (unchanged idea, now complete)
* `art/dump.sql` / client `objects_types.xml`: object 2834 is a device (`IsDevice 1`) with the stable background.
* 2834 in the object lists of abilities 77-80 (server and client `skill_types.xml`).
* `lifxpluss.xml`: `<stable objectTypeId="2834" behavesLike="144" maxAnimals="30" product="0" allowAnimals="1053"/>` (the Coop keeps chickens only); no `<warehouse>` entry.
* The boot query literal (RVA 0x7A1210) is rewritten to include 2834; only ONE extra 4-digit id fits. A cage built after boot only becomes a stable after the next server restart (`CmStable object N loaded, type 2834` in the log).

## How the gates were found (reusable method)
* A linear capstone sweep of `.text` for windows that mention several of 133/134/143/144/517 as `cmp` immediates (`tools/scan_stable_types.py`, 4 million instructions, ~1 minute) found exactly one place per executable: the ability gate.
* The window picker: the client log named the failing function (`CmInventory::ShowComplexObjectInventory`); its name string was found in `.rdata`, its RIP-relative references led to the function, and the jump table was decoded by hand from the bytes (index table + offset table).
* The table: every RIP-relative reference to the table's address range, validated instruction by instruction with the disassembler (`tools/patch_client_stabletable.py --dry` prints the plan).
* The client Ghidra project (never analysed) was not needed.

## Files
`tools/patch_client_stablegate.py`, `patch_client_stablewindow.py`, `patch_client_stabletable.py` (idempotent, verify the original bytes, back up the exe; the GMC Patcher ships the combined result as `yo_cm_client.gmcx`), `server/plus-hooks/hook_stable_alias.cpp/.h`.

## Verified in game
Window `Kaninchenstall`: Tiere 3/30, three rabbits with age and quality, dung indicator, food consumption, trough, Slaughter button. Not tested: breeding over time, harvest and slaughter results, the chicken refusal (species rule).
