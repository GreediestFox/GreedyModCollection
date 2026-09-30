// Craftable movable statues (Tusgaal, McTir, Sovereign) reusing the MMO statue models. Objects 3169-3171, twins 3172-3174, recipes 5949-5951.

if (!isObject(LiFxStatuesPack))
{
    new ScriptObject(LiFxStatuesPack)
    {
    };
}

package LiFxStatuesPack
{
    function LiFxStatuesPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxStatuesPack);
    }
    function LiFxStatuesPack::version() {
        return "1.0.0";
    }
    function LiFxStatuesPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3169,1637,'Statue Tusgaal',0,1,0,0,0,0,0,0,3,0,20000,'',0,0,0,0,0,0,'mod/StatuesMod/art/2D/Recipes/statue_tusgaal.png','',57600,120,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3172,1902,'Statue Tusgaal',0,0,0,0,0,0,0,0,3,1,20000,'',0,0,0,0,0,0,'mod/StatuesMod/art/2D/Recipes/statue_tusgaal.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3170,1637,'Statue McTir',0,1,0,0,0,0,0,0,3,0,20000,'',0,0,0,0,0,0,'mod/StatuesMod/art/2D/Recipes/statue_mctir.png','',57600,120,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3173,1902,'Statue McTir',0,0,0,0,0,0,0,0,3,1,20000,'',0,0,0,0,0,0,'mod/StatuesMod/art/2D/Recipes/statue_mctir.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3171,1637,'Statue Sovereign',0,1,0,0,0,0,0,0,3,0,20000,'',0,0,0,0,0,0,'mod/StatuesMod/art/2D/Recipes/statue_sovereign.png','',57600,120,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3174,1902,'Statue Sovereign',0,0,0,0,0,0,0,0,3,1,20000,'',0,0,0,0,0,0,'mod/StatuesMod/art/2D/Recipes/statue_sovereign.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5949,'Statue Tusgaal','Statue ported from Life is Feudal MMO (recipe designed for YO)',NULL,62,0,3169,10,1,0,0,'mod/StatuesMod/art/2D/Recipes/statue_tusgaal.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5950,'Statue McTir','Statue ported from Life is Feudal MMO (recipe designed for YO)',NULL,62,0,3170,10,1,0,0,'mod/StatuesMod/art/2D/Recipes/statue_mctir.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5951,'Statue Sovereign','Statue ported from Life is Feudal MMO (recipe designed for YO)',NULL,62,0,3171,10,1,0,0,'mod/StatuesMod/art/2D/Recipes/statue_sovereign.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5949,1636,0,90,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5949,1635,0,50,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5950,1636,0,90,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5950,1635,0,50,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5951,1636,0,90,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5951,1635,0,50,2,0)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3169,3172)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3170,3173)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3171,3174)");
    }
};
activatePackage(LiFxStatuesPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxStatuesPack);
