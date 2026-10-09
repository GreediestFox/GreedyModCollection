/* ===================================================================================
    Random gather quantity per gatherable type - public interface (2026-10-09).

    data/gatherables.xml only knows a fixed <quantity> per gatherable type; the server reads it through
    Gathering::GetQuantitybyGatherType(type) (RVA +0x3797C0, returns the byte at entry+0x2A of the
    gatherables table). This hook detours that function: for the configured types it returns a random
    value between min and max (inclusive, 1..255), every other type keeps the XML value.

        <gatherQuantity enabled="1">
            <range type="14" min="10" max="20" />   <!-- plant fiber (item 471) -->
        </gatherQuantity>

    Verified build yo_1.4.4.5, SHA-256 ACD99BD1A785D95B06494CBADE51A2C14EE60A521D0E88A4BE6AE1B3B685219F.
*  =================================================================================== */

#pragma once

namespace tinyxml2
{
    class XMLElement;
}

namespace Hooks
{
    namespace Engine
    {
        bool ConfigureGatherQuantity(const tinyxml2::XMLElement* root);
        void AttachGatherQuantityHook();
        void DetachGatherQuantityHook();
    }
}
