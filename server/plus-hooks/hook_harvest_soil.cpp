// Ported 2026-10-04 from Daniel's Agriculture_mod (unchanged except the logging include, see agri_log.h).
/* ===================================================================================
    Ziel 3 der Agriculture_mod-Aufgabe: nach dem Ernten eines Felds soll die
    Bodenkachel wieder "steppe_soil" (bzw. die jeweilige settled-Variante)
    zeigen statt in der wahrend des Anbaus genutzten "*_loose"-Variante
    (z. B. "steppe_soil_loose") stehen zu bleiben.

    ------------------------------------------------------------------------------
    Hintergrund (statische RE von ddctd_cm_yo_server.exe, 2026-09-21, Server_mod/
    RE_Tools mit neu geschriebenem rtti_scan.py - RTTI-Vtable-Scanner, siehe dort):

    Per RTTI-Scan (Methodik aus Ermittlungsstand.md 4zz-h/4zz-j, dort fuer
    PlowAuto/MiningBaseImp etabliert) gefunden: `AbilityImp::HarvestPlant`
    (Vtable-RVA 0x7F72E0) ist die native Implementierung von Ability 73
    ("Harvest Crops"), strukturell identisch zu PlowAuto (10-Slot-Vtable,
    Slot 1 = Gate 0x3A4DB0, Slot 3 = onDoPerform 0x3A5030, alle anderen
    Slots byte-identisch mit PlowAuto/MiningBaseImp -> gemeinsame
    "kontinuierliche, pro-Kachel validierte Ability"-Basisklasse). Existiert
    NUR serverseitig (per RTTI-Gegencheck auf yo_cm_client.exe bestaetigt -
    dort existiert nur die namespace-lose `HarvestPlant_Ability`-Metadaten-
    /Factory-Klasse, keine `AbilityImp::HarvestPlant`-Implementierung -
    exakt das gleiche Muster wie bei PlowAuto in 4zz-j).

    `AbilityImp::HarvestPlant::_onDoPerform` (RVA 0x3A5030) ruft bei
    RVA 0x3A5868 denselben nativen Terrain-Substanz-Schreibbefehl auf
    (RVA 0x3AF070, `rcx = [rdi+8]`, `rdx = lea [rdi+0xf8]`) wie
    `AbilityImp::PlowAuto::_onDoPerform` bei seinem eigenen Schreibbefehl
    (RVA 0x3A5FD1, s. Ermittlungsstand.md Zeile 796: "der konkrete
    Substanz-Schreibbefehl darin ist noch nicht isoliert" - hiermit isoliert).

    PlowAuto schreibt die neue Substanz per HARTCODIERTEM Vergleich direkt
    vor seinem 0x3AF070-Aufruf (RVA 0x3A5FA1-0x3A5FC6):
        orig==1 (soil)       -> [obj+0x134] = 6  (LoosenSoil)
        orig==9 (steppesoil) -> [obj+0x134] = 0xA (loosensteppesoil)
    Das ist exakt die "Boden wird beim Pfluegen/Anlegen eines Felds
    aufgelockert"-Transformation.

    HarvestPlant schreibt seine neue Substanz stattdessen ueber einen
    Rueckgabewert einer kleinen Helper-Funktion (RVA 0x3A9740), die per
    FNV-1a-Hashmap-Lookup (Tabelle bei RVA 0xB98D50, Key = ein Pflanzen-
    /Feldfrucht-Objekttyp aus [rdi+0x134] - NICHT die aktuelle Boden-
    substanz selbst) einen Eintrag `{msgCode: u32, replacementSubstance: u8}`
    nachschlaegt. `0x3A9740` selbst hat GENAU EINEN Aufrufer im gesamten
    Server-Exe (per find_xrefs.py bestaetigt, RVA 0x3A53EE, innerhalb von
    `_onDoPerform` selbst) - ein sauberer, alleiniger Choke-Point.

    Die Tabelle wird bei Programmstart durch eine Reihe hartcodierter
    Literale befuellt (RVA-Bereich 0x40C02-0x40D73, 16 Eintraege = 8
    Feldfrucht-Objekttyp-Paare, z. B. 0x69/0x6A, 0x73/0x74, ... 0xAF/0xB0).
    **Alle 16 Eintraege haben denselben `replacementSubstance`-Wert: 0x0A
    (10 = loosensteppesoil / "steppe_soil_loose")** - unabhaengig von der
    tatsaechlich beackerten Feldfrucht. Das ist die Root Cause: nach der
    Ernte bleibt die Kachel unveraendert auf der waehrend des Anbaus
    genutzten "loose"-Substanz stehen, statt auf die settled-Variante
    zurueckzukehren (waehrend PlowAuto den UMGEKEHRTEN Uebergang beim
    Pfluegen/Anlegen des Felds bereits korrekt beherrscht).

    ------------------------------------------------------------------------------
    Fix (Choke-Point-Hook, analog zur frueheren, inzwischen entfernten hook_plow_speed.cpp-Technik): statt alle 16
    Literal-Stellen in der Tabellen-Initialisierung einzeln zu patchen (was
    zukuenftige, heute unbekannte Feldfrucht-Eintraege NICHT abdecken
    wuerde), wird der Rueckgabewert von `0x3A9740` direkt am einzigen
    Choke-Point umgemappt - unmittelbar nach dem `movzx eax, byte ptr
    [rax+4]`, das den Tabelleneintrag liest, RVA 0x3A9759:

        +0x3a9740: mov   [rsp+8], cl            ; unveraendert (Funktionsprolog)
        +0x3a9744: sub   rsp, 0x28              ; unveraendert
        +0x3a9748: lea   rdx, [rsp+0x30]        ; unveraendert
        +0x3a974d: lea   rcx, [rip+0x7ef5fc]    ; unveraendert (Tabellenobjekt)
        +0x3a9754: call  0x3a94a0               ; unveraendert (FNV-Hashmap-Lookup)
        +0x3a9759: movzx eax, byte ptr [rax+4]  ; <-- Patch beginnt hier (4 Byte)
        +0x3a975d: add   rsp, 0x28              ; <-- wird im Trampolin repliziert
        +0x3a9761: ret                          ; <-- wird im Trampolin repliziert

    Direkt nach der Funktion (RVA 0x3A9762-0x3A976F, 14 Byte `int3`-Padding
    bis zum Start der naechsten echten Funktion bei 0x3A9770) liegt genug
    ungenutzter Platz, um den 14-Byte "jmp qword ptr [rip+0]; <abs64>"-Stub
    (identische Technik wie die frueher hier verwendete, inzwischen entfernte hook_plow_speed.cpp) direkt ab +0x3A9759
    einzuschreiben, ohne die (unveraenderte) Nachbarfunktion +0x3A9770 zu
    beruehren.

    Das Trampolin repliziert `movzx eax, byte ptr [rax+4]` byte-identisch,
    mapped AL dann symmetrisch zu PlowAutos "Auflockern"-Tabelle zurueck
    (6->1 LoosenSoil->soil, 8->7 forest_soil_loose->forestsoil, 0xA->9
    loosensteppesoil->steppesoil; jeder andere Wert bleibt unveraendert -
    zukunftssicher fuer heute unbekannte Substanz-/Feldfrucht-Kombinationen),
    repliziert dann `add rsp, 0x28` + `ret` und kehrt damit exakt wie im
    Original zum einzigen Aufrufer (+0x3A53EE) zurueck.

    NICHT Teil dieser Untersuchung: die in Ermittlungsstand.md Zeile 97
    referenzierten, verloren gegangenen Dateien `outputs/patch_harvest_soil_
    revert.py` / `outputs/ddctd_cm_yo_server_patched.exe` eines fruheren,
    unabhaengigen Patch-Tasks - diese komplette RE-Kette (RTTI-Klassen-
    findung, Choke-Point-Isolierung, Tabellen-Dekodierung) wurde 2026-09-21
    unabhaengig und von Grund auf neu erarbeitet, wie mit dem User
    abgestimmt.
*  =================================================================================== */

#include "hook_harvest_soil.h"

#include "core/tinyxml2.h"
#include "agri_log.h"

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
            constexpr std::uintptr_t kPatchRva = 0x3A9759;   // start of the overwritten block (movzx eax, byte ptr [rax+4])
            // No explicit resume address needed (unlike the former hook_plow_speed, since removed): the
            // trampoline ends with its own "ret", which pops the return address
            // already on the stack from the original "call 0x3A9740" (at
            // +0x3A53EE, AbilityImp::HarvestPlant::_onDoPerform) and lands back
            // there directly - no jmp-to-fixed-address needed.

            // Complete instructions from +0x3A9759 through +0x3A9761 (9
            // bytes: movzx + add rsp,0x28 + ret), followed by 5 bytes of
            // this function's own trailing int3 padding (safe to overwrite -
            // never executed, and the next real function starts only at
            // +0x3A9770). 14 bytes total, matching the "jmp qword ptr
            // [rip+0]; <abs64>" stub size used below and in
            // hook_plow_speed.cpp, since removed - superseded, see README.md). Verified byte-for-byte against the
            // shipped exe before writing this file.
            constexpr std::array<std::uint8_t, 14> kOriginalPatchBytes = {
                0x0F, 0xB6, 0x40, 0x04,                   // movzx eax, byte ptr [rax+4]
                0x48, 0x83, 0xC4, 0x28,                   // add rsp, 0x28
                0xC3,                                     // ret
                0xCC, 0xCC, 0xCC, 0xCC, 0xCC              // int3 padding (unused, safe to overwrite)
            };

            struct HarvestSoilState
            {
                bool enabled = false;
                bool attached = false;

                std::uintptr_t moduleBase = 0;
                std::uintptr_t patchAddress = 0;
                void* trampoline = nullptr;
                std::size_t trampolineSize = 0;
            };

            HarvestSoilState gHarvestSoil;

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

                code.insert(code.end(), std::begin(instruction), std::end(instruction));
                AppendU64(code, static_cast<std::uint64_t>(destination));
            }

            // ----------------------------------------------------------------- //
            bool MatchesExpectedBytes(std::uintptr_t address)
            {
                return std::memcmp(
                    reinterpret_cast<const void*>(address),
                    kOriginalPatchBytes.data(),
                    kOriginalPatchBytes.size()) == 0;
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
                        "Harvest soil hook was written, but its page protection could not be "
                        "restored.");
                }

                return true;
            }

            // ----------------------------------------------------------------- //
            // Builds:
            //   movzx eax, byte ptr [rax+4]      ; replicate (reads the table entry's
            //                                     ; replacementSubstance byte into AL)
            //   cmp al, 6    ; LoosenSoil -> soil
            //   jne  check8
            //   mov  al, 1
            //   jmp  done
            // check8:
            //   cmp al, 8    ; forest_soil_loose -> forestsoil
            //   jne  check10
            //   mov  al, 7
            //   jmp  done
            // check10:
            //   cmp al, 0xA  ; loosensteppesoil -> steppesoil
            //   jne  done
            //   mov  al, 9
            // done:
            //   add rsp, 0x28                     ; replicate
            //   ret                                ; replicate - returns to +0x3A53F3
            bool BuildTrampoline()
            {
                std::vector<std::uint8_t> code;
                code.reserve(4 + 3 * (2 + 2 + 2) + 4 + 1 + 14);

                // movzx eax, byte ptr [rax+4] - replicated verbatim.
                code.push_back(0x0F);
                code.push_back(0xB6);
                code.push_back(0x40);
                code.push_back(0x04);

                auto emitRemapStep =
                    [&code](std::uint8_t loose, std::uint8_t settled, bool isLast)
                {
                    // cmp al, loose
                    code.push_back(0x3C);
                    code.push_back(loose);

                    // jne nextStep (patched once nextStep is known)
                    code.push_back(0x75);
                    const std::size_t jneOperand = code.size();
                    code.push_back(0x00); // placeholder

                    // mov al, settled
                    code.push_back(0xB0);
                    code.push_back(settled);

                    std::size_t jmpDoneOperand = 0;
                    if (!isLast)
                    {
                        // jmp done (patched once "done" is known)
                        code.push_back(0xEB);
                        jmpDoneOperand = code.size();
                        code.push_back(0x00); // placeholder
                    }

                    const std::size_t nextStep = code.size();
                    code[jneOperand] = static_cast<std::uint8_t>(nextStep - (jneOperand + 1));

                    return jmpDoneOperand;
                };

                std::vector<std::size_t> jmpDoneOperands;

                std::size_t op = emitRemapStep(6, 1, false);       // LoosenSoil -> soil
                jmpDoneOperands.push_back(op);
                op = emitRemapStep(8, 7, false);                   // forest_soil_loose -> forestsoil
                jmpDoneOperands.push_back(op);
                emitRemapStep(0xA, 9, true);                       // loosensteppesoil -> steppesoil (falls through to done)

                const std::size_t done = code.size();
                for (const std::size_t operand : jmpDoneOperands)
                    code[operand] = static_cast<std::uint8_t>(done - (operand + 1));

                // add rsp, 0x28 - replicated verbatim.
                code.push_back(0x48);
                code.push_back(0x83);
                code.push_back(0xC4);
                code.push_back(0x28);

                // ret - replicated verbatim. Returns to +0x3A53F3, exactly as the
                // original function's own epilogue would have.
                code.push_back(0xC3);

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

                gHarvestSoil.trampoline = trampoline;
                gHarvestSoil.trampolineSize = code.size();
                return true;
            }

            // ----------------------------------------------------------------- //
            std::array<std::uint8_t, kOriginalPatchBytes.size()> MakeSelectorPatch()
            {
                std::array<std::uint8_t, kOriginalPatchBytes.size()> patch{};
                patch.fill(0x90);

                std::vector<std::uint8_t> jump;
                AppendAbsoluteJump(jump, reinterpret_cast<std::uintptr_t>(gHarvestSoil.trampoline));
                std::memcpy(patch.data(), jump.data(), jump.size());

                return patch;
            }
        }

        // --------------------------------------------------------------------- //
        bool ConfigureHarvestSoil(const tinyxml2::XMLElement* root)
        {
            if (gHarvestSoil.attached)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure harvest soil while the hook is attached.");
                return false;
            }

            gHarvestSoil = {};

            if (root == nullptr)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure harvest soil: XML root element is null.");
                return false;
            }

            const tinyxml2::XMLElement* section =
                root->FirstChildElement("harvestSoil");

            // Missing or disabled section preserves the original game behaviour.
            if (section == nullptr)
                return true;

            gHarvestSoil.enabled = section->BoolAttribute("enabled", false);
            return true;
        }

        // --------------------------------------------------------------------- //
        void AttachHarvestSoilHook()
        {
            if (!gHarvestSoil.enabled || gHarvestSoil.attached)
                return;

            Redshark::LogInfo("AttachHarvestSoilHook: checkpoint A - resolving module base.");

            gHarvestSoil.moduleBase =
                reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (gHarvestSoil.moduleBase == 0)
            {
                Redshark::ShowErrorMessage(
                    "Can't attach harvest soil hook: main module base is null.");
                return;
            }

            gHarvestSoil.patchAddress = gHarvestSoil.moduleBase + kPatchRva;

            Redshark::LogInfo(
                "AttachHarvestSoilHook: checkpoint B - module base 0x%llx, checking patch "
                "bytes at +0x3A9759.",
                static_cast<unsigned long long>(gHarvestSoil.moduleBase));

            if (!MatchesExpectedBytes(gHarvestSoil.patchAddress))
            {
                Redshark::ShowErrorMessage(
                    "Can't attach harvest soil hook: expected code was not found at +0x3A9759.");
                return;
            }

            Redshark::LogInfo(
                "AttachHarvestSoilHook: checkpoint C - bytes matched, building trampoline.");

            if (!BuildTrampoline())
            {
                Redshark::ShowErrorMessage(
                    "Can't attach harvest soil hook: trampoline allocation failed.");
                return;
            }

            Redshark::LogInfo(
                "AttachHarvestSoilHook: checkpoint D - trampoline built (%zu bytes at %p), "
                "patching call site.",
                gHarvestSoil.trampolineSize,
                gHarvestSoil.trampoline);

            const auto patch = MakeSelectorPatch();
            if (!WriteExecutableMemory(
                    gHarvestSoil.patchAddress,
                    patch.data(),
                    patch.size()))
            {
                VirtualFree(gHarvestSoil.trampoline, 0, MEM_RELEASE);
                gHarvestSoil.trampoline = nullptr;
                gHarvestSoil.trampolineSize = 0;
                Redshark::ShowErrorMessage(
                    "Can't attach harvest soil hook: patch memory is not writable.");
                return;
            }

            Redshark::LogInfo(
                "AttachHarvestSoilHook: checkpoint E - patched successfully. Harvested fields "
                "now revert their tile substance to the settled variant (LoosenSoil->soil, "
                "forest_soil_loose->forestsoil, loosensteppesoil->steppesoil) instead of "
                "staying on the loosened one used while the crop was growing.");

            gHarvestSoil.attached = true;
        }

        // --------------------------------------------------------------------- //
        void DetachHarvestSoilHook()
        {
            if (!gHarvestSoil.attached)
                return;

            if (!WriteExecutableMemory(
                    gHarvestSoil.patchAddress,
                    kOriginalPatchBytes.data(),
                    kOriginalPatchBytes.size()))
            {
                Redshark::ShowErrorMessage(
                    "Can't detach harvest soil hook: original bytes could not be restored.");
                return;
            }

            gHarvestSoil.attached = false;

            if (gHarvestSoil.trampoline != nullptr)
            {
                VirtualFree(gHarvestSoil.trampoline, 0, MEM_RELEASE);
                gHarvestSoil.trampoline = nullptr;
                gHarvestSoil.trampolineSize = 0;
            }
        }
    }
}
