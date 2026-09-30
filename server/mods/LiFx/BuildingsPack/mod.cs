// Buildings ported from Life is Feudal MMO: Watchtower (3602), Castle Wall Hoarding angle no buttress (3603); recipes 6353-6354.

if (!isObject(LiFxBuildingsPack))
{
    new ScriptObject(LiFxBuildingsPack)
    {
    };
}

package LiFxBuildingsPack
{
    function LiFxBuildingsPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxBuildingsPack);
    }
    function LiFxBuildingsPack::version() {
        return "1.0.0";
    }
    function LiFxBuildingsPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3602,172,'Watchtower',0,0,1,0,0,0,0,0,0,0,600000,'',0,0,0,0,0,0,'mod/BuildingsMod/art/2D/Recipes/watchtower.png','',150000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3603,173,'Castle Wall Hoarding (angle, no buttress)',0,0,1,0,0,0,0,0,0,0,2600000,'',0,0,0,0,0,0,'mod/BuildingsMod/art/2D/Recipes/castlewallhoarding_angle.png','',161500,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6353,'Watchtower','Ported from Life is Feudal MMO',NULL,19,90,3602,30,1,0,0,'mod/BuildingsMod/art/2D/Recipes/watchtower.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6354,'Castle Wall Hoarding (angle, no buttress)','Ported from Life is Feudal MMO',NULL,20,30,3603,25,1,0,0,'mod/BuildingsMod/art/2D/Recipes/castlewallhoarding_angle.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6353,32,0,5,15,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6353,235,0,10,150,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6353,244,0,10,1800,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6353,269,0,45,300,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6354,326,0,5,50,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6354,269,0,15,60,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6354,271,0,30,300,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6354,3560,0,10,50,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6354,282,0,10,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6354,32,0,5,15,0)");
    }
};
activatePackage(LiFxBuildingsPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxBuildingsPack);
