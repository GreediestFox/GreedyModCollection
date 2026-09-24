$ErrorActionPreference = 'Stop'
$srcFile = "E:\ClaudeScratch\jorvik\client_pack\Jorvik MOD client pack\Jorvik MOD client pack\data\recipe.xml"
$lines = Get-Content $srcFile -Encoding UTF8
$block = $lines[13795..15113]  # 0-indexed: lines 13796-15114

$map = @{}
Get-Content "E:\ClaudeScratch\jorvik\id_map.txt" | Where-Object { $_ -match '^\d+=\d+$' } | ForEach-Object {
    $parts = $_ -split '='
    $map[[int]$parts[0]] = [int]$parts[1]
}
$exclude = @(2411,2412,2413,2414,2415,2416,2417,2418,2419,2420,2421,2422,2423,2424,2425,2426,2427,2428,2429,2430,2431,2432,2433,2434,2435,2436,2438,2439,2440,2441,2442,2443,2444,2445,2446,2465)

$kept = New-Object System.Collections.Generic.List[string]
$currentBlock = New-Object System.Collections.Generic.List[string]
$inRow = $false
$currentKeep = $true

foreach ($line in $block) {
    if ($line -match '^\s*<row>\s*$') {
        $inRow = $true
        $currentBlock = New-Object System.Collections.Generic.List[string]
        $currentKeep = $true
        $currentBlock.Add($line)
        continue
    }
    if ($inRow) {
        if ($line -match '<ResultObjectTypeID>(\d+)</ResultObjectTypeID>') {
            $rid = [int]$Matches[1]
            if ($exclude -contains $rid) { $currentKeep = $false }
            $newLine = $line
            if ($map.ContainsKey($rid)) { $newLine = $line -replace "<ResultObjectTypeID>$rid</ResultObjectTypeID>", "<ResultObjectTypeID>$($map[$rid])</ResultObjectTypeID>" }
            $currentBlock.Add($newLine)
            continue
        }
        if ($line -match '<StartingToolsID>(\d+)</StartingToolsID>') {
            $sid = [int]$Matches[1]
            $newLine = $line
            if ($map.ContainsKey($sid)) { $newLine = $line -replace "<StartingToolsID>$sid</StartingToolsID>", "<StartingToolsID>$($map[$sid])</StartingToolsID>" }
            $currentBlock.Add($newLine)
            continue
        }
        $currentBlock.Add($line)
        if ($line -match '</row>') {
            $inRow = $false
            if ($currentKeep) { $kept.AddRange($currentBlock) }
        }
    }
}
"Kept recipe rows: " + (($kept | Select-String '<ID>').Count)
($kept -join "`r`n") | Out-File "E:\ClaudeScratch\jorvik\client_recipe_remapped.xml" -Encoding utf8
"done"
