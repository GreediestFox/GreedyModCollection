// Altars and idols ported from Life is Feudal MMO (Ferryman, Rob, Sofapek, Terskel altars; Sofapek, Terskel idols). Object ids 3100-3105, recipes 5900-5905.

if (!isObject(LiFxAltarsIdolsPack))
{
    new ScriptObject(LiFxAltarsIdolsPack)
    {
    };
}

package LiFxAltarsIdolsPack
{
    function LiFxAltarsIdolsPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxAltarsIdolsPack);
    }
    function LiFxAltarsIdolsPack::version() {
        return "1.0.0";
    }
    function LiFxAltarsIdolsPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3100,1637,'Altar Ferryman',0,0,1,0,0,0,1,0,0,0,800000,'',0,0,0,0,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/altar_ferryman.png','',20,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3101,1637,'Altar Rob',0,0,1,0,0,0,1,0,0,0,800000,'',0,0,0,0,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/altar_rob.png','',20,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3102,1637,'Altar Sofapek',0,0,1,0,0,0,1,0,0,0,800000,'',0,0,0,0,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/altar_sofapek.png','',20,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3103,1637,'Altar Terskel',0,0,1,0,0,0,1,0,0,0,800000,'',0,0,0,0,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/altar_terskel.png','',20,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3104,1722,'Idol Sofapek',0,0,1,0,0,0,0,0,0,0,5000,'',0,0,0,0,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/idol_sofapek.png','',1,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3105,1722,'Idol Terskel',0,0,1,0,0,0,0,0,0,0,5000,'',0,0,0,0,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/idol_terskel.png','',1,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5900,'Altar Ferryman','Ported from Life is Feudal MMO',NULL,62,0,3100,10,1,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/altar_ferryman.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5901,'Altar Rob','Ported from Life is Feudal MMO',NULL,62,0,3101,10,1,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/altar_rob.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5902,'Altar Sofapek','Ported from Life is Feudal MMO',NULL,62,0,3102,10,1,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/altar_sofapek.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5903,'Altar Terskel','Ported from Life is Feudal MMO',NULL,62,0,3103,10,1,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/altar_terskel.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5904,'Idol Sofapek','Ported from Life is Feudal MMO',34,54,0,3104,25,1,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/idol_sofapek.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5905,'Idol Terskel','Ported from Life is Feudal MMO',34,54,0,3105,25,1,0,0,'mod/AltarsIdolsMod/art/2D/Recipes/idol_terskel.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5900,1636,0,50,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5900,1635,0,40,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5901,1636,0,50,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5901,1635,0,40,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5902,1636,0,50,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5902,1635,0,40,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5903,1636,0,50,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5903,1635,0,40,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5904,233,0,65,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5904,34,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5904,247,0,0,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5905,233,0,65,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5905,34,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5905,247,0,0,1,0)");
    }
};
activatePackage(LiFxAltarsIdolsPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxAltarsIdolsPack);
