// Siege ladders, platform and kit ported from Life is Feudal MMO. Kit 3593, movables 3594-3597, twins 3598-3601, recipes 6348-6352.

if (!isObject(LiFxSiegePack))
{
    new ScriptObject(LiFxSiegePack)
    {
    };
}

package LiFxSiegePack
{
    function LiFxSiegePack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxSiegePack);
    }
    function LiFxSiegePack::version() {
        return "1.0.0";
    }
    function LiFxSiegePack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3593,1132,'Siege Ladder Kit',0,0,0,0,0,0,0,0,4,10000,95000,'',0,0,0,0,0,0,'art/2D/Items/Warfare_Kit_Large.png','Used to create Siege Ladders. Crafted in a Siege Engineer''s Workshop.',2000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3594,75,'Small Siege Ladder',0,1,0,0,0,0,0,0,6,0,20000,'',0,0,0,0,0,0,'mod/SiegeMod/art/2D/Recipes/small_ladder.png','',5000,150,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3598,1902,'Small Siege Ladder',0,0,0,0,0,0,0,0,6,1,20000,'',0,0,0,0,0,0,'mod/SiegeMod/art/2D/Recipes/small_ladder.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3595,75,'Large Siege Ladder',0,1,0,0,0,0,0,0,6,0,20000,'',0,0,0,0,0,0,'mod/SiegeMod/art/2D/Recipes/large_ladder.png','',5000,150,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3599,1902,'Large Siege Ladder',0,0,0,0,0,0,0,0,6,1,20000,'',0,0,0,0,0,0,'mod/SiegeMod/art/2D/Recipes/large_ladder.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3596,75,'Big Siege Ladder',0,1,0,0,0,0,0,0,6,0,20000,'',0,0,0,0,0,0,'mod/SiegeMod/art/2D/Recipes/big_ladder.png','',5000,150,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3600,1902,'Big Siege Ladder',0,0,0,0,0,0,0,0,6,1,20000,'',0,0,0,0,0,0,'mod/SiegeMod/art/2D/Recipes/big_ladder.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3597,75,'Siege Platform',0,1,0,0,0,0,0,0,6,0,20000,'',0,0,0,0,0,0,'mod/SiegeMod/art/2D/Recipes/platform.png','',5000,150,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3601,1902,'Siege Platform',0,0,0,0,0,0,0,0,6,1,20000,'',0,0,0,0,0,0,'mod/SiegeMod/art/2D/Recipes/platform.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6348,'Siege Ladder Kit','',NULL,10,30,3593,20,1,0,0,'art/2D/Items/Warfare_Kit_Large.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6349,'Small Siege Ladder','Ported from Life is Feudal MMO',NULL,10,30,3594,40,1,0,0,'mod/SiegeMod/art/2D/Recipes/small_ladder.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6350,'Large Siege Ladder','Ported from Life is Feudal MMO',NULL,10,60,3595,40,1,0,0,'mod/SiegeMod/art/2D/Recipes/large_ladder.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6351,'Big Siege Ladder','Ported from Life is Feudal MMO',NULL,10,60,3596,40,1,0,0,'mod/SiegeMod/art/2D/Recipes/big_ladder.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6352,'Siege Platform','Ported from Life is Feudal MMO',NULL,10,30,3597,40,1,0,0,'mod/SiegeMod/art/2D/Recipes/platform.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6348,1413,0,20,20,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6348,262,0,20,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6348,326,0,20,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6348,282,0,20,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6349,3593,0,40,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6349,32,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6350,3593,0,40,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6350,32,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6351,3593,0,40,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6351,32,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6352,3593,0,40,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6352,32,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3594,3598)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3595,3599)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3596,3600)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3597,3601)");
    }
};
activatePackage(LiFxSiegePack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxSiegePack);
