/* ===================================================================================
	Logging adapter for the Agriculture hooks. See agri_log.h.
*  =================================================================================== */

#include "agri_log.h"

#include "core/cm_aux.h"

#include <cstdarg>
#include <cstdio>
#include <ctime>

namespace Redshark
{
	namespace
	{
		void Write(const char* level, const char* format, va_list args)
		{
			char msg[1024];
			vsnprintf(msg, sizeof(msg), format, args);
			std::time_t now = std::time(nullptr);
			std::tm tmv{};
			localtime_s(&tmv, &now);
			char stamp[32];
			std::strftime(stamp, sizeof(stamp), "%Y-%m-%d %H:%M:%S", &tmv);
			std::FILE* f = nullptr;
			if (fopen_s(&f, "logs/agriculture.log", "a") == 0 && f)
			{
				std::fprintf(f, "%s [%s] %s\n", stamp, level, msg);
				std::fclose(f);
			}
		}
	}

	void ShowErrorMessage(const char* format, ...)
	{
		char msg[1024];
		va_list args;
		va_start(args, format);
		vsnprintf(msg, sizeof(msg), format, args);
		va_end(args);
		std::FILE* f = nullptr;
		if (fopen_s(&f, "logs/agriculture.log", "a") == 0 && f)
		{
			std::time_t now = std::time(nullptr);
			std::tm tmv{};
			localtime_s(&tmv, &now);
			char stamp[32];
			std::strftime(stamp, sizeof(stamp), "%Y-%m-%d %H:%M:%S", &tmv);
			std::fprintf(f, "%s [ERROR] %s\n", stamp, msg);
			std::fclose(f);
		}
		Lifx::ShowErrorMessage("%s", msg);
	}

	void LogInfo(const char* format, ...)
	{
		va_list args;
		va_start(args, format);
		Write("INFO", format, args);
		va_end(args);
	}
}
