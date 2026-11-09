@echo off
rem Publishes the placeholder of the subdomain rezepte.wulpischelande.de (folder "rezepte-publish").
rem If it asks you to log in or says the login expired:  cf.cmd auth login
cd /d "%~dp0rezepte-publish"
echo.
if not exist node_modules (
  echo Erster Start: Pakete werden installiert ...
  call npm.cmd install
)
echo Rezepte-Seite wird veroeffentlicht ...
call cf.cmd deploy
echo.
echo Fertig. Adresse: https://rezepte.wulpischelande.de
echo (Neue Subdomains brauchen manchmal ein paar Minuten, bis sie erreichbar sind.)
pause
