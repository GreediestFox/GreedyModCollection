// Ported 2026-10-04 from Daniel's Agriculture_mod (unchanged except the logging include, see agri_log.h).
#include "hook_plow_cart_parity.h"

#include "core/tinyxml2.h"
#include "agri_log.h"

#include <Windows.h>

#include <array>
#include <cstdint>
#include <cstring>

namespace Hooks
{
    namespace Engine
    {
        namespace
        {
            // "je +0x07" inside the acceleration-tuning function at +0x9E310
            // that excludes the Plow (object_type_id 53) from the "cart"
            // multiplier branch every other category-2 object takes.
            constexpr std::uintptr_t kPatchRva = 0x9E34D;

            constexpr std::array<std::uint8_t, 2> kOriginalBytes = { 0x74, 0x07 };  // je +0x07
            constexpr std::array<std::uint8_t, 2> kPatchedBytes  = { 0x90, 0x90 };  // nop; nop

            struct PlowCartParityState
            {
                bool enabled = false;
                bool attached = false;

                std::uintptr_t moduleBase = 0;
                std::uintptr_t patchAddress = 0;
            };

            PlowCartParityState gPlowCartParity;

            // ----------------------------------------------------------------- //
            bool MatchesExpectedBytes(
                std::uintptr_t address,
                const std::array<std::uint8_t, 2>& expected)
            {
                return std::memcmp(
                    reinterpret_cast<const void*>(address),
                    expected.data(),
                    expected.size()) == 0;
            }

            // ----------------------------------------------------------------- //
            bool WriteExecutableMemory(
                std::uintptr_t address,
                const void* bytes,
                std::size_t size)
            {
                DWORD oldProtection = 0;
                if (!VirtualProtect(
                        reinterpret_cast<void*>(address),
                        size,
                        PAGE_EXECUTE_READWRITE,
                        &oldProtection))
                {
                    return false;
                }

                std::memcpy(reinterpret_cast<void*>(address), bytes, size);
                FlushInstructionCache(
                    GetCurrentProcess(),
                    reinterpret_cast<const void*>(address),
                    size);

                DWORD ignoredProtection = 0;
                if (!VirtualProtect(
                        reinterpret_cast<void*>(address),
                        size,
                        oldProtection,
                        &ignoredProtection))
                {
                    Redshark::ShowErrorMessage(
                        "Plow cart parity hook was written, but its page protection could not "
                        "be restored.");
                }

                return true;
            }
        }

        // --------------------------------------------------------------------- //
        bool ConfigurePlowCartParity(const tinyxml2::XMLElement* root)
        {
            if (gPlowCartParity.attached)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure plow cart parity while the hook is attached.");
                return false;
            }

            gPlowCartParity = {};

            if (root == nullptr)
            {
                Redshark::ShowErrorMessage(
                    "Can't configure plow cart parity: XML root element is null.");
                return false;
            }

            const tinyxml2::XMLElement* section =
                root->FirstChildElement("plowCartParity");

            // Missing or disabled section preserves the original game behaviour.
            if (section == nullptr)
                return true;

            gPlowCartParity.enabled = section->BoolAttribute("enabled", false);
            return true;
        }

        // --------------------------------------------------------------------- //
        void AttachPlowCartParityHook()
        {
            if (!gPlowCartParity.enabled || gPlowCartParity.attached)
                return;

            Redshark::LogInfo("AttachPlowCartParityHook: checkpoint A - resolving module base.");

            gPlowCartParity.moduleBase =
                reinterpret_cast<std::uintptr_t>(GetModuleHandleW(nullptr));
            if (gPlowCartParity.moduleBase == 0)
            {
                Redshark::ShowErrorMessage(
                    "Can't attach plow cart parity hook: main module base is null.");
                return;
            }

            gPlowCartParity.patchAddress = gPlowCartParity.moduleBase + kPatchRva;

            Redshark::LogInfo(
                "AttachPlowCartParityHook: checkpoint B - module base 0x%llx, checking patch "
                "bytes at +0x9E34D.",
                static_cast<unsigned long long>(gPlowCartParity.moduleBase));

            if (!MatchesExpectedBytes(gPlowCartParity.patchAddress, kOriginalBytes))
            {
                Redshark::ShowErrorMessage(
                    "Can't attach plow cart parity hook: expected code (74 07) was not found "
                    "at +0x9E34D.");
                return;
            }

            if (!WriteExecutableMemory(
                    gPlowCartParity.patchAddress,
                    kPatchedBytes.data(),
                    kPatchedBytes.size()))
            {
                Redshark::ShowErrorMessage(
                    "Can't attach plow cart parity hook: patch memory is not writable.");
                return;
            }

            Redshark::LogInfo(
                "AttachPlowCartParityHook: checkpoint C - patched successfully. The Plow now "
                "takes the same acceleration-tuning branch ([rsi+0xc]) as every other "
                "category-2 (cart-type) object, instead of falling back to the generic "
                "default ([rsi+4]).");

            gPlowCartParity.attached = true;
        }

        // --------------------------------------------------------------------- //
        void DetachPlowCartParityHook()
        {
            if (!gPlowCartParity.attached)
                return;

            if (!WriteExecutableMemory(
                    gPlowCartParity.patchAddress,
                    kOriginalBytes.data(),
                    kOriginalBytes.size()))
            {
                Redshark::ShowErrorMessage(
                    "Can't detach plow cart parity hook: original bytes could not be restored.");
                return;
            }

            gPlowCartParity.attached = false;
        }
    }
}
