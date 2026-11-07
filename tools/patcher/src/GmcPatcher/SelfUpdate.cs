using System.Diagnostics;
using System.Reflection;
using Gmc.Core;

namespace GmcPatcher;

/// <summary>
/// The patcher replaces itself with the exe named in the signed manifest, then restarts itself with the same arguments
/// (so Steam's "%command%" still launches the game, now through the new patcher). The old exe stays as GmcPatcher.exe.old
/// until the next start; if the new exe cannot be started it is rolled back.
/// </summary>
static class SelfUpdate
{
    public const string EnvFlag = "GMC_SELFUPDATED";

    public static Version Current => typeof(SelfUpdate).Assembly.GetName().Version ?? new Version(0, 0, 0);

    /// <summary>The publisher's public key (compiled in from key\gmc_sign_public.txt); null = none, then the old restricted mode applies.</summary>
    public static string? PublicKey()
    {
        var asm = Assembly.GetExecutingAssembly();
        using var s = asm.GetManifestResourceStream("gmckey.public");
        if (s is null) return null;
        string k = new StreamReader(s).ReadToEnd();
        string line = k.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault(l => !l.StartsWith('#')) ?? "";
        return line.Length > 0 ? line : null;
    }

    public static bool IsNewer(PatcherInfo p) =>
        Version.TryParse(p.Version, out var v) && v > Current;

    /// <summary>Removes leftovers of a previous self-update.</summary>
    public static void Cleanup()
    {
        string? exe = Environment.ProcessPath;
        if (exe is null) return;
        foreach (string f in new[] { exe + ".old", exe + ".new" })
            try { if (File.Exists(f)) File.Delete(f); } catch (Exception) { }
    }

    /// <returns>The exit code of the restarted (new) patcher, or null if nothing was replaced (the current patcher then just carries on).</returns>
    public static async Task<int?> TryAsync(Updater updater, PatcherInfo info, string[] args, IUpdateLog log)
    {
        string? exe = Environment.ProcessPath;
        if (exe is null || !exe.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
            || !Path.GetFileName(exe).StartsWith("GmcPatcher", StringComparison.OrdinalIgnoreCase)) return null;   // dotnet run / renamed host: never self-replace
        string tmp = exe + ".new", old = exe + ".old";
        log.Info($"New patcher {info.Version} available (this is {Current.ToString(3)}) - updating the patcher first");
        try
        {
            await updater.DownloadPatcherAsync(info, tmp);
            if (File.Exists(old)) File.Delete(old);
            File.Move(exe, old);                 // a running exe can be renamed, not overwritten
            File.Move(tmp, exe);
        }
        catch (Exception ex)
        {
            log.Warn("Patcher self-update failed, continuing with the current patcher: " + ex.Message);
            try { if (File.Exists(tmp)) File.Delete(tmp); } catch (Exception) { }
            try { if (!File.Exists(exe) && File.Exists(old)) File.Move(old, exe); } catch (Exception) { }
            return null;
        }

        try
        {
            var psi = new ProcessStartInfo(exe) { UseShellExecute = false, WorkingDirectory = Environment.CurrentDirectory };
            foreach (string a in args) psi.ArgumentList.Add(a);
            psi.Environment[EnvFlag] = "1";     // the new patcher must not update itself again (loop guard)
            log.Info("Restarting with the new patcher");
            using var p = Process.Start(psi) ?? throw new InvalidOperationException("process did not start");
            p.WaitForExit();
            return p.ExitCode;
        }
        catch (Exception ex)
        {
            log.Error("New patcher could not be started, rolling back: " + ex.Message);
            try { File.Delete(exe); File.Move(old, exe); } catch (Exception) { }
            return null;
        }
    }
}
