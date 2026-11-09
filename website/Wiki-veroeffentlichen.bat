@echo off
rem Builds the wiki (folder "wiki"), copies the result to "wiki-publish\public" and publishes it from there.
rem (The wiki is deployed from its own plain folder because cf.cmd would otherwise try to build it as a framework project.)
rem If it asks you to log in or says the login expired:  cf.cmd auth login
set "ROOT=%~dp0"
echo.
cd /d "%ROOT%wiki"
if not exist node_modules (
  echo Erster Start: Pakete werden installiert ... das dauert ein paar Minuten.
  call npm.cmd install
)
echo Wiki wird gebaut ...
call npm.cmd run build
if errorlevel 1 (
  echo.
  echo FEHLER beim Bauen. Bitte den Text oben kopieren und an Claude schicken.
  pause
  exit /b 1
)
echo.
robocopy "%ROOT%wiki\dist" "%ROOT%wiki-publish\public" /MIR /NFL /NDL /NJH /NJS >nul
cd /d "%ROOT%wiki-publish"
if not exist node_modules (
  call npm.cmd install
)
echo Wiki wird veroeffentlicht ...
call cf.cmd deploy
echo.
echo Fertig. Adresse: https://wiki.wulpischelande.de
pause
