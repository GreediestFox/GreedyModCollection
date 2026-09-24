$ErrorActionPreference = 'Stop'
$srcFile = "E:\ClaudeScratch\jorvik\server_pack\Jorvik MOD server pack\Jorvik MOD server pack\sql\dump.sql"
$lines = Get-Content $srcFile -Encoding UTF8
# recipe_requirement INSERT VALUES rows: lines 3212-6923 (1-indexed) per "Dumping data" markers found earlier (3207) through just before "effects" dump (6926)
$block = $lines[3211..6922]  # 0-indexed

$map = @{}
Get-Content "E:\ClaudeScratch\jorvik\id_map.txt" | Where-Object { $_ -match '^\d+=\d+$' } | ForEach-Object {
    $parts = $_ -split '='
    $map[[int]$parts[0]] = [int]$parts[1]
}

# recipe IDs we kept (from the recipes_remapped.sql we already produced)
$keptRecipeIds = @{}
Get-Content "E:\ClaudeScratch\jorvik\recipes_remapped.sql" | ForEach-Object {
    if ($_ -match '^\((\d+),') { $keptRecipeIds[[int]$Matches[1]] = $true }
}

$rowRegex = '^\((\d+),(\d+),(NULL|-?\d+),(NULL|-?\d+),(NULL|-?\d+),(NULL|-?\d+),(NULL|-?\d+)\)([,;])$'

$kept = @()
$skippedNotOurs = 0
$skippedDropped = 0
foreach ($line in $block) {
    $t = $line.Trim()
    if ($t -eq "") { continue }
    if ($t -notmatch $rowRegex) { continue }  # not a 7-field req row (e.g. blueprint table leaked in) - skip silently, not ours
    $reqId = $Matches[1]; $recipeId = [int]$Matches[2]; $matId = $Matches[3]
    $quality = $Matches[4]; $infl = $Matches[5]; $qty = $Matches[6]; $region = $Matches[7]; $term = $Matches[8]

    if ($recipeId -lt 1087 -or $recipeId -gt 1174) { $skippedNotOurs++; continue }
    if (-not $keptRecipeIds.ContainsKey($recipeId)) { $skippedDropped++; continue }

    $matInt = 0
    if ([int]::TryParse($matId, [ref]$matInt) -and $map.ContainsKey($matInt)) { $matId = [string]$map[$matInt] }

    $kept += "(NULL,$recipeId,$matId,$quality,$infl,$qty,$region)$term"
}
"Kept requirement rows: " + $kept.Count
"Skipped (recipe id out of jorvik range): " + $skippedNotOurs
"Skipped (recipe was dropped - modular): " + $skippedDropped
$kept -join "`r`n" | Out-File "E:\ClaudeScratch\jorvik\requirements_remapped.sql" -Encoding utf8
"done"
