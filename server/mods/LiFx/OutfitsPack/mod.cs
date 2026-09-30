// Stonecutter's and Breeder's Outfits (MMO), using look-alike meshes of the Blacksmith's/Carpenter's outfits. Objects 3591-3592, recipes 6346-6347.

if (!isObject(LiFxOutfitsPack))
{
    new ScriptObject(LiFxOutfitsPack)
    {
    };
}

package LiFxOutfitsPack
{
    function LiFxOutfitsPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxOutfitsPack);
    }
    function LiFxOutfitsPack::version() {
        return "1.0.0";
    }
    function LiFxOutfitsPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3591,225,'Stonecutter''s Outfit',0,0,0,0,0,0,0,0,4,1,1200,'',0,0,0,0,0,0,'mod/OutfitsMod/art/2D/Items/miner.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3592,225,'Breeder''s Outfit',0,0,0,0,0,0,0,0,4,1,1200,'',0,0,0,0,0,0,'mod/OutfitsMod/art/2D/Items/breeder.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6346,'Stonecutter''s Outfit','',NULL,25,60,3591,50,1,0,0,'mod/OutfitsMod/art/2D/Items/miner.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6347,'Breeder''s Outfit','',NULL,25,60,3592,50,1,0,0,'mod/OutfitsMod/art/2D/Items/breeder.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6346,261,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6346,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6346,3554,0,20,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6346,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6347,261,0,15,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6347,260,0,15,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6347,3556,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6347,295,0,10,40,0)");
    }
};
activatePackage(LiFxOutfitsPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxOutfitsPack);
