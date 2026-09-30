// King outfits (6), crowns/tiaras (6) and dyed rags (3) ported from Life is Feudal MMO. Objects 3611-3625, recipes 6355-6369.

if (!isObject(LiFxKingOutfitsPack))
{
    new ScriptObject(LiFxKingOutfitsPack)
    {
    };
}

package LiFxKingOutfitsPack
{
    function LiFxKingOutfitsPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxKingOutfitsPack);
    }
    function LiFxKingOutfitsPack::version() {
        return "1.0.0";
    }
    function LiFxKingOutfitsPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3611,1491,'Mantle Red Capelet',0,0,0,0,0,0,0,0,4,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_A_no_crown.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3612,1491,'Checker Capelet Mantle',0,0,0,0,0,0,0,0,4,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_A_SkinA_no_crown.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3613,1491,'Mantle Gold and White Capelet',0,0,0,0,0,0,0,0,4,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_A_SkinB_no_crown.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3614,1491,'Royal Outfit',0,0,0,0,0,0,0,0,4,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_B_no_crown.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3615,1491,'Monarch''s Dress',0,0,0,0,0,0,0,0,4,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_B_SkinA_no_crown.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3616,1491,'Liege''s Garb',0,0,0,0,0,0,0,0,4,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_B_SkinB_no_crown.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3617,1491,'Tiara of the Sleepless Eye',0,0,0,0,0,0,0,0,0,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_A.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3618,1491,'Svefnibrann',0,0,0,0,0,0,0,0,0,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_A_SkinA.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3619,1491,'Crown of the Eternal Watchman',0,0,0,0,0,0,0,0,0,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_A_SkinB.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3620,1491,'Golden Mark',0,0,0,0,0,0,0,0,0,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_B.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3621,1491,'Tiara of the Sleepless Eye (red)',0,0,0,0,0,0,0,0,0,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_B_SkinA.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3622,1491,'Thousand-Strong Host',0,0,0,0,0,0,0,0,0,1,100,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_B_SkinB.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3623,1491,'Dyed Gottlung Rags',0,0,0,0,0,0,0,0,4,1,300,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_eur_v1.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3624,1491,'Dyed Northern Rags',0,0,0,0,0,0,0,0,4,1,300,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_vik_v1.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3625,1491,'Dyed Steppe Rags',0,0,0,0,0,0,0,0,4,1,300,'',0,0,0,0,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_mon_v1.png','',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6355,'Mantle Red Capelet','',NULL,25,70,3611,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_A_no_crown.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6356,'Checker Capelet Mantle','',NULL,25,70,3612,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_A_SkinA_no_crown.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6357,'Mantle Gold and White Capelet','',NULL,25,70,3613,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_A_SkinB_no_crown.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6358,'Royal Outfit','',NULL,25,80,3614,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_B_no_crown.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6359,'Monarch''s Dress','',NULL,25,80,3615,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_B_SkinA_no_crown.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6360,'Liege''s Garb','',NULL,25,80,3616,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Male_King_Outfit_B_SkinB_no_crown.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6361,'Tiara of the Sleepless Eye','',NULL,52,70,3617,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_A.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6362,'Svefnibrann','',NULL,52,70,3618,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_A_SkinA.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6363,'Crown of the Eternal Watchman','',NULL,52,70,3619,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_A_SkinB.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6364,'Golden Mark','',NULL,52,75,3620,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_B.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6365,'Tiara of the Sleepless Eye (red)','',NULL,52,75,3621,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_B_SkinA.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6366,'Thousand-Strong Host','',NULL,52,75,3622,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/Hat_Male_King_Outfit_B_SkinB.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6367,'Dyed Gottlung Rags','',NULL,25,30,3623,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_eur_v1.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6368,'Dyed Northern Rags','',NULL,25,30,3624,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_vik_v1.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6369,'Dyed Steppe Rags','',NULL,25,30,3625,50,1,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_mon_v1.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6355,264,0,15,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6355,261,0,10,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6355,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6356,264,0,15,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6356,261,0,10,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6356,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6357,264,0,15,10,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6357,261,0,10,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6357,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6358,264,0,15,14,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6358,261,0,10,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6358,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6359,264,0,15,14,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6359,261,0,10,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6359,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6360,264,0,15,14,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6360,261,0,10,8,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6360,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6361,2894,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6361,3046,0,20,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6362,2894,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6362,3046,0,20,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6363,2894,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6363,3046,0,20,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6364,2894,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6364,3046,0,20,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6365,2894,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6365,3046,0,20,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6366,2894,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6366,3046,0,20,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6367,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6367,260,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6367,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6368,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6368,260,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6368,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6369,261,0,10,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6369,260,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6369,295,0,10,40,0)");
    }
};
activatePackage(LiFxKingOutfitsPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxKingOutfitsPack);
