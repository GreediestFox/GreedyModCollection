// Lay Down (client side, dev feature). The server sends commandToClient('LayDownAnim', phase) when ability 192 starts.
// knockdown (hold) -> knockdown_stay (re-asserted a few times) ; movement keys or jump stand the character up with riseup_KD.
// While lying the camera is free-look (mouse turns the camera, not the body).
$LayDown::active = 0;

function LayDown_player() {
    if (!isObject(ServerConnection)) { return 0; }
    %p = ServerConnection.getControlObject();
    if (!isObject(%p)) { return 0; }
    return %p;
}

function clientCmdLayDownAnim(%phase) {
    %p = LayDown_player();
    if (!isObject(%p)) { return; }
    if (%phase $= "start") {
        if ($LayDown::active) { return; }
        $LayDown::active = 1;
        $LayDown::savedFreeLook = $mvFreeLook;
        $mvFreeLook = true;      // camera turns freely around the lying body
        cancel($LayDown::ev1); cancel($LayDown::ev2); cancel($LayDown::ev3); cancel($LayDown::watchEv);
        %p.setActionThread("knockdown_stay", true);
        $LayDown::ev1 = schedule(400, 0, "LayDown_stay");
        $LayDown::ev2 = schedule(1200, 0, "LayDown_stay");
        $LayDown::ev3 = schedule(2000, 0, "LayDown_stay");
        $LayDown::watchEv = schedule(1200, 0, "LayDown_watch");
        echo("[LayDown] client start");
    }
}

function LayDown_stay() {
    if (!$LayDown::active) { return; }
    %p = LayDown_player();
    if (!isObject(%p)) { return; }
    %p.setActionThread("knockdown_stay", true);
}

function LayDown_watch() {
    if (!$LayDown::active) { return; }
    if ($mvForwardAction != 0 || $mvBackwardAction != 0 || $mvLeftAction != 0 || $mvRightAction != 0) {
        LayDown_rise();
        return;
    }
    $LayDown::watchEv = schedule(80, 0, "LayDown_watch");
}

function LayDown_rise() {
    if ($LayDown::active) { commandToServer('LayDownRise'); }
    $LayDown::active = 0;
    cancel($LayDown::ev1); cancel($LayDown::ev2); cancel($LayDown::ev3);
    $mvFreeLook = $LayDown::savedFreeLook;
    %p = LayDown_player();
    if (!isObject(%p)) { return; }
    %p.setActionThread("riseup_KD", false);
    echo("[LayDown] client rise");
}

package LayDownInput
{
    // mouse yaw is no longer blocked: free look ($mvFreeLook) keeps the body still while the camera turns
    function yaw(%val) {
        Parent::yaw(%val);
    }
    function jump(%val) {
        if ($LayDown::active && %val) { LayDown_rise(); return; }
        Parent::jump(%val);
    }
};
activatePackage(LayDownInput);

// ---- other players lying down: the server names the ghost id of a lying player (and repeats it every 2 s).
// We keep that ghost in knockdown_stay until told to stop, or until the messages stop arriving (6 s), or the object is gone.
$LayDown::othCount = 0;

function clientCmdLayDownOther(%gid, %phase) {
    if (!isObject(ServerConnection)) { return; }
    %o = ServerConnection.resolveGhostID(%gid);
    if (!isObject(%o)) { return; }
    %id = %o.getId();
    if (%phase $= "stop") {
        if ($LayDown::oth[%id]) {
            $LayDown::oth[%id] = 0;
            %o.setActionThread("riseup_KD", false);
        }
        return;
    }
    if (!$LayDown::oth[%id]) {
        $LayDown::oth[%id] = 1;
        $LayDown::othList = $LayDown::othList SPC %id;
        %o.setActionThread("knockdown_stay", true);
    }
    $LayDown::othSeen[%id] = getSimTime();
    if (!$LayDown::othTick) { $LayDown::othTick = 1; schedule(250, 0, "LayDown_othTick"); }
}

function LayDown_othTick() {
    %keep = "";
    %n = getWordCount($LayDown::othList);
    for (%i = 0; %i < %n; %i++) {
        %id = getWord($LayDown::othList, %i);
        if (!$LayDown::oth[%id]) { continue; }
        if (!isObject(%id)) { $LayDown::oth[%id] = 0; continue; }
        if (getSimTime() - $LayDown::othSeen[%id] > 6000) {
            $LayDown::oth[%id] = 0;
            %id.setActionThread("riseup_KD", false);
            continue;
        }
        %id.setActionThread("knockdown_stay", true);
        %keep = %keep SPC %id;
    }
    $LayDown::othList = trim(%keep);
    if ($LayDown::othList $= "") { $LayDown::othTick = 0; return; }
    schedule(250, 0, "LayDown_othTick");
}
