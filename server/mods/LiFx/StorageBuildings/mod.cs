/**
* <author>GreedyFox</author>
* <description>Storage buildings ported from the MMO / Arden for Lif:YO: Small Wooden Shed 2400 (recipe 5720 = my own design, the MMO has none), Ore Stockpile 2830 (1619), Stone Stockpile 2831 (1620),
*              Grain Stockpile 2832 (1621), Compost Heap 2833 (1622); all under Buildings > Storage (parent 69), plain containers (IsContainer=1, IsDevice=0). The server's open-inventory check only
*              knows the vanilla warehouses, so these types are registered in lifxpluss.xml <greenhouseAlias><warehouse objectTypeId=".."/> (Plus hook_greenhouse_alias). The Woodshed 2480 is in WoodshedPort.
*              Also needed (NOT done by this script): models under art/models/3d/construction/storage/*, icons in art/2D/Objects + Recipes, <object> blocks in data/cm_objects.xml, 2400/2830-2833 in the
*              Rename (312) / Manage Object Rights (339) lists of skill_types.xml, the Manure_diff material for the compost heap in client art/materials.cs, and the client rows (objects_types/recipe/recipe_requirement.xml).
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*/

if (!isObject(LiFxStorageBuildings))
{
    new ScriptObject(LiFxStorageBuildings)
    {
    };
}

package LiFxStorageBuildings
{
    function LiFxStorageBuildings::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxStorageBuildings);
    }

    function LiFxStorageBuildings::version() {
        return "1.0.0";
    }

    function LiFxStorageBuildings::dbChanges() {
        // recipe_requirement has no uniqueness constraint and art/dump.sql seeds the same rows every boot: clear first.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` IN (1619,1620,1621,1622,5720)");

        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2400,69,'Small Wooden Shed',1,0,1,0,0,0,0,5000000,8,0,10000,'art/images/warehouse',0,0,0,0,0,0,'art/2D/Objects/small_wooden_shed.png','',0,0,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2830,69,'Ore Stockpile',1,0,1,0,0,0,0,15000000,8,0,100000,'art/images/universal',0,0,0,0,0,0,'art/2D/Objects/ore_stockpile.png','',0,0,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2831,69,'Stone Stockpile',1,0,1,0,0,0,0,15000000,8,0,100000,'art/images/universal',0,0,0,0,0,0,'art/2D/Objects/stone_stockpile.png','',0,0,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2832,69,'Grain Stockpile',1,0,1,0,0,0,0,15000000,8,0,100000,'art/images/universal',0,0,0,0,0,0,'art/2D/Objects/grain_stockpile.png','',0,0,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2833,69,'Compost Heap',1,0,1,0,0,0,0,15000000,8,0,100000,'art/images/universal',0,0,0,0,0,0,'art/2D/Objects/compost_heap.png','',0,0,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1619,'Ore Stockpile','',32,18,30,2830,40,1,0,0,'art/2D/Recipes/ore_stockpile.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1620,'Stone Stockpile','',32,18,30,2831,40,1,0,0,'art/2D/Recipes/stone_stockpile.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1621,'Grain Stockpile','',32,18,60,2832,40,1,0,0,'art/2D/Recipes/grain_stockpile.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1622,'Compost Heap','',32,18,30,2833,40,1,0,0,'art/2D/Recipes/compost_heap.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5720,'Small Wooden Shed','',32,18,30,2400,20,1,0,0,'art/2D/Recipes/small_wooden_shed.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1619,235,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1619,32,0,30,20,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1619,324,0,10,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1619,281,0,10,50,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1619,233,0,10,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1620,235,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1620,32,0,30,20,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1620,324,0,10,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1620,281,0,10,50,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1620,233,0,10,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1621,235,0,10,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1621,32,0,25,15,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1621,1356,0,10,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1621,281,0,10,50,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1621,233,0,10,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1621,1477,0,15,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1622,327,0,20,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1622,324,0,10,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1622,281,0,10,50,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1622,233,0,10,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1622,32,0,30,20,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5720,326,0,25,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5720,281,0,15,20,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5720,32,0,40,5,0)");
    }
};
activatePackage(LiFxStorageBuildings);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxStorageBuildings);
