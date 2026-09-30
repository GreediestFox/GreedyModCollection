// Lay Down (ability 192, formerly Disapproval). The DLL calls LiFxLayDown_onStart(charId) right after the engine registers the perform.
// Plays knockdown, then holds knockdown_stay. Stand-up (riseup_KD) is triggered by LiFxLayDown_onEnd(charId) when the ability ends/cancels.

if (!isObject(LiFxLayDown))
{
    new ScriptObject(LiFxLayDown)
    {
    };
}

function LiFxLayDown::findPlayer(%charId) {
    for (%i = 0; %i < ClientGroup.getCount(); %i++) {
        %c = ClientGroup.getObject(%i);
        if (%c.getCharacterId() == %charId) {
            return LiFxGMCommands::getPlayer(%c);
        }
    }
    return 0;
}

function LiFxLayDown::findClient(%charId) {
    for (%i = 0; %i < ClientGroup.getCount(); %i++) {
        %c = ClientGroup.getObject(%i);
        if (%c.getCharacterId() == %charId) { return %c; }
    }
    return 0;
}

function LiFxLayDown_onStart(%charId) {
    %c = LiFxLayDown::findClient(%charId);
    $LiFxLayDown::lying[%charId] = 1;
    cancel($LiFxLayDown_beatEv[%charId]);
    LiFxLayDown_beat(%charId);
    if (isObject(%c)) { commandToClient(%c, 'LayDownAnim', "start"); }
    %p = LiFxLayDown::findPlayer(%charId);
    if (!isObject(%p)) {
        echo("[LayDown] no player object for char" SPC %charId);
        return;
    }
    $LiFxLayDown::active[%charId] = 1;
    echo("[LayDown] start char" SPC %charId SPC "player" SPC %p SPC "-> client command sent");
}

function LiFxLayDown_stay(%charId) {
    if (!$LiFxLayDown::active[%charId]) { return; }
    %p = LiFxLayDown::findPlayer(%charId);
    if (!isObject(%p)) { return; }
    %p.setActionThread("knockdown_stay");
    echo("[LayDown] stay char" SPC %charId);
}

function LiFxLayDown_onEnd(%charId) {
    $LiFxLayDown::active[%charId] = 0;
    cancel($LiFxLayDown::stay[%charId]);
    %p = LiFxLayDown::findPlayer(%charId);
    if (!isObject(%p)) { return; }
    %p.setActionThread("riseup_KD");
    echo("[LayDown] rise char" SPC %charId);
}

// ---- visible to other players: tell every other client (that has a ghost of the lying player) to hold the lying pose.
// The message is repeated every 2 s while lying (late joiners, ghost re-creation); the lying client reports standing up.
function LiFxLayDown_broadcast(%charId, %phase) {
    %src = LiFxLayDown::findClient(%charId);
    if (!isObject(%src)) { return 0; }
    %p = LiFxGMCommands::getPlayer(%src);
    if (!isObject(%p)) { return 0; }
    %sent = 0;
    for (%i = 0; %i < ClientGroup.getCount(); %i++) {
        %c = ClientGroup.getObject(%i);
        if (%c == %src) { continue; }
        %gid = %c.getGhostID(%p);
        if (%gid >= 0) { commandToClient(%c, 'LayDownOther', %gid, %phase); %sent++; }
    }
    return %sent;
}

function LiFxLayDown_beat(%charId) {
    if (!$LiFxLayDown::lying[%charId]) { return; }
    %src = LiFxLayDown::findClient(%charId);
    if (!isObject(%src) || !isObject(LiFxGMCommands::getPlayer(%src))) {
        $LiFxLayDown::lying[%charId] = 0;
        return;
    }
    LiFxLayDown_broadcast(%charId, "start");
    $LiFxLayDown_beatEv[%charId] = schedule(2000, 0, "LiFxLayDown_beat", %charId);
}

// sent by the lying player's own client when it stands up
function serverCmdLayDownRise(%client) {
    %charId = %client.getCharacterId();
    if (!$LiFxLayDown::lying[%charId]) { return; }
    $LiFxLayDown::lying[%charId] = 0;
    cancel($LiFxLayDown_beatEv[%charId]);
    %n = LiFxLayDown_broadcast(%charId, "stop");
    echo("[LayDown] char" SPC %charId SPC "stood up; told" SPC %n SPC "other clients");
}
