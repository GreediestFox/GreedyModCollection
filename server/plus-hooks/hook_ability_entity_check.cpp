// Ported 2026-10-04 from Daniel's tree-felling mod (unchanged except includes; logging -> logs/agriculture.log).
/* ===================================================================================
    Extends the server's hardcoded "is this entity's ObjectTypeID one the
    Ability system is allowed to process?" check (626/653 only) with a
    configurable list of additional ObjectTypeIDs. See
    hook_ability_entity_check.h for the full write-up of how this was found
    (2026-08-05 live debugging, third and final link in the
    "Ability::canProcessEntity() - failed on checking entity" chain).

    ------------------------------------------------------------------------------
    What is CONFIRMED against the shipped ddctd_cm_yo_server.exe (build yo_1.4.4.5,
    12,441,032 bytes, SHA-256 ACD99BD1A785D95B06494CBADE51A2C14EE60A521D0E88A4BE6AE1B3B685219F),
    found via live debugging on 2026-08-05:

      A small "checker" function at +0x39ED90 (reached via a virtual call
      from Ability::canProcessEntity() at +0x3293C1) first does a C++
      dynamic_cast onto class "ComplexObject_Entity" (confirmed the Test
      Log passes this fine), then a few instructions later, at
      +0x39EE12..+0x39EE24 (19 bytes), does:
          cmp eax, 0x272      ; 626 = Hardwood Log
          je  +0x39EE25       ; -> continue to the deeper per-ability check
          cmp eax, 0x28D      ; 653 = Softwood Log
          je  +0x39EE25       ; -> continue to the deeper per-ability check
          xor sil, sil        ; else: result = false
          jmp +0x39EE37       ; -> shared epilogue, returns false
      EAX holds the target entity's ObjectTypeID at this point. Confirmed
      live: a breakpoint at +0x39EE12 is hit when sawing a Billet from the
      Test Log, and the Test Log's ObjectTypeID (2010) matches neither
      literal, so it falls to the "xor sil,sil" / false path - exactly
      matching the observed "failed on checking entity" rejection.
*  =================================================================================== */

#include "hook_ability_entity_check.h"

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
            constexpr std::uintptr_t kSelectorRva = 0x39EE12;
            constexpr std::uintptr_t kMatchedTargetRva = 0x39EE25; // continues to the deeper per-ability check
            constexpr std::uintptr_t kFailureTargetRva = 0x39EE37; // shared epilogue, returns false via SIL

            // Complete instructions from +0x39EE12 through +0x39EE24.
            // Verified byte-for-byte against the shipped exe before writing
            // this file.
            constexpr std::array<std::uint8_t, 19> kOriginalSelectorBytes = {
                0x3D, 0x72, 0x02, 0x00, 0x00, // cmp eax,0x272 (626)
                0x74, 0x0C,                   // je +0x39EE25
                0x3D, 0x8D, 0x02, 0x00, 0x00, // cmp eax,0x28D (653)
                0x74, 0x05,                   // je +0x39EE25
                0x40, 0x32, 0xF6,             // xor sil,sil
                0xEB, 0x12                    // jmp +0x39EE37
            };

            constexpr std::size_t kAbsoluteJumpSize = 14;

            // Keeps every generated "je <matched>" a one-byte (rel8) jump,
            // same reasoning as kMaxLogTypeIds in hook_log_description.cpp.
            constexpr std::size_t kMaxObjectTypeIds = 15;

            struct AbilityEntityCheckState
            {
                bool enabled = false;
                bool attached = false;
                std::vector<U32> extraObjectTypeIds;

                std::uintptr_t moduleBase = 0;
                std::uintptr_t selectorAddress = 0;
                void* trampoline = nullptr;
                std::size_t trampolineSize = 0;
            };

            AbilityEntityCheckState gAbilityEntityCheck;

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
            // xor sil, sil (3 bytes - needs a REX prefix to address SIL).
            void AppendXorSil(std::vector<std::uint8_t>& code)
            {
                code.push_back(0x40);
                code.push_back(0x32);
                code.push_back(0xF6);
            }

            // ----------------------------------------------------------------- //
            bool MatchesAbilityEntityCheckSelector(std::uintptr_t address)
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
                        "Ability entity check hook was written, but its page protection could not be restored.");
                }

                return true;
            }

            // ----------------------------------------------------------------- //
            bool BuildTrampoline()
            {
                std::vector<std::uint8_t> code;
                code.reserve(
                    // 626/653 + configured extras, 7 bytes each (5-byte cmp eax,imm32 + 2-byte je)
                    (2 + gAbilityEntityCheck.extraObjectTypeIds.size()) * 7
                    + 3               // xor sil,sil (no-match path)
                    + 2 * kAbsoluteJumpSize);

                std::vector<std::size_t> matchJeOperands;
                matchJeOperands.reserve(2 + gAbilityEntityCheck.extraObjectTypeIds.size());

                auto appendCheck = [&](std::uint32_t objectTypeId)
                {
                    // cmp eax, objectTypeId (short "3D id" form - only valid
                    // for EAX, which is exactly the register this selector
                    // already uses).
                    code.push_back(0x3D);
                    AppendU32(code, objectTypeId);

                    // je <matched> (patched once "matched" is known)
                    code.push_back(0x74);
                    matchJeOperands.push_back(code.size());
                    code.push_back(0x00); // placeholder
                };

                // Preserve the original vanilla behaviour (626/653) first,
                // then every mod-added log type.
                appendCheck(626);
                appendCheck(653);
                for (const U32 objectTypeId : gAbilityEntityCheck.extraObjectTypeIds)
                    appendCheck(objectTypeId);

                // No id matched: replicate the vanilla "xor sil,sil" before
                // jumping to the shared epilogue - it expects SIL to already
                // hold the (false) result.
                AppendXorSil(code);
                AppendAbsoluteJump(
                    code,
                    gAbilityEntityCheck.moduleBase + kFailureTargetRva);

                const std::size_t matched = code.size();
                for (const std::size_t operand : matchJeOperands)
                    code[operand] = static_cast<std::uint8_t>(matched - (operand + 1));

                // Matched: jump to the vanilla deeper per-ability check.
                AppendAbsoluteJump(
                    code,
                    gAbilityEntityCheck.moduleBase + kMatchedTargetRva);

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

                gAbilityEntityCheck.trampoline = trampoline;
                gAbilityEntityCheck.trampolineSize = code.size();
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
                    reinterpret_cast<std::uint64_t>(gAbilityEntityCheck.trampoline);
                std::memcpy(patch.data() + 6, &destination, sizeof(destination));

                return patch;
            }
        }

        // --------------------------------------------------------------------- //
        bool ConfigureAbilityEntityCheck(const tinyxml2::XMLElement* root)
        {
            if (gAbilityEntityCheck.attached)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure ability entity check while the hook is attached.");
                return false;
            }

            gAbilityEntityCheck = {};

            if (root == nullptr)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure ability entity check: XML root element is null.");
                return false;
            }

            const tinyxml2::XMLElement* section =
                root->FirstChildElement("abilityEntityCheck");

            // Missing or disabled section preserves the original game behaviour.
            if (section == nullptr)
                return true;

            gAbilityEntityCheck.enabled = section->BoolAttribute("enabled", false);
            if (!gAbilityEntityCheck.enabled)
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
                        "Invalid <abilityEntityCheck><objectTypeId>: must contain a positive integer.");
                    gAbilityEntityCheck = {};
                    return false;
                }

                if (objectTypeId == 626 || objectTypeId == 653)
                {
                    Redshark::ShowErrorMessage(
                        "Invalid <abilityEntityCheck><objectTypeId>: %u is already handled by the "
                        "vanilla game and does not need to be listed.",
                        objectTypeId);
                    gAbilityEntityCheck = {};
                    return false;
                }

                gAbilityEntityCheck.extraObjectTypeIds.push_back(static_cast<U32>(objectTypeId));
            }

            if (gAbilityEntityCheck.extraObjectTypeIds.empty())
            {
                Redshark::ShowErrorMessage(
                    "abilityEntityCheck is enabled, but no <objectTypeId> entries were provided.");
                gAbilityEntityCheck = {};
                return false;
            }

            if (gAbilityEntityCheck.extraObjectTypeIds.size() > kMaxObjectTypeIds - 2)
            {
                Redshark::ShowErrorMessage(
                    "Invalid <abilityEntityCheck>: at most %zu extra <objectTypeId> entries are supported.",
                    kMaxObjectTypeIds - 2);
                gAbilityEntityCheck = {};
                return false;
            }

            return true;
        }

        // --------------------------------------------------------------------- //
        void AttachAbilityEntityCheckHook()
        {
            if (!gAbilityEntityCheck.enabled || gAbilityEntityCheck.attached)
                return;

            Redshark::LogInfo("AttachAbilityEntityCheckHook: checkpoint A - resolving module base.");

            gAbilityEntityCheck.moduleBase =
                reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (gAbilityEntityCheck.moduleBase == 0)
            {
                Redshark::ShowErrorMessage(
                    "Can't attach ability entity check hook: main module base is null.");
                return;
            }

            Redshark::LogInfo(
                "AttachAbilityEntityCheckHook: checkpoint B - module base 0x%llx, checking selector bytes.",
                static_cast<unsigned long long>(gAbilityEntityCheck.moduleBase));

            gAbilityEntityCheck.selectorAddress =
                gAbilityEntityCheck.moduleBase + kSelectorRva;
            if (!MatchesAbilityEntityCheckSelector(gAbilityEntityCheck.selectorAddress))
            {
                Redshark::ShowErrorMessage(
                    "Can't attach ability entity check hook: expected code was not found at +0x39EE12.");
                return;
            }

            Redshark::LogInfo("AttachAbilityEntityCheckHook: checkpoint C - selector matched, building trampoline.");

            if (!BuildTrampoline())
            {
                Redshark::ShowErrorMessage(
                    "Can't attach ability entity check hook: trampoline allocation failed.");
                return;
            }

            Redshark::LogInfo(
                "AttachAbilityEntityCheckHook: checkpoint D - trampoline built (%zu bytes at %p), patching selector.",
                gAbilityEntityCheck.trampolineSize,
                gAbilityEntityCheck.trampoline);

            const auto patch = MakeSelectorPatch();
            if (!WriteExecutableMemory(
                    gAbilityEntityCheck.selectorAddress,
                    patch.data(),
                    patch.size()))
            {
                VirtualFree(gAbilityEntityCheck.trampoline, 0, MEM_RELEASE);
                gAbilityEntityCheck.trampoline = nullptr;
                gAbilityEntityCheck.trampolineSize = 0;
                Redshark::ShowErrorMessage(
                    "Can't attach ability entity check hook: selector memory is not writable.");
                return;
            }

            Redshark::LogInfo("AttachAbilityEntityCheckHook: checkpoint E - selector patched successfully.");

            gAbilityEntityCheck.attached = true;
        }

        // --------------------------------------------------------------------- //
        void DetachAbilityEntityCheckHook()
        {
            if (!gAbilityEntityCheck.attached)
                return;

            if (!WriteExecutableMemory(
                    gAbilityEntityCheck.selectorAddress,
                    kOriginalSelectorBytes.data(),
                    kOriginalSelectorBytes.size()))
            {
                Redshark::ShowErrorMessage(
                    "Can't detach ability entity check hook: original selector could not be restored.");
                return;
            }

            gAbilityEntityCheck.attached = false;

            if (gAbilityEntityCheck.trampoline != nullptr)
            {
                VirtualFree(gAbilityEntityCheck.trampoline, 0, MEM_RELEASE);
                gAbilityEntityCheck.trampoline = nullptr;
                gAbilityEntityCheck.trampolineSize = 0;
            }
        }
    }
}
