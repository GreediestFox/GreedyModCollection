# Daniel's client mods (ported 2026-10-04)

- `preloadZoneTextures.cs`, `cm_lampEffectsDayNightLOD.cs`: new client scripts, copy to `scripts/client/`.
- `*.patch`: changes to vanilla client files (apply with `git apply` / `patch -p1` in the client folder): init.cs execs both scripts; craftWindow.gui + craft.cs show the required minimum quality per ingredient; tooltipManager.cs adds the food shelf-life row.
- After changing a vanilla `.cs`/`.gui`, rename its `.dso` sibling, or the client keeps loading the old compiled file.
