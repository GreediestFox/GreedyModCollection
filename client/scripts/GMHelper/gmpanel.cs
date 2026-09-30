// GM panel (client side, GreedyFox). F6 opens/closes it. Every button sends the same commands you can type in chat ("!give ...") through gm(...)
// or calls the game's own GM slash commands (/DELOBJ, /COMPLETE, /ANIMAL) with doSlashCommand(...). Replies show in the System chat tab.
// Only accounts that are GMs on the server get answers; others are ignored by the server.

exec("mod/GMHelper/gmobjects.cs");

// ------------------------------------------------------------------------------------------------
// Dropdown menus (the game's own GuiPopUpMenuCtrlEx, styled like its other dropdowns but with a smaller font)
// Created on first use (F6), because this file is loaded before the game's own GUI profiles exist.
function GMPanel_profiles()
{
   if (!isObject(GMPopUpTextListProfile)) {
      new GuiControlProfile(GMPopUpTextListProfile : CreateCharWindowPopUpTextListProfile) {
         fontSize = 14;
         textOffset = "8 4";
      };
   }
   if (!isObject(GMPopUpProfile)) {
      new GuiControlProfile(GMPopUpProfile : CreateCharWindowPopUpProfile) {
         profileForChildren = GMPopUpTextListProfile;
         fontSize = 14;
         textOffset = "12 2";
      };
   }
}

function GMPanel_popupScroll()
{
   // same setup as the game's own long dropdowns (character creation): visible scrollbar with thumb and arrows
   %gui = new GuiScrollCtrl() {
      hScrollBar = "alwaysOff";
      vScrollBar = "alwaysOn";
      horizSizing = "width";
      vertSizing = "height";
      profile = "GuiPopUpScrollBarProfile";
      constantThumbHeight = false;
      childMargin = "10 0";
      trackOffset = 11;
      lockHorizScroll = true;
      arrowSadowSize = 2;
      addContentWidth = -60;
   };
   return %gui;
}

function GMPanel_popup(%parent, %name, %x, %y, %w)
{
   %c = new GuiPopUpMenuCtrlEx(%name) {
      position = %x SPC %y;
      extent = %w SPC "26";
      Profile = "GMPopUpProfile";
      backPnlProfile = "GuiPopUpBorder";
      buttonOffset = 5;
      cellHeight = 26;
      createScrollCommand = "GMPanel_popupScroll();";
      maxPopupHeight = 320;
      leftIndentMenu = 8;
      rightIndentMenu = 8;
      addScrollSize = "-3 0";
      menuOffset = 0;
   };
   %parent.add(%c);
   return %c;
}

// One entry of the grouped command dropdown: label shown, template put into the command box.
function GMPanel_cmd(%label, %tpl)
{
   $GMCmd::label[$GMCmd::n] = %label;
   $GMCmd::tpl[$GMCmd::n] = %tpl;
   $GMCmd::n++;
}

function GMPanel_cmdTable()
{
   $GMCmd::n = 0;
   GMPanel_cmd("Player - Heal myself", "/HEALSELF");
   GMPanel_cmd("Player - Damage reduction ON (god-like)", "/DECREASEHPDMG 1");
   GMPanel_cmd("Player - Damage reduction OFF", "/DECREASEHPDMG 0");
   GMPanel_cmd("Player - Invulnerable (GM)", "/INVUL");
   GMPanel_cmd("Player - Remove all effects", "/REMOVEEFFECTS");
   GMPanel_cmd("Player - Set alignment (-1000 .. 1000)", "/ALIGNMENT 0");
   GMPanel_cmd("Player - Set criminal flag (seconds)", "/CRIMINAL 60");
   GMPanel_cmd("Skills - Set my skill (id or name, 0-100)", "/SETMYSKILL 32 100");
   GMPanel_cmd("Skills - Set my stat (0 STR 1 AGI 2 WIL 3 INT 4 CON)", "/SETMYSTAT 0 100");
   GMPanel_cmd("Skills - Change a player skill (name skill amount)", "/SETPLAYERSKILL name 32 50");
   GMPanel_cmd("Items - Add item (id or name, quantity, quality, durability)", "/ADD 241 10 100 0");
   GMPanel_cmd("Items - Add all weapons", "/ADDWEAPONS");
   GMPanel_cmd("Items - Add all tools", "/ADDTOOLS");
   GMPanel_cmd("Items - Add all armor", "/ADDARMOR");
   GMPanel_cmd("Items - Add all crafting resources", "/ADDRESOURCES");
   GMPanel_cmd("Items - Add alchemy reagents (quantity quality)", "/ADDREAGENTS 1 100");
   GMPanel_cmd("Items - Add antidotes", "/ADDANTIDOTE");
   GMPanel_cmd("Objects - Complete selected building", "/COMPLETE");
   GMPanel_cmd("Objects - Delete selected object", "/DELOBJ");
   GMPanel_cmd("Objects - Add object by type id", "/ADDOBJ 1199");
   GMPanel_cmd("Objects - Claim monument up (selected)", "/CLAIM UP");
   GMPanel_cmd("Objects - Claim monument down (selected)", "/CLAIM DOWN");
   GMPanel_cmd("Objects - Finish working container timer", "/WC FINISH");
   GMPanel_cmd("World - Grow crops here (one day)", "/GROWCROPS");
   GMPanel_cmd("World - Weather: Fair", "/WEATHER Fair");
   GMPanel_cmd("World - Weather: Cloudy", "/WEATHER Cloudy");
   GMPanel_cmd("World - Weather: Shower", "/WEATHER Shower");
   GMPanel_cmd("World - Weather: Snowy", "/WEATHER Snowy");
   GMPanel_cmd("World - Which weather now?", "/WEATHER");
   GMPanel_cmd("World - Force judgment hour ON", "/JHFORCEON");
   GMPanel_cmd("World - Force judgment hour OFF", "/JHFORCEOFF");
   GMPanel_cmd("Teleport - To a player (name)", "/TPTOPLAYER name");
   GMPanel_cmd("Teleport - Bring the selected player to me", "/TPPLAYER");
   GMPanel_cmd("Teleport - To the spawn point", "/TPTOSPAWN");
   GMPanel_cmd("Teleport - Jump on any terrain (GM)", "/JT GM");
   GMPanel_cmd("Teleport - Unstuck", "/STUCK");
   GMPanel_cmd("Animals - Boar", "/ANIMAL BoarData");
   GMPanel_cmd("Animals - Wolf", "/ANIMAL WolfData");
   GMPanel_cmd("Animals - Bear", "/ANIMAL BearData");
   GMPanel_cmd("Animals - Moose", "/ANIMAL MooseData");
   GMPanel_cmd("Animals - Deer (male)", "/ANIMAL DeerMaleData");
   GMPanel_cmd("Animals - Wild horse", "/ANIMAL WildHorseData");
   GMPanel_cmd("GM tools - Players online", "!players");
   GMPanel_cmd("GM tools - Place object or building (type id)", "!place 168");
   GMPanel_cmd("GM tools - Heal a player (name or id)", "!heal name");
   GMPanel_cmd("GM tools - Give item to a player", "!giveto name 241 10 100 0");
   GMPanel_cmd("GM tools - Show my last actions", "!gmlog 10");
}

// ------------------------------------------------------------------------------------------------
// Search bars over the dropdowns: type to filter the list (all words must match, any case), click the arrow to browse the results.
// A short timer (300 ms, only while the panel is open) notices changed text, so no key events are needed.
function GMPanel_search(%parent, %name, %x, %y, %w)
{
   %c = new GuiTextEditCtrl(%name) {
      text = "";
      position = %x SPC %y;
      extent = %w SPC "20";
      minExtent = "8 2";
      maxLength = "60";
      profile = "GuiTextEditProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
   };
   %parent.add(%c);
   return %c;
}

// %kind: "b" buildings, "m" 3D objects, "c" commands. Rebuilds the popup with the entries matching %filter.
function GMPanel_refill(%popup, %kind, %filter)
{
   %filter = strlwr(trim(%filter));
   %count = %kind $= "b" ? $GMObj::bCount : (%kind $= "m" ? $GMObj::mCount : (%kind $= "i" ? $GMObj::iCount : $GMCmd::n));
   %popup.clear();
   %shown = 0;
   %list = "";
   for (%i = 0; %i < %count; %i++) {
      if (%kind $= "c") {
         %name = $GMCmd::label[%i];
         %id = %i + 1;
      } else {
         %rec = %kind $= "b" ? $GMObj::b[%i] : (%kind $= "m" ? $GMObj::m[%i] : $GMObj::i[%i]);
         %id = getWord(%rec, 0);
         %name = getWords(%rec, 1) @ "  (" @ %id @ ")";
      }
      %ok = 1;
      %lname = strlwr(%name);
      for (%w = 0; %w < getWordCount(%filter); %w++) {
         if (strpos(%lname, getWord(%filter, %w)) < 0) {
            %ok = 0;
         }
      }
      if (%ok) {
         %shown++;
         %list = %list @ %id @ "\t" @ %name @ "\n";
      }
   }
   %popup.add("Results: " @ %shown @ " of " @ %count, 0);
   for (%i = 0; %i < getRecordCount(%list); %i++) {
      %r = getRecord(%list, %i);
      %popup.add(getField(%r, 1), getField(%r, 0));
   }
   %popup.setSelected(0, false);
}

function GMPanel_searchCheck(%edit, %popup, %kind)
{
   %txt = %edit.getText();
   if (%txt !$= $GMPanel::lastSearch[%kind]) {
      $GMPanel::lastSearch[%kind] = %txt;
      GMPanel_refill(%popup, %kind, %txt);
   }
}

function GMPanel_searchTick()
{
   if (!isObject(GMPanel) || !GMPanel.isAwake()) {
      $GMPanel::tick = 0;
      return;
   }
   GMPanel_searchCheck(GMP_bldSearch, GMP_bldPopup, "b");
   GMPanel_searchCheck(GMP_objSearch, GMP_objPopup, "m");
   GMPanel_searchCheck(GMP_itemSearch, GMP_itemPopup, "i");
   GMPanel_searchCheck(GMP_cmdSearch, GMP_cmdPopup, "c");
   $GMPanel::tick = schedule(300, 0, GMPanel_searchTick);
}

function GMPanel_startTick()
{
   if ($GMPanel::tick) {
      cancel($GMPanel::tick);
   }
   $GMPanel::tick = schedule(300, 0, GMPanel_searchTick);
}

// After a pick the search box shows the chosen name; remembered so the timer does not refilter the list around it.
function GMPanel_picked(%edit, %kind, %txt)
{
   %edit.setText(%txt);
   $GMPanel::lastSearch[%kind] = %txt;
}

function GMPanel_fillPopups()
{
   GMPanel_cmdTable();
   $GMPanel::lastSearch["b"] = "";
   $GMPanel::lastSearch["m"] = "";
   $GMPanel::lastSearch["c"] = "";
   $GMPanel::lastSearch["i"] = "";
   GMPanel_refill(GMP_itemPopup, "i", "");
   GMPanel_refill(GMP_bldPopup, "b", "");
   GMPanel_refill(GMP_objPopup, "m", "");
   GMPanel_refill(GMP_cmdPopup, "c", "");
}
function GMP_itemPopup::onSelect(%this, %id, %txt)
{
   if (%id > 0) {
      GMP_itemType.setText(%id);
      GMPanel_picked(GMP_itemSearch, "i", %txt);
   }
}

function GMP_bldPopup::onSelect(%this, %id, %txt)
{
   if (%id > 0) {
      GMP_objType.setText(%id);
      GMPanel_picked(GMP_bldSearch, "b", %txt);
   }
}

function GMP_objPopup::onSelect(%this, %id, %txt)
{
   if (%id > 0) {
      GMP_objType.setText(%id);
      GMPanel_picked(GMP_objSearch, "m", %txt);
   }
}

function GMP_cmdPopup::onSelect(%this, %id, %txt)
{
   if (%id > 0) {
      GMP_slash.setText($GMCmd::tpl[%id - 1]);
      GMPanel_picked(GMP_cmdSearch, "c", %txt);
   }
}
function GMPanel_label(%parent, %text, %x, %y, %w)
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

function GMPanel_edit(%parent, %name, %text, %x, %y, %w)
{
   %c = new GuiTextEditCtrl(%name) {
      text = %text;
      position = %x SPC %y;
      extent = %w SPC "20";
      minExtent = "8 2";
      maxLength = "120";
      profile = "GuiTextEditProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
   };
   %parent.add(%c);
   return %c;
}

function GMPanel_button(%parent, %text, %cmd, %x, %y, %w)
{
   %c = new GuiButtonCtrl() {
      text = %text;
      buttonType = "PushButton";
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

function GMPanel_build()
{
   if (isObject(GMPanel)) {
      return;
   }
   GMPanel_profiles();
   %root = new GuiControl(GMPanel) {
      position = "0 0";
      extent = "100% 100%";
      horizSizing = "width";
      vertSizing = "height";
      profile = "GuiDefaultProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
   };
   %win = new GuiWindowCtrl(GMPanelDlg) {
      text = "GM Panel";
      resizeWidth = "0";
      resizeHeight = "0";
      canMove = "1";
      canClose = "1";
      canMinimize = "0";
      canMaximize = "0";
      canCollapse = "0";
      position = "300 120";
      extent = "1130 658";
      minExtent = "48 92";
      horizSizing = "center";
      vertSizing = "center";
      profile = "GuiWindowProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
      closeCommand = "Canvas.popDialog(GMPanel);";
   };
   %root.add(%win);

   // inventory-style parchment/wood background, behind all controls
   %bg = new GuiBitmapCtrl() {
      imageIndex = "LearningWindowBackground";
      profile = "GuiAtlas3ImageProfile";
      position = "7 42";
      extent = "1116 606";
      wrap = true;
   };
   %win.add(%bg);

   // ---- Items ----
   GMPanel_label(%win, "<font:Arial:14>Items", 24, 44, 200);
   GMPanel_label(%win, "type", 24, 64, 40);
   GMPanel_edit(%win, GMP_itemType, "241", 24, 82, 60);
   GMPanel_label(%win, "quantity", 94, 64, 60);
   GMPanel_edit(%win, GMP_itemQty, "1", 94, 82, 60);
   GMPanel_label(%win, "quality", 164, 64, 60);
   GMPanel_edit(%win, GMP_itemQuality, "100", 164, 82, 60);
   GMPanel_label(%win, "durability", 234, 64, 70);
   GMPanel_edit(%win, GMP_itemDur, "100", 234, 82, 60);
   GMPanel_button(%win, "Give to me", "GMPanel_giveMe();", 310, 80, 110);
   GMPanel_button(%win, "Delete (qty) from me", "GMPanel_deleteMe();", 430, 80, 160);
   GMPanel_label(%win, "player (char id or name)", 24, 112, 200);
   GMPanel_edit(%win, GMP_player, "", 24, 130, 200);
   GMPanel_button(%win, "Give to player", "GMPanel_giveTo();", 234, 128, 110);
   GMPanel_label(%win, "Search items: type to filter, click the arrow to browse (fills the item type box):", 24, 160, 520);
   GMPanel_popup(%win, GMP_itemPopup, 24, 180, 350);
   GMPanel_search(%win, GMP_itemSearch, 32, 183, 296);

   // ---- Objects ----
   GMPanel_label(%win, "<font:Arial:14>Objects (movable or building, 1 tile in front of you)", 24, 210, 400);
   GMPanel_label(%win, "type", 24, 230, 40);
   GMPanel_edit(%win, GMP_objType, "168", 24, 248, 60);
   GMPanel_button(%win, "Create object", "GMPanel_place();", 94, 246, 120);
   GMPanel_button(%win, "Delete selected", "doSlashCommand(\"/DELOBJ\");", 224, 246, 120);
   GMPanel_button(%win, "Complete selected", "doSlashCommand(\"/COMPLETE\");", 354, 246, 130);
   GMPanel_label(%win, "Search buildings / 3D objects: type to filter, click the arrow to browse (fills the type box):", 24, 278, 520);
   GMPanel_popup(%win, GMP_bldPopup, 24, 298, 250);
   GMPanel_popup(%win, GMP_objPopup, 284, 298, 250);
   GMPanel_search(%win, GMP_bldSearch, 32, 301, 196);
   GMPanel_search(%win, GMP_objSearch, 292, 301, 196);

   // ---- Animals ----
   GMPanel_label(%win, "animal datablock (game GM command /ANIMAL)", 24, 326, 320);
   GMPanel_edit(%win, GMP_animal, "BoarData", 24, 344, 160);
   GMPanel_button(%win, "Spawn animal", "GMPanel_animal();", 194, 342, 120);
   GMPanel_button(%win, "<", "GMPanel_animalStep(-1);", 324, 342, 30);
   GMPanel_button(%win, ">", "GMPanel_animalStep(1);", 360, 342, 30);

   // ---- Ground (the cell you stand on) ----
   GMPanel_label(%win, "<font:Arial:14>Ground under you (changes the surface material)", 24, 380, 420);
   GMPanel_button(%win, "Rock", "gm(\"ground rock\");", 24, 400, 100);
   GMPanel_button(%win, "River rock", "gm(\"ground riverrock\");", 134, 400, 110);
   GMPanel_button(%win, "Swamp", "gm(\"ground swamp\");", 254, 400, 100);
   GMPanel_button(%win, "Lower (undo)", "gm(\"lower\");", 364, 400, 110);

   // ---- Players ----
   GMPanel_label(%win, "<font:Arial:14>Players (uses the player field above)", 24, 424, 400);
   GMPanel_button(%win, "List online", "gm(\"players\");", 24, 444, 100);
   GMPanel_button(%win, "Go to", "GMPanel_playerCmd(\"tp\");", 134, 444, 80);
   GMPanel_button(%win, "Bring", "GMPanel_playerCmd(\"bring\");", 224, 444, 80);
   GMPanel_button(%win, "Kick", "GMPanel_playerCmd(\"kick\");", 314, 444, 80);
   GMPanel_button(%win, "Ban", "GMPanel_playerCmd(\"ban\");", 404, 444, 60);
   GMPanel_button(%win, "Unban", "GMPanel_playerCmd(\"unban\");", 474, 444, 60);

   // ---- Teleport ----
   GMPanel_label(%win, "<font:Arial:14>Teleport", 24, 484, 200);
   GMPanel_label(%win, "x", 24, 504, 20);
   GMPanel_edit(%win, GMP_tpX, "", 24, 522, 70);
   GMPanel_label(%win, "y", 104, 504, 20);
   GMPanel_edit(%win, GMP_tpY, "", 104, 522, 70);
   GMPanel_label(%win, "z", 184, 504, 20);
   GMPanel_edit(%win, GMP_tpZ, "", 184, 522, 70);
   GMPanel_button(%win, "Teleport", "GMPanel_tppos();", 264, 520, 90);
   GMPanel_button(%win, "My position", "gm(\"pos\");", 364, 520, 100);

   // ---- Announcement ----
   GMPanel_label(%win, "<font:Arial:14>Announcement to everyone online", 24, 560, 400);
   GMPanel_edit(%win, GMP_announce, "", 24, 580, 400);
   GMPanel_button(%win, "Send", "GMPanel_announce();", 434, 578, 90);
   GMPanel_button(%win, "Help", "gm(\"help\");", 24, 614, 70);
   GMPanel_button(%win, "Who am I", "gm(\"whoami\");", 104, 614, 90);
   GMPanel_button(%win, "Geo id", "gm(\"geo\");", 204, 614, 80);
   GMPanel_button(%win, "GM Logout", "GMPanel_logout();", 420, 614, 110);

   // ================= right column =================
   GMPanel_label(%win, "<font:Arial:14>Player tools (uses the player field on the left, empty = you)", 584, 44, 500);
   GMPanel_button(%win, "Heal", "GMPanel_heal();", 584, 64, 100);
   GMPanel_button(%win, "Invulnerable ON", "GMPanel_godOn();", 694, 64, 130);
   GMPanel_button(%win, "Invulnerable OFF", "GMPanel_godOff();", 834, 64, 130);

   GMPanel_label(%win, "<font:Arial:14>Weather (the game's own /WEATHER command)", 584, 116, 500);
   GMPanel_button(%win, "Fair", "doSlashCommand(\"/WEATHER Fair\");", 584, 140, 100);
   GMPanel_button(%win, "Cloudy", "doSlashCommand(\"/WEATHER Cloudy\");", 694, 140, 100);
   GMPanel_button(%win, "Shower", "doSlashCommand(\"/WEATHER Shower\");", 804, 140, 100);
   GMPanel_button(%win, "Snowy", "doSlashCommand(\"/WEATHER Snowy\");", 914, 140, 100);
   GMPanel_button(%win, "Which weather now?", "doSlashCommand(\"/WEATHER\");", 584, 172, 160);
   GMPanel_label(%win, "<font:Arial:14>Adminland (a protected area from your position towards east/north, 1 tile = 4 m)", 584, 240, 540);
   GMPanel_label(%win, "name", 584, 260, 60);
   GMPanel_edit(%win, GMP_alName, "", 584, 278, 150);
   GMPanel_label(%win, "length", 744, 260, 60);
   GMPanel_edit(%win, GMP_alLen, "10", 744, 278, 60);
   GMPanel_label(%win, "width", 814, 260, 60);
   GMPanel_edit(%win, GMP_alWid, "10", 814, 278, 60);
   GMPanel_button(%win, "Create adminland", "GMPanel_adminland(\"create\");", 584, 312, 140);
   GMPanel_button(%win, "Preview", "GMPanel_adminland(\"preview\");", 734, 312, 90);
   GMPanel_button(%win, "Delete (by name)", "GMPanel_adminland(\"delete\");", 834, 312, 140);

   GMPanel_label(%win, "<font:Arial:14>GM action log", 584, 364, 300);
   GMPanel_button(%win, "Show last 10 actions", "gm(\"gmlog 10\");", 584, 384, 170);

   GMPanel_label(%win, "<font:Arial:14>Game GM commands: type to search, click the arrow to browse, adjust the arguments, press Run", 584, 424, 540);
   GMPanel_popup(%win, GMP_cmdPopup, 584, 446, 480);
   GMPanel_search(%win, GMP_cmdSearch, 592, 449, 426);
   GMPanel_edit(%win, GMP_slash, "/", 584, 482, 380);
   GMPanel_button(%win, "Run", "GMPanel_slash();", 974, 480, 90);
   GMPanel_label(%win, "<font:Arial:14>Inflation helper (coins in the world per non-GM player)", 584, 526, 540);
   GMPanel_button(%win, "Show inflation now", "gm(\"inflation\");", 584, 548, 170);
   GMPanel_button(%win, "History (10)", "gm(\"inflation history 10\");", 764, 548, 130);
   GMPanel_button(%win, "Economy dashboard", "GMPanel_dashboard();", 904, 548, 180);
   GMPanel_label(%win, "baseline (copper per player)", 584, 582, 200);
   GMPanel_edit(%win, GMP_baseline, "", 584, 602, 120);
   GMPanel_button(%win, "Set baseline", "GMPanel_baseline();", 714, 600, 120);
   GMPanel_fillPopups();
}

function GMPanel_logout()
{
   GMPanel_godOff();
   $GMLogin::loggingOut = 1;
   gm("logout");
}

// Asks the server to refresh the dashboard data file, then opens the dashboard page (same PC as the server) in the default browser.
$GMPanel::dashboardDir = "F:/SteamLibrary/steamapps/common/Life is Feudal Your Own Dedicated Server/mods/LiFx/GMCommands/economy";
$GMPanel::dashboardFile = "F:/SteamLibrary/steamapps/common/Life is Feudal Your Own Dedicated Server/mods/LiFx/GMCommands/economy/GMEconomyDashboard.html";

function GMPanel_dashboard()
{
   gm("inflation export");
   schedule(1500, 0, GMPanel_openDashboard);
}

function GMPanel_openDashboard()
{
   // gotoWebPage fails here (the engine cannot find the default browser in the registry); shellExecute lets Windows open the file with its default program.
   shellExecute($GMPanel::dashboardFile, "", $GMPanel::dashboardDir);
}

function GMPanel_baseline()
{
   gm("inflation baseline" SPC trim(GMP_baseline.getText()));
}

function GMPanel_heal()
{
   gm("heal" SPC trim(GMP_player.getText()));
}

// The game's /INVUL is a TOGGLE (adds or removes invulnerability effect 45 on you). The panel remembers whether it switched it on, so
// ON and OFF only send it when a change is needed. If you used /INVUL by hand, press OFF twice or type /INVUL to resync.
$GMPanel::god = 0;

function GMPanel_godOn()
{
   if (!$GMPanel::god) {
      doSlashCommand("/INVUL");
      $GMPanel::god = 1;
   }
}

function GMPanel_godOff()
{
   if ($GMPanel::god) {
      doSlashCommand("/INVUL");
      $GMPanel::god = 0;
   }
}

function GMPanel_god(%mode)
{
   if (%mode $= "on") {
      GMPanel_godOn();
   } else {
      GMPanel_godOff();
   }
}

function GMPanel_slash()
{
   %c = trim(GMP_slash.getText());
   if (%c $= "" || %c $= "/" || %c $= "!") {
      return;
   }
   if (getSubStr(%c, 0, 1) $= "!") {
      gm(getSubStr(%c, 1, 200));
   } else {
      doSlashCommand(%c);
   }
}

function GMPanel_adminland(%what)
{
   %name = trim(GMP_alName.getText());
   %len = trim(GMP_alLen.getText());
   %wid = trim(GMP_alWid.getText());
   if (%what $= "create") {
      gm("adminland create" SPC %name SPC %len SPC %wid);
   } else if (%what $= "preview") {
      gm("adminland preview" SPC %len SPC %wid);
   } else {
      gm("adminland delete" SPC %name);
   }
}

function GMPanel_giveMe()
{
   gm("give" SPC trim(GMP_itemType.getText()) SPC trim(GMP_itemQty.getText()) SPC trim(GMP_itemQuality.getText()) SPC trim(GMP_itemDur.getText()));
}

function GMPanel_deleteMe()
{
   // removes <quantity> items of <type> from the GM's own inventory (backpack, bags, equipped)
   gm("delete" SPC trim(GMP_itemType.getText()) SPC trim(GMP_itemQty.getText()));
}

function GMPanel_giveTo()
{
   gm("giveto" SPC trim(GMP_player.getText()) SPC trim(GMP_itemType.getText()) SPC trim(GMP_itemQty.getText()) SPC trim(GMP_itemQuality.getText()) SPC trim(GMP_itemDur.getText()));
}

function GMPanel_place()
{
   gm("place" SPC trim(GMP_objType.getText()));
}

function GMPanel_spawn()
{
   gm("spawn" SPC trim(GMP_objType.getText()));
}

function GMPanel_build2()
{
   gm("build" SPC trim(GMP_objType.getText()));
}

$GMPanel::animals = "BoarData WolfData BearData DeerMaleData HindData HareData GrouseData MooseData AurochsBullData AurochsCowData SowData MuttonData WildHorseData ChieftainData WitchData HunterData MoleData BearKnoolData";

function GMPanel_animalStep(%dir)
{
   %n = getWordCount($GMPanel::animals);
   %cur = trim(GMP_animal.getText());
   %idx = -1;
   for (%i = 0; %i < %n; %i++) {
      if (getWord($GMPanel::animals, %i) $= %cur) {
         %idx = %i;
      }
   }
   %idx = (%idx + %dir + %n) % %n;
   GMP_animal.setText(getWord($GMPanel::animals, %idx));
}

function GMPanel_animal()
{
   doSlashCommand("/ANIMAL" SPC trim(GMP_animal.getText()));
}

function GMPanel_playerCmd(%cmd)
{
   gm(%cmd SPC trim(GMP_player.getText()));
}

function GMPanel_tppos()
{
   gm("tppos" SPC trim(GMP_tpX.getText()) SPC trim(GMP_tpY.getText()) SPC trim(GMP_tpZ.getText()));
}

function GMPanel_announce()
{
   gm("announce" SPC trim(GMP_announce.getText()));
}

// ------------------------------------------------------------------------------------------------
// GM login: F6 asks for the GM password first. The server checks it (!login), switches GM mode on for you and answers with
// clientCmdGMLoginResult: 1 = ok, 0 = wrong / login needed, 2 = no password set yet, 3 = new password too short.
$GMPanel::loggedIn = 0;
$GMLogin::pending = 0;
$GMLogin::loggingOut = 0;

function GMLogin_build()
{
   if (isObject(GMLoginRoot)) {
      return;
   }
   %root = new GuiControl(GMLoginRoot) {
      position = "0 0";
      extent = "100% 100%";
      horizSizing = "width";
      vertSizing = "height";
      profile = "GuiDefaultProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
   };
   %win = new GuiWindowCtrl(GMLoginDlg) {
      text = "GM Login";
      resizeWidth = "0";
      resizeHeight = "0";
      canMove = "1";
      canClose = "1";
      canMinimize = "0";
      canMaximize = "0";
      canCollapse = "0";
      position = "400 260";
      extent = "380 230";
      minExtent = "48 92";
      horizSizing = "center";
      vertSizing = "center";
      profile = "GuiWindowProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
      closeCommand = "Canvas.popDialog(GMLoginRoot);";
   };
   %root.add(%win);
   %bg = new GuiBitmapCtrl() {
      imageIndex = "LearningWindowBackground";
      profile = "GuiAtlas3ImageProfile";
      position = "7 42";
      extent = "366 180";
      wrap = true;
   };
   %win.add(%bg);
   GMPanel_label(%win, "GM password", 28, 60, 300);
   %pw = new GuiTextEditCtrl(GMLogin_pw) {
      text = "";
      password = "1";
      passwordMask = "*";
      position = "28 82";
      extent = "320 22";
      minExtent = "8 2";
      maxLength = "64";
      profile = "GuiTextEditProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
      altCommand = "GMLogin_submit();";
   };
   %win.add(%pw);
   %msg = new GuiMLTextCtrl(GMLogin_msg) {
      text = "";
      position = "28 112";
      extent = "320 50";
      minExtent = "8 2";
      profile = "GuiMLTextProfile";
      visible = "1";
      active = "1";
      isContainer = "0";
   };
   %win.add(%msg);
   GMPanel_button(%win, "Login", "GMLogin_submit();", 28, 172, 150);
   GMPanel_button(%win, "Cancel", "Canvas.popDialog(GMLoginRoot);", 198, 172, 150);
}

function GMLogin_status(%plain, %color)
{
   // shown in two places so it cannot be missed: the message line and the window title
   GMLogin_msg.setText("<color:" @ %color @ ">" @ %plain);
   if (GMLoginRoot.isAwake() && GMLogin_msg.isMethod("forceReflow")) {
      GMLogin_msg.forceReflow();
   }
   GMLoginDlg.setText(%plain $= "" ? "GM Login" : "GM Login - " @ %plain);
}

function GMLogin_submit()
{
   %pw = GMLogin_pw.getText();
   if (%pw $= "") {
      GMLogin_status("Enter the GM password", "ff5050");
      return;
   }
   $GMLogin::pending = 1;
   GMLogin_status("checking...", "d8d8d8");
   gm("login" SPC %pw);
}

function GMLogin_setPass()
{
   %pw = GMLogin_pw.getText();
   if (strlen(%pw) < 6) {
      GMLogin_status("New password: at least 6 characters", "ff5050");
      return;
   }
   $GMLogin::pending = 1;
   GMLogin_status("saving...", "d8d8d8");
   gm("setpass" SPC %pw);
}

function clientCmdGMLoginResult(%code)
{
   if (%code == 1) {
      $GMPanel::loggedIn = 1;
      $GMLogin::pending = 0;
      GMLogin_build();
      GMLogin_pw.setText("");
      GMLogin_status("", "d8d8d8");
      Canvas.popDialog(GMLoginRoot);
      GMPanel_build();
      Canvas.pushDialog(GMPanel);
      GMPanel_startTick();
      return;
   }
   if ($GMLogin::loggingOut) {
      // deliberate logout from the panel: leave GM mode and close the panel, no login window
      $GMLogin::loggingOut = 0;
      $GMPanel::loggedIn = 0;
      if (isObject(GMPanel) && GMPanel.isAwake()) {
         Canvas.popDialog(GMPanel);
      }
      return;
   }
   $GMPanel::loggedIn = 0;
   GMLogin_build();
   if (%code == 2) {
      GMLogin_status("No GM password is set on the server", "ffd060");
   } else if (%code == 3) {
      GMLogin_status("Password too short (6+ characters)", "ff5050");
   } else if ($GMLogin::pending) {
      GMLogin_status("Wrong Login", "ff5050");
   } else {
      GMLogin_status("GM login required", "ffd060");
   }
   $GMLogin::pending = 0;
   if (isObject(GMPanel) && GMPanel.isAwake()) {
      Canvas.popDialog(GMPanel);
   }
   if (!GMLoginRoot.isAwake()) {
      Canvas.pushDialog(GMLoginRoot);
   }
}
function toggleGMPanel()
{
   if ($GMPanel::loggedIn) {
      GMPanel_build();
      if (GMPanel.isAwake()) {
         Canvas.popDialog(GMPanel);
      } else {
         Canvas.pushDialog(GMPanel);
         GMPanel_startTick();
      }
   } else {
      GMLogin_build();
      if (GMLoginRoot.isAwake()) {
         Canvas.popDialog(GMLoginRoot);
      } else {
         GMLogin_pw.setText("");
         GMLogin_status("", "d8d8d8");
         Canvas.pushDialog(GMLoginRoot);
         GMLogin_pw.setFirstResponder();
      }
   }
}
GlobalActionMap.bindCmd(keyboard, "f6", "toggleGMPanel();", "");











