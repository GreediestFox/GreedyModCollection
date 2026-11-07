namespace Gmc.Core;

/// <summary>Builds a manifest from a modpack folder (used by the generator CLI).</summary>
public static class ManifestBuilder
{
    /// <param name="stageDir">
    /// Content-addressed mode: every file is copied to <c>stageDir/&lt;sha256&gt;</c> and its URL is <c>baseUrl + sha256</c>.
    /// Uploaded objects then never change, so CDN caches can never serve a stale file, and unchanged files are not re-uploaded.
    /// </param>
    /// <param name="pinnedExecutables">Executables that may be listed (name in the folder root -> required SHA-256); same rule as the patcher applies on the player's side.</param>
    public static Manifest Build(string sourceDir, string baseUrl, string version, IEnumerable<string> keepLocal, Action<string>? warn = null, string? stageDir = null,
        IReadOnlyDictionary<string, string>? pinnedExecutables = null, bool allowExecutables = false)
    {
        if (stageDir is not null) Directory.CreateDirectory(stageDir);
        string root = Path.GetFullPath(sourceDir);
        if (!baseUrl.EndsWith('/')) baseUrl += "/";
        var m = new Manifest { Version = version, KeepLocal = keepLocal.ToList() };
        foreach (string full in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
        {
            string rel = Path.GetRelativePath(root, full).Replace('\\', '/');
            if (rel.Equals("manifest.json", StringComparison.OrdinalIgnoreCase)) continue;
            string sha = "";
            bool pinned = allowExecutables;      // only used for signed manifests (the patcher accepts executables only then)
            if (!allowExecutables && PathSafety.IsForbidden(rel) && pinnedExecutables is not null && pinnedExecutables.TryGetValue(rel, out string? pin))
            {
                sha = Hashing.Sha256File(full);
                if (!Hashing.SameHash(sha, pin)) { warn?.Invoke($"skipped {rel}: hash differs from the pinned one"); continue; }
                pinned = true;
            }
            if (!PathSafety.TryResolve(root, rel, out _, out string err, allowForbiddenExtension: pinned)) { warn?.Invoke($"skipped {rel}: {err}"); continue; }
            if (sha.Length == 0) sha = Hashing.Sha256File(full);
            if (stageDir is not null)
            {
                string staged = Path.Combine(stageDir, sha);
                if (!File.Exists(staged)) File.Copy(full, staged);
            }
            m.Files.Add(new ManifestFile
            {
                Path = rel,
                Sha256 = sha,
                Size = new FileInfo(full).Length,
                Url = baseUrl + (stageDir is not null ? sha : EncodePath(rel)),
            });
        }
        return m;
    }

    /// <summary>URL-encodes each path segment, keeping "/" separators.</summary>
    public static string EncodePath(string rel) => string.Join('/', rel.Split('/').Select(Uri.EscapeDataString));

    /// <summary>
    /// Files that were in <paramref name="previous"/> (or in its delete list) but are no longer in <paramref name="current"/>.
    /// Older deletions are carried forward so players who skipped a version still get them; paths that came back are dropped.
    /// </summary>
    public static List<string> ComputeDeleted(Manifest? previous, Manifest current, IEnumerable<string>? extra = null)
    {
        var now = new HashSet<string>(current.Files.Select(f => PathSafety.Normalize(f.Path)), StringComparer.OrdinalIgnoreCase);
        var result = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        if (previous is not null)
        {
            foreach (var f in previous.Files) result.Add(PathSafety.Normalize(f.Path));
            foreach (var d in previous.Delete) result.Add(PathSafety.Normalize(d));
        }
        if (extra is not null) foreach (var d in extra) if (!string.IsNullOrWhiteSpace(d)) result.Add(PathSafety.Normalize(d.Trim()));
        result.RemoveWhere(now.Contains);
        return result.ToList();
    }
}
