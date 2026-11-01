// VIP slots (2026-10-24, Steam IDs since 2026-11-01): when the server is FULL and a VIP connects, one inactive player is
// removed so the VIP can join. "Inactive" = the character has not moved and has not chatted for $LiFxVip::idleMinutes.
// VIPs are registered by STEAM ID in mods/LiFx/VipSlotsPack/config.cs ($LiFxVip::steamIds).
// The engine checks for a free slot in GameConnection::onConnectRequest, before the account (and so the Steam ID) is known.
// So the mod learns the connection address of every registered VIP when they join (the account's Steam ID is read from the
// database) and remembers it in known_ips.cs; a later connection from such an address counts as that VIP.
// Never removed: registered VIPs and registered GM Steam IDs. Log lines start with [VIP].

exec("mods/LiFx/VipSlotsPack/config.cs");
if (isFile("mods/LiFx/VipSlotsPack/known_ips.cs")) {
    exec("mods/LiFx/VipSlotsPack/known_ips.cs");
}

if (!isObject(LiFxVipSlots))
{
    new ScriptObject(LiFxVipSlots)
    {
    };
}

// "IP:1.2.3.4:56789" -> "1.2.3.4"
function LiFxVip_ipOf(%netAddress)
{
    %a = %netAddress;
    if (getSubStr(%a, 0, 3) $= "IP:") {
        %a = getSubStr(%a, 3, 200);
    }
    %last = -1;
    %p = strpos(%a, ":");
    while (%p >= 0) {
        %last = %p;
        %p = strpos(%a, ":", %p + 1);
    }
    if (%last > 0) {
        %a = getSubStr(%a, 0, %last);
    }
    return trim(%a);
}

// "1.2.3.4" -> "1_2_3_4" (safe as an array key in the saved file)
function LiFxVip_key(%ip)
{
    return strreplace(%ip, ".", "_");
}

function LiFxVip_isVipSteam(%steam)
{
    if (%steam $= "") {
        return 0;
    }
    for (%i = 0; %i < getWordCount($LiFxVip::steamIds); %i++) {
        if (getWord($LiFxVip::steamIds, %i) $= %steam) {
            return 1;
        }
    }
    return 0;
}

// the connection address belongs to a VIP that has joined from it before
function LiFxVip_isKnownIp(%ip)
{
    if (%ip $= "") {
        return 0;
    }
    return ($LiFxVip::known[LiFxVip_key(%ip)] !$= "");
}

function LiFxVip_isProtected(%client)
{
    if (%client.vipProtected == 1 || %client.gmSteam == 1) {
        return 1;
    }
    return 0;
}

function LiFxVip_player(%client)
{
    %p = %client.player;
    if (!isObject(%p)) {
        %p = %client.getControlObject();
    }
    return %p;
}

// writes known_ips.cs (only addresses of registered VIPs, at most 40)
function LiFxVip_saveKnown()
{
    %fo = new FileObject();
    if (%fo.openForWrite("mods/LiFx/VipSlotsPack/known_ips.cs")) {
        %fo.writeLine("// written by the server (VIP addresses learned from registered Steam IDs) - do not edit");
        %fo.writeLine("$LiFxVip::knownList = \"" @ $LiFxVip::knownList @ "\";");
        for (%i = 0; %i < getWordCount($LiFxVip::knownList); %i++) {
            %k = getWord($LiFxVip::knownList, %i);
            %fo.writeLine("$LiFxVip::known[\"" @ %k @ "\"] = \"" @ $LiFxVip::known[%k] @ "\";");
        }
        %fo.close();
    }
    %fo.delete();
}

function LiFxVip_learn(%ip, %steam)
{
    %k = LiFxVip_key(%ip);
    if ($LiFxVip::known[%k] $= %steam) {
        return;
    }
    if ($LiFxVip::known[%k] $= "") {
        $LiFxVip::knownList = trim(%k SPC $LiFxVip::knownList);
        if (getWordCount($LiFxVip::knownList) > 40) {
            $LiFxVip::knownList = getWords($LiFxVip::knownList, 0, 39);
        }
    }
    $LiFxVip::known[%k] = %steam;
    LiFxVip_saveKnown();
    echo("[VIP] learned connection address" SPC %ip SPC "for VIP Steam ID" SPC %steam);
}

// one activity sweep: remember when each character last moved
function LiFxVip_tick()
{
    $LiFxVip::tickEv = schedule(20000, 0, "LiFxVip_tick");
    %now = getSimTime();
    %n = ClientGroup.getCount();
    for (%i = 0; %i < %n; %i++) {
        %c = ClientGroup.getObject(%i);
        %p = LiFxVip_player(%c);
        if (!isObject(%p)) {
            if (%c.vipActive $= "") {
                %c.vipActive = %now;
            }
            continue;
        }
        %pos = %p.getPosition();
        if (%c.vipActive $= "" || %pos !$= %c.vipPos) {
            %c.vipPos = %pos;
            %c.vipActive = %now;
        }
    }
}

// a chat message counts as activity (the original handler still runs)
package LiFxVipSlots
{
    function serverCmdLocalChatMessage(%client, %a, %b, %c) {
        %client.vipActive = getSimTime();
        Parent::serverCmdLocalChatMessage(%client, %a, %b, %c);
    }

    function LiFxVipSlots::setup() {
        if ($LiFxVip::tickEv $= "" || !isEventPending($LiFxVip::tickEv)) {
            $LiFxVip::tickEv = schedule(20000, 0, "LiFxVip_tick");
        }
        echo("[VIP] slots active: registered VIP Steam IDs:" SPC getWordCount($LiFxVip::steamIds) @ ", known addresses:" SPC getWordCount($LiFxVip::knownList) @ ", idle limit" SPC $LiFxVip::idleMinutes SPC "min, enabled" SPC $LiFxVip::enabled);
    }
    function LiFxVipSlots::version() {
        return "2.0.0";
    }

    // A character has entered the game: its account is known now, so read the Steam ID from the database.
    // Hooked on the engine function itself (works with or without the LiFx framework).
    function GameConnection::onClientEnterGame(%this) {
        Parent::onClientEnterGame(%this);
        LiFxVip_lookupSteam(%this);
    }

    // The engine checks for a free slot inside onConnectRequest: make room first when the address belongs to a VIP.
    function GameConnection::onConnectRequest(%this, %netAddress, %name) {
        LiFxVip_makeRoom(%this, %netAddress);
        return Parent::onConnectRequest(%this, %netAddress, %name);
    }

    function LiFxVip_lookupSteam(%client) {
        if (!$LiFxVip::enabled) {
            return;
        }
        %acc = %client.getAccountId();
        if (%acc $= "" || %acc == 0) {
            return;
        }
        %req = new ScriptObject() {
            class = "LiFxVipReq";
            client = %client;
        };
        dbi.select(%req, "onSteam", "SELECT SteamID FROM `account` WHERE ID=" @ %acc);
    }

    function LiFxVipReq::onSteam(%this, %rs) {
        %c = %this.client;
        if (%rs.Ok() && %rs.nextRecord() && isObject(%c)) {
            %steam = %rs.getFieldValue("SteamID");
            %c.vipSteam = %steam;
            if (LiFxVip_isVipSteam(%steam)) {
                %c.vipProtected = 1;
                %ip = LiFxVip_ipOf(%c.getAddress());
                if (%ip !$= "") {
                    LiFxVip_learn(%ip, %steam);
                }
            }
        }
        dbi.remove(%rs);
        %rs.delete();
        %this.delete();
    }

    // Removes the longest-idle player when the server is full and the connecting address belongs to a registered VIP.
    function LiFxVip_makeRoom(%client, %netAddress) {
        if (!$LiFxVip::enabled) {
            return;
        }
        %ip = LiFxVip_ipOf(%netAddress);
        if (!LiFxVip_isKnownIp(%ip)) {
            return;
        }
        %max = $Server::MaxPlayers;
        if (%max $= "" || %max < 1) {
            %max = 64;
        }
        %count = ClientGroup.getCount();
        // the connecting client may already be counted in the group
        if (ClientGroup.isMember(%client)) {
            %count--;
        }
        if (%count < %max) {
            return;
        }
        %idleMs = $LiFxVip::idleMinutes * 60000;
        %now = getSimTime();
        %best = 0;
        %bestIdle = 0;
        %n = ClientGroup.getCount();
        for (%i = 0; %i < %n; %i++) {
            %c = ClientGroup.getObject(%i);
            if (%c == %client || LiFxVip_isProtected(%c)) {
                continue;
            }
            if (%c.vipActive $= "") {
                continue;
            }
            %idle = %now - %c.vipActive;
            if (%idle >= %idleMs && %idle > %bestIdle) {
                %best = %c;
                %bestIdle = %idle;
            }
        }
        if (!%best) {
            echo("[VIP] server full, VIP address" SPC %ip SPC "wants to join but nobody has been inactive for" SPC $LiFxVip::idleMinutes SPC "min");
            return;
        }
        echo("[VIP] server full: removing inactive character" SPC %best.getCharacterId() SPC "(account" SPC %best.getAccountId() @ ", idle" SPC mFloor(%bestIdle / 60000) SPC "min) for a VIP");
        %best.cmSendClientMessage(2475, "<color:ffd060>You were removed because you were inactive and the server needed a free slot. You can rejoin at any time.");
        %best.delete("Removed for inactivity (the server was full)");
    }
};

activatePackage(LiFxVipSlots);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxVipSlots);
