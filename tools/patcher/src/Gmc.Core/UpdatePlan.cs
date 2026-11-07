namespace Gmc.Core;

public sealed record PlannedDownload(ManifestFile File, string FullPath, Uri Url);

public sealed class UpdatePlan
{
    public List<PlannedDownload> Downloads { get; } = new();
    public List<string> Deletes { get; } = new();           // full paths
    public List<string> KeptLocal { get; } = new();         // rel paths left alone (keepLocal)
    public List<string> Rejected { get; } = new();          // "path: reason"
    public int UpToDate { get; set; }
    public long DownloadBytes => Downloads.Sum(d => d.File.Size);

    /// <summary>
    /// Compares a manifest with the local game folder.
    /// <paramref name="localHash"/> returns the SHA-256 of a full path, or null if the file does not exist.
    /// </summary>
    /// <param name="pinnedExecutables">
    /// The only executables a manifest may ship: file name in the game folder root -> the one SHA-256 allowed for it
    /// (compiled into the patcher, never taken from the manifest). Anything else with an executable extension stays rejected.
    /// </param>
    public static UpdatePlan Create(Manifest manifest, Uri manifestUrl, string root, KeepLocal keep, Func<string, string?> localHash,
        IReadOnlyDictionary<string, string>? pinnedExecutables = null, bool trustedSigned = false)
    {
        var plan = new UpdatePlan();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in manifest.Files)
        {
            // trustedSigned: the manifest signature was verified with the patcher's built-in public key, so executables are allowed.
            bool pinned = trustedSigned;
            if (!trustedSigned && PathSafety.IsForbidden(f.Path) && pinnedExecutables is not null
                && pinnedExecutables.TryGetValue(PathSafety.Normalize(f.Path), out string? pin))
            {
                if (!Hashing.SameHash(f.Sha256, pin)) { plan.Rejected.Add($"{f.Path}: executable does not match the hash pinned in the patcher"); continue; }
                pinned = true;
            }
            if (!PathSafety.TryResolve(root, f.Path, out string full, out string err, allowForbiddenExtension: pinned)) { plan.Rejected.Add($"{f.Path}: {err}"); continue; }
            if (!seen.Add(full)) { plan.Rejected.Add($"{f.Path}: listed twice"); continue; }
            if (f.Sha256.Length != 64 || f.Size < 0) { plan.Rejected.Add($"{f.Path}: bad sha256/size"); continue; }
            if (!TryFileUrl(manifestUrl, f.Url, out Uri? url)) { plan.Rejected.Add($"{f.Path}: url must be https"); continue; }

            string? cur = localHash(full);
            if (Hashing.SameHash(cur, f.Sha256)) { plan.UpToDate++; continue; }
            if (cur is not null && keep.Matches(f.Path)) { plan.KeptLocal.Add(f.Path); continue; }
            plan.Downloads.Add(new PlannedDownload(f, full, url!));
        }
        foreach (string d in manifest.Delete)
        {
            if (!PathSafety.TryResolve(root, d, out string full, out string err, allowForbiddenExtension: trustedSigned)) { plan.Rejected.Add($"delete {d}: {err}"); continue; }
            if (seen.Contains(full)) { plan.Rejected.Add($"delete {d}: also listed as a file"); continue; }
            if (keep.Matches(d)) { plan.KeptLocal.Add(d); continue; }
            if (localHash(full) is not null) plan.Deletes.Add(full);
        }
        return plan;
    }

    /// <summary>File URLs may be absolute or relative to the manifest URL; only https is accepted.</summary>
    public static bool TryFileUrl(Uri manifestUrl, string url, out Uri? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (!Uri.TryCreate(manifestUrl, url, out var u)) return false;
        if (u.Scheme != Uri.UriSchemeHttps) return false;
        result = u;
        return true;
    }
}
