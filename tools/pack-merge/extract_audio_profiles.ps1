$ErrorActionPreference = 'Stop'
$srcFile = "E:\ClaudeScratch\jorvik\client_pack\Jorvik MOD client pack\Jorvik MOD client pack\art\datablocks\audioProfiles.cs"
$text = [IO.File]::ReadAllText($srcFile)

$names = @("env_fire","env_bell","wolf_idle","aurochsbull_idle","aurochsbull_eat","aurochsbull_sleep","wranen_stand","slave_working","wildhorse_idle","wildhorse_eat","wildhorse_sleep","sow_idle","sow_eat","sow_sleep","aurochscow_idle","aurochscow_eat","aurochscow_sleep")

$out = @()
foreach ($n in $names) {
    $pattern = 'datablock SFXProfile\(' + [regex]::Escape($n) + '\)\s*\{[^}]*\};'
    $matches = [regex]::Matches($text, $pattern)
    foreach ($m in $matches) { $out += $m.Value }
}
"Found blocks: " + $out.Count
($out -join "`r`n`r`n") | Out-File "E:\ClaudeScratch\jorvik\audio_profiles_extracted.cs" -Encoding utf8
"done"
