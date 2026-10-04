// Pillars and signs ported from Life is Feudal MMO (Owls' Pillar, Gavran's Soul, Horse Head Pillar, Sign of Hidden Paths, Sleeper's Sign). Object ids 3106-3110, recipes 5906-5910.

if (!isObject(LiFxSignsPillarsPack))
{
    new ScriptObject(LiFxSignsPillarsPack)
    {
    };
}

package LiFxSignsPillarsPack
{
    function LiFxSignsPillarsPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxSignsPillarsPack);
    }
    function LiFxSignsPillarsPack::version() {
        return "1.0.0";
    }
    function LiFxSignsPillarsPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3106,1637,'Owls'' Pillar',0,0,1,0,0,0,1,0,0,0,20000,'',0,0,0,0,0,0,'mod/SignsPillarsMod/art/2D/Recipes/sign_a.png','',20,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3107,1637,'Gavran''s Soul',0,0,1,0,0,0,1,0,0,0,20000,'',0,0,0,0,0,0,'mod/SignsPillarsMod/art/2D/Recipes/sign_b.png','',20,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3108,1637,'Horse Head Pillar',0,0,1,0,0,0,1,0,0,0,20000,'',0,0,0,0,0,0,'mod/SignsPillarsMod/art/2D/Recipes/sign_c.png','',20,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3109,1637,'Sign of Hidden Paths',0,0,1,0,0,0,1,0,0,0,20000,'',0,0,0,0,0,0,'mod/SignsPillarsMod/art/2D/Recipes/sign_d.png','',20,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3110,1637,'Sleeper''s Sign',0,0,1,0,0,0,1,0,0,0,20000,'',0,0,0,0,0,0,'mod/SignsPillarsMod/art/2D/Recipes/sign_e.png','',20,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5906,'Owls'' Pillar','Ported from Life is Feudal MMO',NULL,53,0,3106,10,1,0,0,'mod/SignsPillarsMod/art/2D/Recipes/sign_a.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5907,'Gavran''s Soul','Ported from Life is Feudal MMO',NULL,53,0,3107,10,1,0,0,'mod/SignsPillarsMod/art/2D/Recipes/sign_b.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5908,'Horse Head Pillar','Ported from Life is Feudal MMO',NULL,53,0,3108,10,1,0,0,'mod/SignsPillarsMod/art/2D/Recipes/sign_c.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5909,'Sign of Hidden Paths','Ported from Life is Feudal MMO',NULL,53,0,3109,10,1,0,0,'mod/SignsPillarsMod/art/2D/Recipes/sign_d.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5910,'Sleeper''s Sign','Ported from Life is Feudal MMO',NULL,53,0,3110,10,1,0,0,'mod/SignsPillarsMod/art/2D/Recipes/sign_e.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5906,233,0,90,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5907,233,0,90,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5908,233,0,90,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5909,233,0,90,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5910,233,0,90,1,0)");
    }
};
activatePackage(LiFxSignsPillarsPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxSignsPillarsPack);
