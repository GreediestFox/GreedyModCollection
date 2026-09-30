# 07 - Porting art (models, textures, icons, character meshes) and the tools built for it

## Idea (GreedyFox)
Bring buildings, furniture, icons, outfits and character skins from the MMO build, Arden, EKR and Jorvik into the YO client, make recolored clothing skins without new meshes, and port all MMO icons.

## What was done / tried
* Models were ported as `.dts` files with their textures and material definitions; early ports rendered orange or flat.
* A tool was written to clone a named object inside a `.dts` with a new material name (recolors), and the wardrobe skin list was made extensible by a client patch.
* A first attempt to switch character meshes (miner, cattleman) failed until the mesh-hiding mechanism was understood.
* All icon zips were compared with the client's `art/2D` and the missing icons copied in.

## Output taken into the server / client
* Outfit/skin items and 2,355 new icons (39 changed) in the client `art/2D`; custom skin ids appear in the wardrobe (`tools/patch_client_skins.py`).
* Helpers: `dts_clone.py`, `verify_clone.py`, `make_dxt1.py`, `port_icons.py`, `hops_icon.py` (see the README tool list).

## Technical notes
* **Orange / untextured models:** the client needs a `singleton Material(...)` with `mapTo = "<material name in the dts>"` and `diffuseMap[0..2]` (diffuse, normal, spec), `materialTag0 = "LiF"` in `art/materials.cs`; missing definitions render orange, and a stale `.dso` next to the script hides edits. The MMO "Devices" texture atlas can be reused by several device models.
* **DDS files:** this client needs DXT1/DXT5-compressed DDS with a full mip chain; uncompressed DDS shows flat grey. `make_dxt1.py` writes a header identical to the vanilla files.
* **TSShape v24 facts (validated byte-exact):** three streams (32/16/8-bit), guard values (one element in every stream, no alignment padding), meshes after the header arrays, names at the end of the 8-bit stream, sequences and the material list in the tail (material list = U8 version 1, S32 count, names, six arrays). After inserting object rows every later 32-bit index shifts (`subShapeNumObjects` must be set at the shifted index); a stale count crashed the server with a null map lookup.
* **Recolors:** clone an object (for example `Male_Peasant` -> `Male_Peasant_SkinB` with its own material), add the Material definition in the client `materials.cs`, point the item/skin at the new mesh in `cm_equipTypes` (client and server).
* **Character mesh switching:** mesh hiding is **server-authoritative**, a bitmask of hidden meshes by object *index* (the DTS "visibility" flag is 1 everywhere, the runtime decides). Therefore the server and client must use the **same** `male.dts`/`female.dts`; a mismatch hides the wrong meshes. `visibleMeshes` exists only in the animal data class (dead end for players).
* **Wardrobe skins** come from Steam inventory item definitions (`SteamActions::GetAvailableSkinsAction`); the patch appends a table of extra skin ids as owned when the equipped item type lists that skin in `cm_equipTypes`. `defaultSkins.xml` only remembers the last choice. The GM `give` command needs durability 100.
* The dedicated server does not need 2D icon files (confirmed).
* Outfit materials that exist only in the MMO client (King, Tatters, Surcoat, FlagOfficer, Merchant) are not mapped in the YO `materials.cs` yet.
* Ghidra helper scripts (decompile at RVA, callers-of, find immediates, disassemble range) drove most of the analysis; see note 10.
