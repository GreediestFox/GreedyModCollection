/* ===================================================================================
    Server-side twin of Client_mod/hooks/engine/hook_plow_cart_parity.cpp.

    Gives the Plow the same "is this a cart-type object" treatment as the
    Handkarren/Wheelbarrow/Trader-cart inside the engine's per-tick
    acceleration/deceleration tuning - a hardcoded `object_type_id`-specific
    branch found 2026-09-20 via static capstone disassembly, independent of
    Ability 143 (already ruled out, s. Ermittlungsstand.md 4zz-d) and
    independent of the displayed mesh (s. the historical Transport.cs
    shapefile-swap test).

    ------------------------------------------------------------------------------
    Background (s. Ermittlungsstand.md sections 4zz-z24/4zz-z25 for the full
    write-up and the correction of an earlier, backwards reading of this
    branch's direction):

    A shared, non-overridden base-class virtual method - present
    byte-identically (save for struct-layout offsets) on both
    ddctd_cm_yo_server.exe and yo_cm_client.exe, confirmed via three matching
    .rdata vtable-pointer hits on each side - selects one of two tuning
    multipliers for the currently-moved/pushed object on every physics tick:

        cmp dword ptr [rbx+0x1074], 2
        jne useDefault                    ; not a "category 2" (cart-like) object
        cmp dword ptr [rbx+0x1080], 0x35   ; 53 = Plow
        je  useDefault                     ; <-- IS the Plow -> also falls back to default!
        movss xmm6, [rsi+0xc]             ; "cart" multiplier - every OTHER category-2 object
        jmp combine
    useDefault:
        movss xmm6, [rsi+4]               ; generic default - non-cart objects, AND (the bug) the Plow
    combine:
        ...

    Every other pushable/towable device the engine classifies as "category 2"
    (Handkarren, Schubkarre, Traderkarren, ...) receives a dedicated "cart"
    tuning value here. The Plow is explicitly carved back OUT of that
    treatment by the second compare and falls through to the same generic
    default used for objects that are not category 2 at all.

    This hook removes exactly that carve-out: it turns the `je useDefault`
    (2 bytes, `74 07`) into two NOPs, so the Plow always takes the same
    "cart" branch as every other category-2 object. No other change to the
    surrounding logic.

    IMPORTANT: the identical construct exists on the CLIENT too (s.
    Client_mod/hooks/engine/hook_plow_cart_parity.cpp, patching the
    equivalent instruction at client RVA +0x154F1D). Both sides must be
    patched together - patching only one side would make client-prediction
    and server-authority compute DIFFERENT tuning values for the Plow and
    very likely make checksum mismatches WORSE, not better.

    ------------------------------------------------------------------------------
    Where the value lives and how it is patched:

    Server RVA +0x9E34D holds the 2-byte conditional jump `je +0x07` (bytes
    74 07) inside the function at +0x9E310. Verified byte-for-byte via
    capstone disassembly of the shipped ddctd_cm_yo_server.exe (verified
    build yo_1.4.4.5, 12,441,032 bytes, SHA-256
    ACD99BD1A785D95B06494CBADE51A2C14EE60A521D0E88A4BE6AE1B3B685219F) before
    writing this file. Patched in place to `90 90` (NOP NOP) - a pure 2-byte
    overwrite, no trampoline needed since no call/jump target is being
    relocated.

    ConfigurePlowCartParity() must be called once with the mod's XML root
    before AttachPlowCartParityHook(). DetachPlowCartParityHook() restores
    the original bytes.
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
        // Reads the <plowCartParity> section from the mod configuration.
        // Must be called before AttachPlowCartParityHook(), and must not be
        // called again while the hook is attached.
        bool ConfigurePlowCartParity(const tinyxml2::XMLElement* root);

        // Verifies the expected vanilla bytes are present at +0x9E34D and
        // patches them to two NOPs. No-op if plowCartParity is not enabled
        // or already attached.
        void AttachPlowCartParityHook();

        // Restores the original vanilla bytes.
        void DetachPlowCartParityHook();
    }
}
