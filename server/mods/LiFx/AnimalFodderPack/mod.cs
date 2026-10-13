/*
* <description>Animal fodder: Grosstierfutter (stables 370), Nutztierfutter (barns 371), Kleintierfutter (coops 372).
*   Crafted at the Quern-stone (116) with Advanced Farming (32) level 30, 10 per craft.
*   Plus hook animalFodder: only these items count as quality-bonus feed (+2 quality per newborn) and are eaten as feed.</description>
*/
if (!isObject(LiFxAnimalFodderPack)) { new ScriptObject(LiFxAnimalFodderPack) { }; }

package LiFxAnimalFodderPack
{
    function LiFxAnimalFodderPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxAnimalFodderPack);
    }
    function LiFxAnimalFodderPack::version() {
        return "1.0.0";
    }
    function LiFxAnimalFodderPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (370,251,'Large Animal Feed',0,0,0,0,0,0,0,0,2,10000,1500,'',0,0,0,0,0,0,'art/2D/Items/feed_large_animal.png','Special feed for stables. While it is in a stable, all newborn animals gain +2 quality.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (371,251,'Livestock Feed',0,0,0,0,0,0,0,0,2,10000,1500,'',0,0,0,0,0,0,'art/2D/Items/feed_livestock.png','Special feed for barns. While it is in a barn, all newborn animals gain +2 quality.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (372,251,'Small Animal Feed',0,0,0,0,0,0,0,0,2,10000,230,'',0,0,0,0,0,0,'art/2D/Items/feed_small_animal.png','Special feed for coops. While it is in a coop, all newborn animals gain +2 quality.',50,NULL,0,0)");
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` = 6500");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6500,'Large Animal Feed','',116,32,30,370,20,10,1,0,'art/2D/Items/feed_large_animal.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6500, 362, 0, 20, 10, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6500, 3177, 0, 25, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6500, 358, 0, 25, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6500, 3175, 0, 25, 10, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6500, 116, 0, 5, 25, 0)");
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` = 6501");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6501,'Livestock Feed','',116,32,30,371,20,10,1,0,'art/2D/Items/feed_livestock.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6501, 362, 0, 20, 10, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6501, 354, 0, 25, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6501, 1026, 0, 25, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6501, 3175, 0, 25, 10, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6501, 116, 0, 5, 25, 0)");
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` = 6502");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6502,'Small Animal Feed','',116,32,30,372,20,10,1,0,'art/2D/Items/feed_small_animal.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6502, 1396, 0, 25, 10, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6502, 3176, 0, 25, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6502, 358, 0, 25, 5, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6502, 352, 0, 20, 10, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 6502, 116, 0, 5, 25, 0)");
    }
};
activatePackage(LiFxAnimalFodderPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxAnimalFodderPack);
