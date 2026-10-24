// QuickJoin (2026-10-24): the main menu button "Join a world" connects straight to our server instead of opening the server browser.
// Set $QuickJoin::enabled = 0 to get the normal server browser back.
// joinToRemoteServer(address, password) is the game's own function (scripts/yo.cs); the "Local Server" dev button uses it as well.
$QuickJoin::enabled = 1;
$QuickJoin::address = "YOUR.SERVER.IP:28000";
$QuickJoin::password = "YOUR-JOIN-PASSWORD";

package QuickJoin
{
    function MainMenuMultiplayerWindow::OnJoinWorldBtnClick(%this)
    {
        if ($QuickJoin::enabled) {
            echo("[QuickJoin] connecting to" SPC $QuickJoin::address);
            joinToRemoteServer($QuickJoin::address, $QuickJoin::password);
            return;
        }
        Parent::OnJoinWorldBtnClick(%this);
    }
};
activatePackage(QuickJoin);