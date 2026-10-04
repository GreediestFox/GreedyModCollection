// Furniture ported from Life is Feudal MMO (movable). Objects 3153-3160, carried twins 3161-3168, recipes 5941-5948.

if (!isObject(LiFxFurniturePack))
{
    new ScriptObject(LiFxFurniturePack)
    {
    };
}

package LiFxFurniturePack
{
    function LiFxFurniturePack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxFurniturePack);
    }
    function LiFxFurniturePack::version() {
        return "1.0.0";
    }
    function LiFxFurniturePack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3153,1637,'Canopy Bed',0,1,0,0,0,0,0,0,3,0,5000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/decorablebed_01.png','',57600,120,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3161,1902,'Canopy Bed',0,0,0,0,0,0,0,0,3,1,5000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/decorablebed_01.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3154,1637,'Carved Bench',0,1,0,0,0,0,0,0,3,0,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/expensivebench_01.png','',57600,120,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3162,1902,'Carved Bench',0,0,0,0,0,0,0,0,3,1,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/expensivebench_01.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3155,1637,'Carved Slavard Bench',0,1,0,0,0,0,0,0,3,0,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/bench_vI.png','',57600,120,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3163,1902,'Carved Slavard Bench',0,0,0,0,0,0,0,0,3,1,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/bench_vI.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3156,1637,'Ornate Gottlung Bench',0,1,0,0,0,0,0,0,3,0,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/bench_vII.png','',57600,120,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3164,1902,'Ornate Gottlung Bench',0,0,0,0,0,0,0,0,3,1,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/bench_vII.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3157,1637,'Ornate Slavard Table',0,1,0,0,0,0,0,0,3,0,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/expensivetable_04.png','',57600,120,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3165,1902,'Ornate Slavard Table',0,0,0,0,0,0,0,0,3,1,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/expensivetable_04.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3158,1637,'Ornate Slavard Throne',0,1,0,0,0,0,0,0,3,0,5000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/throne_vI.png','',57600,120,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3166,1902,'Ornate Slavard Throne',0,0,0,0,0,0,0,0,3,1,5000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/throne_vI.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3159,1637,'Simple Table',0,1,0,0,0,0,0,0,3,0,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/table_02.png','',57600,120,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3167,1902,'Simple Table',0,0,0,0,0,0,0,0,3,1,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/table_02.png','',NULL,NULL,0,0)");
        // [removed 2026-09-30] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3160,1637,'Primitive Bed',0,1,0,0,0,0,0,0,1,0,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/sleeping_bag.png','',57600,120,0,0)");
        // [removed 2026-09-30] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3168,1902,'Primitive Bed',0,0,0,0,0,0,0,0,1,1,1000,'',0,0,0,0,0,0,'mod/FurnitureMod/art/2D/Recipes/sleeping_bag.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5941,'Canopy Bed','Ported from Life is Feudal MMO',NULL,8,100,3153,10,1,0,0,'mod/FurnitureMod/art/2D/Recipes/decorablebed_01.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5942,'Carved Bench','Ported from Life is Feudal MMO',NULL,8,100,3154,10,1,0,0,'mod/FurnitureMod/art/2D/Recipes/expensivebench_01.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5943,'Carved Slavard Bench','Ported from Life is Feudal MMO',NULL,8,100,3155,10,1,0,0,'mod/FurnitureMod/art/2D/Recipes/bench_vI.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5944,'Ornate Gottlung Bench','Ported from Life is Feudal MMO',NULL,8,100,3156,10,1,0,0,'mod/FurnitureMod/art/2D/Recipes/bench_vII.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5945,'Ornate Slavard Table','Ported from Life is Feudal MMO',NULL,8,100,3157,10,1,0,0,'mod/FurnitureMod/art/2D/Recipes/expensivetable_04.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5946,'Ornate Slavard Throne','Ported from Life is Feudal MMO',NULL,8,100,3158,10,1,0,0,'mod/FurnitureMod/art/2D/Recipes/throne_vI.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5947,'Simple Table','Ported from Life is Feudal MMO',NULL,8,30,3159,10,1,0,0,'mod/FurnitureMod/art/2D/Recipes/table_02.png')");
        // [removed 2026-09-30] dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5948,'Primitive Bed','Ported from Life is Feudal MMO',NULL,8,0,3160,30,1,0,0,'mod/FurnitureMod/art/2D/Recipes/sleeping_bag.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5941,326,0,23,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5941,327,0,23,60,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5941,264,0,22,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5941,347,0,22,200,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5942,327,0,45,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5942,281,0,45,60,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5943,327,0,45,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5943,281,0,45,60,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5944,327,0,45,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5944,281,0,45,60,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5945,327,0,45,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5945,281,0,45,60,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5946,327,0,30,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5946,281,0,30,60,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5946,347,0,30,60,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5947,327,0,90,4,0)");
        // [removed 2026-09-30] dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5948,326,0,30,2,0)");
        // [removed 2026-09-30] dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5948,1477,0,20,4,0)");
        // [removed 2026-09-30] dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5948,471,0,10,30,0)");
        // [removed 2026-09-30] dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5948,36,0,10,30,0)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3153,3161)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3154,3162)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3155,3163)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3156,3164)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3157,3165)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3158,3166)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3159,3167)");
        // [removed 2026-09-30] dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3160,3168)");
    }
};
activatePackage(LiFxFurniturePack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxFurniturePack);
