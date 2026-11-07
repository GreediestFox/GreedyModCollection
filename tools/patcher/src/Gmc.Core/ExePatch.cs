using System.Text;

namespace Gmc.Core;

/// <summary>
/// Binary delta that turns one exact game exe (stock SHA-256) into one exact patched exe (target SHA-256).
/// The delta is compiled into the patcher; it is never downloaded.
/// Format (little endian): "GMCX1" | name (u16 len + utf8) | stock sha (32) | target sha (32) | stock len (i64) | target len (i64)
///                          | count (i32) | count x [offset (i64) | len (i32) | bytes]
/// </summary>
public sealed class ExePatch
{
    static readonly byte[] Magic = "GMCX1"u8.ToArray();

    public string FileName { get; init; } = "";
    public string StockSha256 { get; init; } = "";
    public string TargetSha256 { get; init; } = "";
    public long StockLength { get; init; }
    public long TargetLength { get; init; }
    public List<(long Offset, byte[] Data)> Chunks { get; init; } = new();

    /// <summary>Creates the delta. Differences closer than <paramref name="mergeGap"/> bytes are joined into one chunk.</summary>
    public static ExePatch Create(string fileName, byte[] stock, byte[] target, int mergeGap = 16)
    {
        var chunks = new List<(long, byte[])>();
        int common = Math.Min(stock.Length, target.Length);
        int i = 0;
        while (i < common)
        {
            if (stock[i] == target[i]) { i++; continue; }
            int start = i, end = i, gap = 0;
            while (i < common && gap <= mergeGap)
            {
                if (stock[i] != target[i]) { end = i; gap = 0; } else gap++;
                i++;
            }
            chunks.Add((start, target[start..(end + 1)]));
        }
        if (target.Length > stock.Length) chunks.Add((stock.Length, target[stock.Length..]));
        return new ExePatch
        {
            FileName = fileName,
            StockSha256 = Hashing.Sha256Bytes(stock),
            TargetSha256 = Hashing.Sha256Bytes(target),
            StockLength = stock.Length,
            TargetLength = target.Length,
            Chunks = chunks,
        };
    }

    /// <summary>Applies the delta to the stock bytes; throws if the input or the result does not match the recorded hashes.</summary>
    public byte[] Apply(byte[] stock)
    {
        if (stock.LongLength != StockLength || !Hashing.SameHash(Hashing.Sha256Bytes(stock), StockSha256))
            throw new InvalidDataException("input is not the expected stock file");
        var result = new byte[TargetLength];
        Array.Copy(stock, result, Math.Min(stock.LongLength, TargetLength));
        foreach (var (off, data) in Chunks)
        {
            if (off < 0 || off + data.LongLength > TargetLength) throw new InvalidDataException("chunk out of range");
            Array.Copy(data, 0, result, off, data.Length);
        }
        if (!Hashing.SameHash(Hashing.Sha256Bytes(result), TargetSha256)) throw new InvalidDataException("patched result has the wrong hash");
        return result;
    }

    public byte[] Serialize()
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms, Encoding.UTF8);
        w.Write(Magic);
        byte[] name = Encoding.UTF8.GetBytes(FileName);
        w.Write((ushort)name.Length); w.Write(name);
        w.Write(Convert.FromHexString(StockSha256)); w.Write(Convert.FromHexString(TargetSha256));
        w.Write(StockLength); w.Write(TargetLength);
        w.Write(Chunks.Count);
        foreach (var (off, data) in Chunks) { w.Write(off); w.Write(data.Length); w.Write(data); }
        w.Flush();
        return ms.ToArray();
    }

    public static ExePatch Deserialize(Stream s)
    {
        using var r = new BinaryReader(s, Encoding.UTF8, leaveOpen: true);
        if (!r.ReadBytes(Magic.Length).AsSpan().SequenceEqual(Magic)) throw new InvalidDataException("not a GMC exe patch");
        string name = Encoding.UTF8.GetString(r.ReadBytes(r.ReadUInt16()));
        string stock = Convert.ToHexString(r.ReadBytes(32)).ToLowerInvariant();
        string target = Convert.ToHexString(r.ReadBytes(32)).ToLowerInvariant();
        long sl = r.ReadInt64(), tl = r.ReadInt64();
        int n = r.ReadInt32();
        var chunks = new List<(long, byte[])>(n);
        for (int k = 0; k < n; k++) { long off = r.ReadInt64(); int len = r.ReadInt32(); chunks.Add((off, r.ReadBytes(len))); }
        return new ExePatch { FileName = name, StockSha256 = stock, TargetSha256 = target, StockLength = sl, TargetLength = tl, Chunks = chunks };
    }
}
