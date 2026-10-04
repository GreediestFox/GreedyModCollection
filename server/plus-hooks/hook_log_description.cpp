// Ported 2026-10-04 from Daniel's tree-felling mod (unchanged except includes; logging -> logs/agriculture.log).
/* ===================================================================================
    Extends the server's hardcoded "is this a Log?" check (626/653 only) with a
    configurable list of additional ObjectTypeIDs, so mod-added log types get
    the same "Qualitaet ... Holzmenge: X" examine text (message id 2551)
    instead of the generic "Haltbarkeit X von Y. Schadensresistenz: Z" text
    (message id 799) that every other movable object gets.

    ------------------------------------------------------------------------------
    What is CONFIRMED against the shipped ddctd_cm_yo_server.exe (build yo_1.4.4.5,
    12,441,032 bytes, SHA-256 ACD99BD1A785D95B06494CBADE51A2C14EE60A521D0E88A4BE6AE1B3B685219F),
    found via live debugging on 2026-08-01 (see notes/live_debug_registernewmovable.md
    for the RegisterNewMovable() investigation that indirectly led here) plus a
    targeted proximity search for the message ids 799/2551 as 32-bit immediates:

      - At +0x39633C..+0x396353 (24 bytes) the function at +0x396260 does:
            cmp ebx, 0x272      ; 626 = Hardwood Log
            je  +0x396432       ; -> "log" branch (message 2551)
            cmp ebx, 0x28D      ; 653 = Softwood Log
            je  +0x396432       ; -> "log" branch (message 2551)
            ; falls through to +0x396354 -> "generic movable" branch (message 799)
        EBX holds the movable object's ObjectTypeID at this point (traced back
        to `call +0x1D3790` a few instructions earlier, which the same value
        also gets stored into [rbp+0x60] for later use).
      - This exact 24-byte sequence appears nowhere else in the file (verified
        via kOriginalSelectorBytes match at attach time, same as
        hook_tree_drop.cpp).
      - No equivalent comparison exists in yo_cm_client.exe - this decision is
        made entirely server-side; the client just displays whichever message
        id the server sends.
*  =================================================================================== */

#include "hook_log_description.h"

#include "core/tinyxml2.h"
#include "agri_log.h"
#include "daniel_types.h"

#include <Windows.h>

#include <array>
#include <cstdint>
#include <vector>

namespace Hooks
{
    namespace Engine
    {
        namespace
        {
            constexpr std::uintptr_t kSelectorRva = 0x39633C;
            constexpr std::uintptr_t kSelectorReturnRva = 0x396354; // "generic movable" (799) fallthrough
            constexpr std::uintptr_t kMatchedTargetRva = 0x396432;  // "log" branch (2551)

            // Complete instructions from +0x39633C through +0x396353. Verified
            // byte-for-byte against the shipped exe before writing this file.
            constexpr std::array<std::uint8_t, 24> kOriginalSelectorBytes = {
                0x81, 0xFB, 0x72, 0x02, 0x00, 0x00, // cmp ebx,0x272 (626)
                0x0F, 0x84, 0xEA, 0x00, 0x00, 0x00, // je +0x396432
                0x81, 0xFB, 0x8D, 0x02, 0x00, 0x00, // cmp ebx,0x28D (653)
                0x0F, 0x84, 0xDE, 0x00, 0x00, 0x00  // je +0x396432
            };

            constexpr std::size_t kAbsoluteJumpSize = 14;

            // Keeps every generated "je <matched>" a one-byte (rel8) jump, same
            // reasoning as kMaxDataBlockIdsPerRule in hook_tree_drop.cpp.
            constexpr std::size_t kMaxLogTypeIds = 15;

            struct LogDescriptionState
            {
                bool enabled = false;
                bool attached = false;
                std::vector<U32> extraObjectTypeIds;

                std::uintptr_t moduleBase = 0;
                std::uintptr_t selectorAddress = 0;
                void* trampoline = nullptr;
                std::size_t trampolineSize = 0;
            };

            LogDescriptionState gLogDescription;

            // ----------------------------------------------------------------- //
            void AppendU32(std::vector<std::uint8_t>& code, std::uint32_t value)
            {
                for (unsigned shift = 0; shift < 32; shift += 8)
                    code.push_back(static_cast<std::uint8_t>(value >> shift));
            }

            // ----------------------------------------------------------------- //
            void AppendU64(std::vector<std::uint8_t>& code, std::uint64_t value)
            {
                for (unsigned shift = 0; shift < 64; shift += 8)
                    code.push_back(static_cast<std::uint8_t>(value >> shift));
            }

            // ----------------------------------------------------------------- //
            void AppendAbsoluteJump(
                std::vector<std::uint8_t>& code,
                std::uintptr_t destination)
            {
                // jmp qword ptr [rip+0], followed by the absolute destination.
                static constexpr std::uint8_t instruction[] = {
                    0xFF, 0x25, 0x00, 0x00, 0x00, 0x00
                };

                code.insert(
                    code.end(),
                    std::begin(instruction),
                    std::end(instruction));
                AppendU64(code, static_cast<std::uint64_t>(destination));
            }

            // ----------------------------------------------------------------- //
            bool MatchesLogDescriptionSelector(std::uintptr_t address)
            {
                return std::memcmp(
                    reinterpret_cast<const void*>(address),
                    kOriginalSelectorBytes.data(),
                    kOriginalSelectorBytes.size()) == 0;
            }

            // ----------------------------------------------------------------- //
            bool WriteExecutableMemory(
                std::uintptr_t address,
                const void* bytes,
                std::size_t size)
            {
                DWORD oldProtection = 0;
                if (!VirtualProtect(
                        reinterpret_cast<void*>(address),
                        size,
                        PAGE_EXECUTE_READWRITE,
                        &oldProtection))
                {
                    return false;
                }

                std::memcpy(reinterpret_cast<void*>(address), bytes, size);
                FlushInstructionCache(
                    GetCurrentProcess(),
                    reinterpret_cast<const void*>(address),
                    size);

                DWORD ignoredProtection = 0;
                if (!VirtualProtect(
                        reinterpret_cast<void*>(address),
                        size,
                        oldProtection,
                        &ignoredProtection))
                {
                    Redshark::ShowErrorMessage(
                        "Log description hook was written, but its page protection could not be restored.");
                }

                return true;
            }

            // ----------------------------------------------------------------- //
            bool BuildTrampoline()
            {
                std::vector<std::uint8_t> code;
                code.reserve(
                    // 626/653 + configured extras, 8 bytes each (6-byte cmp + 2-byte je)
                    (2 + gLogDescription.extraObjectTypeIds.size()) * 8
                    + 2 * kAbsoluteJumpSize);

                std::vector<std::size_t> matchJeOperands;
                matchJeOperands.reserve(2 + gLogDescription.extraObjectTypeIds.size());

                auto appendCheck = [&](std::uint32_t objectTypeId)
                {
                    // cmp ebx, objectTypeId
                    code.push_back(0x81);
                    code.push_back(0xFB);
                    AppendU32(code, objectTypeId);

                    // je <matched> (patched once "matched" is known)
                    code.push_back(0x74);
                    matchJeOperands.push_back(code.size());
                    code.push_back(0x00); // placeholder
                };

                // Preserve the original vanilla behaviour (626/653) first, then
                // every mod-added log type.
                appendCheck(626);
                appendCheck(653);
                for (const U32 objectTypeId : gLogDescription.extraObjectTypeIds)
                    appendCheck(objectTypeId);

                // No id matched: fall through to the vanilla "generic movable" path.
                AppendAbsoluteJump(
                    code,
                    gLogDescription.moduleBase + kSelectorReturnRva);

                const std::size_t matched = code.size();
                for (const std::size_t operand : matchJeOperands)
                    code[operand] = static_cast<std::uint8_t>(matched - (operand + 1));

                // Matched: jump to the vanilla "log" path.
                AppendAbsoluteJump(
                    code,
                    gLogDescription.moduleBase + kMatchedTargetRva);

                void* trampoline = VirtualAlloc(
                    nullptr,
                    code.size(),
                    MEM_COMMIT | MEM_RESERVE,
                    PAGE_READWRITE);
                if (trampoline == nullptr)
                    return false;

                std::memcpy(trampoline, code.data(), code.size());

                DWORD oldProtection = 0;
                if (!VirtualProtect(
                        trampoline,
                        code.size(),
                        PAGE_EXECUTE_READ,
                        &oldProtection))
                {
                    VirtualFree(trampoline, 0, MEM_RELEASE);
                    return false;
                }

                FlushInstructionCache(GetCurrentProcess(), trampoline, code.size());

                gLogDescription.trampoline = trampoline;
                gLogDescription.trampolineSize = code.size();
                return true;
            }

            // ----------------------------------------------------------------- //
            std::array<std::uint8_t, kOriginalSelectorBytes.size()> MakeSelectorPatch()
            {
                std::array<std::uint8_t, kOriginalSelectorBytes.size()> patch{};
                patch.fill(0x90);

                // jmp qword ptr [rip+0], followed by the trampoline address.
                patch[0] = 0xFF;
                patch[1] = 0x25;
                patch[2] = 0x00;
                patch[3] = 0x00;
                patch[4] = 0x00;
                patch[5] = 0x00;
                const std::uint64_t destination =
                    reinterpret_cast<std::uint64_t>(gLogDescription.trampoline);
                std::memcpy(patch.data() + 6, &destination, sizeof(destination));

                return patch;
            }
        }

        // --------------------------------------------------------------------- //
        bool ConfigureLogDescription(const tinyxml2::XMLElement* root)
        {
            if (gLogDescription.attached)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure log description while the hook is attached.");
                return false;
            }

            gLogDescription = {};

            if (root == nullptr)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure log description: XML root element is null.");
                return false;
            }

            const tinyxml2::XMLElement* section =
                root->FirstChildElement("logDescription");

            // Missing or disabled section preserves the original game behaviour.
            if (section == nullptr)
                return true;

            gLogDescription.enabled = section->BoolAttribute("enabled", false);
            if (!gLogDescription.enabled)
                return true;

            for (const tinyxml2::XMLElement* entry =
                     section->FirstChildElement("objectTypeId");
                 entry != nullptr;
                 entry = entry->NextSiblingElement("objectTypeId"))
            {
                unsigned objectTypeId = 0;
                if (entry->QueryUnsignedText(&objectTypeId) != tinyxml2::XML_SUCCESS
                    || objectTypeId == 0)
                {
                    Redshark::ShowErrorMessage(
                        "Invalid <logDescription><objectTypeId>: must contain a positive integer.");
                    gLogDescription = {};
                    return false;
                }

                if (objectTypeId == 626 || objectTypeId == 653)
                {
                    Redshark::ShowErrorMessage(
                        "Invalid <logDescription><objectTypeId>: %u is already handled by the "
                        "vanilla game and does not need to be listed.",
                        objectTypeId);
                    gLogDescription = {};
                    return false;
                }

                gLogDescription.extraObjectTypeIds.push_back(static_cast<U32>(objectTypeId));
            }

            if (gLogDescription.extraObjectTypeIds.empty())
            {
                Redshark::ShowErrorMessage(
                    "logDescription is enabled, but no <objectTypeId> entries were provided.");
                gLogDescription = {};
                return false;
            }

            if (gLogDescription.extraObjectTypeIds.size() > kMaxLogTypeIds - 2)
            {
                Redshark::ShowErrorMessage(
                    "Invalid <logDescription>: at most %zu extra <objectTypeId> entries are supported.",
                    kMaxLogTypeIds - 2);
                gLogDescription = {};
                return false;
            }

            return true;
        }

        // --------------------------------------------------------------------- //
        void AttachLogDescriptionHook()
        {
            if (!gLogDescription.enabled || gLogDescription.attached)
                return;

            Redshark::LogInfo("AttachLogDescriptionHook: checkpoint A - resolving module base.");

            gLogDescription.moduleBase =
                reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (gLogDescription.moduleBase == 0)
            {
                Redshark::ShowErrorMessage(
                    "Can't attach log description hook: main module base is null.");
                return;
            }

            Redshark::LogInfo(
                "AttachLogDescriptionHook: checkpoint B - module base 0x%llx, checking selector bytes.",
                static_cast<unsigned long long>(gLogDescription.moduleBase));

            gLogDescription.selectorAddress =
                gLogDescription.moduleBase + kSelectorRva;
            if (!MatchesLogDescriptionSelector(gLogDescription.selectorAddress))
            {
                Redshark::ShowErrorMessage(
                    "Can't attach log description hook: expected code was not found at +0x39633C.");
                return;
            }

            Redshark::LogInfo("AttachLogDescriptionHook: checkpoint C - selector matched, building trampoline.");

            if (!BuildTrampoline())
            {
                Redshark::ShowErrorMessage(
                    "Can't attach log description hook: trampoline allocation failed.");
                return;
            }

            Redshark::LogInfo(
                "AttachLogDescriptionHook: checkpoint D - trampoline built (%zu bytes at %p), patching selector.",
                gLogDescription.trampolineSize,
                gLogDescription.trampoline);

            const auto patch = MakeSelectorPatch();
            if (!WriteExecutableMemory(
                    gLogDescription.selectorAddress,
                    patch.data(),
                    patch.size()))
            {
                VirtualFree(gLogDescription.trampoline, 0, MEM_RELEASE);
                gLogDescription.trampoline = nullptr;
                gLogDescription.trampolineSize = 0;
                Redshark::ShowErrorMessage(
                    "Can't attach log description hook: selector memory is not writable.");
                return;
            }

            Redshark::LogInfo("AttachLogDescriptionHook: checkpoint E - selector patched successfully.");

            gLogDescription.attached = true;
        }

        // --------------------------------------------------------------------- //
        void DetachLogDescriptionHook()
        {
            if (!gLogDescription.attached)
                return;

            if (!WriteExecutableMemory(
                    gLogDescription.selectorAddress,
                    kOriginalSelectorBytes.data(),
                    kOriginalSelectorBytes.size()))
            {
                Redshark::ShowErrorMessage(
                    "Can't detach log description hook: original selector could not be restored.");
                return;
            }

            gLogDescription.attached = false;

            if (gLogDescription.trampoline != nullptr)
            {
                VirtualFree(gLogDescription.trampoline, 0, MEM_RELEASE);
                gLogDescription.trampoline = nullptr;
                gLogDescription.trampolineSize = 0;
            }
        }
    }
}
