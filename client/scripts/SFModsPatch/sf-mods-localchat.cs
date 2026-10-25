// SF Mods add-on: local chat colours by number of players nearby (opt-in).
// Nearby players = other players within $pref::SFMods::LC::Radius metres (ghosted players only).
// Level = min(players nearby, Max); every level has its own colour code (RRGGBB) that can be edited in the window (Ctrl+Shift+L or sfLC()).
// Settings are stored in data/prefs.cs together with the other SF Mods settings.

function SFLC_defaults()
{
   if ($pref::SFMods::LC::Enabled $= "") $pref::SFMods::LC::Enabled = 0;
   if ($pref::SFMods::LC::Max $= "") $pref::SFMods::LC::Max = 8;
   if ($pref::SFMods::LC::Radius $= "") $pref::SFMods::LC::Radius = 40;
   %def = "9fd6ff 7fe0c8 8ff08f c8f070 f0e060 f0b050 f08a4a f05a5a f050a8 c050f0 8a70f0 6a9af0";
   for (%i = 1; %i <= 12; %i++) {
      if ($pref::SFMods::LC::Color[%i] $= "") $pref::SFMods::LC::Color[%i] = getWord(%def, %i - 1);
   }
}
SFLC_defaults();

function SFLC_save()
{
   export("$pref::*", "data/prefs.cs", false);
}

function SFLC_validHex(%h)
{
   return strlen(%h) == 6 && strlen(stripChars(strlwr(%h), "0123456789abcdef")) == 0;
}

function SFLC_countNearby()
{
   if (!isObject(ServerConnection) || !isObject(ServersGroup)) return 0;
   %me = ServerConnection.getControlObject();
   if (!isObject(%me)) return 0;
   %mp = %me.getPosition();
   %count = 0;
   for (%n = 0; %n < ServersGroup.getCount(); %n++) {
      %grp = ServersGroup.getObject(%n);
      for (%i = 0; %i < %grp.getCount(); %i++) {
         %obj = %grp.getObject(%i);
         if (!isObject(%obj) || %obj == %me) continue;
         if (%obj.getClassName() !$= "Player") continue;
         if (VectorLen(VectorSub(%obj.getPosition(), %mp)) <= $pref::SFMods::LC::Radius) %count++;
      }
   }
   return %count;
}

function SFLC_colorize(%text)
{
   if (!$pref::SFMods::LC::Enabled) return %text;
   // line already coloured by the sender ("=<color:...>"): leave it alone
   if (getSubStr(%text, 0, 1) $= "=") return %text;
   %max = $pref::SFMods::LC::Max;
   if (%max < 1) %max = 1;
   if (%max > 12) %max = 12;
   %n = $SFLC::forceLevel !$= "" ? $SFLC::forceLevel : SFLC_countNearby();
   %level = %n > %max ? %max : %n;
   echo("[SFLocalChat] nearby=" @ %n @ " level=" @ %level);
   if (%level < 1) return %text;
   %hex = $pref::SFMods::LC::Color[%level];
   if (!SFLC_validHex(%hex)) return %text;
   return "=<color:" @ strupr(%hex) @ ">" @ %text;
}

package SFLocalChatColors
{
   function clientCmdLocalChatMessage(%sender, %position, %text)
   {
      if (!$SFLC::inPreview) echo("[SFLCRAW] sender=" @ expandEscape(%sender) @ " text=" @ expandEscape(%text));
      return Parent::clientCmdLocalChatMessage(%sender, %position, SFLC_colorize(%text));
   }
};
activatePackage(SFLocalChatColors);

// ---- settings window
function SFLC_label(%parent, %text, %x, %y, %w)
{
   %c = new GuiMLTextCtrl() {
      text = %text;
      position = %x SPC %y;
      extent = %w SPC "18";
      minExtent = "8 2";
      profile = "GuiMLTextProfile";
      visible = "1";
      active = "1";
      isContainer = "0";
   };
   %parent.add(%c);
   return %c;
}

function SFLC_edit(%parent, %name, %text, %x, %y, %w, %len)
{
   %c = new GuiTextEditCtrl(%name) {
      text = %text;
      position = %x SPC %y;
      extent = %w SPC "20";
      minExtent = "8 2";
      maxLength = %len;
      profile = "GuiTextEditProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
   };
   %parent.add(%c);
   return %c;
}

function SFLC_button(%parent, %text, %cmd, %x, %y, %w)
{
   %c = new GuiButtonCtrl() {
      text = %text;
      position = %x SPC %y;
      extent = %w SPC "24";
      minExtent = "8 8";
      profile = "GuiButtonProfile";
      visible = "1";
      active = "1";
      command = %cmd;
      isContainer = "0";
   };
   %parent.add(%c);
   return %c;
}

function SFLC_build()
{
   if (isObject(SFLCRoot)) return;
   %root = new GuiControl(SFLCRoot) {
      position = "0 0";
      extent = "100% 100%";
      horizSizing = "width";
      vertSizing = "height";
      profile = "GuiDefaultProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
   };
   %win = new GuiWindowCtrl(SFLCWin) {
      text = "SF Mods - Local Chat Colours";
      resizeWidth = "0";
      resizeHeight = "0";
      canMove = "1";
      canClose = "1";
      canMinimize = "0";
      canMaximize = "0";
      canCollapse = "0";
      position = "300 120";
      extent = "380 500";
      minExtent = "48 92";
      horizSizing = "center";
      vertSizing = "center";
      profile = "GuiWindowProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
      closeCommand = "Canvas.popDialog(SFLCRoot);";
   };
   %root.add(%win);
   %chk = new GuiCheckBoxCtrl(SFLC_enabled) {
      text = "Colour local chat by players nearby (opt-in)";
      position = "20 44";
      extent = "340 24";
      minExtent = "8 2";
      profile = isObject(SFGuiCheckBoxProfile) ? "SFGuiCheckBoxProfile" : "GuiCheckBoxProfile";
      visible = "1";
      active = "1";
      isContainer = "0";
   };
   %win.add(%chk);
   SFLC_label(%win, "Range (metres)", 20, 76, 150);
   SFLC_edit(%win, SFLC_radius, $pref::SFMods::LC::Radius, 180, 74, 60, 4);
   SFLC_label(%win, "Max players (2-12)", 20, 102, 150);
   SFLC_edit(%win, SFLC_max, $pref::SFMods::LC::Max, 180, 100, 60, 2);
   SFLC_label(%win, "<font:Arial:14>Colour codes (RRGGBB)", 20, 134, 300);
   for (%i = 1; %i <= 12; %i++) {
      %y = 158 + (%i - 1) * 24;
      SFLC_label(%win, %i @ (%i == 1 ? " player" : " players"), 20, %y + 2, 90);
      %nm = "SFLC_col" @ %i;
      SFLC_edit(%win, %nm, $pref::SFMods::LC::Color[%i], 120, %y, 80, 6);
      %sm = new GuiMLTextCtrl("SFLC_smp" @ %i) {
         text = "";
         position = "216 " @ (%y + 2);
         extent = "150 18";
         minExtent = "8 2";
         profile = "GuiMLTextProfile";
         visible = "1";
         active = "1";
         isContainer = "0";
      };
      %win.add(%sm);
   }
   SFLC_button(%win, "Apply + save", "SFLC_apply();", 20, 462, 110);
   SFLC_button(%win, "Preview in chat", "SFLC_preview();", 140, 462, 120);
   SFLC_button(%win, "Reset colours", "SFLC_resetColors();", 270, 462, 100);
}

function SFLC_fillOne(%p)
{
   if (!isObject(%p @ "enabled")) return;
   (%p @ "enabled").setStateOn($pref::SFMods::LC::Enabled ? 1 : 0);
   (%p @ "radius").setText($pref::SFMods::LC::Radius);
   (%p @ "max").setText($pref::SFMods::LC::Max);
   for (%i = 1; %i <= 12; %i++) {
      %h = $pref::SFMods::LC::Color[%i];
      (%p @ "col" @ %i).setText(%h);
      (%p @ "smp" @ %i).setText(SFLC_validHex(%h) ? "<color:" @ strupr(%h) @ ">" @ (%p $= "SFLT_" ? "Text" : "Sample text") : "<color:FF5050>bad");
   }
}

function SFLC_applyOne(%p)
{
   if (!isObject(%p @ "enabled")) return;
   $pref::SFMods::LC::Enabled = (%p @ "enabled").isStateOn() ? 1 : 0;
   %r = mFloor((%p @ "radius").getValue());
   $pref::SFMods::LC::Radius = %r < 1 ? 40 : (%r > 500 ? 500 : %r);
   %m = mFloor((%p @ "max").getValue());
   $pref::SFMods::LC::Max = %m < 2 ? 2 : (%m > 12 ? 12 : %m);
   for (%i = 1; %i <= 12; %i++) {
      %h = strlwr(trim((%p @ "col" @ %i).getValue()));
      if (SFLC_validHex(%h)) $pref::SFMods::LC::Color[%i] = %h;
   }
   SFLC_save();
   SFLC_fill();
}
function SFLC_apply()
{
   SFLC_applyOne("SFLC_");
}
function SFLT_apply()
{
   SFLC_applyOne("SFLT_");
}
function SFLC_fill()
{
   SFLC_fillOne("SFLC_");
   SFLC_fillOne("SFLT_");
}

function SFLC_resetColors()
{
   for (%i = 1; %i <= 12; %i++) $pref::SFMods::LC::Color[%i] = "";
   SFLC_defaults();
   SFLC_fill();
}

// writes one sample line per level into the local chat (through the same colouring code)
function SFLC_preview()
{
   SFLC_apply();
   SFLC_previewRun();
}
function SFLT_preview()
{
   SFLT_apply();
   SFLC_previewRun();
}
function SFLC_previewRun()
{
   %was = $pref::SFMods::LC::Enabled;
   $pref::SFMods::LC::Enabled = 1;
   $SFLC::inPreview = 1;
   for (%i = 1; %i <= $pref::SFMods::LC::Max; %i++) {
      $SFLC::forceLevel = %i;
      clientCmdLocalChatMessage("Preview", "0 0 0", "Colour for " @ %i @ (%i == 1 ? " player" : " players") @ " nearby");
   }
   $SFLC::forceLevel = "";
   $SFLC::inPreview = 0;
   $pref::SFMods::LC::Enabled = %was;
}

function sfLC()
{
   SFLC_build();
   if (SFLCRoot.isAwake()) {
      Canvas.popDialog(SFLCRoot);
   } else {
      SFLC_fill();
      Canvas.pushDialog(SFLCRoot);
   }
}
GlobalActionMap.bindCmd(keyboard, "ctrl shift l", "sfLC();", "");

// diagnostics for the planned SF window tab: write the SF window's control tree to the log once
function SFLC_dump(%c, %ind)
{
   echo("[SFLCDUMP] " @ %ind @ %c.getClassName() SPC %c.getName() SPC %c.internalName SPC "pos=" @ %c.position SPC "ext=" @ %c.extent);
   if (%c.getClassName() $= "GuiWindowCtrl" || %c.isMemberOfClass("GuiControl")) {
      for (%i = 0; %i < %c.getCount(); %i++) SFLC_dump(%c.getObject(%i), %ind @ "  ");
   }
}


// ---- tab "Chat" inside the SF Mods window (4th tab row; the panes move down 23 px)
function SFLT_build()
{
   if ($SFLT::built || !isObject(SFGuiWindowCtrl)) return;
   %ref = SFGuiWindowCtrl.findObjectByInternalName("SFGuiButtonTools", 1);
   if (!isObject(%ref)) { warn("SFLT: Tools button not found, no Chat tab"); return; }
   $SFLT::built = 1;
   %w = SFGuiWindowCtrl;
   for (%i = 0; %i < %w.getCount(); %i++) {
      %c = %w.getObject(%i);
      if (strstr(%c.internalName, "SFGuiPane") == 0) %c.position = getWord(%c.position, 0) SPC (getWord(%c.position, 1) + 23);
   }
   %btn = new GuiButtonCtrl(SFGuiButtonLChat) {
      text = "Chat";
      position = "15 95";
      extent = "56 24";
      minExtent = "8 2";
      profile = %ref.profile;
      buttonType = %ref.buttonType;
      groupNum = %ref.groupNum;
      visible = "1";
      active = "1";
      command = "sfWindowShowLChat();";
      isContainer = "0";
   };
   %w.add(%btn);
   %pane = new GuiBitmapBorderCtrl() {
      internalName = "SFGuiPaneLChat";
      position = "15 118";
      extent = "262 262";
      minExtent = "8 2";
      profile = %w.findObjectByInternalName("SFGuiPaneTools", 1).profile;
      visible = "0";
      active = "1";
      isContainer = "1";
   };
   %w.add(%pane);
   $SFLT::pane = %pane;
   %chk = new GuiCheckBoxCtrl(SFLT_enabled) {
      text = "Colour local chat by players nearby";
      position = "8 6";
      extent = "245 22";
      minExtent = "8 2";
      profile = isObject(SFGuiCheckBoxProfile) ? "SFGuiCheckBoxProfile" : "GuiCheckBoxProfile";
      visible = "1";
      active = "1";
      isContainer = "0";
   };
   %pane.add(%chk);
   SFLT_text(%pane, "Range", 8, 36, 44);
   SFLC_edit(%pane, SFLT_radius, $pref::SFMods::LC::Radius, 54, 34, 40, 4);
   SFLT_text(%pane, "Max players", 110, 36, 80);
   SFLC_edit(%pane, SFLT_max, $pref::SFMods::LC::Max, 196, 34, 40, 2);
   SFLT_text(%pane, "Colour codes (RRGGBB) per nearby players", 8, 62, 250);
   for (%i = 1; %i <= 12; %i++) {
      %col = (%i - 1) >= 6 ? 1 : 0;
      %row = (%i - 1) % 6;
      %x = 6 + %col * 132;
      %y = 84 + %row * 24;
      SFLT_text(%pane, %i, %x, %y + 2, 20);
      SFLC_edit(%pane, "SFLT_col" @ %i, "", %x + 22, %y, 58, 6);
      %sm = new GuiMLTextCtrl("SFLT_smp" @ %i) {
         text = "";
         position = (%x + 84) SPC (%y + 3);
         extent = "34 18";
         minExtent = "8 2";
         profile = "GuiMLTextProfile";
         visible = "1";
         active = "1";
         isContainer = "0";
      };
      %pane.add(%sm);
   }
   SFLC_button(%pane, "Save", "SFLT_apply();", 8, 232, 70);
   SFLC_button(%pane, "Preview", "SFLT_preview();", 96, 232, 78);
   SFLC_button(%pane, "Reset", "SFLC_resetColors();", 190, 232, 64);
   SFLC_fill();
}

// small white label in SF's text style
function SFLT_text(%parent, %text, %x, %y, %w)
{
   %c = new GuiTextCtrl() {
      text = %text;
      position = %x SPC %y;
      extent = %w SPC "20";
      minExtent = "8 2";
      profile = "SFGuiTextProfile";
      visible = "1";
      active = "1";
      isContainer = "0";
   };
   %parent.add(%c);
   return %c;
}

function sfWindowResizeLChat()
{
   SFGuiWindowCtrl.extent.y = 420;
   SFGuiContButtons.POSITION.y = 385;
}

function sfWindowShowLChat()
{
   SFLC_fill();
   SFGuiWindow.setPane("SFGuiPaneLChat");
   sfWindowResizeLChat();
}

package SFLocalChatTab
{
   // every SF pane sits 23 px lower now: the window and the button row grow by the same amount
   function SFGuiWindow::setPane(%this, %pane)
   {
      Parent::setPane(%this, %pane);
      if (isObject($SFLT::pane)) $SFLT::pane.setVisible(%pane $= "SFGuiPaneLChat");
   }
   function sfWindowResizeClicker() { Parent::sfWindowResizeClicker(); SFLT_grow(); }
   function sfWindowResizeOrganiser() { Parent::sfWindowResizeOrganiser(); SFLT_grow(); }
   function sfWindowResizeHerbs() { Parent::sfWindowResizeHerbs(); SFLT_grow(); }
   function sfWindowResizeSplitter() { Parent::sfWindowResizeSplitter(); SFLT_grow(); }
   function sfWindowResizeGather() { Parent::sfWindowResizeGather(); SFLT_grow(); }
   function sfWindowResizeHealing() { Parent::sfWindowResizeHealing(); SFLT_grow(); }
   function sfWindowResizeArcher() { Parent::sfWindowResizeArcher(); SFLT_grow(); }
   function sfWindowResizePoison() { Parent::sfWindowResizePoison(); SFLT_grow(); }
   function sfWindowResizeBlocking() { Parent::sfWindowResizeBlocking(); SFLT_grow(); }
   function sfWindowResizeCrosshair() { Parent::sfWindowResizeCrosshair(); SFLT_grow(); }
   function sfWindowResizeInfoBar() { Parent::sfWindowResizeInfoBar(); SFLT_grow(); }
   function sfWindowResizeGFX() { Parent::sfWindowResizeGFX(); SFLT_grow(); }
   function sfWindowResizeTools() { Parent::sfWindowResizeTools(); SFLT_grow(); }
};

function SFLT_grow()
{
   SFGuiWindowCtrl.extent.y = SFGuiWindowCtrl.extent.y + 23;
   SFGuiContButtons.POSITION.y = SFGuiContButtons.POSITION.y + 23;
}

SFLT_build();
activatePackage(SFLocalChatTab);
warn("SF Mods local chat colours loaded");
// diagnostic: which markup does the local chat accept? (run sfLCtest() in the console)
function sfLCtest()
{
   %was = $pref::SFMods::LC::Enabled; $pref::SFMods::LC::Enabled = 0;
   clientCmdLocalChatMessage("A-plain", "0 0 0", "plain text for comparison");
   clientCmdLocalChatMessage("<color:ff0000>B-sender-tag", "0 0 0", "text after a coloured sender");
   clientCmdLocalChatMessage("C-entity", "0 0 0", "&lt;color:ff0000&gt;entity tags");
   clientCmdLocalChatMessage("D-backslash", "0 0 0", "\c2backslash colour code");
   clientCmdLocalChatMessage("E-spush", "0 0 0", "<spush><color:ff0000>spush tag<spop>");
   $pref::SFMods::LC::Enabled = %was;
   onChatMessage("<spush><color:ff0000>F-onChatMessage red<spop>", 0, 0);
}