// BooksPack settings (kept on reinstall).
// Public address of the scriptorium website (Daniel's BookWebsite). Every client opens documents there, so it must be
// reachable from the players' PCs: localhost only works for a player on this machine.
$BooksPack::baseUrl = "http://localhost:3000";
// Minimum time between two documents created by the same player (milliseconds).
$BooksPack::createCooldownMs = 10000;
// Secret key of the stone-tablet page of the website (TABLET_ADMIN_KEY in its .env); only handed to logged-in GMs. Set it locally, never commit it.
// $BooksPack::tabletAdminKey = "<your key>";
