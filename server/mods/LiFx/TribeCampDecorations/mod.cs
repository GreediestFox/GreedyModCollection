/**
* <author>GreedyFox</author>
* <description>Tribe Camp decorations for Lif:YO: the MMO's tribe camp props (torches, totems, shields, big shields, trees) as buildable Premium Decorations (parent 1637), ids 2940-2961,
*              recipes 5800-5821, each costs 1 Master Decorator's Kit (1636, the "big" kit) at skill 62 (Premium Decorations ability 338). Models: client + server art/models/3d/environment/natives
*              (tribecamp_tree_v3/v4.dts copied from Arden) and .../trees/oak/tribecamptree01-05. NOT done by this script: <object> blocks in data/cm_objects.xml (server + client), icons (existing
*              premium decoration icons are reused), client rows (objects_types/recipe/recipe_requirement.xml, requirement ids 91157+).
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*/

if (!isObject(LiFxTribeCampDecorations))
{
    new ScriptObject(LiFxTribeCampDecorations)
    {
    };
}

package LiFxTribeCampDecorations
{
    function LiFxTribeCampDecorations::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxTribeCampDecorations);
    }

    function LiFxTribeCampDecorations::version() {
        return "1.0.0";
    }

    function LiFxTribeCampDecorations::dbChanges() {
        // recipe_requirement has no uniqueness constraint and art/dump.sql seeds the same rows every boot: clear first.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` IN (5800,5801,5802,5803,5804,5805,5806,5807,5808,5809,5810,5811,5812,5813,5814,5815,5816,5817,5818,5819,5820,5821)");

        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2940,1637,'Tribe Camp Torch I',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Torch_on_the_stand.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2941,1637,'Tribe Camp Torch II',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Torch_on_the_stand.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2942,1637,'Tribe Camp Torch III',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Torch_on_the_stand.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2943,1637,'Tribe Camp Torch IV',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Torch_on_the_stand.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2944,1637,'Tribe Camp Torch V',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Torch_on_the_stand.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2945,1637,'Tribe Camp Totem I',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_pillar.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2946,1637,'Tribe Camp Totem II',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_pillar2.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2947,1637,'Tribe Camp Shield I',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_pillar_with_shield.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2948,1637,'Tribe Camp Shield II',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_pillar_with_shield2.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2949,1637,'Tribe Camp Shield III',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_pillar_with_shield.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2950,1637,'Tribe Camp Shield IV',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_pillar_with_shield2.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2951,1637,'Tribe Camp Shield V',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_pillar_with_shield.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2952,1637,'Tribe Camp Big Shield I',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_pillar_with_shield.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2953,1637,'Tribe Camp Big Shield II',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_pillar_with_shield2.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2954,1637,'Tribe Camp Big Shield III',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/wooden_pillar_with_shield.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2955,1637,'Tribe Camp Tree I',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Pillars_AA.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2956,1637,'Tribe Camp Tree II',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Pillars_AB.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2957,1637,'Tribe Camp Tree III',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Pillars_AA.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2958,1637,'Tribe Camp Tree IV',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Pillars_AB.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2959,1637,'Tribe Camp Tree V',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Pillars_AA.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2960,1637,'Tribe Camp Tree VI',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Pillars_BA.png','',200,120,0,0)");
        // [removed 2026-09-29] dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (2961,1637,'Tribe Camp Tree VII',0,0,1,0,0,0,1,0,3,0,20000,'',0,0,0,0,0,0,'art/2D/Objects/Pillars_BB.png','',200,120,0,0)");
        // All 22 recipes (and their recipe_requirement rows) disabled per GreedyFox's request 2026-09-28: the
        // objects (2940-2961) still exist and can be placed via GM, but are no longer player-craftable.
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5800,'Tribe Camp Torch I','',NULL,62,0,2940,10,1,0,0,'art/2D/Recipes/Torch_on_the_stand.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5801,'Tribe Camp Torch II','',NULL,62,0,2941,10,1,0,0,'art/2D/Recipes/Torch_on_the_stand.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5802,'Tribe Camp Torch III','',NULL,62,0,2942,10,1,0,0,'art/2D/Recipes/Torch_on_the_stand.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5803,'Tribe Camp Torch IV','',NULL,62,0,2943,10,1,0,0,'art/2D/Recipes/Torch_on_the_stand.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5804,'Tribe Camp Torch V','',NULL,62,0,2944,10,1,0,0,'art/2D/Recipes/Torch_on_the_stand.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5805,'Tribe Camp Totem I','',NULL,62,0,2945,10,1,0,0,'art/2D/Recipes/wooden_pillar.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5806,'Tribe Camp Totem II','',NULL,62,0,2946,10,1,0,0,'art/2D/Recipes/wooden_pillar2.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5807,'Tribe Camp Shield I','',NULL,62,0,2947,10,1,0,0,'art/2D/Recipes/wooden_pillar_with_shield.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5808,'Tribe Camp Shield II','',NULL,62,0,2948,10,1,0,0,'art/2D/Recipes/wooden_pillar_with_shield2.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5809,'Tribe Camp Shield III','',NULL,62,0,2949,10,1,0,0,'art/2D/Recipes/wooden_pillar_with_shield.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5810,'Tribe Camp Shield IV','',NULL,62,0,2950,10,1,0,0,'art/2D/Recipes/wooden_pillar_with_shield2.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5811,'Tribe Camp Shield V','',NULL,62,0,2951,10,1,0,0,'art/2D/Recipes/wooden_pillar_with_shield.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5812,'Tribe Camp Big Shield I','',NULL,62,0,2952,10,1,0,0,'art/2D/Recipes/wooden_pillar_with_shield.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5813,'Tribe Camp Big Shield II','',NULL,62,0,2953,10,1,0,0,'art/2D/Recipes/wooden_pillar_with_shield2.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5814,'Tribe Camp Big Shield III','',NULL,62,0,2954,10,1,0,0,'art/2D/Recipes/wooden_pillar_with_shield.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5815,'Tribe Camp Tree I','',NULL,62,0,2955,10,1,0,0,'art/2D/Recipes/Pillars_AA.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5816,'Tribe Camp Tree II','',NULL,62,0,2956,10,1,0,0,'art/2D/Recipes/Pillars_AB.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5817,'Tribe Camp Tree III','',NULL,62,0,2957,10,1,0,0,'art/2D/Recipes/Pillars_AA.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5818,'Tribe Camp Tree IV','',NULL,62,0,2958,10,1,0,0,'art/2D/Recipes/Pillars_AB.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5819,'Tribe Camp Tree V','',NULL,62,0,2959,10,1,0,0,'art/2D/Recipes/Pillars_AA.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5820,'Tribe Camp Tree VI','',NULL,62,0,2960,10,1,0,0,'art/2D/Recipes/Pillars_BA.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe` VALUES (5821,'Tribe Camp Tree VII','',NULL,62,0,2961,10,1,0,0,'art/2D/Recipes/Pillars_BB.png')");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5800,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5801,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5802,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5803,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5804,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5805,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5806,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5807,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5808,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5809,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5810,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5811,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5812,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5813,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5814,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5815,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5816,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5817,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5818,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5819,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5820,1636,0,90,1,0)");
        // dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES (NULL,5821,1636,0,90,1,0)");
    }
};
activatePackage(LiFxTribeCampDecorations);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxTribeCampDecorations);
