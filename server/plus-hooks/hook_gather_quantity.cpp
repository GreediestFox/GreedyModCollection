// Random gather quantity per gatherable type (2026-10-09). See hook_gather_quantity.h.

#include "hook_gather_quantity.h"
#include "core/tinyxml2.h"
#include "agri_log.h"
#include "daniel_types.h"
#include <Windows.h>

#include <array>
#include <cstdint>
#include <cstring>
#include <map>
#include <random>
#include <vector>

namespace Hooks
{
    namespace Engine
    {
        namespace
        {
            constexpr std::uintptr_t kFunctionRva = 0x3797C0;   // Gathering::GetQuantitybyGatherType(uint type) -> byte
            constexpr std::uintptr_t kResumeRva = 0x3797CE;     // ja 0x3797FF (uses the flags of the stolen cmp)

            // Complete instructions from +0x3797C0 through +0x3797CD (no relative addressing).
            // NOTE: hook_crop_types rewrites the cmp immediate (+0x3797CA, the gatherable type limit). This hook therefore
            // attaches after it, checks only the opcodes below, and copies the CURRENT 14 bytes (with the raised limit)
            // into its trampoline.
            constexpr std::array<std::uint8_t, 14> kOriginalBytes = {
                0x89, 0x4C, 0x24, 0x08,             // mov dword ptr [rsp+8],ecx
                0x48, 0x83, 0xEC, 0x38,             // sub rsp,0x38
                0x81, 0xF9, 0xDA, 0x00, 0x00, 0x00  // cmp ecx,0xDA  (immediate may be raised by hook_crop_types)
            };
            constexpr std::size_t kOpcodeBytes = 10;      // mov + sub + "81 F9"; the 4-byte immediate is not checked
            std::array<std::uint8_t, 14> gStolenBytes{};  // the bytes actually found at attach time (restored on detach)

            using GetQuantityFn = std::uint32_t(__fastcall*)(std::uint32_t type);

            struct Range { std::uint32_t min; std::uint32_t max; };

            struct GatherQuantityState
            {
                bool enabled = false;
                bool attached = false;
                std::map<std::uint32_t, Range> ranges;
                std::uintptr_t functionAddress = 0;
                void* trampoline = nullptr;
                GetQuantityFn original = nullptr;
            };

            GatherQuantityState gGather;

            std::uint32_t __fastcall GetQuantityDetour(std::uint32_t type)
            {
                const auto it = gGather.ranges.find(type);
                if (it == gGather.ranges.end())
                    return gGather.original(type);
                thread_local std::mt19937 rng(std::random_device{}());
                std::uniform_int_distribution<std::uint32_t> dist(it->second.min, it->second.max);
                return dist(rng);
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

            std::array<std::uint8_t, 14> AbsoluteJump(std::uint64_t destination)
            {
                std::array<std::uint8_t, 14> jmp = { 0xFF, 0x25, 0x00, 0x00, 0x00, 0x00 };   // jmp qword ptr [rip+0]
                std::memcpy(jmp.data() + 6, &destination, sizeof(destination));
                return jmp;
            }

            // stolen instructions + jmp back to +0x3797CE
            bool BuildTrampoline(std::uintptr_t moduleBase)
            {
                std::vector<std::uint8_t> code(gStolenBytes.begin(), gStolenBytes.end());
                const auto back = AbsoluteJump(moduleBase + kResumeRva);
                code.insert(code.end(), back.begin(), back.end());

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
                gGather.trampoline = mem;
                gGather.original = reinterpret_cast<GetQuantityFn>(mem);
                return true;
            }
        }

        bool ConfigureGatherQuantity(const tinyxml2::XMLElement* root)
        {
            if (gGather.attached)
                return false;
            gGather = {};
            if (root == nullptr)
                return false;
            const tinyxml2::XMLElement* section = root->FirstChildElement("gatherQuantity");
            if (section == nullptr)
                return true;
            gGather.enabled = section->BoolAttribute("enabled", false);
            if (!gGather.enabled)
                return true;

            for (const tinyxml2::XMLElement* r = section->FirstChildElement("range"); r != nullptr; r = r->NextSiblingElement("range"))
            {
                const unsigned type = r->UnsignedAttribute("type", 9999);
                unsigned lo = r->UnsignedAttribute("min", 0);
                unsigned hi = r->UnsignedAttribute("max", 0);
                if (type > 0xDA || lo < 1 || hi < lo || hi > 255)
                {
                    Redshark::ShowErrorMessage("Invalid <gatherQuantity><range>: type 0..218, 1 <= min <= max <= 255.");
                    gGather = {};
                    return false;
                }
                gGather.ranges[type] = { lo, hi };
            }
            if (gGather.ranges.empty())
                gGather.enabled = false;
            return true;
        }

        void AttachGatherQuantityHook()
        {
            if (!gGather.enabled || gGather.attached)
                return;

            const auto moduleBase = reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (moduleBase == 0)
                return;
            gGather.functionAddress = moduleBase + kFunctionRva;
            if (std::memcmp(reinterpret_cast<const void*>(gGather.functionAddress), kOriginalBytes.data(), kOpcodeBytes) != 0)
            {
                Redshark::ShowErrorMessage("Can't attach gather quantity hook: expected code was not found at +0x3797C0.");
                return;
            }
            std::memcpy(gStolenBytes.data(), reinterpret_cast<const void*>(gGather.functionAddress), gStolenBytes.size());
            if (!BuildTrampoline(moduleBase))
            {
                Redshark::ShowErrorMessage("Can't attach gather quantity hook: trampoline allocation failed.");
                return;
            }
            const auto patch = AbsoluteJump(reinterpret_cast<std::uint64_t>(&GetQuantityDetour));
            if (!WriteExecutableMemory(gGather.functionAddress, patch.data(), patch.size()))
            {
                VirtualFree(gGather.trampoline, 0, MEM_RELEASE);
                gGather.trampoline = nullptr;
                Redshark::ShowErrorMessage("Can't attach gather quantity hook: code is not writable.");
                return;
            }
            gGather.attached = true;
            for (const auto& [type, range] : gGather.ranges)
                Redshark::LogInfo("AttachGatherQuantityHook: gatherable type %u -> %u..%u per gather.", type, range.min, range.max);
        }

        void DetachGatherQuantityHook()
        {
            if (!gGather.attached)
                return;
            if (!WriteExecutableMemory(gGather.functionAddress, gStolenBytes.data(), gStolenBytes.size()))
                return;
            gGather.attached = false;
            if (gGather.trampoline != nullptr)
            {
                VirtualFree(gGather.trampoline, 0, MEM_RELEASE);
                gGather.trampoline = nullptr;
            }
        }
    }
}
