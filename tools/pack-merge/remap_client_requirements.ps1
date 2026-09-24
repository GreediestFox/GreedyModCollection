$ErrorActionPreference = 'Stop'
$srcFile = "E:\ClaudeScratch\jorvik\client_pack\Jorvik MOD client pack\Jorvik MOD client pack\data\recipe_requirement.xml"
$lines = Get-Content $srcFile -Encoding UTF8
$block = $lines[30344..33081]  # 0-indexed: lines 30345-33082

$map = @{}
Get-Content "E:\ClaudeScratch\jorvik\id_map.txt" | Where-Object { $_ -match '^\d+=\d+$' } | ForEach-Object {
    $parts = $_ -split '='
    $map[[int]$parts[0]] = [int]$parts[1]
}
$keptRecipeIds = @{}
Get-Content "E:\ClaudeScratch\jorvik\recipes_remapped.sql" | ForEach-Object {
    if ($_ -match '^\((\d+),') { $keptRecipeIds[[int]$Matches[1]] = $true }
}

$kept = New-Object System.Collections.Generic.List[string]
$currentBlock = New-Object System.Collections.Generic.List[string]
$inRow = $false
$currentKeep = $true
$nextId = 92000

foreach ($line in $block) {
    if ($line -match '^\s*<row>\s*$') {
        $inRow = $true
        $currentBlock = New-Object System.Collections.Generic.List[string]
        $currentKeep = $true
        $currentBlock.Add($line)
        continue
    }
    if ($inRow) {
        if ($line -match '<ID>(\d+)</ID>') {
            $currentBlock.Add(($line -replace '<ID>\d+</ID>', "<ID>$nextId</ID>"))
            $nextId++
            continue
        }
        if ($line -match '<RecipeID>(\d+)</RecipeID>') {
            $rid = [int]$Matches[1]
            if (-not $keptRecipeIds.ContainsKey($rid)) { $currentKeep = $false }
            $currentBlock.Add($line)
            continue
        }
        if ($line -match '<MaterialObjectTypeID>(\d+)</MaterialObjectTypeID>') {
            $mid = [int]$Matches[1]
            $newLine = $line
            if ($map.ContainsKey($mid)) { $newLine = $line -replace "<MaterialObjectTypeID>$mid</MaterialObjectTypeID>", "<MaterialObjectTypeID>$($map[$mid])</MaterialObjectTypeID>" }
            $currentBlock.Add($newLine)
            continue
        }
        $currentBlock.Add($line)
        if ($line -match '</row>') {
            $inRow = $false
            if ($currentKeep) { $kept.AddRange($currentBlock) } else { $nextId-- }
        }
    }
}
"Kept requirement rows: " + (($kept | Select-String '<RecipeID>').Count)
"Next free client requirement ID after this batch: $nextId"
($kept -join "`r`n") | Out-File "E:\ClaudeScratch\jorvik\client_requirements_remapped.xml" -Encoding utf8
"done"
