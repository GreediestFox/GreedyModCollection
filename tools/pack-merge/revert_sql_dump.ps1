$ErrorActionPreference = 'Stop'
$f = "F:\SteamLibrary\steamapps\common\Life is Feudal Your Own Dedicated Server\sql\dump.sql"
$t = [IO.File]::ReadAllText($f)

function RemoveSpan($text, $startMarker, $endMarkerRegex, $precedingTerminatorFix) {
    $startIdx = $text.IndexOf($startMarker)
    if ($startIdx -lt 0) { throw "start marker not found: $startMarker" }
    # find the comma or semicolon just before startMarker that begins our inserted block (we inserted ",\r\n" + rows)
    # walk backward from startIdx to find the preceding ",\r\n" we added
    $beforeText = $text.Substring(0, $startIdx)
    $lastCommaNewline = $beforeText.LastIndexOf(",`r`n")
    if ($lastCommaNewline -lt 0) { $lastCommaNewline = $beforeText.LastIndexOf(",`n") }
    if ($lastCommaNewline -lt 0) { throw "could not find preceding comma+newline before $startMarker" }
    # find end of our block: the end marker regex match after startIdx
    $searchFrom = $startIdx
    $m = [regex]::Match($text.Substring($searchFrom), $endMarkerRegex)
    if (-not $m.Success) { throw "end marker not found after start: $startMarker" }
    $endIdx = $searchFrom + $m.Index + $m.Length
    $result = $text.Substring(0, $lastCommaNewline) + $precedingTerminatorFix + $text.Substring($endIdx)
    return $result
}

# objects_types block: (3000,... ... (3053,213,'Copper Blanks',...);
$t = RemoveSpan $t "(3000,69,'Small Wooden Shed'" "\(3053,213,'Copper Blanks'[^\r\n]*\);" ";"

# recipe block: (1087,'Small Wooden Shed', ... (1174,'Copper Coins',...);
$t = RemoveSpan $t "(1087,'Small Wooden Shed','Object from Jorvik MOD'" "\(1174,'Copper Coins'[^\r\n]*\);" ";"

# recipe_requirement block: (NULL,1087,... ... (NULL,1174,453,0,10,15,0);
$t = RemoveSpan $t "(NULL,1087,233,0,20,4,0)" "\(NULL,1174,453,0,10,15,0\);" ";"

[IO.File]::WriteAllText($f, $t, (New-Object Text.UTF8Encoding($false)))
"reverted sql/dump.sql"
