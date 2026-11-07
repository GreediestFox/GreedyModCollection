using System.Security.Cryptography;

namespace Gmc.Core;

public static class Hashing
{
    /// <summary>Lower-case hex SHA-256 of a file.</summary>
    public static string Sha256File(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16, FileOptions.SequentialScan);
        return Sha256Stream(fs);
    }

    public static string Sha256Stream(Stream s) => Convert.ToHexString(SHA256.HashData(s)).ToLowerInvariant();

    public static string Sha256Bytes(ReadOnlySpan<byte> data) => Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();

    public static bool SameHash(string? a, string? b) =>
        a is not null && b is not null && string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
}
