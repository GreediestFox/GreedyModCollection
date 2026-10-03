/* ===================================================================================
	Copyright (c) 2026, LiFx Contributors. All Rights Reserved.

	This file is a part of LiFx.

	LIFX IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
	MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
	IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
	DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
	ARISING FROM, OUT OF OR IN CONNECTION WITH LIFX OR THE USE OR OTHER
	DEALINGS IN LIFX.
*  =================================================================================== *//*
	Craft item effect id range. See hook_craft_effects_range.h for the design.
*/

#include "hook_craft_effects_range.h"

#include "core/tinyxml2.h"
#include "server/api/t3d_console.h"

#include <Windows.h>

#include <cstdarg>
#include <cstdint>
#include <cstdio>
#include <cstring>
#include <ctime>

namespace Hooks
{
    namespace Engine
    {
        namespace
        {
            constexpr std::uintptr_t kIdCheck = 0x4DAC33;      // 83 F9 25        cmp ecx, 25h      (id - 1 <= 37)
            constexpr std::uintptr_t kIndexCheck = 0x4DAC3F;   // 40 80 FF 26     cmp dil, 26h      (index < 38)
            constexpr std::uint8_t kIdCheckOld[] = { 0x83, 0xF9, 0x25 };
            constexpr std::uint8_t kIndexCheckOld[] = { 0x40, 0x80, 0xFF, 0x26 };
            constexpr std::uint8_t kNewMaxIndex = 0x7E;        // ids 1..127 (index 0..126)

            bool g_enabled = false;
            bool g_attached = false;

            void FileLog(const char* fmt, ...)
            {
                char msg[400];
                va_list args;
                va_start(args, fmt);
                vsnprintf(msg, sizeof(msg), fmt, args);
                va_end(args);
                std::time_t now = std::time(nullptr);
                std::tm tmv{};
                localtime_s(&tmv, &now);
                char stamp[32];
                std::strftime(stamp, sizeof(stamp), "%Y-%m-%d %H:%M:%S", &tmv);
                std::FILE* f = nullptr;
                if (fopen_s(&f, "logs/craft_effects_range.log", "a") == 0 && f)
                {
                    std::fprintf(f, "%s %s\n", stamp, msg);
                    std::fclose(f);
                }
            }

            bool WriteByte(std::uintptr_t addr, std::uint8_t value)
            {
                DWORD old = 0;
                if (!VirtualProtect(reinterpret_cast<void*>(addr), 1, PAGE_EXECUTE_READWRITE, &old))
                    return false;
                *reinterpret_cast<std::uint8_t*>(addr) = value;
                DWORD dummy = 0;
                VirtualProtect(reinterpret_cast<void*>(addr), 1, old, &dummy);
                FlushInstructionCache(GetCurrentProcess(), reinterpret_cast<void*>(addr), 1);
                return true;
            }
        }

        bool ConfigureCraftEffectsRange(const tinyxml2::XMLElement* root)
        {
            g_enabled = false;
            if (root == nullptr)
                return true;
            const tinyxml2::XMLElement* section = root->FirstChildElement("craftEffectsRange");
            if (section == nullptr || !section->BoolAttribute("enabled", false))
                return true;
            g_enabled = true;
            return true;
        }

        void AttachCraftEffectsRangeHook()
        {
            if (!g_enabled || g_attached)
                return;
            const std::uintptr_t base = reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (base == 0)
                return;

            // both sites are checked first: all or none, so a different server build cannot be half patched
            if (std::memcmp(reinterpret_cast<const void*>(base + kIdCheck), kIdCheckOld, sizeof(kIdCheckOld)) != 0
                || std::memcmp(reinterpret_cast<const void*>(base + kIndexCheck), kIndexCheckOld, sizeof(kIndexCheckOld)) != 0)
            {
                FileLog("expected code not found at %p / %p - not patching", reinterpret_cast<void*>(base + kIdCheck), reinterpret_cast<void*>(base + kIndexCheck));
                Lifx::ShowErrorMessage("Can't apply the craft effect id range change: expected code not found (different server build?).");
                return;
            }

            if (!WriteByte(base + kIdCheck + 2, kNewMaxIndex) || !WriteByte(base + kIndexCheck + 3, kNewMaxIndex + 1))
            {
                Lifx::ShowErrorMessage("Craft effect id range change failed (VirtualProtect).");
                return;
            }

            g_attached = true;
            FileLog("item_effects.xml effect ids widened: 1..38 -> 1..%u", kNewMaxIndex + 1);
        }
    }
}
