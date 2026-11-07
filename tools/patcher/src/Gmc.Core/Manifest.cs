using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gmc.Core;

public sealed class Manifest
{
    [JsonPropertyName("version")] public string Version { get; set; } = "0.0.0";
    [JsonPropertyName("files")] public List<ManifestFile> Files { get; set; } = new();
    [JsonPropertyName("delete")] public List<string> Delete { get; set; } = new();
    [JsonPropertyName("keepLocal")] public List<string> KeepLocal { get; set; } = new();
    /// <summary>UTC time the manifest was built (ISO 8601). A signed manifest older than the last applied one is refused (replay protection).</summary>
    [JsonPropertyName("issued")] public string Issued { get; set; } = "";
    /// <summary>The newest patcher exe. Optional; the patcher replaces itself with it before updating the game files.</summary>
    [JsonPropertyName("patcher")] public PatcherInfo? Patcher { get; set; }

    public static Manifest Parse(string json) =>
        JsonSerializer.Deserialize(json, GmcJson.Default.Manifest) ?? throw new InvalidDataException("manifest is empty");

    public string ToJson() => JsonSerializer.Serialize(this, GmcJson.Default.Manifest);
}

public sealed class ManifestFile
{
    [JsonPropertyName("path")] public string Path { get; set; } = "";
    [JsonPropertyName("sha256")] public string Sha256 { get; set; } = "";
    [JsonPropertyName("size")] public long Size { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; } = "";
}

public sealed class PatcherInfo
{
    [JsonPropertyName("version")] public string Version { get; set; } = "";
    [JsonPropertyName("sha256")] public string Sha256 { get; set; } = "";
    [JsonPropertyName("size")] public long Size { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; } = "";
}

/// <summary>Settings file next to the patcher exe (gmc-patcher.json).</summary>
public sealed class PatcherConfig
{
    /// <summary>HTTPS URL of manifest.json.</summary>
    [JsonPropertyName("manifestUrl")] public string ManifestUrl { get; set; } = "";
    /// <summary>Game folder. Empty = the folder of the game exe that Steam passes via %command%.</summary>
    [JsonPropertyName("gameRoot")] public string GameRoot { get; set; } = "";
    /// <summary>Extra keepLocal patterns (added to the manifest's list).</summary>
    [JsonPropertyName("keepLocal")] public List<string> KeepLocal { get; set; } = new();
    /// <summary>Safety check: the game folder must contain this file, otherwise nothing is written.</summary>
    [JsonPropertyName("requiredFile")] public string RequiredFile { get; set; } = "yo_cm_client.exe";
    /// <summary>Number of files downloaded at the same time (1-32).</summary>
    [JsonPropertyName("parallelDownloads")] public int ParallelDownloads { get; set; } = 8;
    [JsonPropertyName("timeoutSeconds")] public int TimeoutSeconds { get; set; } = 30;
    /// <summary>Apply the exe patch that is built into the patcher.</summary>
    [JsonPropertyName("applyExePatch")] public bool ApplyExePatch { get; set; } = true;
    /// <summary>Keep the patcher running until the game exits (Steam then shows "playing" the whole time).</summary>
    [JsonPropertyName("waitForGame")] public bool WaitForGame { get; set; } = false;
    /// <summary>Let the patcher replace itself with the version listed in the manifest (default on).</summary>
    [JsonPropertyName("selfUpdate")] public bool SelfUpdate { get; set; } = true;
}

public sealed class HashCacheEntry
{
    [JsonPropertyName("size")] public long Size { get; set; }
    [JsonPropertyName("mtime")] public long MtimeTicks { get; set; }
    [JsonPropertyName("sha256")] public string Sha256 { get; set; } = "";
}

[JsonSourceGenerationOptions(WriteIndented = true, ReadCommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true)]
[JsonSerializable(typeof(Manifest))]
[JsonSerializable(typeof(PatcherConfig))]
[JsonSerializable(typeof(Dictionary<string, HashCacheEntry>))]
public partial class GmcJson : JsonSerializerContext
{
}
