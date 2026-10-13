/* ===================================================================================
	Copyright (c) 2026, LiFx Contributors. All Rights Reserved.

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
	Animal fodder: special feed items that give newborn animals a quality bonus, no quality gain from ordinary feed,
	and a slaughter quality bonus during a window of in-game months.

	How the engine does it (found by decompilation of ddctd_cm_yo_server.exe):

	  1. "Is this item animal feed" is NOT data driven. Two tiny predicates test the item type id against a hard-coded
	     bit mask (0x80055D over ids 0x160..0x177, plus 0x402, 0x403, 0x406):
	       RVA 0x1B0E70 isFeed(mgr, typeRecord*)   used by the feeding tick (CmStable at RVA 0x1B29D0) and checkContainerLimits
	       RVA 0x1B0E10 isFeed(mgr, itemTypeId)    used by checkContainerLimits (feed weight cap)
	     Both are detoured so the configured fodder item types count as feed, which makes the animals eat them like any feed
	     (consumption is by item weight, FoodQuality is the weighted average of the eaten items' quality).

	  2. Vanilla quality gain: the per-animal tick (RVA 0x1B2FA0) raises an animal's quality by 1 (INC R8B at RVA 0x1B33AC and
	     0x1B33FC) when the stable's FoodQuality is high enough and the animals are fed and clean. With
	     disableFeedGrowth="1" those two INCs are replaced by NOPs: ordinary feed no longer raises quality; the decay
	     branch (starving / dirty) is left alone.

	  3. Newborn quality: the breeding tick (RVA 0x1B21E0) computes it from the two parents and calls
	     CmStable::bornAnimal(stable, inventory, animalTypeId, quality) at RVA 0x1AC2E0. The hook adds newbornBonus to the
	     quality when the stable's inventory holds the fodder item type configured for that species (capped at 100).

	  4. Slaughter: every product (meat, hide, ...) passes CmBreedingManager::_slaughterAddProduct(mgr, inventory, itemType,
	     quantity, quality, ...) at RVA 0x1ABD00 (callers: stable slaughter RVA 0x1B37B0 and SlaughterAnimalsInv). The hook adds
	     slaughterBonus to the quality while the in-game month (getGameTime() text "DD-MM-YYYY HH:MM:SS", impl RVA 0x468760)
	     is in slaughterMonths.

	Configured from lifxpluss.xml:
	    <animalFodder enabled="1" verbose="1" disableFeedGrowth="1" newbornBonus="2" slaughterBonus="5" slaughterMonths="10 11 12 1 2">
	        <fodder itemTypeId="3950" animals="1039 1040 1042 1043 1044 1045 1140 1685 1663" />
	        <fodder itemTypeId="3951" animals="1046 1047 1048 1049 1050 1051" />
	        <fodder itemTypeId="3952" animals="1052 1053" />
	    </animalFodder>
*/

#include "server/cm_server.h"

namespace tinyxml2 { class XMLElement; }

// isFeed(mgr, typeRecord*) -> bool. RVA 0x1B0E70. The record's item type id is at +8.
__CM_DECL_EXTERNAL(unsigned char, __fastcall, _Fodder_IsFeedRec, LPVOID mgr, LPVOID typeRecord);

// isFeed(mgr, itemTypeId) -> bool. RVA 0x1B0E10.
__CM_DECL_EXTERNAL(unsigned char, __fastcall, _Fodder_IsFeedType, LPVOID mgr, U32 itemTypeId);

// CmStable::bornAnimal(stable, inventory, animalTypeId, quality). RVA 0x1AC2E0.
__CM_DECL_EXTERNAL(void, __fastcall, _Fodder_BornAnimal, LPVOID stable, LPVOID inventory, U32 animalTypeId, U32 quality);

// CmBreedingManager::_slaughterAddProduct(mgr, inventory, itemTypeId, quantity, quality, extra). RVA 0x1ABD00.
__CM_DECL_EXTERNAL(void, __fastcall, _Fodder_SlaughterAdd, LPVOID mgr, LPVOID inventory, U32 itemTypeId, U32 quantity, U32 quality, LPVOID extra);

// The engine's getGameTime() body, no arguments, returns "DD-MM-YYYY HH:MM:SS". RVA 0x468760. Only called, never detoured.
__CM_DECL_EXTERNAL(const char*, __fastcall, _Fodder_GameTime, void);

namespace Hooks
{
	namespace Engine
	{
		unsigned char __fastcall OnFodderIsFeedRec(LPVOID mgr, LPVOID typeRecord);
		unsigned char __fastcall OnFodderIsFeedType(LPVOID mgr, U32 itemTypeId);
		void __fastcall OnFodderBornAnimal(LPVOID stable, LPVOID inventory, U32 animalTypeId, U32 quality);
		void __fastcall OnFodderSlaughterAdd(LPVOID mgr, LPVOID inventory, U32 itemTypeId, U32 quantity, U32 quality, LPVOID extra);

		bool ConfigureAnimalFodder(const tinyxml2::XMLElement* root);
		void AttachAnimalFodderHook();
	}
}
