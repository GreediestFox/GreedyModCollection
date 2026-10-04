//-----------------------------------------------------------------------------
// CLAUDE_MOD: preloadZoneTextures.cs (EXPERIMENTELL)
//
// Zweck: Baum- und Groundcover-Texturen (Wald/Gras) VOR dem eigentlichen
// Spielbeitritt in den Textur-Cache laden, damit sie nicht erst waehrend/nach
// dem Beitritt sichtbar nachgeladen werden ("New streamable texture: ..."
// Eintraege im Client-Log, ca. 26 Sekunden lang nach dem World-Join in
// unserem Testlauf).
//
// HINTERGRUND / warum das passiert:
// Torque3D laedt Baum-/Groundcover-Texturen "lazy": die zugehoerigen
// Material-Objekte werden zwar schon beim Client-Start registriert
// (loadMaterials() in core.cs), aber die eigentlichen .dds-Texturdaten
// werden erst dann von der Platte gestreamt, wenn ein Objekt mit diesem
// Material tatsaechlich gerendert wird. Welche Baumarten/Groundcover-Typen
// das sind, weiss die Engine erst, sobald Welt + Kameraposition existieren -
// also erst NACH dem Ladebildschirm. Das ist kein Bug, sondern das normale
// Streaming-Verhalten der Engine (siehe core/scripts/client/streamingEvents.cs:
// "initial load" vs. laufendes Hintergrund-Streaming mit blinkendem
// StreamingIcon).
//
// Aus dem mitgelieferten Client-Log wurden die tatsaechlich NACH dem
// World-Join nachgeladenen Texturen extrahiert (160 eindeutige Dateien,
// alle 13 Baumarten + 4 Groundcover-Atlanten, ca. 341MB). Diese Liste wird
// hier per bekanntem TorqueScript-Trick (unsichtbarer GuiBitmapCtrl je
// Textur, ausserhalb des sichtbaren Bereichs, nie dem Canvas hinzugefuegt)
// erzwungen in den Textur-Cache geladen, bevor der Spieler ueberhaupt das
// Hauptmenue sieht.
//
// WICHTIG - bitte auf einem Testserver verifizieren:
// - Dieser Mechanismus (GuiBitmapCtrl mit .bitmap/.setBitmap auf eine .dds
//   ausserhalb des GUI-Kontexts) ist eine in der Torque3D-Community bekannte
//   Preload-Technik, wurde hier aber NICHT im echten Spiel getestet.
// - Die 160 Dateien stammen aus einem einzelnen Testlauf (Spawn + GM-Flug).
//   Falls dein Server mehrere, unterschiedlich bewachsene Startgebiete hat,
//   deckt diese Liste ggf. nicht alle davon ab - sie deckt aber bereits alle
//   13 im Spiel vorhandenen Baumarten ab, sollte also generell nuetzlich sein.
// - Erfolgskontrolle im Log: die "New streamable texture" Eintraege fuer
//   Baeume/Groundcover sollten jetzt VOR dem Weltbeitritt auftauchen statt
//   danach, und das sichtbare Nachladen/Pop-in von Baeumen/Gras nach dem
//   Beitritt sollte reduziert sein oder ganz wegfallen.
// - Kosten: ca. 341MB zusaetzlicher Festplatten-Lesevorgang + dauerhafter
//   VRAM-Verbrauch, der jetzt schon beim Programmstart statt erst beim
//   Spielbeitritt anfaellt (die Ladezeit wird also eher verschoben als
//   verkuerzt - das ist auch so gewollt: es soll waehrend des Wartens auf
//   dem Ladebildschirm/Hauptmenue passieren, nicht mehr sichtbar im Spiel).
//
// Wird einmalig beim Client-Start aus scripts/client/init.cs aufgerufen
// (siehe CLAUDE_MOD-Kommentar dort), also lange bevor ein Server-Beitritt
// ueberhaupt moeglich ist.
//-----------------------------------------------------------------------------

function ClaudePreloadEnvironmentTextures()
{
    if (isObject(ClaudeTexturePreloadGroup))
    {
        // Bereits in dieser Sitzung ausgefuehrt (z.B. nach Reconnect).
        return;
    }

    new SimGroup(ClaudeTexturePreloadGroup);
    // Absichtlich NICHT dem Canvas hinzufuegen - die Controls bleiben
    // unsichtbar und werden nie gerendert. Die Gruppe bleibt fuer die
    // gesamte Client-Sitzung bestehen (nicht loeschen!), damit die
    // referenzierten Texturen im GFX-Textur-Cache resident bleiben, bis
    // die echte Welt sie ohnehin braucht.

    // Hinweis: bewusst OHNE fuehrendes '%textures = "";' - ein Leerstring
    // gefolgt von SPC erzeugte im Test ein zusaetzliches leeres Wort in der
    // Liste (161 statt 160, eine "Datei nicht gefunden: " Warnung mit leerem
    // Namen). Die Liste startet daher direkt mit dem ersten echten Pfad.
    %textures = "art/Textures/GroundCover/ForestGrassesAtlas_Diffuse.dds";
    %textures = %textures SPC "art/Textures/GroundCover/SandSteppeSnowGrasses_Diffuse.dds";
    %textures = %textures SPC "art/Textures/GroundCover/SandSteppeSnowGrasses_Normal.dds";
    %textures = %textures SPC "art/Textures/GroundCover/SoilGrassesAtlas_Diffuse.dds";
    %textures = %textures SPC "art/Textures/GroundCover/SoilGrassesAtlas_Normal.dds";
    %textures = %textures SPC "art/Textures/GroundCover/SwampGrassesAtlas_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/groundcover/forestgrasses/textures/ForestGrassesAtlas_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/groundcover/sandsteppesnowgrasses/textures/SandSteppeSnowGrasses_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/groundcover/sandsteppesnowgrasses/textures/SandSteppeSnowGrasses_Normal.dds";
    %textures = %textures SPC "art/models/3d/environment/groundcover/soilgrasses/textures/SoilGrassesAtlas_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/groundcover/soilgrasses/textures/SoilGrassesAtlas_Normal.dds";
    %textures = %textures SPC "art/models/3d/environment/groundcover/swampgrasses/textures/SwampGrassesAtlas_Diffuse.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/apple/apple_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/apple_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/apple_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/apple_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/apple_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/apple_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/apple_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/apple_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/apple_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/textures/AppleTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/textures/AppleTree_Normal.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/textures/appletree_diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/apple/textures/appletree_normal.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/aspen/aspen_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/aspen_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/aspen_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/aspen_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/aspen_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/aspen_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/aspen_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/aspen_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/aspen_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/textures/AspenTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/textures/AspenTree_Normal.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/textures/aspentree_diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/aspen/textures/aspentree_normal.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/birch/birch_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/birch_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/birch_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/birch_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/birch_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/birch_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/birch_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/birch_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/birch_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/textures/BirchTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/textures/BirchTree_Normal.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/textures/birchtree_diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/birch/textures/birchtree_normal.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/elm/elm_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/elm_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/elm_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/elm_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/elm_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/elm_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/elm_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/elm_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/elm_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/textures/ElmTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/textures/ElmTree_Normal.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/textures/elmtree_diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/elm/textures/elmtree_normal.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/hazel/hazel_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/hazel_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/hazel_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/hazel_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/hazel_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/hazel_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/hazel_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/hazel_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/hazel_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/textures/HazelTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/textures/HazelTree_Normal.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/textures/hazeltree_diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/hazel/textures/hazeltree_normal.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/juniper/juniper_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/juniper/juniper_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/juniper/juniper_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/juniper/juniper_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/juniper/juniper_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/juniper/juniper_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/juniper/juniper_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/juniper/juniper_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/juniper/juniper_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/juniper/textures/JuniperTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/juniper/textures/junipertree_diffuse.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/maple/maple_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/maple_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/maple_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/maple_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/maple_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/maple_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/maple_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/maple_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/maple_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/textures/MapleTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/textures/MapleTree_Normal.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/textures/mapletree_diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/maple/textures/mapletree_normal.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/mulberry_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/mulberry_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/mulberry_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/mulberry_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/mulberry_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/mulberry_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/mulberry_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/mulberry_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/mulberry_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/textures/MulberryTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/textures/MulberryTree_Normal.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/textures/mulberrytree_diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/mulberry/textures/mulberrytree_normal.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/oak/oak_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/oak_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/oak_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/oak_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/oak_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/oak_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/oak_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/oak_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/oak_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/textures/OakTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/textures/OakTree_Normal.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/textures/oaktree_diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/oak/textures/oaktree_normal.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/pine/pine_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/pine/pine_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/pine/pine_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/pine/pine_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/pine/pine_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/pine/pine_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/pine/pine_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/pine/pine_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/pine/pine_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/pine/textures/PineTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/pine/textures/pinetree_diffuse.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/spinny/spinny_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spinny/spinny_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spinny/spinny_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spinny/spinny_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spinny/spinny_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spinny/spinny_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spinny/spinny_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spinny/spinny_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spinny/spinny_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spinny/textures/SpinnyTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spinny/textures/SpinnyTree_Normal.dds";

    %textures = %textures SPC "art/models/3d/environment/trees/spruce/spruce_11.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spruce/spruce_12.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spruce/spruce_13.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spruce/spruce_21.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spruce/spruce_22.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spruce/spruce_23.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spruce/spruce_31.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spruce/spruce_32.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spruce/spruce_33.stbin.imposter.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spruce/textures/SpruceTree_Diffuse.dds";
    %textures = %textures SPC "art/models/3d/environment/trees/spruce/textures/sprucetree_diffuse.dds";

    %count = getWordCount(%textures);
    %loaded = 0;
    %i = 0;
    while (%i < %count)
    {
        %tex = getWord(%textures, %i);
        if (isFile(%tex))
        {
            %ctrl = new GuiBitmapCtrl()
            {
                extent = "1 1";
                position = "-1000 -1000";
                visible = false;
            };
            %ctrl.setBitmap(%tex);
            ClaudeTexturePreloadGroup.add(%ctrl);
            %loaded++;
        }
        else
        {
            warn("ClaudePreloadEnvironmentTextures: Datei nicht gefunden: " @ %tex);
        }
        %i = %i + 1;
    }

    echo("ClaudePreloadEnvironmentTextures: " @ %loaded @ " von " @ %count @ " Umgebungstexturen vorab angefordert.");
}

ClaudePreloadEnvironmentTextures();
