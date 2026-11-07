namespace Gmc.Core;

/// <summary>Validates manifest paths: relative, inside the game root, no executables.</summary>
public static class PathSafety
{
    /// <summary>File types the patcher never downloads, writes or deletes.</summary>
    public static readonly IReadOnlySet<string> ForbiddenExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".dll", ".sys", ".drv", ".ocx", ".cpl", ".scr", ".com", ".pif", ".msi", ".msp",
        ".bat", ".cmd", ".ps1", ".psm1", ".vbs", ".vbe", ".js", ".jse", ".wsf", ".wsh", ".hta", ".lnk", ".reg",
    };

    static readonly HashSet<string> ReservedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
    };

    /// <summary>Normalises a manifest path to forward slashes without leading "./".</summary>
    public static string Normalize(string relPath)
    {
        string p = relPath.Replace('\\', '/');
        while (p.StartsWith("./")) p = p[2..];
        return p;
    }

    /// <summary>
    /// Checks a manifest path and resolves it below <paramref name="root"/>.
    /// Rejects absolute paths, drive letters, UNC, "..", alternate data streams, reserved device names,
    /// invalid characters and forbidden (executable) extensions.
    /// </summary>
    public static bool TryResolve(string root, string relPath, out string fullPath, out string error, bool allowForbiddenExtension = false)
    {
        fullPath = "";
        error = "";
        if (string.IsNullOrWhiteSpace(relPath)) { error = "empty path"; return false; }
        string p = relPath.Replace('\\', '/');
        if (p.StartsWith('/') || Path.IsPathRooted(relPath) || p.Contains(':')) { error = "absolute path or drive/stream"; return false; }
        string[] parts = p.Split('/');
        foreach (string part in parts)
        {
            if (part.Length == 0) { error = "empty path segment"; return false; }
            if (part == "." || part == "..") { error = "'.' or '..' segment"; return false; }
            if (part.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) { error = "invalid character"; return false; }
            if (part.EndsWith(' ') || part.EndsWith('.')) { error = "segment ends with space or dot"; return false; }
            if (ReservedNames.Contains(Path.GetFileNameWithoutExtension(part))) { error = "reserved device name"; return false; }
        }
        if (!allowForbiddenExtension && IsForbidden(p)) { error = "executable file type"; return false; }
        if (IsExcluded(p)) { error = "logs folder or Steam library (never managed)"; return false; }

        string rootFull = Path.GetFullPath(root);
        if (!rootFull.EndsWith(Path.DirectorySeparatorChar)) rootFull += Path.DirectorySeparatorChar;
        string candidate = Path.GetFullPath(Path.Combine(rootFull, p.Replace('/', Path.DirectorySeparatorChar)));
        if (!candidate.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase)) { error = "outside the game folder"; return false; }
        fullPath = candidate;
        return true;
    }

    public static bool IsForbidden(string path) => ForbiddenExtensions.Contains(Path.GetExtension(path));

    /// <summary>The game's runtime "logs" folder and Steam's own libraries (steam_api*.dll, steamclient*.dll, ...) are never touched, whatever a manifest says.</summary>
    public static bool IsExcluded(string normalizedPath)
    {
        string[] parts = normalizedPath.Replace('\\', '/').Split('/');
        if (parts.Length > 1 && parts[0].Equals("logs", StringComparison.OrdinalIgnoreCase)) return true;
        // player data and settings (written by the game): never downloaded over, never deleted, whatever a manifest says
        if (parts.Length > 1 && parts[0].Equals("screenshots", StringComparison.OrdinalIgnoreCase)) return true;
        if (parts.Length == 2 && parts[0].Equals("data", StringComparison.OrdinalIgnoreCase) && parts[1].Equals("prefs.cs", StringComparison.OrdinalIgnoreCase)) return true;
        if (parts.Length > 2 && parts[0].Equals("data", StringComparison.OrdinalIgnoreCase)
            && (parts[1].Equals("hotbars", StringComparison.OrdinalIgnoreCase) || parts[1].Equals("inventory", StringComparison.OrdinalIgnoreCase)
                || parts[1].Equals("presets", StringComparison.OrdinalIgnoreCase))) return true;
        string name = parts[^1];
        return name.StartsWith("steam", StringComparison.OrdinalIgnoreCase) && name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
    }
}
