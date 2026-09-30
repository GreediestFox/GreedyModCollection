/**
* <author>GreedyFox</author>
* <description>Ports the MMO's Big Drying Frame (object 2829) into Lif:YO via the LiFx ServerAutoloader framework.
*              Follows the exact pattern established for the prior four workshops: raw dbi.Update() for
*              object+recipe, forward-slash paths, StartingToolsID-gated bulk recipe. UNLIKE Stonemason's
*              Workplace/Big Tanning Tub/Ore Washer, this one's ability gate IS live — see note #4.
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*
* ============================ READ BEFORE USING ============================
* 1. ID COLLISION: object ID 2829 and recipe ID 1618 verified free against the live DB 2026-09-16.
*    recipe_requirement uses NULL/auto-increment, not the MMO's own IDs (5587-5591). Still NOT checked against
*    https://www.lifxmod.com/info/object-id-list/.
* 2. ART ASSETS ARE GENUINELY ABSENT FROM THIS MACHINE'S MMO CLIENT — checked thoroughly 2026-09-16, unlike
*    Stonemason's Workplace/Big Tanning Tub/Ore Washer (all of which DID have real art available and deployed).
*    No `bigdrying2x2` folder under the MMO client's loose art/models/3d/construction/craftingdevices/ tree, and
*    a full QuickBMS extraction of art/shapes.cmpack (123/123 entries, confirmed complete) has no
*    dryingframe_2x2/bigdrying match either. No big_drying_frame.png icon in art/2D/Objects.cmpack or
*    Recipes.cmpack either (only the SMALL Drying Frame's icon, drying_frame.png, exists). Same situation as
*    Tailor's Workshop — this specific client build just doesn't ship this content. FaceImage/ImagePath below
*    still point at paths that don't exist yet (art/2D/Objects/big_drying_frame.png etc.) — the object will
*    register in the DB but be invisible / fail to place in-game until sourced from elsewhere.
* 3. NO DATABLOCK YET. cm_objects.xml (MMO source) shows a standard-ish state machine but hasn't been fully
*    re-checked here since there's no model to wire it to yet — do that when art is sourced.
* 4. THE MMO-STYLE ABILITY GATE IS LIVE AND WORKING — unlike Stonemason's Workplace/Big Tanning Tub/Ore Washer,
*    this workshop's gate is "extend an existing ability" (matching Tailor's Workshop's "Sew Armor" shape, not
*    the "invent new" shape). Checked live data/skill_types.xml 2026-09-16: "Dry a Hide" (id 84, state=complete)
*    and "Pick up a Hide" (id 115, state=working) are BOTH already registered at the SAME IDs as MMO, both
*    gating on object_type_id="118" (the small Drying Frame). Extended both to "118 2829" directly in the live
*    data/skill_types.xml (backup: data/skill_types.xml.bak_pre_bigdryingframe_extend) — confirmed via server
*    restart, clean boot, no ability-parsing errors. This means the REAL MMO interaction (drying a hide, then
*    picking it up once dried) works for this workshop, not just the recipe-based bulk-production substitute
*    below. This was NOT done via dbi.Update() / TorqueScript — it's a live server data file edit, done
*    manually alongside deploying this mod, and needs to be REDONE if the live skill_types.xml is ever reset
*    or restored from an earlier backup.
* 5. THE BULK "MASS PRODUCTION" RECIPE BELOW IS A REAL PORT, not invented — vanilla YO already has "Thick dried
*    hide" (recipe 242) gated on the SMALL Drying Frame (StartingToolsID=118): 1x Thick Hides (region item) + 15
*    durability off the frame -> 1x Thick Dried Hide, SkillTypeID=23 Procuration, SkillLvl=30, SkillDepends=15.
*    This version just redirects StartingToolsID to 2829 and scales output 5x, following the same convention as
*    the other three "mass production" recipes in this project. Nicely, this closes a real material chain: Big
*    Drying Frame -> Thick Dried Hide -> (feeds directly into) Big Tanning Tub's own bulk recipe (5021) -> Thick
*    Leather -> Tailor's Workshop's bulk recipe (5022) -> Leather Strips.
* =============================================================================
*/

if (!isObject(LiFxBigDryingFrame))
{
    new ScriptObject(LiFxBigDryingFrame)
    {
    };
}

package LiFxBigDryingFrame
{
    function LiFxBigDryingFrame::setup() {
        LiFx::registerCallback($LiFx::hooks::onServerCreatedCallbacks, Datablock, LiFxBigDryingFrame);
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxBigDryingFrame);

        // Register the new object (building) itself. NOTE: per StonemasonsWorkplacePort note #0, this call
        // alone does NOT reach the live DB — kept for convention, but dbChanges() below does the real INSERT.
        LiFx::registerObjectsTypes(LiFxBigDryingFrame::ObjectsTypesBigDryingFrame(), LiFxBigDryingFrame);
    }

    function LiFxBigDryingFrame::version() {
        return "0.1.0-draft";
    }

    function LiFxBigDryingFrame::Datablock() {
        // TODO: exec a datablock script here once art (model) is sourced — see note #2/#3 above.
        // exec("yolauncher/modpack/mods/LiFx/BigDryingFrame/art/datablocks/BigDryingFrame.cs");
    }

    // Straight port of the MMO's objects_types row for ID 2829 ("Big Drying Frame"), ParentID 64 ("Crafting").
    function LiFxBigDryingFrame::ObjectsTypesBigDryingFrame() {
        return new ScriptObject(ObjectsTypesBigDryingFrame : ObjectsTypes)
        {
            id = 2829; // verified free against the live DB 2026-09-16 — see note #1 above
            ObjectName = "Big Drying Frame";
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
            UnitWeight = 50000;
            BackgrndImage = "";
            WorkAreaTop = 0;
            WorkAreaLeft = 0;
            WorkAreaWidth = 0;
            WorkAreaHeight = 0;
            BtnCloseTop = 0;
            BtnCloseLeft = 0;
            // TODO: repoint once art exists — see note #2 above
            FaceImage = "art/2D/Objects/big_drying_frame.png";
            Description = "";
            BasePrice = 0;
            OwnerTimeout = 0;
            AllowExportFromRed = 0;
            AllowExportFromGreen = 0;
        };
    }

    function LiFxBigDryingFrame::dbChanges() {
        //////////////////////////////////////// Idempotency guard ////////////////////////////////////////
        // CRITICAL, learned 2026-09-17: recipe_requirement has no natural uniqueness constraint, and
        // art/dump.sql ALSO seeds these same rows on every boot now (see JewelersWorkshopPort note #9 for
        // the full incident) — without this DELETE, every boot doubles every material's Influence weight.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` IN (1618,5024)");

        //////////////////////////////////////// Object ////////////////////////////////////////
        // (ID, ParentID, Name, IsContainer, IsMovableObject, IsUnmovableobject, IsTool, IsDevice, IsDoor,
        //  IsPremium, MaxContSize, Length, MaxStackSize, UnitWeight, BackgndImage, WorkAreaTop, WorkAreaLeft,
        //  WorkAreaWidth, WorkAreaHeight, BtnCloseTop, BtnCloseLeft, FaceImage, Description, BasePrice,
        //  OwnerTimeout, AllowExportFromRed, AllowExportFromGreen)
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2829,64,'Big Drying Frame',1,0,1,0,1,0,0,300000,6,0,50000,'',0,0,0,0,0,0,'art/2D/Objects/big_drying_frame.png','',0,0,0,0)");

        //////////////////////////////////////// Recipe (build the structure) ////////////////////////////////////////
        // Ported from the MMO's recipe.xml (ID 1618) + recipe_requirement.xml (rows for RecipeID 1618).
        // SkillTypeID=18 ("Construction") straight from the MMO's own recipe.xml — matches YO's live value.
        // Influence sum (20+10+10+10+30=80) + SkillDepends (40) = 120 != 100, using raw dbi.Update() (same
        // overflow pattern as Tailor's Workshop/Big Tanning Tub/Jeweler's Workshop's build recipes).
        // (RecipeID, Name, Description, StartingToolsID, SkillTypeID, SkillLvl, ResultObjectTypeID,
        //  SkillDepends, Quantity, Autorepeat, IsBlueprint, ImagePath)
        // StartingToolsID=32 (Hammer) — 2026-09-17 fix, was NULL, see BigTanningTubPort for the full note.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1618,'Big Drying Frame','',32,18,60,2829,40,1,0,0,'art/2D/Recipes/big_drying_frame.png')");

        //////////////////////////////////// Recipe Requirements (build) ////////////////////////////////////
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1618, 235, 0, 20, 30, 0)");  // 30 x Boards
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1618, 282, 0, 10, 3, 0)");   // 3 x Metal Band
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1618, 262, 0, 10, 2, 0)");   // 2 x Linen Rope
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1618, 233, 0, 10, 3, 0)");   // 3 x Building Log
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1618, 32, 0, 30, 20, 0)");   // 20 x Hammer

        //////////////////////////////////// Ability gate — LIVE, see note #4 (done manually in skill_types.xml) ////////////////////////////////////
        // Not a dbi.Update() — "Dry a Hide" (id 84) and "Pick up a Hide" (id 115) were extended directly in the
        // live server's data/skill_types.xml alongside deploying this mod. Nothing to do here in script; see
        // note #4 for the full detail and what to redo if that file is ever reset.

        //////////////////////////////////// "Mass production" recipe (PORTED, see note #5) ////////////////////////////////////
        // Station-gated via StartingToolsID=2829. Ported from vanilla YO's own "Thick dried hide" recipe (242,
        // StartingToolsID=118 small Drying Frame): 5x Thick Hides -> 5x Thick Dried Hide per craft (5x scale on
        // input, output stays at the same 1:1 ratio as vanilla since vanilla's own ratio is already 1:1 and this
        // mod scales via Quantity), Autorepeat=1. SkillTypeID=23 Procuration, matches vanilla recipe 242 exactly.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5024,'Thick Dried Hide (Big Drying Frame)','',2829,23,30,474,15,5,1,0,'art/2D/Items/thick_dried_hide.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5024, 399, 0, 80, 5, 1)");   // 5 x Thick Hides (region item)
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5024, 2829, 0, 20, 15, 0)"); // 15 durability off the Big Drying Frame itself
    }
};
activatePackage(LiFxBigDryingFrame);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxBigDryingFrame);
