// Ported 2026-10-04 from Daniel's tree-felling mod (unchanged except includes; logging -> logs/agriculture.log).
/* ===================================================================================
    Extends the server's hardcoded "Saw out" material selector (626/653 only)
    with a configurable list of additional ObjectTypeIDs, each mapped to its
    own Billet/Board product ids, so mod-added log types can be sawn into
    mod-added items instead of falling into the "unknown material" error path.

    ------------------------------------------------------------------------------
    What is CONFIRMED against the shipped ddctd_cm_yo_server.exe (build yo_1.4.4.5,
    12,441,032 bytes, SHA-256 ACD99BD1A785D95B06494CBADE51A2C14EE60A521D0E88A4BE6AE1B3B685219F),
    found 2026-08-03 via a proximity search for 626/653 clustered together
    with the vanilla Billet/Board/Building Log ObjectTypeIDs (324/325/326/
    327/233) as 32-bit immediates (same technique that found the log
    description hardcode, extended to a second anchor set):

      At +0x39E761..+0x39E77C (28 bytes) the "Saw out" ability-completion
      function does:
          mov edx, 1
          mov r8d, edx                 ; edx = r8d = 1 ("softwood" material index)
          cmp eax, 0x28D                ; 653 = Softwood Log
          je  +0x39E77D
          cmp eax, 0x272                ; 626 = Hardwood Log
          jne +0x39E7A7                 ; -> "unknown material" error path, no item/no Durability cost
          mov r8d, r13d                 ; r13d == 0 here -> edx = r8d = 0 ("hardwood" material index)
          mov edx, r13d
          ; +0x39E77D onward (unconditionally reached for 626/653, skipped
          ; otherwise): dispatches on the ABILITY id (37/38/39) read from
          ; [r15+8]:
          ;   ability 37 ("a Billet"):      item = edx + 324, Durability cost = 10
          ;   ability 38 ("a Board"):       item = r8d + 326, Durability cost = 20
          ;   ability 39 ("a Building Log"): item = 233 (fixed, ignores edx/r8d), Durability cost = 100
      EAX holds the target's ObjectTypeID at this point (same register/same
      value as hook_log_description.cpp's selector, confirmed by inspection -
      both selectors run against the same "which log is this" classification
      done earlier in the ability-completion code path).

      The Durability-cost values (10/20/100) are NOT conditioned on material
      at all - they're plain immediates in the per-ability branches. That
      means once an ObjectTypeID is accepted by this selector, the existing
      Durability logic already does exactly what's wanted for a new log type
      with no further changes: this hook only needs to get mod-added
      ObjectTypeIDs *past* the 626/653 gate, with edx/r8d set so the
      existing "item = edx + 324" / "item = r8d + 326" arithmetic lands on
      the configured product ids. Building Log's output (fixed 233) needs no
      hook at all - every material already shares that one ObjectTypeID.
*  =================================================================================== */

#include "hook_saw_output.h"

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
            constexpr std::uintptr_t kSelectorRva = 0x39E761;
            constexpr std::uintptr_t kMatchedTargetRva = 0x39E77D;  // ability-id dispatch (billet/board/buildinglog)
            constexpr std::uintptr_t kFallbackTargetRva = 0x39E7A7; // "unknown material" error path

            // The vanilla arithmetic this hook piggybacks on instead of
            // touching the ability-id dispatch itself.
            constexpr std::int32_t kBilletBaseObjectTypeId = 324; // item = edx + 324
            constexpr std::int32_t kBoardBaseObjectTypeId = 326;  // item = r8d + 326

            // Complete instructions from +0x39E761 through +0x39E77C.
            // Verified byte-for-byte against the shipped exe before writing
            // this file.
            constexpr std::array<std::uint8_t, 28> kOriginalSelectorBytes = {
                0xBA, 0x01, 0x00, 0x00, 0x00,       // mov edx,1
                0x44, 0x8B, 0xC2,                   // mov r8d,edx
                0x3D, 0x8D, 0x02, 0x00, 0x00,       // cmp eax,0x28D (653)
                0x74, 0x0D,                          // je +0x39E77D
                0x3D, 0x72, 0x02, 0x00, 0x00,       // cmp eax,0x272 (626)
                0x75, 0x30,                          // jne +0x39E7A7
                0x45, 0x8B, 0xC5,                    // mov r8d,r13d
                0x41, 0x8B, 0xD5                     // mov edx,r13d
            };

            constexpr std::size_t kAbsoluteJumpSize = 14;

            // Keeps every generated "je <matched>" a one-byte (rel8) jump -
            // same reasoning as kMaxLogTypeIds in hook_log_description.cpp.
            constexpr std::size_t kMaxMappings = 15;

            struct SawOutputMapping
            {
                U32 objectTypeId = 0;
                std::int32_t billetOffset = 0; // billetObjectTypeId - 324
                std::int32_t boardOffset = 0;  // boardObjectTypeId - 326
            };

            struct SawOutputState
            {
                bool enabled = false;
                bool attached = false;
                std::vector<SawOutputMapping> mappings;

                std::uintptr_t moduleBase = 0;
                std::uintptr_t selectorAddress = 0;
                void* trampoline = nullptr;
                std::size_t trampolineSize = 0;
            };

            SawOutputState gSawOutput;

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
            // mov edx, imm32 (5 bytes)
            void AppendMovEdx(std::vector<std::uint8_t>& code, std::int32_t value)
            {
                code.push_back(0xBA);
                AppendU32(code, static_cast<std::uint32_t>(value));
            }

            // ----------------------------------------------------------------- //
            // mov r8d, imm32 (6 bytes - needs a REX.B prefix to reach r8d)
            void AppendMovR8d(std::vector<std::uint8_t>& code, std::int32_t value)
            {
                code.push_back(0x41);
                code.push_back(0xB8);
                AppendU32(code, static_cast<std::uint32_t>(value));
            }

            // ----------------------------------------------------------------- //
            bool MatchesSawOutputSelector(std::uintptr_t address)
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
                        "Saw output hook was written, but its page protection could not be restored.");
                }

                return true;
            }

            // ----------------------------------------------------------------- //
            bool BuildTrampoline()
            {
                std::vector<std::uint8_t> code;
                code.reserve(
                    // 626/653 + configured extras, 7 bytes each (5-byte cmp eax,imm32 + 2-byte je)
                    (2 + gSawOutput.mappings.size()) * 7
                    + kAbsoluteJumpSize  // no-match tail
                    + 2 * (5 + 6 + kAbsoluteJumpSize)  // softwood + hardwood branches
                    + gSawOutput.mappings.size() * (5 + 6 + kAbsoluteJumpSize));

                struct PendingBranch
                {
                    std::size_t jePatchOffset;
                    std::int32_t edxValue;
                    std::int32_t r8dValue;
                };
                std::vector<PendingBranch> pending;
                pending.reserve(2 + gSawOutput.mappings.size());

                auto appendCheck = [&](std::uint32_t objectTypeId, std::int32_t edxValue, std::int32_t r8dValue)
                {
                    // cmp eax, objectTypeId (short "3D id" form - only valid
                    // for EAX, which is exactly the register this selector
                    // already uses).
                    code.push_back(0x3D);
                    AppendU32(code, objectTypeId);

                    // je <branch> (patched once every branch's offset is known)
                    code.push_back(0x74);
                    const std::size_t jePatchOffset = code.size();
                    code.push_back(0x00); // placeholder

                    pending.push_back({jePatchOffset, edxValue, r8dValue});
                };

                // Preserve the original vanilla behaviour (653 = softwood,
                // 626 = hardwood) first, then every mod-added mapping.
                appendCheck(653, 1, 1);
                appendCheck(626, 0, 0);
                for (const SawOutputMapping& mapping : gSawOutput.mappings)
                    appendCheck(mapping.objectTypeId, mapping.billetOffset, mapping.boardOffset);

                // No id matched: original "unknown material" error path.
                AppendAbsoluteJump(code, gSawOutput.moduleBase + kFallbackTargetRva);

                // One branch per pending check, in the same order - each
                // sets edx/r8d then jumps to the vanilla ability-id dispatch.
                for (const PendingBranch& branch : pending)
                {
                    const std::size_t branchStart = code.size();
                    code[branch.jePatchOffset] =
                        static_cast<std::uint8_t>(branchStart - (branch.jePatchOffset + 1));

                    AppendMovEdx(code, branch.edxValue);
                    AppendMovR8d(code, branch.r8dValue);
                    AppendAbsoluteJump(code, gSawOutput.moduleBase + kMatchedTargetRva);
                }

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

                gSawOutput.trampoline = trampoline;
                gSawOutput.trampolineSize = code.size();
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
                    reinterpret_cast<std::uint64_t>(gSawOutput.trampoline);
                std::memcpy(patch.data() + 6, &destination, sizeof(destination));

                return patch;
            }
        }

        // --------------------------------------------------------------------- //
        bool ConfigureSawOutput(const tinyxml2::XMLElement* root)
        {
            if (gSawOutput.attached)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure saw output while the hook is attached.");
                return false;
            }

            gSawOutput = {};

            if (root == nullptr)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure saw output: XML root element is null.");
                return false;
            }

            const tinyxml2::XMLElement* section =
                root->FirstChildElement("sawOutput");

            // Missing or disabled section preserves the original game behaviour.
            if (section == nullptr)
                return true;

            gSawOutput.enabled = section->BoolAttribute("enabled", false);
            if (!gSawOutput.enabled)
                return true;

            for (const tinyxml2::XMLElement* entry =
                     section->FirstChildElement("mapping");
                 entry != nullptr;
                 entry = entry->NextSiblingElement("mapping"))
            {
                unsigned objectTypeId = 0;
                unsigned billetObjectTypeId = 0;
                unsigned boardObjectTypeId = 0;

                if (entry->QueryUnsignedAttribute("objectTypeId", &objectTypeId) != tinyxml2::XML_SUCCESS
                    || entry->QueryUnsignedAttribute("billetObjectTypeId", &billetObjectTypeId) != tinyxml2::XML_SUCCESS
                    || entry->QueryUnsignedAttribute("boardObjectTypeId", &boardObjectTypeId) != tinyxml2::XML_SUCCESS
                    || objectTypeId == 0 || billetObjectTypeId == 0 || boardObjectTypeId == 0)
                {
                    Redshark::ShowErrorMessage(
                        "Invalid <sawOutput><mapping>: objectTypeId, billetObjectTypeId and "
                        "boardObjectTypeId must all be positive integers.");
                    gSawOutput = {};
                    return false;
                }

                if (objectTypeId == 626 || objectTypeId == 653)
                {
                    Redshark::ShowErrorMessage(
                        "Invalid <sawOutput><mapping>: objectTypeId %u is already handled by "
                        "the vanilla game and does not need to be listed.",
                        objectTypeId);
                    gSawOutput = {};
                    return false;
                }

                SawOutputMapping mapping;
                mapping.objectTypeId = static_cast<U32>(objectTypeId);
                mapping.billetOffset =
                    static_cast<std::int32_t>(billetObjectTypeId) - kBilletBaseObjectTypeId;
                mapping.boardOffset =
                    static_cast<std::int32_t>(boardObjectTypeId) - kBoardBaseObjectTypeId;
                gSawOutput.mappings.push_back(mapping);
            }

            if (gSawOutput.mappings.empty())
            {
                Redshark::ShowErrorMessage(
                    "sawOutput is enabled, but no <mapping> entries were provided.");
                gSawOutput = {};
                return false;
            }

            if (gSawOutput.mappings.size() > kMaxMappings - 2)
            {
                Redshark::ShowErrorMessage(
                    "Invalid <sawOutput>: at most %zu extra <mapping> entries are supported.",
                    kMaxMappings - 2);
                gSawOutput = {};
                return false;
            }

            return true;
        }

        // --------------------------------------------------------------------- //
        void AttachSawOutputHook()
        {
            if (!gSawOutput.enabled || gSawOutput.attached)
                return;

            Redshark::LogInfo("AttachSawOutputHook: checkpoint A - resolving module base.");

            gSawOutput.moduleBase =
                reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (gSawOutput.moduleBase == 0)
            {
                Redshark::ShowErrorMessage(
                    "Can't attach saw output hook: main module base is null.");
                return;
            }

            Redshark::LogInfo(
                "AttachSawOutputHook: checkpoint B - module base 0x%llx, checking selector bytes.",
                static_cast<unsigned long long>(gSawOutput.moduleBase));

            gSawOutput.selectorAddress = gSawOutput.moduleBase + kSelectorRva;
            if (!MatchesSawOutputSelector(gSawOutput.selectorAddress))
            {
                Redshark::ShowErrorMessage(
                    "Can't attach saw output hook: expected code was not found at +0x39E761.");
                return;
            }

            Redshark::LogInfo("AttachSawOutputHook: checkpoint C - selector matched, building trampoline.");

            if (!BuildTrampoline())
            {
                Redshark::ShowErrorMessage(
                    "Can't attach saw output hook: trampoline allocation failed.");
                return;
            }

            Redshark::LogInfo(
                "AttachSawOutputHook: checkpoint D - trampoline built (%zu bytes at %p), patching selector.",
                gSawOutput.trampolineSize,
                gSawOutput.trampoline);

            const auto patch = MakeSelectorPatch();
            if (!WriteExecutableMemory(
                    gSawOutput.selectorAddress,
                    patch.data(),
                    patch.size()))
            {
                VirtualFree(gSawOutput.trampoline, 0, MEM_RELEASE);
                gSawOutput.trampoline = nullptr;
                gSawOutput.trampolineSize = 0;
                Redshark::ShowErrorMessage(
                    "Can't attach saw output hook: selector memory is not writable.");
                return;
            }

            Redshark::LogInfo("AttachSawOutputHook: checkpoint E - selector patched successfully.");

            gSawOutput.attached = true;
        }

        // --------------------------------------------------------------------- //
        void DetachSawOutputHook()
        {
            if (!gSawOutput.attached)
                return;

            if (!WriteExecutableMemory(
                    gSawOutput.selectorAddress,
                    kOriginalSelectorBytes.data(),
                    kOriginalSelectorBytes.size()))
            {
                Redshark::ShowErrorMessage(
                    "Can't detach saw output hook: original selector could not be restored.");
                return;
            }

            gSawOutput.attached = false;

            if (gSawOutput.trampoline != nullptr)
            {
                VirtualFree(gSawOutput.trampoline, 0, MEM_RELEASE);
                gSawOutput.trampoline = nullptr;
                gSawOutput.trampolineSize = 0;
            }
        }
    }
}
