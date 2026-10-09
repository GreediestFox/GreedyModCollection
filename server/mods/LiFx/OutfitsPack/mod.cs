// Stonecutter's and Breeder's Outfits (MMO), using look-alike meshes of the Blacksmith's/Carpenter's outfits. Objects 3591-3592, recipes 6346-6347. 2026-10-03: Meister tiers 3925-3929 (recipes 6483-6487) + DB effects 46-56.

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
        //////////////// Geselle/Meister tiers (2026-10-03) ////////////////
        // DB effects 46-56 -> merged Skill-Raised player effects (cm_effects.xml); item mapping in data\item_effects.xml.
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (46,'Skill Raised: Healing, Herbalism',NULL,NULL,31)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (47,'Skill Raised: Construction, Building Maintain',NULL,NULL,77)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (48,'Skill Raised: Procuration',NULL,NULL,35)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (49,'Skill Raised: Materials Preparation',NULL,NULL,78)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (50,'Skill Raised: Carpentry, Bowcraft, Warfare engineering',NULL,NULL,29)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (51,'Skill Raised: Forging, Armorsmithing',NULL,NULL,27)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (52,'Skill Raised: Cooking, Brewing',NULL,NULL,80)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (53,'Skill Raised: Herbalism, Healing',NULL,NULL,32)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (54,'Skill Raised: Construction, Building Maintain, Masonry, Architecture',NULL,NULL,34)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (55,'Skill Raised: Procuration, Tailoring, Warhorse training',NULL,NULL,81)");
        dbi.Update("INSERT IGNORE INTO `effects` (`ID`,`Effect_name`,`ResultPreparationID`,`ResultPotionID`,`PlayerEffectID`) VALUES (56,'Skill Raised: Materials Preparation, Mining',NULL,NULL,85)");
        // Meister outfits 3925-3929 = former wardrobe skins of 303-307 as own items (Tailor's Workshop only, Tailoring 90).
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3925,225,'Master Blacksmith''s Outfit',0,0,0,0,0,0,0,0,4,1,1200,'',0,0,0,0,0,0,'art/2D/equipIcons/Outfits/Male_Craft_Blacksmith_SkinA.png','A black shirt with a leather apron covering the smith''s body and legs. One''s gaze becomes lost in the crimson patterns and symbols, the secret nature of which is known only to Accurs and the smiths themselves. The interwoven symbols frame a ruby Eye of the Sleeper with a sheen of metal dust. The apron is belted with a thin woven belt on which tongs and small hammers are hung in strict order to make sure that every tool is available and in the right place.',100000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3926,225,'Master Carpenter''s Outfit',0,0,0,0,0,0,0,0,4,1,1200,'',0,0,0,0,0,0,'art/2D/equipIcons/Outfits/Male_Craft_Carpenter_SkinA.png','A foppish getup worn by city-dwelling carpenters and joiners. Apparently this kind of clothing is, to a large extent, a sign of mastery and a symbol of belonging to the profession, but it is also especially good for work — convenient pockets sewn onto the apron and jacket, sturdy, dirt-resistant pants made of boiled leather, and loops on the straps for tools.',100000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3927,225,'Master Alchemist''s Outfit',0,0,0,0,0,0,0,0,4,1,1200,'',0,0,0,0,0,0,'art/2D/equipIcons/Outfits/Male_Craft_Alchemist_SkinA.png','A decorated potion-brewer''s cloak made of magenta linen. It is light and spacious, but fits very well, with sleeves with strings for greater comfort. This outfit has a special feature: a belt not unlike a sword belt on which hang countless chubby flasks of smoky glass.',100000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3928,225,'Master Engineer''s Outfit',0,0,0,0,0,0,0,0,4,1,1200,'',0,0,0,0,0,0,'art/2D/equipIcons/Outfits/Male_Craft_Engineer_SkinA.png','A crimson shirt adorned with dull golden patterns. From the outfit''s countless tassels hang bronze tools whose names are known only to the engineers themselves... if they are known to anyone at all. Measurements and angles, which are absolutely necessary for various drafting and engineering projects, are sewn into the leather jacket, which is belted with a broad belt.',100000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3929,225,'Master Cook''s Outfit',0,0,0,0,0,0,0,0,4,1,1200,'',0,0,0,0,0,0,'art/2D/equipIcons/Outfits/Male_Craft_Cook_SkinA.png','A cook''s outfit — a blue cotton vest, a shirt of light-colored linen, and, most importantly, a broad apron made of white fabric with pockets for salt and spices, as well as countless straps. A handy thing for attaching knives, spoon, ladles, and other crucial cooking utensils to.',100000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6483,'Master Blacksmith''s Outfit','',2836,25,90,3925,60,1,0,0,'art/2D/equipIcons/Outfits/Male_Craft_Blacksmith_SkinA.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6483,303,0,20,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6483,261,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6483,266,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6483,424,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6483,3049,0,20,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6483,2836,0,20,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6484,'Master Carpenter''s Outfit','',2836,25,90,3926,60,1,0,0,'art/2D/equipIcons/Outfits/Male_Craft_Carpenter_SkinA.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6484,304,0,20,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6484,261,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6484,266,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6484,424,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6484,3052,0,20,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6484,2836,0,20,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6485,'Master Alchemist''s Outfit','',2836,25,90,3927,60,1,0,0,'art/2D/equipIcons/Outfits/Male_Craft_Alchemist_SkinA.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6485,305,0,20,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6485,261,0,10,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6485,266,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6485,425,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6485,3052,0,20,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6485,2836,0,20,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6486,'Master Engineer''s Outfit','',2836,25,90,3928,60,1,0,0,'art/2D/equipIcons/Outfits/Male_Craft_Engineer_SkinA.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6486,306,0,20,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6486,261,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6486,266,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6486,424,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6486,3049,0,20,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6486,2836,0,20,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6487,'Master Cook''s Outfit','',2836,25,90,3929,60,1,0,0,'art/2D/equipIcons/Outfits/Male_Craft_Cook_SkinA.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6487,307,0,20,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6487,261,0,10,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6487,266,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6487,424,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6487,3052,0,20,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6487,2836,0,20,40,0)");
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
