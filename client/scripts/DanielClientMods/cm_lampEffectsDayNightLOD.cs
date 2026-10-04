//-----------------------------------------------------------------------------
// cm_lampEffectsDayNightLOD.cs
//
// Rein clientseitige Tageslicht-Steuerung fuer die drei Effekte, die an
// Objekt-ID 166 ("Lamp post", siehe Client_mod\data\cm_objects.xml) haengen:
//   - PointLight            "Lamppost_Light"
//   - ParticleEmitterNode   "Lamppost_FireEmitter" (eigene Kopie von
//                           "FireEmitterLiF2Z", siehe Abschnitt weiter unten)
//   - ParticleEmitterNode   "Motes"
//
// Ziel: bei hellem Tageslicht werden diese drei Effekte fuer ALLE Laternen
// auf der Karte gemeinsam deaktiviert (Renderkosten sparen), bei Daemmerung/
// Nacht wieder aktiviert.
//
// VORGESCHICHTE (warum NICHT distanzbasiert pro Laterne):
// Eine praezise, pro-Laterne-Entfernung genaue Loesung ist aus einem
// unsignierten Client-Mod-Script nachweislich NICHT moeglich: jede
// Positionsabfrage (getPosition(), .position, getWorldBox(),
// getWorldBoxCenter()) liefert fuer normale Spieler (nicht Gamemaster) auf
// JEDEM SceneObject einen ungueltigen NaN-Vektor - auch auf dem eigenen,
// selbstgesteuerten Spielerobjekt. Objekt-Kennungen (getObjectGID/
// getGhostID/getInternalName/getFilename/dynamische Felder) sind ebenfalls
// keine Grundlage fuer eine Server-Client-Zuordnung pro Laterne. Eine
// serverseitige, grobe "Alle an/Alle aus"-Loesung (Server prueft Naehe zu
// irgendeinem gebauten Objekt) wurde gebaut und erfolgreich getestet, vom
// Nutzer aber verworfen, da alle Laternen des Clients gemeinsam schalten,
// nicht einzeln pro Laterne.
//
// STATTDESSEN: Tageslicht-basiert, komplett clientseitig.
// ScatterSky::elevation (Sonnenstand ueber dem Horizont in Grad) ist laut
// Engine-Referenz eines von nur ZWEI tatsaechlich live vernetzten Feldern
// des ScatterSky-Objekts ("Only azimuth and elevation are networked
// fields."). Das ist ein reiner Feld-Lesezugriff auf ein Objekt (keine
// Positionsabfrage) und daher NICHT von der Rechtesperre betroffen - genau
// wie color/brightness/castShadows auf den Lichtern bereits erfolgreich
// gelesen werden (siehe LampDayNightLOD::isLight unten). Diese Loesung
// braucht daher keinerlei Server-Aenderung.
//
// Das ScatterSky-Objekt wird bei uns NICHT ueber eine feste Script-Datei
// geladen (Client\scripts\client\init.cs hat die Zeile
// "//exec("./cm_environment.cs");" auskommentiert) - die tatsaechliche
// Instanz kommt aus den vom Server geladenen Missionsdaten und kann anders
// heissen als der dortige Platzhaltername "theSky". Deshalb wird sie hier
// einmalig per Klassensuche gefunden und die ID zwischengespeichert
// (LampDayNightLOD::findScatterSky). Gesucht wird ab "MissionGroup" (der
// Standard-Wurzelgruppe fuer alle geladenen Missions-/Levelinhalte, siehe
// z.B. Client\scripts\client\serverConnection.cs), mit RootGroup als
// Fallback, falls MissionGroup noch nicht existiert.
//
// KALIBRIERUNG (per Test mit morning()/noon()/evening()/midnight() ermittelt):
// morning() ~ elevation 0.3 Grad (Sonne gerade ueber dem Horizont)
// noon()    ~ elevation 46 Grad (hellichter Tag)
// evening() ~ elevation -1 Grad (Sonne gerade unter dem Horizont)
//
// ZWEI unabhaengige Schwellenwerte statt einem einzelnen Wert mit
// symmetrischer Hysterese: per Test wurden Einschalt- und Ausschalt-Punkt
// jeweils getrennt voneinander eingeregelt (Ausschalten bei elevation > 1.5,
// Einschalten bei elevation < -1) - das laesst sich mit einem gemeinsamen
// Schwellenwert +/- Hysterese nicht unabhaengig genug einstellen. Kein
// Flackern, da beim Vergleich immer nur EIN Wert (je nach aktuellem Zustand)
// herangezogen wird:
//   - im Zustand AN bleibt es AN, bis elevation > ElevationOff
//   - im Zustand AUS bleibt es AUS, bis elevation < ElevationOn
//
// Alle $pref::LampDayNightLOD::* Variablen koennen ueber die Konsole oder
// prefs.cs angepasst werden, ohne dieses Script zu editieren.
//-----------------------------------------------------------------------------

$pref::LampDayNightLOD::Enabled      = true;
$pref::LampDayNightLOD::ElevationOn  = -1;    // Grad - Effekte AN, sobald die Elevation beim Sinken darunter faellt
$pref::LampDayNightLOD::ElevationOff = 1.5;   // Grad - Effekte AUS, sobald die Elevation beim Steigen darueber steigt
$pref::LampDayNightLOD::Interval     = 60000; // Millisekunden zwischen zwei Pruefungen
$pref::LampDayNightLOD::Debug        = false; // true = Details in der Konsole ausgeben

$LampDayNightLOD::ScheduleId     = -1;
$LampDayNightLOD::SkyObjectId    = -1;   // -1 = noch nicht gesucht/gefunden
$LampDayNightLOD::EffectsEnabled = true; // aktueller Soll-Zustand der drei Effekte

//-----------------------------------------------------------------------------
// Sucht rekursiv das erste Objekt der Klasse "ScatterSky" ab %group.
// %depth begrenzt die Rekursionstiefe als Sicherheitsnetz.
//-----------------------------------------------------------------------------
function LampDayNightLOD::findScatterSkyIn(%group, %depth)
{
   if(!isObject(%group) || %depth > 8)
      return -1;

   %count = %group.getCount();
   for(%i = 0; %i < %count; %i++)
   {
      %obj = %group.getObject(%i);
      if(!isObject(%obj))
         continue;

      if(%obj.getClassName() $= "ScatterSky")
         return %obj.getId();

      // Rekursiv absteigen, falls %obj selbst eine Gruppe/Set ist (native
      // Methode, daher isMethod() als bareword-Objektmethode pruefen - siehe
      // Vorbild %obj.isMethod(getPosition) in serverConnection.cs).
      if(%obj.isMethod(getCount))
      {
         %found = LampDayNightLOD::findScatterSkyIn(%obj, %depth + 1);
         if(%found != -1)
            return %found;
      }
   }

   return -1;
}

function LampDayNightLOD::findScatterSky()
{
   if(isObject($LampDayNightLOD::SkyObjectId))
      return $LampDayNightLOD::SkyObjectId;

   // Schneller Pfad zuerst: in den bisher beobachteten Welten heisst die
   // geladene ScatterSky-Instanz "theSky" (siehe cm_logger.cs, das denselben
   // globalen Namen direkt referenziert). Erst wenn das fehlschlaegt, per
   // rekursiver Klassensuche ab MissionGroup/RootGroup suchen (Fallback,
   // falls der Name in einer anderen Welt/Version einmal abweicht).
   if(isObject(theSky) && theSky.getClassName() $= "ScatterSky")
   {
      $LampDayNightLOD::SkyObjectId = theSky.getId();
   }
   else
   {
      %root = isObject(MissionGroup) ? MissionGroup : RootGroup;
      $LampDayNightLOD::SkyObjectId = LampDayNightLOD::findScatterSkyIn(%root, 0);
   }

   if($pref::LampDayNightLOD::Debug)
   {
      if(isObject($LampDayNightLOD::SkyObjectId))
         echo("LampDayNightLOD: ScatterSky gefunden, id=" @ $LampDayNightLOD::SkyObjectId
            @ (isObject(theSky) ? " (via theSky)" : " (via Klassensuche)"));
      else
         echo("LampDayNightLOD: ScatterSky noch nicht gefunden (Mission evtl. noch nicht geladen).");
   }

   return $LampDayNightLOD::SkyObjectId;
}

//-----------------------------------------------------------------------------
// Diagnose fuer den Fall, dass ScatterSky weiterhin nicht gefunden wird:
// listet die obersten Eintraege von MissionGroup (Klasse + Name), damit wir
// sehen, wie die Missionsdaten tatsaechlich aufgebaut sind.
//-----------------------------------------------------------------------------
function LampDayNightLOD::dumpMissionGroup(%limit)
{
   if(%limit <= 0)
      %limit = 40;

   echo("--------------------------------------------------------------");
   echo("LampDayNightLOD::dumpMissionGroup:");
   echo("  isObject(theSky)=" @ isObject(theSky) @ (isObject(theSky) ? ("  Klasse=" @ theSky.getClassName()) : ""));
   echo("  isObject(MissionGroup)=" @ isObject(MissionGroup));

   if(!isObject(MissionGroup))
   {
      echo("--------------------------------------------------------------");
      return;
   }

   %count = MissionGroup.getCount();
   %shown = mMin(%count, %limit);
   for(%i = 0; %i < %shown; %i++)
   {
      %obj = MissionGroup.getObject(%i);
      if(!isObject(%obj))
         continue;
      echo("  [" @ %i @ "] " @ %obj.getClassName() @ "  Name=[" @ %obj.getName() @ "]  id=" @ %obj.getId());
   }
   echo("  Insgesamt " @ %count @ " Eintraege in MissionGroup (davon " @ %shown @ " angezeigt).");
   echo("--------------------------------------------------------------");
}

//-----------------------------------------------------------------------------
// Eigenstaendiger Feuer-Partikel-Datenblock NUR fuer die Laterne.
//
// Hintergrund: Das ORIGINAL "FireEmitterLiF2Z" (siehe Client\art\datablocks\
// particles.cs) wird nicht nur von "Lamp post" (166) verwendet, sondern auch
// von "Floor lamp" (86, 4x) und "Powderkeg"/"Bomb" (1151, 1x). Da sich
// Kind-Objekte in CmChildObjectsGroup NICHT ihrem ComplexObject zuordnen
// lassen (weder ueber Position noch ueber isMounted()/getObjectMount() -
// letzteres liefert hier durchweg isMounted=0, per Test bestaetigt), wuerde
// ein Match auf den Original-Namen faelschlich auch Floor lamp und Powderkeg
// mitschalten.
//
// Loesung: Eigene Kopie des Datenblocks unter neuem Namen "Lamppost_FireEmitter"
// (alle Werte 1:1 von FireEmitterLiF2Z uebernommen, "id"-Feld bewusst
// weggelassen - das war im Original nur eine interne Revisionsnummer,
// siehe "//CM_REV"-Kommentare in particles.cs, kein Pflichtfeld fuer
// TorqueScript-Datenbloecke; die Engine vergibt die interne ID automatisch).
// In Client_mod\data\cm_objects.xml wurde NUR bei Objekt 166 (Zustand
// "Complete") der <emitter>-Verweis von "FireEmitterLiF2Z" auf
// "Lamppost_FireEmitter" umgestellt - Floor lamp und Powderkeg bleiben
// unveraendert beim Original und werden von dieser Schaltung nie beruehrt.
//-----------------------------------------------------------------------------
datablock ParticleEmitterData(Lamppost_FireEmitter)
{
   ejectionPeriodMS = "50";
   periodVarianceMS = "1";
   ejectionVelocity = "0.1";
   velocityVariance = "0.1";
   thetaMin         = "0";
   thetaMax         = "50";
   particles        = "FireParticleLiF2Z";
   blendStyle = "ADDITIVE";
   ejectionOffset = "0.05";
   softnessDistance = "10";
   ambientFactor = "0";
};

//-----------------------------------------------------------------------------
// Erkennung der drei Ziel-Effekttypen.
//-----------------------------------------------------------------------------

// Fingerprint fuer "Lamppost_Light": echte, gebaute Instanzen sind unbenannt
// (im Gegensatz zu den Prototyp-Singletons aus cm_lightProto.cs) und werden
// daher ueber color/brightness/castShadows identifiziert. Lamppost_Light ist
// die einzige Lichtdefinition mit color ~= (1.0, 0.45, 0.05), brightness
// ~= 0.1 und castShadows = 0.
function LampDayNightLOD::isLight(%obj)
{
   if(%obj.getClassName() !$= "PointLight" || %obj.getName() !$= "")
      return false;

   %c = %obj.color;
   %r = getWord(%c, 0);
   %g = getWord(%c, 1);
   %b = getWord(%c, 2);

   return ( mAbs(%r - 1.0)  < 0.01 &&
            mAbs(%g - 0.45) < 0.01 &&
            mAbs(%b - 0.05) < 0.01 &&
            mAbs(%obj.brightness - 0.1) < 0.01 &&
            !%obj.castShadows );
}

// ParticleEmitterNode-Instanzen tragen eine direkte Referenz auf ihr
// ParticleEmitterData-Datenblock (Feld "emitter") - dessen Name entspricht
// exakt dem <emitter>-Tag in cm_objects.xml. "Motes" ist (nach Pruefung von
// Lamp post/Floor lamp/Powderkeg) exklusiv der Laterne vorbehalten,
// "Lamppost_FireEmitter" ist unsere eigene, garantiert exklusive Kopie (s.o.)
// - das ORIGINAL "FireEmitterLiF2Z" wird hier bewusst NICHT mehr gematcht.
function LampDayNightLOD::isParticle(%obj)
{
   if(%obj.getClassName() !$= "ParticleEmitterNode")
      return false;

   if(!isObject(%obj.emitter))
      return false;

   %name = %obj.emitter.getName();
   return (%name $= "Lamppost_FireEmitter" || %name $= "Motes");
}

function LampDayNightLOD::isTarget(%obj)
{
   if(!isObject(%obj))
      return false;

   return LampDayNightLOD::isLight(%obj) || LampDayNightLOD::isParticle(%obj);
}

//-----------------------------------------------------------------------------
// Iteriert CmChildObjectsGroup (dort landen laut Diagnose alle Kind-Objekte
// - Lichter, Partikel, Decals, Sounds - aller ComplexObjects auf der Karte)
// und ruft %callbackName(%obj) fuer jedes Ziel-Objekt auf.
//-----------------------------------------------------------------------------
function LampDayNightLOD::forEachTarget(%callbackName)
{
   %checked = 0;

   if(!isObject(CmChildObjectsGroup))
      return %checked;

   %count = CmChildObjectsGroup.getCount();
   for(%i = 0; %i < %count; %i++)
   {
      %obj = CmChildObjectsGroup.getObject(%i);

      if(!LampDayNightLOD::isTarget(%obj))
         continue;

      %checked++;
      eval(%callbackName @ "(" @ %obj @ ");");
   }

   return %checked;
}

//-----------------------------------------------------------------------------
// Schaltet ein einzelnes Ziel-Objekt gemaess $LampDayNightLOD::PendingEnabledState.
// Licht: setLightEnabled(). Partikel: setActive() (analoge Methode, siehe
// ParticleEmitterNode-Referenz).
//-----------------------------------------------------------------------------
function LampDayNightLOD::setOneEnabled(%obj)
{
   if(%obj.getClassName() $= "PointLight")
      %obj.setLightEnabled($LampDayNightLOD::PendingEnabledState);
   else
      %obj.setActive($LampDayNightLOD::PendingEnabledState);
}

function LampDayNightLOD::setAllEnabled(%enabled)
{
   $LampDayNightLOD::PendingEnabledState = %enabled ? true : false;
   %checked = LampDayNightLOD::forEachTarget("LampDayNightLOD::setOneEnabled");

   if($pref::LampDayNightLOD::Debug)
      echo("LampDayNightLOD: " @ %checked @ " Laternen-Effekt-Instanzen (Licht+Feuer+Motes) auf "
         @ ($LampDayNightLOD::PendingEnabledState ? "AN" : "AUS") @ " gesetzt.");
}

//-----------------------------------------------------------------------------
// Periodische Pruefung des Sonnenstands mit ZWEI unabhaengigen
// Schwellenwerten (siehe Kalibrierungs-Kommentar am Dateianfang):
//   - im Zustand AN bleibt es AN, bis elevation > ElevationOff
//   - im Zustand AUS bleibt es AUS, bis elevation < ElevationOn
//
// WICHTIG: setAllEnabled() wird JEDEN Tick aufgerufen, nicht nur bei einem
// Wechsel des Soll-Zustands. Grund (per Test gefunden): interagiert ein
// Spieler tagsueber mit einer Laterne (z.B. an-/ausknipsen), erzeugt die
// Engine dafuer ganz neue Licht-/Partikel-Kindobjekte, die im jeweiligen
// Default-Zustand (an) starten - unabhaengig davon, ob gerade Tag oder Nacht
// ist. Ohne staendiges Neu-Anwenden blieben solche frisch erzeugten
// Instanzen dauerhaft an, weil kein Tag/Nacht-Wechsel (und damit kein
// Trigger) mehr stattfindet, bis zur naechsten Daemmerung. Der Soll-Zustand
// wird daher bei jedem Tick auf ALLE gefundenen Instanzen angewendet - das
// ist bei einem 60-Sekunden-Intervall und rein lokaler Iteration (keine
// Netzwerk-/Positionsabfrage) vernachlaessigbar teuer.
//-----------------------------------------------------------------------------
function LampDayNightLOD::update()
{
   $LampDayNightLOD::ScheduleId = schedule($pref::LampDayNightLOD::Interval, 0, "LampDayNightLOD::update");

   if(!$pref::LampDayNightLOD::Enabled)
      return;

   %sky = LampDayNightLOD::findScatterSky();
   if(!isObject(%sky))
      return; // Himmel/Mission noch nicht geladen - naechster Versuch beim naechsten Tick

   %elevation = %sky.elevation;

   if($LampDayNightLOD::EffectsEnabled)
      %shouldEnable = (%elevation < $pref::LampDayNightLOD::ElevationOff);
   else
      %shouldEnable = (%elevation < $pref::LampDayNightLOD::ElevationOn);

   if($pref::LampDayNightLOD::Debug)
      echo("LampDayNightLOD: elevation=" @ %elevation
         @ "  aktuell=" @ ($LampDayNightLOD::EffectsEnabled ? "AN" : "AUS")
         @ "  soll=" @ (%shouldEnable ? "AN" : "AUS"));

   $LampDayNightLOD::EffectsEnabled = %shouldEnable;
   LampDayNightLOD::setAllEnabled(%shouldEnable);
}

//-----------------------------------------------------------------------------
// Diagnose-/Testwerkzeuge (per Konsole aufrufbar).
//-----------------------------------------------------------------------------

// Sofortige Pruefung ohne auf den naechsten Tick zu warten.
function LampDayNightLOD::forceCheck()
{
   %sky = LampDayNightLOD::findScatterSky();
   if(!isObject(%sky))
   {
      echo("LampDayNightLOD::forceCheck: ScatterSky nicht gefunden.");
      return;
   }

   echo("LampDayNightLOD::forceCheck: elevation=" @ %sky.elevation
      @ "  ElevationOn=" @ $pref::LampDayNightLOD::ElevationOn
      @ "  ElevationOff=" @ $pref::LampDayNightLOD::ElevationOff
      @ "  aktueller Zustand=" @ ($LampDayNightLOD::EffectsEnabled ? "AN" : "AUS"));
}

// Aktuellen Status ausgeben (Himmel gefunden?, elevation, Zustand).
function LampDayNightLOD::status()
{
   %sky = $LampDayNightLOD::SkyObjectId;
   echo("--------------------------------------------------------------");
   echo("LampDayNightLOD Status:");
   echo("  Enabled=" @ $pref::LampDayNightLOD::Enabled
      @ "  ElevationOn=" @ $pref::LampDayNightLOD::ElevationOn
      @ "  ElevationOff=" @ $pref::LampDayNightLOD::ElevationOff
      @ "  Interval=" @ $pref::LampDayNightLOD::Interval);
   echo("  ScatterSky-Id=" @ %sky @ "  gefunden=" @ isObject(%sky));
   if(isObject(%sky))
      echo("  elevation=" @ %sky.elevation @ "  azimuth=" @ %sky.azimuth);
   echo("  EffectsEnabled=" @ $LampDayNightLOD::EffectsEnabled);
   echo("--------------------------------------------------------------");
}

// Listet die ersten %limit gefundenen Ziel-Objekte auf (Klasse, bei
// Partikeln zusaetzlich der Emitter-Name) - zur Kontrolle, ob die Erkennung
// tatsaechlich funktioniert.
function LampDayNightLOD::dumpTargets(%limit)
{
   if(%limit <= 0)
      %limit = 20;

   echo("--------------------------------------------------------------");
   echo("LampDayNightLOD::dumpTargets (max " @ %limit @ "):");

   if(!isObject(CmChildObjectsGroup))
   {
      echo("  CmChildObjectsGroup existiert nicht.");
      return;
   }

   %count = CmChildObjectsGroup.getCount();
   %shown = 0;
   for(%i = 0; %i < %count && %shown < %limit; %i++)
   {
      %obj = CmChildObjectsGroup.getObject(%i);
      if(!LampDayNightLOD::isTarget(%obj))
         continue;

      %shown++;
      if(%obj.getClassName() $= "PointLight")
         echo("  [" @ %shown @ "] PointLight  id=" @ %obj.getId() @ "  enabled=" @ %obj.isEnabled);
      else
         echo("  [" @ %shown @ "] ParticleEmitterNode  id=" @ %obj.getId()
            @ "  emitter=" @ %obj.emitter.getName() @ "  active=" @ %obj.active);
   }

   echo("  Insgesamt " @ %shown @ " von " @ %count @ " CmChildObjectsGroup-Eintraegen gefunden.");
   echo("--------------------------------------------------------------");
}

//-----------------------------------------------------------------------------
// Untersucht, ob sich "FireEmitterLiF2Z"-Instanzen an "Lamp post" (166) von
// denen an "Floor lamp" (86)/"Powderkeg" (1151) unterscheiden lassen, ohne
// Positionsdaten zu brauchen: SceneObject::isMounted()/getObjectMount() ist
// KEINE Positionsabfrage, sondern liefert nur die ID des Objekts, an dem
// dieses Kind haengt (mountet) - falls unsere Kind-Objekte tatsaechlich
// gemountet sind, koennten wir darueber den Elterntyp bestimmen. Testet pro
// gefundenem Ziel-Partikel: isMounted(), getObjectMount(), sowie - falls
// vorhanden (per isMethod() abgesichert, siehe LampDayNightLOD::isLight
// Vorbild) - Klasse/Shape/Datenblock/dynamische Felder des Elternobjekts.
// Ergebnis bitte einmal in-game ausfuehren und das Log pruefen, bevor wir
// eine Ausschlussregel bauen.
//-----------------------------------------------------------------------------
function LampDayNightLOD::dumpMountInfo(%limit)
{
   if(%limit <= 0)
      %limit = 20;

   echo("--------------------------------------------------------------");
   echo("LampDayNightLOD::dumpMountInfo (max " @ %limit @ "):");

   if(!isObject(CmChildObjectsGroup))
   {
      echo("  CmChildObjectsGroup existiert nicht.");
      return;
   }

   %count = CmChildObjectsGroup.getCount();
   %shown = 0;
   for(%i = 0; %i < %count && %shown < %limit; %i++)
   {
      %obj = CmChildObjectsGroup.getObject(%i);
      if(!LampDayNightLOD::isParticle(%obj))
         continue;

      %shown++;
      %mounted = %obj.isMounted();
      %mountId = %mounted ? %obj.getObjectMount() : -1;

      echo("  [" @ %shown @ "] id=" @ %obj.getId() @ "  emitter=" @ %obj.emitter.getName()
         @ "  isMounted=" @ %mounted @ "  mountId=" @ %mountId);

      if(isObject(%mountId))
      {
         %line = "        Mount-Objekt: Klasse=" @ %mountId.getClassName();

         if(%mountId.isMethod(getShapeName))
            %line = %line @ "  ShapeName=" @ %mountId.getShapeName();
         if(%mountId.isMethod(getDataBlock) && isObject(%mountId.getDataBlock()))
            %line = %line @ "  DataBlock=" @ %mountId.getDataBlock().getName();

         %line = %line @ "  DynFieldCount=" @ %mountId.getDynamicFieldCount();
         echo(%line);
      }
   }

   echo("  Insgesamt " @ %shown @ " Partikel-Ziele untersucht.");
   echo("--------------------------------------------------------------");
}

//-----------------------------------------------------------------------------
function LampDayNightLOD::start()
{
   LampDayNightLOD::stop();
   $LampDayNightLOD::ScheduleId = schedule($pref::LampDayNightLOD::Interval, 0, "LampDayNightLOD::update");
}

function LampDayNightLOD::stop()
{
   if($LampDayNightLOD::ScheduleId != -1 && isEventPending($LampDayNightLOD::ScheduleId))
      cancel($LampDayNightLOD::ScheduleId);
   $LampDayNightLOD::ScheduleId = -1;
}

//-----------------------------------------------------------------------------
echo("LampDayNightLOD: Script geladen. Enabled=" @ $pref::LampDayNightLOD::Enabled
   @ " ElevationOn=" @ $pref::LampDayNightLOD::ElevationOn
   @ " ElevationOff=" @ $pref::LampDayNightLOD::ElevationOff
   @ " Interval=" @ $pref::LampDayNightLOD::Interval);
LampDayNightLOD::start();
