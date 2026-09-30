# 08 - Content packs ported into the server

## Idea (GreedyFox)
Extend the YO server with content from other builds and mods, then curate it: Jorvik mod (54 objects), Arden fortifications, EKR buildings, altars and idols, statues, displays (mannequins, shields), chests and furniture, cloaks/flags/outfits, horn-helmet skins, Knool creatures and weapons, food and drinks, tribe-camp decorations, siege equipment, and plain MMO items. Afterwards: remove what is too heavy or unwanted and adjust skills and levels.

## What was done / tried
* Each pack is a `mods/LiFx/<Pack>/mod.cs` that registers objects, recipes, requirements and (for movables) conversions with `INSERT IGNORE` in `dbChanges()`, plus client data rows, a `cm_objects` model block per object and art under `mod/<Name>/art`.
* Content was tested in game in stages; pieces that did not fit were removed or commented out rather than deleted, so they can be switched back on.

## Output taken into the server (ids; see `docs/reports` for every id)
* Jorvik (3001-3053 + recipes 1088-1174), Slavic fortifications (2900-2923, recipes 5500-5523, cloned from vanilla wall recipes), EKR buildings (3655-3753, recipes 6404-6482), altars/idols (3100-3105), displays (3111-3152), signs and pillars (3106-3110), furniture (3153-3167), statues (3169-3174), food and drinks (3175-3193), chests (3537-3544), metal material chain (3545-3590), outfits (3591-3592), siege (3593-3601), Buildings pack (3602-3603), King outfits (3611-3625), cloaks/flags (3626-3642), horn skins (3643-3654), Knool (2461-2466), MMO plain items (3800-3913).
* Removed on request: twelve EKR buildings with more than 20,000 polygons (heavy on frame rate), the Engel statue, the tribe-camp decorations, the Knool NPCs and the Primitive Bed. Metal bar/ingot/lump and Knool weapon recipes are commented out (items still spawnable).
* Skill/level changes: furniture and fancy recipes in Carpentry (levels 90/100), decorations (pillars, lights, shields, mannequins, candles) in Arts (skill 53), "verzierte" vanilla recipes lowered from 100 to 90.

## Technical notes
* Id ranges must not collide across packs; keep a registry of used object, recipe and requirement ids (the packs here use disjoint ranges, listed in the README table).
* Movable objects need a base type, a twin with `ParentID 1902` and an `objects_conversions` row; the siege set (kit 3593, ladders and platform 3594-3597, twins 3598-3601, recipes 6348-6352) follows that pattern and needs the datablock range fix (note 02).
* The build window groups recipes by `SkillTypeID` and lists all recipes regardless of the player's skill level.
* Homecoming prayer (ability 146) only counts object types 84, 85 and 523 as beds (two loops in `CharacterParameters::_getObjectBindPoint`); custom beds work for "Lay down" (ability 276) but never as a bind point without a code cave in the Plus DLL.
* Jorvik: one duplicate shed was removed together with its entries in the Lift-Object exclusion list and an `objects_patch` row that would otherwise break the boot.
* New type ids must be baked into `art/dump.sql` if world rows reference them (note 01).
* EKR: an "unmovable" flag must be set on both server and client (`IsMovableObject` in the client `objects_types.xml` decides whether the build window offers it).
* Food and drinks: 8 dishes (3186-3193) with recipes; the 26 extra MMO dishes in the MMO items pack are plain items (no food effect data, no recipes).
* Lay down: ability 192 plays a lying pose through a Plus hook plus a client script; other players do not see it yet (client-only visual).
* Translation status: `docs/reports/` lists every id with its German name status.
