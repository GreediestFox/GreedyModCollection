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
*  =================================================================================== */

/*
	Stable alias: lets a custom building behave as one of the vanilla animal-husbandry buildings (Coop 144, Barn 143/517,
	Small Stable 133, Large Stable 134) and restricts which animals a stable accepts.

	How the engine wires stables (found by decompilation of ddctd_cm_yo_server.exe):

	  1. CmBreedingManager's boot query loads only the completed unmovable objects whose ObjectTypeID is in a hard-coded
	     list: the literal " WHERE IsComplete =1 AND ObjectTypeID IN (133,134,143,144,517) AND (" at RVA 0x7A1210 (68 chars,
	     the length is baked into the code as lea r8d,[rax+44h]). It is rewritten in memory, same length, so the list also
	     contains the aliased types (spaces are dropped elsewhere to make room: one extra 4-digit id fits).
	  2. A parameter table in .data (RVA 0xACEB40, 11 dwords per record, keyed by stable type, terminated by -1) holds per
	     type: max animals (dword 5), harvest/dung numbers (6, 7, 8) and the harvest product item (10). It is read by six
	     tiny getters, each detoured here so an aliased type is looked up as its behavesLike type. Optional per-stable
	     overrides: maxAnimals, product (0 = nothing to harvest).
	     RVAs: 0x1B0EB0 isStable(type); 0x1B0D60 maxAnimals; 0x1B0D20 / 0x1B04D0 / 0x1B0410 numeric parameters;
	     0x1B07E0 product item.
	  3. CmBreedingManager::checkContainerLimits (RVA 0x1AC440) is the gate every item move into a building container passes.
	     It only counts animals (descendants of 0x409 "Animals") and weighs feed; it never looks at the species. The
	     <stable allowAnimals=".."> rule is applied by wrapping it: the wrapper records the item type in TLS, isStable(type)
	     (called first, with the container's real type) flags a violation, and the wrapper then returns 0 (item refused).

	Configured from lifxpluss.xml:
	    <stableAlias enabled="1" verbose="1">
	        <stable objectTypeId="2834" behavesLike="144" maxAnimals="30" product="0" allowAnimals="1053" />
	        <stable objectTypeId="144" allowAnimals="1052" />
	    </stableAlias>
	(allowAnimals is a space separated list of item type ids; other, non-animal items such as feed are never restricted, unless strict="1" is set:
	 then EVERY other item is refused, which turns a plain container into a single-item store, e.g. <stable objectTypeId="2834" allowAnimals="1053" strict="1" />.
	 A stable entry without behavesLike only adds rules and leaves the building's own table record alone.)
*/

#include "server/cm_server.h"

namespace tinyxml2 { class XMLElement; }

// isStable(mgr, containerType) -> bool. RVA 0x1B0EB0.
__CM_DECL_EXTERNAL(unsigned char, __fastcall, _Stable_IsStable, LPVOID mgr, int typeId);

// Table getters (mgr, stableType) -> dword.
__CM_DECL_EXTERNAL(U32, __fastcall, _Stable_MaxAnimals, LPVOID mgr, int typeId);   // RVA 0x1B0D60, dword 5
__CM_DECL_EXTERNAL(U32, __fastcall, _Stable_Param6, LPVOID mgr, int typeId);       // RVA 0x1B04D0, dword 6
__CM_DECL_EXTERNAL(U32, __fastcall, _Stable_Param7, LPVOID mgr, int typeId);       // RVA 0x1B0D20, dword 7
__CM_DECL_EXTERNAL(U32, __fastcall, _Stable_Param8, LPVOID mgr, int typeId);       // RVA 0x1B0410, dword 8
__CM_DECL_EXTERNAL(U32, __fastcall, _Stable_Product, LPVOID mgr, int typeId);      // RVA 0x1B07E0, dword 10

// isAnimalItemType(mgr, itemTypeId) -> bool (descendant of 0x409 Animals). RVA 0x1B0DA0. Only called, never detoured.
__CM_DECL_EXTERNAL(unsigned char, __fastcall, _Stable_IsAnimal, LPVOID mgr, U32 itemTypeId);

// CmBreedingManager::checkContainerLimits(mgr, objectId*, itemTypeId, quantity, container) -> bool. RVA 0x1AC440.
__CM_DECL_EXTERNAL(unsigned char, __fastcall, _Stable_CheckLimits,
                   LPVOID mgr, LPVOID objectId, U32 itemTypeId, U32 quantity, LPVOID container);

namespace Hooks
{
	namespace Engine
	{
		unsigned char __fastcall OnStableIsStable(LPVOID mgr, int typeId);
		U32 __fastcall OnStableMaxAnimals(LPVOID mgr, int typeId);
		U32 __fastcall OnStableParam6(LPVOID mgr, int typeId);
		U32 __fastcall OnStableParam7(LPVOID mgr, int typeId);
		U32 __fastcall OnStableParam8(LPVOID mgr, int typeId);
		U32 __fastcall OnStableProduct(LPVOID mgr, int typeId);
		unsigned char __fastcall OnStableCheckLimits(LPVOID mgr, LPVOID objectId, U32 itemTypeId, U32 quantity, LPVOID container);

		bool ConfigureStableAlias(const tinyxml2::XMLElement* root);
		void AttachStableAliasHook();
	}
}
