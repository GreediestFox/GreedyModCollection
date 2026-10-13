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
*  =================================================================================== */

/*
	Animal fodder. See hook_animal_fodder.h for the design.
*/

#include "hook_animal_fodder.h"

#include "core/tinyxml2.h"
#include "server/api/t3d_console.h"

#include <Windows.h>
#include <detours.h>

#include <atomic>
#include <cstdarg>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <ctime>
#include <mutex>
#include <string>
#include <vector>

__CM_INSTATNTIATE(_Fodder_IsFeedRec);
__CM_INSTATNTIATE(_Fodder_IsFeedType);
__CM_INSTATNTIATE(_Fodder_BornAnimal);
__CM_INSTATNTIATE(_Fodder_SlaughterAdd);
__CM_INSTATNTIATE(_Fodder_GameTime);

namespace Hooks
{
    namespace Engine
    {
        namespace
        {
            constexpr std::uintptr_t kIsFeedRecRva = 0x1B0E70;
            constexpr std::uintptr_t kIsFeedTypeRva = 0x1B0E10;
            constexpr std::uintptr_t kBornAnimalRva = 0x1AC2E0;
            constexpr std::uintptr_t kSlaughterAddRva = 0x1ABD00;
            constexpr std::uintptr_t kGameTimeRva = 0x468760;
            constexpr std::uintptr_t kGrowthInc1Rva = 0x1B33AC;   // INC R8B  (41 FE C0): quality + 1
            constexpr std::uintptr_t kGrowthInc2Rva = 0x1B33FC;   // INC R8B  (41 FE C0): quality + 1

            // Inventory wrapper -> +0x48 map -> +0x30 bucket array, +0x38 bucket count; node: [0] next, [1] key, [2] item;
            // item: +0x18 type record (id at +8), +0x20 quality (u16), +0x24 quantity.
            constexpr std::size_t kInvMapOffset = 0x48;
            constexpr std::size_t kMapBucketsOffset = 0x30;
            constexpr std::size_t kMapCountOffset = 0x38;
            constexpr std::size_t kItemRecordOffset = 0x18;
            constexpr std::size_t kRecordIdOffset = 0x08;
            constexpr std::size_t kItemQuantityOffset = 0x24;

            struct Fodder
            {
                U32 itemTypeId = 0;
                std::vector<U32> animals;
            };

            struct State
            {
                std::vector<Fodder> fodders;
                std::vector<int> months;       // in-game months (1..12) in which the slaughter bonus applies
                int newbornBonus = 0;
                int slaughterBonus = 0;
                bool disableFeedGrowth = false;
                bool enabled = false;
                bool attached = false;
                bool verbose = true;
                std::uintptr_t base = 0;
                std::mutex logMutex;
                std::atomic<int> cachedMonth{0};
                std::atomic<std::uint64_t> cachedAtMs{0};
            };

            State& S()
            {
                static State* state = new State();   // intentionally leaked (hooks may fire during shutdown)
                return *state;
            }

            void FileLog(const char* fmt, ...)
            {
                State& s = S();
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

                std::lock_guard<std::mutex> lock(s.logMutex);
                std::FILE* f = nullptr;
                if (fopen_s(&f, "logs/animal_fodder.log", "a") == 0 && f)
                {
                    std::fprintf(f, "%s %s\n", stamp, msg);
                    std::fclose(f);
                }
            }

            bool MatchBytes(std::uintptr_t addr, const std::uint8_t* bytes, std::size_t n)
            {
                return std::memcmp(reinterpret_cast<const void*>(addr), bytes, n) == 0;
            }

            bool IsFodderType(U32 typeId)
            {
                for (const Fodder& f : S().fodders)
                    if (f.itemTypeId == typeId)
                        return true;
                return false;
            }

            // The fodder item type that belongs to this species (0 = none).
            U32 FodderForAnimal(U32 animalTypeId)
            {
                for (const Fodder& f : S().fodders)
                    for (U32 a : f.animals)
                        if (a == animalTypeId)
                            return f.itemTypeId;
                return 0;
            }

            // POD-only helpers (they contain __try).
            // Reads the item type id from an item type record.
            bool ReadRecordId(LPVOID record, U32* out)
            {
                __try
                {
                    *out = *reinterpret_cast<const U32*>(static_cast<const std::uint8_t*>(record) + kRecordIdOffset);
                    return true;
                }
                __except (EXCEPTION_EXECUTE_HANDLER)
                {
                    return false;
                }
            }

            // True when the inventory holds at least one item of the given type.
            bool InventoryHasType(LPVOID inventory, U32 wantType, int* errorOut)
            {
                *errorOut = 0;
                __try
                {
                    const auto* inv = static_cast<const std::uint8_t*>(inventory);
                    const auto* map = *reinterpret_cast<const std::uint8_t* const*>(inv + kInvMapOffset);
                    if (map == nullptr)
                        return false;
                    const std::uint64_t count = *reinterpret_cast<const std::uint64_t*>(map + kMapCountOffset);
                    const auto* buckets = *reinterpret_cast<const std::uint64_t* const*>(map + kMapBucketsOffset);
                    if (buckets == nullptr || count == 0 || count > 1000000)
                        return false;
                    std::uint64_t guard = 0;
                    for (std::uint64_t b = 0; b < count; ++b)
                    {
                        const auto* node = reinterpret_cast<const std::uint64_t*>(buckets[b]);
                        while (node != nullptr)
                        {
                            const auto* item = reinterpret_cast<const std::uint8_t*>(node[2]);
                            if (item != nullptr)
                            {
                                const auto* rec = *reinterpret_cast<const std::uint8_t* const*>(item + kItemRecordOffset);
                                if (rec != nullptr && *reinterpret_cast<const U32*>(rec + kRecordIdOffset) == wantType
                                    && *reinterpret_cast<const U32*>(item + kItemQuantityOffset) > 0)
                                    return true;
                            }
                            node = reinterpret_cast<const std::uint64_t*>(node[0]);
                            if (++guard > 200000)
                                return false;
                        }
                    }
                    return false;
                }
                __except (EXCEPTION_EXECUTE_HANDLER)
                {
                    *errorOut = 1;
                    return false;
                }
            }

            // In-game month (1..12) from getGameTime(), 0 when unknown.
            int ReadGameMonth()
            {
                __try
                {
                    const char* t = _Fodder_GameTime();
                    if (t != nullptr && std::strlen(t) >= 10 && t[2] == '-' && t[5] == '-')
                    {
                        const int m = (t[3] - '0') * 10 + (t[4] - '0');
                        if (m >= 1 && m <= 12)
                            return m;
                    }
                }
                __except (EXCEPTION_EXECUTE_HANDLER)
                {
                }
                return 0;
            }

            bool InSlaughterWindow()
            {
                State& s = S();
                const std::uint64_t now = GetTickCount64();
                const std::uint64_t at = s.cachedAtMs.load();
                int month = s.cachedMonth.load();
                if (at == 0 || now - at > 10000)
                {
                    month = ReadGameMonth();
                    s.cachedMonth.store(month);
                    s.cachedAtMs.store(now);
                    static int lastLogged = -1;
                    if (month != lastLogged)
                    {
                        lastLogged = month;
                        FileLog("in-game month is now %d", month);
                    }
                }
                if (month == 0)
                    return false;
                for (int m : s.months)
                    if (m == month)
                        return true;
                return false;
            }

            bool PatchNop3(std::uintptr_t addr)
            {
                static const std::uint8_t incR8b[] = { 0x41, 0xFE, 0xC0 };
                static const std::uint8_t nop3[] = { 0x0F, 0x1F, 0x00 };
                if (!MatchBytes(addr, incR8b, sizeof(incR8b)))
                    return false;
                DWORD old = 0;
                if (!VirtualProtect(reinterpret_cast<LPVOID>(addr), sizeof(nop3), PAGE_EXECUTE_READWRITE, &old))
                    return false;
                std::memcpy(reinterpret_cast<void*>(addr), nop3, sizeof(nop3));
                FlushInstructionCache(GetCurrentProcess(), reinterpret_cast<LPCVOID>(addr), sizeof(nop3));
                DWORD tmp = 0;
                VirtualProtect(reinterpret_cast<LPVOID>(addr), sizeof(nop3), old, &tmp);
                return true;
            }

            U32 Capped(U32 quality, int bonus)
            {
                const U32 q = quality + static_cast<U32>(bonus);
                return q > 100 ? (quality > 100 ? quality : 100) : q;
            }
        }

        // --------------------------------------------------------------------- //
        unsigned char __fastcall OnFodderIsFeedRec(LPVOID mgr, LPVOID typeRecord)
        {
            U32 id = 0;
            if (typeRecord != nullptr && ReadRecordId(typeRecord, &id) && IsFodderType(id))
                return 1;
            return _Fodder_IsFeedRec(mgr, typeRecord);
        }

        unsigned char __fastcall OnFodderIsFeedType(LPVOID mgr, U32 itemTypeId)
        {
            if (IsFodderType(itemTypeId))
                return 1;
            return _Fodder_IsFeedType(mgr, itemTypeId);
        }

        void __fastcall OnFodderBornAnimal(LPVOID stable, LPVOID inventory, U32 animalTypeId, U32 quality)
        {
            State& s = S();
            U32 q = quality;
            if (s.newbornBonus > 0)
            {
                const U32 fodder = FodderForAnimal(animalTypeId);
                if (fodder != 0 && inventory != nullptr)
                {
                    int err = 0;
                    const bool has = InventoryHasType(inventory, fodder, &err);
                    static long logged = 0;
                    if (err != 0 && logged < 5)
                    {
                        ++logged;
                        FileLog("newborn: inventory scan failed (access violation), no bonus");
                    }
                    if (has)
                    {
                        q = Capped(quality, s.newbornBonus);
                        static long bonusLogged = 0;
                        if (s.verbose && bonusLogged < 30)
                        {
                            ++bonusLogged;
                            FileLog("newborn animal type %u: fodder %u in stable, quality %u -> %u", animalTypeId, fodder, quality, q);
                        }
                    }
                    else
                    {
                        static long plainLogged = 0;
                        if (s.verbose && plainLogged < 30)
                        {
                            ++plainLogged;
                            FileLog("newborn animal type %u: no fodder %u in stable, quality %u", animalTypeId, fodder, quality);
                        }
                    }
                }
            }
            _Fodder_BornAnimal(stable, inventory, animalTypeId, q);
        }

        void __fastcall OnFodderSlaughterAdd(LPVOID mgr, LPVOID inventory, U32 itemTypeId, U32 quantity, U32 quality, LPVOID extra)
        {
            State& s = S();
            U32 q = quality;
            if (s.slaughterBonus > 0 && InSlaughterWindow())
            {
                q = Capped(quality, s.slaughterBonus);
                static long logged = 0;
                if (s.verbose && logged < 30)
                {
                    ++logged;
                    FileLog("slaughter product type %u x%u: quality %u -> %u (slaughter window)", itemTypeId, quantity, quality, q);
                }
            }
            _Fodder_SlaughterAdd(mgr, inventory, itemTypeId, quantity, q, extra);
        }

        // --------------------------------------------------------------------- //
        namespace
        {
            void ParseIdList(const char* text, std::vector<U32>& out)
            {
                const char* p = text;
                while (p != nullptr && *p)
                {
                    char* end = nullptr;
                    const unsigned long id = std::strtoul(p, &end, 10);
                    if (end == p)
                    {
                        ++p;
                        continue;
                    }
                    out.push_back(static_cast<U32>(id));
                    p = end;
                }
            }
        }

        bool ConfigureAnimalFodder(const tinyxml2::XMLElement* root)
        {
            State& s = S();
            if (s.attached)
            {
                Lifx::ShowErrorMessage("Can't configure animalFodder while the hook is attached.");
                return false;
            }
            s.enabled = false;
            s.fodders.clear();
            s.months.clear();
            s.newbornBonus = 0;
            s.slaughterBonus = 0;
            s.disableFeedGrowth = false;
            if (root == nullptr)
                return true;

            const tinyxml2::XMLElement* section = root->FirstChildElement("animalFodder");
            if (section == nullptr || !section->BoolAttribute("enabled", false))
                return true;

            s.verbose = section->BoolAttribute("verbose", true);
            s.disableFeedGrowth = section->BoolAttribute("disableFeedGrowth", false);
            s.newbornBonus = section->IntAttribute("newbornBonus", 0);
            s.slaughterBonus = section->IntAttribute("slaughterBonus", 0);
            if (s.newbornBonus < 0 || s.newbornBonus > 100 || s.slaughterBonus < 0 || s.slaughterBonus > 100)
            {
                Lifx::ShowErrorMessage("Invalid <animalFodder>: newbornBonus and slaughterBonus must be 0..100.");
                return false;
            }
            if (const char* months = section->Attribute("slaughterMonths"))
            {
                std::vector<U32> ids;
                ParseIdList(months, ids);
                for (U32 m : ids)
                {
                    if (m < 1 || m > 12)
                    {
                        Lifx::ShowErrorMessage("Invalid <animalFodder slaughterMonths>: %u is not a month (1..12).", m);
                        s.months.clear();
                        return false;
                    }
                    s.months.push_back(static_cast<int>(m));
                }
            }

            for (const tinyxml2::XMLElement* e = section->FirstChildElement("fodder"); e != nullptr; e = e->NextSiblingElement("fodder"))
            {
                Fodder f;
                unsigned id = 0;
                if (e->QueryUnsignedAttribute("itemTypeId", &id) != tinyxml2::XML_SUCCESS || id == 0)
                {
                    Lifx::ShowErrorMessage("Invalid <animalFodder><fodder>: itemTypeId is required.");
                    s.fodders.clear();
                    return false;
                }
                f.itemTypeId = id;
                if (const char* list = e->Attribute("animals"))
                    ParseIdList(list, f.animals);
                s.fodders.push_back(f);
            }

            s.enabled = !s.fodders.empty() || s.slaughterBonus > 0 || s.disableFeedGrowth;
            return true;
        }

        // --------------------------------------------------------------------- //
        void AttachAnimalFodderHook()
        {
            State& s = S();
            if (!s.enabled || s.attached)
                return;

            s.base = reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (s.base == 0)
            {
                Lifx::ShowErrorMessage("Can't attach animal-fodder hook: main module base is null.");
                return;
            }

            // mov ecx,[rdx+8]; lea eax,[rcx-160h]                     (isFeed(record))
            static constexpr std::uint8_t isFeedRecPrologue[] = { 0x8B, 0x4A, 0x08, 0x8D, 0x81, 0xA0, 0xFE, 0xFF, 0xFF };
            // sub rsp,28h; mov rcx,[rip+x]                            (isFeed(typeId))
            static constexpr std::uint8_t isFeedTypePrologue[] = { 0x48, 0x83, 0xEC, 0x28, 0x48, 0x8B, 0x0D };
            // mov rax,rsp; push rbp; lea rbp,[rax-5Fh]; sub rsp,0D0h  (bornAnimal)
            static constexpr std::uint8_t bornPrologue[] = { 0x48, 0x8B, 0xC4, 0x55, 0x48, 0x8D, 0x68, 0xA1, 0x48, 0x81, 0xEC, 0xD0, 0x00, 0x00, 0x00 };
            // mov [rsp+18h],rbp; push rsi; push rdi; push r14         (_slaughterAddProduct)
            static constexpr std::uint8_t slaughterPrologue[] = { 0x48, 0x89, 0x6C, 0x24, 0x18, 0x56, 0x57, 0x41, 0x56, 0x48, 0x83, 0xEC, 0x40 };
            // push rbp; push rbx; push rsi; push rdi; push r12..r15   (getGameTime body)
            static constexpr std::uint8_t gameTimePrologue[] = { 0x40, 0x55, 0x53, 0x56, 0x57, 0x41, 0x54, 0x41, 0x55, 0x41, 0x56, 0x41, 0x57 };

            const bool needFeed = !s.fodders.empty();
            const bool needBorn = s.newbornBonus > 0 && !s.fodders.empty();
            const bool needSlaughter = s.slaughterBonus > 0 && !s.months.empty();

            if ((needFeed && (!MatchBytes(s.base + kIsFeedRecRva, isFeedRecPrologue, sizeof(isFeedRecPrologue))
                              || !MatchBytes(s.base + kIsFeedTypeRva, isFeedTypePrologue, sizeof(isFeedTypePrologue))))
                || (needBorn && !MatchBytes(s.base + kBornAnimalRva, bornPrologue, sizeof(bornPrologue)))
                || (needSlaughter && (!MatchBytes(s.base + kSlaughterAddRva, slaughterPrologue, sizeof(slaughterPrologue))
                                      || !MatchBytes(s.base + kGameTimeRva, gameTimePrologue, sizeof(gameTimePrologue)))))
            {
                Lifx::ShowErrorMessage("Can't attach animal-fodder hook: expected code not found (different server build?).");
                return;
            }

            if (s.disableFeedGrowth)
            {
                if (PatchNop3(s.base + kGrowthInc1Rva) && PatchNop3(s.base + kGrowthInc2Rva))
                    FileLog("vanilla feed quality growth disabled (2 patch sites)");
                else
                {
                    Lifx::ShowErrorMessage("Can't disable the vanilla feed quality growth: expected code not found (different server build?).");
                    s.disableFeedGrowth = false;
                }
            }

            _Fodder_IsFeedRec = reinterpret_cast<_Fodder_IsFeedRec_Fn>(s.base + kIsFeedRecRva);
            _Fodder_IsFeedType = reinterpret_cast<_Fodder_IsFeedType_Fn>(s.base + kIsFeedTypeRva);
            _Fodder_BornAnimal = reinterpret_cast<_Fodder_BornAnimal_Fn>(s.base + kBornAnimalRva);
            _Fodder_SlaughterAdd = reinterpret_cast<_Fodder_SlaughterAdd_Fn>(s.base + kSlaughterAddRva);
            _Fodder_GameTime = reinterpret_cast<_Fodder_GameTime_Fn>(s.base + kGameTimeRva);

            LONG rc = NO_ERROR;
            if (needFeed)
            {
                rc = DetourAttach(&(PVOID&)_Fodder_IsFeedRec, OnFodderIsFeedRec);
                if (rc == NO_ERROR) rc = DetourAttach(&(PVOID&)_Fodder_IsFeedType, OnFodderIsFeedType);
            }
            if (rc == NO_ERROR && needBorn) rc = DetourAttach(&(PVOID&)_Fodder_BornAnimal, OnFodderBornAnimal);
            if (rc == NO_ERROR && needSlaughter) rc = DetourAttach(&(PVOID&)_Fodder_SlaughterAdd, OnFodderSlaughterAdd);
            if (rc != NO_ERROR)
            {
                Lifx::ShowErrorMessage("DetourAttach failed for the animal-fodder hooks. Error: %ld", rc);
                return;
            }

            s.attached = true;
            FileLog("hooks attached: %u fodder type(s) (feed %s), newborn bonus +%d (%s), slaughter bonus +%d (%s, %u month(s)), feed growth %s",
                    static_cast<unsigned>(s.fodders.size()), needFeed ? "on" : "off", s.newbornBonus, needBorn ? "on" : "off",
                    s.slaughterBonus, needSlaughter ? "on" : "off", static_cast<unsigned>(s.months.size()),
                    s.disableFeedGrowth ? "disabled" : "vanilla");
        }
    }
}
