/**
* <author>GreedyFox</author>
* <description>Ports the MMO's Tailor's Workshop (object 2836) into Lif:YO via the LiFx ServerAutoloader framework.
*              Modeled directly on the pattern used by Log Cart (registerObjectsTypes + raw dbi.Update SQL for
*              the recipe) and Knool-Pack (registerObjectsTypes for a straight MMO->YO object port).
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*
* ============================ READ BEFORE USING ============================
* 1. ID COLLISION CHECK NOT DONE: object ID 2836, recipe ID 1625 and requirement IDs 5623-5627 are the MMO's
*    own IDs. They are free against Knool-Pack (2394-2399) and Log Cart (3016/1093) on this machine, but I have
*    NOT checked them against every mod under F:\Lif Mods, or against the community registry the other mods
*    point to (https://www.lifxmod.com/info/object-id-list/). Reserve/verify there before running this on a
*    real world.
* 2. ART ASSETS ARE MISSING: FaceImage/ImagePath below still point at the ORIGINAL MMO paths
*    (art\2D\Objects\tailors_workshop.png, art\2D\Recipes\tailors_workshop.png) and the object's 3D model
*    (art/models/3d/construction/craftingbonus/leather_workshop/leather_workshop*.dts, per cm_objects.xml)
*    is not part of the staticData_extracted dump you have on F:\Lif Datein. Until those files exist under this
*    mod's own folder (and the paths below are repointed at them, the way Log Cart points at
*    "yolauncher/modpack/mods/LiFx/LogCart/2D/..."), the workshop will register in the database but will be
*    invisible / fail to place in-game.
* 3. NO DATABLOCK YET: Log Cart wires its physical/collision behavior via a separate exec()'d datablock script
*    (Transport.cs). This object doesn't have one here - Datablock() is a stub. Without it the object may not
*    behave correctly as a placeable structure (state transitions Incomplete->Damaged->Complete etc.).
* 4A. `LiFx::registerObjectsTypes()` BELOW DOES NOT ACTUALLY CREATE THE OBJECT — found 2026-09-15 while
*    investigating why Log Cart's "installed" object/recipe don't show up live. Reading ServerAutoloader-4.3.0\
*    main.cs closely: registerObjectsTypes() only APPENDS its INSERT SQL to a file (sql/dump.sql); nothing
*    anywhere reads that file back and executes it. Confirmed empirically against the live DB: Log Cart's own
*    object 3016 doesn't exist despite being "installed". FIXED here by also inserting the object row via raw
*    dbi.Update() in dbChanges() below — the same proven-live mechanism the recipe/recipe_requirement rows
*    already use. The registerObjectsTypes() call in setup() is kept only for convention/in case some
*    undiscovered part of the pipeline consumes sql/dump.sql. See CLAUDE.md's top section for the full finding,
*    which also applies to the still-unresolved "is the LiFx framework even deployed on the live server"
*    question — deployment status has not been fixed by this change.
* 4. THE MMO-STYLE ABILITY GATE ("Sew Armor" id 282, extending object_type_id "1394"->"1394 2836") IS CONFIRMED
*    IMPOSSIBLE, NOT JUST UNIMPLEMENTED — investigated to a firm conclusion 2026-09-15 (see
*    StonemasonsWorkplacePort note #4/#5 for the full writeup). Abilities are one compiled C++ class per ID
*    (311 total, RTTI-confirmed via prior LiFx/Plus RE), not XML/DB-driven — live-tested by actually adding a
*    new ability ID to the deployed server's data/skill_types.xml and getting "using of unregistered ability".
*    HOWEVER — unlike Stonemason's Workplace/Big Tanning Tub, this one's gate shape is "extend an EXISTING
*    ability's object list" (1394->1394 2836), not "invent a new ability" — ability 282 "Sew Armor" (or its live
*    YO equivalent, may have a different ID, needs checking) is presumably ALREADY REGISTERED in the engine.
*    Extending an already-registered ability (confirmed viable via the PlowCartTest fix, ability id 71, and
*    "Run Millstones" id 262) is a GENUINELY OPEN option for this workshop specifically that wasn't available
*    for the other two — not attempted yet, would need: (a) finding "Sew Armor"'s real ID in the LIVE
*    data/skill_types.xml (search by name, not by assuming MMO's id 282 carries over — recall skill_type IDs
*    don't reliably match between MMO and YO, ability IDs likely don't either), (b) extending its
*    object_type_id list to include 2836, same edit pattern as the PlowCartTest fix.
* 4A. SOLVED A DIFFERENT WAY FOR NOW (same as the other two workshops): recipe-level StartingToolsID
*    station-gating, the mechanism vanilla YO already uses for Kiln/Furnace/Forge and Anvil/Tailor's Bench
*    itself. See the new bulk recipe (ID 5022) in dbChanges() below, modeled on vanilla's own "Leather strips"
*    recipe (ID 904, which requires the Tailor's Bench object 1394 as StartingToolsID) but gated on this
*    workshop (2836) instead, output scaled up for bulk production.
* =============================================================================
* 10. WORKSHOP-CRAFTED CLOTHING + ARMOR (2026-09-19, live-DB verified; in-game test pending): recipes 5100-5164 are clones of
*    all 65 Tailoring recipes (skill 25) that used the Weaver's Toolkit (295: 17 clothing/container/banner recipes) or the
*    vanilla Tailor's Bench (1394: 48 padded + leather armor recipes). Each clone has StartingToolsID=2836 and its tool row
*    (295 or 1394) replaced by the device row 2836 with DOUBLED Influence (same Quantity); every other requirement row is
*    unchanged. The menu entry is the existing "Sew Armor" ability 282 (Tailoring row 25, already gated to "1394 2836").
*    Originals (216-226, 616-619, 890, 927, 928, 1086, 369-416) are untouched. id_map = order of ascending original ID.
*    Also in art/dump.sql (see note #9 of JewelersWorkshopPort) and the client's recipe.xml / recipe_requirement.xml (ids 90100+).
*    Metal armor (Armorsmithing, Forge and Anvil) is deliberately NOT included: it needs a forge, not a sewing bench.
**/

if (!isObject(LiFxTailorsWorkshop))
{
    new ScriptObject(LiFxTailorsWorkshop)
    {
    };
}

package LiFxTailorsWorkshop
{
    function LiFxTailorsWorkshop::setup() {
        LiFx::registerCallback($LiFx::hooks::onServerCreatedCallbacks, Datablock, LiFxTailorsWorkshop);
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxTailorsWorkshop);

        // Register the new object (building) itself. NOTE: per caveat #4A, this call alone does NOT reach
        // the live DB — kept for convention, but dbChanges() below does the real INSERT via dbi.Update().
        LiFx::registerObjectsTypes(LiFxTailorsWorkshop::ObjectsTypesTailorsWorkshop(), LiFxTailorsWorkshop);
    }

    function LiFxTailorsWorkshop::version() {
        return "0.1.0-draft";
    }

    function LiFxTailorsWorkshop::Datablock() {
        // TODO: exec a datablock script here once you have one, e.g.:
        // exec("yolauncher/modpack/mods/LiFx/TailorsWorkshop/art/datablocks/TailorsWorkshop.cs");
    }

    // Straight port of the MMO's objects_types row for ID 2836 ("Tailors Workshop"),
    // ParentID 454 (same parent category as the Jeweler's Workshop in MMO data).
    function LiFxTailorsWorkshop::ObjectsTypesTailorsWorkshop() {
        return new ScriptObject(ObjectsTypesTailorsWorkshop : ObjectsTypes)
        {
            id = 2836; // VERIFY this is still free — see note #1 above
            ObjectName = "Tailors Workshop";
            ParentID = 454;
            IsContainer = 0;
            IsMovableObject = 0;
            IsUnmovableobject = 1;
            IsTool = 0;
            IsDevice = 1;
            IsDoor = 0;
            IsPremium = 0;
            MaxContSize = 100000;
            Length = 6;
            MaxStackSize = 0;
            UnitWeight = 200000;
            BackgrndImage = "";
            WorkAreaTop = 0;
            WorkAreaLeft = 0;
            WorkAreaWidth = 0;
            WorkAreaHeight = 0;
            BtnCloseTop = 0;
            BtnCloseLeft = 0;
            // TODO: repoint at this mod's own bundled copy of the art, e.g.
            // "yolauncher/modpack/mods/LiFx/TailorsWorkshop/2D/Objects/tailors_workshop.png"
            FaceImage = "art/2D/Objects/tailors_workshop.png";
            Description = "";
            BasePrice = 0;
            OwnerTimeout = 0;
            AllowExportFromRed = 0;
            AllowExportFromGreen = 0;
        };
    }

    // Recipe to build it, ported from the MMO's recipe.xml (ID 1625) + recipe_requirement.xml (5623-5627).
    // Using raw dbi.Update() like Log Cart does, NOT LiFx::registerRecipe() — the MMO's own Influence values
    // (30+15+15+10+10 = 80) plus SkillDepends (40) add up to 120, not the 100 that registerRecipe() enforces,
    // so that helper would reject this row outright. Rebalance the Influence/SkillDepends numbers to sum to
    // 100 if you want to use the validated helper instead; left as-is here to match the MMO source exactly.
    function LiFxTailorsWorkshop::dbChanges() {
        //////////////////////////////////////// Idempotency guard ////////////////////////////////////////
        // CRITICAL, learned 2026-09-17: recipe_requirement has no natural uniqueness constraint, and
        // art/dump.sql ALSO seeds these same rows on every boot now (see JewelersWorkshopPort note #9 for
        // the full incident) — without this DELETE, every boot doubles every material's Influence weight.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` IN (1625,5022,5100,5101,5102,5103,5104,5105,5106,5107,5108,5109,5110,5111,5112,5113,5114,5115,5116,5117,5118,5119,5120,5121,5122,5123,5124,5125,5126,5127,5128,5129,5130,5131,5132,5133,5134,5135,5136,5137,5138,5139,5140,5141,5142,5143,5144,5145,5146,5147,5148,5149,5150,5151,5152,5153,5154,5155,5156,5157,5158,5159,5160,5161,5162,5163,5164)");

        //////////////////////////////////////// Object ////////////////////////////////////////
        // (ID, ParentID, Name, IsContainer, IsMovableObject, IsUnmovableobject, IsTool, IsDevice, IsDoor,
        //  IsPremium, MaxContSize, Length, MaxStackSize, UnitWeight, BackgndImage, WorkAreaTop, WorkAreaLeft,
        //  WorkAreaWidth, WorkAreaHeight, BtnCloseTop, BtnCloseLeft, FaceImage, Description, BasePrice,
        //  OwnerTimeout, AllowExportFromRed, AllowExportFromGreen) — see caveat #4A for why this is here
        // instead of relying on LiFx::registerObjectsTypes() alone.
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2836,454,'Tailors Workshop',0,0,1,0,1,0,0,100000,6,0,200000,'',0,0,0,0,0,0,'art/2D/Objects/tailors_workshop.png','',0,0,0,0)");

        //////////////////////////////////////// Recipe ////////////////////////////////////////
        // (RecipeID, Name, Description, StartingToolsID, SkillTypeID, SkillLvl, ResultObjectTypeID,
        //  SkillDepends, Quantity, Autorepeat, IsBlueprint, ImagePath)
        // SkillTypeID=19 ("Masonry") is correct as-is, straight from the MMO's own recipe.xml row (ID 1625) —
        // this is the skill needed to BUILD the structure, not the Tailoring skill the workshop unlocks once
        // built (that's a separate association, on the ability gate, see note #4). 2026-09-15 correction: this
        // was briefly "fixed" to 25 (Tailoring) under the mistaken assumption that MMO/YO skill_type IDs don't
        // match — they DO match exactly (verified: MMO's own skill_types.xml has 18=Construction, 19=Masonry,
        // 23=Procuration, 25=Tailoring, identical to the live YO DB). Reverted back to the correct value, 19.
        // StartingToolsID=32 (Hammer) — 2026-09-17 fix, was NULL, see BigTanningTubPort for the full note.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1625,'Tailor\\'s Workshop','',32,19,90,2836,40,1,0,0,'art/2D/Recipes/tailors_workshop.png')");

        //////////////////////////////////// Recipe Requirements ////////////////////////////////////
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1625, 32, 0, 30, 20, 0)");  // 20 x Hammer
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1625, 235, 0, 15, 40, 1)");  // 40 x Boards (region item)
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1625, 324, 0, 15, 10, 0)");  // 10 x Hardwood Billet
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1625, 281, 0, 10, 80, 0)");  // 80 x Nails
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1625, 233, 0, 10, 20, 0)");  // 20 x Building Log

        //////////////////////////////////// Ability gate (NOT INCLUDED — see note #4) ////////////////////////////////////
        // Not attempted via skill_types.xml yet — this is the one workshop where extend-existing (not
        // invent-new) is genuinely viable, but finding "Sew Armor"'s real live ability ID and object list needs
        // doing first. See note #4 for the plan.

        //////////////////////////////////// "Mass production" recipe (see note #4A) ////////////////////////////////////
        // Station-gated via StartingToolsID=2836, same mechanism vanilla YO uses for Kiln/Furnace/Forge and
        // Anvil/Stonemason's Workplace/Big Tanning Tub. Modeled on vanilla's own "Leather strips" recipe (ID
        // 904: StartingToolsID=1394 Tailor's Bench, 1x Thick Leather + 30 durability off the bench -> 3x
        // Leather strips, SkillTypeID=25 Tailoring, SkillLvl=0, SkillDepends=20, Autorepeat=1). This version:
        // 5x Thick Leather -> 15x Leather strips per craft (5x scale on both sides), bench-equivalent wear left
        // at the original 30 (one wear-cost per batch, matching the pattern used for the other two workshops).
        // (RecipeID, Name, Description, StartingToolsID, SkillTypeID, SkillLvl, ResultObjectTypeID,
        //  SkillDepends, Quantity, Autorepeat, IsBlueprint, ImagePath)
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5022,'Leather Strips (Tailor\\'s Workshop)','',2836,25,0,1393,20,15,1,0,'art/2D/Items/Leather_strips.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5022, 424, 0, 70, 5, 0)");   // 5 x Thick Leather
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5022, 2836, 0, 10, 30, 0)"); // 30 durability off the Tailor's Workshop itself

        //////////////////////////////////// Workshop-crafted clothing + armor (5100-5164) - see note #10 ////////////////////////////////////
        // Clones of the 17 Weaver's-Toolkit clothing/container recipes and the 48 Tailor's-Bench armor recipes (skill 25),
        // StartingToolsID=2836 and the tool row (295 or 1394) replaced by the device row 2836 with DOUBLED Influence.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5100,'Rags','',2836,25,0,297,50,1,0,0,'art/2D/Items/rags_eur.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5101,'Monk\\'s outfit','',2836,25,60,298,50,1,0,0,'art/2D/Items/monks_outfit.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5102,'Simple Clothing','',2836,25,30,299,50,1,0,0,'art/2D/Items/simple_clothes.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5103,'Decorated Clothing','',2836,25,60,301,50,1,0,0,'art/2D/Items/decorated_clothes.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5104,'Blacksmith\\'s outfit','',2836,25,60,303,50,1,0,0,'art/2D/Items/blacksmiths_outfit.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5105,'Carpenter\\'s outfit','',2836,25,60,304,50,1,0,0,'art/2D/Items/carpenters_outfit.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5106,'Alchemist\\'s Outfit','',2836,25,60,305,50,1,0,0,'art/2D/Items/alchemists_outfit.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5107,'Engineer\\'s outfit','',2836,25,60,306,50,1,0,0,'art/2D/Items/engineers_outfit.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5108,'Cook\\'s outfit','',2836,25,60,307,50,1,0,0,'art/2D/Items/cooks_outfit.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5109,'Novice Padded Helm','',2836,25,0,885,30,1,0,0,'art/2D/Items/novice_padded_helm.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5110,'Novice Padded Breastplate','',2836,25,0,886,30,1,0,0,'art/2D/Items/novice_padded_breastplate.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5111,'Novice Padded Vambraces','',2836,25,0,887,30,1,0,0,'art/2D/Items/novice_padded_vambraces.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5112,'Novice Padded Gauntlets','',2836,25,0,888,30,1,0,0,'art/2D/Items/novice_padded_gauntlets.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5113,'Novice Padded Leggings','',2836,25,0,889,30,1,0,0,'art/2D/Items/novice_padded_leggings.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5114,'Novice Padded Greaves','',2836,25,0,890,30,1,0,0,'art/2D/Items/novice_padded_greaves.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5115,'Regular Padded Helm','',2836,25,30,891,30,1,0,0,'art/2D/Items/regular_padded_helm.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5116,'Regular Padded Tunic','',2836,25,30,892,30,1,0,0,'art/2D/Items/regular_padded_breastplate.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5117,'Regular Padded Vambraces','',2836,25,30,893,30,1,0,0,'art/2D/Items/regular_padded_vambraces.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5118,'Regular Padded Gauntlets','',2836,25,30,894,30,1,0,0,'art/2D/Items/regular_padded_gauntlets.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5119,'Regular Padded Leggings','',2836,25,30,895,30,1,0,0,'art/2D/Items/regular_padded_leggings.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5120,'Regular Padded Greaves','',2836,25,30,896,30,1,0,0,'art/2D/Items/regular_padded_greaves.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5121,'Heavy Padded Helm','',2836,25,90,897,30,1,0,1,'art/2D/Items/heavy_padded_helm.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5122,'Heavy Padded Tunic','',2836,25,90,898,30,1,0,1,'art/2D/Items/heavy_padded_breastplate.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5123,'Heavy Padded Vambraces','',2836,25,90,899,30,1,0,1,'art/2D/Items/heavy_padded_vambraces.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5124,'Heavy Padded Gauntlets','',2836,25,90,900,30,1,0,1,'art/2D/Items/heavy_padded_gauntlets.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5125,'Heavy Padded Leggings','',2836,25,90,901,30,1,0,1,'art/2D/Items/heavy_padded_leggings.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5126,'Heavy Padded Greaves','',2836,25,90,902,30,1,0,1,'art/2D/Items/heavy_padded_greaves.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5127,'Royal Padded Helm','',2836,25,100,903,30,1,0,1,'art/2D/Items/royal_padded_helm.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5128,'Royal Padded Tunic','',2836,25,100,904,30,1,0,1,'art/2D/Items/royal_padded_brestplate.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5129,'Royal Padded Vambraces','',2836,25,100,905,30,1,0,1,'art/2D/Items/royal_padded_vambraces.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5130,'Royal Padded Gauntlets','',2836,25,100,906,30,1,0,1,'art/2D/Items/royal_padded_gauntlets.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5131,'Royal Padded Leggings','',2836,25,100,907,30,1,0,1,'art/2D/Items/royal_padded_leggings.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5132,'Royal Padded Greaves','',2836,25,100,908,30,1,0,1,'art/2D/Items/royal_padded_greaves.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5133,'Novice Leather Helm','',2836,25,30,857,30,1,0,0,'art/2D/Items/novice_leather_helm.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5134,'Novice Leather Breastplate','',2836,25,30,858,30,1,0,0,'art/2D/Items/novice_leather_breastplate.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5135,'Novice Leather Vambraces','',2836,25,30,859,30,1,0,0,'art/2D/Items/novice_leather_vambraces.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5136,'Novice Leather Gauntlets','',2836,25,30,860,30,1,0,0,'art/2D/Items/novice_leather_gauntlets.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5137,'Novice Leather Leggings','',2836,25,30,861,30,1,0,0,'art/2D/Items/novice_leather_leggings.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5138,'Novice Leather Greaves','',2836,25,30,862,30,1,0,0,'art/2D/Items/novice_leather_greaves.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5139,'Regular Leather Helm','',2836,25,60,863,30,1,0,0,'art/2D/Items/regular_leather_helm.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5140,'Regular Leather Breastplate','',2836,25,60,864,30,1,0,0,'art/2D/Items/regular_leather_breastplate.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5141,'Regular Leather Vambraces','',2836,25,60,865,30,1,0,0,'art/2D/Items/regular_leather_vambraces.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5142,'Regular Leather Gauntlets','',2836,25,60,866,30,1,0,0,'art/2D/Items/regular_leather_gauntlets.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5143,'Regular Leather Leggings','',2836,25,60,867,30,1,0,0,'art/2D/Items/regular_leather_leggings.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5144,'Regular Leather Greaves','',2836,25,60,868,30,1,0,0,'art/2D/Items/regular_leather_greaves.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5145,'Heavy Leather Helm','',2836,25,90,869,30,1,0,1,'art/2D/Items/heavy_leather_helm.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5146,'Heavy Leather Breastplate','',2836,25,90,870,30,1,0,1,'art/2D/Items/heavy_leather_breastplate.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5147,'Heavy Leather Vambraces','',2836,25,90,871,30,1,0,1,'art/2D/Items/heavy_leather_vambraces.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5148,'Heavy Leather Gauntlets','',2836,25,90,872,30,1,0,1,'art/2D/Items/heavy_leather_gauntlets.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5149,'Heavy Leather Leggings','',2836,25,90,873,30,1,0,1,'art/2D/Items/heavy_leather_leggings.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5150,'Heavy Leather Greaves','',2836,25,90,874,30,1,0,1,'art/2D/Items/heavy_leather_greaves.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5151,'Royal Leather Helm','',2836,25,100,875,30,1,0,1,'art/2D/Items/royal_leather_helm.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5152,'Royal Leather Breastplate','',2836,25,100,876,30,1,0,1,'art/2D/Items/royal_leather_brestplate.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5153,'Royal Leather Vambraces','',2836,25,100,877,30,1,0,1,'art/2D/Items/royal_leather_vambraces.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5154,'Royal Leather Gauntlets','',2836,25,100,878,30,1,0,1,'art/2D/Items/royal_leather_gauntlets.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5155,'Royal Leather Leggings','',2836,25,100,879,30,1,0,1,'art/2D/Items/royal_leather_leggings.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5156,'Royal Leather Greaves','',2836,25,100,880,30,1,0,1,'art/2D/Items/royal_leather_greaves.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5157,'Backpack','',2836,25,90,458,25,1,0,0,'art/2D/Items/backpack.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5158,'Sack','',2836,25,60,459,25,1,0,0,'art/2D/Items/sack.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5159,'Pouch','',2836,25,0,461,25,1,0,0,'art/2D/Items/pouch.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5160,'Bag','',2836,25,30,460,25,1,0,0,'art/2D/Items/bag.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5161,'Tabard','',2836,25,0,1376,50,1,0,0,'art/2D/Items/tabard.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5162,'Steppe Rags','',2836,25,0,1138,50,1,0,0,'art/2D/Items/rags_mon.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5163,'North Rags','',2836,25,0,1137,50,1,0,0,'art/2D/Items/rags_vik.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5164,'Banner','',2836,25,0,2004,50,1,0,0,'art/2D/Items/flag_game_12.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5100, 1477, 0, 40, 6, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5100, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5101, 261, 0, 15, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5101, 260, 0, 15, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5101, 425, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5101, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5102, 261, 0, 15, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5102, 260, 0, 15, 6, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5102, 425, 0, 10, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5102, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5103, 264, 0, 10, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5103, 263, 0, 10, 6, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5103, 266, 0, 10, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5103, 424, 0, 10, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5103, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5104, 261, 0, 10, 4, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5104, 260, 0, 10, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5104, 424, 0, 20, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5104, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5105, 261, 0, 10, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5105, 260, 0, 10, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5105, 266, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5105, 424, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5105, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5106, 261, 0, 15, 6, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5106, 260, 0, 15, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5106, 425, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5106, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5107, 261, 0, 10, 6, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5107, 260, 0, 10, 6, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5107, 266, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5107, 424, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5107, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5108, 261, 0, 15, 6, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5108, 260, 0, 15, 6, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5108, 424, 0, 10, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5108, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5109, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5109, 1477, 0, 60, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5110, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5110, 1477, 0, 50, 4, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5110, 1393, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5111, 2836, 0, 40, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5111, 1477, 0, 50, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5112, 2836, 0, 40, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5112, 1477, 0, 50, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5113, 2836, 0, 40, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5113, 1477, 0, 50, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5114, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5114, 1477, 0, 50, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5114, 1393, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5115, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5115, 261, 0, 40, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5115, 260, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5116, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5116, 261, 0, 40, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5116, 260, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5116, 1393, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5117, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5117, 261, 0, 40, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5117, 260, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5118, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5118, 261, 0, 40, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5118, 260, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5119, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5119, 261, 0, 40, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5119, 260, 0, 20, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5120, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5120, 261, 0, 40, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5120, 260, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5120, 1393, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5121, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5121, 261, 0, 25, 1, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5121, 265, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5121, 1390, 0, 25, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5122, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5122, 261, 0, 40, 4, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5122, 265, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5122, 1393, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5123, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5123, 261, 0, 40, 3, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5123, 265, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5123, 1393, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5124, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5124, 261, 0, 40, 1, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5124, 265, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5125, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5125, 261, 0, 40, 3, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5125, 265, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5126, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5126, 261, 0, 40, 1, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5126, 1393, 0, 20, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5127, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5127, 261, 0, 25, 1, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5127, 263, 0, 5, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5127, 265, 0, 5, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5127, 1390, 0, 25, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5128, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5128, 261, 0, 40, 4, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5128, 263, 0, 5, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5128, 265, 0, 5, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5128, 1393, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5129, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5129, 261, 0, 40, 3, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5129, 263, 0, 5, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5129, 265, 0, 5, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5129, 1393, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5130, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5130, 261, 0, 40, 1, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5130, 263, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5130, 265, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5131, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5131, 261, 0, 40, 3, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5131, 263, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5131, 265, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5132, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5132, 261, 0, 40, 1, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5132, 263, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5132, 1393, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5133, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5133, 1477, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5133, 1393, 0, 40, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5134, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5134, 1477, 0, 20, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5134, 425, 0, 40, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5135, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5135, 1477, 0, 20, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5135, 425, 0, 40, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5136, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5136, 1477, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5136, 425, 0, 40, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5137, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5137, 1477, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5137, 425, 0, 40, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5138, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5138, 1477, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5138, 425, 0, 40, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5139, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5139, 425, 0, 50, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5139, 1393, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5140, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5140, 424, 0, 30, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5140, 1393, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5140, 260, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5140, 261, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5141, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5141, 425, 0, 40, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5141, 260, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5141, 261, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5142, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5142, 425, 0, 50, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5142, 260, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5143, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5143, 425, 0, 40, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5143, 260, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5143, 261, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5144, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5144, 425, 0, 30, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5144, 424, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5144, 260, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5145, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5145, 425, 0, 30, 1, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5145, 263, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5145, 1390, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5146, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5146, 424, 0, 30, 3, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5146, 1393, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5146, 263, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5146, 261, 0, 10, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5147, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5147, 425, 0, 30, 2, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5147, 263, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5147, 261, 0, 20, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5148, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5148, 425, 0, 50, 2, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5148, 263, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5149, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5149, 425, 0, 30, 2, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5149, 263, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5149, 261, 0, 20, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5150, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5150, 425, 0, 30, 1, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5150, 424, 0, 20, 2, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5150, 263, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5151, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5151, 424, 0, 30, 1, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5151, 263, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5151, 1390, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5152, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5152, 424, 0, 30, 4, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5152, 264, 0, 5, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5152, 263, 0, 5, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5152, 261, 0, 10, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5152, 1390, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5153, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5153, 424, 0, 30, 2, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5153, 263, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5153, 261, 0, 20, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5154, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5154, 425, 0, 50, 2, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5154, 263, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5155, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5155, 425, 0, 30, 2, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5155, 263, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5155, 261, 0, 20, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5156, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5156, 425, 0, 30, 1, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5156, 424, 0, 20, 2, 1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5156, 263, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5157, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5157, 424, 0, 40, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5157, 260, 0, 25, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5158, 262, 0, 25, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5158, 261, 0, 40, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5158, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5159, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5159, 425, 0, 65, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5160, 2836, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5160, 425, 0, 65, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5161, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5161, 261, 0, 40, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5162, 1477, 0, 40, 6, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5162, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5163, 1477, 0, 40, 6, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5163, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5164, 2836, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5164, 261, 0, 30, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5164, 237, 0, 10, 2, 0)");
    }
};
activatePackage(LiFxTailorsWorkshop);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxTailorsWorkshop);
