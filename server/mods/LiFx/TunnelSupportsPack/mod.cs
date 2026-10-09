// Tunnel supports never decay (2026-10-09).
// Mine supports are tunnel "timber", not objects: boards on the walls = light timber (ability 15 "Reinforce Tunnel"),
// columns = heavy timber (ability 16 "Construct Supporting Column"). Each decay iteration (the morning clean-up)
// TerrainDeformer::_decayTimber lowers their durability by $cm_config::Geo::LightTimberDecayValue / HeavyTimberDecayValue
// (engine-bound config variables, defaults set in scripts/server/cm_config.cs);
// at 0 the timber is removed and the unsupported tunnel starts to collapse.
// Setting both decay values to 0 keeps every support forever. Unsupported tunnels still decay as before
// ($Geo::TunnelDecayValue is not touched).

if (!isObject(LiFxTunnelSupportsPack))
{
    new ScriptObject(LiFxTunnelSupportsPack)
    {
    };
}

package LiFxTunnelSupportsPack
{
    function LiFxTunnelSupportsPack::setup() {
        // apply right away and again once the server is fully created (after cm_config.cs ran)
        LiFxTunnelSupportsPack::apply("setup");
        LiFx::registerCallback($LiFx::hooks::onServerCreatedCallbacks, onServerCreated, LiFxTunnelSupportsPack);
    }
    function LiFxTunnelSupportsPack::version() {
        return "1.0.0";
    }
    function LiFxTunnelSupportsPack::onServerCreated() {
        LiFxTunnelSupportsPack::apply("server created");
    }
    function LiFxTunnelSupportsPack::apply(%when) {
        echo("[TunnelSupports] " @ %when @ ": light timber decay " @ $cm_config::Geo::LightTimberDecayValue @ " -> 0, heavy timber decay "
             @ $cm_config::Geo::HeavyTimberDecayValue @ " -> 0 (light timber resource " @ $cm_config::Geo::LightTimberDefaultDecayResource
             @ ", unsupported tunnel resource " @ $cm_config::Geo::TunnelBaseDecayResource @ ", decay " @ $cm_config::Geo::TunnelDecayValue @ " stays)");
        $cm_config::Geo::LightTimberDecayValue = 0;
        $cm_config::Geo::HeavyTimberDecayValue = 0;
    }
};
activatePackage(LiFxTunnelSupportsPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxTunnelSupportsPack);
