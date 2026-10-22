# 23 Running the server on a Windows VPS

Setup of the dedicated server on a rented Windows VPS (remote desktop). `server/vps/Start_LiF_Server_VPS.bat` does the repeatable parts: Defender exclusion + firewall rules (UDP/TCP 28000-28002), file check, MariaDB detection/start, DB password read from `config_local.cs`, stale-link cleanup, start, and a "why did it die" report.

Problems met, in the order they appeared:

1. **Defender removes the hook DLLs.** `pdh.dll` (proxy loaded in place of the Windows DLL) and `4ba5cb5e.dll` (the Plus hook DLL) get flagged. Add the server folder as an exclusion BEFORE copying, restore quarantined files ("Allow on device"), `Unblock-File` everything that came through a browser download.
2. **Error #2059 "Plugin auth_gssapi_client could not be loaded ... MariaDB 10.2/lib/plugin".** The server has the 10.2 plugin path compiled in. Set `MARIADB_PLUGIN_DIR` to the installed MariaDB's `lib\plugin` (the bat does this).
3. **MariaDB must be installed** (12.x LTS-like stable, not 13 rolling/preview). Root password must equal the one in `config_local.cs`; no remote root access; add `max_sp_recursion_depth=255` and `max_allowed_packet=64M` to `my.ini`; do not copy `innodb_file_format` from the shipped `docs/my.ini` (rejected by new MariaDB).
4. **No hook lines in the log + "wrong gatherable type ... ability id=68 ... Fatal: Can't load skills/abilities".** The hook DLLs did not load, so our crop types (rye, barley, oat, hops, sugarbeet) were unknown. Cause: the **Visual C++ 2015-2022 Redistributable x64** was missing (both DLLs import VCRUNTIME140/MSVCP140). A healthy start shows `[lifx-... ] hook attached` lines at the top of the log.
5. **Ports:** game port is `<port>` in `config/world_1.xml` (also uses +1, +2). The provider panel had no firewall page; the Windows Firewall rule was enough. `Test-NetConnection <ip> -Port 28000` from a home PC checks TCP.
6. **Lag when joining from inside the VPS** is the client running without a GPU, not the server; from a home PC it runs smoothly.
7. **GM password** comes only from server files: write the new password (6+ chars) into `mods/LiFx/GMCommands/gm_setpass.txt`; the server hashes it into `gm_password.cs` (never edit that file by hand).