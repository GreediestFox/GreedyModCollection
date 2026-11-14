@echo off
rem Publishes the small status helper (status.wulpischelande.de) that reads the server listing on life-is-feudal.org.
rem It uses the wrangler of the website project (no extra install).
rem If it asks you to log in or says the login expired, run this once in this folder:   node_modules\.bin\wrangler.cmd login
cd /d "%~dp0status-worker"
echo.
echo Status-Helfer wird veroeffentlicht ...
call "%~dp0node_modules\.bin\wrangler.cmd" deploy
echo.
echo Fertig. Test im Browser: https://status.wulpischelande.de/
echo (Neue Subdomains brauchen manchmal ein paar Minuten, bis sie erreichbar sind.)
pause
