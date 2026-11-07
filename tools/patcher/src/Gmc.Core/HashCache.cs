using System.Text.Json;

namespace Gmc.Core;

/// <summary>Caches file hashes by (size, last write time) so unchanged files are not re-hashed on every launch.</summary>
public sealed class HashCache
{
    readonly string? _file;
    readonly Dictionary<string, HashCacheEntry> _map;
    bool _dirty;

    public HashCache(string? cacheFile)
    {
        _file = cacheFile;
        _map = new(StringComparer.OrdinalIgnoreCase);
        if (cacheFile is null || !File.Exists(cacheFile)) return;
        try
        {
            var loaded = JsonSerializer.Deserialize(File.ReadAllText(cacheFile), GmcJson.Default.DictionaryStringHashCacheEntry);
            if (loaded is not null) foreach (var kv in loaded) _map[kv.Key] = kv.Value;
        }
        catch (Exception) { /* a broken cache is simply rebuilt */ }
    }

    public int HashedCount { get; private set; }

    /// <summary>SHA-256 of the file, or null if it does not exist.</summary>
    public string? Get(string fullPath)
    {
        var fi = new FileInfo(fullPath);
        if (!fi.Exists) { if (_map.Remove(fullPath)) _dirty = true; return null; }
        long ticks = fi.LastWriteTimeUtc.Ticks;
        if (_map.TryGetValue(fullPath, out var e) && e.Size == fi.Length && e.MtimeTicks == ticks) return e.Sha256;
        string sha = Hashing.Sha256File(fullPath);
        HashedCount++;
        Set(fullPath, fi.Length, ticks, sha);
        return sha;
    }

    /// <summary>Records the hash of a file we just wrote (after verifying it).</summary>
    public void Remember(string fullPath, string sha)
    {
        var fi = new FileInfo(fullPath);
        if (fi.Exists) Set(fullPath, fi.Length, fi.LastWriteTimeUtc.Ticks, sha);
    }

    public void Forget(string fullPath) { if (_map.Remove(fullPath)) _dirty = true; }

    void Set(string fullPath, long size, long ticks, string sha)
    {
        _map[fullPath] = new HashCacheEntry { Size = size, MtimeTicks = ticks, Sha256 = sha };
        _dirty = true;
    }

    public void Save()
    {
        if (_file is null || !_dirty) return;
        string tmp = _file + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(_map, GmcJson.Default.DictionaryStringHashCacheEntry));
        File.Move(tmp, _file, overwrite: true);
        _dirty = false;
    }
}
