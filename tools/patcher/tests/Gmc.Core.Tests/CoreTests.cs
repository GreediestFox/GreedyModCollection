using System.Text;
using Gmc.Core;
using Xunit;

namespace Gmc.Core.Tests;

public sealed class TempDir : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "gmc-test-" + Guid.NewGuid().ToString("N"));
    public TempDir() => Directory.CreateDirectory(Path);
    public string Write(string rel, string content)
    {
        string full = System.IO.Path.Combine(Path, rel);
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(full)!);
        File.WriteAllText(full, content);
        return full;
    }
    public void Dispose() { try { Directory.Delete(Path, true); } catch (IOException) { } }
}

public class HashingTests
{
    [Fact]
    public void Sha256_of_abc_matches_known_vector() =>
        Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", Hashing.Sha256Bytes("abc"u8));

    [Fact]
    public void File_and_bytes_hash_agree()
    {
        using var t = new TempDir();
        string f = t.Write("a.txt", "hello world");
        Assert.Equal(Hashing.Sha256Bytes(Encoding.UTF8.GetBytes("hello world")), Hashing.Sha256File(f));
    }

    [Fact]
    public void SameHash_is_case_insensitive_and_null_safe()
    {
        Assert.True(Hashing.SameHash("ABCDEF", "abcdef"));
        Assert.False(Hashing.SameHash(null, "abc"));
        Assert.False(Hashing.SameHash("abc", null));
    }

    [Fact]
    public void HashCache_reuses_hash_until_file_changes()
    {
        using var t = new TempDir();
        string f = t.Write("data/x.xml", "one");
        var cache = new HashCache(System.IO.Path.Combine(t.Path, "cache.json"));
        string h1 = cache.Get(f)!;
        Assert.Equal(1, cache.HashedCount);
        Assert.Equal(h1, cache.Get(f));
        Assert.Equal(1, cache.HashedCount);              // served from cache
        cache.Save();

        var reloaded = new HashCache(System.IO.Path.Combine(t.Path, "cache.json"));
        Assert.Equal(h1, reloaded.Get(f));
        Assert.Equal(0, reloaded.HashedCount);           // survived the save

        File.WriteAllText(f, "two, longer");
        Assert.NotEqual(h1, reloaded.Get(f));
        Assert.Equal(1, reloaded.HashedCount);
        Assert.Null(reloaded.Get(System.IO.Path.Combine(t.Path, "missing.xml")));
    }
}

public class PathSafetyTests
{
    const string Root = @"C:\Games\LiF";

    [Theory]
    [InlineData("mod/GMHelper/gm.cs")]
    [InlineData(@"data\loc\de\data\objects_types_Name.xml")]
    [InlineData("art/2D/Items/thin beer.png")]
    [InlineData("./mod/x.dts")]
    public void Accepts_normal_relative_paths(string p)
    {
        Assert.True(PathSafety.TryResolve(Root, PathSafety.Normalize(p), out string full, out string err), err);
        Assert.StartsWith(Root + @"\", full);
    }

    [Theory]
    [InlineData("../evil.cs")]
    [InlineData("mod/../../evil.cs")]
    [InlineData(@"..\evil.cs")]
    [InlineData("/etc/passwd")]
    [InlineData(@"\Windows\x.cs")]
    [InlineData(@"C:\Windows\x.cs")]
    [InlineData("C:x.cs")]
    [InlineData(@"\\server\share\x.cs")]
    [InlineData("data/file.xml:stream")]
    [InlineData("mod//x.cs")]
    [InlineData("mod/./x.cs")]
    [InlineData("CON.txt")]
    [InlineData("mod/nul")]
    [InlineData("mod/x.cs ")]
    [InlineData("")]
    public void Rejects_paths_outside_or_weird(string p) =>
        Assert.False(PathSafety.TryResolve(Root, p, out _, out _));

    [Theory]
    [InlineData("yo_cm_client.exe")]
    [InlineData("server/4ba5cb5e.dll")]
    [InlineData("mod/run.BAT")]
    [InlineData("mod/x.ps1")]
    [InlineData("mod/x.lnk")]
    public void Rejects_executables(string p) =>
        Assert.False(PathSafety.TryResolve(Root, p, out _, out string err) && err.Length == 0);

    [Theory]
    [InlineData("logs/2026-10-07/C_x.log")]
    [InlineData("LOGS/x.txt")]
    [InlineData("steam_api64.dll")]
    [InlineData("steamclient64.dll")]
    [InlineData("bin/steamwebrtc.dll")]
    [InlineData("data/prefs.cs")]
    [InlineData("DATA/Prefs.cs")]
    [InlineData("data/hotbars/abc_Name")]
    [InlineData("data/inventory/abc_Name")]
    [InlineData("data/presets/hud/first/hotbar.obj")]
    [InlineData("screenshots/a.png")]
    public void Rejects_logs_folder_and_steam_libraries(string p) =>
        Assert.False(PathSafety.TryResolve(Root, p, out _, out _));

    [Fact]
    public void Folders_that_only_contain_the_words_are_fine()
    {
        Assert.False(PathSafety.IsExcluded("mod/logs/readme.txt"));
        Assert.False(PathSafety.IsExcluded("art/2D/steam.png"));
        Assert.False(PathSafety.IsExcluded("logs"));
    }

    [Fact]
    public void Dso_and_data_files_are_not_forbidden()
    {
        Assert.False(PathSafety.IsForbidden("mod/sf-mods/init.cs.dso"));
        Assert.False(PathSafety.IsForbidden("art/models/a.dts"));
    }
}

public class KeepLocalTests
{
    [Fact]
    public void Folder_pattern_matches_everything_below()
    {
        var k = new KeepLocal(new[] { "mod/sf-mods/configs/" });
        Assert.True(k.Matches(@"mod\sf-mods\configs\chat.xml"));
        Assert.True(k.Matches("mod/sf-mods/configs/sub/x.cs"));
        Assert.False(k.Matches("mod/sf-mods/scripts/x.cs"));
    }

    [Fact]
    public void Wildcards_and_case()
    {
        var k = new KeepLocal(new[] { "data/*.cs", "**/user.cfg" });
        Assert.True(k.Matches("DATA/Prefs.CS"));
        Assert.False(k.Matches("data/sub/prefs.cs"));
        Assert.True(k.Matches("user.cfg"));
        Assert.True(k.Matches("a/b/c/user.cfg"));
    }
}

public class ManifestTests
{
    static Manifest M(params string[] paths) => new()
    {
        Files = paths.Select(p => new ManifestFile { Path = p, Sha256 = new string('a', 64), Size = 1, Url = "https://x/" + p }).ToList(),
    };

    [Fact]
    public void Deleted_files_are_the_ones_dropped_since_previous()
    {
        var prev = M("a.cs", "b.cs", "c.cs");
        var cur = M("a.cs", "c.cs", "d.cs");
        Assert.Equal(new[] { "b.cs" }, ManifestBuilder.ComputeDeleted(prev, cur));
    }

    [Fact]
    public void Old_deletions_carry_forward_unless_file_returns()
    {
        var prev = M("a.cs");
        prev.Delete = new() { "old.cs", "back.cs" };
        var cur = M("a.cs", "back.cs");
        Assert.Equal(new[] { "old.cs" }, ManifestBuilder.ComputeDeleted(prev, cur));
    }

    [Fact]
    public void Extra_deletions_and_no_previous()
    {
        var cur = M("a.cs");
        Assert.Equal(new[] { "scripts/client/init.cs.dso" },
            ManifestBuilder.ComputeDeleted(null, cur, new[] { @"scripts\client\init.cs.dso", "a.cs", " " }));
    }

    [Fact]
    public void Json_roundtrip_uses_spec_field_names()
    {
        var m = M("mod/a.cs");
        m.Version = "1.2.0";
        m.Delete = new() { "x.cs" };
        string json = m.ToJson();
        Assert.Contains("\"version\"", json);
        Assert.Contains("\"sha256\"", json);
        var back = Manifest.Parse(json);
        Assert.Equal("1.2.0", back.Version);
        Assert.Equal("mod/a.cs", back.Files[0].Path);
        Assert.Equal(new[] { "x.cs" }, back.Delete);
    }

    [Fact]
    public void Builder_content_addressed_stages_files_by_hash()
    {
        using var t = new TempDir();
        t.Write("src/mod/a.cs", "x");
        t.Write("src/mod/copy.cs", "x");
        string stage = System.IO.Path.Combine(t.Path, "stage");
        var m = ManifestBuilder.Build(System.IO.Path.Combine(t.Path, "src"), "https://cdn.example/o/", "1", Array.Empty<string>(), null, stage);
        string sha = Hashing.Sha256Bytes("x"u8);
        Assert.All(m.Files, f => Assert.Equal("https://cdn.example/o/" + sha, f.Url));
        Assert.Single(Directory.GetFiles(stage));                    // identical content stored once
        Assert.Equal("x", File.ReadAllText(System.IO.Path.Combine(stage, sha)));
    }

    [Fact]
    public void Builder_hashes_files_encodes_urls_and_skips_executables()
    {
        using var t = new TempDir();
        t.Write("mod/a b.cs", "x");
        t.Write("evil.exe", "MZ");
        var warnings = new List<string>();
        var m = ManifestBuilder.Build(t.Path, "https://cdn.example/gmc", "1.0.0", Array.Empty<string>(), warnings.Add);
        var f = Assert.Single(m.Files);
        Assert.Equal("mod/a b.cs", f.Path);
        Assert.Equal("https://cdn.example/gmc/mod/a%20b.cs", f.Url);
        Assert.Equal(Hashing.Sha256Bytes("x"u8), f.Sha256);
        Assert.Single(warnings);
    }
}

public class UpdatePlanTests
{
    static readonly Uri ManifestUrl = new("https://cdn.example/gmc/manifest.json");

    [Fact]
    public void Plans_downloads_deletes_and_respects_keepLocal()
    {
        using var t = new TempDir();
        string same = t.Write("mod/same.cs", "same");
        t.Write("mod/changed.cs", "old");
        t.Write("mod/sf-mods/configs/c.xml", "mine");
        t.Write("scripts/stale.cs.dso", "x");
        var m = new Manifest
        {
            Files =
            {
                new() { Path = "mod/same.cs", Sha256 = Hashing.Sha256File(same), Size = 4, Url = "files/mod/same.cs" },
                new() { Path = "mod/changed.cs", Sha256 = Hashing.Sha256Bytes("new"u8), Size = 3, Url = "files/mod/changed.cs" },
                new() { Path = "mod/new.cs", Sha256 = Hashing.Sha256Bytes("n"u8), Size = 1, Url = "https://other.example/new.cs" },
                new() { Path = "mod/sf-mods/configs/c.xml", Sha256 = Hashing.Sha256Bytes("theirs"u8), Size = 6, Url = "files/c.xml" },
                new() { Path = "../escape.cs", Sha256 = new string('a', 64), Size = 1, Url = "files/e.cs" },
                new() { Path = "mod/insecure.cs", Sha256 = new string('a', 64), Size = 1, Url = "http://plain.example/x" },
            },
            Delete = { "scripts/stale.cs.dso", "scripts/not-there.dso", "yo_cm_client.exe" },
            KeepLocal = { "mod/sf-mods/configs/" },
        };
        var cache = new HashCache(null);
        var plan = UpdatePlan.Create(m, ManifestUrl, t.Path, new KeepLocal(m.KeepLocal), cache.Get);

        Assert.Equal(1, plan.UpToDate);
        Assert.Equal(new[] { "mod/changed.cs", "mod/new.cs" }, plan.Downloads.Select(d => d.File.Path));
        Assert.Equal("https://cdn.example/gmc/files/mod/changed.cs", plan.Downloads[0].Url.ToString());
        Assert.Equal(new[] { "mod/sf-mods/configs/c.xml" }, plan.KeptLocal);
        Assert.Single(plan.Deletes);
        Assert.EndsWith("stale.cs.dso", plan.Deletes[0]);
        Assert.Equal(3, plan.Rejected.Count);    // ../escape, http url, delete of an exe
    }

    [Fact]
    public void KeepLocal_file_is_installed_when_missing()
    {
        using var t = new TempDir();
        var m = new Manifest { Files = { new() { Path = "cfg/user.xml", Sha256 = new string('b', 64), Size = 1, Url = "f/user.xml" } } };
        var plan = UpdatePlan.Create(m, ManifestUrl, t.Path, new KeepLocal(new[] { "cfg/" }), new HashCache(null).Get);
        Assert.Single(plan.Downloads);
    }
}

public class ExePatchTests
{
    [Fact]
    public void Roundtrip_with_growth_and_serialization()
    {
        var rnd = new Random(1);
        byte[] stock = new byte[100_000]; rnd.NextBytes(stock);
        byte[] target = (byte[])stock.Clone();
        target[10] ^= 0xFF; target[12] ^= 0xFF; target[50_000] = 7;
        Array.Resize(ref target, 101_000);
        for (int i = 100_000; i < target.Length; i++) target[i] = (byte)i;

        var p = ExePatch.Create("game.exe", stock, target);
        Assert.Equal(3, p.Chunks.Count);          // 10..12 merged, 50000, tail
        var back = ExePatch.Deserialize(new MemoryStream(p.Serialize()));
        Assert.Equal(target, back.Apply(stock));
    }

    [Fact]
    public void Refuses_wrong_input()
    {
        byte[] stock = { 1, 2, 3, 4 }, target = { 1, 9, 3, 4 };
        var p = ExePatch.Create("game.exe", stock, target);
        Assert.Throws<InvalidDataException>(() => p.Apply(new byte[] { 1, 2, 3, 5 }));
        Assert.Throws<InvalidDataException>(() => p.Apply(target));
    }

    [Fact]
    public void Handles_shrinking_target()
    {
        byte[] stock = { 1, 2, 3, 4, 5, 6 }, target = { 1, 2, 9 };
        var p = ExePatch.Create("game.exe", stock, target);
        Assert.Equal(target, p.Apply(stock));
    }
}

public class PinnedExecutableTests
{
    static readonly Uri Url = new("https://x/manifest.json");
    static readonly string Pin = new('c', 64);
    static readonly Dictionary<string, string> Pins = new(StringComparer.OrdinalIgnoreCase) { ["yo_cm_client.exe"] = Pin };

    static UpdatePlan Plan(string path, string sha, IReadOnlyDictionary<string, string>? pins)
    {
        var m = new Manifest { Files = { new() { Path = path, Sha256 = sha, Size = 1, Url = "o/x" } } };
        using var t = new TempDir();
        return UpdatePlan.Create(m, Url, t.Path, new KeepLocal(Array.Empty<string>()), new HashCache(null).Get, pins);
    }

    [Fact]
    public void Pinned_exe_with_the_pinned_hash_is_downloaded()
    {
        var plan = Plan("yo_cm_client.exe", Pin, Pins);
        Assert.Empty(plan.Rejected);
        Assert.Single(plan.Downloads);
    }

    [Fact]
    public void Pinned_exe_with_another_hash_is_rejected()
    {
        var plan = Plan("yo_cm_client.exe", new string('d', 64), Pins);
        Assert.Empty(plan.Downloads);
        Assert.Single(plan.Rejected);
    }

    [Theory]
    [InlineData("other.exe")]
    [InlineData("sub/yo_cm_client.exe")]
    [InlineData("steamclient.dll")]
    [InlineData("mod/run.bat")]
    public void Every_other_executable_stays_rejected(string path)
    {
        var plan = Plan(path, Pin, Pins);
        Assert.Empty(plan.Downloads);
        Assert.Single(plan.Rejected);
    }

    [Fact]
    public void Without_pins_the_exe_is_rejected_like_before()
    {
        var plan = Plan("yo_cm_client.exe", Pin, null);
        Assert.Empty(plan.Downloads);
        Assert.Single(plan.Rejected);
    }

    [Fact]
    public void An_executable_can_never_be_deleted()
    {
        var m = new Manifest { Delete = { "yo_cm_client.exe" } };
        using var t = new TempDir();
        t.Write("yo_cm_client.exe", "x");
        var plan = UpdatePlan.Create(m, Url, t.Path, new KeepLocal(Array.Empty<string>()), new HashCache(null).Get, Pins);
        Assert.Empty(plan.Deletes);
        Assert.Single(plan.Rejected);
    }
}