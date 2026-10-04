// Food spoilage (ported 2026-10-04 from Daniel's "food spoilage system" / FoodPerishability).
// Food loses quality once per day of real time, applied at server start only (the server holds loaded items in memory,
// so item quality is never changed while it runs). At 0 quality the stack turns into Dung (1032, q10).
// Rates by the item's ParentID chain: 250/279 = 2 Q/d, 227-231 = 4 Q/d, 248 (meat/fish) = 8 Q/d; Honey (376) never spoils.
// Items inside a Barrel (106), also nested, do not spoil.
// Differences to Daniel's version: bookkeeping lives in its own table `food_perish_items` instead of extra columns on the
// game's `items` table, and the 24h gate is evaluated in SQL so everything is queued in one go during the DB-changes phase.
// Client part: gui\scripts\tooltipManager.cs (shelf-life row) + messages 5195-5198.
// Test: UPDATE food_perish_state SET LastTickAt = NULL WHERE ID = 1;  then restart (applies one day of spoilage).

if (!isObject(LiFxFoodSpoilagePack))
{
    new ScriptObject(LiFxFoodSpoilagePack)
    {
    };
}

package LiFxFoodSpoilagePack
{
    function LiFxFoodSpoilagePack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxFoodSpoilagePack);
    }
    function LiFxFoodSpoilagePack::version() {
        return "1.0.0";
    }
    function LiFxFoodSpoilagePack::dbChanges() {
        // ---- rate and barrel functions ----
        dbi.Update("DROP FUNCTION IF EXISTS `f_foodPerishRate`");
        dbi.Update("CREATE FUNCTION `f_foodPerishRate`(`inObjectTypeID` INT UNSIGNED) RETURNS TINYINT UNSIGNED DETERMINISTIC READS SQL DATA " @
            "BEGIN DECLARE curID INT UNSIGNED; DECLARE curParent INT UNSIGNED; DECLARE steps INT DEFAULT 0; " @
            "IF inObjectTypeID = 376 THEN RETURN NULL; END IF; SET curID = inObjectTypeID; " @
            "WHILE steps < 32 DO SET curParent = NULL; SELECT ParentID INTO curParent FROM objects_types WHERE ID = curID LIMIT 1; " @
            "IF curParent IS NULL THEN RETURN NULL; END IF; " @
            "IF curParent IN (250, 279) THEN RETURN 2; END IF; IF curParent IN (227, 228, 229, 230, 231) THEN RETURN 4; END IF; " @
            "IF curParent = 248 THEN RETURN 8; END IF; SET curID = curParent; SET steps = steps + 1; END WHILE; RETURN NULL; END");
        dbi.Update("DROP FUNCTION IF EXISTS `f_foodPerishInBarrel`");
        dbi.Update("CREATE FUNCTION `f_foodPerishInBarrel`(`inContainerID` INT UNSIGNED) RETURNS TINYINT UNSIGNED DETERMINISTIC READS SQL DATA " @
            "BEGIN DECLARE curID INT UNSIGNED; DECLARE curType INT UNSIGNED; DECLARE curParent INT UNSIGNED; DECLARE steps INT DEFAULT 0; " @
            "IF inContainerID IS NULL THEN RETURN 0; END IF; SET curID = inContainerID; " @
            "WHILE steps < 32 AND curID IS NOT NULL DO SET curType = NULL; SET curParent = NULL; " @
            "SELECT ObjectTypeID, ParentID INTO curType, curParent FROM containers WHERE ID = curID LIMIT 1; " @
            "IF curType IS NULL THEN RETURN 0; END IF; IF curType = 106 THEN RETURN 1; END IF; " @
            "SET curID = curParent; SET steps = steps + 1; END WHILE; RETURN 0; END");

        // ---- tables ----
        dbi.Update("CREATE TABLE IF NOT EXISTS `food_perish_types` (`ObjectTypeID` INT UNSIGNED NOT NULL, `Rate` TINYINT UNSIGNED NOT NULL, PRIMARY KEY (`ObjectTypeID`)) ENGINE=InnoDB");
        dbi.Update("CREATE TABLE IF NOT EXISTS `food_perish_items` (`ItemID` INT UNSIGNED NOT NULL, `TrackedSince` DATETIME NOT NULL, PRIMARY KEY (`ItemID`)) ENGINE=InnoDB");
        dbi.Update("CREATE TABLE IF NOT EXISTS `food_perish_state` (`ID` TINYINT UNSIGNED NOT NULL, `LastTickAt` DATETIME NULL DEFAULT NULL, `DoTick` TINYINT UNSIGNED NOT NULL DEFAULT 0, PRIMARY KEY (`ID`)) ENGINE=InnoDB");
        dbi.Update("INSERT IGNORE INTO `food_perish_state` (`ID`, `LastTickAt`, `DoTick`) VALUES (1, NULL, 0)");

        // ---- type cache (objects_types is reseeded from art/dump.sql before mods run) ----
        dbi.Update("DELETE FROM `food_perish_types`");
        dbi.Update("INSERT INTO `food_perish_types` (`ObjectTypeID`, `Rate`) SELECT `ID`, `f_foodPerishRate`(`ID`) FROM `objects_types` WHERE `f_foodPerishRate`(`ID`) IS NOT NULL");

        // ---- daily tick: only if >= 24h since the last one ----
        dbi.Update("UPDATE `food_perish_state` SET `DoTick` = (`LastTickAt` IS NULL OR TIMESTAMPDIFF(SECOND, `LastTickAt`, NOW()) >= 86400) WHERE `ID` = 1");
        // 1) spoil tracked items outside a barrel by one day's rate
        dbi.Update("UPDATE `items` i JOIN `food_perish_items` f ON f.ItemID = i.ID JOIN `food_perish_types` c ON c.ObjectTypeID = i.ObjectTypeID JOIN `food_perish_state` s ON s.ID = 1 AND s.DoTick = 1 " @
            "SET i.Quality = GREATEST(0, CAST(i.Quality AS SIGNED) - c.Rate) WHERE f_foodPerishInBarrel(i.ContainerID) = 0");
        // 2) spoiled tracked stacks become dung (quantity kept)
        dbi.Update("UPDATE `items` i JOIN `food_perish_items` f ON f.ItemID = i.ID JOIN `food_perish_types` c ON c.ObjectTypeID = i.ObjectTypeID JOIN `food_perish_state` s ON s.ID = 1 AND s.DoTick = 1 " @
            "SET i.ObjectTypeID = 1032, i.Quality = 10 WHERE i.Quality = 0");
        // 3) forget items that no longer exist or are no longer food
        dbi.Update("DELETE f FROM `food_perish_items` f LEFT JOIN `items` i ON i.ID = f.ItemID LEFT JOIN `food_perish_types` c ON c.ObjectTypeID = i.ObjectTypeID WHERE i.ID IS NULL OR c.ObjectTypeID IS NULL");
        // 4) start tracking new food items (they get one full day before their first spoilage)
        dbi.Update("INSERT IGNORE INTO `food_perish_items` (`ItemID`, `TrackedSince`) SELECT i.ID, NOW() FROM `items` i JOIN `food_perish_types` c ON c.ObjectTypeID = i.ObjectTypeID");
        dbi.Update("UPDATE `food_perish_state` SET `LastTickAt` = NOW(), `DoTick` = 0 WHERE `ID` = 1 AND `DoTick` = 1");
    }
};
activatePackage(LiFxFoodSpoilagePack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxFoodSpoilagePack);
