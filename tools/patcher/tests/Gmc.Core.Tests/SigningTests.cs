using System.Text;
using Gmc.Core;
using Xunit;

namespace Gmc.Core.Tests;

public class SigningTests
{
    const string Base = "https://cdn.example/gmc/";
    static readonly (string priv, string pub) Keys = ManifestSigner.GenerateKeyPair();

    static byte[] Bytes(string s) => Encoding.UTF8.GetBytes(s);

    /// <summary>Publishes a manifest (signed with <paramref name="signWith"/>, or unsigned if null) plus its files.</summary>
    static (FakeServer server, PatcherConfig cfg, Manifest m) Publish(Dictionary<string, string> files, string? signWith, string issued = "2026-11-07T10:00:00.0000000Z",
        PatcherInfo? patcher = null, byte[]? patcherBytes = null)
    {
        var s = new FakeServer();
        var m = new Manifest { Version = "2.0.0", Issued = issued, Patcher = patcher };
        foreach (var (path, content) in files)
        {
            byte[] b = Bytes(content);
            s.Files[Base + "o/" + path] = b;
            m.Files.Add(new ManifestFile { Path = path, Sha256 = Hashing.Sha256Bytes(b), Size = b.Length, Url = "o/" + path });
        }
        if (patcher is not null && patcherBytes is not null) s.Files[Base + patcher.Url] = patcherBytes;
        byte[] json = Bytes(m.ToJson());
        s.Files[Base + "manifest.json"] = json;
        if (signWith is not null) s.Files[Base + "manifest.json.sig"] = Bytes(ManifestSigner.Sign(json, signWith));
        return (s, new PatcherConfig { ManifestUrl = Base + "manifest.json", TimeoutSeconds = 5 }, m);
    }

    [Fact]
    public void Sign_and_verify_roundtrip_and_rejects_changes()
    {
        byte[] data = Bytes("hello");
        string sig = ManifestSigner.Sign(data, Keys.priv);
        Assert.True(ManifestSigner.Verify(data, sig, Keys.pub));
        Assert.False(ManifestSigner.Verify(Bytes("hellp"), sig, Keys.pub));
        Assert.False(ManifestSigner.Verify(data, sig, ManifestSigner.GenerateKeyPair().publicKey));
        Assert.False(ManifestSigner.Verify(data, "not base64!!", Keys.pub));
        Assert.False(ManifestSigner.Verify(data, "", Keys.pub));
    }

    [Fact]
    public async Task Signed_manifest_installs_exe_dll_bat_and_js()
    {
        using var t = new TempDir();
        var (server, cfg, _) = Publish(new() { ["tool.exe"] = "EXE", ["bin/lib.dll"] = "DLL", ["start.bat"] = "BAT", ["mod/x.js"] = "JS", ["data/a.xml"] = "A" }, Keys.priv);
        await new Updater(cfg, t.Path, null, new ListLog(), server, null, Keys.pub).RunAsync();
        Assert.Equal("EXE", File.ReadAllText(Path.Combine(t.Path, "tool.exe")));
        Assert.Equal("DLL", File.ReadAllText(Path.Combine(t.Path, "bin", "lib.dll")));
        Assert.Equal("BAT", File.ReadAllText(Path.Combine(t.Path, "start.bat")));
        Assert.Equal("JS", File.ReadAllText(Path.Combine(t.Path, "mod", "x.js")));
    }

    [Fact]
    public async Task Without_a_public_key_executables_stay_rejected()
    {
        using var t = new TempDir();
        var (server, cfg, _) = Publish(new() { ["tool.exe"] = "EXE", ["data/a.xml"] = "A" }, Keys.priv);
        var log = new ListLog();
        await new Updater(cfg, t.Path, null, log, server).RunAsync();
        Assert.False(File.Exists(Path.Combine(t.Path, "tool.exe")));
        Assert.True(File.Exists(Path.Combine(t.Path, "data", "a.xml")));
        Assert.Contains(log.Lines, l => l.Contains("tool.exe") && l.Contains("executable"));
    }

    [Fact]
    public async Task Steam_dlls_and_logs_are_never_written_even_when_signed()
    {
        using var t = new TempDir();
        var (server, cfg, _) = Publish(new() { ["steam_api64.dll"] = "S", ["logs/x.txt"] = "L", ["ok.dll"] = "OK" }, Keys.priv);
        await new Updater(cfg, t.Path, null, new ListLog(), server, null, Keys.pub).RunAsync();
        Assert.False(File.Exists(Path.Combine(t.Path, "steam_api64.dll")));
        Assert.False(File.Exists(Path.Combine(t.Path, "logs", "x.txt")));
        Assert.True(File.Exists(Path.Combine(t.Path, "ok.dll")));
    }

    [Fact]
    public async Task Missing_signature_aborts_before_anything_is_written()
    {
        using var t = new TempDir();
        var (server, cfg, _) = Publish(new() { ["a.dll"] = "X" }, null);
        var ex = await Assert.ThrowsAsync<UpdateAbortedException>(() => new Updater(cfg, t.Path, null, new ListLog(), server, null, Keys.pub).RunAsync());
        Assert.Contains("not signed", ex.Message);
        Assert.False(File.Exists(Path.Combine(t.Path, "a.dll")));
    }

    [Fact]
    public async Task Tampered_manifest_is_refused()
    {
        using var t = new TempDir();
        var (server, cfg, m) = Publish(new() { ["a.dll"] = "X" }, Keys.priv);
        // attacker swaps the file and fixes the hash in the manifest - but cannot re-sign it
        byte[] evil = Bytes("EVIL");
        server.Files[Base + "o/a.dll"] = evil;
        m.Files[0].Sha256 = Hashing.Sha256Bytes(evil); m.Files[0].Size = evil.Length;
        server.Files[Base + "manifest.json"] = Bytes(m.ToJson());
        var ex = await Assert.ThrowsAsync<UpdateAbortedException>(() => new Updater(cfg, t.Path, null, new ListLog(), server, null, Keys.pub).RunAsync());
        Assert.Contains("INVALID", ex.Message);
        Assert.False(File.Exists(Path.Combine(t.Path, "a.dll")));
    }

    [Fact]
    public async Task Manifest_signed_with_another_key_is_refused()
    {
        using var t = new TempDir();
        var (server, cfg, _) = Publish(new() { ["a.dll"] = "X" }, ManifestSigner.GenerateKeyPair().privateKey);
        await Assert.ThrowsAsync<UpdateAbortedException>(() => new Updater(cfg, t.Path, null, new ListLog(), server, null, Keys.pub).RunAsync());
    }

    [Fact]
    public async Task Older_signed_manifest_is_refused_after_a_newer_one_was_applied()
    {
        using var t = new TempDir();
        string cache = Path.Combine(t.Path, "cache.json");
        var (newer, cfg, _) = Publish(new() { ["a.xml"] = "new" }, Keys.priv, "2026-11-07T10:00:00.0000000Z");
        await new Updater(cfg, t.Path, cache, new ListLog(), newer, null, Keys.pub).RunAsync();
        var (older, _, _) = Publish(new() { ["a.xml"] = "old" }, Keys.priv, "2026-11-01T10:00:00.0000000Z");
        var ex = await Assert.ThrowsAsync<UpdateAbortedException>(() => new Updater(cfg, t.Path, cache, new ListLog(), older, null, Keys.pub).RunAsync());
        Assert.Contains("older", ex.Message);
        Assert.Equal("new", File.ReadAllText(Path.Combine(t.Path, "a.xml")));
    }

    [Fact]
    public async Task Patcher_download_is_verified_and_written()
    {
        using var t = new TempDir();
        byte[] exe = Bytes("NEW PATCHER EXE");
        var info = new PatcherInfo { Version = "1.2.0", Sha256 = Hashing.Sha256Bytes(exe), Size = exe.Length, Url = "o/patcher" };
        var (server, cfg, m) = Publish(new() { ["a.xml"] = "A" }, Keys.priv, patcher: info, patcherBytes: exe);
        var u = new Updater(cfg, t.Path, null, new ListLog(), server, null, Keys.pub) { RetryDelayMs = 1 };
        var fetched = await u.FetchManifestAsync();
        Assert.Equal("1.2.0", fetched.Patcher!.Version);
        string dest = Path.Combine(t.Path, "GmcPatcher.exe.new");
        await u.DownloadPatcherAsync(fetched.Patcher, dest);
        Assert.Equal("NEW PATCHER EXE", File.ReadAllText(dest));
    }

    [Fact]
    public async Task Patcher_with_wrong_hash_is_not_accepted()
    {
        using var t = new TempDir();
        byte[] exe = Bytes("NEW PATCHER EXE");
        var info = new PatcherInfo { Version = "1.2.0", Sha256 = Hashing.Sha256Bytes(Bytes("something else")), Size = exe.Length, Url = "o/patcher" };
        var (server, cfg, _) = Publish(new(), Keys.priv, patcher: info, patcherBytes: exe);
        var u = new Updater(cfg, t.Path, null, new ListLog(), server, null, Keys.pub) { RetryDelayMs = 1 };
        var fetched = await u.FetchManifestAsync();
        string dest = Path.Combine(t.Path, "GmcPatcher.exe.new");
        await Assert.ThrowsAsync<UpdateAbortedException>(() => u.DownloadPatcherAsync(fetched.Patcher!, dest));
        Assert.False(File.Exists(dest));
    }

    [Fact]
    public async Task Patcher_download_needs_a_signed_manifest()
    {
        using var t = new TempDir();
        var info = new PatcherInfo { Version = "9.0.0", Sha256 = new string('a', 64), Size = 5, Url = "o/p" };
        var (server, cfg, _) = Publish(new(), null, patcher: info);
        var u = new Updater(cfg, t.Path, null, new ListLog(), server);          // no public key -> unsigned mode
        var fetched = await u.FetchManifestAsync();
        await Assert.ThrowsAsync<UpdateAbortedException>(() => u.DownloadPatcherAsync(fetched.Patcher!, Path.Combine(t.Path, "p.new")));
    }

    [Fact]
    public void Manifest_builder_ships_executables_only_when_allowed()
    {
        using var t = new TempDir();
        t.Write("tool.exe", "E"); t.Write("lib/a.dll", "D"); t.Write("data/x.xml", "X");
        var plain = ManifestBuilder.Build(t.Path, Base, "1", Array.Empty<string>());
        Assert.Equal(new[] { "data/x.xml" }, plain.Files.Select(f => f.Path).ToArray());
        var all = ManifestBuilder.Build(t.Path, Base, "1", Array.Empty<string>(), allowExecutables: true);
        Assert.Equal(3, all.Files.Count);
    }
}
