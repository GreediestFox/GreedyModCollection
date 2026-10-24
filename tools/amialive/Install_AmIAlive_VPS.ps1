# Installs AmIAlive as the watchdog of the LiF server (run once, as Administrator, from the server folder).
# - scheduled task "AmIAlive": starts AmIAlive.exe at every boot (SYSTEM, hidden, restarts itself if it ever exits)
# - the old task "LiF Server" (boot + every 5 min) is removed: AmIAlive takes over its job
$ErrorActionPreference = 'Stop'
$dir = $PSScriptRoot
$exe = Join-Path $dir 'AmIAlive.exe'
foreach ($f in 'AmIAlive.exe', 'Start_LiF_Server_VPS.bat', 'ddctd_cm_yo_server.exe') { if (-not (Test-Path (Join-Path $dir $f))) { throw "$f not found in $dir (run this from the server folder)." } }
Get-ChildItem $dir -File | Unblock-File
try { Add-MpPreference -ExclusionPath $dir; Add-MpPreference -ExclusionProcess 'AmIAlive.exe' } catch { Write-Warning 'Defender exclusion not set' }

if (Get-ScheduledTask -TaskName 'LiF Server' -ErrorAction SilentlyContinue) {
    Unregister-ScheduledTask -TaskName 'LiF Server' -Confirm:$false
    Write-Host 'Old task "LiF Server" removed'
}
Get-Process AmIAlive -ErrorAction SilentlyContinue | Stop-Process -Force

$action    = New-ScheduledTaskAction -Execute $exe -WorkingDirectory $dir
$trigger   = New-ScheduledTaskTrigger -AtStartup
$principal = New-ScheduledTaskPrincipal -UserId 'SYSTEM' -LogonType ServiceAccount -RunLevel Highest
$settings  = New-ScheduledTaskSettingsSet -StartWhenAvailable -MultipleInstances IgnoreNew -ExecutionTimeLimit ([TimeSpan]::Zero) -RestartCount 999 -RestartInterval (New-TimeSpan -Minutes 1)
Register-ScheduledTask -TaskName 'AmIAlive' -Action $action -Trigger $trigger -Principal $principal -Settings $settings -Force | Out-Null
Start-ScheduledTask -TaskName 'AmIAlive'

Write-Host ''
Write-Host 'Task "AmIAlive" created and started. It watches the server and starts it when it is not running.'
Write-Host 'In the server folder:'
Write-Host '  .\AmIAlive.exe status     what it sees'
Write-Host '  .\AmIAlive.exe restart    planned restart (closes the server, starts it again; the watchdog waits)'
Write-Host '  .\AmIAlive.exe stop       stop on purpose (the watchdog leaves it off)'
Write-Host '  .\AmIAlive.exe start      start again after a stop'
Write-Host 'Log: AmIAlive.log in the server folder.'
