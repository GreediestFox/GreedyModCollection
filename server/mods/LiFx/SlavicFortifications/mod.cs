/**
* <author>GreedyFox</author>
* <description>Slavic fortification variants (from Life is Feudal: Arden) plus new stone/wooden fortification pieces for Lif:YO.
*              24 new buildings, object IDs 2900-2923, recipe IDs 5500-5523, listed under the vanilla Walls / Stone Walls
*              (172) / Wooden Walls (171) / Castle Walls (173) categories (each object inherits the ParentID of the piece it was cloned from).
*              2900-2913: Slav_* re-skins of Stone Wall, Stairs, Inverted Stairs, Tower, Inner Tower, Angular Tower, Gatehouse,
*              Inner Gatehouse, Castle Tower, Castle Tower Angle, Castle Tower with transitions, Castle Gatehouse, Castle Gatehouse with
*              Drawbridge, Castle Wall Hoarding.
*              2914-2921: Stone Wall Corner / no-merlon / drawbridge gatehouse models (MMO client) with plain and Slavic looks.
*              2922-2923: Wooden Wall Corner (wooden_wall_cor) and Wooden Wall Corner (loophole).
*              Each recipe clones the ingredients of the vanilla piece it is based on, so there is no net material gain over vanilla.
*
* Also needed (NOT done by this script): the model files under art/models/3d/construction/fortifications/ on client AND server, the
* 24 <object> blocks in data/cm_objects.xml (client + server) and the client rows in objects_types.xml / recipe.xml /
* recipe_requirement.xml (requirement IDs 91000-91113). art/dump.sql on the server carries the same DB rows.
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*/

if (!isObject(LiFxSlavicFortifications))
{
    new ScriptObject(LiFxSlavicFortifications)
    {
    };
}

package LiFxSlavicFortifications
{
    function LiFxSlavicFortifications::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxSlavicFortifications);
    }

    function LiFxSlavicFortifications::version() {
        return "1.0.0";
    }

    function LiFxSlavicFortifications::dbChanges() {
        // recipe_requirement has no uniqueness constraint and art/dump.sql seeds the same rows every boot: clear first (see Stonemason mod).
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` BETWEEN 5500 AND 5523");

        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2900,172,'Slavic Stone Wall',0,0,1,0,0,0,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_wall.png','',11000,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2901,172,'Slavic Stone Wall with Stairs',0,0,1,0,0,0,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_wall_with_stairs.png','',70000,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2902,172,'Slavic Stone Wall with Inverted Stairs',0,0,1,0,0,0,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_wall_with_stairs.png','',70000,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2903,172,'Slavic Stone Tower',0,0,1,0,0,0,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_tower.png','',64000,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2904,172,'Slavic Stone Inner Tower',0,0,1,0,0,0,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_inner_tower.png','',64000,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2905,172,'Slavic Stone Angular Tower',0,0,1,0,0,0,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_angular_tower.png','',58000,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2906,172,'Slavic Stone Gatehouse',0,0,1,0,0,1,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_gatehouse.png','',139000,86400,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2907,172,'Slavic Stone Inner Gatehouse',0,0,1,0,0,1,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_inner_gatehouse.png','',139000,86400,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2908,130,'Slavic Castle Tower',0,0,1,0,0,0,0,0,0,0,3000000,'',0,0,0,0,0,0,'art/2D/Objects/castle_tower.png','',193250,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2909,130,'Slavic Castle Tower Angle',0,0,1,0,0,0,0,0,0,0,3000000,'',0,0,0,0,0,0,'art/2D/Objects/castle_tower.png','',323700,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2910,130,'Slavic Castle Tower with transitions',0,0,1,0,0,0,0,0,0,0,3000000,'',0,0,0,0,0,0,'art/2D/Objects/castle_tower.png','',193250,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2911,130,'Slavic Castle Gatehouse',0,0,1,0,0,1,0,0,0,0,4000000,'',0,0,0,0,0,0,'art/2D/Objects/castle_gatehouse.png','',554000,86400,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2912,130,'Slavic Castle Gatehouse with Drawbridge',0,0,1,0,0,1,0,0,0,0,4000000,'',0,0,0,0,0,0,'art/2D/Objects/castle_gatehouse.png','',554000,86400,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2913,173,'Slavic Castle Wall Hoarding (no buttress)',0,0,1,0,0,0,0,0,0,0,2600000,'',0,0,0,0,0,0,'art/2D/Objects/castle_wall_with_hoarding.png','',161500,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2914,172,'Stone Wall Corner',0,0,1,0,0,0,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_wall_corner.png','',11000,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2915,172,'Slavic Stone Wall Corner',0,0,1,0,0,0,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_wall_corner.png','',11000,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2916,172,'Stone Wall (no merlon)',0,0,1,0,0,0,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_wall_no_merlon.png','',11000,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2917,172,'Slavic Stone Wall (no merlon)',0,0,1,0,0,0,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_wall_no_merlon.png','',11000,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2918,172,'Stone Gatehouse with Drawbridge',0,0,1,0,0,1,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_gatehouse_db.png','',139000,86400,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2919,172,'Slavic Stone Gatehouse with Drawbridge',0,0,1,0,0,1,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_gatehouse_db.png','',139000,86400,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2920,172,'Stone Inner Gatehouse with Drawbridge',0,0,1,0,0,1,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_inner_gatehouse_db.png','',139000,86400,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2921,172,'Slavic Stone Inner Gatehouse with Drawbridge',0,0,1,0,0,1,0,0,0,0,600000,'',0,0,0,0,0,0,'art/2D/Objects/stone_inner_gatehouse_db.png','',139000,86400,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2922,130,'Wooden Wall Corner',0,0,1,0,0,0,0,0,0,0,400000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_wall_corner.png','',7500,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2923,130,'Wooden Wall Corner (loophole)',0,0,1,0,0,0,0,0,0,0,400000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_wall_corner_loophole.png','',7500,NULL,0,0);");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5500,'Slavic Stone Wall','',32,19,0,2900,30,1,0,0,'art/2D/Recipes/stone_wall.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5501,'Slavic Stone Wall with Stairs','',32,19,0,2901,30,1,0,0,'art/2D/Recipes/stone_wall_with_stairs.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5502,'Slavic Stone Wall with Inverted Stairs','',32,19,0,2902,30,1,0,0,'art/2D/Recipes/stone_wall_with_inverted_stairs.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5503,'Slavic Stone Tower','',32,19,30,2903,35,1,0,0,'art/2D/Recipes/stone_tower.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5504,'Slavic Stone Inner Tower','',32,19,30,2904,35,1,0,0,'art/2D/Recipes/stone_inner_tower.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5505,'Slavic Stone Angular Tower','',32,19,30,2905,35,1,0,0,'art/2D/Recipes/stone_angular_tower.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5506,'Slavic Stone Gatehouse','',32,19,60,2906,35,1,0,0,'art/2D/Recipes/stone_gatehouse.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5507,'Slavic Stone Inner Gatehouse','',32,19,60,2907,35,1,0,0,'art/2D/Recipes/stone_inner_gatehouse.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5508,'Slavic Castle Tower','',32,20,30,2908,25,1,0,0,'art/2D/Recipes/castle_tower.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5509,'Slavic Castle Tower Angle','',32,20,60,2909,20,1,0,0,'art/2D/Recipes/castle_tower_angle.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5510,'Slavic Castle Tower with transitions','',32,20,30,2910,25,1,0,0,'art/2D/Recipes/castle_tower_angle_with_transitions.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5511,'Slavic Castle Gatehouse','',32,20,60,2911,20,1,0,0,'art/2D/Recipes/castle_gatehouse.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5512,'Slavic Castle Gatehouse with Drawbridge','',32,20,60,2912,20,1,0,0,'art/2D/Recipes/castle_gatehouse_with_drawbridge.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5513,'Slavic Castle Wall Hoarding (no buttress)','',32,20,30,2913,25,1,0,0,'art/2D/Recipes/castle_wall_hoarding_no_buttres.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5514,'Stone Wall Corner','',32,19,0,2914,30,1,0,0,'art/2D/Recipes/stone_wall_corner.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5515,'Slavic Stone Wall Corner','',32,19,0,2915,30,1,0,0,'art/2D/Recipes/stone_wall_corner.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5516,'Stone Wall (no merlon)','',32,19,0,2916,30,1,0,0,'art/2D/Recipes/stone_wall_no_merlon.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5517,'Slavic Stone Wall (no merlon)','',32,19,0,2917,30,1,0,0,'art/2D/Recipes/stone_wall_no_merlon.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5518,'Stone Gatehouse with Drawbridge','',32,19,60,2918,35,1,0,0,'art/2D/Recipes/stone_gatehouse_db.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5519,'Slavic Stone Gatehouse with Drawbridge','',32,19,60,2919,35,1,0,0,'art/2D/Recipes/stone_gatehouse_db.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5520,'Stone Inner Gatehouse with Drawbridge','',32,19,60,2920,35,1,0,0,'art/2D/Recipes/stone_gatehouse_db_inner.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5521,'Slavic Stone Inner Gatehouse with Drawbridge','',32,19,60,2921,35,1,0,0,'art/2D/Recipes/stone_gatehouse_db_inner.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5522,'Wooden Wall Corner','',32,18,0,2922,30,1,0,0,'art/2D/Recipes/wooden_wall_corner.png');");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5523,'Wooden Wall Corner (loophole)','',32,18,30,2923,30,1,0,0,'art/2D/Recipes/wooden_wall_corner_loophole.png');");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5500,269,0,45,30,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5500,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5500,235,0,10,10,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5500,244,0,10,200,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5501,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5501,235,0,10,50,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5501,269,0,40,60,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5501,244,0,10,400,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5501,233,0,5,10,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5502,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5502,235,0,10,50,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5502,269,0,40,60,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5502,244,0,10,400,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5502,233,0,5,10,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5503,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5503,235,0,10,80,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5503,269,0,40,100,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5503,244,0,10,600,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5504,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5504,235,0,10,80,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5504,269,0,40,100,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5504,244,0,10,600,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5505,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5505,235,0,10,70,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5505,269,0,40,100,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5505,244,0,10,600,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5506,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5506,235,0,5,150,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5506,269,0,40,250,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5506,244,0,5,1400,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5506,287,0,10,1,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5507,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5507,235,0,5,150,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5507,269,0,40,250,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5507,244,0,5,1400,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5507,287,0,10,1,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5508,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5508,326,0,10,30,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5508,269,0,15,20,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5508,271,0,30,600,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5508,528,0,10,75,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5508,281,0,5,300,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5509,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5509,326,0,10,100,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5509,269,0,15,30,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5509,271,0,30,800,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5509,528,0,10,90,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5509,281,0,10,600,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5510,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5510,326,0,10,30,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5510,269,0,15,20,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5510,271,0,30,600,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5510,528,0,10,75,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5510,281,0,5,300,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5511,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5511,326,0,10,200,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5511,269,0,15,80,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5511,271,0,30,1600,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5511,528,0,10,200,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5511,287,0,10,2,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5512,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5512,326,0,10,200,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5512,269,0,15,80,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5512,271,0,30,1600,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5512,528,0,10,200,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5512,287,0,10,2,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5513,326,0,5,50,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5513,269,0,15,60,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5513,271,0,30,400,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5513,528,0,10,50,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5513,282,0,10,10,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5513,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5514,269,0,45,30,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5514,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5514,235,0,10,10,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5514,244,0,10,200,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5515,269,0,45,30,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5515,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5515,235,0,10,10,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5515,244,0,10,200,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5516,269,0,45,30,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5516,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5516,235,0,10,10,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5516,244,0,10,200,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5517,269,0,45,30,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5517,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5517,235,0,10,10,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5517,244,0,10,200,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5518,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5518,235,0,5,150,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5518,269,0,40,250,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5518,244,0,5,1400,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5518,287,0,10,1,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5519,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5519,235,0,5,150,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5519,269,0,40,250,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5519,244,0,5,1400,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5519,287,0,10,1,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5520,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5520,235,0,5,150,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5520,269,0,40,250,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5520,244,0,5,1400,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5520,287,0,10,1,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5521,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5521,235,0,5,150,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5521,269,0,40,250,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5521,244,0,5,1400,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5521,287,0,10,1,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5522,234,0,45,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5522,235,0,20,5,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5522,32,0,5,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5523,234,0,45,15,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5523,235,0,20,5,0);");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5523,32,0,5,15,0);");
    }
};
activatePackage(LiFxSlavicFortifications);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxSlavicFortifications);
