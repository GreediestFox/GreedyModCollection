using System.Reflection;
using Gmc.Core;

namespace GmcPatcher;

/// <summary>
/// Applies the exe patches compiled into this patcher (resources "exepatch.*.gmcx").
/// A patch is only applied to the exact stock file it was made for; the stock file is kept as *.gmc_stock.
/// </summary>
static class ExePatcher
{
    /// <summary>File name -> SHA-256 of the patched exe, from the compiled-in patches. These are the only executables the updater may download.</summary>
    public static Dictionary<string, string> PinnedExecutables()
    {
        var pins = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var asm = Assembly.GetExecutingAssembly();
        foreach (string res in asm.GetManifestResourceNames().Where(n => n.StartsWith("exepatch.", StringComparison.Ordinal)))
        {
            try
            {
                using var s = asm.GetManifestResourceStream(res)!;
                var p = ExePatch.Deserialize(s);
                if (p.FileName == Path.GetFileName(p.FileName)) pins[p.FileName] = p.TargetSha256;
            }
            catch (Exception) { }
        }
        // exe pins: a tiny text resource "<file name>=<sha256>" per exe, no byte patch needed (the exe itself comes from the manifest)
        foreach (string res in asm.GetManifestResourceNames().Where(n => n.StartsWith("exepin.", StringComparison.Ordinal)))
        {
            try
            {
                using var r = new StreamReader(asm.GetManifestResourceStream(res)!);
                foreach (string line in r.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    int eq = line.IndexOf('=');
                    if (eq <= 0 || line.StartsWith('#')) continue;
                    string name = line[..eq].Trim(), sha = line[(eq + 1)..].Trim();
                    if (name == Path.GetFileName(name) && sha.Length == 64) pins[name] = sha.ToLowerInvariant();
                }
            }
            catch (Exception) { }
        }
        return pins;
    }

    public static void ApplyAll(string root, IUpdateLog log)
    {
        var asm = Assembly.GetExecutingAssembly();
        foreach (string res in asm.GetManifestResourceNames().Where(n => n.StartsWith("exepatch.", StringComparison.Ordinal)))
        {
            try
            {
                using var s = asm.GetManifestResourceStream(res)!;
                Apply(root, ExePatch.Deserialize(s), log);
            }
            catch (Exception ex) { log.Warn($"Exe patch {res} not applied: {ex.Message}"); }
        }
    }

    static void Apply(string root, ExePatch patch, IUpdateLog log)
    {
        // the name is compiled in, but is still kept to a plain file name inside the game folder
        if (patch.FileName != Path.GetFileName(patch.FileName)) throw new InvalidOperationException("invalid file name " + patch.FileName);
        string exe = Path.Combine(root, patch.FileName);
        if (!File.Exists(exe)) throw new FileNotFoundException(patch.FileName + " not found in the game folder");
        string sha = Hashing.Sha256File(exe);
        if (Hashing.SameHash(sha, patch.TargetSha256)) { log.Info($"{patch.FileName}: patched version present"); return; }
        string stockCopy = exe + ".gmc_stock";
        string source = exe;
        if (!Hashing.SameHash(sha, patch.StockSha256))
        {
            // Not the stock exe and not our current patch: typically an exe patched by an OLDER GMC patcher build.
            // The stock copy it kept lets us build the current patch from the original instead.
            if (File.Exists(stockCopy) && Hashing.SameHash(Hashing.Sha256File(stockCopy), patch.StockSha256))
            {
                log.Info($"{patch.FileName}: older patch version found - rebuilding from the kept stock copy");
                source = stockCopy;
            }
            else
            {
                log.Warn($"{patch.FileName}: unknown version (Steam update?) - not patched. The modpack may not work until a new patcher is released.");
                return;
            }
        }
        byte[] patched = patch.Apply(File.ReadAllBytes(source));       // verifies input and output hashes
        if (!File.Exists(stockCopy)) File.Copy(exe, stockCopy);
        string tmp = exe + ".gmc-patch";
        File.WriteAllBytes(tmp, patched);
        if (!Hashing.SameHash(Hashing.Sha256File(tmp), patch.TargetSha256)) { File.Delete(tmp); throw new IOException("written file has the wrong hash"); }
        File.SetAttributes(exe, FileAttributes.Normal);
        File.Move(tmp, exe, overwrite: true);
        log.Info($"{patch.FileName}: patch applied (stock copy kept as {Path.GetFileName(stockCopy)})");
    }

    /// <summary>Puts the stock exe back (GmcPatcher.exe --restore-exe).</summary>
    public static void Restore(string root, IUpdateLog log)
    {
        foreach (string stock in Directory.EnumerateFiles(root, "*.gmc_stock"))
        {
            string exe = stock[..^".gmc_stock".Length];
            File.Copy(stock, exe, overwrite: true);
            log.Info($"Restored {Path.GetFileName(exe)}");
        }
    }
}
