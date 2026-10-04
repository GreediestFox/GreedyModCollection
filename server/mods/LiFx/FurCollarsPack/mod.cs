// Fur collars (from the EKR modpack, 2026-10-04): the fur parts of the Viking helmets (Padded90/Chain90/Padded60_Vik_Helmet_Add_Dw)
// worn alone in a ring slot (7/8), cosmetic only. Objects 3936-3938, recipes 6488-6490 (Tailoring 90, Weaver's Toolkit).

if (!isObject(LiFxFurCollarsPack))
{
    new ScriptObject(LiFxFurCollarsPack)
    {
    };
}

package LiFxFurCollarsPack
{
    function LiFxFurCollarsPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxFurCollarsPack);
    }
    function LiFxFurCollarsPack::version() {
        return "1.0.0";
    }
    function LiFxFurCollarsPack::dbChanges() {
        // Slot 17: the engine already accepts equipment slots 1..17 (client and server check slot-1 < 0x11), but every character
        // only gets DB rows for 1..16 (15/16 are the fists). Re-create the allocation procedure with 17 and add the row for
        // existing characters; slot 17 is reserved for the fur collars.
        dbi.Update("DROP PROCEDURE IF EXISTS `p_allocate_equipment_slots`");
        dbi.Update("CREATE PROCEDURE `p_allocate_equipment_slots`(IN `in_charID` INT UNSIGNED) BEGIN insert ignore equipment_slots (CharacterID, Slot) select in_charID, t.slot from (select 1 as slot union all select 2 union all select 3 union all select 4 union all select 5 union all select 6 union all select 7 union all select 8 union all select 9 union all select 10 union all select 11 union all select 12 union all select 13 union all select 14 union all select 15 union all select 16 union all select 17) as t; END");
        dbi.Update("INSERT IGNORE INTO `equipment_slots` (`CharacterID`, `Slot`) SELECT DISTINCT `CharacterID`, 17 FROM `equipment_slots` WHERE NOT EXISTS (SELECT 1 FROM `equipment_slots` e2 WHERE e2.CharacterID = equipment_slots.CharacterID AND e2.Slot = 17)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3936,478,'Boar Fur Collar',0,0,0,0,0,0,0,0,3,1,500,'',0,0,0,0,0,0,'art/2D/Items/boar_fur_collar.png','A fur collar from a Viking helmet, worn on its own. Cosmetic, takes a ring slot.',2000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3937,478,'Bear Fur Collar',0,0,0,0,0,0,0,0,3,1,500,'',0,0,0,0,0,0,'art/2D/Items/bear_fur_collar.png','A fur collar from a Viking helmet, worn on its own. Cosmetic, takes a ring slot.',2000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3938,478,'Small Boar Fur Collar',0,0,0,0,0,0,0,0,3,1,500,'',0,0,0,0,0,0,'art/2D/Items/small_boar_fur_collar.png','A fur collar from a Viking helmet, worn on its own. Cosmetic, takes a ring slot.',2000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6488,'Boar Fur Collar','',295,25,90,3936,50,1,0,0,'art/2D/Items/boar_fur_collar.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6488,429,0,25,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6488,488,0,25,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6488,1393,0,25,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6488,1477,0,25,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6489,'Bear Fur Collar','',295,25,90,3937,50,1,0,0,'art/2D/Items/bear_fur_collar.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6489,426,0,25,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6489,490,0,25,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6489,1393,0,25,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6489,261,0,25,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6490,'Small Boar Fur Collar','',295,25,90,3938,50,1,0,0,'art/2D/Items/small_boar_fur_collar.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6490,429,0,25,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6490,489,0,25,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6490,1393,0,25,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6490,261,0,25,1,0)");
    }
};
activatePackage(LiFxFurCollarsPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxFurCollarsPack);
