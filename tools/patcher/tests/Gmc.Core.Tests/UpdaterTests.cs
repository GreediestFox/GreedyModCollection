using System.Net;
using System.Text;
using Gmc.Core;
using Xunit;

namespace Gmc.Core.Tests;

/// <summary>In-memory "web server": URL -> bytes, with optional corruption / failures per URL.</summary>
sealed class FakeServer : HttpMessageHandler
{
    public Dictionary<string, byte[]> Files { get; } = new();
    public Dictionary<string, int> CorruptTimes { get; } = new();   // serve wrong bytes this many times
    public bool Offline { get; set; }
    public List<string> Requests { get; } = new();

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        string url = request.RequestUri!.ToString();
        byte[]? data;
        lock (this)
        {
            Requests.Add(url);
            if (Offline) throw new HttpRequestException("no route to host");
            if (!Files.TryGetValue(url, out data)) return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            if (CorruptTimes.TryGetValue(url, out int n) && n > 0)
            {
                CorruptTimes[url] = n - 1;
                data = data.Select(b => (byte)(b ^ 0x55)).ToArray();
            }
        }
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(data) });
    }
}

sealed class ListLog : IUpdateLog
{
    public List<string> Lines { get; } = new();
    public void Info(string msg) => Lines.Add("INFO " + msg);
    public void Warn(string msg) => Lines.Add("WARN " + msg);
    public void Error(string msg) => Lines.Add("ERROR " + msg);
    public void Progress(int index, int count, string file, long doneBytes, long totalBytes) { }
    public void ClearProgress() { }
}

public class UpdaterTests
{
    const string Base = "https://cdn.example/gmc/";

    static (FakeServer server, PatcherConfig config) Publish(Manifest m, Dictionary<string, string> files)
    {
        var s = new FakeServer();
        foreach (var (path, content) in files)
        {
            byte[] b = Encoding.UTF8.GetBytes(content);
            s.Files[Base + "files/" + path] = b;
            m.Files.Add(new ManifestFile { Path = path, Sha256 = Hashing.Sha256Bytes(b), Size = b.Length, Url = "files/" + path });
        }
        s.Files[Base + "manifest.json"] = Encoding.UTF8.GetBytes(m.ToJson());
        return (s, new PatcherConfig { ManifestUrl = Base + "manifest.json", TimeoutSeconds = 5 });
    }

    [Fact]
    public async Task Fresh_install_then_second_run_downloads_nothing()
    {
        using var t = new TempDir();
        var (server, cfg) = Publish(new Manifest { Version = "1.0.0" }, new() { ["mod/a.cs"] = "A", ["data/b.xml"] = "B" });
        string cache = Path.Combine(t.Path, "cache.json");

        await new Updater(cfg, t.Path, cache, new ListLog(), server).RunAsync();
        Assert.Equal("A", File.ReadAllText(Path.Combine(t.Path, "mod", "a.cs")));
        Assert.Equal("B", File.ReadAllText(Path.Combine(t.Path, "data", "b.xml")));

        server.Requests.Clear();
        await new Updater(cfg, t.Path, cache, new ListLog(), server).RunAsync();
        Assert.Equal(new[] { Base + "manifest.json" }, server.Requests);   // only the manifest
    }

    [Fact]
    public async Task Hash_mismatch_is_retried_and_then_succeeds()
    {
        using var t = new TempDir();
        var (server, cfg) = Publish(new Manifest(), new() { ["mod/a.cs"] = "good" });
        server.CorruptTimes[Base + "files/mod/a.cs"] = 2;
        await new Updater(cfg, t.Path, null, new ListLog(), server) { RetryDelayMs = 1 }.RunAsync();
        Assert.Equal("good", File.ReadAllText(Path.Combine(t.Path, "mod", "a.cs")));
    }

    [Fact]
    public async Task Three_bad_downloads_abort_and_keep_the_old_file()
    {
        using var t = new TempDir();
        t.Write("mod/a.cs", "old");
        var (server, cfg) = Publish(new Manifest(), new() { ["mod/a.cs"] = "new" });
        server.CorruptTimes[Base + "files/mod/a.cs"] = 3;
        var ex = await Assert.ThrowsAsync<UpdateAbortedException>(() =>
            new Updater(cfg, t.Path, null, new ListLog(), server) { RetryDelayMs = 1 }.RunAsync());
        Assert.Contains("mod/a.cs", ex.Message);
        Assert.Equal("old", File.ReadAllText(Path.Combine(t.Path, "mod", "a.cs")));
        Assert.False(File.Exists(Path.Combine(t.Path, "mod", "a.cs.gmc-download")));
    }

    [Fact]
    public async Task Deletes_listed_files_but_not_keepLocal_ones()
    {
        using var t = new TempDir();
        t.Write("scripts/client/init.cs.dso", "stale");
        t.Write("mod/sf-mods/configs/c.xml", "mine");
        var m = new Manifest { Delete = { "scripts/client/init.cs.dso", "mod/sf-mods/configs/c.xml" }, KeepLocal = { "mod/sf-mods/configs/" } };
        var (server, cfg) = Publish(m, new());
        await new Updater(cfg, t.Path, null, new ListLog(), server).RunAsync();
        Assert.False(File.Exists(Path.Combine(t.Path, "scripts", "client", "init.cs.dso")));
        Assert.True(File.Exists(Path.Combine(t.Path, "mod", "sf-mods", "configs", "c.xml")));
    }

    [Fact]
    public async Task Offline_throws_HttpRequestException_and_changes_nothing()
    {
        using var t = new TempDir();
        t.Write("mod/a.cs", "old");
        var (server, cfg) = Publish(new Manifest(), new() { ["mod/a.cs"] = "new" });
        server.Offline = true;
        await Assert.ThrowsAsync<HttpRequestException>(() => new Updater(cfg, t.Path, null, new ListLog(), server).RunAsync());
        Assert.Equal("old", File.ReadAllText(Path.Combine(t.Path, "mod", "a.cs")));
    }

    [Fact]
    public async Task Parallel_download_of_many_files()
    {
        using var t = new TempDir();
        var files = Enumerable.Range(0, 200).ToDictionary(i => $"art/2D/Items/icon{i}.png", i => "icon " + i);
        var (server, cfg) = Publish(new Manifest(), files);
        cfg.ParallelDownloads = 8;
        server.CorruptTimes[Base + "files/art/2D/Items/icon7.png"] = 1;          // one retry in the middle
        await new Updater(cfg, t.Path, null, new ListLog(), server) { RetryDelayMs = 1 }.RunAsync();
        foreach (var (p, c) in files) Assert.Equal(c, File.ReadAllText(Path.Combine(t.Path, p.Replace('/', '\\'))));
        Assert.Empty(Directory.GetFiles(t.Path, "*.gmc-download", SearchOption.AllDirectories));
    }

    [Fact]
    public async Task Parallel_abort_keeps_old_file_and_leaves_no_temp_files()
    {
        using var t = new TempDir();
        t.Write("mod/bad.cs", "old");
        var files = Enumerable.Range(0, 50).ToDictionary(i => $"mod/f{i}.cs", i => "f" + i);
        files["mod/bad.cs"] = "new";
        var (server, cfg) = Publish(new Manifest(), files);
        cfg.ParallelDownloads = 8;
        server.CorruptTimes[Base + "files/mod/bad.cs"] = 3;
        await Assert.ThrowsAsync<UpdateAbortedException>(() =>
            new Updater(cfg, t.Path, null, new ListLog(), server) { RetryDelayMs = 1 }.RunAsync());
        Assert.Equal("old", File.ReadAllText(Path.Combine(t.Path, "mod", "bad.cs")));
        Assert.Empty(Directory.GetFiles(t.Path, "*.gmc-download", SearchOption.AllDirectories));
    }

    [Fact]
    public async Task Http_manifest_url_is_refused()
    {
        using var t = new TempDir();
        var cfg = new PatcherConfig { ManifestUrl = "http://cdn.example/manifest.json" };
        await Assert.ThrowsAsync<UpdateAbortedException>(() => new Updater(cfg, t.Path, null, new ListLog(), new FakeServer()).RunAsync());
    }
}

public class PinnedExeUpdateTests
{
    const string Base = "https://cdn.example/gmc/";

    static (FakeServer server, PatcherConfig cfg, byte[] exe) Publish()
    {
        var s = new FakeServer();
        byte[] exe = Encoding.UTF8.GetBytes("NEW-EXE-BYTES");
        s.Files[Base + "o/exe"] = exe;
        var m = new Manifest { Version = "1.0.7" };
        m.Files.Add(new ManifestFile { Path = "yo_cm_client.exe", Sha256 = Hashing.Sha256Bytes(exe), Size = exe.Length, Url = "o/exe" });
        s.Files[Base + "manifest.json"] = Encoding.UTF8.GetBytes(m.ToJson());
        return (s, new PatcherConfig { ManifestUrl = Base + "manifest.json", TimeoutSeconds = 5 }, exe);
    }

    [Fact]
    public async Task Pinned_exe_replaces_the_game_exe_and_keeps_the_old_one_as_gmc_stock()
    {
        using var t = new TempDir();
        t.Write("yo_cm_client.exe", "OLD-EXE-BYTES");
        var (server, cfg, exe) = Publish();
        var pins = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["yo_cm_client.exe"] = Hashing.Sha256Bytes(exe) };
        await new Updater(cfg, t.Path, null, new ListLog(), server, pins).RunAsync();
        Assert.Equal("NEW-EXE-BYTES", File.ReadAllText(Path.Combine(t.Path, "yo_cm_client.exe")));
        Assert.Equal("OLD-EXE-BYTES", File.ReadAllText(Path.Combine(t.Path, "yo_cm_client.exe.gmc_stock")));
    }

    [Fact]
    public async Task A_second_run_does_not_touch_the_stock_copy()
    {
        using var t = new TempDir();
        t.Write("yo_cm_client.exe", "OLD-EXE-BYTES");
        var (server, cfg, exe) = Publish();
        var pins = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["yo_cm_client.exe"] = Hashing.Sha256Bytes(exe) };
        await new Updater(cfg, t.Path, null, new ListLog(), server, pins).RunAsync();
        await new Updater(cfg, t.Path, null, new ListLog(), server, pins).RunAsync();
        Assert.Equal("OLD-EXE-BYTES", File.ReadAllText(Path.Combine(t.Path, "yo_cm_client.exe.gmc_stock")));
        Assert.Equal(2, server.Requests.Count(r => r.EndsWith("manifest.json")));
        Assert.Single(server.Requests, r => r.EndsWith("o/exe"));          // downloaded only once
    }

    [Fact]
    public async Task Manifest_exe_with_another_hash_is_ignored_and_the_old_exe_stays()
    {
        using var t = new TempDir();
        t.Write("yo_cm_client.exe", "OLD-EXE-BYTES");
        var (server, cfg, _) = Publish();
        var pins = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["yo_cm_client.exe"] = new string('f', 64) };
        var log = new ListLog();
        await new Updater(cfg, t.Path, null, log, server, pins).RunAsync();
        Assert.Equal("OLD-EXE-BYTES", File.ReadAllText(Path.Combine(t.Path, "yo_cm_client.exe")));
        Assert.DoesNotContain(server.Requests, r => r.EndsWith("o/exe"));
        Assert.Contains(log.Lines, l => l.Contains("pinned"));
    }
}