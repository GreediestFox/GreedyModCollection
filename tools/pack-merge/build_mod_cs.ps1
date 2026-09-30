$ErrorActionPreference = 'Stop'

function ConvertPaths($line) {
    # Convert SQL-escaped backslash paths to forward slashes (avoids TS/SQL double-escaping issues)
    return $line -replace '\\\\', '/'
}

$objLines = Get-Content "E:\ClaudeScratch\jorvik\objects_types_remapped.sql"
$recipeLines = Get-Content "E:\ClaudeScratch\jorvik\recipes_remapped.sql"
$reqLines = Get-Content "E:\ClaudeScratch\jorvik\requirements_remapped.sql"

$out = New-Object System.Collections.Generic.List[string]
$out.Add('/**')
$out.Add('* <author>Warped ibun (Jorvik Mod), ported by GreedyFox</author>')
$out.Add('* <description>Jorvik Mod 2.1.0 content (modular log cabin system excluded), ids remapped to 3000-3053 to avoid collisions with existing custom content (2400/2461-2466/2480 were already used by Small Wooden Shed/Knool weapons/Woodshed).</description>')
$out.Add('* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>')
$out.Add('*/')
$out.Add('')
$out.Add('if (!isObject(LiFxJorvikModPack))')
$out.Add('{')
$out.Add('    new ScriptObject(LiFxJorvikModPack)')
$out.Add('    {')
$out.Add('    };')
$out.Add('}')
$out.Add('')
$out.Add('package LiFxJorvikModPack')
$out.Add('{')
$out.Add('    function LiFxJorvikModPack::setup() {')
$out.Add('        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxJorvikModPack);')
$out.Add('    }')
$out.Add('    function LiFxJorvikModPack::version() {')
$out.Add('        return "1.0.0";')
$out.Add('    }')
$out.Add('    function LiFxJorvikModPack::dbChanges() {')

foreach ($line in $objLines) {
    $row = $line.TrimEnd(',', ';')
    $row = ConvertPaths $row
    $out.Add('        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES ' + $row + '");')
}
$out.Add('')
$out.Add('        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` BETWEEN 1087 AND 1174");')
foreach ($line in $recipeLines) {
    $row = $line.TrimEnd(',', ';')
    $row = ConvertPaths $row
    $out.Add('        dbi.Update("INSERT IGNORE INTO `recipe` VALUES ' + $row + '");')
}
foreach ($line in $reqLines) {
    $row = $line.TrimEnd(',', ';')
    $row = ConvertPaths $row
    $out.Add('        dbi.Update("INSERT IGNORE INTO `recipe_requirement` VALUES ' + $row + '");')
}

$out.Add('    }')
$out.Add('};')
$out.Add('activatePackage(LiFxJorvikModPack);')
$out.Add('LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxJorvikModPack);')

$out -join "`r`n" | Out-File "E:\ClaudeScratch\jorvik\JorvikModPack_mod.cs" -Encoding utf8
"Generated mod.cs with " + ($objLines.Count + $recipeLines.Count + $reqLines.Count) + " dbi.Update calls"
