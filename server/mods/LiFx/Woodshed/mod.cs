/**
* <author>GreedyFox</author>
* <description>Woodshed (the MMO's "Log Warehouse", object 2480, recipe 1280) as a normal storage building for Lif:YO. Model/icons come from the MMO client. Under Buildings > Storage (parent 69).
*              The MMO's special log storage (abilities 404/405 "Put into / Pull from Log Warehouse", capacity in spaces) does not exist in the YO server, so this is a plain container (IsContainer=1)
*              NOT flagged as a device (the client hides "Look in Inventory" for devices); the server's "open inventory" check only accepts incomplete sites, devices, warehouse types 131/132/516, windmills and ruins, so 2480 is added to that check by the Plus hook (config <greenhouseAlias><warehouse objectTypeId="2480"/>).
*              Also needed (NOT done by this script): models under art/models/3d/construction/farming/woodshed + misc/site/Woodshed_inc.dts, icons woodshed.png/woodshed_icon.png in art/2D/Objects,
*              the <object id="2480"> block in data/cm_objects.xml, 2480 in the Rename (312) / Manage Object Rights (339) lists of skill_types.xml, and the client rows (objects_types/recipe/recipe_requirement.xml).
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*/

if (!isObject(LiFxWoodshed))
{
    new ScriptObject(LiFxWoodshed)
    {
    };
}

package LiFxWoodshed
{
    function LiFxWoodshed::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxWoodshed);
    }

    function LiFxWoodshed::version() {
        return "1.0.0";
    }

    function LiFxWoodshed::dbChanges() {
        // recipe_requirement has no uniqueness constraint and art/dump.sql seeds the same rows every boot: clear first.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` = 1280");

        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2480,69,'Woodshed',1,0,1,0,0,0,0,10000000,8,0,10000,'art/images/warehouse',0,0,0,0,0,0,'art/2D/Objects/woodshed.png','',0,0,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (1280,'Woodshed','',32,18,60,2480,30,1,0,0,'art/2D/Objects/woodshed_icon.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1280, 326, 0, 15, 15, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1280, 281, 0, 15, 30, 0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL, 1280, 32, 0, 50, 5, 0)");
    }
};
activatePackage(LiFxWoodshed);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxWoodshed);
