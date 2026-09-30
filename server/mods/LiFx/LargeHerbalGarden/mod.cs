/**
* <author>GreedyFox</author>
* <description>Ports the MMO's Large Herbal Garden (object 2835) into Lif:YO via the LiFx ServerAutoloader
*              framework. Follows the pattern established for the prior five workshops, with one notable
*              DEVIATION from Stonemason's Workplace/Big Tanning Tub/Ore Washer's "recipe workaround" — see
*              note #4, this one gets the REAL MMO ability gate and does NOT need a synthetic bulk recipe.
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*
* ============================ READ BEFORE USING ============================
* 1. ID COLLISION: object ID 2835 and recipe ID 1624 verified free against the live DB 2026-09-16.
*    recipe_requirement uses NULL/auto-increment, not the MMO's own IDs (5617-5622). Still NOT checked against
*    https://www.lifxmod.com/info/object-id-list/.
* 2. ART ASSETS ARE GENUINELY ABSENT FROM THIS MACHINE'S MMO CLIENT — checked thoroughly 2026-09-16, same
*    situation as Tailor's Workshop and Big Drying Frame (unlike Stonemason's Workplace/Big Tanning Tub/Ore
*    Washer, which all had real art). No model in the loose art/models tree, none in a full QuickBMS extraction
*    of art/shapes.cmpack (123/123 entries, confirmed complete), no large_herbal_garden.png icon in either
*    art/2D/Objects.cmpack or Recipes.cmpack (only the SMALL Herbal Garden's icon, herbal_garden.png/
*    Herbal_Garden.png, exists in both). FaceImage/ImagePath below point at paths that don't exist yet — object
*    will register in the DB but be invisible / fail to place in-game until art is sourced elsewhere.
* 3. NO DATABLOCK YET, same reason as note #2 — nothing to wire it to until art exists.
* 4. THE MMO-STYLE ABILITY GATE IS LIVE AND WORKING, AND THIS TIME NO SYNTHETIC "BULK RECIPE" WAS NEEDED AT
*    ALL — different from every other workshop in this project. Checked live data/skill_types.xml 2026-09-16:
*    "Grow Herbs" (id 272), "Plant" (id 273), and "Harvest" (id 274) are ALL already registered at the SAME
*    IDs as MMO, all three gating on object_type_id="1353" (the small Herbal Garden) only. Extended all three
*    to "1353 2835" directly in the live data/skill_types.xml (backup:
*    data/skill_types.xml.bak_pre_herbalgarden_extend) — confirmed via server restart, clean boot, no
*    ability-parsing errors. Unlike Stonemason's Workplace/Big Tanning Tub/Ore Washer (which needed an invented
*    StartingToolsID-gated recipe because their real ability was unregistered) and unlike Tailor's Workshop/Big
*    Drying Frame (which got BOTH a real ability gate AND a real ported bulk recipe), this workshop's actual
*    production IS the ability system itself — Grow Herbs/Plant/Harvest is a plant-then-wait-then-harvest cycle,
*    not a material-conversion recipe, and neither MMO's nor YO's `recipe` table has ANY row using the small
*    Herbal Garden (1353) as StartingToolsID (checked live: zero rows) — there's no analogous "instant craft"
*    recipe to port or invent here, because that's not how this workshop actually functions. The real MMO
*    interaction (plant herbs, grow them, harvest them) now genuinely works at this building once art exists to
*    place it. This was NOT done via dbi.Update() / TorqueScript — it's a live server data file edit, done
*    manually alongside deploying this mod, and needs to be REDONE if the live skill_types.xml is ever reset or
*    restored from an earlier backup.
* 5. THE 400x FERTILE SOIL REQUIREMENT BELOW IS UNVERIFIED — flagged as unusually large back when this data
*    was first extracted (CLAUDE.md Appendix C) and never independently re-checked against the source XML a
*    second time. Kept as originally extracted rather than second-guessed; revisit if it feels wrong in play.
*    Also note only material 326 (Hardwood Board) is flagged as a region item among the six requirements below
*    — the rest are not, per the original extraction.
* =============================================================================
* 6. BULK HERB GROWING (2026-09-19) REPLACES THE "ABILITY GATE" OF NOTE #4. The client shows the herb-garden window (Plant / Harvest, growth timer) ONLY for
*    object type 1353 (hard-coded in yo_cm_client.exe, CmInventory::ShowComplexObjectInventory), so a separate Large Herbal Garden can never get it - clicking Plant
*    opened nothing. Instead the Large Herbal Garden is now a crafting building: ability 328 (a repurposed unused quest ability, renamed "Cultivate Herbs") sits under
*    Herbalism (skill row 12) with ent_req 2835, and recipes 5200-5265 (StartingToolsID 2835, skill 12, SkillLvl 0) exist for each of the 66 Alchemy Herbs:
*    5 herbs + 50 Water + 15 Dung (+ 20 durability of the garden) -> 20 herbs. Vanilla garden: 1 herb + 10 water + max 6 fertilizer. Tune the four numbers in
*    the dbi.Update lines (and art/dump.sql + client recipe*.xml). 2835 was removed from abilities 272/273/274 (Grow Herbs/Plant/Harvest).
** 7. TIME GATE (2026-09-19): the 66 "Grow <herb>" recipes (5200-5265) are now plant-then-collect. A server hook (Plus hook_herb_garden_gate, config <herbGardenGate> in
*    lifxpluss.xml) intercepts CmCreationManager::craftWithDevice for ability 328: the first grow craft at a garden consumes the ingredients, suppresses the herbs and starts a
*    per-garden timer (batches persisted in config/herb_garden_batches.txt); crafting again while growing is refused with message 3632 ("Not ready yet. Soon.") and consumes
*    nothing; once ready the hook swaps in the hidden server-only "Collect <herb>" recipe (grow id + 100 = 5300-5365; only wears the garden 5, result = the herbs) and clears
*    the timer. The grow recipes have Autorepeat=0 so a finished craft can't chain into another planting. Vanilla small garden growth = 259,200,000 ms = 72 game hours.
**/

if (!isObject(LiFxLargeHerbalGarden))
{
    new ScriptObject(LiFxLargeHerbalGarden)
    {
    };
}

package LiFxLargeHerbalGarden
{
    function LiFxLargeHerbalGarden::setup() {
        LiFx::registerCallback($LiFx::hooks::onServerCreatedCallbacks, Datablock, LiFxLargeHerbalGarden);
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxLargeHerbalGarden);

        // Register the new object (building) itself. NOTE: per StonemasonsWorkplacePort note #0, this call
        // alone does NOT reach the live DB — kept for convention, but dbChanges() below does the real INSERT.
        LiFx::registerObjectsTypes(LiFxLargeHerbalGarden::ObjectsTypesLargeHerbalGarden(), LiFxLargeHerbalGarden);
    }

    function LiFxLargeHerbalGarden::version() {
        return "0.1.0-draft";
    }

    function LiFxLargeHerbalGarden::Datablock() {
        // TODO: exec a datablock script here once art (model) is sourced — see note #2/#3 above.
        // exec("yolauncher/modpack/mods/LiFx/LargeHerbalGarden/art/datablocks/LargeHerbalGarden.cs");
    }

    // Straight port of the MMO's objects_types row for ID 2835 ("Large Herbal Garden"), ParentID 64 ("Crafting").
    function LiFxLargeHerbalGarden::ObjectsTypesLargeHerbalGarden() {
        return new ScriptObject(ObjectsTypesLargeHerbalGarden : ObjectsTypes)
        {
            id = 2835; // verified free against the live DB 2026-09-16 — see note #1 above
            ObjectName = "Large Herbal Garden";
            ParentID = 64;
            IsContainer = 1;
            IsMovableObject = 0;
            IsUnmovableobject = 1;
            IsTool = 0;
            IsDevice = 1;
            IsDoor = 0;
            IsPremium = 0;
            MaxContSize = 1000000;
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
            FaceImage = "art/2D/Objects/large_herbal_garden.png";
            Description = "";
            BasePrice = 0;
            OwnerTimeout = 0;
            AllowExportFromRed = 0;
            AllowExportFromGreen = 0;
        };
    }

    function LiFxLargeHerbalGarden::dbChanges() {
        //////////////////////////////////////// Idempotency guard ////////////////////////////////////////
        // CRITICAL, learned 2026-09-17: recipe_requirement has no natural uniqueness constraint, and
        // art/dump.sql ALSO seeds these same rows on every boot now (see JewelersWorkshopPort note #9 for
        // the full incident) — without this DELETE, every boot doubles every material's Influence weight.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` IN (1624,5200,5201,5202,5203,5204,5205,5206,5207,5208,5209,5210,5211,5212,5213,5214,5215,5216,5217,5218,5219,5220,5221,5222,5223,5224,5225,5226,5227,5228,5229,5230,5231,5232,5233,5234,5235,5236,5237,5238,5239,5240,5241,5242,5243,5244,5245,5246,5247,5248,5249,5250,5251,5252,5253,5254,5255,5256,5257,5258,5259,5260,5261,5262,5263,5264,5265,5300,5301,5302,5303,5304,5305,5306,5307,5308,5309,5310,5311,5312,5313,5314,5315,5316,5317,5318,5319,5320,5321,5322,5323,5324,5325,5326,5327,5328,5329,5330,5331,5332,5333,5334,5335,5336,5337,5338,5339,5340,5341,5342,5343,5344,5345,5346,5347,5348,5349,5350,5351,5352,5353,5354,5355,5356,5357,5358,5359,5360,5361,5362,5363,5364,5365,5299)");

        //////////////////////////////////////// Object ////////////////////////////////////////
        // (ID, ParentID, Name, IsContainer, IsMovableObject, IsUnmovableobject, IsTool, IsDevice, IsDoor,
        //  IsPremium, MaxContSize, Length, MaxStackSize, UnitWeight, BackgndImage, WorkAreaTop, WorkAreaLeft,
        //  WorkAreaWidth, WorkAreaHeight, BtnCloseTop, BtnCloseLeft, FaceImage, Description, BasePrice,
        //  OwnerTimeout, AllowExportFromRed, AllowExportFromGreen)
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2835,64,'Large Herbal Garden',1,0,1,0,1,0,0,1000000,6,0,50000,'',0,0,0,0,0,0,'art/2D/Objects/large_herbal_garden.png','',0,0,0,0)");

        //////////////////////////////////////// Recipe (build the structure) ////////////////////////////////////////
        // Ported from the MMO's recipe.xml (ID 1624) + recipe_requirement.xml (rows for RecipeID 1624).
        // SkillTypeID=18 ("Construction") straight from the MMO's own recipe.xml — matches YO's live value.
        // Influence sum (5+60+10+5+5+5=90) + SkillDepends (10) = 100 exactly — this one WOULD validate against
        // LiFx::registerRecipe(), but using raw dbi.Update() anyway for consistency (registerRecipe() is inert
        // either way, see StonemasonsWorkplacePort note #0).
        // (RecipeID, Name, Description, StartingToolsID, SkillTypeID, SkillLvl, ResultObjectTypeID,
        //  SkillDepends, Quantity, Autorepeat, IsBlueprint, ImagePath)
        // StartingToolsID=32 (Hammer) — 2026-09-17 fix, was NULL, see BigTanningTubPort for the full note.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1624,'Large Herbal Garden','',32,18,90,2835,10,1,0,0,'art/2D/Recipes/large_herbal_garden.png')");

        //////////////////////////////////// Recipe Requirements (build) ////////////////////////////////////
        // See note #5 above — the 400 quantity for Fertile Soil is unverified but kept as originally extracted.
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1624, 32, 0, 5, 20, 0)");     // 20 x Hammer
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1624, 334, 0, 60, 400, 0)");  // 400 x Fertile Soil
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1624, 326, 0, 10, 10, 1)");   // 10 x Hardwood Board (region item)
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1624, 254, 0, 5, 10, 0)");    // 10 x Handle
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1624, 1131, 0, 5, 20, 0)");   // 20 x Metal Components
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1624, 281, 0, 5, 50, 0)");    // 50 x Nails

        //////////////////////////////////// Bulk herb-growing recipes (5200-5265) - see note #6 ////////////////////////////////////
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5200,'Grow Pitaku Koro','',2835,12,0,684,10,35,0,0,'art/2D/Items/pitaku_koro.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5201,'Grow Albus Viduae','',2835,12,0,685,10,35,0,0,'art/2D/Items/albus_viduae.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5202,'Grow Aureus Magistrum','',2835,12,0,686,10,35,0,0,'art/2D/Items/aureus_magistrum.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5203,'Grow Sapienta Mantis','',2835,12,0,687,10,35,0,0,'art/2D/Items/sapienta_mantis.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5204,'Grow Nocte Lumen','',2835,12,0,688,10,35,0,0,'art/2D/Items/nocte_lumen.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5205,'Grow Chorea Iram','',2835,12,0,689,10,35,0,0,'art/2D/Items/chorea_iram.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5206,'Grow Desertus Smilax','',2835,12,0,690,10,35,0,0,'art/2D/Items/desertus_smilax.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5207,'Grow Pungentibus Chorea','',2835,12,0,691,10,35,0,0,'art/2D/Items/pungentibus_chorea.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5208,'Grow Mons Bastardus','',2835,12,0,692,10,35,0,0,'art/2D/Items/mons_bastardus.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5209,'Grow Filia Prati','',2835,12,0,693,10,35,0,0,'art/2D/Items/filia_prati.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5210,'Grow Adipem Nebulo','',2835,12,0,694,10,35,0,0,'art/2D/Items/adipem_nebulo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5211,'Grow Rosa Kingsa','',2835,12,0,695,10,35,0,0,'art/2D/Items/rosa_kingsa.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5212,'Grow Bacce Hamsa','',2835,12,0,696,10,35,0,0,'art/2D/Items/bacce_hamsa.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5213,'Grow Suryodaya bhagya','',2835,12,0,697,10,35,0,0,'art/2D/Items/suryodaya_bhagya.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5214,'Grow Saltare Diabolus','',2835,12,0,698,10,35,0,0,'art/2D/Items/saltare_diabolus.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5215,'Grow Kurupa Andhere','',2835,12,0,699,10,35,0,0,'art/2D/Items/kurupa_andhere.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5216,'Grow Topasa Maidana','',2835,12,0,700,10,35,0,0,'art/2D/Items/topasa_maidana.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5217,'Grow Rakta Stema','',2835,12,0,701,10,35,0,0,'art/2D/Items/rakta_stema.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5218,'Grow Phlavar Pharest','',2835,12,0,702,10,35,0,0,'art/2D/Items/phlavar_pharest.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5219,'Grow Mauna Boba','',2835,12,0,703,10,35,0,0,'art/2D/Items/mauna_boba.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5220,'Grow Falcem Malleorum','',2835,12,0,704,10,35,0,0,'art/2D/Items/falcem_malleorum.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5221,'Grow Curaila Jangha','',2835,12,0,705,10,35,0,0,'art/2D/Items/curaila_jangha.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5222,'Grow Aquila Peccatum','',2835,12,0,706,10,35,0,0,'art/2D/Items/aquila_peccatum.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5223,'Grow Nequissimum Propodium','',2835,12,0,707,10,35,0,0,'art/2D/Items/nequissimum_propodium.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5224,'Grow Viridi ursae','',2835,12,0,708,10,35,0,0,'art/2D/Items/viridi_ursae.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5225,'Grow Muncha Vana','',2835,12,0,709,10,35,0,0,'art/2D/Items/muncha_vana.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5226,'Grow Caeci Custos','',2835,12,0,710,10,35,0,0,'art/2D/Items/caeci_custos.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5227,'Grow Errantia Ludaeo','',2835,12,0,711,10,35,0,0,'art/2D/Items/errantia_ludaeo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5228,'Grow Oscularetur','',2835,12,0,712,10,35,0,0,'art/2D/Items/oscularetur.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5229,'Grow Mala Fugam','',2835,12,0,713,10,35,0,0,'art/2D/Items/mala_fugam.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5230,'Grow Curva Manus','',2835,12,0,714,10,35,0,0,'art/2D/Items/curva_manus.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5231,'Grow Pecuarius Ventus','',2835,12,0,715,10,35,0,0,'art/2D/Items/pecuarius_ventus.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5232,'Grow Petra Stellam','',2835,12,0,716,10,35,0,0,'art/2D/Items/petra_stellam.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5233,'Grow Acerba Moretum','',2835,12,0,717,10,35,0,0,'art/2D/Items/acerba_moretum.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5234,'Grow Dulcis Radix','',2835,12,0,718,10,35,0,0,'art/2D/Items/dulcis_radix.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5235,'Grow Kromenta Salicia','',2835,12,0,719,10,35,0,0,'art/2D/Items/kromenta_salicia.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5236,'Grow Vertato Zonda','',2835,12,0,720,10,35,0,0,'art/2D/Items/vertato_zonda.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5237,'Grow Khalari Gratsi','',2835,12,0,721,10,35,0,0,'art/2D/Items/khalari_gratsi.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5238,'Grow Remerta Poskot','',2835,12,0,722,10,35,0,0,'art/2D/Items/remerta_poskot.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5239,'Grow Holmatu Stazo','',2835,12,0,723,10,35,0,0,'art/2D/Items/holmatu_stazo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5240,'Grow Kaleda Mesgano','',2835,12,0,724,10,35,0,0,'art/2D/Items/kaleda_mesgano.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5241,'Grow Fakha Rudob','',2835,12,0,725,10,35,0,0,'art/2D/Items/fakha_rudob.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5242,'Grow Kacaro Vilko','',2835,12,0,726,10,35,0,0,'art/2D/Items/kacaro_vilko.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5243,'Grow Fassari Tolge','',2835,12,0,727,10,35,0,0,'art/2D/Items/fassari_tolge.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5244,'Grow Sarmento Gaute','',2835,12,0,728,10,35,0,0,'art/2D/Items/sarmento_gaute.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5245,'Grow Persetu Hara','',2835,12,0,729,10,35,0,0,'art/2D/Items/persetu_hara.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5246,'Grow Hallatra Kronye','',2835,12,0,730,10,35,0,0,'art/2D/Items/hallatra_kronye.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5247,'Grow Laster Kutta','',2835,12,0,731,10,35,0,0,'art/2D/Items/laster_kutta.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5248,'Grow Utrokka Khuru','',2835,12,0,732,10,35,0,0,'art/2D/Items/utrokka_khuru.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5249,'Grow Kyasaga Sherl','',2835,12,0,733,10,35,0,0,'art/2D/Items/kyasaga_sherl.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5250,'Grow Jukola Beshaar','',2835,12,0,734,10,35,0,0,'art/2D/Items/jukola_beshaar.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5251,'Grow Ripyote Quamisy','',2835,12,0,735,10,35,0,0,'art/2D/Items/ripyote_quamisy.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5252,'Grow Fuskegtra Xelay','',2835,12,0,736,10,35,0,0,'art/2D/Items/fuskegtra_xelay.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5253,'Grow Burmenta Wallo','',2835,12,0,737,10,35,0,0,'art/2D/Items/burmenta_wallo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5254,'Grow Fohatta Torn','',2835,12,0,738,10,35,0,0,'art/2D/Items/fohatta_torn.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5255,'Grow Dustali Krabo','',2835,12,0,739,10,35,0,0,'art/2D/Items/dustali_krabo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5256,'Grow Gratias Sivara','',2835,12,0,740,10,35,0,0,'art/2D/Items/gratias_sivara.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5257,'Grow Memen Anik','',2835,12,0,741,10,35,0,0,'art/2D/Items/memen_anik.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5258,'Grow Gortaka Messen','',2835,12,0,742,10,35,0,0,'art/2D/Items/gortaka_messen.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5259,'Grow Kalya Nori','',2835,12,0,743,10,35,0,0,'art/2D/Items/kalya_nori.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5260,'Grow Uliya Sundara','',2835,12,0,744,10,35,0,0,'art/2D/Items/uliya_sundara.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5261,'Grow Jenaro Vannakam','',2835,12,0,745,10,35,0,0,'art/2D/Items/jenaro_vannakam.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5262,'Grow Huryosa Gulla','',2835,12,0,746,10,35,0,0,'art/2D/Items/huryosa_gulla.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5263,'Grow Ital Iranta','',2835,12,0,747,10,35,0,0,'art/2D/Items/ital_iranta.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5264,'Grow Murkha Bola','',2835,12,0,748,10,35,0,0,'art/2D/Items/murkha_bola.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5265,'Grow Naraen Pandanomo','',2835,12,0,750,10,35,0,0,'art/2D/Items/naraen_pandanomo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5200, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5200, 684, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5200, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5200, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5201, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5201, 685, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5201, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5201, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5202, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5202, 686, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5202, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5202, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5203, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5203, 687, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5203, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5203, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5204, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5204, 688, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5204, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5204, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5205, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5205, 689, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5205, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5205, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5206, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5206, 690, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5206, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5206, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5207, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5207, 691, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5207, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5207, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5208, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5208, 692, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5208, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5208, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5209, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5209, 693, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5209, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5209, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5210, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5210, 694, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5210, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5210, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5211, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5211, 695, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5211, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5211, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5212, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5212, 696, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5212, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5212, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5213, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5213, 697, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5213, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5213, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5214, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5214, 698, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5214, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5214, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5215, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5215, 699, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5215, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5215, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5216, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5216, 700, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5216, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5216, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5217, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5217, 701, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5217, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5217, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5218, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5218, 702, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5218, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5218, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5219, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5219, 703, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5219, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5219, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5220, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5220, 704, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5220, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5220, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5221, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5221, 705, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5221, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5221, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5222, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5222, 706, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5222, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5222, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5223, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5223, 707, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5223, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5223, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5224, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5224, 708, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5224, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5224, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5225, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5225, 709, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5225, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5225, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5226, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5226, 710, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5226, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5226, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5227, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5227, 711, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5227, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5227, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5228, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5228, 712, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5228, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5228, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5229, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5229, 713, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5229, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5229, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5230, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5230, 714, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5230, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5230, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5231, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5231, 715, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5231, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5231, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5232, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5232, 716, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5232, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5232, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5233, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5233, 717, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5233, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5233, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5234, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5234, 718, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5234, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5234, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5235, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5235, 719, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5235, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5235, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5236, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5236, 720, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5236, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5236, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5237, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5237, 721, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5237, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5237, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5238, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5238, 722, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5238, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5238, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5239, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5239, 723, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5239, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5239, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5240, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5240, 724, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5240, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5240, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5241, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5241, 725, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5241, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5241, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5242, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5242, 726, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5242, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5242, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5243, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5243, 727, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5243, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5243, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5244, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5244, 728, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5244, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5244, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5245, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5245, 729, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5245, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5245, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5246, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5246, 730, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5246, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5246, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5247, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5247, 731, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5247, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5247, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5248, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5248, 732, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5248, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5248, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5249, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5249, 733, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5249, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5249, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5250, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5250, 734, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5250, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5250, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5251, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5251, 735, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5251, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5251, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5252, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5252, 736, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5252, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5252, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5253, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5253, 737, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5253, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5253, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5254, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5254, 738, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5254, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5254, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5255, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5255, 739, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5255, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5255, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5256, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5256, 740, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5256, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5256, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5257, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5257, 741, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5257, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5257, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5258, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5258, 742, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5258, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5258, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5259, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5259, 743, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5259, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5259, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5260, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5260, 744, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5260, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5260, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5261, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5261, 745, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5261, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5261, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5262, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5262, 746, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5262, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5262, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5263, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5263, 747, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5263, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5263, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5264, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5264, 748, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5264, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5264, 1032, 0, 10, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5265, 2835, 0, 10, 20, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5265, 750, 0, 60, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5265, 204, 0, 20, 50, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5265, 1032, 0, 10, 15, 0)");

        //////////////////////////////////// Server-only 'collect' recipes (5300-5365) for the time gate - see note #7 ////////////////////////////////////
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5300,'Collect Pitaku Koro','',2835,12,0,684,100,35,0,0,'art/2D/Items/pitaku_koro.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5300, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5301,'Collect Albus Viduae','',2835,12,0,685,100,35,0,0,'art/2D/Items/albus_viduae.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5301, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5302,'Collect Aureus Magistrum','',2835,12,0,686,100,35,0,0,'art/2D/Items/aureus_magistrum.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5302, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5303,'Collect Sapienta Mantis','',2835,12,0,687,100,35,0,0,'art/2D/Items/sapienta_mantis.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5303, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5304,'Collect Nocte Lumen','',2835,12,0,688,100,35,0,0,'art/2D/Items/nocte_lumen.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5304, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5305,'Collect Chorea Iram','',2835,12,0,689,100,35,0,0,'art/2D/Items/chorea_iram.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5305, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5306,'Collect Desertus Smilax','',2835,12,0,690,100,35,0,0,'art/2D/Items/desertus_smilax.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5306, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5307,'Collect Pungentibus Chorea','',2835,12,0,691,100,35,0,0,'art/2D/Items/pungentibus_chorea.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5307, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5308,'Collect Mons Bastardus','',2835,12,0,692,100,35,0,0,'art/2D/Items/mons_bastardus.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5308, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5309,'Collect Filia Prati','',2835,12,0,693,100,35,0,0,'art/2D/Items/filia_prati.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5309, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5310,'Collect Adipem Nebulo','',2835,12,0,694,100,35,0,0,'art/2D/Items/adipem_nebulo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5310, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5311,'Collect Rosa Kingsa','',2835,12,0,695,100,35,0,0,'art/2D/Items/rosa_kingsa.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5311, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5312,'Collect Bacce Hamsa','',2835,12,0,696,100,35,0,0,'art/2D/Items/bacce_hamsa.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5312, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5313,'Collect Suryodaya bhagya','',2835,12,0,697,100,35,0,0,'art/2D/Items/suryodaya_bhagya.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5313, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5314,'Collect Saltare Diabolus','',2835,12,0,698,100,35,0,0,'art/2D/Items/saltare_diabolus.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5314, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5315,'Collect Kurupa Andhere','',2835,12,0,699,100,35,0,0,'art/2D/Items/kurupa_andhere.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5315, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5316,'Collect Topasa Maidana','',2835,12,0,700,100,35,0,0,'art/2D/Items/topasa_maidana.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5316, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5317,'Collect Rakta Stema','',2835,12,0,701,100,35,0,0,'art/2D/Items/rakta_stema.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5317, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5318,'Collect Phlavar Pharest','',2835,12,0,702,100,35,0,0,'art/2D/Items/phlavar_pharest.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5318, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5319,'Collect Mauna Boba','',2835,12,0,703,100,35,0,0,'art/2D/Items/mauna_boba.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5319, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5320,'Collect Falcem Malleorum','',2835,12,0,704,100,35,0,0,'art/2D/Items/falcem_malleorum.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5320, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5321,'Collect Curaila Jangha','',2835,12,0,705,100,35,0,0,'art/2D/Items/curaila_jangha.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5321, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5322,'Collect Aquila Peccatum','',2835,12,0,706,100,35,0,0,'art/2D/Items/aquila_peccatum.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5322, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5323,'Collect Nequissimum Propodium','',2835,12,0,707,100,35,0,0,'art/2D/Items/nequissimum_propodium.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5323, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5324,'Collect Viridi ursae','',2835,12,0,708,100,35,0,0,'art/2D/Items/viridi_ursae.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5324, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5325,'Collect Muncha Vana','',2835,12,0,709,100,35,0,0,'art/2D/Items/muncha_vana.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5325, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5326,'Collect Caeci Custos','',2835,12,0,710,100,35,0,0,'art/2D/Items/caeci_custos.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5326, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5327,'Collect Errantia Ludaeo','',2835,12,0,711,100,35,0,0,'art/2D/Items/errantia_ludaeo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5327, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5328,'Collect Oscularetur','',2835,12,0,712,100,35,0,0,'art/2D/Items/oscularetur.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5328, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5329,'Collect Mala Fugam','',2835,12,0,713,100,35,0,0,'art/2D/Items/mala_fugam.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5329, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5330,'Collect Curva Manus','',2835,12,0,714,100,35,0,0,'art/2D/Items/curva_manus.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5330, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5331,'Collect Pecuarius Ventus','',2835,12,0,715,100,35,0,0,'art/2D/Items/pecuarius_ventus.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5331, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5332,'Collect Petra Stellam','',2835,12,0,716,100,35,0,0,'art/2D/Items/petra_stellam.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5332, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5333,'Collect Acerba Moretum','',2835,12,0,717,100,35,0,0,'art/2D/Items/acerba_moretum.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5333, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5334,'Collect Dulcis Radix','',2835,12,0,718,100,35,0,0,'art/2D/Items/dulcis_radix.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5334, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5335,'Collect Kromenta Salicia','',2835,12,0,719,100,35,0,0,'art/2D/Items/kromenta_salicia.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5335, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5336,'Collect Vertato Zonda','',2835,12,0,720,100,35,0,0,'art/2D/Items/vertato_zonda.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5336, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5337,'Collect Khalari Gratsi','',2835,12,0,721,100,35,0,0,'art/2D/Items/khalari_gratsi.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5337, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5338,'Collect Remerta Poskot','',2835,12,0,722,100,35,0,0,'art/2D/Items/remerta_poskot.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5338, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5339,'Collect Holmatu Stazo','',2835,12,0,723,100,35,0,0,'art/2D/Items/holmatu_stazo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5339, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5340,'Collect Kaleda Mesgano','',2835,12,0,724,100,35,0,0,'art/2D/Items/kaleda_mesgano.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5340, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5341,'Collect Fakha Rudob','',2835,12,0,725,100,35,0,0,'art/2D/Items/fakha_rudob.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5341, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5342,'Collect Kacaro Vilko','',2835,12,0,726,100,35,0,0,'art/2D/Items/kacaro_vilko.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5342, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5343,'Collect Fassari Tolge','',2835,12,0,727,100,35,0,0,'art/2D/Items/fassari_tolge.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5343, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5344,'Collect Sarmento Gaute','',2835,12,0,728,100,35,0,0,'art/2D/Items/sarmento_gaute.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5344, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5345,'Collect Persetu Hara','',2835,12,0,729,100,35,0,0,'art/2D/Items/persetu_hara.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5345, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5346,'Collect Hallatra Kronye','',2835,12,0,730,100,35,0,0,'art/2D/Items/hallatra_kronye.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5346, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5347,'Collect Laster Kutta','',2835,12,0,731,100,35,0,0,'art/2D/Items/laster_kutta.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5347, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5348,'Collect Utrokka Khuru','',2835,12,0,732,100,35,0,0,'art/2D/Items/utrokka_khuru.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5348, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5349,'Collect Kyasaga Sherl','',2835,12,0,733,100,35,0,0,'art/2D/Items/kyasaga_sherl.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5349, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5350,'Collect Jukola Beshaar','',2835,12,0,734,100,35,0,0,'art/2D/Items/jukola_beshaar.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5350, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5351,'Collect Ripyote Quamisy','',2835,12,0,735,100,35,0,0,'art/2D/Items/ripyote_quamisy.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5351, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5352,'Collect Fuskegtra Xelay','',2835,12,0,736,100,35,0,0,'art/2D/Items/fuskegtra_xelay.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5352, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5353,'Collect Burmenta Wallo','',2835,12,0,737,100,35,0,0,'art/2D/Items/burmenta_wallo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5353, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5354,'Collect Fohatta Torn','',2835,12,0,738,100,35,0,0,'art/2D/Items/fohatta_torn.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5354, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5355,'Collect Dustali Krabo','',2835,12,0,739,100,35,0,0,'art/2D/Items/dustali_krabo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5355, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5356,'Collect Gratias Sivara','',2835,12,0,740,100,35,0,0,'art/2D/Items/gratias_sivara.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5356, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5357,'Collect Memen Anik','',2835,12,0,741,100,35,0,0,'art/2D/Items/memen_anik.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5357, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5358,'Collect Gortaka Messen','',2835,12,0,742,100,35,0,0,'art/2D/Items/gortaka_messen.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5358, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5359,'Collect Kalya Nori','',2835,12,0,743,100,35,0,0,'art/2D/Items/kalya_nori.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5359, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5360,'Collect Uliya Sundara','',2835,12,0,744,100,35,0,0,'art/2D/Items/uliya_sundara.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5360, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5361,'Collect Jenaro Vannakam','',2835,12,0,745,100,35,0,0,'art/2D/Items/jenaro_vannakam.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5361, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5362,'Collect Huryosa Gulla','',2835,12,0,746,100,35,0,0,'art/2D/Items/huryosa_gulla.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5362, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5363,'Collect Ital Iranta','',2835,12,0,747,100,35,0,0,'art/2D/Items/ital_iranta.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5363, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5364,'Collect Murkha Bola','',2835,12,0,748,100,35,0,0,'art/2D/Items/murkha_bola.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5364, 2835, 0, 100, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5365,'Collect Naraen Pandanomo','',2835,12,0,750,100,35,0,0,'art/2D/Items/naraen_pandanomo.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5365, 2835, 0, 100, 5, 0)");

        // Visible 'Collect Herbs' entry (5299): needs only garden wear; the time-gate hook maps it to whatever is growing (result Water x1 is a harmless fallback), see note #7.
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5299,'Collect Herbs','',2835,12,0,204,10,1,0,0,'art/2D/Items/pitaku_koro.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 5299, 2835, 0, 100, 5, 0)");

        //////////////////////////////////// Ability gate — LIVE, see note #4 (done manually in skill_types.xml) ////////////////////////////////////
        // Not a dbi.Update() — "Grow Herbs" (id 272), "Plant" (id 273), and "Harvest" (id 274) were extended
        // directly in the live server's data/skill_types.xml alongside deploying this mod. Nothing to do here
        // in script; see note #4 for the full detail and what to redo if that file is ever reset.

        //////////////////////////////////// No "mass production" recipe — see note #4 ////////////////////////////////////
        // Deliberately not added. This workshop's real production IS the ability system (plant, wait, harvest)
        // — there is no material-conversion recipe to substitute or port, unlike every other workshop in this
        // project. Don't add a synthetic recipe here; it wouldn't match how this building actually works.
    }
};
activatePackage(LiFxLargeHerbalGarden);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxLargeHerbalGarden);
