@echo off
rem Publishes the website (runs "cf.cmd deploy" in this folder).
rem First time only: run  npm.cmd install  in this folder.
rem If it asks you to log in or says the login expired:  cf.cmd auth login
cd /d "%~dp0"
echo.
echo Website wird veroeffentlicht ...
echo.
call cf.cmd deploy
echo.
echo Fertig. Es kann bis zu einer Minute dauern, bis alle Besucher die neue Version sehen.
pause
