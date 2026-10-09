// Tunnel inspect text for supported tunnels (2026-10-09). See hook_tunnel_inspect.h.

#include "hook_tunnel_inspect.h"
#include "core/tinyxml2.h"
#include "agri_log.h"
#include "daniel_types.h"
#include <Windows.h>

#include <array>
#include <cstdint>
#include <cstring>
#include <vector>

namespace Hooks
{
    namespace Engine
    {
        namespace
        {
            constexpr std::uintptr_t kPatchRva = 0x397AF5;          // lea rcx,[rsp+0x60]
            constexpr std::uintptr_t kContinueRva = 0x397B08;       // lea rcx,[rbp+0x10] (after mov edx,2555)
            constexpr std::uintptr_t kCalcTunnelDecayRva = 0x57CB50; // InspectHelper::calculateTunnelDecay(colPtr*, alt)
            constexpr std::uintptr_t kGetLevelRva = 0x4AEDD0;       // GeoColumn level lookup (col, out qword*, alt, int* idx)
            constexpr std::uint32_t kVanillaMessageId = 2555;

            // Complete instructions from +0x397AF5 through +0x397B07, byte-for-byte from the shipped exe.
            constexpr std::array<std::uint8_t, 19> kOriginalBytes = {
                0x48, 0x8D, 0x4C, 0x24, 0x60,       // lea  rcx,[rsp+0x60]
                0xE8, 0x51, 0x50, 0x1E, 0x00,       // call 0x14057CB50
                0x89, 0x44, 0x24, 0x30,             // mov  [rsp+0x30],eax
                0xBA, 0xFB, 0x09, 0x00, 0x00        // mov  edx,0x9FB (2555)
            };

            using CalcTunnelDecayFn = int(__fastcall*)(void* columnSharedPtr, std::uint16_t altitude);
            using GetLevelFn = std::uint64_t*(__fastcall*)(void* column, std::uint64_t* out, std::uint16_t altitude, int* index);

            struct TunnelInspectState
            {
                bool enabled = false;
                bool attached = false;
                std::uint32_t messageId = 5213;
                std::uintptr_t moduleBase = 0;
                std::uintptr_t patchAddress = 0;
                void* trampoline = nullptr;
                std::size_t trampolineSize = 0;
                CalcTunnelDecayFn calcTunnelDecay = nullptr;
                GetLevelFn getLevel = nullptr;
            };

            TunnelInspectState gTunnelInspect;

            // Called from the trampoline instead of the original call. columnSharedPtr points at
            // { GeoColumn*, refcount block } (consumed by calculateTunnelDecay, so read it first).
            std::uint32_t __fastcall InspectTunnelTile(void* columnSharedPtr, std::uint32_t altitude, int* outDays)
            {
                bool timbered = false;
                void* column = columnSharedPtr ? *reinterpret_cast<void**>(columnSharedPtr) : nullptr;
                if (column != nullptr)
                {
                    std::uint64_t level = 0;
                    gTunnelInspect.getLevel(column, &level, static_cast<std::uint16_t>(altitude), nullptr);
                    const std::uint8_t flags = static_cast<std::uint8_t>(level >> 24);
                    timbered = (flags & 0x60) != 0;   // 0x20 light timber (boards), 0x40 heavy timber (column)
                }

                *outDays = gTunnelInspect.calcTunnelDecay(columnSharedPtr, static_cast<std::uint16_t>(altitude));
                return timbered ? gTunnelInspect.messageId : kVanillaMessageId;
            }

            void AppendU64(std::vector<std::uint8_t>& code, std::uint64_t value)
            {
                for (unsigned shift = 0; shift < 64; shift += 8)
                    code.push_back(static_cast<std::uint8_t>(value >> shift));
            }

            bool WriteExecutableMemory(std::uintptr_t address, const void* bytes, std::size_t size)
            {
                DWORD oldProtection = 0;
                if (!VirtualProtect(reinterpret_cast<void*>(address), size, PAGE_EXECUTE_READWRITE, &oldProtection))
                    return false;
                std::memcpy(reinterpret_cast<void*>(address), bytes, size);
                FlushInstructionCache(GetCurrentProcess(), reinterpret_cast<const void*>(address), size);
                DWORD ignored = 0;
                VirtualProtect(reinterpret_cast<void*>(address), size, oldProtection, &ignored);
                return true;
            }

            // The trampoline runs inside the inspect function's frame (rsp 16-byte aligned, shadow space
            // present, exactly like the original call). It replaces: lea rcx / call calc / store days / mov edx.
            bool BuildTrampoline()
            {
                std::vector<std::uint8_t> code;
                // lea rcx,[rsp+0x60]        ; column shared ptr (edx = altitude, set at +0x397ACE, untouched)
                code.insert(code.end(), { 0x48, 0x8D, 0x4C, 0x24, 0x60 });
                // lea r8,[rsp+0x30]         ; &days (the original stores eax there)
                code.insert(code.end(), { 0x4C, 0x8D, 0x44, 0x24, 0x30 });
                // mov rax, InspectTunnelTile ; call rax
                code.insert(code.end(), { 0x48, 0xB8 });
                AppendU64(code, reinterpret_cast<std::uint64_t>(&InspectTunnelTile));
                code.insert(code.end(), { 0xFF, 0xD0 });
                // mov edx,eax               ; message id for the inspect message
                code.insert(code.end(), { 0x89, 0xC2 });
                // jmp qword ptr [rip+0] -> +0x397B08
                code.insert(code.end(), { 0xFF, 0x25, 0x00, 0x00, 0x00, 0x00 });
                AppendU64(code, gTunnelInspect.moduleBase + kContinueRva);

                void* mem = VirtualAlloc(nullptr, code.size(), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
                if (mem == nullptr)
                    return false;
                std::memcpy(mem, code.data(), code.size());
                DWORD old = 0;
                if (!VirtualProtect(mem, code.size(), PAGE_EXECUTE_READ, &old))
                {
                    VirtualFree(mem, 0, MEM_RELEASE);
                    return false;
                }
                FlushInstructionCache(GetCurrentProcess(), mem, code.size());
                gTunnelInspect.trampoline = mem;
                gTunnelInspect.trampolineSize = code.size();
                return true;
            }
        }

        bool ConfigureTunnelInspect(const tinyxml2::XMLElement* root)
        {
            if (gTunnelInspect.attached)
                return false;
            gTunnelInspect = {};
            if (root == nullptr)
                return false;
            const tinyxml2::XMLElement* section = root->FirstChildElement("tunnelInspect");
            if (section == nullptr)
                return true;
            gTunnelInspect.enabled = section->BoolAttribute("enabled", false);
            gTunnelInspect.messageId = section->UnsignedAttribute("messageId", 5213);
            return true;
        }

        void AttachTunnelInspectHook()
        {
            if (!gTunnelInspect.enabled || gTunnelInspect.attached)
                return;

            gTunnelInspect.moduleBase = reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (gTunnelInspect.moduleBase == 0)
            {
                Redshark::ShowErrorMessage("Can't attach tunnel inspect hook: main module base is null.");
                return;
            }
            gTunnelInspect.patchAddress = gTunnelInspect.moduleBase + kPatchRva;
            if (std::memcmp(reinterpret_cast<const void*>(gTunnelInspect.patchAddress), kOriginalBytes.data(), kOriginalBytes.size()) != 0)
            {
                Redshark::ShowErrorMessage("Can't attach tunnel inspect hook: expected code was not found at +0x397AF5.");
                return;
            }
            gTunnelInspect.calcTunnelDecay = reinterpret_cast<CalcTunnelDecayFn>(gTunnelInspect.moduleBase + kCalcTunnelDecayRva);
            gTunnelInspect.getLevel = reinterpret_cast<GetLevelFn>(gTunnelInspect.moduleBase + kGetLevelRva);

            if (!BuildTrampoline())
            {
                Redshark::ShowErrorMessage("Can't attach tunnel inspect hook: trampoline allocation failed.");
                return;
            }

            // jmp qword ptr [rip+0] -> trampoline (14 bytes), rest nop
            std::array<std::uint8_t, kOriginalBytes.size()> patch{};
            patch.fill(0x90);
            patch[0] = 0xFF; patch[1] = 0x25; patch[2] = 0x00; patch[3] = 0x00; patch[4] = 0x00; patch[5] = 0x00;
            const std::uint64_t dest = reinterpret_cast<std::uint64_t>(gTunnelInspect.trampoline);
            std::memcpy(patch.data() + 6, &dest, sizeof(dest));

            if (!WriteExecutableMemory(gTunnelInspect.patchAddress, patch.data(), patch.size()))
            {
                VirtualFree(gTunnelInspect.trampoline, 0, MEM_RELEASE);
                gTunnelInspect.trampoline = nullptr;
                Redshark::ShowErrorMessage("Can't attach tunnel inspect hook: code is not writable.");
                return;
            }
            gTunnelInspect.attached = true;
            Redshark::LogInfo("AttachTunnelInspectHook: patched +0x397AF5, supported tunnel tiles show message %u.", gTunnelInspect.messageId);
        }

        void DetachTunnelInspectHook()
        {
            if (!gTunnelInspect.attached)
                return;
            if (!WriteExecutableMemory(gTunnelInspect.patchAddress, kOriginalBytes.data(), kOriginalBytes.size()))
                return;
            gTunnelInspect.attached = false;
            if (gTunnelInspect.trampoline != nullptr)
            {
                VirtualFree(gTunnelInspect.trampoline, 0, MEM_RELEASE);
                gTunnelInspect.trampoline = nullptr;
            }
        }
    }
}
