// BeveragesPack: six drinks, each applies one buff and one drawback from EXISTING effects (Plan A of the beverage brief).
// How it works (found by decompiling the server): drinking runs CharacterSaveableEffects::applyPotionItemEffect, which reads the effect list of the
// ITEM INSTANCE (DB: items.FeatureID -> features.has_effects = 1, plus rows in item_effects(ItemID, EffectID, Magnitude)); each EffectID is an `effects`
// table row id that maps to a player effect (cm_effects.xml), Magnitude/1000 is the strength, the duration comes from the item quality, then "Full" (25) is added.
// An item without that list only gives Full. The Plus hook drinkEffects (lifxpluss.xml) now applies the same effects by item type at the moment of drinking;
// the earlier database triggers are dropped at boot (DROP TRIGGER below).
// Items: 3920 Thin Beer (Duennbier), 3921 Strong Beer (Starkbier), 3922 Wheat Beer (Weissbier), 3923 Strong Spirits; vanilla Wine 962 (Wein) and Mead 1119 (Met) get lists too.
if (!isObject(LiFxBeveragesPack)) { new ScriptObject(LiFxBeveragesPack) { }; }

package LiFxBeveragesPack
{
    function LiFxBeveragesPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxBeveragesPack);
    }
    function LiFxBeveragesPack::version() {
        return "0.2.0";
    }
    function LiFxBeveragesPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3920,37,'Thin Beer',0,0,0,0,0,0,0,0,2,10000,750,'',0,0,0,0,0,0,'art/2D/Items/beer.png','A light beer. Your steps quicken, but your sword arm slows.',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3921,37,'Strong Beer',0,0,0,0,0,0,0,0,2,10000,750,'',0,0,0,0,0,0,'art/2D/Items/beer.png','A heavy beer. Your sword arm quickens, but your steps slow.',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3922,37,'Wheat Beer',0,0,0,0,0,0,0,0,2,10000,750,'',0,0,0,0,0,0,'art/2D/Items/beer.png','A cloudy wheat beer. It makes you stronger, but your hands shake.',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3923,37,'Strong Spirits',0,0,0,0,0,0,0,0,2,10000,750,'',0,0,0,0,0,0,'art/2D/Items/beer.png','A burning spirit. It makes you tougher, but clumsy.',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (41,'Drink: Accelerated',NULL,NULL,6)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (42,'Drink: Slowed',NULL,NULL,5)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (43,'Drink: Clumsiness',NULL,NULL,7)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (44,'Drink: Swiftness',NULL,NULL,8)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (45,'Drink: Shaky Hands',NULL,NULL,79)");
        dbi.Update("DROP TRIGGER IF EXISTS `lifx_bev_items_bi`");
        dbi.Update("DROP TRIGGER IF EXISTS `lifx_bev_items_ai`");

    }
};
activatePackage(LiFxBeveragesPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxBeveragesPack);
