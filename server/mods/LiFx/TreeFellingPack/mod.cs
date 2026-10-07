// Tree felling (ported 2026-10-04 from Daniel's prototype): Amberwood (from spruce) and Whitewood (from maple) logs.
// Objects 3930-3935 (logs + carried twins + billets); boards are the MMO items 3800/3802. Plus hooks: treeDrops,
// logDescription, sawOutput, abilityEntityCheck (lifxpluss.xml); client saw menu needs the client exe patch.

if (!isObject(LiFxTreeFellingPack))
{
    new ScriptObject(LiFxTreeFellingPack)
    {
    };
}

package LiFxTreeFellingPack
{
    function LiFxTreeFellingPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxTreeFellingPack);
    }
    function LiFxTreeFellingPack::version() {
        return "1.0.0";
    }
    function LiFxTreeFellingPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3930,622,'Amberwood Log',0,1,0,0,0,0,0,0,2,0,5,'',0,0,0,0,0,0,'art/2D/Items/soft_log.png','A log of amberwood, felled from a spruce.',0,150,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3931,1902,'Amberwood Log',0,0,0,0,0,0,0,0,2,1,5000,'',0,0,0,0,0,0,'art/2D/Objects/softwood_log.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3932,622,'Whitewood Log',0,1,0,0,0,0,0,0,2,0,5,'',0,0,0,0,0,0,'art/2D/Items/hard_log.png','A log of whitewood, felled from a maple.',0,150,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3933,1902,'Whitewood Log',0,0,0,0,0,0,0,0,2,1,5000,'',0,0,0,0,0,0,'art/2D/Objects/hardwood_log.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3934,234,'Amberwood Billet',0,0,0,0,0,0,0,0,3,10000,10000,'',0,0,0,0,0,0,'art/2D/Items/Amberwood_billet.png','Sawed off of amberwood logs. Can be used as fuel.',400,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3935,234,'Whitewood Billet',0,0,0,0,0,0,0,0,3,10000,10000,'',0,0,0,0,0,0,'art/2D/Items/Whitewood_billet.png','Sawed off of whitewood logs. Can be used as fuel.',400,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` VALUES (2284,3930,3931)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` VALUES (2285,3932,3933)");
    }
};
activatePackage(LiFxTreeFellingPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxTreeFellingPack);
