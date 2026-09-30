// Elegant cloaks (4) and officer banners (13) ported from Life is Feudal MMO. Objects 3626-3642, recipes 6370-6386.

if (!isObject(LiFxCloakFlagPack))
{
    new ScriptObject(LiFxCloakFlagPack)
    {
    };
}

package LiFxCloakFlagPack
{
    function LiFxCloakFlagPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxCloakFlagPack);
    }
    function LiFxCloakFlagPack::version() {
        return "1.0.0";
    }
    function LiFxCloakFlagPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3626,1491,'Elegant Purple Cloak',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/robe_pink.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3627,1491,'Elegant Red Cloak',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/robe_red.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3628,1491,'Elegant Blue Cloak',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/robe_blue.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3629,1491,'Elegant Orange Cloak',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/robe_orange.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3630,1491,'Captain of the Light Cavalry Banner',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_01.png','This heraldic banner is worn only by the best warriors who do not cover their shiny armor with tabards.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3631,1491,'Captain of the Heavy Cavalry Banner',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_02.png','This scarlet banner with a brocade border is the essential symbol of the light cavalry that was in use in the time of the Vulpiс Empire.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3632,1491,'Banner of the Pikemen''s Commander',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_03.png','Every truly battle-hardened warrior knows this symbol: the head of a unicorn on red canvas has long been the mark of heavy cavalry.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3633,1491,'Banner of the Halberdier Commander',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_04.png','Pikemen love to show off their spears on their banners, however they are drawn. Be they artless silhouettes like arrowheads or elegant sharp-pointed complex shapes, the leaf-green background is constant.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3634,1491,'Captain of the Shieldbearers Banner',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_05.png','A halberd is a serious weapon in principle, and its banner was designed to match: a sturdy bolt of emerald silk emblazoned with the image of the halberd embroidered in silver-white thread.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3635,1491,'Captain of the Heavy Infantry Banner',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_06.png','These light blue infantry banners are always found on the front lines. Under this standard march shieldbearers, protecting their brothers-in-arms the pikemen and archers from enemy arrows.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3636,1491,'Captain of the Berserkers Banner',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_07.png','This piece of thick blue velvet is decorated with a simple, recognizable symbol: a full-height infantry shield.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3637,1491,'Captain of the Swordsmen Banner',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_08.png','Paint from the northern shores gives this banner a pale but fast gold hue. Once, these flags with a white axe terrified the inhabitants of the coastal villages, suddenly appearing through a thick fog accompanied by the cries of warriors and the noise of arriving ships.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3638,1491,'Captain of the Archers Banner',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_09.png','The shape of a two-handed sword is stitched into this pale yellow cloth. Such banners fly high over the fray of battle, sowing fear into the hearts of those who stand against them.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3639,1491,'Captain of the Crossbowmen Banner',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_10.png','The bow in this embroidered complex design is barely visible on the violet banner. Some say that this symbol, like the bow itself, was invented by Aori Goldenhanded.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3640,1491,'Siege Engine Master Banner',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_11.png','This massive purple silk banner is marked with a white symbol that can be seen from afar. This mark looks like all the Slavard runes thrown together, represents the crossbow, as well as all warriors that use them.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3641,1491,'Healer''s Standard',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_13.png','A red clover on a white canvas is the traditional banner of all healers, who are indispensable on any battlefield.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3642,1491,'General''s Standard',0,0,0,0,0,0,0,0,4,1,200,'',0,0,0,0,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_12.png','The sunlight playing on the simple pattern of gold thread gives life to the sparks, and the details shimmer when the wind blows. This proud, shining banner is imbued with a special strength: hundred, thousands of people are willing to follow it into the furious flames of battle...',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6370,'Elegant Purple Cloak','',NULL,25,60,3626,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/robe_pink.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6371,'Elegant Red Cloak','',NULL,25,60,3627,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/robe_red.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6372,'Elegant Blue Cloak','',NULL,25,60,3628,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/robe_blue.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6373,'Elegant Orange Cloak','',NULL,25,60,3629,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/robe_orange.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6374,'Captain of the Light Cavalry Banner','',NULL,25,40,3630,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_01.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6375,'Captain of the Heavy Cavalry Banner','',NULL,25,40,3631,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_02.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6376,'Banner of the Pikemen''s Commander','',NULL,25,40,3632,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_03.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6377,'Banner of the Halberdier Commander','',NULL,25,40,3633,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_04.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6378,'Captain of the Shieldbearers Banner','',NULL,25,40,3634,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_05.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6379,'Captain of the Heavy Infantry Banner','',NULL,25,40,3635,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_06.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6380,'Captain of the Berserkers Banner','',NULL,25,40,3636,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_07.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6381,'Captain of the Swordsmen Banner','',NULL,25,40,3637,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_08.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6382,'Captain of the Archers Banner','',NULL,25,40,3638,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_09.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6383,'Captain of the Crossbowmen Banner','',NULL,25,40,3639,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_10.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6384,'Siege Engine Master Banner','',NULL,25,40,3640,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_11.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6385,'Healer''s Standard','',NULL,25,40,3641,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_13.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6386,'General''s Standard','',NULL,25,40,3642,50,1,0,0,'mod/CloakFlagMod/art/2D/Items/flag_game_12.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6370,261,0,10,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6370,264,0,15,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6370,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6371,261,0,10,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6371,264,0,15,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6371,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6372,261,0,10,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6372,264,0,15,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6372,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6373,261,0,10,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6373,264,0,15,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6373,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6374,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6374,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6374,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6375,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6375,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6375,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6376,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6376,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6376,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6377,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6377,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6377,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6378,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6378,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6378,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6379,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6379,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6379,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6380,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6380,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6380,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6381,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6381,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6381,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6382,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6382,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6382,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6383,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6383,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6383,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6384,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6384,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6384,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6385,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6385,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6385,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6386,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6386,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6386,295,0,10,40,0)");
    }
};
activatePackage(LiFxCloakFlagPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxCloakFlagPack);
