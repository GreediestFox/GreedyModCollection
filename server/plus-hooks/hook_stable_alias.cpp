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
	Stable alias. See hook_stable_alias.h for the design.
*/

#include "hook_stable_alias.h"

#include "core/tinyxml2.h"
#include "server/api/t3d_console.h"

#include <Windows.h>
#include <detours.h>

#include <cstdarg>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <ctime>
#include <mutex>
#include <string>
#include <vector>

__CM_INSTATNTIATE(_Stable_IsStable);
__CM_INSTATNTIATE(_Stable_MaxAnimals);
__CM_INSTATNTIATE(_Stable_Param6);
__CM_INSTATNTIATE(_Stable_Param7);
__CM_INSTATNTIATE(_Stable_Param8);
__CM_INSTATNTIATE(_Stable_Product);
__CM_INSTATNTIATE(_Stable_IsAnimal);
__CM_INSTATNTIATE(_Stable_CheckLimits);

namespace Hooks
{
    namespace Engine
    {
        namespace
        {
            constexpr std::uintptr_t kIsStableRva = 0x1B0EB0;
            constexpr std::uintptr_t kMaxAnimalsRva = 0x1B0D60;
            constexpr std::uintptr_t kParam6Rva = 0x1B04D0;
            constexpr std::uintptr_t kParam7Rva = 0x1B0D20;
            constexpr std::uintptr_t kParam8Rva = 0x1B0410;
            constexpr std::uintptr_t kProductRva = 0x1B07E0;
            constexpr std::uintptr_t kIsAnimalRva = 0x1B0DA0;
            constexpr std::uintptr_t kCheckLimitsRva = 0x1AC440;
            constexpr std::uintptr_t kTableRva = 0xACEB40;        // stable parameter table (11 dwords per record)
            constexpr std::uintptr_t kSqlLiteralRva = 0x7A1210;   // " WHERE IsComplete =1 AND ObjectTypeID IN (133,134,143,144,517) AND ("
            constexpr std::size_t kSqlLiteralLen = 68;
            // ManageAnimals_Ability::checkEntity (RVA 0x354D90) ends in a cascade "type == 133/134/143/517/144 -> level check, else NO".
            // The Coop block (type 144, no level needed) is 18 bytes at 0x35502A:  cmp eax,90h ; jne fail ; mov bl,1 ; jmp done
            constexpr std::uintptr_t kManageGateRva = 0x35502A;
            constexpr std::uintptr_t kManageSharedOkRva = 0x35501B;   // 'mov bl,1 ; jmp done' of the neighbouring block (type 517)

            struct Rule
            {
                U32 typeId = 0;
                U32 behavesLike = 0;           // 0 = the type keeps its own table record
                int maxAnimals = -1;           // -1 = keep the record's value
                int product = -1;              // -1 = keep; 0 = nothing to harvest
                std::vector<U32> allowAnimals; // empty = no species restriction
                bool strict = false;           // true: EVERY item not in allowAnimals is refused (plain rabbit-only storage), not only animals
            };

            struct State
            {
                std::vector<Rule> rules;
                bool enabled = false;
                bool attached = false;
                bool verbose = true;
                bool anyAllow = false;
                std::uintptr_t base = 0;
                std::mutex logMutex;
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
                if (fopen_s(&f, "logs/stable_alias.log", "a") == 0 && f)
                {
                    std::fprintf(f, "%s %s\n", stamp, msg);
                    std::fclose(f);
                }
            }

            const Rule* FindRule(int typeId)
            {
                for (const Rule& r : S().rules)
                    if (static_cast<int>(r.typeId) == typeId)
                        return &r;
                return nullptr;
            }

            // TLS: item type of the move currently going through checkContainerLimits (0 = not inside it), and the verdict.
            thread_local U32 t_item = 0;
            thread_local bool t_denied = false;

            bool MatchBytes(std::uintptr_t addr, const std::uint8_t* bytes, std::size_t n)
            {
                return std::memcmp(reinterpret_cast<const void*>(addr), bytes, n) == 0;
            }

            // A table getter starts with `mov eax,[rip+disp32]` reading the first dword of the parameter table.
            bool IsTableGetter(std::uintptr_t fn, std::uintptr_t tableAddr)
            {
                const std::uint8_t* p = reinterpret_cast<const std::uint8_t*>(fn);
                if (p[0] != 0x8B || p[1] != 0x05)
                    return false;
                const std::int32_t disp = *reinterpret_cast<const std::int32_t*>(p + 2);
                return fn + 6 + disp == tableAddr;
            }

            // Lets an aliased type (behavesLike 144) pass the Manage ability's compiled-in building check exactly like the Coop:
            //   cmp eax,90h ; je shared_ok ; cmp eax,<typeId> ; jne fail      (same 18 bytes, reuses the neighbour's success block)
            bool PatchManageGate(U32 typeId)
            {
                static const std::uint8_t oldBytes[18] = { 0x3D, 0x90, 0x00, 0x00, 0x00, 0x0F, 0x85, 0x32, 0xFF, 0xFF, 0xFF, 0xB3, 0x01, 0xE9, 0x2D, 0xFF, 0xFF, 0xFF };
                static const std::uint8_t sharedOk[7] = { 0xB3, 0x01, 0xE9, 0x47, 0xFF, 0xFF, 0xFF };
                const std::uintptr_t addr = S().base + kManageGateRva;
                if (!MatchBytes(addr, oldBytes, sizeof(oldBytes)) || !MatchBytes(S().base + kManageSharedOkRva, sharedOk, sizeof(sharedOk)))
                {
                    Lifx::ShowErrorMessage("Can't patch the Manage gate for type %u: expected code not found (different server build?).", typeId);
                    return false;
                }
                std::uint8_t patch[18] = { 0x3D, 0x90, 0x00, 0x00, 0x00,                 // cmp eax,90h
                                           0x74, 0xEA,                                   // je  kManageSharedOkRva
                                           0x3D, 0, 0, 0, 0,                             // cmp eax,<typeId>
                                           0x0F, 0x85, 0x2B, 0xFF, 0xFF, 0xFF };         // jne fail (old jne target)
                std::memcpy(patch + 8, &typeId, 4);
                DWORD old = 0;
                if (!VirtualProtect(reinterpret_cast<LPVOID>(addr), sizeof(patch), PAGE_EXECUTE_READWRITE, &old))
                {
                    Lifx::ShowErrorMessage("Can't patch the Manage gate: VirtualProtect failed (%lu).", GetLastError());
                    return false;
                }
                std::memcpy(reinterpret_cast<void*>(addr), patch, sizeof(patch));
                FlushInstructionCache(GetCurrentProcess(), reinterpret_cast<LPCVOID>(addr), sizeof(patch));
                DWORD tmp = 0;
                VirtualProtect(reinterpret_cast<LPVOID>(addr), sizeof(patch), old, &tmp);
                FileLog("Manage gate patched: building type %u is accepted like the Coop (no level requirement)", typeId);
                return true;
            }

            // Rewrites the boot query's type list in place. Returns the number of extra ids that fit.
            bool PatchSqlLiteral(const std::vector<U32>& extra)
            {
                const std::uintptr_t addr = S().base + kSqlLiteralRva;
                static const char original[] = " WHERE IsComplete =1 AND ObjectTypeID IN (133,134,143,144,517) AND (";
                static_assert(sizeof(original) - 1 == kSqlLiteralLen, "literal length");
                if (std::memcmp(reinterpret_cast<const void*>(addr), original, kSqlLiteralLen + 1) != 0)
                {
                    Lifx::ShowErrorMessage("Can't patch the stable boot query: expected text not found (different server build?).");
                    return false;
                }
                std::string ids = "133,134,143,144,517";
                for (U32 t : extra)
                    ids += "," + std::to_string(t);
                const std::string head = " WHERE";
                const std::string mid = " IsComplete AND ObjectTypeID IN(" + ids + ")AND(";
                if (head.size() + mid.size() > kSqlLiteralLen)
                {
                    Lifx::ShowErrorMessage("Can't patch the stable boot query: too many aliased stable types (%u extra ids do not fit in %u bytes).",
                                           static_cast<unsigned>(extra.size()), static_cast<unsigned>(kSqlLiteralLen));
                    return false;
                }
                std::string text = head + std::string(kSqlLiteralLen - head.size() - mid.size(), ' ') + mid;
                DWORD old = 0;
                if (!VirtualProtect(reinterpret_cast<LPVOID>(addr), kSqlLiteralLen + 1, PAGE_READWRITE, &old))
                {
                    Lifx::ShowErrorMessage("Can't patch the stable boot query: VirtualProtect failed (%lu).", GetLastError());
                    return false;
                }
                std::memcpy(reinterpret_cast<void*>(addr), text.c_str(), kSqlLiteralLen);   // the terminating NUL is already there
                DWORD tmp = 0;
                VirtualProtect(reinterpret_cast<LPVOID>(addr), kSqlLiteralLen + 1, old, &tmp);
                FileLog("boot query rewritten: \"%s\"", text.c_str());
                return true;
            }
        }

        // --------------------------------------------------------------------- //
        unsigned char __fastcall OnStableIsStable(LPVOID mgr, int typeId)
        {
            const Rule* r = FindRule(typeId);
            if (r != nullptr)
            {
                // Species rule, evaluated for the container's real type inside checkContainerLimits.
                if (t_item != 0 && !r->allowAnimals.empty() && (r->strict || _Stable_IsAnimal(mgr, t_item) != 0))
                {
                    bool ok = false;
                    for (U32 a : r->allowAnimals)
                        if (a == t_item)
                            ok = true;
                    if (!ok)
                    {
                        t_denied = true;
                        static long logged = 0;
                        if (S().verbose && logged < 20)
                        {
                            ++logged;
                            FileLog("refused: item type %u is not allowed in building type %d", t_item, typeId);
                        }
                    }
                }
                if (r->behavesLike != 0)
                    typeId = static_cast<int>(r->behavesLike);
            }
            return _Stable_IsStable(mgr, typeId);
        }

        U32 __fastcall OnStableMaxAnimals(LPVOID mgr, int typeId)
        {
            const Rule* r = FindRule(typeId);
            if (r != nullptr)
            {
                if (r->maxAnimals >= 0)
                    return static_cast<U32>(r->maxAnimals);
                if (r->behavesLike != 0)
                    typeId = static_cast<int>(r->behavesLike);
            }
            return _Stable_MaxAnimals(mgr, typeId);
        }

        U32 __fastcall OnStableParam6(LPVOID mgr, int typeId)
        {
            const Rule* r = FindRule(typeId);
            return _Stable_Param6(mgr, (r != nullptr && r->behavesLike != 0) ? static_cast<int>(r->behavesLike) : typeId);
        }

        U32 __fastcall OnStableParam7(LPVOID mgr, int typeId)
        {
            const Rule* r = FindRule(typeId);
            return _Stable_Param7(mgr, (r != nullptr && r->behavesLike != 0) ? static_cast<int>(r->behavesLike) : typeId);
        }

        U32 __fastcall OnStableParam8(LPVOID mgr, int typeId)
        {
            const Rule* r = FindRule(typeId);
            return _Stable_Param8(mgr, (r != nullptr && r->behavesLike != 0) ? static_cast<int>(r->behavesLike) : typeId);
        }

        U32 __fastcall OnStableProduct(LPVOID mgr, int typeId)
        {
            const Rule* r = FindRule(typeId);
            if (r != nullptr)
            {
                if (r->product >= 0)
                    return static_cast<U32>(r->product);
                if (r->behavesLike != 0)
                    typeId = static_cast<int>(r->behavesLike);
            }
            return _Stable_Product(mgr, typeId);
        }

        unsigned char __fastcall OnStableCheckLimits(LPVOID mgr, LPVOID objectId, U32 itemTypeId, U32 quantity, LPVOID container)
        {
            if (!S().anyAllow)
                return _Stable_CheckLimits(mgr, objectId, itemTypeId, quantity, container);

            const U32 prevItem = t_item;
            const bool prevDenied = t_denied;
            t_item = itemTypeId;
            t_denied = false;
            unsigned char ok = _Stable_CheckLimits(mgr, objectId, itemTypeId, quantity, container);
            const bool denied = t_denied;
            t_item = prevItem;
            t_denied = prevDenied;
            return denied ? 0 : ok;
        }

        // --------------------------------------------------------------------- //
        bool ConfigureStableAlias(const tinyxml2::XMLElement* root)
        {
            State& s = S();
            if (s.attached)
            {
                Lifx::ShowErrorMessage("Can't configure stableAlias while the hook is attached.");
                return false;
            }
            s.enabled = false;
            s.anyAllow = false;
            s.rules.clear();
            if (root == nullptr)
                return true;

            const tinyxml2::XMLElement* section = root->FirstChildElement("stableAlias");
            if (section == nullptr || !section->BoolAttribute("enabled", false))
                return true;
            s.verbose = section->BoolAttribute("verbose", true);

            for (const tinyxml2::XMLElement* e = section->FirstChildElement("stable"); e != nullptr; e = e->NextSiblingElement("stable"))
            {
                Rule r;
                unsigned typeId = 0, like = 0;
                if (e->QueryUnsignedAttribute("objectTypeId", &typeId) != tinyxml2::XML_SUCCESS || typeId == 0)
                {
                    Lifx::ShowErrorMessage("Invalid <stableAlias><stable>: objectTypeId is required.");
                    s.rules.clear();
                    return false;
                }
                r.typeId = typeId;
                if (e->QueryUnsignedAttribute("behavesLike", &like) == tinyxml2::XML_SUCCESS)
                {
                    if (like != 133 && like != 134 && like != 143 && like != 144 && like != 517)
                    {
                        Lifx::ShowErrorMessage("Invalid <stable objectTypeId=%u>: behavesLike must be a vanilla stable type (133, 134, 143, 144 or 517).", typeId);
                        s.rules.clear();
                        return false;
                    }
                    r.behavesLike = like;
                }
                r.strict = e->BoolAttribute("strict", false);
                int v = 0;
                if (e->QueryIntAttribute("maxAnimals", &v) == tinyxml2::XML_SUCCESS && v >= 0)
                    r.maxAnimals = v;
                if (e->QueryIntAttribute("product", &v) == tinyxml2::XML_SUCCESS && v >= 0)
                    r.product = v;
                if (const char* list = e->Attribute("allowAnimals"))
                {
                    const char* p = list;
                    while (*p)
                    {
                        char* end = nullptr;
                        const unsigned long id = std::strtoul(p, &end, 10);
                        if (end == p)
                        {
                            ++p;
                            continue;
                        }
                        r.allowAnimals.push_back(static_cast<U32>(id));
                        p = end;
                    }
                    if (!r.allowAnimals.empty())
                        s.anyAllow = true;
                }
                s.rules.push_back(r);
            }
            if (s.rules.empty())
                return true;   // enabled but empty: nothing to do

            s.enabled = true;
            return true;
        }

        // --------------------------------------------------------------------- //
        void AttachStableAliasHook()
        {
            State& s = S();
            if (!s.enabled || s.attached)
                return;

            s.base = reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (s.base == 0)
            {
                Lifx::ShowErrorMessage("Can't attach stable-alias hook: main module base is null.");
                return;
            }

            const std::uintptr_t table = s.base + kTableRva;
            // push rdi; push r12; push r13; push r14; push r15; mov eax,1880h (checkContainerLimits)
            static constexpr std::uint8_t checkLimitsPrologue[] = { 0x40, 0x57, 0x41, 0x54, 0x41, 0x55, 0x41, 0x56, 0x41, 0x57, 0xB8, 0x80, 0x18, 0x00, 0x00 };
            if (!IsTableGetter(s.base + kIsStableRva, table) || !IsTableGetter(s.base + kMaxAnimalsRva, table)
                || !IsTableGetter(s.base + kParam6Rva, table) || !IsTableGetter(s.base + kParam7Rva, table)
                || !IsTableGetter(s.base + kParam8Rva, table) || !IsTableGetter(s.base + kProductRva, table)
                || !MatchBytes(s.base + kCheckLimitsRva, checkLimitsPrologue, sizeof(checkLimitsPrologue)))
            {
                Lifx::ShowErrorMessage("Can't attach stable-alias hook: expected code not found (different server build?).");
                return;
            }

            // Types that need to be loaded at boot: every rule with behavesLike whose id is not vanilla.
            std::vector<U32> extra;
            for (const Rule& r : s.rules)
                if (r.behavesLike != 0)
                    extra.push_back(r.typeId);
            if (!extra.empty() && !PatchSqlLiteral(extra))
                return;

            // A type that behaves like the Coop also has to pass the Manage ability's own building check (one type is supported).
            U32 coopLike = 0;
            for (const Rule& r : s.rules)
                if (r.behavesLike == 144 && r.typeId != 144)
                {
                    if (coopLike == 0)
                        coopLike = r.typeId;
                    else
                        FileLog("only one Coop-like type can be added to the Manage gate; type %u is not patched", r.typeId);
                }
            if (coopLike != 0 && !PatchManageGate(coopLike))
                return;

            _Stable_IsStable = reinterpret_cast<_Stable_IsStable_Fn>(s.base + kIsStableRva);
            _Stable_MaxAnimals = reinterpret_cast<_Stable_MaxAnimals_Fn>(s.base + kMaxAnimalsRva);
            _Stable_Param6 = reinterpret_cast<_Stable_Param6_Fn>(s.base + kParam6Rva);
            _Stable_Param7 = reinterpret_cast<_Stable_Param7_Fn>(s.base + kParam7Rva);
            _Stable_Param8 = reinterpret_cast<_Stable_Param8_Fn>(s.base + kParam8Rva);
            _Stable_Product = reinterpret_cast<_Stable_Product_Fn>(s.base + kProductRva);
            _Stable_IsAnimal = reinterpret_cast<_Stable_IsAnimal_Fn>(s.base + kIsAnimalRva);
            _Stable_CheckLimits = reinterpret_cast<_Stable_CheckLimits_Fn>(s.base + kCheckLimitsRva);

            LONG rc = DetourAttach(&(PVOID&)_Stable_IsStable, OnStableIsStable);
            if (rc == NO_ERROR) rc = DetourAttach(&(PVOID&)_Stable_MaxAnimals, OnStableMaxAnimals);
            if (rc == NO_ERROR) rc = DetourAttach(&(PVOID&)_Stable_Param6, OnStableParam6);
            if (rc == NO_ERROR) rc = DetourAttach(&(PVOID&)_Stable_Param7, OnStableParam7);
            if (rc == NO_ERROR) rc = DetourAttach(&(PVOID&)_Stable_Param8, OnStableParam8);
            if (rc == NO_ERROR) rc = DetourAttach(&(PVOID&)_Stable_Product, OnStableProduct);
            if (rc == NO_ERROR && s.anyAllow) rc = DetourAttach(&(PVOID&)_Stable_CheckLimits, OnStableCheckLimits);
            if (rc != NO_ERROR)
            {
                Lifx::ShowErrorMessage("DetourAttach failed for the stable-alias hooks. Error: %ld", rc);
                return;
            }

            s.attached = true;
            FileLog("hooks attached: %u stable rule(s), %u aliased type(s) added to the boot query, species rules %s",
                    static_cast<unsigned>(s.rules.size()), static_cast<unsigned>(extra.size()), s.anyAllow ? "on" : "off");
        }
    }
}
