$ErrorActionPreference = 'Stop'
$srcFile = "E:\ClaudeScratch\jorvik\client_pack\Jorvik MOD client pack\Jorvik MOD client pack\data\objects_types.xml"
$lines = Get-Content $srcFile -Encoding UTF8
$block = $lines[36181..37980]  # 0-indexed: lines 36182-37981 covers full <row>...</row> range (2400-2489)

$map = @{}
Get-Content "E:\ClaudeScratch\jorvik\id_map.txt" | Where-Object { $_ -match '^\d+=\d+$' } | ForEach-Object {
    $parts = $_ -split '='
    $map[[int]$parts[0]] = [int]$parts[1]
}
$exclude = @(2411,2412,2413,2414,2415,2416,2417,2418,2419,2420,2421,2422,2423,2424,2425,2426,2427,2428,2429,2430,2431,2432,2433,2434,2435,2436,2438,2439,2440,2441,2442,2443,2444,2445,2446,2465)

$kept = New-Object System.Collections.Generic.List[string]
$currentBlock = New-Object System.Collections.Generic.List[string]
$inRow = $false
$currentId = -1
$currentKeep = $false
$keptCount = 0
$droppedCount = 0

foreach ($line in $block) {
    if ($line -match '^\s*<row>\s*$') {
        $inRow = $true
        $currentBlock = New-Object System.Collections.Generic.List[string]
        $currentBlock.Add($line)
        continue
    }
    if ($inRow) {
        if ($line -match '<ID>(\d+)</ID>') {
            $currentId = [int]$Matches[1]
            $currentKeep = -not ($exclude -contains $currentId)
            if ($currentKeep -and $map.ContainsKey($currentId)) {
                $newLine = $line -replace "<ID>$currentId</ID>", "<ID>$($map[$currentId])</ID>"
                $currentBlock.Add($newLine)
            } else {
                $currentBlock.Add($line)
            }
            continue
        }
        $currentBlock.Add($line)
        if ($line -match '</row>') {
            $inRow = $false
            if ($currentKeep) {
                $kept.AddRange($currentBlock)
                $keptCount++
            } else {
                $droppedCount++
            }
        }
    }
}
"Kept rows: $keptCount"
"Dropped (modular) rows: $droppedCount"
($kept -join "`r`n") | Out-File "E:\ClaudeScratch\jorvik\client_objects_types_remapped.xml" -Encoding utf8
"done"
