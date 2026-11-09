@echo off
rem Keeps the website's server status up to date (reads the game server's log, uploads status.json to R2).
rem Leave this window open while the server runs. Close it to stop.
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -Command "& ([scriptblock]::Create([IO.File]::ReadAllText('tools\status-updater.ps1')))"
pause
