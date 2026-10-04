/* ===================================================================================
    "Is this entity's ObjectTypeID one I'm allowed to process?" override -
    public interface.

    This is the THIRD and final hardcoded 626/653 check found in the chain
    that produces "Ability::canProcessEntity() - failed on checking entity"
    for the Saw-out abilities ("a Billet"/"a Board"/"a Building Log") on any
    mod-added log, e.g. our Test Log (2010, renumbered from 4090 on
    2026-08-05). The other two links in that chain turned out to be
    red herrings/already fine:

      1. The renumber from 4090->2010 (see Server_mod/config/tree_drops.xml)
         was based on a "separate, smaller-capacity ID lookup" theory that
         live debugging on 2026-08-05 DISPROVED - the identical rejection
         happened at 2010 too, ruling out ID magnitude entirely.
      2. Live debugging then traced the rejection to a virtual call inside
         Ability::canProcessEntity() (ddctd_cm_yo_server.exe+0x3293C1, a
         "checker" vtable slot at offset +0x10) into a small function at
         +0x39ED90. That function's FIRST gate is a C++ dynamic_cast
         (__RTDynamicCast) of the entity onto class "ComplexObject_Entity" -
         initially suspected as the real blocker, but a breakpoint at
         +0x39EE12 (see below) confirmed the Test Log PASSES this cast fine,
         so it was never the problem.

    The actual blocker is a plain, simple hardcoded ID compare a few
    instructions later in that SAME function, at +0x39EE12..+0x39EE24 (19
    bytes) - confirmed live 2026-08-05 (breakpoint hit while sawing a
    Billet from the Test Log):
        cmp eax, 0x272      ; 626 = Hardwood Log
        je  +0x39EE25       ; -> continue to the deeper per-ability check
        cmp eax, 0x28D      ; 653 = Softwood Log
        je  +0x39EE25       ; -> continue to the deeper per-ability check
        xor sil, sil        ; else: result = false
        jmp +0x39EE37       ; -> shared epilogue, returns false
    EAX holds the target entity's ObjectTypeID at this point (obtained a
    few instructions earlier via two calls that unwrap the just-verified
    ComplexObject_Entity instance back down to its ObjectTypeID). This is
    the same register/same value hook_log_description.cpp's and
    hook_saw_output.cpp's selectors already key off - all three hardcodes
    are independent, unrelated pieces of native code that all happen to
    gate on the exact same two literals.

    Note the fallback path is NOT a simple fallthrough like
    hook_log_description.cpp's - it explicitly zeroes SIL (the low byte of
    RSI, repurposed here as the boolean accumulator returned via
    `movzx eax, sil` at the function's very end) before jumping to the
    shared epilogue at +0x39EE37. The trampoline built by this hook
    reproduces that "xor sil,sil" for its own no-match path so the epilogue
    still sees a correctly-zeroed result.

    ConfigureAbilityEntityCheck() must be called once with the mod's XML
    root before AttachAbilityEntityCheckHook(). AttachAbilityEntityCheckHook()
    patches the comparison inside ddctd_cm_yo_server.exe (verified build
    yo_1.4.4.5, SHA-256 ACD99BD1A785D95B06494CBADE51A2C14EE60A521D0E88A4BE6AE1B3B685219F)
    at RVA +0x39EE12. DetachAbilityEntityCheckHook() restores the original
    bytes.
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
        // Reads the <abilityEntityCheck> section from the mod configuration
        // and stores the parsed list of extra ObjectTypeIDs. Must be called
        // before AttachAbilityEntityCheckHook(), and must not be called
        // again while the hook is attached.
        bool ConfigureAbilityEntityCheck(const tinyxml2::XMLElement* root);

        // Verifies the expected vanilla bytes are present at +0x39EE12,
        // builds the replacement trampoline from the configured ids, and
        // patches the comparison in place. No-op if abilityEntityCheck is
        // not enabled or already attached.
        void AttachAbilityEntityCheckHook();

        // Restores the original vanilla bytes and frees the trampoline.
        void DetachAbilityEntityCheckHook();
    }
}
