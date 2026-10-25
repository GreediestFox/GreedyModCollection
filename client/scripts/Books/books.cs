// Books registry, client side (GMC BooksPack; replaces the name-tag display of Daniel's YOscriptorium client mod).
//
// Written documents are the item types 3960 Letter, 3961 Book, 3962 Stone Tablet. Their registry key is the item's DB Durability
// (set by the server when it creates the item; it survives storage and the tradepost). The key is read in the item tooltip,
// which the game builds with an informer that carries the item's data (tooltipManager.cs registers booksTooltip for the three
// types). The server answers BookLookup(key) with BookEntry(key, type, tag, title, baseUrl); entries are cached for the session.
//
// Ctrl+P while pointing at a document: opens it on the scriptorium website in the game's browser window (openWeb).
// Ctrl+P otherwise: opens the "write" dialog (paste the code the website shows, give a title, create the item).
//
// No polling: nothing runs until a document's tooltip is shown. The old YOscriptorium client mod is not needed with this.

$Books::hoverKey = 0;
$Books::cacheDir = "mod/Books/cache/";

function booksTypeName(%type)
{
   if (%type $= "letter") return "Brief";
   if (%type $= "book") return "Buch";
   if (%type $= "tablet") return "Steintafel";
   return "";
}

// ---------------------------------------------------------------- tooltip (registered in gui/scripts/tooltipManager.cs)
function booksTooltip(%informer)
{
   createBaseTooltip();
   // the informer hands durability to scripts divided by 100 (DB 54321 -> "543.21", 1 -> "0.01"): undo that for the key
   %key = mFloor(%informer.getData("items", "Durability") * 100 + 0.5);
   cancel($Books::clearEv);
   $Books::hoverKey = %key;
   %title = $Books::title[%key];
   if (%title $= "" && %key > 0 && !$Books::asked[%key])
   {
      $Books::asked[%key] = 1;
      commandToServer('BookLookup', %key);
   }
   %txt = createCaptionTextCtrl();
   TooltipMainControl.add(%txt);
   %name = %informer.getData("objects_types", "Name");
   // the sub line always exists so a late server answer can fill it in
   %sub = createTextCtrl("GuiGrayTextTooltipProfile", %title !$= "" ? %name : " ");
   $Books::capCtrl = %txt;
   $Books::subCtrl = %sub;
   $Books::typeName = %name;
   %txt.setText(%title !$= "" ? %title : %name);
   TooltipMainControl.add(%sub);
   addItemDescription(%informer);
}

function GuiInventoryItem::onMouseEnter(%this)
{
   // the tooltip (which learns the document key) normally appears after 500 ms; a quick right-click would find no key
   %b = %this.bitmap;
   $Books::enterType = "";
   if (strstr(%b, "yo_letter") >= 0) $Books::enterType = "letter";
   else if (strstr(%b, "yo_book") >= 0) $Books::enterType = "book";
   else if (strstr(%b, "yo_stone_tablet") >= 0) $Books::enterType = "tablet";
   if ($Books::enterType !$= "")
      %this.hovertime = 30;
}

function booksClearHover()
{
   $Books::hoverKey = 0;
}

function GuiInventoryItem::onMouseLeave(%this)
{
   // keep the key a moment: the right-click menu is built right after the pointer left the item
   cancel($Books::clearEv);
   $Books::clearEv = schedule(600, 0, "booksClearHover");
}

// ---------------------------------------------------------------- server answers
function clientCmdBookEntry(%key, %type, %tag, %title, %baseUrl)
{
   if (%baseUrl !$= "") $Books::baseUrl = %baseUrl;
   if (%tag $= "")
   {
      $Books::title[%key] = "";
      return;
   }
   $Books::type[%key] = %type;
   $Books::tag[%key] = %tag;
   $Books::title[%key] = %title;
   // tooltip still open on this item: update it live
   if ($Books::hoverKey == %key && isObject($Books::capCtrl) && isObject($Books::subCtrl))
   {
      $Books::capCtrl.setText(%title);
      $Books::subCtrl.setText($Books::typeName);
   }
}

function clientCmdBookResult(%ok, %msg)
{
   if (isObject(BooksWrite_msg))
      BooksWrite_msg.setText("<color:" @ (%ok ? "88dd88" : "ff7777") @ ">" @ %msg);
   if (%ok && isObject(BooksWrite_tag))
   {
      BooksWrite_tag.setText("");
      BooksWrite_title.setText("");
   }
}

// ---------------------------------------------------------------- Ctrl+P
function booksLangParam(%first)
{
   if ($pref::language::pack $= "") return "";
   return (%first ? "?" : "&") @ "lang=" @ $pref::language::pack;
}

function booksOpen(%url)
{
   if (%url $= "") return;
   // the downloads below (title, ticket) use plain http: the engine downloader has no TLS. The web page itself is opened with https.
   if (getSubStr(%url, 0, 7) $= "http://" && strpos(%url, "localhost") < 0 && strpos(%url, "127.0.0.1") < 0) %url = "https://" @ getSubStr(%url, 7, 2000);
   %ctrl = openWeb(%url);
   if (isObject(%ctrl)) %ctrl.setText("Skriptorium");
}

function booksCtrlP()
{
   booksAct($Books::hoverKey);
}

// %key > 0: read the registered document (or ask the server once if unknown); no key: open the write dialog
function booksAct(%key)
{
   if (%key > 0)
   {
      if ($Books::tag[%key] $= "")
      {
         if (!$Books::asked[%key]) { $Books::asked[%key] = 1; commandToServer('BookLookup', %key); }
         echo("[Books] key" SPC %key SPC "is not known (yet)");
         return;
      }
      booksOpen($Books::baseUrl @ "/" @ $Books::type[%key] @ ".html?id=" @ $Books::tag[%key] @ booksLangParam(false));
      return;
   }
   booksWriteOpen();
}

// ---------------------------------------------------------------- write dialog
function booksWriteOpen()
{
   booksWriteBuild();
   if (!BooksWriteRoot.isAwake()) Canvas.pushDialog(BooksWriteRoot);
   BooksWrite_msg.setText("");
   if ($Books::baseUrl $= "") commandToServer('BookLookup', 1);   // the answer also carries the website address
}

function booksWriteLabel(%parent, %text, %x, %y, %w)
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
}

function booksWriteEdit(%parent, %name, %x, %y, %w, %max)
{
   %c = new GuiTextEditCtrl(%name) {
      text = "";
      position = %x SPC %y;
      extent = %w SPC "22";
      minExtent = "8 2";
      maxLength = %max;
      profile = "GuiTextEditProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
   };
   %parent.add(%c);
}

function booksWriteButton(%parent, %text, %cmd, %x, %y, %w)
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
   };
   %parent.add(%c);
}

function booksWriteBuild()
{
   if (isObject(BooksWriteRoot)) return;
   %root = new GuiControl(BooksWriteRoot) {
      position = "0 0";
      extent = "100% 100%";
      horizSizing = "width";
      vertSizing = "height";
      profile = "GuiDefaultProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
   };
   %win = new GuiWindowCtrl(BooksWriteDlg) {
      text = "Skriptorium";
      resizeWidth = "0";
      resizeHeight = "0";
      canMove = "1";
      canClose = "1";
      canMinimize = "0";
      canMaximize = "0";
      canCollapse = "0";
      position = "400 240";
      extent = "420 300";
      minExtent = "48 92";
      horizSizing = "center";
      vertSizing = "center";
      profile = "GuiWindowProfile";
      visible = "1";
      active = "1";
      isContainer = "1";
      closeCommand = "Canvas.popDialog(BooksWriteRoot);";
   };
   %root.add(%win);
   %bg = new GuiBitmapCtrl() {
      imageIndex = "LearningWindowBackground";
      profile = "GuiAtlas3ImageProfile";
      position = "7 42";
      extent = "406 250";
      wrap = true;
   };
   %win.add(%bg);
   booksWriteLabel(%win, "1. Schriftstueck auf der Webseite schreiben:", 24, 56, 370);
   booksWriteButton(%win, "Webseite oeffnen", "booksOpen($Books::baseUrl @ \"/index.html\" @ booksLangParam(true));", 24, 78, 180);
   booksWriteLabel(%win, "2. Code von der Webseite (z.B. _bokabc234defg):", 24, 114, 370);
   booksWriteEdit(%win, "BooksWrite_tag", 24, 136, 250, 14);
   booksWriteButton(%win, "Titel laden", "booksWriteFetchTitle();", 284, 135, 110);
   booksWriteLabel(%win, "3. Titel:", 24, 168, 370);
   booksWriteEdit(%win, "BooksWrite_title", 24, 190, 370, 100);
   %msg = new GuiMLTextCtrl(BooksWrite_msg) {
      text = "";
      position = "24 220";
      extent = "370 30";
      minExtent = "8 2";
      profile = "GuiMLTextProfile";
   };
   %win.add(%msg);
   booksWriteButton(%win, "Erstellen", "booksWriteSubmit();", 24, 258, 180);
   booksWriteButton(%win, "Schliessen", "Canvas.popDialog(BooksWriteRoot);", 214, 258, 180);
}

function booksWriteSubmit()
{
   %tag = trim(BooksWrite_tag.getText());
   %title = trim(BooksWrite_title.getText());
   if (%tag $= "" || %title $= "")
   {
      BooksWrite_msg.setText("<color:ff7777>Bitte Code und Titel eingeben.");
      return;
   }
   BooksWrite_msg.setText("Wird erstellt ...");
   commandToServer('BookCreate', %tag, %title);
}

// The website serves the plain title at /api/title/<code>; it is downloaded into a cache file and read a moment later.
function booksWriteFetchTitle()
{
   %tag = trim(BooksWrite_tag.getText());
   if (strlen(%tag) != 14 || $Books::baseUrl $= "")
   {
      BooksWrite_msg.setText("<color:ff7777>Erst den Code (14 Zeichen) eingeben.");
      return;
   }
   %file = $Books::cacheDir @ %tag @ ".txt";
   createPath(%file);
   %helper = new SimDownloadHelper() {
      url = $Books::baseUrl @ "/api/title/" @ %tag;
      fileName = %file;
   };
   %helper.start();
   BooksWrite_msg.setText("Titel wird geladen ...");
   schedule(1500, 0, "booksWriteReadTitle", %file);
}

function booksWriteReadTitle(%file)
{
   %line = "";
   %fo = new FileObject();
   if (%fo.openForRead(%file))
   {
      %line = %fo.readLine();
      %fo.close();
   }
   %fo.delete();
   if (!isObject(BooksWrite_title)) return;
   if (%line $= "")
   {
      BooksWrite_msg.setText("<color:ff7777>Kein Titel gefunden. Bitte von Hand eingeben.");
      return;
   }
   BooksWrite_title.setText(%line);
   BooksWrite_msg.setText("");
}

GlobalActionMap.bindCmd(keyboard, "ctrl p", "booksCtrlP();", "");

// ---------------------------------------------------------------- right-click menu ("Schreiben" / "Lesen")
// Server side the ability "Open Gift" (id 336) is repurposed: it is offered on the document items 3960-3962 (skill_types.xml) and its
// German name is "Schreiben". Here the entry is renamed to "Lesen" for registered documents and its click is taken over, so the real
// ability never runs: Lesen opens the page on the scriptorium website, Schreiben opens the write dialog.
function booksIsWriteEntry(%text)
{
   %text = trim(%text);   // the default entry's text has a leading space
   return %text $= "Schreiben" || %text $= "Open Gift";
}

function booksMenuHook(%btn, %text, %kind)
{
   echo("[Books] menu button" SPC %kind SPC "text=" @ %text SPC "hoverKey=" @ $Books::hoverKey SPC "tag=" @ $Books::tag[$Books::hoverKey]);
   if (!isObject(%btn) || !booksIsWriteEntry(%text)) return %btn;
   %key = $Books::hoverKey;
   if ($Books::tag[%key] !$= "" && isObject(%btn-->textName)) %btn-->textName.setText("Lesen");
   %ov = new GuiMouseEventCtrl()
   {
      position = "0 0";
      extent = %btn.getExtent();
      horizSizing = "width";
      vertSizing = "height";
      profile = "GuiBaseTextProfile";
      visible = "1";
      booksKey = %key;
      booksType = $Books::enterType;
      booksBtn = %btn;
   };
   %btn.add(%ov);
   %btn.bringToFront(%ov);
   return %btn;
}

function GuiMouseEventCtrl::onMouseEnter(%this)
{
   if (%this.booksKey $= "") return;
   if (isObject(%this.booksBtn-->MouseOverPnl)) %this.booksBtn-->MouseOverPnl.setVisible(true);
}

function GuiMouseEventCtrl::onMouseLeave(%this)
{
   if (%this.booksKey $= "") return;
   if (isObject(%this.booksBtn-->MouseOverPnl)) %this.booksBtn-->MouseOverPnl.setVisible(false);
}

function GuiMouseEventCtrl::onMouseDown(%this, %mod, %pos, %clicks)
{
   if (%this.booksKey $= "") return;
   echo("[Books] menu overlay mouse down key" SPC %this.booksKey);
}

function GuiMouseEventCtrl::onMouseUp(%this, %mod, %pos, %clicks)
{
   if (%this.booksKey $= "") return;
   %key = %this.booksKey;
   ContextMenuClose();
   // registered document: read it; anything else: the write dialog
   if (%key > 0 && $Books::tag[%key] !$= "") booksAct(%key);
   else if (%this.booksType !$= "") booksAutoWrite(%this.booksType);
   else booksWriteOpen();
}

// ---------------------------------------------------------------- automatic write ("Schreiben" on an unwritten letter/book)
// The scriptorium page is opened with a random ticket. When the document is finished there, the website answers
// /api/ticket/<ticket> with "<code>\n<title>"; the client then asks the server to turn one unwritten item into that document.
function booksAutoWrite(%type)
{
   if ($Books::baseUrl $= "")
   {
      booksWriteOpen();   // website address unknown yet: the dialog also asks the server for it
      return;
   }
   %chars = "abcdefghjkmnpqrstuvwxyz23456789";
   %t = "w";
   for (%i = 0; %i < 11; %i++) %t = %t @ getSubStr(%chars, getRandom(0, strlen(%chars) - 1), 1);
   $Books::ticket = %t;
   if (%type $= "tablet")
   {
      commandToServer('BookTabletKey');   // GMs only: the server answers with the key of the tablet page (clientCmdBookTabletKey)
      return;
   }
   booksOpen($Books::baseUrl @ "/" @ %type @ ".html?new=1&ticket=" @ %t @ booksLangParam(false));
   schedule(4000, 0, "booksPollTicket", %t, 0);
}

function clientCmdBookTabletKey(%key)
{
   %t = $Books::ticket;
   if (%t $= "" || %key $= "") return;
   booksOpen($Books::baseUrl @ "/admin/tablet?key=" @ %key @ "&ticket=" @ %t @ booksLangParam(false));
   schedule(4000, 0, "booksPollTicket", %t, 0);
}

function booksPollTicket(%t, %n)
{
   if ($Books::ticket !$= %t) return;   // a newer write was started
   if (%n > 200)
   {
      $Books::ticket = "";
      return;
   }
   %file = $Books::cacheDir @ "ticket_" @ %t @ ".txt";
   createPath(%file);
   if (isFile(%file)) fileDelete(%file);
   %helper = new SimDownloadHelper() {
      url = $Books::baseUrl @ "/api/ticket/" @ %t;
      fileName = %file;
   };
   %helper.start();
   schedule(1500, 0, "booksReadTicket", %t, %n, %file);
}

function booksReadTicket(%t, %n, %file)
{
   if ($Books::ticket !$= %t) return;
   %tag = "";
   %title = "";
   %fo = new FileObject();
   if (%fo.openForRead(%file))
   {
      %tag = trim(%fo.readLine());
      %title = trim(%fo.readLine());
      %fo.close();
   }
   %fo.delete();
   if (%tag !$= "" && %title !$= "" && strlen(%tag) == 14)
   {
      $Books::ticket = "";
      commandToServer('BookApply', %tag, %title);
      return;
   }
   schedule(3000, 0, "booksPollTicket", %t, %n + 1);
}

// remembers which tool tab was clicked last (the click command runs before the game fills the recipe list)
function booksTagCraftTab(%win, %type)
{
   %stack = %win-->inventoryToolsStack;
   if (!isObject(%stack)) return;
   %btn = %stack.getObject(%stack.getCount() - 1);
   if (isObject(%btn)) %btn.command = "$Books::craftTool=" @ %type @ ";" @ %btn.command;
}

package BooksMenu
{
   function GuiCraftWindows::addInventoryToolButton(%this, %objectTypeId, %image, %active, %selected)
   {
      Parent::addInventoryToolButton(%this, %objectTypeId, %image, %active, %selected);
      booksTagCraftTab(%this, %objectTypeId);
      // the game only builds tabs for its fixed set of tools; the Metal Scissors tab (letter recipe) is appended after the last one.
      // The game lists the recipes of the whole "Craft" ability under it, so addRecipeListItem below keeps only the letter there.
      if (%objectTypeId == 527 && !$Books::scissorsTabBusy)
      {
         $Books::scissorsTabBusy = 1;
         Parent::addInventoryToolButton(%this, 3047, "mod/Books/art/2D/Items/yo_scissors.png", 1, 0);
         booksTagCraftTab(%this, 3047);
         $Books::scissorsTabBusy = 0;
      }
   }
   function GuiCraftWindows::init(%this, %isInventoryCraft)
   {
      $Books::craftTool = 0;
      Parent::init(%this, %isInventoryCraft);
   }
   function GuiCraftWindows::addRecipeListItem(%this, %recipeId, %abilityId, %name, %image)
   {
      // recipe 6600 (Brief) is shown only on the scissors tab, and the scissors tab shows nothing else
      if (%recipeId == 6600 && $Books::craftTool != 3047) return;
      if ($Books::craftTool == 3047 && %recipeId != 6600) return;
      Parent::addRecipeListItem(%this, %recipeId, %abilityId, %name, %image);
   }
   function createSingleAbilityWidgetButton(%menu, %text, %image)
   {
      return booksMenuHook(Parent::createSingleAbilityWidgetButton(%menu, %text, %image), %text, "single");
   }
   function createDefaultAbilityWidgetButton(%menu, %text)
   {
      return booksMenuHook(Parent::createDefaultAbilityWidgetButton(%menu, %text), %text, "default");
   }
   function addInGroupAbilityWidgetButton(%menu, %text, %skillID, %image)
   {
      return booksMenuHook(Parent::addInGroupAbilityWidgetButton(%menu, %text, %skillID, %image), %text, "group");
   }
};
activatePackage(BooksMenu);