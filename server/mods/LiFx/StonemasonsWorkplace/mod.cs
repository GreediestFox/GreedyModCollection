/**
* <author>GreedyFox</author>
* <description>Ports the MMO's Stonemason's Workplace (object 2020) into Lif:YO via the LiFx ServerAutoloader
*              framework. Modeled on TailorsWorkshopPort/BigTanningTubPort's pattern, with one deliberate
*              deviation — see note #0, the most important caveat in this file.
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*
* ============================ READ BEFORE USING ============================
* 0. THIS MOD HAS NOT BEEN PROVEN TO WORK, AND NEITHER HAVE THE EARLIER DRAFTS — two structural
*    problems found 2026-09-15, while investigating why Log Cart's "installed" object/recipe don't actually
*    show up on the live DB despite being marked installed by the mod installer tool:
*    (a) THE LIVE SERVER HAS NO LiFx/ServerAutoloader DEPLOYMENT AT ALL. `F:\SteamLibrary\...\Life is Feudal
*        Your Own Dedicated Server\` has no `mods` folder, and its root main.cs is 100% vanilla
*        (`core/parseArgs.cs` -> `scripts/root.cs`, no LiFx exec anywhere). Every mod worked on in this
*        project (F:\Lif Mods, F:\Lif Mod Packs) has been staged separately from the actual running server
*        and never copied in. This mod.cs will not run until that deployment gap is closed.
*    (b) `LiFx::registerObjectsTypes()` and `LiFx::registerRecipe()` DO NOT WRITE TO THE LIVE DATABASE even
*        once deployed — read `ServerAutoloader-4.3.0\main.cs` closely: both only APPEND their INSERT SQL to
*        a file, `sql/dump.sql`, and nothing anywhere reads that file back and executes it (no dbi.Update()/
*        dbi.exec() call touches it). Confirmed empirically: Log Cart's object 3016 and recipe 1093 don't
*        exist on the live `lif_1` DB despite that mod sitting installed across multiple server boots.
*    DEVIATION FROM THE EARLIER TEMPLATE: because of (b), this draft does NOT rely on
*    `LiFx::registerObjectsTypes()` to actually create the object row — it's still called in setup() to match
*    the framework's expected convention (and in case some undiscovered part of the pipeline does consume
*    sql/dump.sql), but the ACTUAL objects_types INSERT is duplicated as a real `dbi.Update()` call in
*    dbChanges() below, the same proven-live mechanism the recipe/recipe_requirement rows already use.
*    TailorsWorkshopPort and BigTanningTubPort do NOT have this duplicate INSERT yet and likely need the same
*    fix — not done here to keep this file scoped to Stonemason's Workplace; flag to GreedyFox before assuming
*    either of those will work as currently written.
* 1. ID COLLISION: object ID 2020 and recipe ID 1179 verified free directly against the live DB (2026-09-15).
*    recipe_requirement uses NULL/auto-increment, not the MMO's own IDs (3620-3626) — see TailorsWorkshopPort
*    note #5 for why that's fine. Still NOT checked against https://www.lifxmod.com/info/object-id-list/.
* 2. ART ASSETS ARE MISSING: FaceImage/ImagePath point at the ORIGINAL MMO paths
*    (art\2D\Objects\mason_workplace.png, art\2D\Recipes\mason_workplace.png) and the 3D model
*    (art/models/3d/construction/craftingdevices/mason_workplace/mason_workplace.dts) is not part of the
*    staticData_extracted dump on F:\Lif Datein.
* 3. NO DATABLOCK YET. Note this object's state machine is UNUSUAL: cm_objects.xml shows only a single
*    1x1 footprint cell, its "Incomplete" state reuses a generic placeholder model
*    (art/models/3d/construction/misc/site/site.dts, shared across many buildings under construction — not
*    this mod's own asset), and its "Damaged" state reuses the SAME model as "Complete" rather than having its
*    own _dmg variant. Don't copy Tailor's/Jeweler's Workshop's three-distinct-model assumption blindly.
* 4. THE MMO-STYLE ABILITY GATE ("Shape Stones"/"Shape", ids 371/372) IS CONFIRMED IMPOSSIBLE, NOT JUST
*    UNIMPLEMENTED — investigated to a firm conclusion 2026-09-15. Abilities in this engine are NOT XML/DB-driven
*    like effects or recipes: RTTI recovery (prior LiFx/Plus RE, see docs/effects_and_abilities.md in the
*    Plus-main repo) found abilities are one compiled C++ class per ability ID (311 total, e.g. Build_Ability,
*    Cook_Ability), each with its own vtable and hand-written behavior. Live-tested by actually adding ids
*    371/372 to the deployed server's data/skill_types.xml: crashed with "AbilityManager::_parseXmlAbilityNode()
*    - using of unregistered ability (id=371)" — confirmed and reverted. Adding a brand-new ability class is
*    infeasible without engine source, which isn't available. Extending an ALREADY-REGISTERED ability's
*    object_type_id list DOES work (e.g. "Run Millstones" id 262) but no existing ability fits this object
*    thematically without being obviously wrong in-game.
* 5. SOLVED A DIFFERENT WAY: USE THE RECIPE SYSTEM'S StartingToolsID FOR STATION-GATING INSTEAD OF THE ABILITY
*    SYSTEM. This is how vanilla YO itself gates recipes to specific placed devices — confirmed live: "Vase"/
*    "Masterwork Vase"/"Urn" recipes require StartingToolsID=117 ("Kiln", an IsUnmovableobject=1 placed device,
*    not a carried tool), "Anvil" requires StartingToolsID=107 ("Furnace"), "Shovel"/"Pickaxe"/etc require
*    StartingToolsID=453 ("Forge and Anvil"). This is a completely different, unrelated-to-abilities mechanism —
*    plain recipe data, using the exact same dbi.Update() INSERT INTO `recipe` pattern already proven live
*    below. It doesn't replicate the MMO's real-time "Shape" animation/interaction, but it DOES achieve
*    "can only be produced at this specific building" plus "mass produce" (via Quantity + Autorepeat, both
*    recipe-level fields) — which is arguably the actually-useful part of a "workshop" in gameplay terms.
*    See the new recipe (ID 5020) in dbChanges() below, modeled directly on vanilla's own Shaped Rock recipe
*    (ID 601, which normally just needs a carried Pickaxe) but gated on this building instead, with a bulk
*    5x-per-craft yield to reward actually building the station over the vanilla anywhere-with-a-pickaxe method.
* =============================================================================
* 6. CRAFTING AT THE BUILDING (2026-09-21): the Stonemason's Workplace now has a menu. Ability 64 "Sculpt" (Materials Preparation, skill row 17; the potter's-wheel device ability)
*    lists building 2020 ("123 2020" in server+client skill_types.xml) and its level requirement was lowered 60 -> 0 (recipes keep their own SkillLvl gates). Recipes 5400-5403 are clones of
*    the pickaxe recipes 601/602/603/632 (Shaped Rock / Granite / Marble Plate / Stone Ammo) with StartingToolsID=2020 and the pickaxe row replaced by the device row 2020 with DOUBLED
*    Influence. Bulk recipe 5020 was moved from the pickaxe (35, uncraftable there because its 2020 device row fails the inventory check) to the building and renamed "Shaped Rock (bulk)"
*    (100 Rock -> 5 Shaped rock = same 20:1 ratio as vanilla). The workshop quality-buff hook rule for 2020 already lists ability 64.
**/

if (!isObject(LiFxStonemasonsWorkplace))
{
    new ScriptObject(LiFxStonemasonsWorkplace)
    {
    };
}

package LiFxStonemasonsWorkplace
{
    function LiFxStonemasonsWorkplace::setup() {
        LiFx::registerCallback($LiFx::hooks::onServerCreatedCallbacks, Datablock, LiFxStonemasonsWorkplace);
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxStonemasonsWorkplace);

        // Register the new object (building) itself. NOTE: per caveat #0(b), this call alone does NOT reach
        // the live DB — kept for convention, but dbChanges() below does the real INSERT via dbi.Update().
        LiFx::registerObjectsTypes(LiFxStonemasonsWorkplace::ObjectsTypesStonemasonsWorkplace(), LiFxStonemasonsWorkplace);
    }

    function LiFxStonemasonsWorkplace::version() {
        return "0.1.0-draft";
    }

    function LiFxStonemasonsWorkplace::Datablock() {
        // TODO: exec a datablock script here once you have one — see note #3 above for this object's unusual
        // single-model Complete/Damaged + generic-placeholder Incomplete state shape, e.g.:
        // exec("yolauncher/modpack/mods/LiFx/StonemasonsWorkplace/art/datablocks/StonemasonsWorkplace.cs");
    }

    // Straight port of the MMO's objects_types row for ID 2020 ("Stonemason's Workplace"), ParentID 64
    // ("Crafting", same parent as Big Tanning Tub / Ore Washer / the other free-standing crafting devices).
    function LiFxStonemasonsWorkplace::ObjectsTypesStonemasonsWorkplace() {
        return new ScriptObject(ObjectsTypesStonemasonsWorkplace : ObjectsTypes)
        {
            id = 2020; // verified free against the live DB 2026-09-15 — see note #1 above
            // Apostrophe deliberately dropped here -- see JewelersWorkshopPort/mod.cs's matching note,
            // 2026-09-18: registerObjectsTypes()'s sql/dump.sql file-write path doesn't escape apostrophes and
            // something reads that file back as a native DB patch, fatally crashing boot on a raw apostrophe.
            // Real in-game/DB name ("Stonemason's Workplace") is unaffected, set separately via dbi.Update().
            ObjectName = "Stonemasons Workplace";
            ParentID = 64;
            IsContainer = 1;
            IsMovableObject = 0;
            IsUnmovableobject = 1;
            IsTool = 0;
            IsDevice = 1;
            IsDoor = 0;
            IsPremium = 0;
            MaxContSize = 1200000;
            Length = 10;
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
            // "yolauncher/modpack/mods/LiFx/StonemasonsWorkplace/2D/Objects/mason_workplace.png"
            FaceImage = "art/2D/Objects/mason_workplace.png";
            Description = "";
            BasePrice = 0;
            OwnerTimeout = 0;
            AllowExportFromRed = 0;
            AllowExportFromGreen = 0;
        };
    }

    // Object row + recipe + requirements, ALL via raw dbi.Update() — see note #0(b) for why the object row
    // is inserted here too, not left to LiFx::registerObjectsTypes() alone.
    //
    // Recipe ported from the MMO's recipe.xml (ID 1179) + recipe_requirement.xml (IDs 3620/3622/3624/3626).
    // Influence sum (20+20+20+20=80) + SkillDepends (20) = 100 EXACTLY — this is the first workshop found in
    // this project where that's true, meaning it WOULD validate against LiFx::registerRecipe(). Deliberately
    // NOT using that helper anyway, per note #0(b): it has the exact same "only writes to a file" problem as
    // registerObjectsTypes(), so raw dbi.Update() is used for consistency and because it's the only mechanism
    // actually confirmed to reach the live DB.
    function LiFxStonemasonsWorkplace::dbChanges() {
        //////////////////////////////////////// Idempotency guard ////////////////////////////////////////
        // CRITICAL, learned 2026-09-17: recipe_requirement has no natural uniqueness constraint, and
        // art/dump.sql ALSO seeds these same rows on every boot now (see JewelersWorkshopPort note #9 for
        // the full incident) — without this DELETE, every boot doubles every material's Influence weight.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` IN (1179,5020,5400,5401,5402,5403,5404,5405)");

        //////////////////////////////////////// Object ////////////////////////////////////////
        // (ID, ParentID, Name, IsContainer, IsMovableObject, IsUnmovableobject, IsTool, IsDevice, IsDoor,
        //  IsPremium, MaxContSize, Length, MaxStackSize, UnitWeight, BackgndImage, WorkAreaTop, WorkAreaLeft,
        //  WorkAreaWidth, WorkAreaHeight, BtnCloseTop, BtnCloseLeft, FaceImage, Description, BasePrice,
        //  OwnerTimeout, AllowExportFromRed, AllowExportFromGreen)
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2020,64,'Stonemason\\'s Workplace',1,0,1,0,1,0,0,1200000,10,0,10000,'',0,0,0,0,0,0,'art/2D/Objects/mason_workplace.png','',0,0,0,0)");

        //////////////////////////////////////// Recipe ////////////////////////////////////////
        // (RecipeID, Name, Description, StartingToolsID, SkillTypeID, SkillLvl, ResultObjectTypeID,
        //  SkillDepends, Quantity, Autorepeat, IsBlueprint, ImagePath)
        // SkillTypeID=18 ("Construction") straight from the MMO's own recipe.xml — verified live to match YO.
        // StartingToolsID=32 (Hammer) — 2026-09-17 fix, was NULL, see BigTanningTubPort for the full note.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1179,'Stonemason\\'s Workplace','',32,18,0,2020,20,1,0,0,'art/2D/Recipes/mason_workplace.png')");

        //////////////////////////////////// Recipe Requirements ////////////////////////////////////
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1179, 324, 0, 20, 10, 0)");   // 10 x Hardwood Billet
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1179, 235, 0, 20, 20, 0)");   // 20 x Boards
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1179, 1356, 0, 20, 10, 0)");  // 10 x Simple Rope
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1179, 32, 0, 20, 20, 0)");    // 20 x Hammer

        //////////////////////////////////// Ability gate (NOT INCLUDED — see note #4) ////////////////////////////////////
        // Confirmed genuinely infeasible without engine source — abilities are compiled C++ classes, not
        // data-driven. Do not attempt a dbi.Update() sketch here or add new ability IDs to skill_types.xml.

        //////////////////////////////////// "Mass production" recipe (see note #5) ////////////////////////////////////
        // Station-gated via StartingToolsID=2020, same mechanism vanilla YO uses for Kiln/Furnace/Forge and Anvil.
        // Modeled on vanilla's own Shaped Rock recipe (ID 601: StartingToolsID=35 Pickaxe, 20x Rock -> 1x Shaped
        // Rock, Autorepeat=1). This version: no pickaxe needed (the station substitutes for it), 100x Rock ->
        // 5x Shaped Rock per craft, still Autorepeat=1 so it queues — the actual "mass production" behavior.
        // (RecipeID, Name, Description, StartingToolsID, SkillTypeID, SkillLvl, ResultObjectTypeID,
        //  SkillDepends, Quantity, Autorepeat, IsBlueprint, ImagePath)
        // SkillTypeID=17 is YO's live "Materials Preparation" — matches vanilla recipe 601's own skill exactly.
        // 2026-09-18: StartingToolsID changed from 2020 (the building) to 35 (Pickaxe), matching vanilla's own
        // Shaped Rock recipe (601) exactly. Confirmed live tonight: a recipe's StartingToolsID pointing at an
        // IsUnmovableobject=1 building is NEVER reachable via any Craft UI in this client -- only a portable
        // item or an IsMovableObject=1 complex_obj (like vanilla's own small Tanning Tub) opens a Craft window.
        // Added the new MaterialObjectTypeID=2020 requirement row below to keep this recipe workshop-exclusive
        // now that StartingToolsID itself no longer enforces that.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5020,'Shaped Rock (bulk)','',2020,17,0,269,10,5,1,0,'art/2D/Items/shaped_rock.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5020, 241, 0, 90, 100, 0)"); // 100 x Rock
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5020, 2020, 0, 30, 20, 0)"); // 20 durability off the Workplace itself

        //////////////////////////////////// Building recipes for the Sculpt menu (5400-5403) - see note #6 ////////////////////////////////////
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5400,'Shaped Rock','',2020,17,0,269,45,1,1,0,'art/2D/Items/shaped_rock.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5401,'Shaped Granite','',2020,17,0,271,45,1,1,0,'art/2D/Items/shaped_granite.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5402,'Marble Plate','',2020,17,0,270,45,1,1,0,'art/2D/Items/marble_plate.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5403,'Stone Ammo','',2020,17,0,1107,30,1,1,0,'art/2D/Items/Stone_bomb.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5400, 2020, 0, 10, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5400, 241, 0, 50, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5401, 2020, 0, 10, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5401, 243, 0, 50, 20, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5402, 2020, 0, 10, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5402, 242, 0, 50, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5403, 241, 0, 50, 60, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5403, 2020, 0, 40, 30, 0)");

        // Bulk versions (100 ingredient -> 5, same 20:1 as vanilla) of Shaped Granite / Marble Plate, added 2026-09-21 (see note #6).
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5404,'Shaped Granite (bulk)','',2020,17,0,271,10,5,1,0,'art/2D/Items/shaped_granite.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5405,'Marble Plate (bulk)','',2020,17,0,270,10,5,1,0,'art/2D/Items/marble_plate.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5404, 243, 0, 90, 100, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5404, 2020, 0, 30, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5405, 242, 0, 90, 100, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5405, 2020, 0, 30, 20, 0)");
    }
};
activatePackage(LiFxStonemasonsWorkplace);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxStonemasonsWorkplace);
