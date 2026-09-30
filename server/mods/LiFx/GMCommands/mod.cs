/**
* <author>GreedyFox</author>
* <description>GM commands for Lif:YO, built on real engine functions (found by decompilation), not on the fictional API of the old GMTools package.
*              Typed in normal chat (or via the client console helper gm("...") in mod/GMHelper/gm.cs). Only known commands are intercepted, so normal
*              "!shout" chat still works. Allowed for accounts flagged IsGM in the database (NetConnection::isGM) or listed in $LiFxGM::accounts.
*              Every command is logged with the prefix [GM].
*              !help | !whoami | !players | !give <type> [qty] [quality] [durability] | !giveto <char id or name> <type> [qty] [quality] [durability]
*              !spawn <movable object type id> | !build <building type id> | !geo   (CreateTestMovable/Unmovable at Lifx::worldToGeoId(pos), a C++ helper in the LiFx DLL)
*              !pos | !tppos <x> <y> <z> | !tp <char id or name> | !bring <char id or name>
*              !announce <text> | !kick <char id or name> | !ban <char id or name> | !unban <char id or name>
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*/

if (!isObject(LiFxGMCommands))
{
    new ScriptObject(LiFxGMCommands)
    {
    };
}

// Extra GM accounts (account ids) that are allowed even when the account is not flagged IsGM. Fill in from "!whoami".
$LiFxGM::accounts = "2";

// Quality given to the ground layer that !ground / !raise puts down (the engine default is 100).
$LiFxGM::groundQuality = 10;

// Terrain substance ids (ter2_id from the game's cm_substances): used by !ground / !raise. Taken from the old GMTools notes, verify in game.
$LiFxGM::substance["rock"] = 3;
$LiFxGM::substance["riverrock"] = 27;
$LiFxGM::substance["swamp"] = 26;

// GM password: only a salted hash is kept, in gm_password.cs (written by "!setpass" from the GM panel, never contains the password itself).
$LiFxGM::passwordHash = "";
$LiFxGM::passwordSalt = "";
if (isFile("mods/LiFx/GMCommands/gm_password.cs")) {
    exec("mods/LiFx/GMCommands/gm_password.cs");
}

// !spawn / !build place the object this many map tiles (4 world units each) in front of the GM.
$LiFxGM::spawnOffsetTiles = 1;

package LiFxGMCommands
{
    function LiFxGMCommands::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxGMCommands);
    }

    // GM action log table (every executed GM command, never passwords).
    function LiFxGMCommands::dbChanges() {
        // Inflation helper: one snapshot per day, plus a small settings table (the coin baseline).
        dbi.Update("CREATE TABLE IF NOT EXISTS `lifx_gm_settings` (`Name` VARCHAR(40) NOT NULL, `Value` DOUBLE NOT NULL DEFAULT 0, PRIMARY KEY (`Name`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3");
        dbi.Update("CREATE TABLE IF NOT EXISTS `lifx_inflation_log` (`SnapshotDate` DATE NOT NULL, `Copper` BIGINT NOT NULL DEFAULT 0, `Silver` BIGINT NOT NULL DEFAULT 0, `Gold` BIGINT NOT NULL DEFAULT 0, `CoinValue` BIGINT NOT NULL DEFAULT 0, `Players` INT NOT NULL DEFAULT 1, `PerPlayer` DOUBLE NOT NULL DEFAULT 0, `Baseline` DOUBLE NOT NULL DEFAULT 0, `InflationPct` DOUBLE NOT NULL DEFAULT 0, PRIMARY KEY (`SnapshotDate`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3");
        dbi.Update("CREATE TABLE IF NOT EXISTS `lifx_gm_log` (`ID` INT UNSIGNED NOT NULL AUTO_INCREMENT, `LoggedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, `AccountID` INT UNSIGNED NOT NULL DEFAULT 0, `CharID` INT UNSIGNED NOT NULL DEFAULT 0, `Action` VARCHAR(255) NOT NULL DEFAULT '', PRIMARY KEY (`ID`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3");
    }

    function LiFxGMCommands::logAction(%client, %text) {
        %text = getSubStr(LiFxGMCommands::clean(%text), 0, 200);
        dbi.Update("INSERT INTO `lifx_gm_log` (`AccountID`, `CharID`, `Action`) VALUES (" @ %client.getAccountId() @ ", " @ %client.getCharacterId() @ ", '" @ %text @ "')");
    }

    // First object of a class anywhere under a group (used to find the world's TimeOfDay and Precipitation objects).
    function LiFxGMCommands::findByClass(%group, %class) {
        %n = %group.getCount();
        for (%i = 0; %i < %n; %i++) {
            %o = %group.getObject(%i);
            if (%o.getClassName() $= %class) {
                return %o;
            }
            if (%o.isMethod("getCount")) {
                %r = LiFxGMCommands::findByClass(%o, %class);
                if (%r) {
                    return %r;
                }
            }
        }
        return 0;
    }

    function LiFxGMCommands::allByClass(%group, %class, %list) {
        %n = %group.getCount();
        for (%i = 0; %i < %n; %i++) {
            %o = %group.getObject(%i);
            if (%o.getClassName() $= %class) {
                %list = %list SPC %o;
            }
            if (%o.isMethod("getCount")) {
                %list = LiFxGMCommands::allByClass(%o, %class, %list);
            }
        }
        return %list;
    }

    function LiFxGMCommands::version() {
        return "0.13.1";
    }

    function LiFxGMCommands::reply(%client, %text) {
        %client.cmSendClientMessage(2475, "<color:d8d8d8>" @ %text);
    }

    // Account-level right to use GM commands (account flagged IsGM, or listed in $LiFxGM::accounts).
    function LiFxGMCommands::hasRight(%client) {
        if (%client.isGM()) {
            return 1;
        }
        %id = %client.getAccountId();
        for (%i = 0; %i < getWordCount($LiFxGM::accounts); %i++) {
            if (getWord($LiFxGM::accounts, %i) $= %id) {
                return 1;
            }
        }
        return 0;
    }

    // Right + logged in with the GM password this session.
    function LiFxGMCommands::isAllowed(%client) {
        return (LiFxGMCommands::hasRight(%client) && %client.gmLogin == 1);
    }

    function LiFxGMCommands::hashPassword(%pw) {
        return LiFxSHA256::hash($LiFxGM::passwordSalt @ %pw);
    }

    // Returns 1 when the salted hash was made and saved, 0 when hashing is unavailable (nothing is stored then).
    function LiFxGMCommands::savePassword(%pw) {
        %salt = getSubStr(LiFxSHA256::hash(getRealTime() SPC getSimTime() SPC %pw SPC getRandom(1, 1000000000)), 0, 24);
        if (%salt $= "") {
            echo("[GM] password hashing is not available (LiFxSHA256::hash returned nothing)");
            return 0;
        }
        $LiFxGM::passwordSalt = %salt;
        $LiFxGM::passwordHash = LiFxGMCommands::hashPassword(%pw);
        if ($LiFxGM::passwordHash $= "") {
            $LiFxGM::passwordSalt = "";
            return 0;
        }
        %fo = new FileObject();
        if (%fo.openForWrite("mods/LiFx/GMCommands/gm_password.cs")) {
            %fo.writeLine("$LiFxGM::passwordSalt = \"" @ $LiFxGM::passwordSalt @ "\";");
            %fo.writeLine("$LiFxGM::passwordHash = \"" @ $LiFxGM::passwordHash @ "\";");
            %fo.close();
        }
        %fo.delete();
        return 1;
    }

    function LiFxGMCommands::activateGM(%client) {
        %p = LiFxGMCommands::getPlayer(%client);
        if (isObject(%p) && %p.isMethod("setGM")) {
            %p.setGM(1);
        }
    }

    function LiFxGMCommands::deactivateGM(%client) {
        %p = LiFxGMCommands::getPlayer(%client);
        if (isObject(%p) && %p.isMethod("setGM")) {
            %p.setGM(0);
        }
    }

    function LiFxGMCommands::getPlayer(%client) {
        %p = %client.player;
        if (!isObject(%p)) {
            %p = %client.getControlObject();
        }
        return %p;
    }

    function LiFxGMCommands::clean(%s) {
        return stripChars(%s, "'\"\\;%`");
    }

    function LiFxGMCommands::findOnline(%charId) {
        %n = ClientGroup.getCount();
        for (%i = 0; %i < %n; %i++) {
            %c = ClientGroup.getObject(%i);
            if (%c.getCharacterId() == %charId) {
                return %c;
            }
        }
        return 0;
    }

    function LiFxGMCommands::isKnown(%cmd) {
        return (%cmd $= "help" || %cmd $= "whoami" || %cmd $= "players" || %cmd $= "give" || %cmd $= "giveto" || %cmd $= "delete"
             || %cmd $= "announce" || %cmd $= "kick" || %cmd $= "ban" || %cmd $= "unban"
             || %cmd $= "pos" || %cmd $= "tp" || %cmd $= "bring" || %cmd $= "tppos" || %cmd $= "spawn" || %cmd $= "build" || %cmd $= "geo" || %cmd $= "place" || %cmd $= "login" || %cmd $= "setpass" || %cmd $= "logout" || %cmd $= "ground" || %cmd $= "raise" || %cmd $= "lower"
             || %cmd $= "heal" || %cmd $= "god" || %cmd $= "time" || %cmd $= "weather" || %cmd $= "adminland" || %cmd $= "gmlog" || %cmd $= "inflation");
    }

    function LiFxGMCommands::giveItem(%caller, %targetClient, %type, %qty, %quality, %dur) {
        if (%type $= "" || %type < 1) {
            LiFxGMCommands::reply(%caller, "Item type id missing.");
            return;
        }
        if (%qty $= "" || %qty < 1) { %qty = 1; }
        if (%quality $= "") { %quality = 100; }
        if (%dur $= "") { %dur = 0; }
        %p = LiFxGMCommands::getPlayer(%targetClient);
        if (!isObject(%p)) {
            LiFxGMCommands::reply(%caller, "No player object found.");
            return;
        }
        %ok = %p.inventoryAddItem(%type, %qty, %quality, %dur, %dur);
        echo("[GM] account" SPC %caller.getAccountId() SPC "give type" SPC %type SPC "qty" SPC %qty SPC "quality" SPC %quality SPC "durability" SPC %dur SPC "to character" SPC %targetClient.getCharacterId() SPC "->" SPC %ok);
        LiFxGMCommands::reply(%caller, "give" SPC %type SPC "x" @ %qty SPC "quality" SPC %quality SPC "to character" SPC %targetClient.getCharacterId() SPC ":" SPC (%ok ? "done" : "failed (no space or unknown item type?)"));
    }

    // !delete <type> [count]: removes items of that type from the caller's inventory (backpack, nested bags, equipped). Uses the engine's Player::inventoryRemoveItem(itemId).
    function LiFxGMCommands::deleteItems(%client, %type, %count) {
        %type = LiFxGMCommands::clean(trim(%type));
        if (%type $= "" || stripChars(%type, "0123456789") !$= "") {
            LiFxGMCommands::reply(%client, "Usage: !delete <item type id> [count]");
            return;
        }
        if (%count $= "" || %count < 1) { %count = 200; }
        %char = %client.getCharacterId();
        %req = new ScriptObject() {
            class = "LiFxGMDel";
            caller = %client;
            type = %type;
        };
        %roots = "SELECT RootContainerID FROM `character` WHERE ID=" @ %char @ " UNION SELECT EquipmentContainerID FROM `character` WHERE ID=" @ %char;
        dbi.select(%req, "onItems", "SELECT i.ID AS ItemID FROM `items` i WHERE i.ObjectTypeID=" @ %type @ " AND (i.ContainerID IN (" @ %roots @ ") OR i.ContainerID IN (SELECT ct.ID FROM `containers` ct WHERE ct.ParentID IN (SELECT i2.ID FROM `items` i2 WHERE i2.ContainerID IN (" @ %roots @ ")))) LIMIT " @ %count);
    }

    function LiFxGMDel::onItems(%this, %rs) {
        %client = %this.caller;
        %p = LiFxGMCommands::getPlayer(%client);
        %n = 0; %failed = 0;
        if (%rs.Ok() && isObject(%p)) {
            while (%rs.nextRecord()) {
                %id = %rs.getFieldValue("ItemID");
                if (%p.inventoryRemoveItem(%id)) { %n++; } else { %failed++; }
            }
        }
        echo("[GM] account" SPC %client.getAccountId() SPC "delete type" SPC %this.type SPC "removed" SPC %n SPC "failed" SPC %failed);
        LiFxGMCommands::reply(%client, "delete type" SPC %this.type @ ":" SPC %n SPC "removed" @ (%failed ? ", " @ %failed SPC "failed" : "") @ ((%n == 0 && %failed == 0) ? " (no such item in your inventory)" : ""));
        dbi.remove(%rs);
        %this.delete();
    }

    // Moves a player object to a world position "x y z" (plain Torque setTransform, keeps the server object; z is raised a little).
    function LiFxGMCommands::teleport(%caller, %movedClient, %pos) {
        %p = LiFxGMCommands::getPlayer(%movedClient);
        if (!isObject(%p)) {
            LiFxGMCommands::reply(%caller, "No player object found to move.");
            return 0;
        }
        %x = getWord(%pos, 0);
        %y = getWord(%pos, 1);
        %z = getWord(%pos, 2) + 1;
        %p.setTransform(%x SPC %y SPC %z SPC "0 0 1 0");
        echo("[GM] account" SPC %caller.getAccountId() SPC "teleport character" SPC %movedClient.getCharacterId() SPC "to" SPC %x SPC %y SPC %z);
        return 1;
    }

    // Map cell (GeoID) one tile in front of the GM; 0 when it cannot be worked out.
    function LiFxGMCommands::frontGeo(%client, %tiles) {
        if (%tiles $= "") {
            %tiles = $LiFxGM::spawnOffsetTiles;
        }
        %p = LiFxGMCommands::getPlayer(%client);
        if (!isObject(%p)) {
            return 0;
        }
        %pos = %p.getPosition();
        %tx = getWord(%pos, 0);
        %ty = getWord(%pos, 1);
        if (%tiles != 0 && %p.isMethod("getForwardVector")) {
            %fv = %p.getForwardVector();
            %fx = getWord(%fv, 0);
            %fy = getWord(%fv, 1);
            %len = mSqrt(%fx * %fx + %fy * %fy);
            if (%len > 0.001) {
                %tx += (%fx / %len) * 4 * %tiles;
                %ty += (%fy / %len) * 4 * %tiles;
            }
        }
        return Lifx::worldToGeoId(%tx, %ty);
    }

    // God mode via ShapeBase::setInvincibleMode(time, speed): "on" = for %seconds (default an hour), "off" = 0.
    function LiFxGMCommands::setGod(%client, %mode, %seconds) {
        %p = LiFxGMCommands::getPlayer(%client);
        if (!isObject(%p) || !%p.isMethod("setInvincibleMode")) {
            return 0;
        }
        if (%mode $= "off") {
            %p.setInvincibleMode(0, 0);
        } else {
            if (%seconds $= "" || %seconds < 1) { %seconds = 3600; }
            %p.setInvincibleMode(%seconds, 0);
        }
        return 1;
    }

    // Looks a character up by id or name (asynchronous), then runs the action in LiFxGMReq::onTarget.
    function LiFxGMCommands::withTarget(%caller, %action, %who, %args) {
        %who = LiFxGMCommands::clean(trim(%who));
        if (%who $= "") {
            LiFxGMCommands::reply(%caller, "Give a character id or name.");
            return;
        }
        %req = new ScriptObject() {
            class = "LiFxGMReq";
            caller = %caller;
            action = %action;
            args = %args;
        };
        if (stripChars(%who, "0123456789") $= "") {
            %where = "c.ID=" @ %who;
        } else {
            %where = "c.Name='" @ %who @ "' OR CONCAT(c.Name,' ',c.LastName)='" @ %who @ "'";
        }
        dbi.select(%req, "onTarget", "SELECT c.ID AS CharID, c.AccountID AS AccountID, c.Name AS Name, c.LastName AS LastName FROM `character` c WHERE " @ %where @ " LIMIT 1");
    }

    function LiFxGMReq::onTarget(%this, %rs) {
        %caller = %this.caller;
        if (%rs.Ok() && %rs.nextRecord()) {
            %charId = %rs.getFieldValue("CharID");
            %accId = %rs.getFieldValue("AccountID");
            %name = %rs.getFieldValue("Name") SPC %rs.getFieldValue("LastName");
            %tgt = LiFxGMCommands::findOnline(%charId);
            %action = %this.action;
            echo("[GM] account" SPC %caller.getAccountId() SPC %action SPC "target character" SPC %charId SPC "(" @ %name @ ") account" SPC %accId SPC "online" SPC (%tgt ? "yes" : "no"));
            if (%action $= "giveto") {
                if (%tgt) {
                    LiFxGMCommands::giveItem(%caller, %tgt, getWord(%this.args, 0), getWord(%this.args, 1), getWord(%this.args, 2), getWord(%this.args, 3));
                } else {
                    LiFxGMCommands::reply(%caller, %name SPC "is not online.");
                }
            } else if (%action $= "kick") {
                if (%tgt) {
                    %tgt.cmSendClientMessage(2475, "<color:ffd060>You were removed from the server by a GM.");
                    %tgt.delete();
                    LiFxGMCommands::reply(%caller, "Kicked" SPC %name SPC "(character" SPC %charId @ ").");
                } else {
                    LiFxGMCommands::reply(%caller, %name SPC "is not online.");
                }
            } else if (%action $= "heal") {
                if (%tgt) {
                    Lifx::healToFull(%charId);
                    LiFxGMCommands::reply(%caller, "Healed" SPC %name @ ".");
                    %tgt.cmSendClientMessage(2475, "<color:ffd060>A GM healed you.");
                } else {
                    LiFxGMCommands::reply(%caller, %name SPC "is not online.");
                }
            } else if (%action $= "god") {
                if (%tgt) {
                    LiFxGMCommands::setGod(%tgt, getWord(%this.args, 0), getWord(%this.args, 1));
                    LiFxGMCommands::reply(%caller, "God mode" SPC getWord(%this.args, 0) SPC "for" SPC %name @ ".");
                } else {
                    LiFxGMCommands::reply(%caller, %name SPC "is not online.");
                }
            } else if (%action $= "tp" || %action $= "bring") {
                if (!%tgt) {
                    LiFxGMCommands::reply(%caller, %name SPC "is not online.");
                } else {
                    %tp = LiFxGMCommands::getPlayer(%tgt);
                    %cp = LiFxGMCommands::getPlayer(%caller);
                    if (!isObject(%tp) || !isObject(%cp)) {
                        LiFxGMCommands::reply(%caller, "Player object missing.");
                    } else if (%action $= "tp") {
                        if (LiFxGMCommands::teleport(%caller, %caller, %tp.getPosition())) {
                            LiFxGMCommands::reply(%caller, "Moved to" SPC %name @ ".");
                        }
                    } else {
                        if (LiFxGMCommands::teleport(%caller, %tgt, %cp.getPosition())) {
                            LiFxGMCommands::reply(%caller, "Brought" SPC %name SPC "to you.");
                        }
                    }
                }
            } else if (%action $= "ban" || %action $= "unban") {
                if (%action $= "ban" && (%accId == %caller.getAccountId() || %tgt.isGM())) {
                    LiFxGMCommands::reply(%caller, "Refused: that account is yours or a GM account.");
                } else {
                    %flag = (%action $= "ban") ? 0 : 1;
                    dbi.Update("UPDATE `account` SET IsActive=" @ %flag @ " WHERE ID=" @ %accId);
                    if (%action $= "ban" && %tgt) {
                        %tgt.cmSendClientMessage(2475, "<color:ffd060>You were banned from the server.");
                        %tgt.delete();
                    }
                    LiFxGMCommands::reply(%caller, (%action $= "ban" ? "Banned" : "Unbanned") SPC %name SPC "(account" SPC %accId @ ").");
                }
            }
        } else {
            LiFxGMCommands::reply(%caller, "No character found with that id or name.");
        }
        dbi.remove(%rs);
        %rs.delete();
        %this.delete();
    }

    // !place: looks the object type up, then creates it as a movable object or as a building, whichever the type is.
    function LiFxGMReq::onPlace(%this, %rs) {
        %caller = %this.caller;
        if (%rs.Ok() && %rs.nextRecord()) {
            %type = %this.type;
            %name = %rs.getFieldValue("Name");
            %mov = %rs.getFieldValue("Mov");
            %unm = %rs.getFieldValue("Unm");
            %geo = %this.geo;
            %char = %caller.getCharacterId();
            %id = 0;
            if (%mov == 1) {
                %id = CreateTestMovable(%type, %geo, %char, 1);
                %kind = "movable object";
            } else if (%unm == 1) {
                %id = CreateTestUnmovable(%type, %geo, %char, 1);
                %kind = "building";
            } else {
                %kind = "not a placeable object";
            }
            echo("[GM] account" SPC %caller.getAccountId() SPC "place type" SPC %type SPC "(" @ %name @ ") as" SPC %kind SPC "at geoId" SPC %geo SPC "-> object id" SPC %id);
            LiFxGMCommands::reply(%caller, "place" SPC %type SPC "(" @ %name @ ", " @ %kind @ ") :" SPC ((%id > 0) ? "created, object id" SPC %id : "FAILED (blocked position or not placeable)"));
        } else {
            LiFxGMCommands::reply(%caller, "Unknown object type.");
        }
        dbi.remove(%rs);
        %rs.delete();
        %this.delete();
    }

    // ---- Inflation helper ----------------------------------------------------------------------------------------------------
    // Coin supply of the whole server (items table: copper 1059, silver 1060, gold 1061; values 1 : 100 : 10000 like their base prices)
    // divided by the number of non-GM active characters, compared with a baseline (copper per player). The baseline is stored in
    // lifx_gm_settings: set it with "!inflation baseline <n>", or it is taken automatically from the first snapshot that has coins.
    function LiFxGMCommands::inflationInner() {
        %excl = "0";
        for (%i = 0; %i < getWordCount($LiFxGM::accounts); %i++) {
            %a = getWord($LiFxGM::accounts, %i);
            if (%a !$= "" && stripChars(%a, "0123456789") $= "") {
                %excl = %excl @ "," @ %a;
            }
        }
        return "SELECT (SELECT COALESCE(SUM(Quantity),0) FROM `items` WHERE ObjectTypeID=1059) AS cu, (SELECT COALESCE(SUM(Quantity),0) FROM `items` WHERE ObjectTypeID=1060) AS si, (SELECT COALESCE(SUM(Quantity),0) FROM `items` WHERE ObjectTypeID=1061) AS au, GREATEST(1, (SELECT COUNT(*) FROM `character` c JOIN `account` a ON a.ID=c.AccountID WHERE c.IsActive>0 AND a.IsGM=0 AND a.ID NOT IN (" @ %excl @ "))) AS pl";
    }

    // Stores the automatic baseline (only the first time there are coins) and today's snapshot.
    function LiFxGMCommands::inflationSnapshot() {
        %inner = LiFxGMCommands::inflationInner();
        dbi.Update("INSERT IGNORE INTO `lifx_gm_settings` (`Name`, `Value`) SELECT 'coinBaseline', ROUND((t.cu + t.si*100 + t.au*10000)/t.pl, 2) FROM (" @ %inner @ ") t WHERE t.cu + t.si*100 + t.au*10000 > 0");
        dbi.Update("INSERT INTO `lifx_inflation_log` (`SnapshotDate`, `Copper`, `Silver`, `Gold`, `CoinValue`, `Players`, `PerPlayer`, `Baseline`, `InflationPct`) SELECT CURDATE(), t.cu, t.si, t.au, t.cu + t.si*100 + t.au*10000, t.pl, ROUND((t.cu + t.si*100 + t.au*10000)/t.pl, 2), b.v, IF(b.v > 0, ROUND(((t.cu + t.si*100 + t.au*10000)/t.pl/b.v - 1)*100, 2), 0) FROM (" @ %inner @ ") t JOIN (SELECT COALESCE(MAX(`Value`),0) AS v FROM `lifx_gm_settings` WHERE `Name`='coinBaseline') b ON 1=1 ON DUPLICATE KEY UPDATE `Copper`=VALUES(`Copper`), `Silver`=VALUES(`Silver`), `Gold`=VALUES(`Gold`), `CoinValue`=VALUES(`CoinValue`), `Players`=VALUES(`Players`), `PerPlayer`=VALUES(`PerPlayer`), `Baseline`=VALUES(`Baseline`), `InflationPct`=VALUES(`InflationPct`)");
    }

    // Writes the whole snapshot history as a small JavaScript data file that the economy dashboard page loads (mods/LiFx/GMCommands/economy).
    function LiFxGMCommands::inflationExport(%caller) {
        LiFxGMCommands::inflationSnapshot();
        %req = new ScriptObject() {
            class = "LiFxGMReq";
            caller = %caller;
        };
        dbi.select(%req, "onInflExport", "SELECT SnapshotDate, Copper, Silver, Gold, CoinValue, Players, PerPlayer, InflationPct, Baseline FROM `lifx_inflation_log` ORDER BY SnapshotDate ASC");
    }

    function LiFxGMReq::onInflExport(%this, %rs) {
        %n = 0;
        %fo = new FileObject();
        if (%fo.openForWrite("mods/LiFx/GMCommands/economy/economy_data.js")) {
            %fo.writeLine("window.LIFX_ECONOMY_ROWS = [");
            if (%rs.Ok()) {
                while (%rs.nextRecord()) {
                    %fo.writeLine("\"" @ %rs.getFieldValue("SnapshotDate") @ "," @ %rs.getFieldValue("Copper") @ "," @ %rs.getFieldValue("Silver") @ "," @ %rs.getFieldValue("Gold") @ "," @ %rs.getFieldValue("CoinValue") @ "," @ %rs.getFieldValue("Players") @ "," @ %rs.getFieldValue("PerPlayer") @ "," @ %rs.getFieldValue("InflationPct") @ "," @ %rs.getFieldValue("Baseline") @ "\",");
                    %n++;
                }
            }
            %fo.writeLine("];");
            %fo.close();
            LiFxGMCommands::reply(%this.caller, "Economy dashboard data written (" @ %n @ " snapshot(s)).");
        } else {
            LiFxGMCommands::reply(%this.caller, "Could not write the economy dashboard data file.");
        }
        %fo.delete();
        dbi.remove(%rs);
        %rs.delete();
        %this.delete();
    }

    function LiFxGMCommands::inflationNow(%caller) {
        LiFxGMCommands::inflationSnapshot();
        %inner = LiFxGMCommands::inflationInner();
        %req = new ScriptObject() {
            class = "LiFxGMReq";
            caller = %caller;
        };
        dbi.select(%req, "onInflation", "SELECT t.cu AS Copper, t.si AS Silver, t.au AS Gold, (t.cu + t.si*100 + t.au*10000) AS CoinValue, t.pl AS Players, ROUND((t.cu + t.si*100 + t.au*10000)/t.pl, 2) AS PerPlayer, b.v AS Baseline FROM (" @ %inner @ ") t JOIN (SELECT COALESCE(MAX(`Value`),0) AS v FROM `lifx_gm_settings` WHERE `Name`='coinBaseline') b ON 1=1");
    }

    function LiFxGMReq::onInflation(%this, %rs) {
        if (%rs.Ok() && %rs.nextRecord()) {
            %cu = %rs.getFieldValue("Copper");
            %si = %rs.getFieldValue("Silver");
            %au = %rs.getFieldValue("Gold");
            %val = %rs.getFieldValue("CoinValue");
            %pl = %rs.getFieldValue("Players");
            %per = %rs.getFieldValue("PerPlayer");
            %base = %rs.getFieldValue("Baseline");
            %text = "Inflation helper (coins in the world):\nCopper" SPC %cu @ ", Silver" SPC %si @ ", Gold" SPC %au @ "  =  value" SPC %val SPC "copper";
            %text = %text @ "\nNon-GM active characters:" SPC %pl @ "   value per player:" SPC %per SPC "copper";
            if (%base > 0) {
                %pct = ((%per / %base) - 1) * 100;
                %text = %text @ "\nBaseline:" SPC %base SPC "copper per player   ->   inflation" SPC mFloatLength(%pct, 1) @ " %";
            } else {
                %text = %text @ "\nNo baseline yet (there are no coins). It is set automatically from the first snapshot with coins, or use: !inflation baseline <copper per player>";
            }
            LiFxGMCommands::reply(%this.caller, %text);
        } else {
            LiFxGMCommands::reply(%this.caller, "Could not read the coin totals.");
        }
        dbi.remove(%rs);
        %rs.delete();
        %this.delete();
    }

    function LiFxGMReq::onInflHistory(%this, %rs) {
        %text = "Inflation snapshots (newest first):";
        if (%rs.Ok()) {
            while (%rs.nextRecord()) {
                %text = %text @ "\n" @ %rs.getFieldValue("SnapshotDate") SPC "- per player" SPC %rs.getFieldValue("PerPlayer") SPC "copper, players" SPC %rs.getFieldValue("Players") SPC ", inflation" SPC %rs.getFieldValue("InflationPct") SPC "%";
            }
        }
        LiFxGMCommands::reply(%this.caller, %text);
        dbi.remove(%rs);
        %rs.delete();
        %this.delete();
    }

    function LiFxGMReq::onLog(%this, %rs) {
        %text = "Last GM actions:";
        if (%rs.Ok()) {
            while (%rs.nextRecord()) {
                %text = %text @ "\n" @ %rs.getFieldValue("LoggedAt") SPC "acc" SPC %rs.getFieldValue("AccountID") SPC "-" SPC %rs.getFieldValue("Action");
            }
        }
        LiFxGMCommands::reply(%this.caller, %text);
        dbi.remove(%rs);
        %rs.delete();
        %this.delete();
    }

    function LiFxGMReq::onPlayers(%this, %rs) {
        %caller = %this.caller;
        %text = "Online players:";
        if (%rs.Ok()) {
            while (%rs.nextRecord()) {
                %text = %text @ "\n" @ %rs.getFieldValue("CharID") SPC "-" SPC %rs.getFieldValue("Name") SPC %rs.getFieldValue("LastName");
            }
        }
        LiFxGMCommands::reply(%caller, %text);
        dbi.remove(%rs);
        %rs.delete();
        %this.delete();
    }

    // Returns 1 when the message was a GM command (and must not go on to the normal chat).
    function LiFxGMCommands::handle(%client, %message) {
        %message = trim(%message);
        %first = getWord(%message, 0);
        %cmd = strlwr(getSubStr(%first, 1, 32));
        %account = %client.getAccountId();

        if (%cmd $= "whoami") {
            %p = LiFxGMCommands::getPlayer(%client);
            LiFxGMCommands::reply(%client, "account" SPC %account SPC "character" SPC %client.getCharacterId() SPC "isGM" SPC %client.isGM() SPC "player object" SPC %p SPC "allowed" SPC LiFxGMCommands::isAllowed(%client));
            echo("[GM] whoami account" SPC %account SPC "player" SPC %p);
            return 1;
        }

        if (!LiFxGMCommands::hasRight(%client)) {
            return 0;   // not a GM account: let it through as ordinary chat (a "!" is a shout)
        }

        if (%cmd $= "login") {
            %pw = getWords(%message, 1);
            if ($LiFxGM::passwordHash $= "") {
                LiFxGMCommands::reply(%client, "No GM password is set yet. Enter a new password in the login window and press Set password.");
                commandToClient(%client, 'GMLoginResult', 2);
                return 1;
            }
            if ($LiFxGM::lockUntil[%account] > getSimTime()) {
                LiFxGMCommands::reply(%client, "Wrong Login (too many attempts, wait half a minute)");
                commandToClient(%client, 'GMLoginResult', 0);
                return 1;
            }
            if (LiFxGMCommands::hashPassword(%pw) $= $LiFxGM::passwordHash) {
                $LiFxGM::fails[%account] = 0;
                %client.gmLogin = 1;
                LiFxGMCommands::activateGM(%client);
                echo("[GM] account" SPC %account SPC "logged in, GM mode activated");
                LiFxGMCommands::reply(%client, "GM login ok. GM mode is on.");
                commandToClient(%client, 'GMLoginResult', 1);
            } else {
                $LiFxGM::fails[%account] += 1;
                if ($LiFxGM::fails[%account] >= 3) {
                    $LiFxGM::lockUntil[%account] = getSimTime() + 30000;
                    $LiFxGM::fails[%account] = 0;
                }
                echo("[GM] account" SPC %account SPC "wrong GM password");
                LiFxGMCommands::reply(%client, "Wrong Login");
                commandToClient(%client, 'GMLoginResult', 0);
            }
            return 1;
        }

        if (%cmd $= "setpass") {
            %pw = getWords(%message, 1);
            if ($LiFxGM::passwordHash !$= "" && %client.gmLogin != 1) {
                LiFxGMCommands::reply(%client, "Log in with the current password first.");
                commandToClient(%client, 'GMLoginResult', 0);
                return 1;
            }
            if (strlen(%pw) < 6) {
                LiFxGMCommands::reply(%client, "The password must be at least 6 characters.");
                commandToClient(%client, 'GMLoginResult', 3);
                return 1;
            }
            if (!LiFxGMCommands::savePassword(%pw)) {
                LiFxGMCommands::reply(%client, "Could not save the password (hashing unavailable on the server).");
                commandToClient(%client, 'GMLoginResult', 0);
                return 1;
            }
            %client.gmLogin = 1;
            LiFxGMCommands::activateGM(%client);
            echo("[GM] account" SPC %account SPC "set a new GM password");
            LiFxGMCommands::reply(%client, "GM password saved. You are logged in, GM mode is on.");
            commandToClient(%client, 'GMLoginResult', 1);
            return 1;
        }

        if (%cmd $= "logout") {
            %client.gmLogin = 0;
            LiFxGMCommands::deactivateGM(%client);
            echo("[GM] account" SPC %account SPC "logged out, GM mode deactivated");
            LiFxGMCommands::reply(%client, "GM logout done. GM mode is off.");
            commandToClient(%client, 'GMLoginResult', 0);
            return 1;
        }

        if (%client.gmLogin != 1) {
            LiFxGMCommands::reply(%client, "GM login required: press F6 and enter the GM password.");
            commandToClient(%client, 'GMLoginResult', 0);
            return 1;
        }

        if (%cmd $= "help") {
            LiFxGMCommands::reply(%client, "GM commands:\n!whoami  !players\n!give <type> [qty] [quality] [durability]\n!giveto <char id or name> <type> [qty] [quality] [durability]\n!delete <type> [count]  (removes items of that type from your inventory)\n!announce <text>\n!place <object type>  (movable or building, automatic)\n!spawn <movable type>  !build <building type>  !geo\n!pos  !tppos <x> <y> <z>\n!tp <char id or name>  !bring <char id or name>\n!kick <char id or name>\n!ban <char id or name>  !unban <char id or name>\n!ground <rock|riverrock|swamp>  !raise <name>  !lower\n!heal [who]  !god on|off [seconds] [who]\n!inflation | !inflation history [n] | !inflation baseline <n> | !inflation export   (coin supply per player)\n!adminland create <name> <len> <wid> [prio] | preview <len> <wid> | delete <name>  !gmlog [n]\n!login <password>  !setpass <new password>  !logout");
            return 1;
        }

        if (%cmd $= "players") {
            %ids = "";
            %n = ClientGroup.getCount();
            for (%i = 0; %i < %n; %i++) {
                %ids = %ids @ (%i ? "," : "") @ ClientGroup.getObject(%i).getCharacterId();
            }
            if (%ids $= "") {
                LiFxGMCommands::reply(%client, "Nobody online.");
                return 1;
            }
            %req = new ScriptObject() {
                class = "LiFxGMReq";
                caller = %client;
            };
            dbi.select(%req, "onPlayers", "SELECT c.ID AS CharID, c.Name AS Name, c.LastName AS LastName FROM `character` c WHERE c.ID IN (" @ %ids @ ")");
            return 1;
        }

        if (%cmd $= "pos") {
            %p = LiFxGMCommands::getPlayer(%client);
            LiFxGMCommands::reply(%client, "position" SPC (isObject(%p) ? %p.getPosition() : "unknown"));
            return 1;
        }

        if (%cmd $= "tppos") {
            %pos = getWords(%message, 1, 3);
            if (getWordCount(%pos) < 3) {
                LiFxGMCommands::reply(%client, "Usage: !tppos <x> <y> <z>");
                return 1;
            }
            if (LiFxGMCommands::teleport(%client, %client, %pos)) {
                LiFxGMCommands::reply(%client, "Moved to" SPC %pos @ ".");
            }
            return 1;
        }

        if (%cmd $= "tp" || %cmd $= "bring") {
            LiFxGMCommands::withTarget(%client, %cmd, getWords(%message, 1), "");
            return 1;
        }

        if (%cmd $= "geo") {
            %p = LiFxGMCommands::getPlayer(%client);
            if (!isObject(%p)) {
                LiFxGMCommands::reply(%client, "No player object found.");
                return 1;
            }
            %pos = %p.getPosition();
            %g = Lifx::worldToGeoId(getWord(%pos, 0), getWord(%pos, 1));
            LiFxGMCommands::reply(%client, "geoId" SPC %g SPC "(terrain block" SPC (%g >> 18) @ ", cell" SPC (%g & 511) SPC ((%g >> 9) & 511) @ ")");
            return 1;
        }

        if (%cmd $= "ground" || %cmd $= "raise" || %cmd $= "lower") {
            if (!isFunction("TestRaiseTerrain") || !isFunction("TestLowerTerrain")) {
                LiFxGMCommands::reply(%client, "Terrain functions are not available on this server build.");
                return 1;
            }
            %geo = LiFxGMCommands::frontGeo(%client, 0);   // the cell you are standing on
            if (%geo == 0) {
                LiFxGMCommands::reply(%client, "Could not work out the map cell here.");
                return 1;
            }
            %char = %client.getCharacterId();
            %name = strlwr(getWord(%message, 1));
            %sub = $LiFxGM::substance[%name];
            if (%cmd $= "lower") {
                TestLowerTerrain(%geo, %char);
                echo("[GM] account" SPC %account SPC "lower terrain at geoId" SPC %geo);
                LiFxGMCommands::reply(%client, "Lowered the ground by one layer at geoId" SPC %geo @ ".");
                return 1;
            }
            if (%sub $= "" || %sub == 0) {
                LiFxGMCommands::reply(%client, "Usage: !" @ %cmd SPC "<rock|riverrock|swamp>");
                return 1;
            }
            if (%cmd $= "ground") {
                TestLowerTerrain(%geo, %char);   // take the top layer off, then put the wanted material back on: same height, new surface
            }
            %q = Lifx::setTerrainQuality($LiFxGM::groundQuality);
            echo("[GM] terrain layer quality set to" SPC %q SPC "(wanted" SPC $LiFxGM::groundQuality @ ", -1 = not available)");
            TestRaiseTerrain(%geo, %char, %sub);
            echo("[GM] account" SPC %account SPC %cmd SPC %name SPC "(substance" SPC %sub @ ") at geoId" SPC %geo);
            LiFxGMCommands::reply(%client, "Ground at geoId" SPC %geo SPC (%cmd $= "ground" ? "changed to" : "raised with") SPC %name @ ".");
            return 1;
        }
        if (%cmd $= "heal") {
            %who = trim(getWords(%message, 1));
            if (%who $= "") {
                Lifx::healToFull(%client.getCharacterId());
                LiFxGMCommands::reply(%client, "Healed you.");
            } else {
                LiFxGMCommands::withTarget(%client, "heal", %who, "");
            }
            return 1;
        }

        if (%cmd $= "god") {
            LiFxGMCommands::reply(%client, "Use the GM panel (F6): Invulnerable ON / OFF. It uses the game's own /INVUL command, which really blocks damage.");
            return 1;
        }

        if (%cmd $= "god_old_unused") {
            %mode = strlwr(getWord(%message, 1));
            if (%mode !$= "on" && %mode !$= "off") {
                LiFxGMCommands::reply(%client, "Usage: !god on|off [seconds]   (yourself)   or   !god on|off <seconds> <char id or name>");
                return 1;
            }
            %sec = getWord(%message, 2);
            %who = trim(getWords(%message, 3));
            if (%who $= "") {
                if (LiFxGMCommands::setGod(%client, %mode, %sec)) {
                    LiFxGMCommands::reply(%client, "God mode" SPC %mode @ ".");
                } else {
                    LiFxGMCommands::reply(%client, "God mode is not available (no player object).");
                }
            } else {
                LiFxGMCommands::withTarget(%client, "god", %who, %mode SPC %sec);
            }
            return 1;
        }

        if (%cmd $= "time") {
            LiFxGMCommands::reply(%client, "The time of day cannot be changed: the game has no command for it and no world clock object on the server.");
            return 1;
        }

        if (%cmd $= "weather") {
            LiFxGMCommands::reply(%client, "Use the game's own command: /WEATHER Fair|Cloudy|Shower|Snowy (or the weather buttons in the GM panel).");
            return 1;
        }
        if (%cmd $= "adminland") {
            %sub = strlwr(getWord(%message, 1));
            %char = %client.getCharacterId();
            if (%sub $= "delete") {
                %name = getWord(%message, 2);
                if (%name $= "" || !isFunction("TestAdminLandDelete")) {
                    LiFxGMCommands::reply(%client, "Usage: !adminland delete <name>");
                    return 1;
                }
                TestAdminLandDelete(%char, %name);
                echo("[GM] account" SPC %account SPC "adminland delete" SPC %name);
                LiFxGMCommands::reply(%client, "Adminland" SPC %name SPC "delete sent.");
                return 1;
            }
            if (%sub $= "create" || %sub $= "preview") {
                if (%sub $= "create") {
                    %name = getWord(%message, 2);
                    %len = getWord(%message, 3);
                    %wid = getWord(%message, 4);
                    %prio = getWord(%message, 5);
                } else {
                    %name = "preview";
                    %len = getWord(%message, 2);
                    %wid = getWord(%message, 3);
                    %prio = 10;
                }
                if (%prio $= "") { %prio = 10; }
                if (%name $= "" || %len < 1 || %wid < 1 || %len > 200 || %wid > 200) {
                    LiFxGMCommands::reply(%client, "Usage: !adminland create <name> <length 1-200> <width 1-200> [priority]   |   !adminland preview <length> <width>   |   !adminland delete <name>");
                    return 1;
                }
                %p = LiFxGMCommands::getPlayer(%client);
                if (!isObject(%p)) {
                    LiFxGMCommands::reply(%client, "No player object found.");
                    return 1;
                }
                %pos = %p.getPosition();
                %x = getWord(%pos, 0);
                %y = getWord(%pos, 1);
                %g1 = Lifx::worldToGeoId(%x, %y);
                %g2 = Lifx::worldToGeoId(%x + (%len - 1) * 4, %y + (%wid - 1) * 4);
                if (%g1 == 0 || %g2 == 0) {
                    LiFxGMCommands::reply(%client, "Could not work out the corner cells.");
                    return 1;
                }
                if (%sub $= "preview") {
                    LiFxGMCommands::reply(%client, "Adminland" SPC %len @ "x" @ %wid SPC "tiles: corner 1 geoId" SPC %g1 SPC "(you), corner 2 geoId" SPC %g2 SPC "(east/north of you).");
                    return 1;
                }
                if (!isFunction("TestAdminLandCreate")) {
                    LiFxGMCommands::reply(%client, "TestAdminLandCreate is not available on this server build.");
                    return 1;
                }
                TestAdminLandCreate(%char, %g1, %g2, %name, %prio);
                echo("[GM] account" SPC %account SPC "adminland create" SPC %name SPC %len @ "x" @ %wid SPC "geoIds" SPC %g1 SPC %g2 SPC "priority" SPC %prio);
                LiFxGMCommands::reply(%client, "Adminland" SPC %name SPC "requested:" SPC %len @ "x" @ %wid SPC "tiles from your position towards east/north.");
                return 1;
            }
            LiFxGMCommands::reply(%client, "Usage: !adminland create <name> <length> <width> [priority]  |  preview <length> <width>  |  delete <name>");
            return 1;
        }

        if (%cmd $= "inflation") {
            %sub = strlwr(getWord(%message, 1));
            if (%sub $= "baseline") {
                %b = getWord(%message, 2);
                if (%b $= "" || stripChars(%b, "0123456789.") !$= "" || %b <= 0) {
                    LiFxGMCommands::reply(%client, "Usage: !inflation baseline <copper per player, e.g. 5000>");
                    return 1;
                }
                dbi.Update("REPLACE INTO `lifx_gm_settings` (`Name`, `Value`) VALUES ('coinBaseline', " @ %b @ ")");
                LiFxGMCommands::reply(%client, "Inflation baseline set to" SPC %b SPC "copper per player. Run !inflation to see the new figures.");
                return 1;
            }
            if (%sub $= "export") {
                LiFxGMCommands::inflationExport(%client);
                return 1;
            }
            if (%sub $= "history") {
                %n = getWord(%message, 2);
                if (%n $= "" || %n < 1 || %n > 60) { %n = 10; }
                %req = new ScriptObject() {
                    class = "LiFxGMReq";
                    caller = %client;
                };
                dbi.select(%req, "onInflHistory", "SELECT SnapshotDate, PerPlayer, Players, InflationPct FROM `lifx_inflation_log` ORDER BY SnapshotDate DESC LIMIT " @ %n);
                return 1;
            }
            LiFxGMCommands::inflationNow(%client);
            return 1;
        }

        if (%cmd $= "gmlog") {
            %n = getWord(%message, 1);
            if (%n $= "" || %n < 1 || %n > 30) { %n = 10; }
            %req = new ScriptObject() {
                class = "LiFxGMReq";
                caller = %client;
            };
            dbi.select(%req, "onLog", "SELECT LoggedAt, AccountID, CharID, Action FROM `lifx_gm_log` ORDER BY ID DESC LIMIT " @ %n);
            return 1;
        }
        if (%cmd $= "place") {
            %type = getWord(%message, 1);
            if (%type $= "" || stripChars(%type, "0123456789") !$= "") {
                LiFxGMCommands::reply(%client, "Usage: !place <object type id>   (works for movable objects and buildings)");
                return 1;
            }
            %geo = LiFxGMCommands::frontGeo(%client);
            if (%geo == 0) {
                LiFxGMCommands::reply(%client, "Could not work out the map cell here.");
                return 1;
            }
            %req = new ScriptObject() {
                class = "LiFxGMReq";
                caller = %client;
                type = %type;
                geo = %geo;
            };
            dbi.select(%req, "onPlace", "SELECT ID, Name, IsMovableObject AS Mov, IsUnmovableobject AS Unm FROM `objects_types` WHERE ID=" @ %type);
            return 1;
        }

        if (%cmd $= "spawn" || %cmd $= "build") {
            %type = getWord(%message, 1);
            if (%type $= "" || %type < 1) {
                LiFxGMCommands::reply(%client, "Usage: !spawn <movable object type id>   (carts etc.)\n       !build <building type id>   (finished building)");
                return 1;
            }
            %p = LiFxGMCommands::getPlayer(%client);
            if (!isObject(%p)) {
                LiFxGMCommands::reply(%client, "No player object found.");
                return 1;
            }
            %pos = %p.getPosition();
            %tx = getWord(%pos, 0);
            %ty = getWord(%pos, 1);
            if ($LiFxGM::spawnOffsetTiles != 0 && %p.isMethod("getForwardVector")) {
                %fv = %p.getForwardVector();
                %fx = getWord(%fv, 0);
                %fy = getWord(%fv, 1);
                %len = mSqrt(%fx * %fx + %fy * %fy);
                if (%len > 0.001) {
                    %tx += (%fx / %len) * 4 * $LiFxGM::spawnOffsetTiles;
                    %ty += (%fy / %len) * 4 * $LiFxGM::spawnOffsetTiles;
                }
            }
            %geo = Lifx::worldToGeoId(%tx, %ty);
            if (%geo == 0) {
                LiFxGMCommands::reply(%client, "Could not work out the map cell here.");
                return 1;
            }
            %char = %client.getCharacterId();
            if (%cmd $= "spawn") {
                %id = CreateTestMovable(%type, %geo, %char, 1);
            } else {
                %id = CreateTestUnmovable(%type, %geo, %char, 1);
            }
            echo("[GM] account" SPC %account SPC %cmd SPC "type" SPC %type SPC "at geoId" SPC %geo SPC "(pos" SPC %pos @ ", target" SPC %tx SPC %ty @ ") -> object id" SPC %id);
            LiFxGMCommands::reply(%client, %cmd SPC "type" SPC %type SPC "at geoId" SPC %geo SPC ":" SPC ((%id > 0) ? "created, object id" SPC %id : "FAILED (wrong type for this command, or blocked position?)"));
            return 1;
        }
        if (%cmd $= "give") {
            LiFxGMCommands::giveItem(%client, %client, getWord(%message, 1), getWord(%message, 2), getWord(%message, 3), getWord(%message, 4));
            return 1;
        }

        if (%cmd $= "delete") {
            LiFxGMCommands::deleteItems(%client, getWord(%message, 1), getWord(%message, 2));
            return 1;
        }

        if (%cmd $= "giveto") {
            LiFxGMCommands::withTarget(%client, "giveto", getWord(%message, 1), getWords(%message, 2));
            return 1;
        }

        if (%cmd $= "kick" || %cmd $= "ban" || %cmd $= "unban") {
            LiFxGMCommands::withTarget(%client, %cmd, getWords(%message, 1), "");
            return 1;
        }

        if (%cmd $= "announce") {
            %text = trim(getWords(%message, 1));
            if (%text $= "") {
                LiFxGMCommands::reply(%client, "Usage: !announce <text>");
                return 1;
            }
            %n = ClientGroup.getCount();
            for (%i = 0; %i < %n; %i++) {
                %c = ClientGroup.getObject(%i);
                %c.cmSendClientMessage(2475, "<color:ffd060>[Announcement] " @ %text);
                commandToClient(%c, 'LocalChatMessage', "Server", "0.000 0.000 0.000", "[Announcement] " @ %text);
            }
            echo("[GM] account" SPC %account SPC "announce to" SPC %n SPC "clients:" SPC %text);
            return 1;
        }

        return 0;
    }

    // The client chat sends (position, message); the console helper gm("...") sends only the message (first argument).
    function serverCmdLocalChatMessage(%client, %a, %b, %c) {
        %m = (%c !$= "") ? %c : ((%b $= "") ? %a : %b);
        %m = trim(%m);
        if (getSubStr(%m, 0, 1) $= "!" && LiFxGMCommands::isKnown(strlwr(getSubStr(getWord(%m, 0), 1, 32)))) {
            if (LiFxGMCommands::handle(%client, %m)) {
                %w = strlwr(getWord(%m, 0));
                if (%client.gmLogin == 1 && %w !$= "!login" && %w !$= "!setpass") {
                    LiFxGMCommands::logAction(%client, %m);
                }
                return;
            }
        }
        // the engine handler takes the client and the message text only (position is taken from the player on the server)
        Parent::serverCmdLocalChatMessage(%client, %m);
    }
};

// The framework's script SHA-256 lives in a package that nothing activates; without this LiFxSHA256::hash() returns an empty string.
if (isPackage(LiFxSHA256)) {
    activatePackage(LiFxSHA256);
}

activatePackage(LiFxGMCommands);

// One-time password bootstrap: if gm_setpass.txt holds a password (6+ characters), the server hashes and saves it itself, then blanks the file.
// (Lets an admin set the GM password without going through the login window; the file is emptied right after use.)
if (isFile("mods/LiFx/GMCommands/gm_setpass.txt")) {
    %fo = new FileObject();
    %bootPw = "";
    if (%fo.openForRead("mods/LiFx/GMCommands/gm_setpass.txt")) {
        if (!%fo.isEOF()) {
            %bootPw = trim(%fo.readLine());
        }
        %fo.close();
    }
    %fo.delete();
    if (strlen(%bootPw) >= 6) {
        if (LiFxGMCommands::savePassword(%bootPw)) {
            echo("[GM] GM password set from gm_setpass.txt");
        } else {
            echo("[GM] gm_setpass.txt found but the password could not be saved");
        }
        %fw = new FileObject();
        if (%fw.openForWrite("mods/LiFx/GMCommands/gm_setpass.txt")) {
            %fw.writeLine("");
            %fw.close();
        }
        %fw.delete();
    }
}
