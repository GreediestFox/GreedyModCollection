// Fresh water / salt water: item 204 (Water from rivers, lakes, ...) is renamed Salt Water, wells give Fresh Water (3180, see config\lifxpluss.xml wellWater itemTypeId),
// Mead, Beer and Dough use Fresh Water. New recipes 6399-6403: Wheat/Oat/Rye Malt, White Flour, Butter (ingredients of the ported food & drinks).

if (!isObject(LiFxFreshWaterPack))
{
    new ScriptObject(LiFxFreshWaterPack)
    {
    };
}

package LiFxFreshWaterPack
{
    function LiFxFreshWaterPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxFreshWaterPack);
    }
    function LiFxFreshWaterPack::version() {
        return "1.0.0";
    }
    function LiFxFreshWaterPack::dbChanges() {
        dbi.Update("UPDATE `objects_types` SET Name='Salt Water' WHERE ID=204");
        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3180 WHERE RecipeID IN (641,642,871) AND MaterialObjectTypeID=204");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6399,'Wheat Malt','',NULL,24,60,3184,20,1,0,0,'art/2D/Items/Malt.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6400,'Oat Malt','',NULL,24,60,3182,20,1,0,0,'art/2D/Items/Malt.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6401,'Rye Malt','',NULL,24,60,3183,20,1,0,0,'art/2D/Items/Malt.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6402,'White Flour','',NULL,32,30,3181,20,1,0,0,'art/2D/Items/flour.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6403,'Butter','',NULL,24,30,3185,20,1,0,0,'art/2D/Items/eggnog.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6399,352,0,60,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6399,141,0,10,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6399,220,0,5,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6399,3180,0,5,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6400,3177,0,60,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6400,141,0,10,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6400,220,0,5,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6400,3180,0,5,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6401,3175,0,60,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6401,141,0,10,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6401,220,0,5,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6401,3180,0,5,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6402,352,0,75,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6402,116,0,5,25,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6403,339,0,60,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6403,220,0,10,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6403,1054,0,10,15,0)");
    }
};
activatePackage(LiFxFreshWaterPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxFreshWaterPack);
