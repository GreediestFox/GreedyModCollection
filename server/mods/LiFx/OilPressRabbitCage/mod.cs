/**
* <author>GreedyFox</author>
* <description>Rabbit Cage 2834 (recipe 1623) and Oil Press 2895 (recipe 1703) ported from the MMO/Arden for Lif:YO, plus the Linen oil item 2873 and its recipe 1686 (10 Flax Seeds -> 2 Linen oil at the Oil Press,
*              Brewing skill 13, tool = the press). The Oil Press is a device-craft building: ability 228 "Crash Fruits" (the wine press's ability) lists 2895 in data/skill_types.xml (server + client), so
*              recipes with SkillTypeID 13 + StartingToolsID 2895 appear at the press. The Rabbit Cage is a plain storage container with its own model/icon (device=0, universal background). It opens as an inventory through the lifxpluss.xml <greenhouseAlias><warehouse objectTypeId="2834"/> entry and accepts ONLY rabbits (item 1053) through the Plus hook hook_stable_alias, lifxpluss.xml <stableAlias><stable objectTypeId="2834" allowAnimals="1053" strict="1"/> (the Coop, 144, accepts only chickens, item 1052). A real Coop-style rabbit stable (full breeding/feeding UI, native model) was attempted and reverted - the client has an unidentified compiled-in gate on Manage/Harvest/Clean/Slaughter for non-vanilla object types that a lot of RE (see project memory) did not fully resolve; GreedyFox chose to keep the cage a simple container instead.
*              Oil_LinenOil_* textures in client art/Textures/TextureLib, and the client rows (objects_types/recipe/recipe_requirement.xml, requirement ids 91179+).
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*/

if (!isObject(LiFxOilPressRabbitCage))
{
    new ScriptObject(LiFxOilPressRabbitCage)
    {
    };
}

package LiFxOilPressRabbitCage
{
    function LiFxOilPressRabbitCage::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxOilPressRabbitCage);
    }

    function LiFxOilPressRabbitCage::version() {
        return "1.0.0";
    }

    function LiFxOilPressRabbitCage::dbChanges() {
        // recipe_requirement has no uniqueness constraint and art/dump.sql seeds the same rows every boot: clear first.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` IN (1623,1703,1686)");

        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2834,64,'Rabbit Cage',1,0,1,0,0,0,0,110000,7,0,20000,'art/images/universal',0,0,0,0,0,0,'art/2D/Objects/rabbit_cage.png','',0,0,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2895,64,'Oil Press',1,0,1,0,1,0,0,5000,3,0,5000,'',0,0,0,0,0,0,'art/2D/Objects/linen_press.png','',0,0,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2873,279,'Linen oil',0,0,0,0,0,0,0,0,2,10000,700,'',0,0,0,0,0,0,'art/2D/Items/linen_oil.png','',0,0,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1623,'Rabbit Cage','',32,18,30,144,35,1,0,0,'art/2D/Recipes/rabbit_cage.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1703,'Oil Press','',32,18,60,2895,35,1,0,0,'art/2D/Recipes/linen_press.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1686,'Linen Oil','',2895,13,30,2873,20,2,1,0,'art/2D/Items/linen_oil.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1623,235,0,40,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1623,471,0,10,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1623,281,0,10,60,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1623,32,0,20,20,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1703,1131,0,30,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1703,235,0,20,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1703,281,0,10,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1703,32,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1686,2895,0,20,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,1686,1030,0,60,10,0)");
    }
};
activatePackage(LiFxOilPressRabbitCage);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxOilPressRabbitCage);
