/**
* <author>GreedyFox</author>
* <description>Ports the MMO's Jeweler's Workshop (object 2894) into Lif:YO via the LiFx ServerAutoloader
*              framework. Follows the pattern established for the prior six workshops. Notable: this is the
*              first workshop needing TWO ability edits (it's IsDoor=1, unlike every other workshop), and the
*              first one where an MMO build material (Amberwood Board) is genuinely absent from YO — see notes
*              #5 and #6.
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*
* ============================ READ BEFORE USING ============================
* 1. ID COLLISION: object ID 2894 and recipe ID 1704 verified free against the live DB 2026-09-16.
*    recipe_requirement uses NULL/auto-increment, not the MMO's own IDs. Still NOT checked against
*    https://www.lifxmod.com/info/object-id-list/.
* 2. ART: 2026-09-17 — confirmed AUTHORITATIVELY (via the MMO client's fully re-extracted staticData.dlpack,
*    which itself only exists after the client's 2026-09-15/16 update to a new packed format) that a
*    standalone "Jeweler's Workshop" building GENUINELY DOES NOT EXIST anywhere in the MMO's own game data —
*    not just missing loose files as suspected earlier. In the real MMO, jewelry crafting is purely
*    toolkit-based (the portable "Jeweler's Toolkit" item, object 527/1616 there); there is no building to
*    port art from at all. Per GreedyFox's explicit choice, this now uses a PLACEHOLDER: the vanilla YO
*    "Herbalist's Shop" (object 140)'s model/icon are reused directly (same 9-cell footprint, same door-capable
*    .dts, `art/models/3d/construction/craftingbonus/herbalistshop/herbalist_shop.dts`, no new files copied —
*    references the existing vanilla asset in place). FaceImage/ImagePath point at
*    `art/2D/Objects/herbalist_shop.png` / `art/2D/Recipes/herbalist_shop.png` (also vanilla, already present).
*    Not authentic MMO art, but fully placeable and visually complete — no missing-asset risk.
* 3. NO DATABLOCK — not needed, this object doesn't need one of its own now that it reuses Herbalist's Shop's
*    existing model wholesale (see note #2).
* 4. THIS WORKSHOP IS IsDoor=1 — UNLIKE EVERY OTHER WORKSHOP IN THIS PROJECT, it needs its own door ability
*    edit on top of the crafting function:
*    (a) "Create Jewelry" (id 201, Jewelry skill) — **2026-09-17/18: REVERTED to full vanilla, after two
*        sessions of trying to make it a complex_obj/station-gated ability failed to ever produce a working
*        in-game interaction (see project_lifx_workshop_porting.md's 2026-09-17 "continued" entry for the full
*        blow-by-blow: object identity, client/server sync, skill training, and a `type="Special"` attribute
*        were all checked and ruled out or found insufficient). Leading theory, NOT proven: ability IDs are
*        compiled C++ classes, and "Create Jewelry" may be a hard-coded inventory-item-triggered class (you
*        right-click the Jeweler's Toolkit, not the building) — editing its XML `<entities>` block only changes
*        REQUIREMENTS, not which UI surface the ability appears on, so a complex_obj-only entity may simply
*        never be reachable no matter how it's written. GreedyFox's explicit decision: stop chasing this, ability
*        201 is back to vanilla-only (inventory_item, object 527, no station requirement, no `type="Special"`)
*        — the original 14 jewelry recipes (245-258) are craftable anywhere with the toolkit again, exactly as
*        base YO always worked. Do NOT re-attempt a complex_obj entity on this ability without new evidence.
*    (b) "Open/Close Door" (id 144) — the generic shared door ability every other doored building uses.
*        Extended object_type_id list to include 2894 (purely additive, unaffected by the (a) revert above —
*        confirmed working live, "Open/Close Door" shows correctly on the built workshop).
*    Edit (b) is in the live data/skill_types.xml, NOT via dbi.Update()/TorqueScript — done manually alongside
*    deploying this mod. **CRITICAL, learned the hard way 2026-09-16: Steam's "verify integrity of game files"
*    on the dedicated server silently reverts ANY direct edit to data/skill_types.xml (and data/cm_objects.xml)
*    back to vanilla. If "Open/Close Door" ever mysteriously stops listing 2894 again, that's almost certainly
*    why — redo that one edit only; do NOT redo (a).**
* 5. MATERIAL SUBSTITUTION: the MMO's own recipe_requirement for the BUILD recipe calls for 30x "Amberwood
*    Board" (MMO object ID 2068), which does not exist in YO's objects_types table. Substituted plain
*    "Boards" (235) as the closest analogous material — a deliberate design deviation.
* 6. BUILD SKILL LOWERED 2026-09-17: originally ported at SkillLvl=60 straight-away per GreedyFox's explicit
*    request ("fully buildable over recipe at masonry 60") — the MMO's own value (90) was a placeholder from
*    the initial port, not something GreedyFox asked to preserve exactly. If this needs to change again, it's
*    recipe.SkillLvl for RecipeID=1704 only — nothing else depends on this value.
* 7. NEW 2026-09-17 — "WORKSHOP-CRAFTED" VARIANTS OF ALL 14 EXISTING JEWELRY RECIPES, GreedyFox's own explicit
*    design (not an MMO port, not the result of the Carpenter's-Shop quality-bonus RE investigation, which
*    never conclusively found the real mechanism — see project_lifx_workshop_porting.md's Carpenter's Shop
*    section for that unresolved thread). Per his instruction: "clone the recipes of the jewelry kit and
*    double the influence of the starting tool, in this case the new jewelry workshop... so we can skip the
*    buff searching and start full implementation." Concretely, for each of YO's 14 existing jewelry recipes
*    (245-258, StartingToolsID=527 the Jeweler's Toolkit, which contributes Influence=10 in every single one
*    of them — confirmed via live query, no exceptions), a new recipe (5025-5038) was created that is an exact
*    clone EXCEPT: StartingToolsID=2894 (Jeweler's Workshop) instead of 527, and the corresponding
*    recipe_requirement row's MaterialObjectTypeID changed from 527 to 2894 with Influence DOUBLED from 10 to
*    20. Every other requirement row (materials, gems) is unchanged. This is a self-contained, deliberately
*    simple placeholder mechanic — a literal design choice to stop chasing the real in-engine quality-bonus
*    formula and ship something working now. The original 14 recipes (245-258) are untouched and still work
*    exactly as before (toolkit-only, Influence=10, craftable anywhere the ability's requirements are met).
*    id_map for reference (old -> new): 245->5025, 246->5026, 247->5027, 248->5028, 249->5029, 250->5030,
*    251->5031, 252->5032, 253->5033, 254->5034, 255->5035, 256->5036, 257->5037, 258->5038.
*    **CORRECTED 2026-09-19:** the doubled row above is now MaterialObjectTypeID=527 (the Toolkit), NOT 2894,
*    and StartingToolsID is 527 too. Crafting via the Toolkit (craftWithTool) sends every requirement row
*    through the inventory check, so a 2894 (device) row made the game say "You need 30 Jeweler's Workshop in
*    your inventory" and the recipes could never be crafted. The workshop bonus is meant to come from a
*    server hook instead (see project_lifx_workshop_buff_re.md); until it exists these recipes are simply
*    "toolkit at double influence" and are NOT tied to the workshop building.
*    **SUPERSEDED 2026-09-19 (evening) - FINAL DESIGN, LIVE-VERIFIED:** 5025-5038 are BUILDING recipes again
*    (StartingToolsID=2894, first requirement row = device 2894, Influence 20, plain names, no "_JW"), crafted
*    at the Jeweler's Workshop itself through a "Craft Jewelry" menu entry. That entry is XML ability 316
*    (the unused quest ability "Forge" of Ulf's Forge 1543), which was moved under the Jewelry skill row (52)
*    in BOTH server and client data\skill_types.xml, renamed "Craft Jewelry", ent_req object_type_id 2894,
*    empty <requirements/>. Reason: a device-craft window lists recipes whose SkillTypeID equals the skill row
*    its ability is nested under; the earlier Sculpt (64) attempt showed an EMPTY window once the recipe skill
*    was 52. The device row is handled natively (Influence -> quality, Quantity -> building wear); the
*    Toolkit route (245-258) is untouched. The quality x1.2 workshop buff comes from the Detours hook
*    (config\lifxpluss.xml <workshopBuff> rule 2894, abilityIds="201 316"). skill_types.xml backups:
*    skill_types.xml.bak_pre_questforge_20260919. If skill_types.xml is ever restored, redo the move of 316.
* 8. SEQUENCING NOTE FOR FUTURE WORKSHOPS: always deploy a workshop's mod.cs (which creates its object via
*    dbi.Update()) before or alongside any skill_types.xml edit that references its new object ID, never
*    before — an earlier attempt at note #4(a) crashed the server with "unknown objTypeID" from getting this
*    order wrong.
* 9. **CRITICAL, learned 2026-09-16: every custom objects_types/recipe/recipe_requirement row in this mod.cs
*    MUST ALSO be appended to the live server's `art/dump.sql`, not just live here in dbChanges().** The
*    native per-boot DB-patch step (`CmServerInfoManager`) does `DELETE FROM objects_types;
*    DELETE FROM recipe; DELETE FROM recipe_requirement;` (+ several other tables) then reseeds strictly from
*    `art/dump.sql` BEFORE any LiFx mod code runs — so the moment this workshop (or ANY placed instance of an
*    object whose objects_types row isn't also in dump.sql) is placed in the world, the very next server
*    restart crashes on a foreign-key violation. All of this mod's rows (including the new 5025-5038 recipes
*    below) were appended to art/dump.sql on 2026-09-17 alongside deploying this update — do the same for any
*    future edit to this file. See project_lifx_workshop_porting.md's 2026-09-16 entries for the full incident.
* =============================================================================
*/

if (!isObject(LiFxJewelersWorkshop))
{
    new ScriptObject(LiFxJewelersWorkshop)
    {
    };
}

package LiFxJewelersWorkshop
{
    function LiFxJewelersWorkshop::setup() {
        LiFx::registerCallback($LiFx::hooks::onServerCreatedCallbacks, Datablock, LiFxJewelersWorkshop);
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxJewelersWorkshop);

        // Register the new object (building) itself. NOTE: per StonemasonsWorkplacePort note #0, this call
        // alone does NOT reach the live DB — kept for convention, but dbChanges() below does the real INSERT.
        LiFx::registerObjectsTypes(LiFxJewelersWorkshop::ObjectsTypesJewelersWorkshop(), LiFxJewelersWorkshop);
    }

    function LiFxJewelersWorkshop::version() {
        return "0.2.0-draft";
    }

    function LiFxJewelersWorkshop::Datablock() {
        // TODO: exec a datablock script here once art (model) is sourced — see note #2 above.
        // exec("yolauncher/modpack/mods/LiFx/JewelersWorkshop/art/datablocks/JewelersWorkshop.cs");
    }

    // Straight port of the MMO's objects_types row for ID 2894 ("Jeweler's Workshop"), ParentID 454 (same
    // parent category as Tailor's Workshop). IsDoor=1 — see note #4 above.
    function LiFxJewelersWorkshop::ObjectsTypesJewelersWorkshop() {
        return new ScriptObject(ObjectsTypesJewelersWorkshop : ObjectsTypes)
        {
            id = 2894; // verified free against the live DB 2026-09-16 — see note #1 above
            // Apostrophe deliberately dropped here (not in the live DB Name, only this ScriptObject property) --
            // 2026-09-18: registerObjectsTypes()'s file-write path (sql/dump.sql) does NOT escape apostrophes
            // when serializing ObjectName, and something (still unidentified) DOES read that file back and
            // apply it as a native DB patch, unlike the earlier "sql/dump.sql is inert" finding assumed. A raw
            // apostrophe here breaks that generated SQL and fatally crashes server boot
            // ("CmServerInfoManager::_applyDbPatch() - can't apply patch file 'sql/dump.sql'"). The real
            // in-game/DB object name ("Jeweler's Workshop", apostrophe intact) is set separately via
            // dbi.Update() below, art/dump.sql, and the client's objects_types.xml -- all already correctly
            // escaped/literal there. This property only feeds the buggy file-write path, so dropping the
            // apostrophe here has zero visible effect in-game.
            ObjectName = "Jewelers Workshop";
            ParentID = 454;
            IsContainer = 0;
            IsMovableObject = 0;
            IsUnmovableobject = 1;
            IsTool = 0;
            IsDevice = 1;
            IsDoor = 1;
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
            FaceImage = "art/2D/Objects/jeweler_workshop.png"; // real icon, found 2026-09-18 in the Arden client
            Description = "";
            BasePrice = 0;
            OwnerTimeout = 0;
            AllowExportFromRed = 0;
            AllowExportFromGreen = 0;
        };
    }

    function LiFxJewelersWorkshop::dbChanges() {
        //////////////////////////////////////// Idempotency guard ////////////////////////////////////////
        // CRITICAL, learned 2026-09-17: recipe_requirement has no natural uniqueness constraint (ID is a
        // bare auto-increment), and art/dump.sql ALSO now seeds these same rows on every boot (see note #9)
        // — without this DELETE, every boot would insert a second, third, fourth... copy of every
        // requirement row, silently doubling each material's Influence weight in the live quality formula.
        // Confirmed this had ALREADY happened project-wide (every custom recipe had exactly 2x its correct
        // row count) the first time this ran after art/dump.sql started carrying the same data. Deleting
        // first makes this function safe to run any number of times, regardless of what dump.sql already
        // restored.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` IN (1704,5025,5026,5027,5028,5029,5030,5031,5032,5033,5034,5035,5036,5037,5038)");

        //////////////////////////////////////// Object ////////////////////////////////////////
        // (ID, ParentID, Name, IsContainer, IsMovableObject, IsUnmovableobject, IsTool, IsDevice, IsDoor,
        //  IsPremium, MaxContSize, Length, MaxStackSize, UnitWeight, BackgndImage, WorkAreaTop, WorkAreaLeft,
        //  WorkAreaWidth, WorkAreaHeight, BtnCloseTop, BtnCloseLeft, FaceImage, Description, BasePrice,
        //  OwnerTimeout, AllowExportFromRed, AllowExportFromGreen)
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2894,454,'Jeweler\\'s Workshop',0,0,1,0,1,1,0,100000,6,0,200000,'',0,0,0,0,0,0,'art/2D/Objects/jeweler_workshop.png','',0,0,0,0)");

        //////////////////////////////////////// Recipe (build the structure) ////////////////////////////////////////
        // Ported from the MMO's recipe.xml (ID 1704) + recipe_requirement.xml, EXCEPT the Amberwood Board row
        // (see note #5) and SkillLvl (see note #6 — lowered to 60 per GreedyFox's explicit request 2026-09-17,
        // was 90 originally). ImagePath is the Herbalist's Shop placeholder icon, see note #2.
        // (RecipeID, Name, Description, StartingToolsID, SkillTypeID, SkillLvl, ResultObjectTypeID,
        //  SkillDepends, Quantity, Autorepeat, IsBlueprint, ImagePath)
        // StartingToolsID=32 (Hammer) — 2026-09-17 fix, was NULL. Every vanilla "Crafting bonus buildings"
        // recipe uses Hammer (32); with NULL this recipe silently never appeared in the in-game Build menu
        // at all (discovered live 2026-09-17 — object/recipe rows were correct, but the client's Masonry >
        // Crafting bonus buildings list evidently keys off holding this specific tool to populate).
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1704,'Jeweler\\'s Workshop','',32,19,60,2894,40,1,0,0,'art/2D/Recipes/jeweler_workshop.png')");

        //////////////////////////////////// Recipe Requirements (build) ////////////////////////////////////
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1704, 32, 0, 30, 20, 0)");   // 20 x Hammer
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1704, 235, 0, 15, 30, 0)");  // 30 x Boards (substitute for Amberwood Board, see note #5)
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1704, 324, 0, 15, 20, 0)");  // 20 x Hardwood Billet
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1704, 281, 0, 10, 90, 0)");  // 90 x Nails
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1704, 233, 0, 10, 15, 0)");  // 15 x Building Log

        //////////////////////////////////// Ability gate — LIVE, see note #4 (done manually in skill_types.xml) ////////////////////////////////////
        // Not a dbi.Update() call — "Open/Close Door" (id 144) had its object_type_id list extended to include
        // 2894, edited directly in the live server's data/skill_types.xml. "Create Jewelry" (id 201) is back to
        // vanilla, no edit needed/wanted there anymore. See note #4/#9 for what to redo if skill_types.xml ever
        // resets via Steam verify-integrity.

        //////////////////////////////////// "Workshop-crafted" jewelry recipes — see note #7 ////////////////////////////////////
        // Exact clones of 245-258, StartingToolsID changed from 527 (Toolkit) to 2894 (Workshop), and the
        // tool's own Influence doubled from 10 to 20 in the matching recipe_requirement row. Everything else
        // (materials, gems, Quantity, SkillLvl, SkillDepends, Autorepeat, ResultObjectTypeID) unchanged from
        // the original recipe it clones.

        // 5025 <- 245 Silver ring
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5025,'Silver Ring','',2894,52,0,479,30,1,1,0,'art/2D/Items/silver_ring.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5025, 2894, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5025, 417, 0, 60, 1, 0)");

        // 5026 <- 246 Gold ring
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5026,'Gold Ring','',2894,52,0,487,30,1,1,0,'art/2D/Items/gold_ring.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5026, 2894, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5026, 418, 0, 60, 1, 0)");

        // 5027 <- 247 Gold and silver ring
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5027,'Gold and Silver Ring','',2894,52,30,499,30,1,1,0,'art/2D/Items/gold_and_silver_ring.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5027, 2894, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5027, 418, 0, 30, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5027, 417, 0, 30, 1, 0)");

        // 5028 <- 248 Gold and silver amulet
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5028,'Gold and Silver Amulet','',2894,52,30,488,30,1,1,0,'art/2D/Items/gold_and_silver_amulet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5028, 2894, 0, 20, 40, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5028, 417, 0, 30, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5028, 418, 0, 30, 1, 0)");

        // 5029 <- 249 Silver jeweled necklace
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5029,'Silver Jeweled Necklace','',2894,52,60,489,30,1,1,0,'art/2D/Items/silver_jeweled_necklace.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5029, 2894, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5029, 417, 0, 20, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5029, 481, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5029, 482, 0, 20, 1, 0)");

        // 5030 <- 250 Gold jeweled necklace
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5030,'Gold Jeweled Necklace','',2894,52,60,490,30,1,1,0,'art/2D/Items/gold_necklace.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5030, 2894, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5030, 418, 0, 20, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5030, 483, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5030, 484, 0, 20, 1, 0)");

        // 5031 <- 251 Exceptional gold and silver amulet
        // NOTE: name shortened to "(Workshop)" not "(Jeweler's Workshop)" -- the full name silently truncated
        // at the recipe.Name column's VARCHAR(50) limit, discovered live 2026-09-17.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5031,'Exceptional Gold and Silver Amulet','',2894,52,90,491,10,1,1,0,'art/2D/Items/exclusive_gold_and_silver_amulet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5031, 2894, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5031, 418, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5031, 417, 0, 10, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5031, 486, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5031, 485, 0, 20, 3, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5031, 484, 0, 20, 4, 0)");

        // 5032 <- 252 Exceptional gold and silver ring
        // NOTE: name shortened, same VARCHAR(50) reason as 5031 above.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5032,'Exceptional Gold and Silver Ring','',2894,52,90,498,10,1,1,0,'art/2D/Items/exclusive_gold_and_silver_ring.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5032, 2894, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5032, 418, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5032, 417, 0, 10, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5032, 486, 0, 20, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5032, 485, 0, 20, 2, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5032, 483, 0, 20, 4, 0)");

        // 5033 <- 253 Silver amethyst ring
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5033,'Silver Amethyst Ring','',2894,52,60,492,20,1,1,0,'art/2D/Items/silver_amethyst_ring.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5033, 2894, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5033, 417, 0, 30, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5033, 481, 0, 40, 1, 0)");

        // 5034 <- 254 Silver garnet ring
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5034,'Silver Garnet Ring','',2894,52,60,493,20,1,1,0,'art/2D/Items/silver_garnet_ring.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5034, 2894, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5034, 417, 0, 30, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5034, 482, 0, 40, 1, 0)");

        // 5035 <- 255 Silver ruby ring
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5035,'Silver Ruby Ring','',2894,52,60,494,20,1,1,0,'art/2D/Items/silver_ruby_ring.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5035, 2894, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5035, 417, 0, 30, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5035, 483, 0, 40, 1, 0)");

        // 5036 <- 256 Gold emerald ring
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5036,'Gold Emerald Ring','',2894,52,60,495,20,1,1,0,'art/2D/Items/gold_emerald_ring.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5036, 2894, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5036, 418, 0, 30, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5036, 484, 0, 40, 1, 0)");

        // 5037 <- 257 Gold sapphire ring
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5037,'Gold Sapphire Ring','',2894,52,60,496,20,1,1,0,'art/2D/Items/gold_sapphire_ring.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5037, 2894, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5037, 418, 0, 30, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5037, 485, 0, 40, 1, 0)");

        // 5038 <- 258 Gold diamond ring
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5038,'Gold Diamond Ring','',2894,52,60,497,20,1,1,0,'art/2D/Items/gold_diamond_ring.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5038, 2894, 0, 20, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5038, 418, 0, 30, 1, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5038, 486, 0, 40, 1, 0)");
    }
};
activatePackage(LiFxJewelersWorkshop);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxJewelersWorkshop);
