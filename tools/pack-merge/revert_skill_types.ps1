$ErrorActionPreference = 'Stop'
$cl = "F:\SteamLibrary\steamapps\common\Life is Feudal Your Own"
$srv = "F:\SteamLibrary\steamapps\common\Life is Feudal Your Own Dedicated Server"

$edits = @(
    @{ Id = 96;  Old = '<ent_req type="not_object_type_id">77 1070 1071 1079 1080 1083 1090 1448 1449 1457 2003</ent_req>'; New = '<ent_req type="not_object_type_id">77 1070 1071 1079 1080 1083 1090 1448 1449 1457 2003 3000 3031</ent_req>' },
    @{ Id = 108; Old = '<ent_req type="object_type_id">1437 1496</ent_req>'; New = '<ent_req type="object_type_id">1437 1496 3030</ent_req>' },
    @{ Id = 144; Old = '<ent_req type="object_type_id">127 128 131 132 134 139 140 146 147 148 186 187 188 189 455 516 518 519 520 521 1055 1085 1086 1087 1088 1089 1109 1077 1665 2894</ent_req>'; New = '<ent_req type="object_type_id">127 128 131 132 134 139 140 146 147 148 186 187 188 189 455 516 518 519 520 521 1055 1085 1086 1087 1088 1089 1109 1077 1665 2894 3003 3004 3031 3038 3040 3041</ent_req>' },
    @{ Id = 145; Old = '<ent_req type="object_type_id">139 140 142 145 146 1055 147 518 519 521 148 1084 1085 1086 1087 1088 1089 1355</ent_req>'; New = '<ent_req type="object_type_id">139 140 142 145 146 1055 147 518 519 521 148 1084 1085 1086 1087 1088 1089 1355 3003</ent_req>' },
    @{ Id = 268; Old = '<ent_req type="object_type_id">86 165 166 1336 1629 1630 1631 1632 1633 1657 1658 1659 1660 1661</ent_req>'; New = '<ent_req type="object_type_id">86 165 166 1336 1629 1630 1631 1632 1633 1657 1658 1659 1660 1661 3012 3014</ent_req>' },
    @{ Id = 288; Old = '<ent_req type="object_type_id">1461 1497</ent_req>'; New = '<ent_req type="object_type_id">1461 1497 3030 3031</ent_req>' },
    @{ Id = 289; Old = '<ent_req type="object_type_id">1437 1496</ent_req>'; New = '<ent_req type="object_type_id">1437 1496 3030 3031</ent_req>' },
    @{ Id = 354; Old = '<ent_req type="object_type_id">1670 1671 1733</ent_req>'; New = '<ent_req type="object_type_id">1670 1671 1733 3040</ent_req>' },
    @{ Id = 105; Old = '<req type="tool_id" parent="1">43 1647</req>'; New = '<req type="tool_id" parent="1">43 1647 3047 3048</req>' }
)
$doubleEdits = @(
    @{ Id = 300; OldA = '<ent_req type="object_type_id">1437 1496</ent_req>'; NewA = '<ent_req type="object_type_id">1437 1496 3030 3031</ent_req>'; OldB = '<ent_req type="object_type_id">1461 1497</ent_req>'; NewB = '<ent_req type="object_type_id">1461 1497 3030 3031</ent_req>' },
    @{ Id = 301; OldA = '<ent_req type="object_type_id">1437 1496</ent_req>'; NewA = '<ent_req type="object_type_id">1437 1496 3030</ent_req>'; OldB = '<ent_req type="object_type_id">1461 1497</ent_req>'; NewB = '<ent_req type="object_type_id">1461 1497 3030</ent_req>' },
    @{ Id = 224; OldA = '<ent_req type="object_type_id">1437 1496</ent_req>'; NewA = '<ent_req type="object_type_id">1437 1496 3030 3031</ent_req>' }
)

function Edit-AbilityBlock($text, $abilId, $oldStr, $newStr) {
    $pattern = '(<ability\b[^>]*\bid="' + $abilId + '">)([\s\S]*?)(</ability>)'
    $m = [regex]::Match($text, $pattern)
    if (-not $m.Success) { throw "ability id=$abilId not found" }
    $body = $m.Groups[2].Value
    if (-not $body.Contains($oldStr)) { throw "revert-source string not found in ability id=$abilId : $oldStr" }
    $newBody = $body.Replace($oldStr, $newStr)
    $newBlock = $m.Groups[1].Value + $newBody + $m.Groups[3].Value
    return $text.Substring(0, $m.Index) + $newBlock + $text.Substring($m.Index + $m.Length)
}

foreach ($f in "$cl\data\skill_types.xml", "$srv\data\skill_types.xml") {
    $t = [IO.File]::ReadAllText($f)
    # to REVERT: swap New (current state) back to Old (original vanilla state)
    foreach ($e in $edits) {
        $t = Edit-AbilityBlock $t $e.Id $e.New $e.Old
    }
    foreach ($e in $doubleEdits) {
        $t = Edit-AbilityBlock $t $e.Id $e.NewA $e.OldA
        if ($e.ContainsKey('NewB')) { $t = Edit-AbilityBlock $t $e.Id $e.NewB $e.OldB }
    }
    [IO.File]::WriteAllText($f, $t, (New-Object Text.UTF8Encoding($false)))
    "reverted $f"
}
