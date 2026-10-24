$d = 'E:\ClaudeScratch\project_archive\Session_2026-10-14\AmIAlive'
$t = "$d\_test"
Remove-Item $t -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory $t | Out-Null
"process=ping`r`nbat=fake.bat`r`ngrace=12`r`ninterval=3`r`n" | Set-Content "$t\AmIAlive.ini" -Encoding ascii
"@echo off`r`nif `"%1`"==`"wait`" (ping -n 100 127.0.0.1 >nul & exit /b)`r`nstart `"`" /min ping -n 600 127.0.0.1 >nul`r`n" | Set-Content "$t\fake.bat" -Encoding ascii
Get-Process ping -ErrorAction SilentlyContinue | Stop-Process -Force

# the watchdog runs in a background job (own PowerShell process, AMIALIVE_DIR points at the test folder)
$job = Start-Job -ScriptBlock {
    param($asmPath, $dir)
    $env:AMIALIVE_DIR = $dir
    $a = [Reflection.Assembly]::LoadFile($asmPath)
    $a.EntryPoint.Invoke($null, @(,[string[]]@()))
} -ArgumentList "$d\AmIAlive.exe", $t

function Cmd($arg) {
    $env:AMIALIVE_DIR = $t
    $a = [Reflection.Assembly]::LoadFile("$d\AmIAlive.exe")
    $a.EntryPoint.Invoke($null, @(,[string[]]@($arg))) | Out-Null
}
function Np { (Get-Process ping -ErrorAction SilentlyContinue | Measure-Object).Count }

Start-Sleep 8
"1) watchdog started notepad on its own: " + ((Np) -gt 0)
Get-Process ping | Stop-Process -Force
Start-Sleep 18
"2) after kill, restarted after grace: " + ((Np) -gt 0) + " (count " + (Np) + ")"
$old = (Get-Process ping | Select-Object -First 1).Id
Cmd 'restart'
Start-Sleep 2
$new = (Get-Process ping -ErrorAction SilentlyContinue | Select-Object -First 1).Id
"3) planned restart: old pid $old -> new pid $new, count " + (Np) + " (must be 1)"
Cmd 'stop'
Start-Sleep 20
"4) after stop, notepad running (must be False): " + ((Np) -gt 0)
Cmd 'start'
Start-Sleep 3
"5) after start, running: " + ((Np) -gt 0)
$c = Start-Process cmd -ArgumentList '/c', 'fake.bat', 'wait' -PassThru -WorkingDirectory $t -WindowStyle Hidden
Start-Sleep 20
$c.Refresh()
"6) old start-bat window closed (must be True): " + $c.HasExited
Stop-Job $job; Remove-Job $job -Force
Get-Process ping -ErrorAction SilentlyContinue | Stop-Process -Force
Get-Content "$t\AmIAlive.log" | Select-Object -Last 30
