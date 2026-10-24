@echo off
setlocal
set "AUTO=%~1"
rem ============================================================================
rem  Start the LiF dedicated server on a Windows VPS.
rem
rem  - Run it as Administrator (right-click -> Run as administrator). It then
rem    also adds the Defender exclusion and the firewall rules (28000-28002).
rem  - It finds MariaDB by itself and reads the database password from
rem    config_local.cs, so nothing has to be edited here.
rem  - Do NOT start the server through Steam (AppID mismatch, see the old bat).
rem  - If the server dies in the first seconds, this window stays open and shows
rem    the reason (missing files / Defender / last log lines).
rem ============================================================================
set "SRV=%~dp0"
set "SRV=%SRV:~0,-1%"
set "WORLD=1"
cd /d "%SRV%"
if not exist "%SRV%\logs" mkdir "%SRV%\logs"
tasklist /FI "IMAGENAME eq ddctd_cm_yo_server.exe" 2>nul | find /I "ddctd_cm_yo_server.exe" >nul
if not errorlevel 1 (if "%AUTO%"=="auto" exit /b 0)
echo.
echo === LiF server start ===
echo Folder: %SRV%

if exist "%SRV%\.setup_done" goto skipsetup
rem ---- 1) Defender exclusion + firewall rules (need admin; silently skipped otherwise)
powershell -NoProfile -Command "try { Add-MpPreference -ExclusionPath '%SRV%' -ErrorAction Stop; Add-MpPreference -ExclusionProcess 'ddctd_cm_yo_server.exe' -ErrorAction Stop; Write-Host 'Defender exclusion OK' } catch { Write-Host 'Defender exclusion NOT set (run as Administrator?)' }"
netsh advfirewall firewall show rule name="LiF Server UDP" >nul 2>&1 || netsh advfirewall firewall add rule name="LiF Server UDP" dir=in action=allow protocol=UDP localport=28000-28002 >nul 2>&1
netsh advfirewall firewall show rule name="LiF Server TCP" >nul 2>&1 || netsh advfirewall firewall add rule name="LiF Server TCP" dir=in action=allow protocol=TCP localport=28000-28002 >nul 2>&1
echo done>"%SRV%\.setup_done"
:skipsetup

rem ---- 2) Are all important files there? (a missing one = Defender quarantined it or the copy was incomplete)
set "MISSING="
for %%F in (ddctd_cm_yo_server.exe pdh.dll 4ba5cb5e.dll libmariadb.dll steam_api64.dll steam_appid.txt config_local.cs config\world_%WORLD%.xml config\lifxpluss.xml ServerAutoloader\main.cs) do (
  if not exist "%SRV%\%%F" (
    echo MISSING: %%F
    set "MISSING=1"
  )
)
if defined MISSING (
  echo.
  echo Files are missing. Check Windows Security - Protection history for quarantined files
  echo ^(restore them, "Allow on device"^) and copy the server folder again after the exclusion.
  powershell -NoProfile -Command "Get-MpThreatDetection -ErrorAction SilentlyContinue | Where-Object { $_.Resources -like '*%SRV%*' } | ForEach-Object { $_.Resources }"
  if not "%AUTO%"=="auto" pause
  exit /b 1
)

rem ---- 3) Find MariaDB
set "MDB="
for /d %%D in ("%ProgramFiles%\MariaDB*" "%ProgramFiles(x86)%\MariaDB*") do if exist "%%D\bin\mysqld.exe" set "MDB=%%D"
if not defined MDB (
  echo MariaDB was not found under Program Files. Install MariaDB first ^(see instructions^).
  if not "%AUTO%"=="auto" pause
  exit /b 1
)
echo MariaDB: %MDB%

rem ---- 4) DB password from config_local.cs (used by mysql.exe through MYSQL_PWD)
for /f "usebackq delims=" %%P in (`powershell -NoProfile -Command "[regex]::Match([IO.File]::ReadAllText('%SRV%\config_local.cs'),'Connect::password\s*=\s*\x22([^\x22]*)\x22').Groups[1].Value"`) do set "MYSQL_PWD=%%P"
if not defined MYSQL_PWD (
  echo Could not read the database password from config_local.cs.
  if not "%AUTO%"=="auto" pause
  exit /b 1
)

rem ---- 5) Start MariaDB if it is not running (service first, else console process)
tasklist /FI "IMAGENAME eq mysqld.exe" 2>nul | find /I "mysqld.exe" >nul
if not errorlevel 1 goto waitinit
echo Starting MariaDB...
set "SVC="
for %%S in (MariaDB MySQL) do sc query %%S >nul 2>&1 && set "SVC=%%S"
if defined SVC net start %SVC% >nul 2>&1
tasklist /FI "IMAGENAME eq mysqld.exe" 2>nul | find /I "mysqld.exe" >nul
if not errorlevel 1 goto waitinit
powershell -NoProfile -Command "Start-Process -FilePath '%MDB%\bin\mysqld.exe' -ArgumentList '--console' -WindowStyle Hidden -RedirectStandardOutput \"$env:TEMP\mysqld_stdout.txt\" -RedirectStandardError \"$env:TEMP\mysqld_stderr.txt\""
:waitinit
set /a TRIES=0
:waitdb
"%MDB%\bin\mysql.exe" --user=root --host=127.0.0.1 -N -e "select 1" >nul 2>&1
if not errorlevel 1 goto dbok
set /a TRIES+=1
if %TRIES% GEQ 40 (
  echo MariaDB does not answer with the password from config_local.cs ^(user root, 127.0.0.1^).
  echo Check that MariaDB runs and that the root password matches config_local.cs.
  if not "%AUTO%"=="auto" pause
  exit /b 1
)
ping -n 3 127.0.0.1 >nul
goto waitdb
:dbok
echo MariaDB is up.

rem ---- 6) Do not start twice
tasklist /FI "IMAGENAME eq ddctd_cm_yo_server.exe" 2>nul | find /I "ddctd_cm_yo_server.exe" >nul
if not errorlevel 1 (
  echo Server is already running. Close it first if you want to restart it.
  if not "%AUTO%"=="auto" pause
  exit /b 1
)

rem ---- 7) Stale-link cleanup before boot (harmless if the database is still empty)
"%MDB%\bin\mysql.exe" --user=root --host=127.0.0.1 --database=lif_1 -B -N -e "SELECT NOW(), u.ID, u.UnmovableObjectID, u.RecipeRequirementID, u.RegionID FROM unmovable_objects_requirements u JOIN unmovable_objects o ON o.ID=u.UnmovableObjectID WHERE o.IsComplete=1 AND u.RecipeRequirementID>=6000;" >> "%SRV%\logs\stale_uor_removed.log" 2>nul
"%MDB%\bin\mysql.exe" --user=root --host=127.0.0.1 --database=lif_1 -e "DELETE u FROM unmovable_objects_requirements u JOIN unmovable_objects o ON o.ID=u.UnmovableObjectID WHERE o.IsComplete=1 AND u.RecipeRequirementID>=6000;" 2>nul
"%MDB%\bin\mysql.exe" --user=root --host=127.0.0.1 --database=lif_1 -e "DELETE wc FROM working_containers wc JOIN unmovable_objects uo ON uo.ID=wc.UnmovableObjectID LEFT JOIN items i ON i.ContainerID=uo.RootContainerID WHERE i.ID IS NULL AND uo.ObjectTypeID IN (2829,2017);" 2>nul

rem ---- 8) Start the server (Steam variables cleared, steam_appid.txt supplies the AppID)
rem Players hold tickets for the game (290080), not for the server tool (320850): force the right AppID, also in steam_appid.txt (Steam updates/validation put 320850 back)
<nul set /p="290080">"%SRV%\steam_appid.txt"
set SteamAppId=290080
set SteamGameId=290080
set SteamOverlayGameId=290080
set "MARIADB_PLUGIN_DIR=%MDB%\lib\plugin"
start "LiF Dedicated Server" /D "%SRV%" ddctd_cm_yo_server.exe -worldID %WORLD%
echo Server starting. It is ready when the log says "ready to accept connections" ^(about 90 seconds^).

rem ---- 9) If it dies within 25 seconds, say why
ping -n 26 127.0.0.1 >nul
tasklist /FI "IMAGENAME eq ddctd_cm_yo_server.exe" 2>nul | find /I "ddctd_cm_yo_server.exe" >nul
if errorlevel 1 (
  echo.
  echo *** The server process is gone after a few seconds. ***
  echo 1. Windows Security - Protection history: was a DLL/exe of this folder blocked or removed?
  echo 2. Visual C++ 2015-2022 Redistributable x64 and x86 installed?
  echo 3. Last lines of the newest log:
  powershell -NoProfile -Command "$f = Get-ChildItem '%SRV%\logs' -File -ErrorAction SilentlyContinue | Where-Object { $_.Name -ne 'stale_uor_removed.log' } | Sort-Object LastWriteTime -Descending | Select-Object -First 1; if ($f) { Write-Host $f.FullName; Get-Content $f.FullName -Tail 30 } else { Write-Host 'no log file found' }"
  if not "%AUTO%"=="auto" pause
  exit /b 1
)
echo Server process is running. You can close this window.
if not "%AUTO%"=="auto" pause