namespace Gmc.Core;

/// <summary>
/// Checks that the copyright / asset-credits file shipped with the modpack is still in the game folder.
/// Warning only: the caller never blocks or changes anything because of the result.
/// </summary>
public static class CreditsCheck
{
    public const string FileName = "copyright.txt";
    /// <summary>"权" (U+6743), part of the heading of copyright.txt. A simple tamper marker, not security.</summary>
    public const string Marker = "权";
    const long MaxSize = 1 << 20;   // 1 MiB

    /// <summary>True when <paramref name="root"/> holds a non-empty copyright.txt (UTF-8) that contains the marker.</summary>
    public static bool IsValid(string root, string fileName = FileName, string marker = Marker)
    {
        try
        {
            var fi = new FileInfo(Path.Combine(root, fileName));
            if (!fi.Exists || fi.Length == 0 || fi.Length > MaxSize) return false;
            return marker.Length == 0 || File.ReadAllText(fi.FullName).Contains(marker, StringComparison.Ordinal);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return false; }
    }
}
