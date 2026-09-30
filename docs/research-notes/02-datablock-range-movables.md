# 02 - Movable objects and the datablock id limit

## Idea (GreedyFox)
Add many more carriable/movable objects (furniture, statues, siege ladders, barrels, shields) than the game allows; the client started to fail after roughly 220 movable types.

## What was done / tried
* First attempt: free slots by removing unused developer-only movable types. It gained only a handful of ids (those types had no mounted image).
* Second attempt: find the real limit in the client and server executables and widen it on both sides.

## Output taken into the server
* Datablock id range widened from 10 to 12 bits. Server side by the Plus hook `datablockRange` (in memory only); client side by an on-disk patch of the user's own client exe (`tools/patch_dbrange_client.py`, idempotent, verifies the original bytes first, backs up).
* Result: movable-type mounted-image ids start at 1201 (were 800+), room for about 2,900 more complex object types. Carrying barrels, shields and benches and the re-added siege set were confirmed working.

## Technical notes
* Datablock ids live in [3 .. 0x402] (1024 ids). Network reads use a ranged integer `readRangedU32(min=3, max=0x402)`; the bit width is computed at runtime from min/max, so only the constants change.
* The counter for mounted-image datablocks (`Complex::ObjectType::TypeStorage` constructor) started at 800 with no upper bound, so with ~220 types it ran into the fixed script ids (sound profiles 1016-1023 and others). That produced the client error `SimObject::setId() ... same mId`.
* Patched sites (range 3..0x1002, complex types start at 1027, ordinary object ids start at 0x1043, `readInt(10)` -> 12): client 12 imm32 sites (constructor, seven ranged reads, the Sim dynamic-id start, three `readInt` widths); server sites in the hook source `source/server/hooks/engine/hook_datablock_range.cpp` (all or nothing, every site byte-checked).
* Client and server **must** match; a client without the patch reports "Invalid packet (mounted images)" on the first carry.
* Every movable object needs a base type, a carried twin (`ParentID 1902`) and an `objects_conversions` row.
* Untested: other datablock classes that might still read ids with a fixed 10-bit width. If "Invalid packet" appears in a new feature, search for `readInt(10)` followed by `+3` and the matching server writers.
* Ghidra tips: pass addresses as `0x...` and string arguments without spaces or parentheses to headless scripts.
