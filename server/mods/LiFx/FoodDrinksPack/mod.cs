// DORMANT - MMO food & drinks (8 products 3186-3193, 11 new ingredients 3175-3185, recipes 5952-5959). Every line is commented out on purpose.
// To enable: run E:\ClaudeScratch\project_archive\FoodDrinks_dormant\enable_fooddrinks.ps1 (uncomments this file, the client XML rows and bakes dump.sql), then restart the server.
if (!isObject(LiFxFoodDrinksPack)) { new ScriptObject(LiFxFoodDrinksPack) { }; }

package LiFxFoodDrinksPack
{
    function LiFxFoodDrinksPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxFoodDrinksPack);
    }
    function LiFxFoodDrinksPack::version() {
        return "1.0.0";
    }
    function LiFxFoodDrinksPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3175,250,'Rye',0,0,0,0,0,0,0,0,2,100000,250,'',0,0,0,0,0,0,'art/2D/Items/Rye.png','A grain. Ingredient for soups.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3176,250,'Barley',0,0,0,0,0,0,0,0,2,100000,250,'',0,0,0,0,0,0,'art/2D/Items/Barley.png','A grain. Ingredient for soups.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3177,250,'Oat',0,0,0,0,0,0,0,0,2,100000,250,'',0,0,0,0,0,0,'art/2D/Items/Oat.png','A grain. Ingredient for soups.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3751,232,'Hops',0,0,0,0,0,0,0,0,2,10000,400,'',0,0,0,0,0,0,'art/2D/Items/Hops.png','A crop that can be grown by Farmers. Used for brewing.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3752,232,'Sugar Beet',0,0,0,0,0,0,0,0,2,10000,230,'',0,0,0,0,0,0,'art/2D/Items/SugarBeet.png','A crop that can be grown by Farmers. Used for various cooking recipes.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3178,340,'Sardine',0,0,0,0,0,0,0,0,2,10000,400,'',0,0,0,0,0,0,'mod/FoodDrinksMod/art/2D/Items/sardine.png','A small fish.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3179,340,'Pike',0,0,0,0,0,0,0,0,2,10000,800,'',0,0,0,0,0,0,'mod/FoodDrinksMod/art/2D/Items/pike.png','A freshwater fish.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3180,16,'Fresh Water',0,0,0,0,0,0,0,0,2,1000,1000,'',0,0,0,0,0,0,'art/2D/Items/water.png','A raw material used for cooking and brewing.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3181,279,'White Flour',0,0,0,0,0,0,0,0,2,10000,500,'',0,0,0,0,0,0,'art/2D/Items/flour.png','Fine flour used for baking.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3182,1111,'Oat Malt',0,0,0,0,0,0,0,0,2,100000,500,'',0,0,0,0,0,0,'art/2D/Items/Malt.png','Malted oats used for brewing.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3183,1111,'Rye Malt',0,0,0,0,0,0,0,0,2,100000,500,'',0,0,0,0,0,0,'art/2D/Items/Malt.png','Malted rye used for brewing.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3184,1111,'Wheat Malt',0,0,0,0,0,0,0,0,2,100000,500,'',0,0,0,0,0,0,'art/2D/Items/Malt.png','Malted wheat used for brewing.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3185,211,'Butter',0,0,0,0,0,0,0,0,2,10000,200,'',0,0,0,0,0,0,'art/2D/Items/eggnog.png','Churned butter used for baking.',50,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3186,228,'White Beer',0,0,0,0,0,0,0,0,2,10000,750,'',0,0,0,0,0,0,'art/2D/Items/compote.png','Food (4 ingredients)',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3187,228,'Oat Stout',0,0,0,0,0,0,0,0,2,10000,750,'',0,0,0,0,0,0,'art/2D/Items/compote.png','Food (4 ingredients)',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3188,228,'South Fish Soup',0,0,0,0,0,0,0,0,2,10000,700,'',0,0,0,0,0,0,'art/2D/Items/apple_castle_soup.png','Food (4 ingredients)',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3189,228,'Red Ale',0,0,0,0,0,0,0,0,2,10000,750,'',0,0,0,0,0,0,'art/2D/Items/compote.png','Food (4 ingredients)',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3190,228,'West Fish Soup',0,0,0,0,0,0,0,0,2,10000,700,'',0,0,0,0,0,0,'art/2D/Items/apple_castle_soup.png','Food (4 ingredients)',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3191,228,'Fish Soup',0,0,0,0,0,0,0,0,2,10000,700,'',0,0,0,0,0,0,'art/2D/Items/apple_castle_soup.png','Food (4 ingredients)',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3192,228,'East Fish Soup',0,0,0,0,0,0,0,0,2,10000,700,'',0,0,0,0,0,0,'art/2D/Items/apple_castle_soup.png','Food (4 ingredients)',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3193,228,'Baguette',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'mod/FoodDrinksMod/art/2D/Items/baguette.png','Food (4 ingredients)',3000,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5952,'White Beer','',NULL,24,60,3186,20,1,0,0,'art/2D/Items/compote.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5953,'Oat Stout','',NULL,24,60,3187,20,1,0,0,'art/2D/Items/compote.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5954,'South Fish Soup','',NULL,24,60,3188,20,1,0,0,'art/2D/Items/apple_castle_soup.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5955,'Red Ale','',NULL,24,60,3189,20,1,0,0,'art/2D/Items/compote.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5956,'West Fish Soup','',NULL,24,60,3190,20,1,0,0,'art/2D/Items/apple_castle_soup.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5957,'Fish Soup','',NULL,24,60,3191,20,1,0,0,'art/2D/Items/apple_castle_soup.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5958,'East Fish Soup','',NULL,24,60,3192,20,1,0,0,'art/2D/Items/apple_castle_soup.png')");
        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5959,'Baguette','',NULL,24,60,3193,20,1,0,0,'mod/FoodDrinksMod/art/2D/Items/baguette.png')");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5952,3184,0,30,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5952,3180,0,30,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5952,1110,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5953,3182,0,30,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5953,3180,0,30,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5953,1110,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5954,3175,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5954,3176,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5954,3180,0,12,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5954,3178,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5954,391,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5954,111,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5955,3183,0,30,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5955,3180,0,30,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5955,1110,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5956,352,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5956,3177,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5956,3180,0,12,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5956,3178,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5956,394,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5956,111,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5957,3175,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5957,3176,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5957,3180,0,12,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5957,3178,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5957,3179,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5957,111,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5958,352,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5958,3177,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5958,3180,0,12,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5958,3178,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5958,392,0,12,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5958,111,0,20,30,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5959,3181,0,20,2,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5959,3185,0,20,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5959,3180,0,20,1,0)");
        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5959,114,0,20,30,0)");
    }
};
activatePackage(LiFxFoodDrinksPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxFoodDrinksPack);
