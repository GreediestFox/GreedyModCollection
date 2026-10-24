// VIP slots settings (kept on reinstall).
// 1 = on. When the server is full and a connection comes from an approved IP, one inactive player is removed to make room.
$LiFxVip::enabled = 1;
// Approved IP addresses (separated by spaces). The IP is what the server sees for the connection (your home/router IP).
// The server log shows it: "Connect request from: <ip>:<port>". Home IPs often change: update this line when it does.
$LiFxVip::ips = "";   // e.g. "203.0.113.5 198.51.100.7"
// A player counts as inactive after this many minutes without moving or chatting.
$LiFxVip::idleMinutes = 20;