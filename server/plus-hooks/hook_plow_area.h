/* ===================================================================================
    Erweitert Ability 143 ("Plowing", native Implementierung AbilityImp::PlowAuto)
    von einer einzelnen Bodenkachel auf eine 3x3-Flaeche (die eigentlich
    umgepfluegte Mount0-Kachel plus ihre 8 Nachbarn).

    See hook_plow_area.cpp for the full writeup and the static RE evidence
    behind it (Doc_Tasks/Ermittlungsstand_Agriculture_mod.md, Abschnitt
    "5c"/"5d", 2026-09-21).

    ConfigurePlowArea() must be called once with the mod's XML root before
    AttachPlowAreaHook(). AttachPlowAreaHook() patches the server
    (ddctd_cm_yo_server.exe) in place. DetachPlowAreaHook() restores the
    original bytes.

    Server-only, wie hook_harvest_soil.cpp: AbilityImp::PlowAuto existiert
    nicht auf dem Client (RTTI-verifiziert, Ermittlungsstand.md 4zz-j).
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
        // Reads the <plowArea> section from the mod configuration. Must be
        // called before AttachPlowAreaHook(), and must not be called again
        // while the hook is attached.
        bool ConfigurePlowArea(const tinyxml2::XMLElement* root);

        // Verifies the expected vanilla bytes are present at +0x3A5FD1,
        // builds the replacement trampoline (which calls back into a plain
        // C++ helper for the actual 3x3 neighbor-expansion logic - see the
        // .cpp for why a hand-assembled instruction stream alone would not
        // be practical here), and patches the call site in place. No-op if
        // plowArea is not enabled or already attached.
        void AttachPlowAreaHook();

        // Restores the original vanilla bytes and frees the trampoline.
        void DetachPlowAreaHook();
    }
}
