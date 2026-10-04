#pragma once

/* ===================================================================================
	Logging adapter for the hooks ported from Daniel's Agriculture_mod (hook_plow_cart_parity,
	hook_harvest_soil, hook_plow_area). Those files call Redshark::ShowErrorMessage / Redshark::LogInfo
	(printf-style); here both append to logs/agriculture.log, and errors are also passed to
	Lifx::ShowErrorMessage like every other Plus hook.
*  =================================================================================== */

namespace Redshark
{
	void ShowErrorMessage(const char* format, ...);
	void LogInfo(const char* format, ...);
}
