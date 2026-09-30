// Horned helmet variants (10) and Stonecutter/Breeder outfit skins (2), ported from MMO skins as standalone items. Objects 3643-3654, recipes 6387-6398 (recipes cost the same as the base item).

if (!isObject(LiFxHornSkinPack))
{
    new ScriptObject(LiFxHornSkinPack)
    {
    };
}

package LiFxHornSkinPack
{
    function LiFxHornSkinPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxHornSkinPack);
    }
    function LiFxHornSkinPack::version() {
        return "1.0.0";
    }
    function LiFxHornSkinPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3643,544,'Half Plate Gottlung Helm with Deer Antlers',0,0,0,0,0,0,0,0,4,1,1600,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/Regular_Plate_Gothlung_HornHelmet.png','The short clipped ends of deer antlers that adorn this helm look so exquisite that they bring tears to the eyes of other northerners.',10000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3644,543,'Heavy Full Plate Helm with Deer Antlers',0,0,0,0,0,0,0,0,4,1,2400,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/Heavy_Plate_Gothlung_HornHelmet.png','The Gottlungs see the deer as a proud and noble animal. Its antlers symbolize the branches of the World Tree, on which the trembling worlds of the Sleeper rest.',40000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3645,546,'Royal Full Plate Helm with Moose Antlers',0,0,0,0,0,0,0,0,4,1,2400,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/Royal_Plate_Gothlung_HornHelmet.png','The commander of the Gottlung troops, wearing a gleaming gold helm decorated with the branching antlers of a young moose, is a truly majestic sight. And yet, for some reason, it causes some northerners to burst out laughing.',200000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3646,639,'Khoorsian Horned Helm',0,0,0,0,0,0,0,0,4,1,1000,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/Regular_Scale_Khoor_HornHelmet.png','The thin, delicate antlers of a young steppe deer look like two Heavenly Sabers.',1500,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3647,641,'Heavy Khoorsian Horned Helm',0,0,0,0,0,0,0,0,4,1,2000,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/Heavy_Scale_Khoor_HornHelmet.png','The more beautiful the wife of a Khoorsian warrior, the more ferocious his temper. The more terrifying and dangerous a warrior is in battle, the longer the horns that decorate his heavy helm.',40000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3648,642,'Royal Khoorsian Horned Helm',0,0,0,0,0,0,0,0,4,1,2000,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/Royal_Scale_Khoor_HornHelmet.png','A sign of great honor: four long, curved horns decorating the dome of a khan''s helm. The branches of the Tree, the Heavenly Blades, and the rays of Amate the Sun are all eternal symbols known to every Khoor.',200000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3649,829,'Heavy Chainmail Helm with Bull Horns',0,0,0,0,0,0,0,0,4,1,1600,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/Heavy_Chain_Slavard_HornHelmet.png','A dispute arose at Konung Halvdan''s feast as to whether a man could topple a bull with a single punch. Jarl Hakon the Anvil knocked one down, cut off its horns, and gifted them to the konung. Thus began the trend of decorating helms with animal horns.',40000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3650,853,'Leather Gottlung Helm with Deer Antlers',0,0,0,0,0,0,0,0,4,1,500,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/Regular_Leather_Gothlung_HornHelmet.png','A leather helm humbly adorned with the antlers of a young deer and worn by regular Gottlung troops.',1000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3651,855,'Heavy Leather Helm with Deer Antlers',0,0,0,0,0,0,0,0,4,1,1000,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/Heavy_Leather_Gothlung_HornHelmet.png','A Gottlung helm to which branching deer antlers are attached using an iron mount. It is said the Gottlungs borrowed this tradition from either the northerners or the Khoors. Or maybe it was from the deer themselves.',20000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3652,883,'Heavy Quilted Helm with Cow Horns',0,0,0,0,0,0,0,0,4,1,800,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/Heavy_Padded_Slavard_HornHelmet.png','Affixed to the band of this Slavard helm are cow horns, pointing up like a peasant''s pitchfork. It is unknown how many foes its owner gored or if he had given up his blade, but these helms are now common on the battlefield, as warriors are fond of them.',40000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3653,225,'Fancy Stonecutter''s Outfit',0,0,0,0,0,0,0,0,4,1,1200,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/miner_skinA.png','At times, a lord will don the clothes of a laborer, put a tool on their belt, and walk around as if they were a stonecutter-except their apron is dyed with woad and embroidered with gold. The commoners, gray from head to toe, gaze at its beauty with envy.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3654,225,'Fancy Breeder''s Outfit',0,0,0,0,0,0,0,0,4,1,1200,'',NULL,NULL,NULL,NULL,NULL,NULL,'mod/HornSkinMod/art/2D/Items/breeder_skinA.png','The dress of a distinguished herdsman. It is ill-suited for actual work, and chiefly worn as a display of splendor: it features an emerald tunic with red trim and an apron with shining silver animals, fit to pay respects to a lord.',3000,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6387,'Half Plate Gottlung Helm with Deer Antlers','',NULL,5,60,3643,40,1,0,0,'mod/HornSkinMod/art/2D/Items/Regular_Plate_Gothlung_HornHelmet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6388,'Heavy Full Plate Helm with Deer Antlers','',NULL,5,90,3644,40,1,0,1,'mod/HornSkinMod/art/2D/Items/Heavy_Plate_Gothlung_HornHelmet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6389,'Royal Full Plate Helm with Moose Antlers','',NULL,5,100,3645,40,1,0,1,'mod/HornSkinMod/art/2D/Items/Royal_Plate_Gothlung_HornHelmet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6390,'Khoorsian Horned Helm','',NULL,5,30,3646,40,1,0,0,'mod/HornSkinMod/art/2D/Items/Regular_Scale_Khoor_HornHelmet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6391,'Heavy Khoorsian Horned Helm','',NULL,5,90,3647,40,1,0,1,'mod/HornSkinMod/art/2D/Items/Heavy_Scale_Khoor_HornHelmet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6392,'Royal Khoorsian Horned Helm','',NULL,5,100,3648,40,1,0,1,'mod/HornSkinMod/art/2D/Items/Royal_Scale_Khoor_HornHelmet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6393,'Heavy Chainmail Helm with Bull Horns','',NULL,5,90,3649,40,1,0,1,'mod/HornSkinMod/art/2D/Items/Heavy_Chain_Slavard_HornHelmet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6394,'Leather Gottlung Helm with Deer Antlers','',NULL,25,30,3650,30,1,0,0,'mod/HornSkinMod/art/2D/Items/Regular_Leather_Gothlung_HornHelmet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6395,'Heavy Leather Helm with Deer Antlers','',NULL,25,90,3651,30,1,0,1,'mod/HornSkinMod/art/2D/Items/Heavy_Leather_Gothlung_HornHelmet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6396,'Heavy Quilted Helm with Cow Horns','',NULL,25,90,3652,30,1,0,1,'mod/HornSkinMod/art/2D/Items/Heavy_Padded_Slavard_HornHelmet.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6397,'Fancy Stonecutter''s Outfit','',NULL,25,60,3653,50,1,0,0,'mod/HornSkinMod/art/2D/Items/miner_skinA.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6398,'Fancy Breeder''s Outfit','',NULL,25,60,3654,50,1,0,0,'mod/HornSkinMod/art/2D/Items/breeder_skinA.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6387,293,0,10,15,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6387,261,0,10,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6387,1388,0,5,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6387,1391,0,30,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6387,1389,0,5,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6388,293,0,10,15,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6388,261,0,10,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6388,1388,0,5,2,1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6388,1391,0,25,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6388,1390,0,10,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6389,293,0,10,15,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6389,264,0,10,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6389,1388,0,5,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6389,1391,0,25,1,1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6389,1390,0,10,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6390,293,0,10,15,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6390,1477,0,20,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6390,1392,0,30,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6391,293,0,10,15,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6391,425,0,10,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6391,1392,0,15,2,1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6391,1388,0,10,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6391,1390,0,15,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6392,293,0,10,15,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6392,425,0,10,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6392,1392,0,15,2,1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6392,1388,0,10,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6392,1390,0,15,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6393,293,0,10,15,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6393,261,0,5,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6393,1388,0,5,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6393,1389,0,20,2,1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6393,1390,0,20,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6394,1394,0,10,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6394,1477,0,20,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6394,1393,0,40,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6395,1394,0,10,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6395,425,0,30,1,1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6395,263,0,10,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6395,1390,0,20,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6396,1394,0,10,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6396,261,0,25,1,1)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6396,265,0,10,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6396,1390,0,25,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6397,261,0,10,4,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6397,260,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6397,3554,0,20,5,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6397,295,0,10,40,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6398,261,0,15,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6398,260,0,15,6,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6398,3556,0,10,3,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,6398,295,0,10,40,0)");
    }
};
activatePackage(LiFxHornSkinPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxHornSkinPack);
