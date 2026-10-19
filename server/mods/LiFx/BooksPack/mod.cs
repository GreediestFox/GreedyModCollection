/*
* <description>Books registry: written documents (Letter 3960, Book 3961, Stone Tablet 3962) that link to the scriptorium website.
*   Every document item carries its registry key in Durability/CreatedDurability (1..65535; survives storage and the tradepost,
*   which re-creates items from type/quality/durability). The key maps to a row in `book_registry` (website tag, title, type).
*   Client -> server: BookCreate(tag, title) creates a registered item in the caller's inventory, BookLookup(key) asks for one entry.
*   Server -> client: BookEntry(key, type, tag, title, baseUrl), BookResult(ok, message).
*   Stone tablets can only be created by a logged-in GM (they are puzzles placed by the server team).
*   The item rows are also baked into art/dump.sql (foreign-key check before dbChanges).</description>
*/
if (!isObject(LiFxBooksPack)) { new ScriptObject(LiFxBooksPack) { }; }
exec("mods/LiFx/BooksPack/config.cs");
if ($BooksPack::createCooldownMs $= "") { $BooksPack::createCooldownMs = 10000; }

package LiFxBooksPack
{
    function LiFxBooksPack::setup() {
        LiFx::registerCallback($LiFx::hooks::onInitServerDBChangesCallbacks, dbChanges, LiFxBooksPack);
    }
    function LiFxBooksPack::version() {
        return "0.3.0";
    }
    function LiFxBooksPack::dbChanges() {
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3960,18,'Letter',0,0,0,0,0,0,0,0,2,1,50,'',0,0,0,0,0,0,'art/2D/Items/yo_letter.png','A written letter. Point at it and press Ctrl+P to read it.',50,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3961,18,'Book',0,0,0,0,0,0,0,0,2,1,1000,'',0,0,0,0,0,0,'art/2D/Items/yo_book.png','A written book. Point at it and press Ctrl+P to read it.',50,NULL,1,1)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3962,18,'Stone Tablet',0,0,0,0,0,0,0,0,2,1,5000,'',0,0,0,0,0,0,'art/2D/Items/yo_stone_tablet.png','An engraved stone tablet. Point at it and press Ctrl+P to read it.',50,NULL,1,1)");
        dbi.Update("CREATE TABLE IF NOT EXISTS `book_registry` ("
            @ "`BookKey` INT UNSIGNED NOT NULL AUTO_INCREMENT, `Tag` VARCHAR(40) NOT NULL, `Title` VARCHAR(120) NOT NULL DEFAULT '',"
            @ " `Type` ENUM('letter','book','tablet') NOT NULL, `CreatedBy` INT UNSIGNED NOT NULL DEFAULT 0,"
            @ " `CreatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, `ReqToken` CHAR(16) NULL DEFAULT NULL,"
            @ " PRIMARY KEY (`BookKey`), KEY `Tag` (`Tag`), KEY `ReqToken` (`ReqToken`))"
            @ " COMMENT='LiFx BooksPack: item Durability = BookKey'");
        // crafted blank documents get a default durability (up to 5000 for tablets): keep the registry keys above that range
        dbi.Update("ALTER TABLE `book_registry` AUTO_INCREMENT=10000");
        // recipes 6600 Letter (Nature Lore 60, started from the Metal Scissors tab, 5 simple cloth) and 6601 Book (Tailoring 30, Tailor's Bench,
        // 5 letters + 1 thick leather). Requirement ids are left to auto-increment; rows are replaced by recipe id on every boot.
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID` IN (6600,6601)");
        dbi.Update("DELETE FROM `recipe` WHERE `ID` IN (6600,6601)");
        dbi.Update("INSERT INTO `recipe` (`ID`,`Name`,`Description`,`StartingToolsID`,`SkillTypeID`,`SkillLvl`,`ResultObjectTypeID`,`SkillDepends`,`Quantity`,`Autorepeat`,`IsBlueprint`,`ImagePath`) VALUES "
            @ "(6600,'Letter','',3047,11,60,3960,30,1,0,0,'art/2D/Items/yo_letter.png'),"
            @ "(6601,'Book','',1394,25,30,3961,30,1,0,0,'art/2D/Items/yo_book.png')");
        dbi.Update("INSERT INTO `recipe_requirement` (`RecipeID`,`MaterialObjectTypeID`,`Quality`,`Influence`,`Quantity`,`IsRegionItemRequired`) VALUES "
            @ "(6600,1477,0,50,5,0),(6600,3047,0,10,30,0),"
            @ "(6601,3960,0,40,5,0),(6601,424,0,40,1,0),(6601,1394,0,10,30,0)");
        // Salzfass: movable container like the Barrel (3984 object, 3993 carried twin, conversion) with the vanilla crate_quest model + CRAFT recipe 6602 (Saw). Lives here because the server's
        // recipe manager reads the recipe table early at boot: recipes added by ZZ_RecipeRevisionPack come too late for building recipes.
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3984,63,'Salt Barrel',1,1,0,0,0,0,0,300000,3,0,5000,'art\\\\images\\\\universal',0,0,0,0,0,0,'art/2D/Items/placeholder_salt_barrel.png','',7800,150,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_types` VALUES (3993,1902,'Salt Barrel',0,0,0,0,0,0,0,0,3,1,5000,'',0,0,0,0,0,0,'art/2D/Items/placeholder_salt_barrel.png','',NULL,NULL,0,0)");
        dbi.Update("INSERT IGNORE INTO `objects_conversions` (ObjectTypeID1,ObjectTypeID2) VALUES (3984,3993)");
        dbi.Update("DELETE FROM `recipe_requirement` WHERE `RecipeID`=6602");
        dbi.Update("DELETE FROM `recipe` WHERE `ID`=6602");
        dbi.Update("INSERT INTO `recipe` (`ID`,`Name`,`Description`,`StartingToolsID`,`SkillTypeID`,`SkillLvl`,`ResultObjectTypeID`,`SkillDepends`,`Quantity`,`Autorepeat`,`IsBlueprint`,`ImagePath`) VALUES "
            @ "(6602,'Salt Barrel','',36,8,30,3984,35,1,0,0,'art/2D/Items/placeholder_salt_barrel.png')");
        dbi.Update("INSERT INTO `recipe_requirement` (`RecipeID`,`MaterialObjectTypeID`,`Quality`,`Influence`,`Quantity`,`IsRegionItemRequired`) VALUES "
            @ "(6602,36,0,10,30,0),(6602,235,0,40,5,0),(6602,282,0,15,2,0),(6602,3983,0,0,2,0)");
    }

    // ---------------------------------------------------------------- helpers
    function LiFxBooksPack::typeOfTag(%tag) {
        // tags come from the scriptorium website: prefix + 10 characters of its id alphabet (see BookWebsite/server/lib/ids.js)
        if (strlen(%tag) != 14) { return ""; }
        if (stripChars(getSubStr(%tag, 4, 10), "23456789abcdefghjkmnpqrstuvwxyz") !$= "") { return ""; }
        %p = getSubStr(%tag, 0, 4);
        if (%p $= "_ltr") { return "letter"; }
        if (%p $= "_bok") { return "book"; }
        if (%p $= "_tbl") { return "tablet"; }
        return "";
    }

    function LiFxBooksPack::itemType(%type) {
        if (%type $= "letter") { return 3960; }
        if (%type $= "book") { return 3961; }
        if (%type $= "tablet") { return 3962; }
        return 0;
    }

    function LiFxBooksPack::cleanTitle(%s) {
        %s = trim(stripChars(%s, "'\"\\;%`<>\t\n\r"));
        if (strlen(%s) > 100) { %s = getSubStr(%s, 0, 100); }
        return %s;
    }

    function LiFxBooksPack::getPlayer(%client) {
        %p = %client.player;
        if (!isObject(%p)) { %p = %client.getControlObject(); }
        return %p;
    }

    function LiFxBooksPack::isGM(%client) {
        return isMethod("LiFxGMCommands", "isAllowed") && LiFxGMCommands::isAllowed(%client);
    }

    // Why a stone tablet was refused: GM rights but not logged in to the GM panel yet, or no GM at all.
    function LiFxBooksPack::gmMsg(%client) {
        if (isMethod("LiFxGMCommands", "hasRight") && LiFxGMCommands::hasRight(%client)) {
            return "Steintafeln: bitte zuerst im GM-Panel (F6) mit dem GM-Passwort anmelden.";
        }
        return "Steintafeln kann nur ein GM erstellen.";
    }

    function LiFxBooksPack::result(%client, %ok, %msg) {
        commandToClient(%client, 'BookResult', %ok, %msg);
        %client.cmSendClientMessage(2475, "<color:" @ (%ok ? "88dd88" : "ff7777") @ ">[Skriptorium] " @ %msg);
    }

    // ---------------------------------------------------------------- create
    function serverCmdBookCreate(%client, %tag, %title) {
        %tag = trim(%tag);
        %type = LiFxBooksPack::typeOfTag(%tag);
        %title = LiFxBooksPack::cleanTitle(%title);
        if (%type $= "") {
            LiFxBooksPack::result(%client, 0, "Unbekannter Code. Er beginnt mit _ltr, _bok oder _tbl und hat 14 Zeichen.");
            return;
        }
        if (%title $= "") {
            LiFxBooksPack::result(%client, 0, "Bitte einen Titel angeben.");
            return;
        }
        if (%type $= "tablet" && !LiFxBooksPack::isGM(%client)) {
            LiFxBooksPack::result(%client, 0, LiFxBooksPack::gmMsg(%client));
            return;
        }
        LiFxBooksPack::register(%client, %type, %tag, %title, 0);
    }

    // Registers the document and hands out the registered item. %blankId > 0: that unwritten item of the player is replaced
    // (BookApply, the automatic write flow); 0: a new item is added (BookCreate, the manual dialog).
    function LiFxBooksPack::register(%client, %type, %tag, %title, %blankId) {
        %now = getSimTime();
        if (%client.booksLastCreate !$= "" && %now - %client.booksLastCreate < $BooksPack::createCooldownMs) {
            LiFxBooksPack::result(%client, 0, "Bitte kurz warten, bevor du das naechste Schriftstueck erstellst.");
            return;
        }
        if (!isObject(LiFxBooksPack::getPlayer(%client))) {
            LiFxBooksPack::result(%client, 0, "Kein Spieler gefunden.");
            return;
        }
        %client.booksLastCreate = %now;
        %token = "";
        for (%i = 0; %i < 16; %i++) { %token = %token @ getSubStr("0123456789abcdef", getRandom(0, 15), 1); }
        %char = %client.getCharacterId();
        // the DBI thread runs the queued statements in order, so the SELECT sees the row of this INSERT
        dbi.Update("INSERT INTO `book_registry` (`Tag`,`Title`,`Type`,`CreatedBy`,`ReqToken`) VALUES ('" @ %tag @ "','" @ %title @ "','" @ %type @ "'," @ %char @ ",'" @ %token @ "')");
        %req = new ScriptObject() {
            class = "LiFxBooksCreate";
            client = %client;
            type = %type;
            tag = %tag;
            title = %title;
            token = %token;
            blankId = %blankId;
        };
        dbi.select(%req, "onKey", "SELECT `BookKey` FROM `book_registry` WHERE `ReqToken`='" @ %token @ "'");
    }

    function LiFxBooksCreate::onKey(%this, %rs) {
        %client = %this.client;
        %key = 0;
        if (%rs.Ok() && %rs.nextRecord()) { %key = %rs.getFieldValue("BookKey"); }
        dbi.remove(%rs);
        if (!isObject(%client)) {
            if (%key > 0) { dbi.Update("DELETE FROM `book_registry` WHERE `BookKey`=" @ %key); }
            %this.delete();
            return;
        }
        %p = LiFxBooksPack::getPlayer(%client);
        if (%key < 1 || %key > 65535) {
            echo("[Books] create failed: key" SPC %key SPC "for tag" SPC %this.tag);
            if (%key > 0) { dbi.Update("DELETE FROM `book_registry` WHERE `BookKey`=" @ %key); }
            LiFxBooksPack::result(%client, 0, "Das Schriftstueck konnte nicht registriert werden.");
        } else if (isObject(%p) && %this.blankId > 0 && !%p.inventoryRemoveItem(%this.blankId)) {
            dbi.Update("DELETE FROM `book_registry` WHERE `BookKey`=" @ %key);
            LiFxBooksPack::result(%client, 0, "Das unbeschriebene Schriftstueck konnte nicht ersetzt werden.");
        } else if (!isObject(%p) || !%p.inventoryAddItem(LiFxBooksPack::itemType(%this.type), 1, 50, %key, %key)) {
            dbi.Update("DELETE FROM `book_registry` WHERE `BookKey`=" @ %key);
            if (%this.blankId > 0 && isObject(%p)) { %p.inventoryAddItem(LiFxBooksPack::itemType(%this.type), 1, 100, 100, 100); }   // give the blank back
            LiFxBooksPack::result(%client, 0, "Kein Platz im Inventar.");
        } else {
            echo("[Books] character" SPC %client.getCharacterId() SPC "created" SPC %this.type SPC "key" SPC %key SPC "tag" SPC %this.tag);
            commandToClient(%client, 'BookEntry', %key, %this.type, %this.tag, %this.title, $BooksPack::baseUrl);
            LiFxBooksPack::result(%client, 1, "Erstellt: " @ %this.title);
        }
        %this.delete();
    }

    // ---------------------------------------------------------------- apply (automatic write flow)
    // The client opened the website with a ticket, the document was finished there and the client fetched its code + title.
    // Replaces one unwritten document item (3960 letter / 3961 book whose Durability is no registry key) of the player.
    function serverCmdBookApply(%client, %tag, %title) {
        %tag = trim(%tag);
        %type = LiFxBooksPack::typeOfTag(%tag);
        %title = LiFxBooksPack::cleanTitle(%title);
        if (%type $= "" || %title $= "") {
            LiFxBooksPack::result(%client, 0, "Ungueltiger Code oder Titel.");
            return;
        }
        if (%type $= "tablet" && !LiFxBooksPack::isGM(%client)) {
            LiFxBooksPack::result(%client, 0, LiFxBooksPack::gmMsg(%client));
            return;
        }
        %char = %client.getCharacterId();
        %req = new ScriptObject() {
            class = "LiFxBooksApply";
            client = %client;
            type = %type;
            tag = %tag;
            title = %title;
        };
        %roots = "SELECT RootContainerID FROM `character` WHERE ID=" @ %char @ " UNION SELECT EquipmentContainerID FROM `character` WHERE ID=" @ %char;
        dbi.select(%req, "onBlank", "SELECT i.ID AS ItemID FROM `items` i WHERE i.ObjectTypeID=" @ LiFxBooksPack::itemType(%type) @ " AND (i.ContainerID IN (" @ %roots @ ") OR i.ContainerID IN (SELECT ct.ID FROM `containers` ct WHERE ct.ParentID IN (SELECT i2.ID FROM `items` i2 WHERE i2.ContainerID IN (" @ %roots @ ")))) AND NOT EXISTS (SELECT 1 FROM `book_registry` b WHERE b.BookKey=i.Durability) AND NOT EXISTS (SELECT 1 FROM `book_registry` t WHERE t.Tag='" @ %tag @ "') LIMIT 1");
    }

    function LiFxBooksApply::onBlank(%this, %rs) {
        %id = 0;
        if (%rs.Ok() && %rs.nextRecord()) { %id = %rs.getFieldValue("ItemID"); }
        dbi.remove(%rs);
        %client = %this.client;
        if (isObject(%client)) {
            if (%id > 0) {
                LiFxBooksPack::register(%client, %this.type, %this.tag, %this.title, %id);
            } else {
                LiFxBooksPack::result(%client, 0, "Kein unbeschriebenes Schriftstueck dieser Art im Inventar (oder bereits uebernommen).");
            }
        }
        %this.delete();
    }

    // The stone-tablet page of the website needs a secret key: it is only handed to GMs.
    function serverCmdBookTabletKey(%client) {
        if (!LiFxBooksPack::isGM(%client)) {
            LiFxBooksPack::result(%client, 0, LiFxBooksPack::gmMsg(%client));
            return;
        }
        commandToClient(%client, 'BookTabletKey', $BooksPack::tabletAdminKey);
    }

    // ---------------------------------------------------------------- lookup
    function serverCmdBookLookup(%client, %key) {
        if (%key $= "" || stripChars(%key, "0123456789") !$= "" || %key < 1 || %key > 65535) { return; }
        %req = new ScriptObject() {
            class = "LiFxBooksLookup";
            client = %client;
            key = %key;
        };
        dbi.select(%req, "onRow", "SELECT `Type`,`Tag`,`Title` FROM `book_registry` WHERE `BookKey`=" @ %key);
    }

    function LiFxBooksLookup::onRow(%this, %rs) {
        %type = ""; %tag = ""; %title = "";
        if (%rs.Ok() && %rs.nextRecord()) {
            %type = %rs.getFieldValue("Type");
            %tag = %rs.getFieldValue("Tag");
            %title = %rs.getFieldValue("Title");
        }
        dbi.remove(%rs);
        if (isObject(%this.client)) {
            commandToClient(%this.client, 'BookEntry', %this.key, %type, %tag, %title, $BooksPack::baseUrl);
        }
        %this.delete();
    }
};
activatePackage(LiFxBooksPack);
LiFx::registerCallback($LiFx::hooks::mods, setup, LiFxBooksPack);
