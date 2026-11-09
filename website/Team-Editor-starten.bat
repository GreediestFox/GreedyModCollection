@echo off
rem Starts the team editor (needs Node.js) and opens it in your browser.
rem Close this window (or press Ctrl+C) to stop it.
cd /d "%~dp0"
start "" /min cmd /c "timeout /t 2 /nobreak >nul & start "" http://localhost:8002/__edit/"
node tools\editor.mjs
pause
