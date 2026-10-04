/* ===================================================================================
    "Which item does Saw out produce, and how much Durability does it cost?"
    override - public interface.

    The server hardcodes exactly two ObjectTypeIDs (626 = Hardwood Log, 653 =
    Softwood Log) as valid Saw-out targets in the SAME function that computes
    the produced item (Billet/Board/Building Log) and the Durability cost
    taken from the log. Every other movable object - including our Test
    Log/4090 - falls into the "unknown material" error path and produces
    nothing. This hook extends that hardcoded check with a configurable list
    of additional ObjectTypeIDs, each mapped to its own Billet/Board product
    ids (Building Log always stays the existing, shared ObjectTypeID 233 -
    the vanilla code hardcodes that one regardless of material, so nothing
    needs to change there).

    NOTE (2026-08-05): this hook's own patch is confirmed working correctly
    (the "unknown material" gate is successfully extended and Durability is
    deducted as configured) - but reaching it at all depends on passing an
    EARLIER, separate native check, Ability::canProcessEntity(), which was
    found to reject large custom ObjectTypeIDs (e.g. 4090) independently of
    this hook. Test Log/Billet/Board were renumbered to 2010/2012/2013 to
    test that theory - see Server_mod/config/tree_drops.xml for the full
    writeup. This hook's own logic and RVA are unaffected by that renumber.

    ConfigureSawOutput() must be called once with the mod's XML root before
    AttachSawOutputHook(). AttachSawOutputHook() patches the selector inside
    ddctd_cm_yo_server.exe (verified build yo_1.4.4.5, SHA-256
    ACD99BD1A785D95B06494CBADE51A2C14EE60A521D0E88A4BE6AE1B3B685219F) at RVA
    +0x39E761. DetachSawOutputHook() restores the original bytes.
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
        // Reads the <sawOutput> section from the mod configuration and
        // stores the parsed list of extra ObjectTypeID -> Billet/Board
        // product mappings. Must be called before AttachSawOutputHook(),
        // and must not be called again while the hook is attached.
        bool ConfigureSawOutput(const tinyxml2::XMLElement* root);

        // Verifies the expected vanilla bytes are present at +0x39E761,
        // builds the replacement trampoline from the configured mappings,
        // and patches the selector in place. No-op if sawOutput is not
        // enabled or already attached.
        void AttachSawOutputHook();

        // Restores the original vanilla bytes and frees the trampoline.
        void DetachSawOutputHook();
    }
}
