/**
* <author>GreedyFox</author>
* <description>Ports the MMO's Big Tanning Tub (object 2017, "bigtannery") into Lif:YO via the LiFx ServerAutoloader
*              framework. Modeled directly on TailorsWorkshopPort/mod.cs's pattern (registerObjectsTypes + raw
*              dbi.Update SQL for the recipe, since the MMO's own Influence/SkillDepends numbers don't sum to 100).
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*
* ============================ READ BEFORE USING ============================
* 1. ID COLLISION: object ID 2017 and recipe ID 1189 were checked DIRECTLY against the live `lif_1` database
*    (2026-09-15, `SELECT COUNT(*) FROM objects_types/recipe WHERE ID=...` both returned 0) — stronger than just
*    comparing against other mods' claimed-ID lists. recipe_requirement rows use NULL/auto-increment IDs (see
*    note 5 below on why the MMO's own requirement IDs 3653-3657 must NOT be reused literally). Still NOT checked
*    against the community registry at https://www.lifxmod.com/info/object-id-list/ — do that before a real world.
* 2. ART ASSETS ARE MISSING: FaceImage/ImagePath below still point at the ORIGINAL MMO paths
*    (art\2D\Objects\big_tanning_tub.png, art\2D\Recipes\big_tanning_tub.png) and the 3D model
*    (art/models/3d/construction/craftingdevices/tanning_tub/big_tanning_tub.dts, per cm_objects.xml) is not part
*    of the staticData_extracted dump on F:\Lif Datein. Until those exist under this mod's own folder, the object
*    will register in the DB but be invisible / fail to place in-game.
* 3. NO DATABLOCK YET, AND THIS ONE CAN'T COPY LOG CART'S PATTERN: per cm_objects.xml, Big Tanning Tub uses a
*    Complete/Working state pair (NOT the Incomplete/Damaged/Complete pattern Log Cart/Tailor's Workshop use).
*    Its "Working" state also has a SmokeEmitterLiF2Z particle emitter and an Object_TanningTub sound emitter that
*    need to be wired into whatever datablock script eventually gets written — a straight copy of an
*    Incomplete/Damaged/Complete datablock (like Transport.cs) will not fit this object's actual state machine.
* 3A. `LiFx::registerObjectsTypes()` BELOW DOES NOT ACTUALLY CREATE THE OBJECT — found 2026-09-15 while
*    investigating why Log Cart's "installed" object/recipe don't show up live. Reading ServerAutoloader-4.3.0\
*    main.cs closely: registerObjectsTypes() only APPENDS its INSERT SQL to a file (sql/dump.sql); nothing
*    anywhere reads that file back and executes it. Confirmed empirically against the live DB: Log Cart's own
*    object 3016 doesn't exist despite being "installed". FIXED here by also inserting the object row via raw
*    dbi.Update() in dbChanges() below — the same proven-live mechanism the recipe/recipe_requirement rows
*    already use. The registerObjectsTypes() call in setup() is kept only for convention/in case some
*    undiscovered part of the pipeline consumes sql/dump.sql. See CLAUDE.md's top section for the full finding,
*    which also applies to the still-unresolved "is the LiFx framework even deployed on the live server"
*    question — deployment status has not been fixed by this change.
* 4. THE MMO-STYLE ABILITY GATE (id 386 "Use Big Tanning Tub", Procuration skill) IS CONFIRMED IMPOSSIBLE, NOT
*    JUST UNIMPLEMENTED — same conclusion as StonemasonsWorkplacePort note #4 (see that file for the full
*    writeup): abilities are one compiled C++ class per ID (311 total, RTTI-confirmed via prior LiFx/Plus RE,
*    docs/effects_and_abilities.md), not XML/DB-driven. Live-tested by actually adding a new ability ID to the
*    deployed server's data/skill_types.xml (for Stonemason's Workplace, same mechanism) — crashed with
*    "using of unregistered ability", confirmed and reverted. Adding a brand-new ability class needs engine
*    source, which isn't available. Do not attempt a dbi.Update() sketch against skill_type, and do not add new
*    ability IDs to skill_types.xml.
* 4A. SOLVED A DIFFERENT WAY instead: recipe-level StartingToolsID station-gating, the same mechanism vanilla
*    YO already uses for Kiln/Furnace/Forge and Anvil (and now Stonemason's Workplace, see
*    StonemasonsWorkplacePort/mod.cs recipe 5020). See the new bulk recipe (ID 5021) in dbChanges() below,
*    modeled directly on vanilla's own "Thick Leather" recipe (ID 186, which normally requires the SMALL
*    Tanning Tub object 472 as StartingToolsID) but gated on THIS object (2017) instead, with output scaled up
*    for bulk production. Doesn't replicate the MMO's real-time "Use Big Tanning Tub" animation/interaction,
*    but does deliver the actually-useful part: production restricted to this building, at scale.
* 5. RECIPE_REQUIREMENT IDs: the MMO's own requirement rows for this recipe are IDs 3653-3657 in its source data,
*    but those IDs are ALREADY IN USE on the live YO database for unrelated recipes (910/911/912) — confirmed via
*    live query 2026-09-15. This is not actually a problem: like TailorsWorkshopPort, the inserts below use NULL
*    for the requirement row's own ID and let auto-increment assign a fresh one; only RecipeID (1189) and
*    MaterialObjectTypeID matter for correctness. Just don't "fix" this by hardcoding 3653-3657 later.
* 6. SKILL ID (corrected 2026-09-15, see git history / chat log if this note looks odd): the recipe's
*    SkillTypeID=18 ("Construction") is the skill needed to BUILD the structure, straight from the MMO's own
*    recipe.xml row (ID 1189) — this is a SEPARATE association from the ability's governing skill (Procuration,
*    id 386, needed to USE the tub once built, see note #4). Do not conflate the two and "correct" this value to
*    23 (Procuration) — that was tried and was wrong. Verified MMO and YO actually use IDENTICAL skill_type
*    numbering (MMO's own skill_types.xml has 18=Construction, 19=Masonry, 23=Procuration, 25=Tailoring,
*    matching the live YO DB exactly) — there is no MMO/YO translation needed for skill IDs at all, on this or
*    any other workshop. Material/object IDs (Hammer=32, Boards=235, etc.) also checked live and match.
* 7. DON'T CONFUSE WITH THE SMALLER, OLDER "TANNING TUB" (object ID 472, near-duplicate ID 1941) which already
*    exists separately in the MMO data. This mod is specifically the BIG Tanning Tub (2017) — confirm with
*    GreedyFox which one they actually want if that's ever unclear.
* =============================================================================
*/

if (!isObject(LiFxBigTanningTub))
{
    new ScriptObject(LiFxBigTanningTub)
    {
    };
}

package LiFxBigTanningTub
{
    function LiFxBigTanningTub::setup() {
        LiFx::registerCallback($LiFx::hooks::onServerCreatedCallbacks, Datablock, LiFxBigTanningTub);
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxBigTanningTub);

        // Register the new object (building) itself. NOTE: per caveat #3A, this call alone does NOT reach
        // the live DB — kept for convention, but dbChanges() below does the real INSERT via dbi.Update().
        LiFx::registerObjectsTypes(LiFxBigTanningTub::ObjectsTypesBigTanningTub(), LiFxBigTanningTub);
    }

    function LiFxBigTanningTub::version() {
        return "0.1.0-draft";
    }

    function LiFxBigTanningTub::Datablock() {
        // TODO: exec a datablock script here once you have one — see note #3 above, this needs its own
        // Complete/Working state machine, not a copy of an Incomplete/Damaged/Complete one, e.g.:
        // exec("yolauncher/modpack/mods/LiFx/BigTanningTub/art/datablocks/BigTanningTub.cs");
    }

    // Straight port of the MMO's objects_types row for ID 2017 ("Big Tanning Tub"), ParentID 64 ("Crafting"),
    // verified to exist under that name on the live YO DB.
    function LiFxBigTanningTub::ObjectsTypesBigTanningTub() {
        return new ScriptObject(ObjectsTypesBigTanningTub : ObjectsTypes)
        {
            id = 2017; // verified free against the live DB 2026-09-15 — see note #1 above
            ObjectName = "Big Tanning Tub";
            ParentID = 64;
            IsContainer = 1;
            IsMovableObject = 0;
            IsUnmovableobject = 1;
            IsTool = 0;
            IsDevice = 1;
            IsDoor = 0;
            IsPremium = 0;
            MaxContSize = 300000;
            Length = 6;
            MaxStackSize = 0;
            UnitWeight = 10000;
            BackgrndImage = "";
            WorkAreaTop = 0;
            WorkAreaLeft = 0;
            WorkAreaWidth = 0;
            WorkAreaHeight = 0;
            BtnCloseTop = 0;
            BtnCloseLeft = 0;
            // TODO: repoint at this mod's own bundled copy of the art, e.g.
            // "yolauncher/modpack/mods/LiFx/BigTanningTub/2D/Objects/big_tanning_tub.png"
            FaceImage = "art/2D/Objects/big_tanning_tub.png";
            Description = "";
            BasePrice = 0;
            OwnerTimeout = 0;
            AllowExportFromRed = 0;
            AllowExportFromGreen = 0;
        };
    }

    // Recipe to build it, ported from the MMO's recipe.xml (ID 1189) + recipe_requirement.xml (rows for
    // RecipeID 1189, MMO IDs 3653-3657 — not reused here, see note #5). Using raw dbi.Update() like
    // TailorsWorkshopPort, NOT LiFx::registerRecipe() — the MMO's own Influence values (30+20+10+10+10 = 80)
    // plus SkillDepends (40) add up to 120, not the 100 that registerRecipe() enforces, so that helper would
    // reject this row outright.
    function LiFxBigTanningTub::dbChanges() {
        //////////////////////////////////////// Idempotency guard ////////////////////////////////////////
        // CRITICAL, learned 2026-09-17: recipe_requirement has no natural uniqueness constraint, and
        // art/dump.sql ALSO seeds these same rows on every boot now (see JewelersWorkshopPort note #9 for
        // the full incident) — without this DELETE, every boot doubles every material's Influence weight.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` IN (1189,5021,5600,5601)");

        //////////////////////////////////////// Object ////////////////////////////////////////
        // (ID, ParentID, Name, IsContainer, IsMovableObject, IsUnmovableobject, IsTool, IsDevice, IsDoor,
        //  IsPremium, MaxContSize, Length, MaxStackSize, UnitWeight, BackgndImage, WorkAreaTop, WorkAreaLeft,
        //  WorkAreaWidth, WorkAreaHeight, BtnCloseTop, BtnCloseLeft, FaceImage, Description, BasePrice,
        //  OwnerTimeout, AllowExportFromRed, AllowExportFromGreen) — see caveat #3A for why this is here
        // instead of relying on LiFx::registerObjectsTypes() alone.
        // Note on FaceImage: an earlier version of this line used backslash-escaped paths
        // ("art\\\\2D\\\\Objects\\\\..."), which produced a genuinely corrupted stored value the
        // first time this deployed live (a raw 0x08 backspace byte ate the "b" of "big_tanning_tub" —
        // confirmed via SELECT HEX(FaceImage)). Root cause genuinely NOT understood: the recipe/ImagePath
        // dbi.Update() calls right below use the IDENTICAL escaping pattern (one even starts with
        // "big_tanning_tub.png" again) and came out byte-correct in the same boot, and
        // StonemasonsWorkplacePort's own objects_types insert (same position — first dbi.Update() call in its
        // dbChanges()) also came out correct. So this isn't a simple/deterministic backslash-count or
        // MySQL-escape-sequence bug — looks more like a one-off or timing-related corruption whose real cause
        // wasn't pinned down. Switched to forward slashes here as a robust fix regardless of cause, matching
        // Log Cart's own working convention elsewhere in this project — worth doing for every workshop's
        // paths as a precaution, not just as a targeted fix for this one observed case.
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2017,64,'Big Tanning Tub',1,0,1,0,1,0,0,300000,6,0,10000,'',0,0,0,0,0,0,'art/2D/Objects/big_tanning_tub.png','',0,0,0,0)");

        //////////////////////////////////////// Recipe ////////////////////////////////////////
        // (RecipeID, Name, Description, StartingToolsID, SkillTypeID, SkillLvl, ResultObjectTypeID,
        //  SkillDepends, Quantity, Autorepeat, IsBlueprint, ImagePath)
        // SkillTypeID=18 ("Construction") straight from the MMO's own recipe.xml — see note #6 above for why
        // this is correct as-is and should NOT be changed to 23 (Procuration, the ability's skill, not the
        // build recipe's skill).
        // StartingToolsID=32 (Hammer) — 2026-09-17 fix, was NULL, which silently kept this recipe out of
        // the in-game Build menu entirely. Every vanilla "Crafting bonus buildings" recipe (Blacksmith's/
        // Carpenter's/Herbalist's Shop, School, + wooden variants) uses Hammer (32), confirmed live — the
        // build UI evidently keys off holding this specific tool to populate that menu category.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1189,'Big Tanning Tub','',32,18,60,2017,40,1,0,0,'art/2D/Recipes/big_tanning_tub.png')");

        //////////////////////////////////// Recipe Requirements ////////////////////////////////////
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1189, 32, 0, 30, 25, 0)");   // 25 x Hammer
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1189, 235, 0, 20, 30, 0)");  // 30 x Boards — IsRegionItemRequired unverified for THIS recipe, left 0, see note below
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1189, 282, 0, 10, 3, 0)");   // 3 x Metal Band
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1189, 262, 0, 10, 2, 0)");   // 2 x Linen Rope
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1189, 324, 0, 10, 10, 0)");  // 10 x Hardwood Billet
        // Note on Boards' IsRegionItemRequired: Tailor's Workshop's own Boards requirement was recorded as a
        // region item (flag=1). The handoff data gathered for THIS recipe's Boards row never recorded that flag
        // either way — left at 0 (not region-locked) rather than assumed. Re-check recipe_requirement.xml's row
        // for RecipeID 1189 / MaterialObjectTypeID 235 directly if this matters before going live.

        //////////////////////////////////// Ability gate (NOT INCLUDED — see note #4) ////////////////////////////////////
        // Confirmed genuinely infeasible without engine source — abilities are compiled C++ classes, not
        // data-driven. Do not attempt a dbi.Update() sketch here or add new ability IDs to skill_types.xml.

        //////////////////////////////////// "Mass production" recipe (see note #4A) ////////////////////////////////////
        // Station-gated via StartingToolsID=2017, same mechanism vanilla YO uses for Kiln/Furnace/Forge and Anvil/
        // Stonemason's Workplace. Modeled on vanilla's own "Thick Leather" recipe (ID 186: StartingToolsID=472
        // small Tanning Tub, 1x Thick Dried Hide [region item] + 4x Water + 20 durability off the tub -> 1x Thick
        // Leather, SkillTypeID=23 Procuration, SkillLvl=60, SkillDepends=10). This version: 5x Thick Dried Hide +
        // 20x Water -> 5x Thick Leather per craft (inputs/output both scaled 5x), tub wear left at the original
        // 20 (one wear-cost per batch, not per hide — a deliberate balance choice, revisit if it feels off).
        // (RecipeID, Name, Description, StartingToolsID, SkillTypeID, SkillLvl, ResultObjectTypeID,
        //  SkillDepends, Quantity, Autorepeat, IsBlueprint, ImagePath)
        // 2026-09-18: StartingToolsID changed from 2017 (the building) to 472 (the vanilla small Tanning Tub),
        // matching vanilla's own Thick Leather recipe (186) exactly. Confirmed live tonight: a recipe's
        // StartingToolsID pointing at an IsUnmovableobject=1 building is NEVER reachable via any Craft UI in
        // this client -- only a portable item or an IsMovableObject=1 complex_obj (like the small Tanning Tub
        // itself) opens a Craft window. The MaterialObjectTypeID=2017 requirement row below still enforces
        // needing the Big Tanning Tub specifically nearby for materials.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5021,'Thick Leather (Big Tanning Tub)','',2017,23,60,424,10,5,1,0,'art/2D/Items/thick_leather.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5021, 474, 0, 75, 5, 1)");  // 5 x Thick Dried Hide (region item)
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5021, 204, 0, 5, 20, 0)");  // 20 x Water
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5021, 2017, 0, 10, 20, 0)"); // 20 durability off the Big Tanning Tub itself
        // Added 2026-09-21: the other two small-Tanning-Tub recipes as x5 batches for the Big Tanning Tub (same input:output ratio as vanilla 187 / 191, no net gain).
        // 2026-09-21 supersedes the 2026-09-18 note above: the Big Tanning Tub (2017) is now in ability 85/116's object lists in skill_types.xml, so StartingToolsID is the big tub itself again (same as Big Drying Frame 5024/2829).
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5600,'Thin Leather (Big Tanning Tub)','',2017,23,60,425,10,5,1,0,'art/2D/Items/thin_leather.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5601,'Flax Fibers (Big Tanning Tub)','',2017,23,0,377,5,100,1,0,'art/2D/Items/flax_fibers.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5600, 475, 0, 75, 5, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5600, 204, 0, 5, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5600, 2017, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5601, 361, 0, 85, 50, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5601, 204, 0, 5, 10, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5601, 2017, 0, 5, 30, 0)");
    }
};
activatePackage(LiFxBigTanningTub);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxBigTanningTub);
