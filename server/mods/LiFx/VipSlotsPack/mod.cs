// VIP slots (2026-10-24): when the server is FULL and a connection comes from an approved GM IP address, one inactive player is
// removed so the VIP can join. "Inactive" = the character has not moved and has not chatted for $LiFxVip::idleMinutes.
// The account is not known yet when the engine checks for a free slot (the check runs in GameConnection::onConnectRequest), so the
// VIP is recognised by the IP address of the connection. Approved IPs: mods/LiFx/VipSlotsPack/config.cs.
// Never removed: accounts in $LiFxGM::accounts / flagged GM (isGM()), and clients that connect from an approved IP themselves.
// Log lines start with [VIP].

exec("mods/LiFx/VipSlotsPack/config.cs");

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

function LiFxVip_isApprovedIp(%ip)
{
    if (%ip $= "") {
        return 0;
    }
    for (%i = 0; %i < getWordCount($LiFxVip::ips); %i++) {
        if (getWord($LiFxVip::ips, %i) $= %ip) {
            return 1;
        }
    }
    return 0;
}

function LiFxVip_isProtected(%client)
{
    if (%client.isGM()) {
        return 1;
    }
    %id = %client.getAccountId();
    for (%i = 0; %i < getWordCount($LiFxGM::accounts); %i++) {
        if (getWord($LiFxGM::accounts, %i) $= %id) {
            return 1;
        }
    }
    return (%client.vipProtected == 1);
}

function LiFxVip_player(%client)
{
    %p = %client.player;
    if (!isObject(%p)) {
        %p = %client.getControlObject();
    }
    return %p;
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
        LiFx::registerCallback($LiFx::hooks::onConnectRequestCallbacks, "LiFxVip_onConnectRequest", LiFxVipSlots);
        if ($LiFxVip::tickEv $= "" || !isEventPending($LiFxVip::tickEv)) {
            $LiFxVip::tickEv = schedule(20000, 0, "LiFxVip_tick");
        }
        echo("[VIP] slots active: approved IPs" SPC ($LiFxVip::ips $= "" ? "(none)" : $LiFxVip::ips) @ ", idle limit" SPC $LiFxVip::idleMinutes SPC "min, enabled" SPC $LiFxVip::enabled);
    }
    function LiFxVipSlots::version() {
        return "1.0.0";
    }
};

// Called by the framework before the engine checks whether the server is full (return value of the framework hook is ignored,
// so this works by removing the idle player first).
function LiFxVip_onConnectRequest(%client, %netAddress, %name)
{
    if (!$LiFxVip::enabled) {
        return;
    }
    %ip = LiFxVip_ipOf(%netAddress);
    if (!LiFxVip_isApprovedIp(%ip)) {
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
        echo("[VIP] server full, VIP IP" SPC %ip SPC "wants to join but nobody has been inactive for" SPC $LiFxVip::idleMinutes SPC "min");
        return;
    }
    echo("[VIP] server full: removing inactive character" SPC %best.getCharacterId() SPC "(account" SPC %best.getAccountId() @ ", idle" SPC mFloor(%bestIdle / 60000) SPC "min) for VIP IP" SPC %ip);
    %best.cmSendClientMessage(2475, "<color:ffd060>You were removed because you were inactive and the server needed a free slot. You can rejoin at any time.");
    %best.delete("Removed for inactivity (the server was full)");
}

activatePackage(LiFxVipSlots);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxVipSlots);
