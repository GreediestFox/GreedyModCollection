/* ===================================================================================
    "Is this movable object a Log?" override - public interface.

    The server hardcodes exactly two ObjectTypeIDs (626 = Hardwood Log, 653 =
    Softwood Log) as the only types that get the special "Qualitaet ...
    Holzmenge: X" examine text (message id 2551); every other movable object -
    including our new Test Log (2010, renumbered from 4090 on 2026-08-05) -
    falls through to the generic "Haltbarkeit
    X von Y. Schadensresistenz: Z" text (message id 799). This hook extends
    that hardcoded check with a configurable list of additional ObjectTypeIDs,
    the same way hook_tree_drop.cpp extends the hardwood/softwood selector.

    ConfigureLogDescription() must be called once with the mod's XML root
    before AttachLogDescriptionHook(). AttachLogDescriptionHook() patches the
    626/653 comparison inside ddctd_cm_yo_server.exe (verified build
    yo_1.4.4.5, SHA-256 ACD99BD1A785D95B06494CBADE51A2C14EE60A521D0E88A4BE6AE1B3B685219F)
    at RVA +0x39633C. DetachLogDescriptionHook() restores the original bytes.
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
        // Reads the <logDescription> section from the mod configuration and
        // stores the parsed list of extra ObjectTypeIDs. Must be called
        // before AttachLogDescriptionHook(), and must not be called again
        // while the hook is attached.
        bool ConfigureLogDescription(const tinyxml2::XMLElement* root);

        // Verifies the expected vanilla bytes are present at +0x39633C,
        // builds the replacement trampoline from the configured ids, and
        // patches the comparison in place. No-op if logDescription is not
        // enabled or already attached.
        void AttachLogDescriptionHook();

        // Restores the original vanilla bytes and frees the trampoline.
        void DetachLogDescriptionHook();
    }
}
