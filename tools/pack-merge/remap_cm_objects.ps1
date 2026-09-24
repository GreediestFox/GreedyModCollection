$ErrorActionPreference = 'Stop'
$srcFile = "E:\ClaudeScratch\jorvik\server_pack\Jorvik MOD server pack\Jorvik MOD server pack\data\cm_objects.xml"
$lines = Get-Content $srcFile -Encoding UTF8
$block = $lines[26216..29885]  # 0-indexed: lines 26217-29886 (includes trailing </object_types> which we'll ignore)

$map = @{}
Get-Content "E:\ClaudeScratch\jorvik\id_map.txt" | Where-Object { $_ -match '^\d+=\d+$' } | ForEach-Object {
    $parts = $_ -split '='
    $map[[int]$parts[0]] = [int]$parts[1]
}
$exclude = @(2411,2412,2413,2414,2415,2416,2417,2418,2419,2420,2421,2422,2423,2424,2425,2426,2427,2428,2429,2430,2431,2432,2433,2434,2435,2436,2438,2439,2440,2441,2442,2443,2444,2445,2446,2465)

$kept = New-Object System.Collections.Generic.List[string]
$inBlock = $false
$currentBlock = New-Object System.Collections.Generic.List[string]
$currentId = -1
$currentKeep = $false
$keptCount = 0
$droppedCount = 0

foreach ($line in $block) {
    if ($line -match '<object id="(\d+)"') {
        $id = [int]$Matches[1]
        $inBlock = $true
        $currentId = $id
        $currentKeep = -not ($exclude -contains $id)
        $currentBlock = New-Object System.Collections.Generic.List[string]
        if ($currentKeep -and $map.ContainsKey($id)) {
            $newLine = $line -replace "id=`"$id`"", "id=`"$($map[$id])`""
            $currentBlock.Add($newLine)
        } else {
            $currentBlock.Add($line)
        }
        continue
    }
    if ($inBlock) {
        $currentBlock.Add($line)
        if ($line -match '</object>') {
            $inBlock = $false
            if ($currentKeep) {
                $kept.AddRange($currentBlock)
                $keptCount++
            } else {
                $droppedCount++
            }
        }
    }
}
"Kept object blocks: $keptCount"
"Dropped (modular) blocks: $droppedCount"
($kept -join "`r`n") | Out-File "E:\ClaudeScratch\jorvik\cm_objects_remapped.xml" -Encoding utf8
"done"
