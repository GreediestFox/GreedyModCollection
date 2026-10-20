// RecipeRevisionPack: recipe changes from the sheet "Rezepte (Änderungen rot)" (GreedyFox / team), generated 2026-10-17 by tools in project_archive\Session_2026-10-14\RecipeRevision.



// - 145 recipe rows changed, 371 recipes with changed ingredients, 38 recipes removed, 23 new placeholder items (ids 3970-3992), 55 new recipes (ids 6510-6570).



// - Ingredient changes are matched by recipe + material (requirement ids are renumbered by other mods). Added rows are deleted by recipe+material before they are inserted (auto ids), so a restart never duplicates them. Recipes 6510-6599 belong to this mod only and are rebuilt on every boot.



// - The server re-seeds recipe/recipe_requirement from art\dump.sql on every boot, then this runs again: everything here is repeatable.



// - Folder name starts with ZZ_ on purpose: mods run their dbChanges in folder order, and TailorsWorkshop (re)creates the workshop twins 5100-5164 - this mod must run AFTER it.



// - The client needs the same data: data\recipe.xml + data\recipe_requirement.xml (shipped through the GMC release).







if (!isObject(LiFxRecipeRevisionPack))



{



    new ScriptObject(LiFxRecipeRevisionPack)



    {



    };



}







package LiFxRecipeRevisionPack



{



    function LiFxRecipeRevisionPack::setup() {



        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxRecipeRevisionPack);



    }



    function LiFxRecipeRevisionPack::version() {



        return "1.0.0";



    }



    function LiFxRecipeRevisionPack::dbChanges() {



        // new placeholder items



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3970,207,'Dyed Linen Cloth',0,0,0,0,0,0,0,0,3,10000,2000,'',0,0,0,0,0,0,'art/2D/Items/placeholder_dyed_linen.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3971,207,'Dyed Silk Cloth',0,0,0,0,0,0,0,0,3,10000,2500,'',0,0,0,0,0,0,'art/2D/Items/placeholder_dyed_silk.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3972,207,'Dyed Wool Cloth',0,0,0,0,0,0,0,0,3,10000,2000,'',0,0,0,0,0,0,'art/2D/Items/placeholder_dyed_wool.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3973,205,'Sword Hilt',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_sword_hilt.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3974,205,'Wooden Shaft',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_wooden_shaft.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3975,205,'Metal Shaft',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_metal_shaft.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3976,207,'Padding',0,0,0,0,0,0,0,0,3,10000,2000,'',0,0,0,0,0,0,'art/2D/Items/placeholder_padding.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3977,207,'Fine Padding',0,0,0,0,0,0,0,0,3,10000,2500,'',0,0,0,0,0,0,'art/2D/Items/placeholder_fine_padding.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3978,205,'Arrowhead',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_arrowhead.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3979,205,'Bodkin Point',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_bodkin_point.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3980,205,'Broadhead Point',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_broadhead_point.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3981,205,'Blunt Point',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_blunt_point.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3982,16,'Dye',0,0,0,0,0,0,0,0,1,1000,1000,'',0,0,0,0,0,0,'art/2D/Items/placeholder_dye.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3983,16,'Salt',0,0,0,0,0,0,0,0,1,1000,1000,'',0,0,0,0,0,0,'art/2D/Items/placeholder_salt.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3984,63,'Salt Barrel',1,1,0,0,0,0,0,300000,3,0,5000,'art\\\\images\\\\universal',0,0,0,0,0,0,'art/2D/Items/placeholder_salt_barrel.png','',7800,150,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3985,205,'Nordmar Mortise Chisel',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_nordmar_chisel.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3986,205,'Mesyrna Herbarium and Athanor',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_mesyrna_herbarium.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3987,205,'Valesos Cauldron',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_valesos_cauldron.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3988,205,'Valesos Diamond Whetstone',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_valesos_whetstone.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3989,205,'Mesyrna Sewing Pattern',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_mesyrna_pattern.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3990,205,'Better Plates',0,0,0,0,0,0,0,0,2,10000,600,'',0,0,0,0,0,0,'art/2D/Items/placeholder_better_plates.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        // dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3991,1491,'Pilgrim''s Clothes',0,0,0,0,0,0,0,0,4,1,100,'',0,0,0,0,0,0,'art/2D/Items/placeholder_pilgrim_clothes.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        // dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3992,1491,'Fancy Decorated Clothes',0,0,0,0,0,0,0,0,4,1,100,'',0,0,0,0,0,0,'art/2D/Items/placeholder_fancy_decorated.png','Placeholder item. Final item and icon follow.',50,NULL,0,0)");



        // recipe rows



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=100 WHERE `ID`=3");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=30 WHERE `ID`=22");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=35");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=30 WHERE `ID`=37");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=30 WHERE `ID`=47");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=35 WHERE `ID`=73");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=45,`Quantity`=2 WHERE `ID`=129");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=5 WHERE `ID`=187");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2894 WHERE `ID`=249");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2894 WHERE `ID`=250");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2894 WHERE `ID`=251");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2894 WHERE `ID`=252");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2894 WHERE `ID`=253");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2894 WHERE `ID`=254");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2894 WHERE `ID`=255");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2894 WHERE `ID`=256");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2894 WHERE `ID`=257");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2894 WHERE `ID`=258");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=30 WHERE `ID`=276");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=30 WHERE `ID`=277");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=30 WHERE `ID`=296");



        dbi.Update("UPDATE `recipe` SET `Quantity`=10 WHERE `ID`=297");



        dbi.Update("UPDATE `recipe` SET `Quantity`=10 WHERE `ID`=299");



        dbi.Update("UPDATE `recipe` SET `Quantity`=10 WHERE `ID`=300");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=301");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=302");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=304");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=305");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=306");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=307");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=308");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=309");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=310");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=311");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=313");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=90 WHERE `ID`=314");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=90 WHERE `ID`=315");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=320");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=90 WHERE `ID`=321");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=323");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=326");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=90 WHERE `ID`=330");



        dbi.Update("UPDATE `recipe` SET `SkillTypeID`=8 WHERE `ID`=332");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=90 WHERE `ID`=336");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=337");



        dbi.Update("UPDATE `recipe` SET `SkillLvl`=60 WHERE `ID`=340");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=369");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=370");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=371");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=372");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=373");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=374");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=376");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20,`IsBlueprint`=0 WHERE `ID`=381");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`IsBlueprint`=0 WHERE `ID`=382");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`IsBlueprint`=0 WHERE `ID`=383");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20,`IsBlueprint`=0 WHERE `ID`=384");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20,`IsBlueprint`=0 WHERE `ID`=385");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`IsBlueprint`=0 WHERE `ID`=386");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20,`IsBlueprint`=0 WHERE `ID`=387");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`IsBlueprint`=0 WHERE `ID`=388");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`IsBlueprint`=0 WHERE `ID`=389");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`IsBlueprint`=0 WHERE `ID`=390");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`IsBlueprint`=0 WHERE `ID`=391");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`IsBlueprint`=0 WHERE `ID`=392");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20 WHERE `ID`=405");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836 WHERE `ID`=406");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20 WHERE `ID`=407");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20 WHERE `ID`=408");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20 WHERE `ID`=409");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20 WHERE `ID`=410");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20 WHERE `ID`=411");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836 WHERE `ID`=412");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20 WHERE `ID`=413");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20 WHERE `ID`=414");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20 WHERE `ID`=415");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillDepends`=20 WHERE `ID`=416");



        dbi.Update("UPDATE `recipe` SET `Quantity`=12 WHERE `ID`=448");



        dbi.Update("UPDATE `recipe` SET `Quantity`=2 WHERE `ID`=481");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=499");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=500");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=501");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=502");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=503");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=504");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=505");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=506");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=507");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=508");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=509");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141,`Quantity`=1 WHERE `ID`=510");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141 WHERE `ID`=511");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141 WHERE `ID`=512");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141 WHERE `ID`=513");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141 WHERE `ID`=514");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141 WHERE `ID`=515");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141 WHERE `ID`=516");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141 WHERE `ID`=517");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141 WHERE `ID`=518");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141 WHERE `ID`=519");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141 WHERE `ID`=520");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=141 WHERE `ID`=522");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=45 WHERE `ID`=552");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=35 WHERE `ID`=593");



        dbi.Update("UPDATE `recipe` SET `SkillTypeID`=1,`SkillLvl`=30,`SkillDepends`=20 WHERE `ID`=626");



        dbi.Update("UPDATE `recipe` SET `SkillTypeID`=8,`SkillLvl`=100 WHERE `ID`=658");



        dbi.Update("UPDATE `recipe` SET `Quantity`=10 WHERE `ID`=872");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=40 WHERE `ID`=890");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=136 WHERE `ID`=899");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=136 WHERE `ID`=900");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=136 WHERE `ID`=901");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=136 WHERE `ID`=902");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=136 WHERE `ID`=903");



        dbi.Update("UPDATE `recipe` SET `Quantity`=5 WHERE `ID`=904");



        dbi.Update("UPDATE `recipe` SET `IsBlueprint`=0 WHERE `ID`=1004");



        dbi.Update("UPDATE `recipe` SET `IsBlueprint`=0 WHERE `ID`=1005");



        dbi.Update("UPDATE `recipe` SET `IsBlueprint`=0 WHERE `ID`=1006");



        dbi.Update("UPDATE `recipe` SET `IsBlueprint`=0 WHERE `ID`=1007");



        dbi.Update("UPDATE `recipe` SET `Quantity`=10 WHERE `ID`=1021");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=5109");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=5110");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=5111");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=5112");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=5113");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=5114");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=25 WHERE `ID`=5116");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=40 WHERE `ID`=5161");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillLvl`=90,`SkillDepends`=30 WHERE `ID`=6360");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillLvl`=90,`SkillDepends`=30 WHERE `ID`=6359");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillLvl`=90,`SkillDepends`=30 WHERE `ID`=6358");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillLvl`=90,`SkillDepends`=30 WHERE `ID`=6356");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillLvl`=90,`SkillDepends`=30 WHERE `ID`=6357");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillLvl`=90,`SkillDepends`=30 WHERE `ID`=6355");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillLvl`=90,`SkillDepends`=30 WHERE `ID`=6372");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillLvl`=90,`SkillDepends`=30 WHERE `ID`=6370");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=2836,`SkillLvl`=90,`SkillDepends`=30 WHERE `ID`=6371");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=40 WHERE `ID`=6483");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=50 WHERE `ID`=6484");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=45 WHERE `ID`=6485");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=40 WHERE `ID`=6486");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=45 WHERE `ID`=6487");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=295,`SkillDepends`=45 WHERE `ID`=6347");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=45 WHERE `ID`=6398");



        dbi.Update("UPDATE `recipe` SET `StartingToolsID`=295 WHERE `ID`=6346");



        dbi.Update("UPDATE `recipe` SET `SkillDepends`=50 WHERE `ID`=6397");



        // clean up rows of earlier versions of this mod (recipes 6510-6599 belong to this mod only)



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID BETWEEN 6510 AND 6599");



        dbi.Update("DELETE FROM `recipe` WHERE ID BETWEEN 6510 AND 6599");



        // requirement changes (matched by recipe + material, so they survive renumbered ids)



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=374,Quality=0,Influence=10,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=2 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1130,Quality=0,Influence=10,Quantity=5,IsRegionItemRequired=1 WHERE RecipeID=2 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3800,Quality=50,Influence=30,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=2 AND MaterialObjectTypeID=347");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=374,Quality=0,Influence=10,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=3 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1130,Quality=0,Influence=10,Quantity=5,IsRegionItemRequired=1 WHERE RecipeID=3 AND MaterialObjectTypeID=264");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3800,Quality=50,Influence=30,Quantity=8,IsRegionItemRequired=1 WHERE RecipeID=3 AND MaterialObjectTypeID=347");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3800,Quality=50,Influence=40,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=5 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3800,Quality=50,Influence=30,Quantity=32,IsRegionItemRequired=0 WHERE RecipeID=8 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=8 AND MaterialObjectTypeID=347");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=269,Quality=0,Influence=20,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=20 AND MaterialObjectTypeID=241");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=20 AND MaterialObjectTypeID=262");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3935,Quality=0,Influence=30,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=22 AND MaterialObjectTypeID=234");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=24 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=25 AND MaterialObjectTypeID=347");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=12,IsRegionItemRequired=0 WHERE RecipeID=28 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=50,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=29 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3802,Quality=0,Influence=10,Quantity=12,IsRegionItemRequired=0 WHERE RecipeID=35 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=25,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=35 AND MaterialObjectTypeID=222");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=25,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=35 AND MaterialObjectTypeID=223");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1029,Quality=0,Influence=40,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=37 AND MaterialObjectTypeID=283");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3935,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=38 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=269,Quality=0,Influence=10,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=40 AND MaterialObjectTypeID=283");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=374,Quality=0,Influence=10,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=41 AND MaterialObjectTypeID=1356");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1131,Quality=0,Influence=20,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=41 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3802,Quality=0,Influence=30,Quantity=12,IsRegionItemRequired=0 WHERE RecipeID=41 AND MaterialObjectTypeID=281");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=269,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=42 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=324,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=42 AND MaterialObjectTypeID=241");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=47 AND MaterialObjectTypeID=374");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3934,Quality=40,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=48 AND MaterialObjectTypeID=325");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=49 AND MaterialObjectTypeID=374");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3935,Quality=0,Influence=10,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=70 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3985,Quality=0,Influence=0,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=71 AND MaterialObjectTypeID=528");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=300,IsRegionItemRequired=0 WHERE RecipeID=71 AND MaterialObjectTypeID=244");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=71 AND MaterialObjectTypeID=32");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3986,Quality=0,Influence=0,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=72 AND MaterialObjectTypeID=528");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=300,IsRegionItemRequired=0 WHERE RecipeID=72 AND MaterialObjectTypeID=244");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=72 AND MaterialObjectTypeID=32");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3051,Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=1 WHERE RecipeID=73 AND MaterialObjectTypeID=234");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3802,Quality=0,Influence=15,Quantity=40,IsRegionItemRequired=0 WHERE RecipeID=73 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3987,Quality=0,Influence=0,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=73 AND MaterialObjectTypeID=403");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=73 AND MaterialObjectTypeID=282");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3802,Quality=0,Influence=20,Quantity=18,IsRegionItemRequired=0 WHERE RecipeID=110 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=113 AND MaterialObjectTypeID=254");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=254,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=114 AND MaterialObjectTypeID=237");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=50,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=117 AND MaterialObjectTypeID=368");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=119 AND MaterialObjectTypeID=254");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=121 AND MaterialObjectTypeID=254");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1408,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=122 AND MaterialObjectTypeID=254");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=60,Influence=15,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=126 AND MaterialObjectTypeID=364");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=366,Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=128 AND MaterialObjectTypeID=364");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=25,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=129 AND MaterialObjectTypeID=216");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1130,Quality=0,Influence=15,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=130 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=132 AND MaterialObjectTypeID=476");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=138 AND MaterialObjectTypeID=476");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=186 AND MaterialObjectTypeID=236");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,186,236,0,5,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=186 AND MaterialObjectTypeID=204");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=187 AND MaterialObjectTypeID=236");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,187,236,0,5,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=187 AND MaterialObjectTypeID=204");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=75,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=189 AND MaterialObjectTypeID=352");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=191 AND MaterialObjectTypeID=236");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,191,236,0,5,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=196 AND MaterialObjectTypeID=366");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=326,Quality=0,Influence=20,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=199 AND MaterialObjectTypeID=293");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=518,Quality=0,Influence=10,Quantity=40,IsRegionItemRequired=0 WHERE RecipeID=199 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=366,Quality=0,Influence=50,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=202 AND MaterialObjectTypeID=368");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3051,Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=211 AND MaterialObjectTypeID=403");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=212 AND MaterialObjectTypeID=366");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=409,Quality=0,Influence=50,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=213 AND MaterialObjectTypeID=366");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=214 AND MaterialObjectTypeID=366");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=220 AND MaterialObjectTypeID=263");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=220 AND MaterialObjectTypeID=264");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=220 AND MaterialObjectTypeID=266");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3970,Quality=0,Influence=15,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=224 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3970,Quality=0,Influence=15,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=226 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3990,Quality=0,Influence=40,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=244 AND MaterialObjectTypeID=366");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3985,Quality=0,Influence=0,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=276 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=25,Quantity=140,IsRegionItemRequired=0 WHERE RecipeID=276 AND MaterialObjectTypeID=233");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3986,Quality=0,Influence=0,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=277 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=25,Quantity=140,IsRegionItemRequired=0 WHERE RecipeID=277 AND MaterialObjectTypeID=233");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=897,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=281 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=281 AND MaterialObjectTypeID=1390");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=898,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=282 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=899,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=283 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=900,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=284 AND MaterialObjectTypeID=263");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=901,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=285 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=902,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=286 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=288 AND MaterialObjectTypeID=325");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=289 AND MaterialObjectTypeID=1356");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3802,Quality=0,Influence=40,Quantity=2,IsRegionItemRequired=1 WHERE RecipeID=292 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3802,Quality=0,Influence=40,Quantity=1,IsRegionItemRequired=1 WHERE RecipeID=293 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3802,Quality=0,Influence=35,Quantity=1,IsRegionItemRequired=1 WHERE RecipeID=294 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=294 AND MaterialObjectTypeID=344");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3802,Quality=0,Influence=30,Quantity=2,IsRegionItemRequired=1 WHERE RecipeID=295 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=295 AND MaterialObjectTypeID=1131");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=295 AND MaterialObjectTypeID=262");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=237,Quality=0,Influence=30,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=296 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=296 AND MaterialObjectTypeID=346");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3979,Quality=0,Influence=40,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=297 AND MaterialObjectTypeID=368");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=297 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=237,Quality=0,Influence=40,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=298 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3981,Quality=0,Influence=15,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=298 AND MaterialObjectTypeID=368");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=298 AND MaterialObjectTypeID=346");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3981,Quality=0,Influence=30,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=299 AND MaterialObjectTypeID=368");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3979,Quality=75,Influence=50,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=300 AND MaterialObjectTypeID=413");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=237,Quality=0,Influence=35,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=301 AND MaterialObjectTypeID=282");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3979,Quality=0,Influence=20,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=301 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=301 AND MaterialObjectTypeID=346");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=237,Quality=0,Influence=35,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=302 AND MaterialObjectTypeID=282");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3980,Quality=0,Influence=20,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=302 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=302 AND MaterialObjectTypeID=346");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=237,Quality=0,Influence=40,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=303 AND MaterialObjectTypeID=282");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3978,Quality=0,Influence=10,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=303 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=303 AND MaterialObjectTypeID=346");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=304 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=305 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=306 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=307 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=308 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=309 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=310 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=311 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=312 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=313 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=314 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3973,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=315 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=316 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1408,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=317 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1408,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=318 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3974,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=319 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3974,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=320 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1408,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=321 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=322 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3974,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=323 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=324 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=325 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3974,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=326 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=327 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=328 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=329 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1408,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=330 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=599,Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=331 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=293,Quality=0,Influence=10,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=332 AND MaterialObjectTypeID=1647");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1131,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=332 AND MaterialObjectTypeID=366");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=332 AND MaterialObjectTypeID=453");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=333 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=334 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1408,Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=335 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3975,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=336 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3975,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=337 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3975,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=338 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3974,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=339 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3974,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=340 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=891,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=351 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=892,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=352 AND MaterialObjectTypeID=260");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=352 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=893,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=353 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=894,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=354 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=895,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=355 AND MaterialObjectTypeID=260");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=355 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=896,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=356 AND MaterialObjectTypeID=260");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=356 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=356 AND MaterialObjectTypeID=1388");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=885,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=357 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=886,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=358 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=887,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=359 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=888,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=360 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=889,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=361 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=890,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=362 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=908,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=363 AND MaterialObjectTypeID=263");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=907,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=364 AND MaterialObjectTypeID=264");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=906,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=365 AND MaterialObjectTypeID=263");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=905,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=366 AND MaterialObjectTypeID=264");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=904,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=367 AND MaterialObjectTypeID=264");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=903,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=368 AND MaterialObjectTypeID=264");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=369 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,369,347,0,5,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=60,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=369 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=370 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,370,347,0,5,5,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=370 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=50,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=370 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=371 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,371,347,0,5,3,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=50,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=371 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=372 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,372,347,0,5,2,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=373 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,373,347,0,5,4,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=50,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=373 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=374 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,374,347,0,5,2,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=375 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,375,347,0,10,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=375 AND MaterialObjectTypeID=260");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=376 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,376,347,0,10,6,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=376 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=376 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=376 AND MaterialObjectTypeID=261");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=377 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,377,347,0,10,4,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=377 AND MaterialObjectTypeID=260");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=378 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,378,347,0,10,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=378 AND MaterialObjectTypeID=260");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=379 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,379,347,0,10,4,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=379 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=379 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=347,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=380 AND MaterialObjectTypeID=1393");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=380 AND MaterialObjectTypeID=424");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,380,424,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=380 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=381 AND MaterialObjectTypeID=265");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=381 AND MaterialObjectTypeID=266");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,381,266,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=381 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=382 AND MaterialObjectTypeID=265");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=382 AND MaterialObjectTypeID=3976");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,382,3976,0,20,3,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=382 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=25,Quantity=3,IsRegionItemRequired=1 WHERE RecipeID=382 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=382 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=383 AND MaterialObjectTypeID=265");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=383 AND MaterialObjectTypeID=3976");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,383,3976,0,15,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=383 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=2,IsRegionItemRequired=1 WHERE RecipeID=383 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=383 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=384 AND MaterialObjectTypeID=265");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=384 AND MaterialObjectTypeID=266");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,384,266,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=384 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=385 AND MaterialObjectTypeID=265");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=385 AND MaterialObjectTypeID=3976");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,385,3976,0,10,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=385 AND MaterialObjectTypeID=1394");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=386 AND MaterialObjectTypeID=266");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,386,266,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=386 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=35,Quantity=1,IsRegionItemRequired=1 WHERE RecipeID=386 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=386 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3971,Quality=0,Influence=25,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=387 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3976,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=387 AND MaterialObjectTypeID=265");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=25,Quantity=1,IsRegionItemRequired=1 WHERE RecipeID=387 AND MaterialObjectTypeID=1390");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=387 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3970,Quality=0,Influence=40,Quantity=3,IsRegionItemRequired=1 WHERE RecipeID=388 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3977,Quality=0,Influence=5,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=388 AND MaterialObjectTypeID=265");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=388 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=388 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3970,Quality=0,Influence=40,Quantity=1,IsRegionItemRequired=1 WHERE RecipeID=389 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3977,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=389 AND MaterialObjectTypeID=265");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=389 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=389 AND MaterialObjectTypeID=263");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=389 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=1 WHERE RecipeID=390 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3970,Quality=0,Influence=40,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=390 AND MaterialObjectTypeID=263");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3976,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=390 AND MaterialObjectTypeID=265");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=390 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3970,Quality=0,Influence=40,Quantity=2,IsRegionItemRequired=1 WHERE RecipeID=391 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3977,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=391 AND MaterialObjectTypeID=265");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=391 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3970,Quality=0,Influence=40,Quantity=1,IsRegionItemRequired=1 WHERE RecipeID=392 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3976,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=392 AND MaterialObjectTypeID=263");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=392 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=392 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=369,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=393 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=425,Quality=0,Influence=40,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=393 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=393 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,393,1356,0,5,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=370,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=394 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=394 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,394,1356,0,5,4,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=394 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=371,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=395 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=395 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,395,1356,0,5,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=395 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=372,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=396 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=396 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,396,1356,0,5,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=373,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=397 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=397 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,397,1356,0,5,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=397 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=374,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=398 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=398 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,398,1356,0,5,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=399 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=375,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=399 AND MaterialObjectTypeID=425");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=399 AND MaterialObjectTypeID=424");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,399,424,0,40,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=376,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=400 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=400 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=400 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=377,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=401 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=401 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=401 AND MaterialObjectTypeID=425");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=402 AND MaterialObjectTypeID=378");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,402,378,0,15,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=402 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=402 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=895,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=403 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=403 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=403 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=896,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=404 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=404 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=404 AND MaterialObjectTypeID=424");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=405 AND MaterialObjectTypeID=897");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,405,897,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=405 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=425,Quality=0,Influence=10,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=406 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=898,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=406 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=406 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=383,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=407 AND MaterialObjectTypeID=261");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=407 AND MaterialObjectTypeID=424");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,407,424,0,25,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=25,Quantity=2,IsRegionItemRequired=1 WHERE RecipeID=407 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=407 AND MaterialObjectTypeID=1394");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=408 AND MaterialObjectTypeID=384");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,408,384,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=408 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=385,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=409 AND MaterialObjectTypeID=261");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=409 AND MaterialObjectTypeID=424");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,409,424,0,20,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=409 AND MaterialObjectTypeID=1394");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=410 AND MaterialObjectTypeID=386");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,410,386,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=410 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=410 AND MaterialObjectTypeID=1394");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=411 AND MaterialObjectTypeID=903");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,411,903,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=411 AND MaterialObjectTypeID=263");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=411 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=904,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=412 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=412 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=425,Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=413 AND MaterialObjectTypeID=261");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=413 AND MaterialObjectTypeID=905");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,413,905,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=1 WHERE RecipeID=413 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=413 AND MaterialObjectTypeID=1394");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=414 AND MaterialObjectTypeID=390");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,414,390,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=50,Quantity=1,IsRegionItemRequired=1 WHERE RecipeID=414 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=414 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=391,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=415 AND MaterialObjectTypeID=261");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=415 AND MaterialObjectTypeID=424");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,415,424,0,20,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=415 AND MaterialObjectTypeID=1394");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=416 AND MaterialObjectTypeID=392");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,416,392,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=416 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=2836,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=416 AND MaterialObjectTypeID=1394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=885,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=417 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=886,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=418 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=887,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=419 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=888,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=420 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=889,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=421 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=890,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=422 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=891,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=423 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=892,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=424 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=893,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=425 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=894,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=426 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=895,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=427 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=896,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=428 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=897,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=429 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=898,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=430 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=899,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=431 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=900,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=432 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=901,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=433 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=902,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=434 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=903,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=435 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=904,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=436 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=905,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=437 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=906,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=438 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=907,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=439 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=908,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=440 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1130,Quality=0,Influence=50,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=441 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=282,Quality=0,Influence=30,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=446 AND MaterialObjectTypeID=283");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=446 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=282,Quality=0,Influence=30,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=447 AND MaterialObjectTypeID=283");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=447 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=60,Quantity=12,IsRegionItemRequired=0 WHERE RecipeID=449 AND MaterialObjectTypeID=383");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=60,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=457 AND MaterialObjectTypeID=384");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=60,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=459 AND MaterialObjectTypeID=390");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=60,Quantity=12,IsRegionItemRequired=0 WHERE RecipeID=460 AND MaterialObjectTypeID=386");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=465 AND MaterialObjectTypeID=336");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=467 AND MaterialObjectTypeID=385");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=471 AND MaterialObjectTypeID=355");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=472 AND MaterialObjectTypeID=354");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=12,IsRegionItemRequired=0 WHERE RecipeID=472 AND MaterialObjectTypeID=386");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=473 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=474 AND MaterialObjectTypeID=385");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=476 AND MaterialObjectTypeID=376");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=476 AND MaterialObjectTypeID=384");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=477 AND MaterialObjectTypeID=384");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=478 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=478 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=478 AND MaterialObjectTypeID=393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=479 AND MaterialObjectTypeID=338");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=479 AND MaterialObjectTypeID=376");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=480 AND MaterialObjectTypeID=375");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=480 AND MaterialObjectTypeID=376");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=481 AND MaterialObjectTypeID=1026");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=481 AND MaterialObjectTypeID=376");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=481 AND MaterialObjectTypeID=668");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=482 AND MaterialObjectTypeID=392");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=482 AND MaterialObjectTypeID=668");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=483 AND MaterialObjectTypeID=375");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=484 AND MaterialObjectTypeID=338");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=484 AND MaterialObjectTypeID=355");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=484 AND MaterialObjectTypeID=375");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=485 AND MaterialObjectTypeID=376");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=485 AND MaterialObjectTypeID=668");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=486 AND MaterialObjectTypeID=1027");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=486 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=486 AND MaterialObjectTypeID=360");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=487 AND MaterialObjectTypeID=354");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=12,IsRegionItemRequired=0 WHERE RecipeID=487 AND MaterialObjectTypeID=385");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=488 AND MaterialObjectTypeID=360");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=488 AND MaterialObjectTypeID=394");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=489 AND MaterialObjectTypeID=338");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=489 AND MaterialObjectTypeID=391");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=490 AND MaterialObjectTypeID=354");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=490 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=18,IsRegionItemRequired=0 WHERE RecipeID=490 AND MaterialObjectTypeID=384");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=491 AND MaterialObjectTypeID=360");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=492 AND MaterialObjectTypeID=338");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=492 AND MaterialObjectTypeID=375");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=493 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=494 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=494 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=24,IsRegionItemRequired=0 WHERE RecipeID=494 AND MaterialObjectTypeID=386");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=495 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=496 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=496 AND MaterialObjectTypeID=390");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=497 AND MaterialObjectTypeID=360");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=24,IsRegionItemRequired=0 WHERE RecipeID=497 AND MaterialObjectTypeID=386");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=498 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=24,IsRegionItemRequired=0 WHERE RecipeID=498 AND MaterialObjectTypeID=383");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=499 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=499 AND MaterialObjectTypeID=1026");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=499 AND MaterialObjectTypeID=668");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=500 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=500 AND MaterialObjectTypeID=338");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=501 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=501 AND MaterialObjectTypeID=1026");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=502 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=502 AND MaterialObjectTypeID=1025");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=502 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=502 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=502 AND MaterialObjectTypeID=383");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=503 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=503 AND MaterialObjectTypeID=338");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=503 AND MaterialObjectTypeID=383");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=504 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=504 AND MaterialObjectTypeID=354");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=504 AND MaterialObjectTypeID=385");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=505 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=505 AND MaterialObjectTypeID=1026");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=505 AND MaterialObjectTypeID=386");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=505 AND MaterialObjectTypeID=390");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=506 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=506 AND MaterialObjectTypeID=354");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=506 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=506 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=506 AND MaterialObjectTypeID=360");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=507 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=507 AND MaterialObjectTypeID=1026");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=507 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=507 AND MaterialObjectTypeID=383");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=508 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=508 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=508 AND MaterialObjectTypeID=383");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=509 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=509 AND MaterialObjectTypeID=338");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=509 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=509 AND MaterialObjectTypeID=384");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=510 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=510 AND MaterialObjectTypeID=1026");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=510 AND MaterialObjectTypeID=338");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=511 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=511 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=511 AND MaterialObjectTypeID=375");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=16,IsRegionItemRequired=1 WHERE RecipeID=511 AND MaterialObjectTypeID=383");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=24,IsRegionItemRequired=0 WHERE RecipeID=511 AND MaterialObjectTypeID=384");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=24,IsRegionItemRequired=0 WHERE RecipeID=511 AND MaterialObjectTypeID=385");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=512 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=512 AND MaterialObjectTypeID=338");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=512 AND MaterialObjectTypeID=360");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=512 AND MaterialObjectTypeID=375");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=1 WHERE RecipeID=512 AND MaterialObjectTypeID=390");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=512 AND MaterialObjectTypeID=958");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=513 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=1 WHERE RecipeID=513 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=513 AND MaterialObjectTypeID=360");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=513 AND MaterialObjectTypeID=375");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=24,IsRegionItemRequired=0 WHERE RecipeID=513 AND MaterialObjectTypeID=384");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=513 AND MaterialObjectTypeID=958");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=514 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=514 AND MaterialObjectTypeID=338");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=1 WHERE RecipeID=514 AND MaterialObjectTypeID=359");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=514 AND MaterialObjectTypeID=375");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=514 AND MaterialObjectTypeID=376");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=514 AND MaterialObjectTypeID=668");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=515 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=515 AND MaterialObjectTypeID=1025");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=515 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=1 WHERE RecipeID=515 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=515 AND MaterialObjectTypeID=375");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=515 AND MaterialObjectTypeID=394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=516 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=516 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=516 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=516 AND MaterialObjectTypeID=360");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=516 AND MaterialObjectTypeID=386");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=517 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=517 AND MaterialObjectTypeID=336");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=517 AND MaterialObjectTypeID=339");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=517 AND MaterialObjectTypeID=354");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=1 WHERE RecipeID=517 AND MaterialObjectTypeID=384");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=517 AND MaterialObjectTypeID=386");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=518 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=518 AND MaterialObjectTypeID=1025");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=1 WHERE RecipeID=518 AND MaterialObjectTypeID=338");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=518 AND MaterialObjectTypeID=339");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=518 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=518 AND MaterialObjectTypeID=392");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=519 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=519 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=519 AND MaterialObjectTypeID=360");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=519 AND MaterialObjectTypeID=385");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=1 WHERE RecipeID=519 AND MaterialObjectTypeID=386");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=519 AND MaterialObjectTypeID=390");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=520 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=1 WHERE RecipeID=520 AND MaterialObjectTypeID=355");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=520 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=520 AND MaterialObjectTypeID=383");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=520 AND MaterialObjectTypeID=386");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=520 AND MaterialObjectTypeID=962");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=1 WHERE RecipeID=521 AND MaterialObjectTypeID=354");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=521 AND MaterialObjectTypeID=356");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=521 AND MaterialObjectTypeID=391");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=521 AND MaterialObjectTypeID=393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=521 AND MaterialObjectTypeID=394");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=141,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=522 AND MaterialObjectTypeID=1054");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=522 AND MaterialObjectTypeID=1025");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=522 AND MaterialObjectTypeID=354");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=522 AND MaterialObjectTypeID=358");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=522 AND MaterialObjectTypeID=383");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=12,Quantity=20,IsRegionItemRequired=1 WHERE RecipeID=522 AND MaterialObjectTypeID=385");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=885,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=525 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=886,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=526 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=887,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=527 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=888,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=528 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=889,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=529 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=890,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=530 AND MaterialObjectTypeID=1477");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=891,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=531 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=892,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=532 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=893,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=533 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=894,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=534 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=895,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=535 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=896,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=536 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=897,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=537 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=898,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=538 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=899,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=539 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=900,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=540 AND MaterialObjectTypeID=263");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=901,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=541 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=902,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=542 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=903,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=543 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=904,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=544 AND MaterialObjectTypeID=264");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=905,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=545 AND MaterialObjectTypeID=264");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=906,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=546 AND MaterialObjectTypeID=263");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=907,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=547 AND MaterialObjectTypeID=264");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=908,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=548 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=40,IsRegionItemRequired=0 WHERE RecipeID=552 AND MaterialObjectTypeID=281");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=552 AND MaterialObjectTypeID=282");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=25,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=552 AND MaterialObjectTypeID=283");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=70,Quantity=10,IsRegionItemRequired=1 WHERE RecipeID=553 AND MaterialObjectTypeID=1068");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3800,Quality=0,Influence=30,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=593 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=25,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=593 AND MaterialObjectTypeID=282");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=286,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=594 AND MaterialObjectTypeID=287");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=233,Quality=30,Influence=30,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=612 AND MaterialObjectTypeID=212");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=241,Quality=30,Influence=30,Quantity=100,IsRegionItemRequired=0 WHERE RecipeID=612 AND MaterialObjectTypeID=269");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1356,Quality=30,Influence=30,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=612 AND MaterialObjectTypeID=324");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=612 AND MaterialObjectTypeID=342");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=233,Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=613 AND MaterialObjectTypeID=269");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=262,Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=613 AND MaterialObjectTypeID=401");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=403,Quality=50,Influence=25,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=613 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=529,Quality=0,Influence=25,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=613 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=262,Quality=0,Influence=30,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=614 AND MaterialObjectTypeID=270");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3800,Quality=50,Influence=30,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=614 AND MaterialObjectTypeID=271");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3935,Quality=50,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=614 AND MaterialObjectTypeID=405");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=262,Quality=0,Influence=30,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=615 AND MaterialObjectTypeID=404");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=271,Quality=60,Influence=30,Quantity=100,IsRegionItemRequired=0 WHERE RecipeID=615 AND MaterialObjectTypeID=406");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=401,Quality=50,Influence=30,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=615 AND MaterialObjectTypeID=480");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=235,Quality=0,Influence=25,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=626 AND MaterialObjectTypeID=281");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=241,Quality=0,Influence=25,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=626 AND MaterialObjectTypeID=325");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=626 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,626,1356,0,20,5,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=200,IsRegionItemRequired=0 WHERE RecipeID=626 AND MaterialObjectTypeID=216");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=269,Quality=0,Influence=10,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=643 AND MaterialObjectTypeID=281");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3051,Quality=0,Influence=30,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=643 AND MaterialObjectTypeID=283");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3802,Quality=0,Influence=15,Quantity=12,IsRegionItemRequired=0 WHERE RecipeID=643 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=643 AND MaterialObjectTypeID=282");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=653 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=599,Quality=0,Influence=40,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=658 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=658 AND MaterialObjectTypeID=366");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=374,Quality=0,Influence=30,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=662 AND MaterialObjectTypeID=1130");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3934,Quality=0,Influence=30,Quantity=8,IsRegionItemRequired=0 WHERE RecipeID=662 AND MaterialObjectTypeID=325");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=60,IsRegionItemRequired=0 WHERE RecipeID=835 AND MaterialObjectTypeID=281");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=60,IsRegionItemRequired=0 WHERE RecipeID=836 AND MaterialObjectTypeID=281");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=32,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=842 AND MaterialObjectTypeID=36");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=36,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=843 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=374,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=843 AND MaterialObjectTypeID=32");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=843 AND MaterialObjectTypeID=1130");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=36,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=844 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=374,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=844 AND MaterialObjectTypeID=32");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=844 AND MaterialObjectTypeID=1130");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=36,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=845 AND MaterialObjectTypeID=32");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=60,IsRegionItemRequired=0 WHERE RecipeID=848 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=80,IsRegionItemRequired=0 WHERE RecipeID=848 AND MaterialObjectTypeID=281");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=60,IsRegionItemRequired=0 WHERE RecipeID=849 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=80,IsRegionItemRequired=0 WHERE RecipeID=849 AND MaterialObjectTypeID=281");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=237,Quality=0,Influence=30,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=850 AND MaterialObjectTypeID=282");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3978,Quality=0,Influence=10,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=850 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3979,Quality=0,Influence=15,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=851 AND MaterialObjectTypeID=282");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3802,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=1 WHERE RecipeID=869 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=80,Influence=60,Quantity=120,IsRegionItemRequired=0 WHERE RecipeID=869 AND MaterialObjectTypeID=334");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=887 AND MaterialObjectTypeID=281");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=887 AND MaterialObjectTypeID=324");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=887 AND MaterialObjectTypeID=326");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=890 AND MaterialObjectTypeID=1393");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,890,1393,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=136,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=899 AND MaterialObjectTypeID=453");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=136,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=900 AND MaterialObjectTypeID=453");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=136,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=901 AND MaterialObjectTypeID=453");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=136,Quality=0,Influence=10,Quantity=20,IsRegionItemRequired=0 WHERE RecipeID=902 AND MaterialObjectTypeID=453");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=136,Quality=0,Influence=10,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=903 AND MaterialObjectTypeID=453");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=518,Quality=0,Influence=20,Quantity=30,IsRegionItemRequired=0 WHERE RecipeID=909 AND MaterialObjectTypeID=293");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=3,IsRegionItemRequired=1 WHERE RecipeID=1004 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=1 WHERE RecipeID=1005 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=1 WHERE RecipeID=1006 AND MaterialObjectTypeID=326");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=2,IsRegionItemRequired=1 WHERE RecipeID=1007 AND MaterialObjectTypeID=283");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3800,Quality=0,Influence=20,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=1032 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3800,Quality=0,Influence=20,Quantity=15,IsRegionItemRequired=0 WHERE RecipeID=1033 AND MaterialObjectTypeID=235");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=10,IsRegionItemRequired=0 WHERE RecipeID=1049 AND MaterialObjectTypeID=1356");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=1086 AND MaterialObjectTypeID=237");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=5103 AND MaterialObjectTypeID=263");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=5103 AND MaterialObjectTypeID=264");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=5103 AND MaterialObjectTypeID=266");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3970,Quality=0,Influence=15,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=5106 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3970,Quality=0,Influence=15,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=5108 AND MaterialObjectTypeID=261");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5109 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5109,347,0,5,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=60,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=5109 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5110 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5110,347,0,5,5,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=5110 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=50,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=5110 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5111 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5111,347,0,5,3,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=50,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=5111 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5112 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5112,347,0,5,2,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5113 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5113,347,0,5,4,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=50,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=5113 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5114 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5114,347,0,5,2,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5115 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5115,347,0,10,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5115 AND MaterialObjectTypeID=260");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5116 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5116,347,0,10,6,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=5116 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=5116 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=5116 AND MaterialObjectTypeID=261");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5117 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5117,347,0,10,4,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=5117 AND MaterialObjectTypeID=260");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5118 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5118,347,0,10,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5118 AND MaterialObjectTypeID=260");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5119 AND MaterialObjectTypeID=347");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5119,347,0,10,4,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=5119 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=5119 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=347,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=5120 AND MaterialObjectTypeID=1393");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5120 AND MaterialObjectTypeID=424");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5120,424,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5120 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=369,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5133 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=425,Quality=0,Influence=40,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5133 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5133 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5133,1356,0,5,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=370,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5134 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5134 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5134,1356,0,5,4,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=5134 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=371,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5135 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5135 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5135,1356,0,5,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=5135 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=372,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5136 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5136 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5136,1356,0,5,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=373,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5137 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5137 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5137,1356,0,5,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=5137 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=374,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5138 AND MaterialObjectTypeID=1477");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5138 AND MaterialObjectTypeID=1356");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5138,1356,0,5,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5139 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=375,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5139 AND MaterialObjectTypeID=425");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5139 AND MaterialObjectTypeID=424");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5139,424,0,40,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=376,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5140 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5140 AND MaterialObjectTypeID=1393");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=5140 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=377,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5141 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=5141 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=5141 AND MaterialObjectTypeID=425");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5142 AND MaterialObjectTypeID=378");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5142,378,0,15,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5142 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5142 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=895,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5143 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=5,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=5143 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=40,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=5143 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=896,Quality=0,Influence=20,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5144 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=5144 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=30,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=5144 AND MaterialObjectTypeID=424");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=5161 AND MaterialObjectTypeID=1393");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,5161,1393,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=590,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=5164 AND MaterialObjectTypeID=237");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=5,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6360 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1393,Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6360 AND MaterialObjectTypeID=264");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6360 AND MaterialObjectTypeID=3052");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6360,3052,0,5,5,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6360 AND MaterialObjectTypeID=3970");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6360,3970,0,20,3,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6360 AND MaterialObjectTypeID=3971");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6360,3971,0,25,5,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=5,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6359 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1393,Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6359 AND MaterialObjectTypeID=264");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6359 AND MaterialObjectTypeID=3049");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6359,3049,0,5,5,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6359 AND MaterialObjectTypeID=3970");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6359,3970,0,20,3,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6359 AND MaterialObjectTypeID=3971");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6359,3971,0,25,5,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=5,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6358 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1393,Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6358 AND MaterialObjectTypeID=264");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6358 AND MaterialObjectTypeID=3049");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6358,3049,0,5,5,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6358 AND MaterialObjectTypeID=3970");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6358,3970,0,20,3,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6358 AND MaterialObjectTypeID=3971");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6358,3971,0,25,5,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=5,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6356 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1393,Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6356 AND MaterialObjectTypeID=264");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6356 AND MaterialObjectTypeID=3049");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6356,3049,0,5,5,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6356 AND MaterialObjectTypeID=3971");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6356,3971,0,25,6,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6356 AND MaterialObjectTypeID=3972");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6356,3972,0,20,3,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=5,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6357 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1393,Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6357 AND MaterialObjectTypeID=264");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6357 AND MaterialObjectTypeID=3052");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6357,3052,0,5,5,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6357 AND MaterialObjectTypeID=3971");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6357,3971,0,25,6,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6357 AND MaterialObjectTypeID=3972");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6357,3972,0,20,3,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=5,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6355 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1393,Quality=0,Influence=5,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6355 AND MaterialObjectTypeID=264");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6355 AND MaterialObjectTypeID=3049");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6355,3049,0,5,5,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6355 AND MaterialObjectTypeID=3971");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6355,3971,0,25,6,0)");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6355 AND MaterialObjectTypeID=3972");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6355,3972,0,20,3,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6372 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1393,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6372 AND MaterialObjectTypeID=264");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6372 AND MaterialObjectTypeID=3971");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6372,3971,0,40,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6370 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1393,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6370 AND MaterialObjectTypeID=264");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6370 AND MaterialObjectTypeID=3971");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6370,3971,0,40,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6371 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=1393,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6371 AND MaterialObjectTypeID=264");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6371 AND MaterialObjectTypeID=3971");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6371,3971,0,40,2,0)");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=263,Quality=0,Influence=10,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=6483 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=264,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6483 AND MaterialObjectTypeID=266");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=295,Quality=0,Influence=10,Quantity=40,IsRegionItemRequired=0 WHERE RecipeID=6483 AND MaterialObjectTypeID=303");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3970,Quality=0,Influence=10,Quantity=3,IsRegionItemRequired=0 WHERE RecipeID=6483 AND MaterialObjectTypeID=2836");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6483 AND MaterialObjectTypeID=3049");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=6483 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=10,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=6484 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=295,Quality=0,Influence=10,Quantity=40,IsRegionItemRequired=0 WHERE RecipeID=6484 AND MaterialObjectTypeID=266");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3971,Quality=0,Influence=10,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6484 AND MaterialObjectTypeID=304");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3972,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6484 AND MaterialObjectTypeID=2836");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6484 AND MaterialObjectTypeID=3052");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6484 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=15,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=6485 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=295,Quality=0,Influence=10,Quantity=40,IsRegionItemRequired=0 WHERE RecipeID=6485 AND MaterialObjectTypeID=266");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3971,Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6485 AND MaterialObjectTypeID=305");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6485 AND MaterialObjectTypeID=2836");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6485 AND MaterialObjectTypeID=3052");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6485 AND MaterialObjectTypeID=425");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=10,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=6486 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=264,Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6486 AND MaterialObjectTypeID=266");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=295,Quality=0,Influence=10,Quantity=40,IsRegionItemRequired=0 WHERE RecipeID=6486 AND MaterialObjectTypeID=306");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3972,Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6486 AND MaterialObjectTypeID=2836");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6486 AND MaterialObjectTypeID=3049");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=10,Quantity=2,IsRegionItemRequired=0 WHERE RecipeID=6486 AND MaterialObjectTypeID=424");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=15,Quantity=6,IsRegionItemRequired=0 WHERE RecipeID=6487 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=295,Quality=0,Influence=10,Quantity=40,IsRegionItemRequired=0 WHERE RecipeID=6487 AND MaterialObjectTypeID=266");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3971,Quality=0,Influence=20,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=6487 AND MaterialObjectTypeID=307");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6487 AND MaterialObjectTypeID=2836");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6487 AND MaterialObjectTypeID=3052");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=425,Quality=0,Influence=10,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6347 AND MaterialObjectTypeID=3556");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=6347 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6347 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=15,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=6398 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=295,Quality=0,Influence=10,Quantity=40,IsRegionItemRequired=0 WHERE RecipeID=6398 AND MaterialObjectTypeID=266");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=425,Quality=0,Influence=10,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6398 AND MaterialObjectTypeID=2836");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3971,Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6398 AND MaterialObjectTypeID=3052");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6398 AND MaterialObjectTypeID=3556");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6398 AND MaterialObjectTypeID=3592");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=266,Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=6346 AND MaterialObjectTypeID=3554");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6346 AND MaterialObjectTypeID=424");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6346,424,0,10,1,0)");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=6346 AND MaterialObjectTypeID=260");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6346 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=260,Quality=0,Influence=15,Quantity=5,IsRegionItemRequired=0 WHERE RecipeID=6397 AND MaterialObjectTypeID=261");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=295,Quality=0,Influence=10,Quantity=40,IsRegionItemRequired=0 WHERE RecipeID=6397 AND MaterialObjectTypeID=2836");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=424,Quality=0,Influence=10,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=6397 AND MaterialObjectTypeID=3049");



        dbi.Update("UPDATE `recipe_requirement` SET MaterialObjectTypeID=3971,Quality=0,Influence=20,Quantity=4,IsRegionItemRequired=0 WHERE RecipeID=6397 AND MaterialObjectTypeID=3554");



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID=6397 AND MaterialObjectTypeID=3591");



        dbi.Update("UPDATE `recipe_requirement` SET Quality=0,Influence=15,Quantity=1,IsRegionItemRequired=0 WHERE RecipeID=6397 AND MaterialObjectTypeID=266");



        // removed recipes



        dbi.Update("DELETE FROM `recipe_requirement` WHERE RecipeID IN (153,192,1009,1010,1011,1012,1013,1014,1015,1016,1017,1018,1019,1022,5121,5122,5123,5124,5125,5126,5127,5128,5129,5130,5131,5132,5145,5146,5147,5148,5149,5150,5151,5152,5153,5154,5155,5156)");



        dbi.Update("DELETE FROM `recipe_possible_blueprints` WHERE RecipeID IN (153,192,1009,1010,1011,1012,1013,1014,1015,1016,1017,1018,1019,1022,5121,5122,5123,5124,5125,5126,5127,5128,5129,5130,5131,5132,5145,5146,5147,5148,5149,5150,5151,5152,5153,5154,5155,5156) OR BaseRecipeID IN (153,192,1009,1010,1011,1012,1013,1014,1015,1016,1017,1018,1019,1022,5121,5122,5123,5124,5125,5126,5127,5128,5129,5130,5131,5132,5145,5146,5147,5148,5149,5150,5151,5152,5153,5154,5155,5156)");



        dbi.Update("DELETE FROM `recipe` WHERE ID IN (153,192,1009,1010,1011,1012,1013,1014,1015,1016,1017,1018,1019,1022,5121,5122,5123,5124,5125,5126,5127,5128,5129,5130,5131,5132,5145,5146,5147,5148,5149,5150,5151,5152,5153,5154,5155,5156)");



        // new recipes



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6510,'Padding','',1394,25,60,3976,30,1,0,0,'art/2D/Items/placeholder_padding.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6511,'Fine Padding','',1394,25,90,3977,30,1,0,0,'art/2D/Items/placeholder_fine_padding.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6512,'Wooden Shaft','',518,8,60,3974,30,1,0,0,'art/2D/Items/placeholder_wooden_shaft.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6513,'Sword Hilt','',453,4,30,3973,30,1,0,0,'art/2D/Items/placeholder_sword_hilt.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6514,'Metal Shaft','',453,4,30,3975,30,1,0,0,'art/2D/Items/placeholder_metal_shaft.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6515,'Arrowhead','',453,4,30,3978,30,10,0,0,'art/2D/Items/placeholder_arrowhead.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6516,'Bodkin Point','',453,4,30,3979,30,10,0,0,'art/2D/Items/placeholder_bodkin_point.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6517,'Broadhead Point','',453,4,30,3980,30,10,0,0,'art/2D/Items/placeholder_broadhead_point.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6518,'Blunt Point','',453,4,30,3981,30,10,0,0,'art/2D/Items/placeholder_blunt_point.png')");





        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6520,'Dye','',105,17,60,3982,10,1,0,0,'art/2D/Items/placeholder_dye.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6521,'Dyed Linen Cloth','',472,25,60,3970,10,1,0,0,'art/2D/Items/placeholder_dyed_linen.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6522,'Dyed Silk Cloth','',472,25,60,3971,10,1,0,0,'art/2D/Items/placeholder_dyed_silk.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6523,'Dyed Wool Cloth','',472,25,60,3972,10,1,0,0,'art/2D/Items/placeholder_dyed_wool.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6524,'Load of Nails','',453,4,90,281,20,50,1,0,'art/2D/Items/nails.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6525,'Load of Metal Bands','',453,4,90,282,20,5,1,0,'art/2D/Items/metal_band.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6526,'Copper Nails','',453,4,0,281,10,10,1,0,'art/2D/Items/nails.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6527,'Dyed Gottlung Rags','',295,25,0,3623,30,1,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_eur_v1.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6528,'Dyed Northern Rags','',295,25,0,3624,30,1,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_vik_v1.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6529,'Dyed Steppe Rags','',295,25,0,3625,30,1,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_mon_v1.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6530,'Salt (placeholder)','',NULL,1,0,3983,0,1,0,0,'art/2D/Items/placeholder_salt.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6531,'Nordmar Mortise Chisel (placeholder)','',NULL,1,0,3985,0,1,0,0,'art/2D/Items/placeholder_nordmar_chisel.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6532,'Mesyrna Herbarium and Athanor (placeholder)','',NULL,1,0,3986,0,1,0,0,'art/2D/Items/placeholder_mesyrna_herbarium.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6533,'Valesos Cauldron (placeholder)','',NULL,1,0,3987,0,1,0,0,'art/2D/Items/placeholder_valesos_cauldron.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6534,'Valesos Diamond Whetstone (placeholder)','',NULL,1,0,3988,0,1,0,0,'art/2D/Items/placeholder_valesos_whetstone.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6535,'Mesyrna Sewing Pattern (placeholder)','',NULL,1,0,3989,0,1,0,0,'art/2D/Items/placeholder_mesyrna_pattern.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6536,'Better Plates (placeholder)','',NULL,1,0,3990,0,1,0,0,'art/2D/Items/placeholder_better_plates.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6537,'Dyed Gottlung Rags','',2836,25,0,3623,30,1,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_eur_v1.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6538,'Dyed Northern Rags','',2836,25,0,3624,30,1,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_vik_v1.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6539,'Dyed Steppe Rags','',2836,25,0,3625,30,1,0,0,'mod/KingOutfitsMod/art/2D/Items/male_tatters_mon_v1.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6540,'Jarl''s Clothes','',295,25,60,1578,30,1,0,0,'art/2D/Items/jarl_clothes.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6541,'Jarl''s Clothes','',2836,25,60,1578,30,1,0,0,'art/2D/Items/jarl_clothes.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6542,'Konung''s Outfit','',295,25,90,1643,30,1,0,0,'art/2D/Items/Villein_RETEXTURE_06_man.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6543,'Konung''s Outfit','',2836,25,90,1643,30,1,0,0,'art/2D/Items/Villein_RETEXTURE_06_man.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6544,'Eminent Cloak','',295,25,90,1639,30,1,0,0,'art/2D/Items/Noble_RETEXTURE_05_man.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6545,'Eminent Cloak','',2836,25,90,1639,30,1,0,0,'art/2D/Items/Noble_RETEXTURE_05_man.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6546,'Merchant''s Cloak','',295,25,90,1640,30,1,0,0,'art/2D/Items/Jarl_RETEXTURE_02_man.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6547,'Merchant''s Cloak','',2836,25,90,1640,30,1,0,0,'art/2D/Items/Jarl_RETEXTURE_02_man.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6548,'Northern Garb','',295,25,90,1638,30,1,0,0,'art/2D/Items/Noble_RETEXTURE_03_man.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6549,'Northern Garb','',2836,25,90,1638,30,1,0,0,'art/2D/Items/Noble_RETEXTURE_03_man.png')");



        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6550,'Pilgrim''s Clothes','',295,25,90,3991,30,1,0,0,'art/2D/Items/placeholder_pilgrim_clothes.png')");



        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6551,'Pilgrim''s Clothes','',2836,25,90,3991,30,1,0,0,'art/2D/Items/placeholder_pilgrim_clothes.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6552,'Makthrid''s Outfit','',295,25,90,1747,30,1,0,0,'art/2D/Items/Cardinal_male_ingame.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6553,'Makthrid''s Outfit','',2836,25,90,1747,30,1,0,0,'art/2D/Items/Cardinal_male_ingame.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6554,'Archimankur''s Outfit','',295,25,90,1746,30,1,0,0,'art/2D/Items/Pope_male_ingame.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6555,'Archimankur''s Outfit','',2836,25,90,1746,30,1,0,0,'art/2D/Items/Pope_male_ingame.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6556,'Runic Tunic','',2836,25,90,1642,30,1,0,0,'art/2D/Items/Villein_RETEXTURE_01_man.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6557,'Strelets'' Garb','',2836,25,90,1641,30,1,0,0,'art/2D/Items/Jarl_RETEXTURE_04_man.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6558,'Villein''s Clothes','',295,25,90,1580,30,1,0,0,'art/2D/Items/villein_clothes.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6559,'Villein''s Clothes','',2836,25,90,1580,30,1,0,0,'art/2D/Items/villein_clothes.png')");



        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6560,'Fancy Decorated Clothes','',2836,25,90,3992,30,1,0,0,'art/2D/Items/placeholder_fancy_decorated.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6561,'Noble Simple Clothes','',2836,25,90,1579,30,1,0,0,'art/2D/Items/noble_clothes.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6562,'Breeder''s Outfit','',2836,25,60,3592,45,1,0,0,'mod/OutfitsMod/art/2D/Items/breeder.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6563,'Stonecutter''s Outfit','',2836,25,60,3591,50,1,0,0,'mod/OutfitsMod/art/2D/Items/miner.png')");



        dbi.Update("INSERT IGNORE INTO `recipe` VALUES (6570,'Fresh Water (from Salt Water)','',111,11,0,3180,20,5,1,0,'art/2D/Items/water.png')");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6510,261,0,30,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6510,1394,0,10,30,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6510,263,0,10,6,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6510,266,0,20,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6511,264,0,30,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6511,1394,0,10,30,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6511,263,0,10,6,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6511,266,0,20,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6512,518,0,10,30,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6512,3802,0,30,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6512,1131,0,10,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6512,1393,0,10,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6513,453,0,15,15,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6513,1393,0,15,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6513,366,0,40,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6514,453,0,15,15,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6514,364,0,50,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6514,366,0,5,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6515,453,0,20,15,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6515,368,0,50,10,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6516,453,0,20,15,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6516,368,0,50,20,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6517,453,0,20,15,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6517,368,0,50,20,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6518,453,0,20,15,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6518,368,0,50,10,0)");











        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6520,105,0,10,10,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6520,212,0,20,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6520,937,0,20,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6520,1091,0,20,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6520,944,0,20,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6521,472,0,10,30,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6521,261,0,40,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6521,3982,0,40,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6522,472,0,10,30,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6522,264,0,40,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6522,3982,0,40,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6523,472,0,10,30,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6523,266,0,40,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6523,3982,0,40,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6524,368,0,60,50,1)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6524,453,0,10,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6524,1647,0,10,30,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6525,366,0,60,25,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6525,453,0,10,10,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6525,1647,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6526,415,0,20,10,1)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6526,453,0,10,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6526,1647,0,10,30,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6527,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6527,1477,0,30,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6527,425,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6527,260,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6528,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6528,1477,0,30,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6528,425,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6528,260,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6529,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6529,1477,0,30,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6529,425,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6529,260,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6530,237,0,100,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6531,237,0,100,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6532,237,0,100,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6533,237,0,100,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6534,237,0,100,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6535,237,0,100,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6536,237,0,100,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6537,2836,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6537,1477,0,30,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6537,425,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6537,260,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6538,2836,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6538,1477,0,30,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6538,425,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6538,260,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6539,2836,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6539,1477,0,30,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6539,425,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6539,260,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6540,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6540,261,0,20,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6540,3972,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6540,260,0,5,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6540,425,0,10,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6540,1393,0,5,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6541,2836,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6541,261,0,20,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6541,3972,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6541,260,0,5,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6541,425,0,10,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6541,1393,0,5,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6542,295,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6542,3970,0,30,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6542,263,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6542,1393,0,10,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6543,2836,0,40,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6543,3970,0,30,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6543,263,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6543,1393,0,10,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6544,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6544,266,0,20,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6544,3970,0,30,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6544,1393,0,5,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6544,260,0,5,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6545,2836,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6545,266,0,20,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6545,3970,0,30,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6545,1393,0,5,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6545,260,0,5,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6546,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6546,3972,0,30,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6546,261,0,20,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6546,265,0,5,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6546,1393,0,5,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6547,2836,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6547,3972,0,30,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6547,261,0,20,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6547,265,0,5,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6547,1393,0,5,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6548,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6548,425,0,15,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6548,3970,0,30,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6548,1393,0,5,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6548,260,0,10,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6549,2836,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6549,425,0,15,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6549,3970,0,30,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6549,1393,0,5,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6549,260,0,10,5,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6550,260,0,10,5,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6550,3970,0,30,3,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6550,295,0,10,40,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6550,425,0,20,2,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6551,260,0,10,5,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6551,3970,0,30,3,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6551,2836,0,20,40,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6551,425,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6552,260,0,10,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6552,3970,0,30,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6552,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6552,425,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6553,260,0,10,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6553,3970,0,30,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6553,2836,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6553,425,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6554,260,0,10,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6554,3970,0,30,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6554,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6554,425,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6555,260,0,10,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6555,3970,0,30,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6555,2836,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6555,425,0,20,2,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6556,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6556,3970,0,30,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6556,264,0,20,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6556,260,0,5,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6556,1393,0,5,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6557,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6557,424,0,10,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6557,3970,0,25,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6557,266,0,15,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6557,260,0,5,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6557,1393,0,5,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6558,295,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6558,3970,0,30,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6558,260,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6558,1393,0,10,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6559,2836,0,40,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6559,3970,0,30,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6559,260,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6559,1393,0,10,1,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6560,263,0,10,4,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6560,264,0,20,3,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6560,3972,0,20,2,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6560,295,0,10,40,0)");



        // dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6560,424,0,10,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6561,263,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6561,3970,0,25,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6561,295,0,10,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6561,425,0,15,3,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6561,1393,0,10,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6562,2836,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6562,260,0,15,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6562,425,0,10,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6562,261,0,20,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6563,2836,0,20,40,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6563,260,0,15,5,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6563,424,0,10,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6563,261,0,20,4,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6563,266,0,15,1,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6570,204,0,65,10,0)");



        dbi.Update("INSERT INTO `recipe_requirement` VALUES (NULL,6570,111,0,15,200,0)");



    }



};



activatePackage(LiFxRecipeRevisionPack);



LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxRecipeRevisionPack);



