# Installs auto-start for the LiF server on this VPS (run once, as Administrator, from the server folder).
# Creates the scheduled task "LiF Server": runs Start_LiF_Server_VPS.bat at every boot (no login needed, as SYSTEM)
# and every 5 minutes afterwards, so a crashed server is started again. The script itself does nothing when the server already runs.
$ErrorActionPreference = 'Stop'
$dir = $PSScriptRoot
$bat = Join-Path $dir 'Start_LiF_Server_VPS.bat'
if (-not (Test-Path $bat)) { throw "Start_LiF_Server_VPS.bat not found in $dir (put this file into the server folder)." }
if (-not (Test-Path (Join-Path $dir 'ddctd_cm_yo_server.exe'))) { throw "ddctd_cm_yo_server.exe not found in $dir." }

# MariaDB service must start with Windows
foreach ($n in 'MariaDB', 'MySQL') {
    $svc = Get-Service -Name $n -ErrorAction SilentlyContinue
    if ($svc) { Set-Service -Name $n -StartupType Automatic; Write-Host "Service $n set to Automatic" }
}

$action    = New-ScheduledTaskAction -Execute 'cmd.exe' -Argument "/c `"$bat`" auto" -WorkingDirectory $dir
$atBoot    = New-ScheduledTaskTrigger -AtStartup
$watchdog  = New-ScheduledTaskTrigger -Once -At (Get-Date).AddMinutes(2) -RepetitionInterval (New-TimeSpan -Minutes 5) -RepetitionDuration (New-TimeSpan -Days 3650)
$principal = New-ScheduledTaskPrincipal -UserId 'SYSTEM' -LogonType ServiceAccount -RunLevel Highest
$settings  = New-ScheduledTaskSettingsSet -StartWhenAvailable -MultipleInstances IgnoreNew -ExecutionTimeLimit (New-TimeSpan -Minutes 10)
Register-ScheduledTask -TaskName 'LiF Server' -Action $action -Trigger @($atBoot, $watchdog) -Principal $principal -Settings $settings -Force | Out-Null

# Defender exclusion for the folder (so the hook DLLs are not removed)
try { Add-MpPreference -ExclusionPath $dir; Add-MpPreference -ExclusionProcess 'ddctd_cm_yo_server.exe' } catch { Write-Warning 'Defender exclusion not set' }

Write-Host ''
Write-Host 'Task "LiF Server" created. Start it now with:  Start-ScheduledTask -TaskName "LiF Server"'
Write-Host 'Stop for maintenance (otherwise it is restarted within 5 min):'
Write-Host '   Disable-ScheduledTask -TaskName "LiF Server"; Stop-Process -Name ddctd_cm_yo_server -Force'
Write-Host 'Enable again:  Enable-ScheduledTask -TaskName "LiF Server"'
Write-Host 'Remove:        Unregister-ScheduledTask -TaskName "LiF Server" -Confirm:$false'
