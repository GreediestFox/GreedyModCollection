-- Halved weight of mined/dug ground materials, ores excluded. Append to the tail of the server art/dump.sql.
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=241;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=242;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=243;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=244;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=245;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=246;
UPDATE `objects_types` SET `UnitWeight`=100 WHERE `ID`=247;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=665;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=332;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=333;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=334;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=3582;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=3583;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=3584;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=3585;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=1450;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=1451;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=1452;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=1453;
UPDATE `objects_types` SET `UnitWeight`=500 WHERE `ID`=1454;
-- GM Robe invulnerability
INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (57,'Invulnerable',NULL,NULL,45);
