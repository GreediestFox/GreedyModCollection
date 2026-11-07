using System.Net;
using System.Security.Cryptography;

namespace Gmc.Core;

public sealed class UpdateAbortedException(string message) : Exception(message);

/// <summary>Where the updater reports to (console + patcher.log in the real patcher, a list in tests).</summary>
public interface IUpdateLog
{
    void Info(string msg);
    void Warn(string msg);
    void Error(string msg);
    void Progress(int index, int count, string file, long doneBytes, long totalBytes);
    void ClearProgress();
}

/// <summary>Fetches the manifest, downloads missing/changed files, removes deleted ones.</summary>
public sealed class Updater(PatcherConfig config, string root, string? cacheFile, IUpdateLog log, HttpMessageHandler? handler = null,
    IReadOnlyDictionary<string, string>? pinnedExecutables = null, string? signingPublicKey = null)
{
    public const int MaxAttempts = 3;
    public int RetryDelayMs { get; init; } = 1000;

    Uri? _manifestUrl;
    bool _trusted;

    HttpClient CreateHttp()
    {
        var http = new HttpClient(handler ?? new SocketsHttpHandler
        {
            AutomaticDecompression = DecompressionMethods.All,
            ConnectTimeout = TimeSpan.FromSeconds(config.TimeoutSeconds),
        }, disposeHandler: handler is null)
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };
        http.DefaultRequestHeaders.UserAgent.ParseAdd("GmcPatcher/1.1");
        return http;
    }

    public async Task RunAsync() => await ApplyAsync(await FetchManifestAsync());

    /// <summary>
    /// Downloads manifest.json. With a public key the detached signature manifest.json.sig must verify
    /// (otherwise the update is refused) and the manifest may then contain executables; without a key it is the old, restricted mode.
    /// </summary>
    public async Task<Manifest> FetchManifestAsync()
    {
        if (!Uri.TryCreate(config.ManifestUrl, UriKind.Absolute, out var manifestUrl) || manifestUrl.Scheme != Uri.UriSchemeHttps)
            throw new UpdateAbortedException("manifestUrl in the config is missing or not https");
        _manifestUrl = manifestUrl;

        using var http = CreateHttp();
        log.Info($"Checking for updates: {manifestUrl}");
        Manifest manifest;
        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(config.TimeoutSeconds)))
        {
            byte[] raw = await http.GetByteArrayAsync(manifestUrl, cts.Token);
            if (signingPublicKey is not null)
            {
                byte[] sig;
                try { sig = await http.GetByteArrayAsync(new Uri(manifestUrl.AbsoluteUri + ".sig"), cts.Token); }
                catch (HttpRequestException) { throw new UpdateAbortedException("manifest.json.sig is missing - update refused (the manifest is not signed)"); }
                if (!ManifestSigner.Verify(raw, sig, signingPublicKey))
                    throw new UpdateAbortedException("manifest signature is INVALID - update refused (the files on the server do not come from the GMC publisher)");
                _trusted = true;
            }
            manifest = Manifest.Parse(System.Text.Encoding.UTF8.GetString(raw));
        }
        if (_trusted) CheckNotOlder(manifest);
        log.Info($"Modpack version {manifest.Version}: {manifest.Files.Count} files{(_trusted ? " (signature OK)" : "")}");
        return manifest;
    }

    /// <summary>Replay protection: a signed manifest older than the last one applied here is refused (an old signed manifest could otherwise bring back old files).</summary>
    void CheckNotOlder(Manifest m)
    {
        if (cacheFile is null || !DateTime.TryParse(m.Issued, null, System.Globalization.DateTimeStyles.RoundtripKind, out var issued)) return;
        string marker = cacheFile + ".issued";
        try
        {
            if (File.Exists(marker) && DateTime.TryParse(File.ReadAllText(marker).Trim(), null, System.Globalization.DateTimeStyles.RoundtripKind, out var last) && issued < last)
                throw new UpdateAbortedException($"manifest is older ({issued:u}) than the last applied one ({last:u}) - update refused");
        }
        catch (IOException) { }
    }

    void RememberIssued(Manifest m)
    {
        if (cacheFile is null || !_trusted || m.Issued.Length == 0) return;
        try { File.WriteAllText(cacheFile + ".issued", m.Issued); } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { }
    }

    /// <summary>Downloads the new patcher exe named in the (verified) manifest to <paramref name="dest"/>; size and SHA-256 are checked.</summary>
    public async Task DownloadPatcherAsync(PatcherInfo info, string dest)
    {
        if (!_trusted || _manifestUrl is null) throw new UpdateAbortedException("self-update needs a signed manifest");
        if (!PlanUrl(info.Url, out var url) || info.Sha256.Length != 64 || info.Size <= 0) throw new UpdateAbortedException("patcher entry in the manifest is invalid");
        using var http = CreateHttp();
        var d = new PlannedDownload(new ManifestFile { Path = Path.GetFileName(dest), Sha256 = info.Sha256, Size = info.Size, Url = info.Url }, dest, url!);
        await DownloadAsync(http, d, _ => { }, CancellationToken.None);
    }

    bool PlanUrl(string url, out Uri? result) => UpdatePlan.TryFileUrl(_manifestUrl!, url, out result);

    public async Task ApplyAsync(Manifest manifest)
    {
        var manifestUrl = _manifestUrl ?? throw new InvalidOperationException("FetchManifestAsync first");
        using var http = CreateHttp();

        var keep = new KeepLocal(manifest.KeepLocal.Concat(config.KeepLocal));
        var cache = new HashCache(cacheFile);
        try
        {
            int checkedFiles = 0, toCheck = manifest.Files.Count + manifest.Delete.Count;
            var plan = UpdatePlan.Create(manifest, manifestUrl, root, keep, full =>
            {
                log.Progress(++checkedFiles, toCheck, "checking " + Path.GetFileName(full), 0, 0);
                return cache.Get(full);
            }, pinnedExecutables, _trusted);
            log.ClearProgress();
            foreach (string r in plan.Rejected) log.Warn("Ignored unsafe/invalid entry: " + r);
            foreach (string k in plan.KeptLocal) log.Info("Kept local file: " + k);
            log.Info($"{plan.UpToDate} up to date, {plan.Downloads.Count} to download ({FormatSize(plan.DownloadBytes)}), {plan.Deletes.Count} to remove");

            // A pinned executable replaces the game's own exe: keep the existing one as *.gmc_stock first (used by --restore-exe).
            foreach (var d in plan.Downloads.Where(d => PathSafety.IsForbidden(d.File.Path) && !d.File.Path.Contains('/')
                                                       && d.File.Path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && File.Exists(d.FullPath)))
            {
                string stockCopy = d.FullPath + ".gmc_stock";
                if (!File.Exists(stockCopy)) { File.Copy(d.FullPath, stockCopy); log.Info($"Kept the current {Path.GetFileName(d.FullPath)} as {Path.GetFileName(stockCopy)}"); }
            }

            // Several downloads at once: most files are small icons, where the per-request round trip dominates.
            long total = plan.DownloadBytes, doneBytes = 0;
            int finished = 0;
            var progressLock = new object();
            using var abort = new CancellationTokenSource();
            UpdateAbortedException? failure = null;
            var options = new ParallelOptions { MaxDegreeOfParallelism = Math.Clamp(config.ParallelDownloads, 1, 32), CancellationToken = abort.Token };
            try
            {
                await Parallel.ForEachAsync(plan.Downloads, options, async (d, ct) =>
                {
                    long reported = 0;
                    try
                    {
                        await DownloadAsync(http, d, b =>
                        {
                            long delta = b - reported; reported = b;
                            long now = Interlocked.Add(ref doneBytes, delta);
                            lock (progressLock) log.Progress(Volatile.Read(ref finished) + 1, plan.Downloads.Count, d.File.Path, now, total);
                        }, ct);
                    }
                    catch (UpdateAbortedException ex)
                    {
                        Interlocked.CompareExchange(ref failure, ex, null);
                        abort.Cancel();                       // stop the other downloads; files already in place stay
                        return;
                    }
                    Interlocked.Add(ref doneBytes, d.File.Size - reported);
                    Interlocked.Increment(ref finished);
                    lock (progressLock) cache.Remember(d.FullPath, d.File.Sha256.ToLowerInvariant());
                });
            }
            catch (OperationCanceledException) when (failure is not null) { }
            if (failure is not null) throw failure;
            log.ClearProgress();
            if (plan.Downloads.Count > 0) log.Info($"Downloaded {plan.Downloads.Count} files ({FormatSize(total)})");

            foreach (string full in plan.Deletes)
            {
                try
                {
                    File.SetAttributes(full, FileAttributes.Normal);
                    File.Delete(full);
                    cache.Forget(full);
                    log.Info("Removed " + Path.GetRelativePath(root, full));
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    log.Warn($"Could not remove {Path.GetRelativePath(root, full)}: {ex.Message}");
                }
            }
            RememberIssued(manifest);
        }
        finally
        {
            try { cache.Save(); } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { log.Warn("Could not save hash cache: " + ex.Message); }
        }
    }

    /// <summary>Downloads into a temp file in the target folder, verifies size + SHA-256, then moves it into place.</summary>
    async Task DownloadAsync(HttpClient http, PlannedDownload d, Action<long> progress, CancellationToken ct)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(d.FullPath)!);
        string tmp = d.FullPath + ".gmc-download";
        string? lastError = null;
        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                string sha = await FetchAsync(http, d.Url, tmp, progress, ct);
                long size = new FileInfo(tmp).Length;
                if (size == d.File.Size && Hashing.SameHash(sha, d.File.Sha256))
                {
                    if (File.Exists(d.FullPath)) File.SetAttributes(d.FullPath, FileAttributes.Normal);
                    File.Move(tmp, d.FullPath, overwrite: true);
                    return;
                }
                lastError = $"hash/size mismatch (got {size} bytes)";
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                TryDelete(tmp);
                throw;                                       // another download failed: stop quietly
            }
            catch (Exception ex) when (ex is HttpRequestException or IOException or OperationCanceledException)
            {
                lastError = ex.Message;
            }
            TryDelete(tmp);
            progress(0);
            log.Warn($"{d.File.Path}: attempt {attempt}/{MaxAttempts} failed: {lastError}");
            if (attempt < MaxAttempts) await Task.Delay(RetryDelayMs * attempt, ct);
        }
        throw new UpdateAbortedException($"{d.File.Path} could not be downloaded ({lastError}); the old file was kept");
    }

    /// <summary>Streams the URL into <paramref name="tmp"/> and returns its SHA-256. Cancels if no data arrives for the timeout.</summary>
    async Task<string> FetchAsync(HttpClient http, Uri url, string tmp, Action<long> progress, CancellationToken ct)
    {
        using var stall = CancellationTokenSource.CreateLinkedTokenSource(ct);
        stall.CancelAfter(TimeSpan.FromSeconds(config.TimeoutSeconds));
        using var resp = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, stall.Token);
        resp.EnsureSuccessStatusCode();
        await using var src = await resp.Content.ReadAsStreamAsync(stall.Token);
        await using var dst = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 16);
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        var buf = new byte[1 << 16];
        long got = 0;
        int n;
        while ((n = await src.ReadAsync(buf, stall.Token)) > 0)
        {
            stall.CancelAfter(TimeSpan.FromSeconds(config.TimeoutSeconds));
            hash.AppendData(buf, 0, n);
            await dst.WriteAsync(buf.AsMemory(0, n), stall.Token);
            got += n;
            progress(got);
        }
        return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
    }

    static void TryDelete(string p) { try { if (File.Exists(p)) File.Delete(p); } catch (IOException) { } catch (UnauthorizedAccessException) { } }

    public static string FormatSize(long b) => b switch
    {
        >= 1L << 30 => $"{b / (double)(1L << 30):0.00} GB",
        >= 1L << 20 => $"{b / (double)(1L << 20):0.0} MB",
        >= 1L << 10 => $"{b / 1024.0:0} KB",
        _ => $"{b} B",
    };
}
