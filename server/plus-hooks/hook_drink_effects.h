#pragma once

/* ===================================================================================
	Copyright (c) 2026, LiFx Contributors. All Rights Reserved.
	Contributed by GreedyFox.

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
	Drink effects: an item type that gives its own buff and drawback when drunk.

	Found by decompilation of ddctd_cm_yo_server.exe:

	  AbilityImp::Drink::_onDoPerform (ability 205, RVA 0x36F850) calls CharacterSaveableEffects::applyPotionItemEffect
	  (RVA 0x1C4A40; fx = CharacterInfo+0x388, item = the drunk inventory item, mult = 1.0f). That function asks the item for its
	  ItemEffectsFeature (RVA 0x27C040; item feature 5): a list of (effects.ID byte, value) entries that comes from the item
	  INSTANCE (database: items.FeatureID -> features.has_effects + item_effects rows, loaded with the inventory). With no list
	  the item only gives "Full" (player effect 25). The core "add effect" routine is RVA 0x1C3AA0
	  (fx, playerEffectId, float value, int durationMs, int 0); the duration the engine passes is q*q*1000/15 ms (q = item quality,
	  u16 at item+0x20; constants at RVA 0x736568 and 0x73AB8C), the value is Magnitude/1000.

	This hook wraps applyPotionItemEffect. For an item type that has a <drink> rule and no effect list of its own, it applies the
	configured player effects first and then lets the engine run, which adds Full as usual. Items that do carry a list (database)
	are left alone, so both ways can coexist. The item type is read from the database by the item id (item+0x08), because the
	item object layout beyond id (+0x08), container (+0x10), quality (+0x20) and the feature table (+0x40) is not documented.

	Configured from lifxpluss.xml (effect id = PLAYER effect id of data/cm_effects.xml, magnitude as in item_effects.Magnitude/1000):
	    <drinkEffects enabled="1" verbose="1" dump="0">
	        <db host="127.0.0.1" port="3306" user="root" password="..." name="lif_1" />
	        <drink objectTypeId="3920">                       <!-- Thin Beer -->
	            <effect id="6" magnitude="0.10" />            <!-- Accelerated -->
	            <effect id="7" magnitude="0.10" />            <!-- Clumsiness -->
	        </drink>
	    </drinkEffects>
	(dump="1" logs the raw item fields of the first drinks to logs/drink_effects.log, to document the item layout)
*/

#include "server/cm_server.h"

namespace tinyxml2 { class XMLElement; }

// CharacterSaveableEffects::applyPotionItemEffect(fx, item, float mult) -> applied. RVA 0x1C4A40. NOTE: mult is a float (xmm2); it is the magnitude of the Full effect (x1e6).
__CM_DECL_EXTERNAL(U64, __fastcall, _Drink_ApplyPotionItemEffect, LPVOID fx, LPVOID item, float mult);

namespace Hooks
{
	namespace Engine
	{
		U64 __fastcall OnDrinkApplyPotionItemEffect(LPVOID fx, LPVOID item, float mult);

		bool ConfigureDrinkEffects(const tinyxml2::XMLElement* root);
		void AttachDrinkEffectsHook();
	}
}
