$ErrorActionPreference = 'Stop'
$srcFile = "E:\ClaudeScratch\jorvik\server_pack\Jorvik MOD server pack\Jorvik MOD server pack\sql\dump.sql"
$lines = Get-Content $srcFile -Encoding UTF8
$block = $lines[1930..2019]  # 0-indexed: lines 1931-2020 (1-indexed) = objects_types rows for Jorvik

# Modular construction IDs to EXCLUDE entirely (categories 2411/2427 + all their children + duplicates)
$exclude = @(2411,2412,2413,2414,2415,2416,2417,2418,2419,2420,2421,2422,2423,2424,2425,2426,2427,2428,2429,2430,2431,2432,2433,2434,2435,2436,2438,2439,2440,2441,2442,2443,2444,2445,2446,2465)

$map = @{}
$nextId = 3000
$keptRows = @()
foreach ($line in $block) {
    if ($line -match '^\((\d+),') {
        $oldId = [int]$Matches[1]
        if ($exclude -contains $oldId) { continue }
        $map[$oldId] = $nextId
        $newLine = $line -replace "^\($oldId,", "($nextId,"
        $keptRows += $newLine
        $nextId++
    }
}
"Kept rows: " + $keptRows.Count
"ID range: 3000-" + ($nextId - 1)
"" | Out-File "E:\ClaudeScratch\jorvik\id_map.txt"
foreach ($k in ($map.Keys | Sort-Object)) { "$k=$($map[$k])" | Out-File "E:\ClaudeScratch\jorvik\id_map.txt" -Append }
$keptRows -join "`r`n" | Out-File "E:\ClaudeScratch\jorvik\objects_types_remapped.sql" -Encoding utf8
"done"
