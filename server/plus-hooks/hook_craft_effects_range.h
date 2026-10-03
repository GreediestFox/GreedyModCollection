#pragma once

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
*  =================================================================================== *//*
	Craft item effect id range: CraftItemEffectsManager::init (RVA 0x4DA990) only accepts <effect id> 1..38 from data/item_effects.xml
	(`cmp ecx,25h` on id-1 at RVA 0x4DAC33 and `cmp dil,26h` on the byte index at RVA 0x4DAC3F); anything above is dropped with
	"bad item effect id". The DB `effects` table itself is loaded into a byte-keyed std::map (CmMixingManager), so it has no such limit.
	This hook raises both immediates in memory (never on disk) to accept ids 1..127.

	    <craftEffectsRange enabled="1" />
*/

#include "server/cm_server.h"

namespace tinyxml2 { class XMLElement; }

namespace Hooks
{
	namespace Engine
	{
		bool ConfigureCraftEffectsRange(const tinyxml2::XMLElement* root);
		void AttachCraftEffectsRangeHook();
	}
}
