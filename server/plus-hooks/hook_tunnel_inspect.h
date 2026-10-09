/* ===================================================================================
    Tunnel inspect text for supported tunnels - public interface (2026-10-09).

    Inspecting a tunnel tile ("Inspect" on tun_side, AbilityImp::Inspect::_inspectEntity)
    always shows message 2555 "It will last %1 days until collapsing", with
    days = InspectHelper::calculateTunnelDecay() = tunnel resource / TunnelDecayValue + 1.
    That formula ignores timber. With the timber decay set to 0 (TunnelSupportsPack),
    supported tiles never collapse, but the text still shows a fixed number of days.

    This hook replaces the call site in the inspect function (RVA +0x397AF5 ..
    +0x397B07: lea rcx / call calculateTunnelDecay / store days / mov edx,2555)
    with a trampoline that reads the tile's level flags first (FUN_1404AEDD0, byte 3:
    0x20 light timber, 0x40 heavy timber) and then calls the original day calculation.
    For a tile with timber the message id becomes <tunnelInspect messageId="...">
    (default 5213, "This tunnel section is supported and will not collapse").
    Verified build yo_1.4.4.5, SHA-256 ACD99BD1A785D95B06494CBADE51A2C14EE60A521D0E88A4BE6AE1B3B685219F.
*  =================================================================================== */

#pragma once

namespace tinyxml2
{
    class XMLElement;
}

namespace Hooks
{
    namespace Engine
    {
        // Reads <tunnelInspect enabled="1" messageId="5213"/>. Missing or disabled = vanilla behaviour.
        bool ConfigureTunnelInspect(const tinyxml2::XMLElement* root);

        // Verifies the vanilla bytes at +0x397AF5 and patches the call site. No-op when disabled.
        void AttachTunnelInspectHook();

        // Restores the original bytes and frees the trampoline.
        void DetachTunnelInspectHook();
    }
}
