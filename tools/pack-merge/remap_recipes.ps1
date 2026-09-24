$ErrorActionPreference = 'Stop'
$srcFile = "E:\ClaudeScratch\jorvik\server_pack\Jorvik MOD server pack\Jorvik MOD server pack\sql\dump.sql"
$lines = Get-Content $srcFile -Encoding UTF8
$block = $lines[3116..3203]  # 0-indexed: lines 3117-3204 (1-indexed), Jorvik recipe rows

# rebuild the id map from the objects_types remap step
$map = @{}
Get-Content "E:\ClaudeScratch\jorvik\id_map.txt" | Where-Object { $_ -match '^\d+=\d+$' } | ForEach-Object {
    $parts = $_ -split '='
    $map[[int]$parts[0]] = [int]$parts[1]
}

# Modular construction object ids being excluded entirely - any recipe whose ResultObjectTypeID is one of these is dropped
$exclude = @(2411,2412,2413,2414,2415,2416,2417,2418,2419,2420,2421,2422,2423,2424,2425,2426,2427,2428,2429,2430,2431,2432,2433,2434,2435,2436,2438,2439,2440,2441,2442,2443,2444,2445,2446,2465)

$rowRegex = "^\((\d+),'((?:[^'\\]|\\.)*)','((?:[^'\\]|\\.)*)',(NULL|-?\d+),(NULL|-?\d+),(NULL|-?\d+),(NULL|-?\d+),(NULL|-?\d+),(NULL|-?\d+),(NULL|-?\d+),(NULL|-?\d+),'((?:[^'\\]|\\.)*)'\)([,;])$"

$kept = @()
$droppedCount = 0
foreach ($line in $block) {
    $t = $line.Trim()
    if ($t -eq "") { continue }
    if ($t -notmatch $rowRegex) { Write-Warning "NO MATCH: $t"; continue }
    $id = $Matches[1]; $name = $Matches[2]; $desc = $Matches[3]
    $startTools = $Matches[4]; $skillType = $Matches[5]; $skillLvl = $Matches[6]
    $result = $Matches[7]; $skillDep = $Matches[8]; $qty = $Matches[9]
    $auto = $Matches[10]; $blueprint = $Matches[11]; $img = $Matches[12]; $term = $Matches[13]

    $resultInt = 0
    [int]::TryParse($result, [ref]$resultInt) | Out-Null
    if ($exclude -contains $resultInt) { $droppedCount++; continue }

    if ($map.ContainsKey($resultInt)) { $result = [string]$map[$resultInt] }
    $stInt = 0
    if ([int]::TryParse($startTools, [ref]$stInt) -and $map.ContainsKey($stInt)) { $startTools = [string]$map[$stInt] }

    $newLine = "($id,'$name','$desc',$startTools,$skillType,$skillLvl,$result,$skillDep,$qty,$auto,$blueprint,'$img')$term"
    $kept += $newLine
}
"Kept recipes: " + $kept.Count
"Dropped (modular): " + $droppedCount
$kept -join "`r`n" | Out-File "E:\ClaudeScratch\jorvik\recipes_remapped.sql" -Encoding utf8
"done"
