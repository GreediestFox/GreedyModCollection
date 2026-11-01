// VIP slots settings (kept on reinstall).
// 1 = on. When the server is full and a registered VIP connects, one inactive player is removed to make room.
$LiFxVip::enabled = 1;
// VIPs by Steam ID (64-bit "7656119..." numbers, separated by spaces). Find yours with !whoami in game, or at steamid.io.
// The address a VIP connects from is learned automatically the first time that Steam ID joins the server (known_ips.cs), so a
// VIP has to have joined at least once from the address they use. Home addresses that change are learned again on the next join.
$LiFxVip::steamIds = "";
// A player counts as inactive after this many minutes without moving or chatting.
$LiFxVip::idleMinutes = 20;
