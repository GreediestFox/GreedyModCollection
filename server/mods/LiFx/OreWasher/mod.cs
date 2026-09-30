/**
* <author>GreedyFox</author>
* <description>Ports the MMO's Ore Washer (object 2021) into Lif:YO via the LiFx ServerAutoloader framework.
*              Follows the exact pattern established for Stonemason's Workplace/Big Tanning Tub/Tailor's
*              Workshop: raw dbi.Update() for object+recipe (registerObjectsTypes()/registerRecipe() are both
*              inert — see StonemasonsWorkplacePort note #0), forward-slash paths (see BigTanningTubPort note
*              on the path-corruption incident), StartingToolsID-gated bulk recipe instead of the ability system.
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*
* ============================ READ BEFORE USING ============================
* 1. ID COLLISION: object ID 2021 and recipe ID 1180 verified free against the live DB 2026-09-16.
*    recipe_requirement uses NULL/auto-increment, not the MMO's own IDs (3621/3623/3625/3627). Still NOT
*    checked against https://www.lifxmod.com/info/object-id-list/.
* 2. ART ASSETS: unlike Tailor's Workshop, this one's art IS available and already deployed. Found in the real
*    LiF:MMO client install (E:\SteamLibrary\...\Life is Feudal MMO\game\eu\) 2026-09-15/16 — the .dts model was
*    loose, the 2D icons were extracted from art/2D/Objects.cmpack and art/2D/Recipes.cmpack via QuickBMS (see
*    reference_lifx_dev_env memory for the tool location/usage). Deployed to the live server's own
*    art/models/3d/construction/craftingdevices/ore_washer/ore_washer.dts, art/2D/Objects/ore_washer.png, and
*    art/2D/Recipes/ore_washer.png. A matching <object id="2021"> entry ALSO needs adding to the live server's
*    data/cm_objects.xml (mapping object ID -> model per state) — same as done for Stonemason's/Big Tanning Tub
*    — do this alongside deploying this mod.cs, not optional; the .dts existing on disk alone isn't enough.
* 3. NO DATABLOCK YET. cm_objects.xml shows a simple single-cell footprint (0,0) with standard
*    Incomplete/Damaged/Complete states (Incomplete uses the shared art/models/3d/construction/misc/site/site.dts
*    placeholder, already present on this server) — no unusual state machine like Big Tanning Tub's.
* 4. THE MMO-STYLE ABILITY GATE ("Wash Ore" id 369 menu + "Wash" id 373 action, Precious Prospecting skill,
*    object_type_id=2021 only) IS CONFIRMED IMPOSSIBLE, NOT JUST UNIMPLEMENTED — checked live
*    data/skill_types.xml for both ability names 2026-09-16, neither exists (unlike Tailor's Workshop's "Sew
*    Armor", which WAS already registered and got successfully extended). No extend-existing option available
*    for this workshop; same "invent a new ability class" wall as Stonemason's Workplace/Big Tanning Tub,
*    confirmed genuinely infeasible without engine source (see StonemasonsWorkplacePort note #4/#5).
* 5. THE "WASH ORE" PRODUCTION RECIPE BELOW IS DESIGNED, NOT PORTED — unlike the other three workshops' bulk
*    recipes (which all had a real vanilla-YO analogue to copy: Kiln/Furnace/Forge and Anvil/Tailor's Bench),
*    neither MMO nor YO has ANY existing recipe using the Ore Washer (or anything StartingToolsID-gated under
*    the Precious Prospecting skill) to model this on — MMO implements "washing ore" purely through its
*    (unported) ability system, with no recipe-table equivalent at all (checked MMO's own recipe.xml for
*    StartingToolsID=2021: zero rows). This recipe (5023, "Washed Gold Ore") is an invented design: wash bulk
*    Rock (241, plentiful/already-used-elsewhere material) into Gold Ore (330), themed around the real-world
*    "panning for gold" concept and the Precious Prospecting skill context the MMO ability used. Reconsider the
*    input/output materials and balance numbers if this doesn't feel right in practice — there's no source data
*    backing the specific numbers the way there was for the other three.
* =============================================================================
*/

if (!isObject(LiFxOreWasher))
{
    new ScriptObject(LiFxOreWasher)
    {
    };
}

package LiFxOreWasher
{
    function LiFxOreWasher::setup() {
        LiFx::registerCallback($LiFx::hooks::onServerCreatedCallbacks, Datablock, LiFxOreWasher);
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxOreWasher);

        // Register the new object (building) itself. NOTE: per StonemasonsWorkplacePort note #0, this call
        // alone does NOT reach the live DB — kept for convention, but dbChanges() below does the real INSERT.
        LiFx::registerObjectsTypes(LiFxOreWasher::ObjectsTypesOreWasher(), LiFxOreWasher);
    }

    function LiFxOreWasher::version() {
        return "0.1.0-draft";
    }

    function LiFxOreWasher::Datablock() {
        // TODO: exec a datablock script here once you have one, e.g.:
        // exec("yolauncher/modpack/mods/LiFx/OreWasher/art/datablocks/OreWasher.cs");
    }

    // Straight port of the MMO's objects_types row for ID 2021 ("Ore Washer"), ParentID 64 ("Crafting").
    function LiFxOreWasher::ObjectsTypesOreWasher() {
        return new ScriptObject(ObjectsTypesOreWasher : ObjectsTypes)
        {
            id = 2021; // verified free against the live DB 2026-09-16 — see note #1 above
            ObjectName = "Ore Washer";
            ParentID = 64;
            IsContainer = 1;
            IsMovableObject = 0;
            IsUnmovableobject = 1;
            IsTool = 0;
            IsDevice = 1;
            IsDoor = 0;
            IsPremium = 0;
            MaxContSize = 500000;
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
            FaceImage = "art/2D/Objects/ore_washer.png";
            Description = "";
            BasePrice = 0;
            OwnerTimeout = 0;
            AllowExportFromRed = 0;
            AllowExportFromGreen = 0;
        };
    }

    function LiFxOreWasher::dbChanges() {
        //////////////////////////////////////// Idempotency guard ////////////////////////////////////////
        // CRITICAL, learned 2026-09-17: recipe_requirement has no natural uniqueness constraint, and
        // art/dump.sql ALSO seeds these same rows on every boot now (see JewelersWorkshopPort note #9 for
        // the full incident) — without this DELETE, every boot doubles every material's Influence weight.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` IN (1180,5610,5611,5612,5613)");

        //////////////////////////////////////// Object ////////////////////////////////////////
        // (ID, ParentID, Name, IsContainer, IsMovableObject, IsUnmovableobject, IsTool, IsDevice, IsDoor,
        //  IsPremium, MaxContSize, Length, MaxStackSize, UnitWeight, BackgndImage, WorkAreaTop, WorkAreaLeft,
        //  WorkAreaWidth, WorkAreaHeight, BtnCloseTop, BtnCloseLeft, FaceImage, Description, BasePrice,
        //  OwnerTimeout, AllowExportFromRed, AllowExportFromGreen)
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2021,64,'Ore Washer',1,0,1,0,1,0,0,500000,10,0,10000,'',0,0,0,0,0,0,'art/2D/Objects/ore_washer.png','',0,0,0,0)");

        //////////////////////////////////////// Recipe (build the structure) ////////////////////////////////////////
        // Ported from the MMO's recipe.xml (ID 1180) + recipe_requirement.xml (rows 3621/3623/3625/3627).
        // SkillTypeID=18 ("Construction") straight from the MMO's own recipe.xml — matches YO's live value.
        // Influence sum (20+20+20+20=80) + SkillDepends (20) = 100 exactly, but using raw dbi.Update() anyway
        // for consistency with the rest of this project (registerRecipe() is inert either way, see
        // StonemasonsWorkplacePort note #0).
        // (RecipeID, Name, Description, StartingToolsID, SkillTypeID, SkillLvl, ResultObjectTypeID,
        //  SkillDepends, Quantity, Autorepeat, IsBlueprint, ImagePath)
        // StartingToolsID=32 (Hammer) — 2026-09-17 fix, was NULL, see BigTanningTubPort for the full note.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1180,'Ore Washer','',32,18,30,2021,20,1,0,0,'art/2D/Recipes/ore_washer.png')");

        //////////////////////////////////// Recipe Requirements (build) ////////////////////////////////////
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1180, 324, 0, 20, 5, 0)");   // 5 x Hardwood Billet
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1180, 235, 0, 20, 10, 0)");  // 10 x Boards
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1180, 281, 0, 20, 20, 0)");  // 20 x Nails
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1180, 32, 0, 20, 20, 0)");   // 20 x Hammer

        //////////////////////////////////// Ability gate (NOT INCLUDED — see note #4) ////////////////////////////////////
        // Confirmed genuinely infeasible without engine source — abilities are compiled C++ classes, not
        // data-driven, and no extend-existing option exists for this workshop (checked live skill_types.xml
        // for both "Wash Ore" and "Wash" by name — neither is registered). Do not attempt a dbi.Update() sketch
        // here or add new ability IDs to skill_types.xml.

        //////////////////////////////////// "Mass production" recipe (DESIGNED, not ported — see note #5) ////////////////////////////////////
        // Station-gated via StartingToolsID=2021, same mechanism vanilla YO uses for Kiln/Furnace/Forge and
        // Anvil/every other workshop in this project. 50x Rock -> 5x Gold Ore per craft, Autorepeat=1.
        // SkillTypeID=31 is YO's live "Precious Prospecting" — matches the MMO ability's skill context even
        // though this specific recipe has no MMO source to verify the ID against (see note #5).
        // REPLACED 2026-09-21: the old "Washed Gold Ore" recipe (5023, 50 Rock -> 5 Gold Ore, an economy break) is gone.
        // Ore Washer (2026-09-21): 100 ore of ANY quality -> 20 ore of the same type, after a 60 s progress bar (5610 Iron, 5611 Copper, 5612 Silver, 5613 Gold).
        // Output quality follows a logarithmic curve of the input quality (+20 at quality 0 falling to +1 at 90, capped at 90) - see herbGardenGate <gate abilityId="64"> in lifxpluss.xml.
        // Opens via the Sculpt ability (id 64, skill row 17): 2021 is in its object list in skill_types.xml. The bar length and quality curve come from the Plus hook (Ability::GetDuration override + _finallyMakeItem).
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5610,'Wash Iron Ore','',2021,17,0,328,0,20,0,0,'art/2D/Items/iron_ore.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5611,'Wash Copper Ore','',2021,17,0,329,0,20,0,0,'art/2D/Items/copper_ore.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5612,'Wash Silver Ore','',2021,17,0,331,0,20,0,0,'art/2D/Items/silver_ore.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5613,'Wash Gold Ore','',2021,17,0,330,0,20,0,0,'art/2D/Items/golden_ore.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5610, 328, 0, 99, 100, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5610, 2021, 0, 1, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5611, 329, 0, 99, 100, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5611, 2021, 0, 1, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5612, 331, 0, 99, 100, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5612, 2021, 0, 1, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5613, 330, 0, 99, 100, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5613, 2021, 0, 1, 20, 0)");
    }
};
activatePackage(LiFxOreWasher);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxOreWasher);
