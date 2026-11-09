# Status updater for the website ("Server-Status" card and the Online/Offline badge).
# Every 60 seconds it looks at the game server on THIS PC and uploads a tiny status.json to your R2 bucket:
#   { "online": true, "players": 3, "version": "1.0.17", "updated": "2026-10-15T18:00:00Z" }
# It only READS the server log and the process list. It never touches the game server or its database.
#
#   Start:   Status-Updater-starten.bat      (keep the window open; close it to stop)
#   Test:    powershell -File tools\status-updater.ps1 -Once -NoUpload
#
# How it decides:
#   online  = the server process is running AND its newest log says "ready to accept connections"
#   players = joins minus leaves in that log (every "GameConnection::onConnect" adds one, every "onDisconnect" removes one)
#   version = the modpack version from the public manifest.json (what players get through the patcher)

param(
  [string]$ServerDir   = "G:\Progam (Launcher)\steamapps\common\Life is Feudal Your Own Dedicated Server",
  [string]$ManifestUrl = "https://pub-23d5a23dd23640eaa704c9caa9ad4cf1.r2.dev/manifest.json",
  [string]$Remote      = "r2:gmc/status.json",
  [string]$Rclone      = "",
  [int]$IntervalSeconds = 60,
  [switch]$Once,
  [switch]$NoUpload,
  [string]$LogFile     = "",        # test only: count players in this log file
  [switch]$AssumeRunning            # test only: pretend the server process is running
)

$ErrorActionPreference = 'Stop'
$script:lastVersion = $null
$script:lastVersionCheck = [datetime]::MinValue

function Find-Rclone {
  if ($Rclone -and (Test-Path $Rclone)) { return $Rclone }
  $c = Get-Command rclone.exe -ErrorAction SilentlyContinue
  if ($c) { return $c.Source }
  $pk = Join-Path $env:LOCALAPPDATA 'Microsoft\WinGet\Packages'
  $f = Get-ChildItem $pk -Recurse -Filter rclone.exe -ErrorAction SilentlyContinue | Select-Object -First 1
  if ($f) { return $f.FullName }
  return $null
}

function Get-NewestLog {
  if ($LogFile) { return $LogFile }
  $d = Join-Path $ServerDir 'logs'
  if (-not (Test-Path $d)) { return $null }
  $f = Get-ChildItem $d -Recurse -Filter 'S1_*.log' -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1
  if ($f) { return $f.FullName }
  return $null
}

function Get-ModpackVersion {
  # asks the public manifest at most every 10 minutes; keeps the last known value if the request fails
  if (((Get-Date) - $script:lastVersionCheck).TotalMinutes -lt 10 -and $script:lastVersion) { return $script:lastVersion }
  $script:lastVersionCheck = Get-Date
  try {
    $m = (Invoke-WebRequest $ManifestUrl -UseBasicParsing -TimeoutSec 20).Content | ConvertFrom-Json
    if ($m.version) { $script:lastVersion = [string]$m.version }
  } catch { }
  return $script:lastVersion
}

function Get-ServerStatus {
  $running = [bool](Get-Process -Name 'ddctd_cm_yo_server' -ErrorAction SilentlyContinue) -or $AssumeRunning
  $log = Get-NewestLog
  $ready = $false; $ids = New-Object 'System.Collections.Generic.HashSet[string]'
  if ($log -and (Test-Path $log)) {
    # open with sharing, because the server is still writing to this file
    $fs = [IO.File]::Open($log, 'Open', 'Read', 'ReadWrite')
    $sr = New-Object IO.StreamReader($fs, [Text.Encoding]::UTF8)
    try {
      while (($line = $sr.ReadLine()) -ne $null) {
        if ($line.Length -lt 20) { continue }
        if ($line.Contains('ready to accept connections')) { $ready = $true; continue }
        if ($line.Contains('<GameConnection::onConnect>')) {
          $m = [regex]::Match($line, '<GameConnection::onConnect>\s*\[\d+\]\s*\[LiFx\]\s*(\d+)')
          if ($m.Success) { [void]$ids.Add($m.Groups[1].Value) }
        } elseif ($line.Contains('GameConnection::onDisconnect(')) {
          $m = [regex]::Match($line, 'GameConnection\|id:\s*(\d+)')
          if ($m.Success) { [void]$ids.Remove($m.Groups[1].Value) }
        }
      }
    } finally { $sr.Dispose(); $fs.Dispose() }
  }
  $online = ($running -and $ready)
  $players = 0; if ($online) { $players = $ids.Count }
  $o = [ordered]@{ online = $online; players = $players }
  $v = Get-ModpackVersion
  if ($v) { $o['version'] = $v }
  $o['updated'] = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
  return $o
}

$rc = $null
if (-not $NoUpload) {
  $rc = Find-Rclone
  if (-not $rc) { throw 'rclone.exe not found. Install it or pass -Rclone <path>. (Use -NoUpload to test without uploading.)' }
}
$tmp = Join-Path $env:TEMP 'gmc-status.json'

do {
  try {
    $s = Get-ServerStatus
    $json = ($s | ConvertTo-Json -Compress)
    [IO.File]::WriteAllText($tmp, $json, (New-Object Text.UTF8Encoding($false)))
    if ($NoUpload) {
      Write-Host ((Get-Date).ToString('HH:mm:ss') + '  ' + $json + '   (not uploaded)')
    } else {
      & $rc copyto $tmp $Remote --header-upload 'Cache-Control: no-cache, max-age=0' --header-upload 'Content-Type: application/json' 2>&1 | Out-Null
      if ($LASTEXITCODE -ne 0) { throw "rclone exited with code $LASTEXITCODE" }
      Write-Host ((Get-Date).ToString('HH:mm:ss') + '  uploaded  ' + $json)
    }
  } catch {
    Write-Host ((Get-Date).ToString('HH:mm:ss') + '  ERROR: ' + $_.Exception.Message) -ForegroundColor Red
  }
  if (-not $Once) { Start-Sleep -Seconds $IntervalSeconds }
} while (-not $Once)
