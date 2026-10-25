// SF Mods window, new layout (2026-10-25), modelled on the Crusader Kings "Find Characters" window:
//   - a vertical column of large tab buttons on the left, the settings of the selected tab on the right
//   - the tabs Block, Antidote and GFX are gone
//   - fixed window size, buttons Changelog / Hotkeys / Save in a row under the settings
// Control tree (from data/sf_tree.txt): SFGuiWindow > SFGuiWindowCtrl > tab buttons (SFGuiButton<Tab>), panes (SFGuiPane<Tab>), container SFGuiContButtons.
// Everything is found by INTERNAL name. The compiled mod re-sets the window height on every tab change, so the layout is re-applied every 250 ms while the window is open.

$sfTabsOrder  = "Clicker Organiser Herbs Splitter Gather Healing Archer Crosshair InfoBar Tools LChat";
$sfTabsHide   = "Poison Blocking GFX";
$sfTabsColX   = 15;      // left edge of the button column
$sfTabsColW   = 110;     // button width
$sfTabsBtnH   = 28;      // button height
$sfTabsStepY  = 31;      // distance between buttons
$sfTabsTopY   = 28;      // first button / pane top (below the title bar)
$sfTabsPaneX  = 135;     // left edge of the settings pane (262 wide)
$sfTabsWinW   = 412;     // 15 + 110 + 10 + 262 + 15
$sfTabsWinH   = 400;

function sfTabsFind(%name)
{
   if (!isObject(SFGuiWindow)) return 0;
   %o = SFGuiWindow.findObjectByInternalName(%name, true);
   if (isObject(%o)) return %o;
   // a few controls (the Chat tab button) only have an object name
   if (isObject(%name)) return %name;
   return 0;
}

function sfTabsLayout()
{
   if (!isObject(SFGuiWindowCtrl) || !isObject(SFGuiWindow)) return 0;

   // hide the removed tabs (button and pane); keep the objects so the compiled code never hits a missing one
   for (%k = 0; %k < getWordCount($sfTabsHide); %k++) {
      %t = getWord($sfTabsHide, %k);
      %b = sfTabsFind("SFGuiButton" @ %t);
      if (isObject(%b)) %b.setVisible(0);
      %p = sfTabsFind("SFGuiPane" @ %t);
      if (isObject(%p)) %p.setVisible(0);
   }

   // tab buttons: one column, fixed order; panes: one fixed spot to the right
   %i = 0;
   for (%k = 0; %k < getWordCount($sfTabsOrder); %k++) {
      %t = getWord($sfTabsOrder, %k);
      %b = sfTabsFind("SFGuiButton" @ %t);
      if (isObject(%b)) {
         %b.position = $sfTabsColX SPC ($sfTabsTopY + %i * $sfTabsStepY);
         %b.extent = $sfTabsColW SPC $sfTabsBtnH;
         %i++;
      }
      %p = sfTabsFind("SFGuiPane" @ %t);
      if (isObject(%p)) %p.position = $sfTabsPaneX SPC $sfTabsTopY;
   }

   // window size and the bottom button row (Changelog, Hotkeys, Save): the three direct children of SFGuiContButtons
   SFGuiWindowCtrl.extent = $sfTabsWinW SPC $sfTabsWinH;
   if (isObject(SFGuiContButtons)) {
      SFGuiContButtons.position = "0" SPC ($sfTabsWinH - 35);
      SFGuiContButtons.extent = $sfTabsWinW SPC "25";
      %n = SFGuiContButtons.getCount();
      for (%k = 0; %k < %n && %k < 3; %k++) {
         %c = SFGuiContButtons.getObject(%k);
         %c.position = ($sfTabsPaneX + %k * 91) SPC "0";
         %c.extent = "80 25";
      }
   }

   // a removed tab must not stay selected
   if ($sfWindowCurrentTab $= "Blocking" || $sfWindowCurrentTab $= "Poison" || $sfWindowCurrentTab $= "GFX"
       || $pref::SFMods::WindowCurrentTab $= "Blocking" || $pref::SFMods::WindowCurrentTab $= "Poison" || $pref::SFMods::WindowCurrentTab $= "GFX") {
      $sfWindowCurrentTab = "Clicker";
      $pref::SFMods::WindowCurrentTab = "Clicker";
      if (isFunction("sfWindowShowClicker")) sfWindowShowClicker();
   }
   if (!$sfTabsLayoutLogged) {
      $sfTabsLayoutLogged = 1;
      echo("[SFTabs] new SF Mods layout applied");
   }
   return 1;
}

// The compiled tab handlers call sfWindowResize<Tab>() which sets the window height per tab (flicker with a fixed layout).
// Once the compiled functions exist they are replaced by empty ones: the window keeps the fixed size set by sfTabsLayout().
function sfTabsStubResize()
{
   if ($sfTabsStubbed) return;
   if (!isFunction("sfWindowResizeClicker")) return;
   %names = "Clicker Organiser Herbs Splitter Gather Healing Archer Poison Blocking Crosshair InfoBar GFX Tools";
   for (%i = 0; %i < getWordCount(%names); %i++) {
      eval("function sfWindowResize" @ getWord(%names, %i) @ "() { }");
   }
   $sfTabsStubbed = 1;
   echo("[SFTabs] per-tab window resize disabled");
}
// ---- Look: parchment background, dark text, green buttons with gold frame (after the Crusader Kings "Find Characters" window) ----
function sfTabsSkinProfiles()
{
   if (isObject(SFGuiParchmentProfile)) return;
   new GuiControlProfile(SFGuiParchmentProfile)
   {
      opaque = 1;
      fillColor = "214 196 150";
      border = "rect1_color";
      borderColor = "92 66 38";
   };
   // green buttons: normal / mouse over / selected tab
   new GuiControlProfile(SFGuiParchWindowProfile : SFGuiSmallWindowProfile)
   {
      opaque = 1;
      fillColor = "214 196 150";
   };
   new GuiControlProfile(SFGuiGreenTabProfile : SFGuiButtonTabProfile)
   {
      opaque = 1;
      fillColor = "36 84 50";
      fillColorHL = "52 112 70";
      fillColorSEL = "78 150 98";
      fillColorNA = "70 70 60";
      borderColor = "206 172 82";
      borderColorHL = "240 214 120";
      borderColorSEL = "240 214 120";
      FontColor = "236 226 190";
      fontColorHL = "255 255 235";
      fontColorSEL = "255 255 255";
      fontColorNA = "130 120 100";
   };
   // dark text on the parchment (the shared SF text profiles stay white for the other SF windows)
   new GuiControlProfile(SFGuiParchText : SFGuiTextProfile) { FontColor = "58 38 22"; };
   new GuiControlProfile(SFGuiParchTextRight : SFGuiTextProfileRight) { FontColor = "58 38 22"; };
   new GuiControlProfile(SFGuiParchTextBold : SFGuiTextBoldProfile) { FontColor = "40 24 12"; };
   new GuiControlProfile(SFGuiParchTextBoldRight : SFGuiTextBoldRightProfile) { FontColor = "40 24 12"; };
   new GuiControlProfile(SFGuiParchTextCenter : SFGuiTextProfileCenter) { FontColor = "58 38 22"; };
   new GuiControlProfile(SFGuiParchTextGreen : SFGuiTextGreenProfile) { FontColor = "24 100 36"; };
   new GuiControlProfile(SFGuiParchTextBoldGreen : SFGuiTextBoldGreenProfile) { FontColor = "24 100 36"; };
}

function sfTabsSkinNode(%o, %d)
{
   if (%d > 10) return;
   %cls = %o.getClassName();
   if (%cls $= "GuiTextCtrl" && isObject(%o.profile)) {
      %pn = %o.profile.getName();
      if (%pn $= "SFGuiTextProfile") %o.setProfile(SFGuiParchText);
      else if (%pn $= "SFGuiTextProfileRight") %o.setProfile(SFGuiParchTextRight);
      else if (%pn $= "SFGuiTextBoldProfile") %o.setProfile(SFGuiParchTextBold);
      else if (%pn $= "SFGuiTextBoldRightProfile") %o.setProfile(SFGuiParchTextBoldRight);
      else if (%pn $= "SFGuiTextProfileCenter") %o.setProfile(SFGuiParchTextCenter);
      else if (%pn $= "SFGuiTextGreenProfile") %o.setProfile(SFGuiParchTextGreen);
      else if (%pn $= "SFGuiTextBoldGreenProfile") %o.setProfile(SFGuiParchTextBoldGreen);
   }
   else if (%cls $= "GuiButtonCtrl" && isObject(%o.profile)) {
      %pn = %o.profile.getName();
      if (%pn $= "SFGuiButtonTabProfile" || %pn $= "GuiButtonProfile") %o.setProfile(SFGuiGreenTabProfile);
   }
   if (%o.isMethod("getCount")) {
      %n = %o.getCount();
      for (%i = 0; %i < %n; %i++) sfTabsSkinNode(%o.getObject(%i), %d + 1);
   }
}

// Moves %bg to the very back of the window (first child = drawn first). Different engine builds name the calls differently: try each and check.
function sfTabsPanelToBack(%bg)
{
   %w = SFGuiWindowCtrl;
   if (%w.getObject(0) == %bg) return 1;
   if (%w.isMethod("reorderChild")) { %w.reorderChild(%bg, %w.getObject(0)); if (%w.getObject(0) == %bg) { echo("[SFTabs] panel moved back with reorderChild"); return 1; } }
   if (%w.isMethod("bringToFront")) { %w.bringToFront(%bg); if (%w.getObject(0) == %bg) { echo("[SFTabs] panel moved back with bringToFront"); return 1; } }
   if (%w.isMethod("pushToBack")) { %w.pushToBack(%bg); if (%w.getObject(0) == %bg) { echo("[SFTabs] panel moved back with pushToBack"); return 1; } }
   return 0;
}
function sfTabsSkin()
{
   if (!isObject(SFGuiWindowCtrl) || SFGuiWindowCtrl.getCount() < 5) return;
   sfTabsSkinProfiles();
   if (!$sfTabsSkinned) {
      $sfTabsSkinned = 1;
      // the parchment is the window's own fill: a separate panel on top of / below the controls hid everything
      if (isObject(SFGuiParchment)) SFGuiParchment.delete();
      SFGuiWindowCtrl.setProfile(SFGuiParchWindowProfile);
      %bg = new GuiControl(SFGuiParchment)
      {
         profile = SFGuiParchmentProfile;
         position = "8 26";
         extent = ($sfTabsWinW - 16) SPC ($sfTabsWinH - 34);
         horizSizing = "right";
         vertSizing = "bottom";
      };
      SFGuiWindowCtrl.add(%bg);
      if (!sfTabsPanelToBack(%bg)) {
         echo("[SFTabs] could not move the parchment panel behind the controls: removed again, using the window fill only");
         %bg.delete();
      }
      sfTabsSkinNode(SFGuiWindowCtrl, 0);
      echo("[SFTabs] parchment skin applied");
   }
   if (isObject(SFGuiParchment)) {
      SFGuiParchment.position = "8 26";
      SFGuiParchment.extent = ($sfTabsWinW - 16) SPC ($sfTabsWinH - 34);
   }

}
// The window is built by the compiled mod a few seconds after start, and its tab handlers reset the height: apply the layout continuously.
function sfTabsWatch()
{
   %awake = (isObject(SFGuiWindow) && SFGuiWindow.isAwake());
   sfTabsStubResize();
   if (isObject(SFGuiWindowCtrl)) sfTabsLayout();
   sfTabsSkin();
   $sfTabsWatchEv = schedule(1000, 0, "sfTabsWatch");
}
if ($sfTabsWatchEv !$= "" && isEventPending($sfTabsWatchEv)) cancel($sfTabsWatchEv);
sfTabsWatch();