# GMC Patcher

Modpack updater for **Life is Feudal: Your Own** (GreedyModCollection). It runs before the game via the Steam launch option:
1. checks for updates, downloads only new or changed files and removes old ones;
2. applies the exe patch that is compiled into it;
3. starts the game with exactly the command line Steam handed over.

If anything goes wrong (no internet, server down, bad file), the game starts anyway.

It does not install itself anywhere: no registry, autostart, services, scheduled tasks or admin rights.

## Contents

| | |
|---|---|
| `GmcPatcher.exe` | the patcher (one self-contained .NET 8 exe, no installation) |
| `gmc-patcher.json` | its configuration, next to the exe |
| `GmcManifest.exe` | for the modpack maintainer: builds `manifest.json` and the exe patch |
| `src\`, `tests\` | source code and unit tests (`dotnet test`) |

## Setup for players

1. Put `GmcPatcher.exe` and `gmc-patcher.json` in a folder of their own, e.g. `C:\Games\GMC\`. Not the game folder: the exe should never be overwritten by game updates.
2. Steam → Library → Life is Feudal: Your Own → Properties → General → **Launch options**:
   ```
   "C:\Games\GMC\GmcPatcher.exe" %command%
   ```
3. Start the game normally via Steam. A console window shows progress (file, X/Y, MB). The first start downloads the whole modpack (about 1.4 GB for the client), later starts only the changes.

`patcher.log` next to the exe records everything.

### gmc-patcher.json

```json
{
  "manifestUrl": "https://gmc.example.org/manifest.json",
  "gameRoot": "",
  "requiredFile": "yo_cm_client.exe",
  "keepLocal": [],
  "timeoutSeconds": 30,
  "applyExePatch": true,
  "waitForGame": false
}
```

| Key | Meaning |
|---|---|
| `manifestUrl` | Address of `manifest.json` (https only). |
| `gameRoot` | Game folder. Empty means the folder of the exe Steam starts (`%command%`). Only set it when using the patcher without Steam. |
| `requiredFile` | Safety check: nothing is written unless this file is in the game folder. |
| `keepLocal` | Additional own files that are never overwritten, e.g. `"data/prefs.cs"`. |
| `timeoutSeconds` | Connection timeout, and the time a download may stall without data. |
| `applyExePatch` | Apply the built-in exe patch (needed for GMC). |
| `waitForGame` | `true` keeps the patcher running until the game closes, so Steam shows "Playing" the whole time and counts playtime correctly. `false` (the default) exits right after the start. |

### Manual use

- `GmcPatcher.exe` without arguments: only updates (needs `gameRoot`).
- `GmcPatcher.exe --restore-exe`: puts the original game exe back (from `yo_cm_client.exe.gmc_stock`).

## How the update works

- `manifest.json` lists every file with path, SHA-256, size and URL, plus a `delete` list and optionally `keepLocal`.
- Local files are hashed. The hashes are cached in `gmc-hashcache.json` by size and modification time, so later starts are fast.
- Downloads go to `<file>.gmc-download` in the target folder. The size and SHA-256 are checked, then the file is moved into place in one step.
- On a mismatch it retries up to 3 times. After that the update is aborted and the old file stays.
- **Safety rules:**
  - Paths with `..`, absolute paths, drive letters, UNC paths, `:` (alternate data streams) and device names (`CON`, `NUL`, ...) are rejected. So is anything outside the game folder.
  - The patcher never downloads, writes or deletes executables or scripts: `.exe .dll .bat .cmd .ps1 .vbs .js .lnk .msi .scr ...`.
  - Player data and settings are never touched either (1.0.6): `data/prefs.cs`, `data/hotbars/`, `data/inventory/`, `data/presets/`, `screenshots/` - rejected as manifest files and as deletions.
  - Anything under the game's `logs/` folder and any `steam*.dll` (Steam's own libraries) is rejected too, whatever the manifest says (1.0.4).
  - Exception (1.0.5): the manifest may ship the patched game exe `yo_cm_client.exe` (game folder root only), and only if its SHA-256 equals the one compiled into the patcher's exe patch. Any other executable, or the same name with another hash, is still rejected; executables are never deleted. The existing exe is kept as `yo_cm_client.exe.gmc_stock` first.
  - It never updates itself.
- **keepLocal** (glob, case-insensitive; `*`, `?`, `**`, and a trailing `/` for whole folders): installed when missing, then never overwritten or deleted. GMC uses this for `mod/sf-mods/configs/`.

### The exe patch

GMC needs byte patches in `yo_cm_client.exe`. They are **compiled into the patcher** and never downloaded:
- The patch only applies if the exe is exactly the Steam version it was made for (SHA-256). The result is checked by SHA-256 too.
- The original stays next to it as `yo_cm_client.exe.gmc_stock`.
- After a Steam update the exe no longer matches. The patcher then logs a warning and starts the game unpatched, and a new patcher with a new patch is needed.

## Publishing an update (maintainer)

The files come from the GMC pack (client part).

1. Collect the modpack folder: everything that should end up in the game folder, with the same structure (`mod\…`, `data\…`, `art\…`).
2. Build the manifest, content-addressed, which is recommended for cloud storage:
   ```
   GmcManifest.exe build --source <modpack folder> --base-url https://gmc.example.org/o/ --version 1.2.0 ^
       --previous manifest.json --delete-list deletes.txt --keep-local mod/sf-mods/configs/ ^
       --stage-dir upload\o --out upload\manifest.json
   ```
   - `--stage-dir` stores every file under its SHA-256 name. Uploaded objects never change, so a CDN can never serve a stale file, and unchanged files don't need re-uploading.
   - `--previous` adds files that dropped out since the last version to `delete`. Older deletions are carried forward.
   - `--delete-list` adds extra paths, one per line (`#` starts a comment), e.g. stale stock `.dso` files that would otherwise hide our edited `.cs` files.
   - `GmcManifest.exe diff --old old.json --new new.json` shows `+` new, `~` changed, `-` removed.
3. Upload. Objects first, the manifest **last**, so no player sees a manifest whose files are still missing.

### Cloudflare R2 (example)

1. Create a bucket (e.g. `gmc`) and enable public access: an `r2.dev` subdomain or your own domain.
2. Upload, for example with `rclone` (configure an R2 remote once):
   ```
   rclone copy upload\o r2:gmc/o --ignore-existing
   rclone copyto upload\manifest.json r2:gmc/manifest.json --header-upload "Cache-Control: no-cache"
   ```
   - The objects under `o/` can be cached for as long as you like, because they never change.
   - `manifest.json` gets `no-cache` so players see a new version immediately.
3. `manifestUrl` in `gmc-patcher.json` = `https://<your-domain>/manifest.json`.

Any other static HTTPS host works the same way (S3, Backblaze B2, your own web server). GitHub Releases would also work, but GMC is meant to stay private, so no public repo.

### New exe patch (after a Steam update or a new client patch)

```
GmcManifest.exe exepatch --stock <unmodified yo_cm_client.exe> --patched <our patched exe> --out src\GmcPatcher\exepatch\yo_cm_client.gmcx
dotnet publish src\GmcPatcher\GmcPatcher.csproj -c Release -o out\patcher
```

The `.gmcx` is embedded into the exe at build time. Since 1.1.0 a new patcher is shipped through the signed manifest (see below); only players on 1.0.x must install 1.1.0 once by hand.

## Building

.NET 8 SDK or newer:
```
dotnet test tests\Gmc.Core.Tests
dotnet publish src\GmcPatcher\GmcPatcher.csproj -c Release -o out\patcher
dotnet publish src\GmcManifest\GmcManifest.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o out\manifest
```

The result is a single exe of about 15 MB (self-contained, trimmed; no packer, no obfuscation). Dependencies: only the .NET standard library; xUnit for the tests only.

## Code signing

A signed exe triggers fewer SmartScreen/antivirus warnings, and players can see who it comes from.

1. Get a code-signing certificate:
   - a standard OV certificate from a CA (Sectigo, DigiCert, SSL.com, …; usually on a USB token or in a cloud HSM), or
   - Azure Trusted Signing (cheap, but needs a verified organisation/person).
   - EV certificates get SmartScreen reputation immediately; OV builds it up over time.
2. Sign with `signtool` (Windows SDK) and a timestamp, so the signature stays valid after the certificate expires:
   ```
   signtool sign /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 /a out\patcher\GmcPatcher.exe
   signtool verify /pa /v out\patcher\GmcPatcher.exe
   ```
   `/a` picks the matching certificate automatically. With a token/HSM use the vendor's instructions instead, e.g. `/sha1 <thumbprint>` or the Trusted Signing `dlib`.
3. Always sign after `dotnet publish` and before distributing. Any later change to the exe breaks the signature.

An unsigned exe works too: Windows then shows "Unknown publisher" (SmartScreen: "More info → Run anyway").


## Signed manifests, executables and self-update (since 1.1.0)

- `GmcManifest genkey --private-out <secret> --public-out <public>` makes an ECDSA P-256 key pair. The **private key never goes into a repo or an upload folder**; the public key is embedded into the patcher (`src/GmcPatcher/key/gmc_sign_public.txt`).
- `GmcManifest build ... --sign-key <private key>` writes `manifest.json.sig` next to `manifest.json` and then also lists .exe/.dll/.bat/.js files. Upload the signature together with the manifest.
- The patcher verifies the signature before it touches anything. Invalid or missing signature = update refused (the game still starts). Only a verified manifest may contain executables. `steam*.dll` and `logs/` stay untouched whatever it says. A signed manifest older than the last applied one is refused (replay protection, field `issued`).
- `--patcher-exe <GmcPatcher.exe> [--patcher-version x.y.z]` adds a `patcher` block (version, sha256, size, url). A patcher older than that version downloads it, renames itself to `GmcPatcher.exe.old`, moves the new exe into place and restarts with the same arguments (env `GMC_SELFUPDATED` stops loops; rollback if the new exe cannot be started). Switch it off with `"selfUpdate": false` in gmc-patcher.json.
