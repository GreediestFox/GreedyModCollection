/* ===================================================================================
    Reverts a harvested field's terrain substance from its "loosened"
    (tilled/planted) variant back to the settled one - public interface.

    See hook_harvest_soil.cpp for the full writeup and the static RE
    evidence behind it (Ermittlungsstand.md, "Ziel 3" / Agriculture_mod
    investigation, 2026-09-21).

    ConfigureHarvestSoil() must be called once with the mod's XML root
    before AttachHarvestSoilHook(). AttachHarvestSoilHook() patches the
    server (ddctd_cm_yo_server.exe) in place. DetachHarvestSoilHook()
    restores the original bytes.
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
        // Reads the <harvestSoil> section from the mod configuration. Must
        // be called before AttachHarvestSoilHook(), and must not be called
        // again while the hook is attached.
        bool ConfigureHarvestSoil(const tinyxml2::XMLElement* root);

        // Verifies the expected vanilla bytes are present at +0x3A9759,
        // builds the replacement trampoline, and patches the call site in
        // place. No-op if harvestSoil is not enabled or already attached.
        void AttachHarvestSoilHook();

        // Restores the original vanilla bytes and frees the trampoline.
        void DetachHarvestSoilHook();
    }
}
