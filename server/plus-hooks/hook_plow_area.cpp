// Ported 2026-10-04 from Daniel's Agriculture_mod (unchanged except the logging include, see agri_log.h).
/* ===================================================================================
    Neue Anfrage (2026-09-21, siehe Doc_Tasks/Ermittlungsstand_Agriculture_mod.md
    Abschnitt "5c"/"5d"): Ability 143 ("Plowing", native Implementierung
    AbilityImp::PlowAuto) soll statt einer einzelnen Bodenkachel direkt eine
    3x3-Flaeche (die eigentlich umgepfluegte Mount0-Kachel plus ihre 8
    Nachbarn) von der Substanz her umwandeln.

    ------------------------------------------------------------------------------
    Zwei Design-Entscheidungen wurden mit dem User per AskUserQuestion
    abgestimmt, bevor dieser Hook geschrieben wurde:

      1. "Direkt auf Mount0-Erkennung aufbauen" (statt die vollstaendige
         Zielerfassungs-Pipeline - RVA 0x3AF570, s.u. - pro Nachbar erneut
         aufzurufen): die bereits validierte CellRef-Struktur der
         Zentrums-Kachel (eingebettet in der PlowAuto-Instanz bei +0xf8)
         wird byte-fuer-byte geklont, nur X/Y werden pro Nachbar verschoben.
         Das ist einfacher und risikoaermer, deckt dafuer Sonderfaelle wie
         eine Nachbarkachel in einer ANDEREN Region/einem anderen Claim
         nur so weit ab, wie die vorhandenen Validierungsfunktionen (s.u.)
         das selbststaendig erkennen und ablehnen koennen.

      2. "Nur umwandeln wenn Substanz 1 oder 9": jede der 8 Nachbarkacheln
         wird VOR dem Schreiben einzeln auf ihre AKTUELLE (nicht die der
         Zentrums-Kachel geklonte) Substanz geprueft - nur soil(1) und
         steppesoil(9) werden umgewandelt, jede andere Substanz (Sand,
         Fels, Wasser, bereits aufgelockerter Boden, ein Gebaeude-Fundament,
         ...) bleibt unangetastet. Im Zweifel (jede Validierung schlaegt
         fehl) wird die jeweilige Nachbarkachel einfach uebersprungen - nie
         erzwungen geschrieben.

    ------------------------------------------------------------------------------
    Statische RE (ddctd_cm_yo_server.exe, RE_Tools/disas.py, 2026-09-21):

    AbilityImp::PlowAuto::_onDoPerform (RVA 0x3A5FA1-0x3A5FDF) bestimmt die
    neue Substanz fuer die Zentrums-Kachel hartcodiert:
        orig==1 (soil)       -> [instance+0x134] = 6  (LoosenSoil)
        orig==9 (steppesoil) -> [instance+0x134] = 0xA (loosensteppesoil)
    und schreibt sie ueber den immer gleichen Choke-Point (auch von
    hook_harvest_soil.cpp genutzt):

        AbilityImp::TerrainWrite(actorPtr, CellRef*) @ RVA 0x3AF070

    Patch-Stelle: der Aufruf dieser Funktion fuer die Zentrums-Kachel plus
    die direkt folgende, aus dem Rueckgabewert (AL) den Funktions-
    Ruckgabecode berechnende Sequenz - zusammen exakt 14 Byte, byte-
    identisch gegen die shipped exe verifiziert:

        +0x3A5FD1: call 0x3AF070            ; E8 9A 90 00 00
        +0x3A5FD6: neg   al                 ; F6 D8
        +0x3A5FD8: sbb   eax, eax           ; 1B C0
        +0x3A5FDA: add   eax, 4             ; 83 C0 04
        +0x3A5FDD: jmp   short +0x3A6052    ; EB 73  <- UNBEDINGT, kein Fallthrough!

    WICHTIG: die letzte der 14 gepatchten Bytes ist ein UNBEDINGTER Sprung
    (EB 73). Im Original faellt die Ausfuehrung an dieser Stelle NIE nach
    +0x3A5FDF durch - +0x3A5FDF ("call 0x405040 ...") ist toter Code aus
    Sicht dieses Pfads, erreichbar nur ueber einen ganz anderen, hier
    irrelevanten Zweig weiter oben in der Funktion. Die tatsaechliche
    Resume-Adresse fuer das Trampolin ist deshalb das SPRUNGZIEL +0x3A6052
    (bestaetigt per Disassemblierung: dort beginnt direkt der Epilog von
    _onDoPerform - "lea r11,[rsp+0x4850]; mov rbx,[r11+0x10]; ...; ret" -,
    nicht die rechnerische Fallthrough-Adresse +0x3A5FDF.

    (rcx = [rbx+8] {actorPtr}, rdx = lea [rbx+0xf8] {CellRef der Zentrums-
    Kachel} - beide bereits VOR der Patch-Stelle durch den unveraenderten
    Code bei +0x3A5FC6/+0x3A5FCD gesetzt, rbx = die PlowAuto-Instanz selbst,
    laut x64-ABI callee-saved und damit ueber jeden Funktionsaufruf hinweg
    gueltig - der Epilog bei +0x3A6052 restauriert ohnehin den urspruenglich
    AUFRUFENDEN rbx-Wert aus einem festen Stack-Slot, unabhaengig davon, was
    wir mit rbx innerhalb unseres Trampolins anstellen.)

    ------------------------------------------------------------------------------
    CellRef-Struktur (eingebettet in der PlowAuto-Instanz bei +0xf8; alle
    Offsets unten relativ zum CellRef-Start, per Disassemblierung von
    RVA 0x3AF070/0x3AEFF0/0x3AF370/0x3AF570/0x58C370/0x4B13B0
    kreuzverifiziert):

        +0x00  qword  RegionPtr1   (Ziel-Region fuer die eigentliche
                                     Schreiboperation; Region-Objekt haelt
                                     width/height als u16 bei +0xe0/+0xe4)
        +0x08  qword  Ref-Control  (wird per "lock inc dword ptr[+8]" beim
                                     Klonen/Kopieren refcounted - siehe
                                     RVA 0x3AF0A9-0x3AF0B2)
        +0x10  int32  X            (Ziel-Spalte innerhalb RegionPtr1)
        +0x14  int32  Y            (Ziel-Zeile innerhalb RegionPtr1)
        +0x18  qword  RegionPtr2   (zweite Region - Claim-/Ownership-Gitter,
                                     gleiche width/height-Konvention)
        +0x28  int32  X2           (Ziel-Spalte innerhalb RegionPtr2)
        +0x2c  int32  Y2           (Ziel-Zeile innerhalb RegionPtr2)
        +0x30  dword  != 0 geprueft von RVA 0x3AEFF0 (Zweck nicht weiter
                       aufgeloest - beim Klonen unveraendert uebernommen)
        +0x34  dword  "fieldA"     (Parameter fuer den Lese-Gate 0x58C370)
        +0x38  dword  Misc         (wird beim Auffrischen aus einem Byte
                                     zero-extended geschrieben, s.u.)
        +0x3c  byte   Substanz     (== instance+0x134 fuer die Zentrums-
                                     Kachel; 1=soil, 9=steppesoil, 6=Loosen-
                                     Soil, 0xA=loosensteppesoil, ...)
        +0x3d  byte   Flag D       (Bit 5 des per-Zelle "flags"-Bytes)
        +0x3e  byte   Flag E       (Bit 6 des per-Zelle "flags"-Bytes)
        +0x3f  byte   Modus-Flag   (von 0x3AF070 gelesen, aber NICHT von
                                     0x3AF570 gesetzt - vermutlich ein
                                     Operations-Flag der Ability selbst,
                                     nicht pro-Kachel; beim Klonen unver-
                                     aendert korrekt aus der Zentrums-Kachel
                                     uebernommen)
        +0x40  qword  != 0 geprueft von RVA 0x3AEFF0 (von RVA 0x3AF570 beim
                       urspruenglichen Aufbau gesetzt: "mov [rcx+0x40],rdx")

    Klon-Groesse fuer diesen Hook: 0x48 Byte (deckt exakt den hoechsten von
    irgendeiner der unten verwendeten Funktionen gelesenen Offset ab,
    +0x40 als qword = bis einschliesslich +0x47).

    X/Y (+0x10/+0x14) sind absichtlich die EINZIGEN Felder, die pro Nachbar
    veraendert werden, BEVOR die unten stehenden Validierungen laufen -
    alles andere (inkl. RegionPtr2/X2/Y2, der zweiten, vermutlich Claim-
    bezogenen Koordinate) bleibt exakt wie bei der Zentrums-Kachel geklont.
    Das ist unproblematisch: RVA 0x3AEFF0 prueft RegionPtr2/X2/Y2 nur auf
    reine Grenzen (>=0, < width/height), nicht auf Uebereinstimmung mit
    RegionPtr1/X/Y - ein unveraendert mitgeklontes, aber weiterhin
    gueltiges zweites Koordinatenpaar besteht diese Pruefung also
    weiterhin, ohne dass wir seine tatsaechliche Bedeutung kennen muessten.

    ------------------------------------------------------------------------------
    Verwendete native Helper-Funktionen (alle bereits fuer die Zentrums-
    Kachel im unveraenderten Code auf demselben Ability-Objekt verwendet -
    hier 1:1 fuer jede der 8 Nachbarkacheln wiederverwendet, NICHT neu
    erfunden):

      RVA 0x3AEFF0  BoundsCheck(?, CellRef*)                    -> AL (1=ok)
          Grenzpruefung fuer BEIDE Koordinatenpaare (RegionPtr1/X/Y UND
          RegionPtr2/X2/Y2) gegen die jeweilige Region-Breite/-Hoehe, plus
          die beiden != 0 - Pruefungen bei +0x30/+0x40. Nimmt CellRef* in
          RDX entgegen (RCX wird von der Funktion nicht gelesen).

      RVA 0x58C370  ReadGate(CellRef*, u32 fieldA, u8 flag)      -> AL
          Duenner Tail-Call-Thunk auf einen internen Manager (RVA
          0x23AE70). Wird im Original unmittelbar vor jedem Live-Substanz-
          Read als Vorbedingung aufgerufen (RVA 0x3AF960-0x3AF96D, dort mit
          flag=1) - hier identisch uebernommen.

      RVA 0x4B13B0  ReadLiveSubstance(RegionPtr1*, u8 out[8], {i32 X;i32 Y}*)
          Liest den TATSAECHLICHEN aktuellen Zustand der Zielkachel aus der
          Region. out[0..1] = "Frische"-Wort (gegen den globalen Schwellwert
          unten verglichen), out[2] = aktuelle Substanz, out[3] = Flags-Byte
          (Bit5/6 -> CellRef+0x3d/+0x3e), out[6] -> CellRef+0x38 (zero-
          extended). Negative X/Y liefern eine genullte out[8] zurueck statt
          eines Fehlers - faellt aber ohnehin am Schwellwert-Vergleich durch.

      Globaler Schwellwert: u16 bei Modulbasis + 0x88DEF8 (hergeleitet aus
          RVA 0x3AF984 "movzx eax, word ptr [rip+0x4DE56D]" - rip-relativ ab
          der naechsten Instruktion bei RVA 0x3AF98B, also Ziel-RVA
          0x3AF98B + 0x4DE56D = 0x88DEF8). Original-Vergleich ist "ja" (JA,
          unsigned strictly-greater) - out[0..1] muss STRIKT GROESSER als
          dieser Wert sein, sonst gilt die gelesene Live-Substanz als nicht
          verwertbar (z. B. Region-/Chunk-Daten noch nicht bereit) und die
          jeweilige Nachbarkachel wird uebersprungen.

      RVA 0x3AF370  ConsistencyCheck(CellRef*)                  -> AL
          Derselbe Konsistenz-/Berechtigungs-Gate, den _onDoPerform selbst
          fuer die Zentrums-Kachel VOR dem Ueberschreiben von +0x3c aufruft
          (RVA 0x3A5F22-0x3A5F29). Bounds-prueft RegionPtr1/X/Y, loest die
          Kachel per RVA 0x4B1140 auf und vergleicht CellRef+0x38/+0x3c/
          +0x3d/+0x3e gegen den frisch aus der Welt gelesenen Ist-Zustand.
          Deshalb WICHTIG: dieser Hook ruft ReadLiveSubstance zuerst auf und
          schreibt dessen Ergebnis in den Klon, BEVOR ConsistencyCheck
          laeuft - sonst wuerde die Pruefung staendig gegen die (falsche)
          geklonte Zentrums-Substanz fehlschlagen. Gibt 0 zurueck wenn alles
          konsistent ist, ungleich 0 bei jeder Art von Problem (Grenzen,
          Aufloesung, Zustands-Mismatch) - in jedem Fall wird die jeweilige
          Nachbarkachel dann uebersprungen statt erzwungen geschrieben.

      RVA 0x3AF070  TerrainWrite(actorPtr, CellRef*)             -> AL
          Der eigentliche Schreib-Choke-Point (s.o.) - fuer jede Nachbar-
          kachel mit dem geklonten, um die neue Substanz aktualisierten
          CellRef erneut aufgerufen. AddReft dabei intern den in CellRef+8
          referenzierten Refcount ein weiteres Mal - unproblematisch, das
          ist exakt das normale Verhalten beim mehrfachen Verwenden
          desselben Smart-Pointer-Ziels.

    ------------------------------------------------------------------------------
    Warum der Trampolin-Code selbst in C++ statt in reinem Hand-Assembler
    (wie bei hook_harvest_soil.cpp) implementiert ist:

    hook_harvest_soil.cpp's Trampolin ersetzt eine simple 3-Wege-
    Vergleichskette (cmp/jne) - noch gut von Hand als Bytes zu schreiben und
    zu verifizieren. Diese Erweiterung braucht dagegen pro Nachbarkachel
    bis zu 5 Funktionsaufrufe mit unterschiedlichen Argument-Registern,
    Byte-weise CellRef-Manipulation und mehrere Abbruchbedingungen - macht
    man das alles 8x in Hand-Assembler entrollt, ist das Ergebnis extrem
    lang und praktisch nicht mehr ohne Debugger verifizierbar. Stattdessen
    patcht der Trampolin-Stub selbst nur das unveraendert Notwendige
    (Zentrums-Schreibaufruf + Ruckgabecode-Berechnung, byte-identisch zum
    Original repliziert) und ruft danach ueber einen ganz normalen x64-
    Funktionsaufruf (RCX = PlowAuto-Instanz, Schattenraum + Stack-Ausrich-
    tung von Hand nachgerechnet - siehe BuildTrampoline()) in
    ExpandPlowNeighbors() hinein, eine stinknormale, gut lesbare und
    testbare C++-Funktion in dieser DLL. Der urspruengliche Rueckgabecode
    wird dafuer ueber PUSH/POP RAX um den Aufruf herum gerettet - _on-
    DoPerform bekommt also exakt denselben Ruckgabewert wie im Original,
    unabhaengig davon, was mit den Nachbarkacheln passiert.

    ------------------------------------------------------------------------------
    Bekannte Grenzen (Stand 2026-09-21, statisch verifiziert, NOCH NICHT
    live getestet):

      - Eine Nachbarkachel, die in eine ANDERE Region als die Zentrums-
        Kachel faellt (z. B. an einer Regionsgrenze), scheitert bereits an
        RVA 0x3AEFF0's Grenzpruefung (da RegionPtr1 unveraendert von der
        Zentrums-Kachel geklont wird) und wird einfach uebersprungen - kein
        Crash, aber auch keine Abdeckung ueber Regionsgrenzen hinweg. Das
        ist eine direkte Konsequenz der gewaehlten "auf Mount0-Erkennung
        aufbauen"-Variante (siehe oben) und wurde dem User so dargestellt.
      - Eine Nachbarkachel in einem fremden Claim/Grundstueck kann durch
        ConsistencyCheck (RVA 0x3AF370) abgelehnt werden, je nachdem was
        dessen +0x38-Vergleich tatsaechlich reprasentiert - auch das fuehrt
        nur zum Ueberspringen dieser einen Nachbarkachel, nie zu einem
        erzwungenen Schreiben.
      - Alle 8 Nachbarn werden sequenziell UND ERST NACH dem unveraenderten
        Zentrums-Schreibvorgang verarbeitet - kein Rollback, falls z. B.
        die Nachbarn 1-3 erfolgreich waren und Nachbar 4 aus irgendeinem
        Grund abweicht. Da jede Nachbarkachel unabhaengig und idempotent
        (gleiche Eingabe -> gleiches Ergebnis) behandelt wird, ist das
        unkritisch - ein erneutes Pfluegen derselben Zentrumskachel wuerde
        lediglich die noch fehlenden Nachbarn nachtraeglich mit erledigen.
*  =================================================================================== */

#include "hook_plow_area.h"

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
            // -------------------------------------------------------------
            // Patch-Stelle (siehe grosser Kommentar oben).
            constexpr std::uintptr_t kPatchRva = 0x3A5FD1;

            // Sprungziel der originalen "jmp short +0x3A6052" (letztes der 14
            // gepatchten Bytes, UNBEDINGT) - NICHT die rechnerische Fallthrough-
            // Adresse +0x3A5FDF, die im Original nie erreicht wird (siehe grosser
            // Kommentar oben). +0x3A6052 ist der Beginn von _onDoPerform's Epilog.
            constexpr std::uintptr_t kResumeRva = 0x3A6052;

            constexpr std::array<std::uint8_t, 14> kOriginalPatchBytes = {
                0xE8, 0x9A, 0x90, 0x00, 0x00,   // call 0x3AF070
                0xF6, 0xD8,                     // neg al
                0x1B, 0xC0,                     // sbb eax, eax
                0x83, 0xC0, 0x04,               // add eax, 4
                0xEB, 0x73                      // jmp short +0x3A6052
            };

            // Native Helper-Funktionen und globaler Schwellwert (siehe
            // grosser Kommentar oben) - jeweils als RVA, zur Modulbasis
            // addiert in AttachPlowAreaHook().
            constexpr std::uintptr_t kFnBoundsCheckRva = 0x3AEFF0;
            constexpr std::uintptr_t kFnConsistencyCheckRva = 0x3AF370;
            constexpr std::uintptr_t kFnReadGateRva = 0x58C370;
            constexpr std::uintptr_t kFnReadSubstanceRva = 0x4B13B0;
            constexpr std::uintptr_t kFnWriteRva = 0x3AF070;
            constexpr std::uintptr_t kThresholdGlobalRva = 0x88DEF8;

            // CellRef-Feld-Offsets (siehe grosser Kommentar oben).
            constexpr std::size_t kCellRefCloneSize = 0x48;
            constexpr std::uintptr_t kOffInstanceActor = 0x08;
            constexpr std::uintptr_t kOffInstanceCellRef = 0xF8;
            constexpr std::uintptr_t kOffRegionPtr1 = 0x00;
            constexpr std::uintptr_t kOffX = 0x10;
            constexpr std::uintptr_t kOffY = 0x14;
            constexpr std::uintptr_t kOffFieldA = 0x34;
            constexpr std::uintptr_t kOffMisc38 = 0x38;
            constexpr std::uintptr_t kOffSubstance = 0x3C;
            constexpr std::uintptr_t kOffFlagD = 0x3D;
            constexpr std::uintptr_t kOffFlagE = 0x3E;

            // Die 8 Nachbarn eines 3x3-Quadrats, Zentrum (0,0) ausgenommen.
            constexpr int kNeighborOffsets[8][2] = {
                { -1, -1 }, { 0, -1 }, { 1, -1 },
                { -1,  0 },            { 1,  0 },
                { -1,  1 }, { 0,  1 }, { 1,  1 },
            };

            // Signaturen wie oben dokumentiert - Aufrufkonvention ist auf
            // x64 in jedem Fall die Standard-MS-x64-Konvention, das
            // Calling-Convention-Schluesselwort spielt hier keine Rolle.
            using Fn_BoundsCheck = std::uint8_t(*)(void* rcxUnused, void* cellRef);
            using Fn_ConsistencyCheck = std::uint8_t(*)(void* cellRef);
            using Fn_ReadGate = std::uint8_t(*)(void* cellRef, std::uint32_t fieldA, std::uint8_t flag);
            using Fn_ReadSubstance = void*(*)(void* regionPtr1, void* outBuf8, void* packedXY);
            using Fn_Write = std::uint8_t(*)(void* actorPtr, void* cellRef);

            struct PlowAreaState
            {
                bool enabled = false;
                bool attached = false;

                std::uintptr_t moduleBase = 0;
                std::uintptr_t patchAddress = 0;
                std::uintptr_t resumeAddress = 0;
                std::uintptr_t thresholdGlobalAddress = 0;

                Fn_BoundsCheck fnBoundsCheck = nullptr;
                Fn_ConsistencyCheck fnConsistencyCheck = nullptr;
                Fn_ReadGate fnReadGate = nullptr;
                Fn_ReadSubstance fnReadSubstance = nullptr;
                Fn_Write fnWrite = nullptr;

                void* trampoline = nullptr;
                std::size_t trampolineSize = 0;
            };

            PlowAreaState gPlowArea;

            // ----------------------------------------------------------------- //
            // Aufgerufen aus dem Trampolin mit RCX = die PlowAuto-Instanz (RBX an
            // der urspruenglichen Patch-Stelle). Laeuft strikt NACH dem
            // unveraenderten Zentrums-Schreibvorgang (siehe BuildTrampoline()) und
            // beeinflusst _onDoPerform's Ruckgabewert nie - jede Nachbarkachel
            // wird unabhaengig validiert; jeder fehlgeschlagene Check ueberspringt
            // nur diese eine Kachel, nie ein erzwungenes Schreiben.
            void ExpandPlowNeighbors(std::uintptr_t instancePtr)
            {
                if (instancePtr == 0)
                    return;

                const std::uintptr_t actorPtr =
                    *reinterpret_cast<std::uintptr_t*>(instancePtr + kOffInstanceActor);
                const std::uintptr_t centerCellRef = instancePtr + kOffInstanceCellRef;

                const std::int32_t centerX =
                    *reinterpret_cast<std::int32_t*>(centerCellRef + kOffX);
                const std::int32_t centerY =
                    *reinterpret_cast<std::int32_t*>(centerCellRef + kOffY);

                for (const auto& offset : kNeighborOffsets)
                {
                    std::uint8_t clone[kCellRefCloneSize];
                    std::memcpy(
                        clone,
                        reinterpret_cast<const void*>(centerCellRef),
                        kCellRefCloneSize);

                    *reinterpret_cast<std::int32_t*>(clone + kOffX) = centerX + offset[0];
                    *reinterpret_cast<std::int32_t*>(clone + kOffY) = centerY + offset[1];

                    // 1) Grenzpruefung (beide Koordinatenpaare).
                    if (!gPlowArea.fnBoundsCheck(nullptr, clone))
                        continue;

                    // 2) Gate fuer den folgenden Live-Substanz-Read.
                    const std::uint32_t fieldA =
                        *reinterpret_cast<std::uint32_t*>(clone + kOffFieldA);
                    if (!gPlowArea.fnReadGate(clone, fieldA, 1))
                        continue;

                    // 3) Tatsaechliche, aktuelle Substanz dieser EINEN Nachbar-
                    //    kachel lesen - NIE die geklonte Zentrums-Substanz dafuer
                    //    verwenden (User-Vorgabe: individuelle Pruefung je Nachbar).
                    void* const regionPtr1 =
                        *reinterpret_cast<void**>(clone + kOffRegionPtr1);
                    std::uint8_t outBuf[8] = {};
                    gPlowArea.fnReadSubstance(regionPtr1, outBuf, clone + kOffX);

                    const std::uint16_t freshnessWord =
                        *reinterpret_cast<std::uint16_t*>(outBuf + 0);
                    const std::uint16_t threshold =
                        *reinterpret_cast<std::uint16_t*>(gPlowArea.thresholdGlobalAddress);
                    if (freshnessWord <= threshold)
                        continue;

                    const std::uint8_t liveSubstance = outBuf[2];
                    const std::uint8_t liveFlagsByte = outBuf[3];

                    std::uint8_t newSubstance = 0;
                    if (liveSubstance == 1)
                        newSubstance = 6;          // soil -> LoosenSoil
                    else if (liveSubstance == 9)
                        newSubstance = 0xA;         // steppesoil -> loosensteppesoil
                    else
                        continue;                   // jede andere Substanz: unangetastet lassen

                    // Klon mit dem frisch gelesenen IST-Zustand auffrischen (siehe
                    // RVA 0x3AF9C0-0x3AF9E6), damit der Konsistenz-Check unten
                    // gegen die Realitaet statt gegen die geklonte Zentrums-Kachel
                    // prueft.
                    *reinterpret_cast<std::uint32_t*>(clone + kOffMisc38) = outBuf[6];
                    clone[kOffSubstance] = liveSubstance;
                    clone[kOffFlagD] = (liveFlagsByte >> 5) & 1;
                    clone[kOffFlagE] = (liveFlagsByte >> 6) & 1;

                    // 4) Derselbe Konsistenz-/Berechtigungs-Gate wie fuer die
                    //    Zentrums-Kachel im Original.
                    if (gPlowArea.fnConsistencyCheck(clone) != 0)
                        continue;

                    // 5) Neue Substanz eintragen und schreiben.
                    clone[kOffSubstance] = newSubstance;
                    gPlowArea.fnWrite(reinterpret_cast<void*>(actorPtr), clone);
                }
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

                code.insert(code.end(), std::begin(instruction), std::end(instruction));
                AppendU64(code, static_cast<std::uint64_t>(destination));
            }

            // ----------------------------------------------------------------- //
            void AppendAbsoluteCall(
                std::vector<std::uint8_t>& code,
                std::uintptr_t destination)
            {
                // mov rax, <abs64 destination> ; call rax
                code.push_back(0x48);
                code.push_back(0xB8);
                AppendU64(code, static_cast<std::uint64_t>(destination));
                code.push_back(0xFF);
                code.push_back(0xD0);
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
                        "Plow area hook was written, but its page protection could not be "
                        "restored.");
                }

                return true;
            }

            // ----------------------------------------------------------------- //
            // Baut den Trampolin-Code:
            //   mov rax, 0x3AF070 ; call rax      ; repliziert "call 0x3AF070"
            //   neg al                            ; repliziert (Byte-identisch)
            //   sbb eax, eax                      ; repliziert (Byte-identisch)
            //   add eax, 4                        ; repliziert (Byte-identisch)
            //   push rax                          ; Ruckgabecode retten
            //   sub rsp, 0x28                      ; Schattenraum + Ausrichtung
            //   mov rcx, rbx                       ; Argument = PlowAuto-Instanz
            //   mov rax, ExpandPlowNeighbors ; call rax
            //   add rsp, 0x28
            //   pop rax                            ; Ruckgabecode wiederherstellen
            //   jmp qword ptr [rip+0]; <abs64 +0x3A6052>   ; zurueck ins Original
            //                                                (Epilog, NICHT die
            //                                                 Fallthrough-Adresse -
            //                                                 siehe grosser Kommentar)
            //
            // Stack-Ausrichtung von Hand nachgerechnet (siehe grosser Kommentar
            // oben): an der Patch-Stelle gilt rsp mod 16 == 0 (sonst waere das
            // dort ersetzte "call 0x3AF070" im Original selbst ungueltig gewesen).
            // Jede Instruktion oben erhaelt diese Invariante bis zum finalen JMP.
            bool BuildTrampoline()
            {
                std::vector<std::uint8_t> code;
                code.reserve(96);

                // call 0x3AF070 (repliziert ueber absoluten Call statt E8 rel32,
                // da das Trampolin an einer voellig anderen Adresse liegt). Von
                // AttachPlowAreaHook() bereits vor diesem Aufruf aufgeloest.
                AppendAbsoluteCall(code, reinterpret_cast<std::uintptr_t>(gPlowArea.fnWrite));

                // neg al ; sbb eax,eax ; add eax,4 - byte-identisch repliziert.
                code.push_back(0xF6);
                code.push_back(0xD8);
                code.push_back(0x1B);
                code.push_back(0xC0);
                code.push_back(0x83);
                code.push_back(0xC0);
                code.push_back(0x04);

                // push rax
                code.push_back(0x50);

                // sub rsp, 0x28
                code.push_back(0x48);
                code.push_back(0x83);
                code.push_back(0xEC);
                code.push_back(0x28);

                // mov rcx, rbx
                code.push_back(0x48);
                code.push_back(0x89);
                code.push_back(0xD9);

                // call ExpandPlowNeighbors
                AppendAbsoluteCall(
                    code,
                    reinterpret_cast<std::uintptr_t>(&ExpandPlowNeighbors));

                // add rsp, 0x28
                code.push_back(0x48);
                code.push_back(0x83);
                code.push_back(0xC4);
                code.push_back(0x28);

                // pop rax
                code.push_back(0x58);

                // jmp qword ptr [rip+0]; <abs64 resumeAddress>
                AppendAbsoluteJump(code, gPlowArea.resumeAddress);

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

                gPlowArea.trampoline = trampoline;
                gPlowArea.trampolineSize = code.size();
                return true;
            }

            // ----------------------------------------------------------------- //
            std::array<std::uint8_t, kOriginalPatchBytes.size()> MakeSelectorPatch()
            {
                std::array<std::uint8_t, kOriginalPatchBytes.size()> patch{};
                patch.fill(0x90);

                std::vector<std::uint8_t> jump;
                AppendAbsoluteJump(jump, reinterpret_cast<std::uintptr_t>(gPlowArea.trampoline));
                std::memcpy(patch.data(), jump.data(), jump.size());

                return patch;
            }
        }

        // --------------------------------------------------------------------- //
        bool ConfigurePlowArea(const tinyxml2::XMLElement* root)
        {
            if (gPlowArea.attached)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure plow area while the hook is attached.");
                return false;
            }

            gPlowArea = {};

            if (root == nullptr)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure plow area: XML root element is null.");
                return false;
            }

            const tinyxml2::XMLElement* section =
                root->FirstChildElement("plowArea");

            // Missing or disabled section preserves the original game behaviour.
            if (section == nullptr)
                return true;

            gPlowArea.enabled = section->BoolAttribute("enabled", false);
            return true;
        }

        // --------------------------------------------------------------------- //
        void AttachPlowAreaHook()
        {
            if (!gPlowArea.enabled || gPlowArea.attached)
                return;

            Redshark::LogInfo("AttachPlowAreaHook: checkpoint A - resolving module base.");

            gPlowArea.moduleBase =
                reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (gPlowArea.moduleBase == 0)
            {
                Redshark::ShowErrorMessage(
                    "Can't attach plow area hook: main module base is null.");
                return;
            }

            gPlowArea.patchAddress = gPlowArea.moduleBase + kPatchRva;
            gPlowArea.resumeAddress = gPlowArea.moduleBase + kResumeRva;
            gPlowArea.thresholdGlobalAddress = gPlowArea.moduleBase + kThresholdGlobalRva;

            gPlowArea.fnBoundsCheck = reinterpret_cast<Fn_BoundsCheck>(
                gPlowArea.moduleBase + kFnBoundsCheckRva);
            gPlowArea.fnConsistencyCheck = reinterpret_cast<Fn_ConsistencyCheck>(
                gPlowArea.moduleBase + kFnConsistencyCheckRva);
            gPlowArea.fnReadGate = reinterpret_cast<Fn_ReadGate>(
                gPlowArea.moduleBase + kFnReadGateRva);
            gPlowArea.fnReadSubstance = reinterpret_cast<Fn_ReadSubstance>(
                gPlowArea.moduleBase + kFnReadSubstanceRva);
            gPlowArea.fnWrite = reinterpret_cast<Fn_Write>(
                gPlowArea.moduleBase + kFnWriteRva);

            Redshark::LogInfo(
                "AttachPlowAreaHook: checkpoint B - module base 0x%llx, checking patch "
                "bytes at +0x3A5FD1.",
                static_cast<unsigned long long>(gPlowArea.moduleBase));

            if (!MatchesExpectedBytes(gPlowArea.patchAddress))
            {
                Redshark::ShowErrorMessage(
                    "Can't attach plow area hook: expected code was not found at +0x3A5FD1.");
                return;
            }

            Redshark::LogInfo(
                "AttachPlowAreaHook: checkpoint C - bytes matched, building trampoline.");

            if (!BuildTrampoline())
            {
                Redshark::ShowErrorMessage(
                    "Can't attach plow area hook: trampoline allocation failed.");
                return;
            }

            Redshark::LogInfo(
                "AttachPlowAreaHook: checkpoint D - trampoline built (%zu bytes at %p), "
                "patching call site.",
                gPlowArea.trampolineSize,
                gPlowArea.trampoline);

            const auto patch = MakeSelectorPatch();
            if (!WriteExecutableMemory(
                    gPlowArea.patchAddress,
                    patch.data(),
                    patch.size()))
            {
                VirtualFree(gPlowArea.trampoline, 0, MEM_RELEASE);
                gPlowArea.trampoline = nullptr;
                gPlowArea.trampolineSize = 0;
                Redshark::ShowErrorMessage(
                    "Can't attach plow area hook: patch memory is not writable.");
                return;
            }

            Redshark::LogInfo(
                "AttachPlowAreaHook: checkpoint E - patched successfully. Plowing a tile "
                "now also converts its up to 8 neighbors (soil->LoosenSoil, "
                "steppesoil->loosensteppesoil) if their own current substance qualifies, "
                "instead of only the single directly-targeted tile.");

            gPlowArea.attached = true;
        }

        // --------------------------------------------------------------------- //
        void DetachPlowAreaHook()
        {
            if (!gPlowArea.attached)
                return;

            if (!WriteExecutableMemory(
                    gPlowArea.patchAddress,
                    kOriginalPatchBytes.data(),
                    kOriginalPatchBytes.size()))
            {
                Redshark::ShowErrorMessage(
                    "Can't detach plow area hook: original bytes could not be restored.");
                return;
            }

            gPlowArea.attached = false;

            if (gPlowArea.trampoline != nullptr)
            {
                VirtualFree(gPlowArea.trampoline, 0, MEM_RELEASE);
                gPlowArea.trampoline = nullptr;
                gPlowArea.trampolineSize = 0;
            }
        }
    }
}
