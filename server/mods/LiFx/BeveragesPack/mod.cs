// Beverage test pack: Thin Beer (3920, Duennbier) and Strong Beer (3921, Starkbier). Each drink applies ONE combined custom effect (100 / 101):
// buff and drawback in one effect (SPEED / ATTACK_SPEED MULTIPLY with fixed values, ignore_magnitude). Items are children of Beer (1117), which the
// "Drink" ability (205) already lists. The effects rows link the potion to the player effect like the cocktails (effects.ResultPotionID -> PlayerEffectID).
if (!isObject(LiFxBeveragesPack)) { new ScriptObject(LiFxBeveragesPack) { }; }

package LiFxBeveragesPack
{
    function LiFxBeveragesPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxBeveragesPack);
    }
    function LiFxBeveragesPack::version() {
        return "0.1.0";
    }
    function LiFxBeveragesPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3920,1117,'Thin Beer',0,0,0,0,0,0,0,0,2,10000,750,'',0,0,0,0,0,0,'art/2D/Items/beer.png','A light beer.',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3921,1117,'Strong Beer',0,0,0,0,0,0,0,0,2,10000,750,'',0,0,0,0,0,0,'art/2D/Items/beer.png','A heavy, strong beer.',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (41,'Thin Beer',NULL,3920,100)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (42,'Strong Beer',NULL,3921,101)");
    }
};
activatePackage(LiFxBeveragesPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxBeveragesPack);
