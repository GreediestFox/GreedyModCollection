// Carved chests ported from Life is Feudal MMO (movable containers). Objects 3537-3540, carried twins 3541-3544, recipes 6294-6297.

if (!isObject(LiFxChestsPack))
{
    new ScriptObject(LiFxChestsPack)
    {
    };
}

package LiFxChestsPack
{
    function LiFxChestsPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxChestsPack);
    }
    function LiFxChestsPack::version() {
        return "1.0.0";
    }
    function LiFxChestsPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3537,1637,'Carved Chest 1',1,1,0,0,0,0,0,500000,4,0,5000,'art\\images\\universal',0,0,0,0,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vI.png','',23000,150,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3541,1902,'Carved Chest 1',0,0,0,0,0,0,0,0,4,1,5000,'',0,0,0,0,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vI.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3538,1637,'Carved Chest 2',1,1,0,0,0,0,0,500000,4,0,5000,'art\\images\\universal',0,0,0,0,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vII.png','',23000,150,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3542,1902,'Carved Chest 2',0,0,0,0,0,0,0,0,4,1,5000,'',0,0,0,0,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vII.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3539,1637,'Carved Chest 3',1,1,0,0,0,0,0,500000,4,0,5000,'art\\images\\universal',0,0,0,0,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vIII.png','',23000,150,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3543,1902,'Carved Chest 3',0,0,0,0,0,0,0,0,4,1,5000,'',0,0,0,0,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vIII.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3540,1637,'Carved Strongbox',1,1,0,0,0,0,0,500000,4,0,5000,'art\\images\\universal',0,0,0,0,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vIV.png','',23000,150,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3544,1902,'Carved Strongbox',0,0,0,0,0,0,0,0,4,1,5000,'',0,0,0,0,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vIV.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6294,'Carved Chest 1','Ported from Life is Feudal MMO',NULL,8,100,3537,10,1,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vI.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6295,'Carved Chest 2','Ported from Life is Feudal MMO',NULL,8,100,3538,10,1,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vII.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6296,'Carved Chest 3','Ported from Life is Feudal MMO',NULL,8,100,3539,10,1,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vIII.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6297,'Carved Strongbox','Ported from Life is Feudal MMO',NULL,8,100,3540,10,1,0,0,'mod/ChestsMod/art/2D/Recipes/chest_vIV.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6294,3802,0,45,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6294,281,0,45,100,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6295,3802,0,45,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6295,281,0,45,100,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6296,3802,0,45,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6296,281,0,45,100,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6297,3802,0,30,20,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6297,281,0,30,160,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6297,1131,0,30,20,0)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3537,3541)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3538,3542)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3539,3543)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3540,3544)");
    }
};
activatePackage(LiFxChestsPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxChestsPack);
