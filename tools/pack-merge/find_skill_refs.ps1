$ErrorActionPreference = 'Stop'
$srcFile = "E:\ClaudeScratch\jorvik\server_pack\Jorvik MOD server pack\Jorvik MOD server pack\data\skill_types.xml"
$lines = Get-Content $srcFile -Encoding UTF8

$keep = @(2400,2401,2402,2403,2404,2405,2406,2407,2408,2409,2410,2437,2447,2448,2449,2450,2451,2452,2453,2454,2455,2456,2457,2458,2459,2460,2461,2462,2463,2464,2466,2467,2468,2469,2470,2471,2472,2473,2474,2475,2476,2477,2478,2479,2480,2481,2482,2483,2484,2485,2486,2487,2488,2489)
$keepSet = @{}
foreach ($k in $keep) { $keepSet[$k] = $true }

$results = @()
for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -match '<ent_req type="(object_type_id|not_object_type_id)">([\d\s]+)</ent_req>' -or $line -match '<req type="tool_id"[^>]*>([\d\s]+)</req>') {
        $idsStr = if ($Matches.Count -ge 3 -and $Matches[2]) { $Matches[2] } else { $Matches[1] }
        $ids = $idsStr -split '\s+' | Where-Object { $_ -ne '' } | ForEach-Object { [int]$_ }
        $matched = $ids | Where-Object { $keepSet.ContainsKey($_) }
        if ($matched.Count -gt 0) {
            # walk backward to find enclosing ability id/name
            $abilId = "?"; $abilName = "?"
            for ($j = $i; $j -ge 0; $j--) {
                if ($lines[$j] -match '<ability[^>]*\bid="(\d+)"') {
                    $abilId = $Matches[1]
                    if ($lines[$j] -match 'name="([^"]*)"') { $abilName = $Matches[1] }
                    break
                }
            }
            $fieldType = 'object_type_id'
            if ($line -match 'not_object_type_id') { $fieldType = 'not_object_type_id' }
            elseif ($line -match 'tool_id') { $fieldType = 'tool_id' }
            $results += [PSCustomObject]@{ Line = $i+1; AbilityId = $abilId; AbilityName = $abilName; FieldType = $fieldType; Matched = ($matched -join ' '); FullText = $line.Trim() }
        }
    }
}
$results | Format-Table -AutoSize | Out-String -Width 300 | Out-File "E:\ClaudeScratch\jorvik\skill_refs.txt" -Encoding utf8
$results.Count
