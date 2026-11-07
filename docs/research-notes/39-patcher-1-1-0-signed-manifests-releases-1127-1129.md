# 39 Patcher 1.1.0 (signed manifests, self-update) and releases 1.0.127-1.0.129

Date: 2026-11-07. Follows [38](38-equipment-rules-missing-release-files.md).

## Release 1.0.127: a fresh, full upload
- The bucket was emptied and rebuilt from the whole F: client (Steam original files included), minus logs, screenshots, player settings (`data/prefs.cs`, hotbars, inventory, presets), caches and machine-specific files.
- The old builder/patcher refused .exe/.dll/.bat/.js (safety rule), so 1.0.127 had no executables except the one pinned `yo_cm_client.exe`.
- Side effect to remember: a public bucket that serves the full game files is a copyright exposure. Only our own changes are meant to be public; consider a private bucket or a custom domain.

## Why the hosting was never the problem
Cloudflare R2 stores any file type. The restriction was in our own tools: `ManifestBuilder`/`PathSafety` skipped executables and the patcher only accepted a hash-pinned exe. Moving to another host would not have changed that.

## Patcher 1.1.0 and the builder (source: `tools/patcher`)
- **Signed manifests.** `GmcManifest build --sign-key` signs `manifest.json` with ECDSA P-256 (`manifest.json.sig`, base64, raw r||s). The patcher embeds the public key and refuses an unsigned or wrongly signed manifest. Only a verified manifest may contain executables, so a swapped file on the web host cannot run on a player's PC.
- **Still never touched:** `steam*.dll`, `logs/`, player settings. A signed manifest older than the last applied one is refused (`issued` + `gmc-hashcache.json.issued`).
- **Self-update.** The manifest has a `patcher` block (version, sha256, size, url). At start the patcher checks its own version first, downloads the newer exe, renames the running exe to `.old`, moves the new one in, restarts with the same Steam arguments (loop guard env `GMC_SELFUPDATED`, rollback if the new exe cannot start), then the normal file update runs.
- **Key handling.** The private key lives outside every repo and upload folder and must be backed up off the PC. If it is lost, no new executables can be shipped; if it leaks, anyone can ship code to players (then a new key pair and a new patcher build are needed).
- Tests: 93 pass (78 old + signature, tampering, wrong key, replay, patcher download, builder rules).
- Not yet exercised on a player's machine: the live swap-and-restart. Players on 1.0.x install the 1.1.0 zip once; later versions update themselves.

## Releases
- **1.0.128**: first signed release, full F: client incl. 32 executables/scripts, patcher 1.1.0 listed, `download/GMC-Patcher.zip` = 1.1.0.
- **1.0.129**: 21 reworked hair textures (12 female, 9 male DIFFUSE) from the supplied zips; 87 other files in the zips were already identical.

## Traps found
- A freshly written .ps1 or a freshly built exe can be locked by the virus scanner for ~1-2 minutes ("Access denied"); wait or copy to another folder and retry.
- `edit`-style quoting of multi-line Python through `python -c` in PowerShell breaks; write a script file instead.
