/* ===================================================================================
	Copyright (c) 2026, LiFx Contributors. All Rights Reserved.
	Contributed by GreedyFox.

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
	Drink effects. See hook_drink_effects.h for the design.
*/

#include "hook_drink_effects.h"

#include "core/tinyxml2.h"
#include "server/api/t3d_console.h"

#include <Windows.h>
#include <detours/detours.h>

#include <cstdarg>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <ctime>
#include <mutex>
#include <string>
#include <vector>

__CM_INSTATNTIATE(_Drink_ApplyPotionItemEffect);

namespace Hooks
{
    namespace Engine
    {
        namespace
        {
            constexpr std::uintptr_t kApplyPotionRva = 0x1C4A40;
            constexpr std::uintptr_t kAddEffectRva = 0x1C3AA0;        // (fx, effectId, float value, int durationMs, int 0)
            constexpr std::uintptr_t kGetEffectsFeatureRva = 0x27C040; // (item) -> ItemEffectsFeature* or null
            constexpr std::uintptr_t kDurationConstARva = 0x736568;    // float 1000.0
            constexpr std::uintptr_t kDurationConstBRva = 0x73AB8C;    // float 15.0
            constexpr std::size_t kItemIdOffset = 0x08;
            constexpr std::size_t kItemQualityOffset = 0x20;           // u16

            struct EffectRef
            {
                unsigned effectId = 0;   // player effect id (cm_effects.xml)
                float magnitude = 0.0f;
            };

            struct Rule
            {
                U32 typeId = 0;
                std::vector<EffectRef> effects;
            };

            struct State
            {
                std::vector<Rule> rules;
                bool enabled = false;
                bool attached = false;
                bool verbose = true;
                bool dump = false;
                std::uintptr_t base = 0;

                std::string dbHost = "127.0.0.1";
                unsigned dbPort = 3306;
                std::string dbUser;
                std::string dbPassword;
                std::string dbName = "lif_1";

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
                char msg[900];
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
                if (fopen_s(&f, "logs/drink_effects.log", "a") == 0 && f)
                {
                    std::fprintf(f, "%s %s\n", stamp, msg);
                    std::fclose(f);
                }
            }

            bool PlausiblePtr(const void* p)
            {
                const std::uintptr_t v = reinterpret_cast<std::uintptr_t>(p);
                return v > 0x10000 && v < 0x00007FFFFFFF0000ull;
            }

            // ----------------------------------------------------------------- //
            // libmariadb (already loaded by the server), bound dynamically like the workshop buff.
            struct MariaApi
            {
                void* (__cdecl* init)(void*) = nullptr;
                void* (__cdecl* realConnect)(void*, const char*, const char*, const char*, const char*, unsigned int, const char*, unsigned long) = nullptr;
                int (__cdecl* query)(void*, const char*) = nullptr;
                void* (__cdecl* storeResult)(void*) = nullptr;
                char** (__cdecl* fetchRow)(void*) = nullptr;
                void (__cdecl* freeResult)(void*) = nullptr;
                void (__cdecl* close)(void*) = nullptr;
                const char* (__cdecl* error)(void*) = nullptr;
                int (__cdecl* options)(void*, int, const void*) = nullptr;
            };

            bool LoadMariaApi(MariaApi& api)
            {
                HMODULE lib = GetModuleHandleA("libmariadb.dll");
                if (!lib)
                    lib = LoadLibraryA("libmariadb.dll");
                if (!lib)
                    return false;
                auto get = [&](const char* name) { return GetProcAddress(lib, name); };
                api.init = reinterpret_cast<decltype(api.init)>(get("mysql_init"));
                api.realConnect = reinterpret_cast<decltype(api.realConnect)>(get("mysql_real_connect"));
                api.query = reinterpret_cast<decltype(api.query)>(get("mysql_query"));
                api.storeResult = reinterpret_cast<decltype(api.storeResult)>(get("mysql_store_result"));
                api.fetchRow = reinterpret_cast<decltype(api.fetchRow)>(get("mysql_fetch_row"));
                api.freeResult = reinterpret_cast<decltype(api.freeResult)>(get("mysql_free_result"));
                api.close = reinterpret_cast<decltype(api.close)>(get("mysql_close"));
                api.error = reinterpret_cast<decltype(api.error)>(get("mysql_error"));
                api.options = reinterpret_cast<decltype(api.options)>(get("mysql_options"));
                return api.init && api.realConnect && api.query && api.storeResult && api.fetchRow && api.freeResult && api.close && api.error;
            }

            // ObjectTypeID of an item row, 0 when unknown. One short-lived connection per call (drinks are rare).
            U32 LookupItemType(U32 itemId)
            {
                static MariaApi api;
                static bool loaded = false;
                static bool loadFailed = false;
                if (!loaded && !loadFailed)
                {
                    if (LoadMariaApi(api)) loaded = true; else { loadFailed = true; FileLog("libmariadb.dll or one of its functions is missing; no item type lookup"); }
                }
                if (!loaded)
                    return 0;

                State& s = S();
                void* conn = api.init(nullptr);
                if (!conn)
                    return 0;
                if (api.options)
                {
                    const unsigned int timeoutSeconds = 3;
                    api.options(conn, 0, &timeoutSeconds);
                    api.options(conn, 11, &timeoutSeconds);
                    api.options(conn, 12, &timeoutSeconds);
                }
                U32 type = 0;
                if (api.realConnect(conn, s.dbHost.c_str(), s.dbUser.c_str(), s.dbPassword.c_str(), s.dbName.c_str(), s.dbPort, nullptr, 0))
                {
                    char sql[96];
                    std::snprintf(sql, sizeof(sql), "SELECT ObjectTypeID FROM items WHERE ID=%u", itemId);
                    if (api.query(conn, sql) == 0)
                    {
                        if (void* res = api.storeResult(conn))
                        {
                            if (char** row = api.fetchRow(res))
                                if (row[0])
                                    type = static_cast<U32>(std::strtoul(row[0], nullptr, 10));
                            api.freeResult(res);
                        }
                    }
                    else
                        FileLog("item type query failed: %s", api.error(conn));
                }
                else
                    FileLog("db connect failed: %s", api.error(conn));
                api.close(conn);
                return type;
            }

            // ----------------------------------------------------------------- //
            // POD-only helper (contains __try): reads the plain fields of the item object.
            struct ItemProbe
            {
                U32 itemId = 0;
                U32 quality = 0;
                LPVOID feature = nullptr;
                U32 raw[24] = {};
                bool ok = false;
            };

            using GetFeatureFn = LPVOID(__fastcall*)(LPVOID item);

            void ProbeItem(LPVOID item, ItemProbe* out)
            {
                __try
                {
                    const auto* b = static_cast<const std::uint8_t*>(item);
                    out->itemId = *reinterpret_cast<const U32*>(b + kItemIdOffset);
                    out->quality = *reinterpret_cast<const std::uint16_t*>(b + kItemQualityOffset);
                    for (int i = 0; i < 24; ++i)
                        out->raw[i] = *reinterpret_cast<const U32*>(b + i * 4);
                    out->feature = reinterpret_cast<GetFeatureFn>(S().base + kGetEffectsFeatureRva)(item);
                    out->ok = true;
                }
                __except (EXCEPTION_EXECUTE_HANDLER)
                {
                    out->ok = false;
                }
            }

            using AddEffectFn = void(__fastcall*)(LPVOID fx, U32 effectId, float value, int durationMs, int zero);

            // POD-only (contains __try). Returns the number of effects added.
            int AddEffects(LPVOID fx, const std::vector<EffectRef>& effects, int durationMs)
            {
                int added = 0;
                __try
                {
                    auto add = reinterpret_cast<AddEffectFn>(S().base + kAddEffectRva);
                    for (const EffectRef& e : effects)
                    {
                        add(fx, e.effectId, e.magnitude, durationMs, 0);
                        ++added;
                    }
                }
                __except (EXCEPTION_EXECUTE_HANDLER)
                {
                    return -1;
                }
                return added;
            }

            int DurationFor(U32 quality)
            {
                const float a = *reinterpret_cast<const float*>(S().base + kDurationConstARva);
                const float b = *reinterpret_cast<const float*>(S().base + kDurationConstBRva);
                const float q = static_cast<float>(quality);
                return static_cast<int>((q * a * q) / b);
            }
        }

        // --------------------------------------------------------------------- //
        U64 __fastcall OnDrinkApplyPotionItemEffect(LPVOID fx, LPVOID item, float mult)
        {
            State& s = S();
            if (PlausiblePtr(fx) && PlausiblePtr(item))
            {
                ItemProbe p;
                ProbeItem(item, &p);
                if (p.ok)
                {
                    const U32 type = LookupItemType(p.itemId);
                    const Rule* rule = nullptr;
                    for (const Rule& r : s.rules)
                        if (r.typeId == type) { rule = &r; break; }

                    static long dumped = 0;
                    if (s.dump && dumped < 40)
                    {
                        ++dumped;
                        char line[700];
                        int n = std::snprintf(line, sizeof(line), "drink: itemId=%u quality=%u type(db)=%u feature=%p rule=%s | raw:", p.itemId, p.quality, type,
                                              p.feature, rule ? "yes" : "no");
                        for (int i = 0; i < 24 && n > 0 && n < static_cast<int>(sizeof(line)) - 12; ++i)
                            n += std::snprintf(line + n, sizeof(line) - n, " %X", p.raw[i]);
                        FileLog("%s", line);
                    }

                    if (rule != nullptr && p.feature == nullptr)
                    {
                        const int duration = DurationFor(p.quality);
                        const int added = AddEffects(fx, rule->effects, duration);
                        if (s.verbose)
                        {
                            static long logged = 0;
                            if (logged < 100)
                            {
                                ++logged;
                                FileLog("drink: item %u type %u quality %u -> %d effect(s), duration %d ms", p.itemId, type, p.quality, added, duration);
                            }
                        }
                    }
                }
            }
            return _Drink_ApplyPotionItemEffect(fx, item, mult);
        }

        // --------------------------------------------------------------------- //
        bool ConfigureDrinkEffects(const tinyxml2::XMLElement* root)
        {
            State& s = S();
            if (s.attached)
            {
                Lifx::ShowErrorMessage("Can't configure drinkEffects while the hook is attached.");
                return false;
            }
            s.rules.clear();
            s.enabled = false;
            if (root == nullptr)
                return true;

            const tinyxml2::XMLElement* section = root->FirstChildElement("drinkEffects");
            if (section == nullptr || !section->BoolAttribute("enabled", false))
                return true;
            s.verbose = section->BoolAttribute("verbose", true);
            s.dump = section->BoolAttribute("dump", false);

            if (const tinyxml2::XMLElement* db = section->FirstChildElement("db"))
            {
                if (const char* v = db->Attribute("host")) s.dbHost = v;
                if (const char* v = db->Attribute("user")) s.dbUser = v;
                if (const char* v = db->Attribute("password")) s.dbPassword = v;
                if (const char* v = db->Attribute("name")) s.dbName = v;
                unsigned port = 0;
                if (db->QueryUnsignedAttribute("port", &port) == tinyxml2::XML_SUCCESS && port != 0) s.dbPort = port;
            }

            for (const tinyxml2::XMLElement* d = section->FirstChildElement("drink"); d != nullptr; d = d->NextSiblingElement("drink"))
            {
                unsigned typeId = 0;
                if (d->QueryUnsignedAttribute("objectTypeId", &typeId) != tinyxml2::XML_SUCCESS || typeId == 0)
                {
                    Lifx::ShowErrorMessage("Invalid <drinkEffects><drink>: objectTypeId is required.");
                    s.rules.clear();
                    return false;
                }
                Rule r;
                r.typeId = typeId;
                for (const tinyxml2::XMLElement* e = d->FirstChildElement("effect"); e != nullptr; e = e->NextSiblingElement("effect"))
                {
                    unsigned id = 0;
                    float mag = 0.0f;
                    if (e->QueryUnsignedAttribute("id", &id) != tinyxml2::XML_SUCCESS || id == 0 || id > 93
                        || e->QueryFloatAttribute("magnitude", &mag) != tinyxml2::XML_SUCCESS || mag < 0.0f)
                    {
                        Lifx::ShowErrorMessage("Invalid <drinkEffects><drink><effect>: id (1..93) and magnitude (>= 0) are required.");
                        s.rules.clear();
                        return false;
                    }
                    EffectRef er;
                    er.effectId = id;
                    er.magnitude = mag;
                    r.effects.push_back(er);
                }
                if (r.effects.empty())
                    continue;
                s.rules.push_back(r);
            }
            if (s.rules.empty())
                return true;   // enabled but empty: nothing to do
            s.enabled = true;
            return true;
        }

        void AttachDrinkEffectsHook()
        {
            State& s = S();
            if (!s.enabled || s.attached)
                return;

            s.base = reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (s.base == 0)
            {
                Lifx::ShowErrorMessage("Can't attach drink-effects hook: main module base is null.");
                return;
            }

            // applyPotionItemEffect: push rbx; push rsi; sub rsp,0D8h; movaps [rsp+80h],xmm8 ...
            // addEffect core:        mov rax,rsp; push rbp; push r14; push r15; lea rbp,[rax-278h] ...
            // getEffectsFeature:     sub rsp,38h; mov edx,5; call ...
            static constexpr std::uint8_t applyPrologue[] = { 0x40, 0x53, 0x56, 0x48, 0x81, 0xEC, 0xD8, 0x00, 0x00, 0x00, 0x44, 0x0F, 0x29, 0x84, 0x24, 0x80 };
            static constexpr std::uint8_t addPrologue[] = { 0x48, 0x8B, 0xC4, 0x55, 0x41, 0x56, 0x41, 0x57, 0x48, 0x8D, 0xA8, 0x88, 0xFD, 0xFF, 0xFF };
            static constexpr std::uint8_t featPrologue[] = { 0x48, 0x83, 0xEC, 0x38, 0xBA, 0x05, 0x00, 0x00, 0x00, 0xE8 };
            if (std::memcmp(reinterpret_cast<const void*>(s.base + kApplyPotionRva), applyPrologue, sizeof(applyPrologue)) != 0
                || std::memcmp(reinterpret_cast<const void*>(s.base + kAddEffectRva), addPrologue, sizeof(addPrologue)) != 0
                || std::memcmp(reinterpret_cast<const void*>(s.base + kGetEffectsFeatureRva), featPrologue, sizeof(featPrologue)) != 0)
            {
                Lifx::ShowErrorMessage("Can't attach drink-effects hook: expected code not found (different server build?).");
                return;
            }

            _Drink_ApplyPotionItemEffect = reinterpret_cast<_Drink_ApplyPotionItemEffect_Fn>(s.base + kApplyPotionRva);
            const LONG rc = DetourAttach(&(PVOID&)_Drink_ApplyPotionItemEffect, OnDrinkApplyPotionItemEffect);
            if (rc != NO_ERROR)
            {
                Lifx::ShowErrorMessage("DetourAttach failed for the drink-effects hook. Error: %ld", rc);
                return;
            }

            s.attached = true;
            FileLog("hook attached: %u drink rule(s)", static_cast<unsigned>(s.rules.size()));
        }
    }
}
