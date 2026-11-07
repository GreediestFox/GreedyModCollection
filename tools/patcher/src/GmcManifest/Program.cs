using Gmc.Core;

// GmcManifest build    --source <modpack folder> --base-url <https://.../files/> --version 1.2.0 --out manifest.json
//                      [--previous old-manifest.json] [--delete-list deletes.txt] [--keep-local <pattern>]... [--stage-dir <upload dir>]
// GmcManifest exepatch --stock <stock exe> --patched <patched exe> --out <file.gmcx>
// GmcManifest diff     --old old-manifest.json --new manifest.json

static Dictionary<string, List<string>> ParseArgs(string[] a)
{
    var d = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
    for (int i = 1; i < a.Length; i++)
    {
        if (!a[i].StartsWith("--")) throw new ArgumentException("unexpected argument " + a[i]);
        if (i + 1 >= a.Length) throw new ArgumentException("missing value for " + a[i]);
        if (!d.TryGetValue(a[i], out var list)) d[a[i]] = list = new();
        list.Add(a[++i]);
    }
    return d;
}

static string Req(Dictionary<string, List<string>> o, string k) =>
    o.TryGetValue(k, out var v) ? v[^1] : throw new ArgumentException("missing " + k);

static string? Opt(Dictionary<string, List<string>> o, string k) => o.TryGetValue(k, out var v) ? v[^1] : null;

static Manifest Load(string p) => Manifest.Parse(File.ReadAllText(p));

try
{
    string cmd = args.Length > 0 ? args[0] : "";
    var o = ParseArgs(args);
    switch (cmd)
    {
        case "build":
        {
            string baseUrl = Req(o, "--base-url");
            if (!baseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("--base-url must be https");
            var keep = o.TryGetValue("--keep-local", out var k) ? k : new List<string>();
            // --allow-exe <name>=<sha256>: the one executable (game folder root) that may be listed, with its required hash
            var pins = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (o.TryGetValue("--allow-exe", out var allow))
                foreach (string a in allow)
                {
                    int eq = a.IndexOf('=');
                    if (eq <= 0 || a.Length - eq - 1 != 64) throw new ArgumentException("--allow-exe needs <file name>=<64 hex sha256>");
                    pins[a[..eq]] = a[(eq + 1)..];
                }
            // --sign-key <private key file>: signs the manifest (manifest.json.sig next to it) and then ships ALL files incl. .exe/.dll/.bat/.js
            string? signKeyFile = Opt(o, "--sign-key");
            string? signKey = signKeyFile is null ? null : File.ReadAllText(signKeyFile).Trim();
            var m = ManifestBuilder.Build(Req(o, "--source"), baseUrl, Req(o, "--version"), keep, w => Console.Error.WriteLine("warning: " + w), Opt(o, "--stage-dir"), pins,
                allowExecutables: signKey is not null);
            m.Issued = DateTime.UtcNow.ToString("O");
            // --patcher-exe <GmcPatcher.exe>: listed in the manifest so installed patchers replace themselves (needs --sign-key)
            string? patcherExe = Opt(o, "--patcher-exe");
            if (patcherExe is not null)
            {
                if (signKey is null) throw new ArgumentException("--patcher-exe needs --sign-key (only a signed manifest may replace the patcher)");
                string psha = Hashing.Sha256File(patcherExe);
                string? stage = Opt(o, "--stage-dir");
                if (stage is not null) { Directory.CreateDirectory(stage); string staged = Path.Combine(stage, psha); if (!File.Exists(staged)) File.Copy(patcherExe, staged); }
                string pver = Opt(o, "--patcher-version") ?? System.Diagnostics.FileVersionInfo.GetVersionInfo(patcherExe).FileVersion ?? "";
                if (!Version.TryParse(pver, out _)) throw new ArgumentException("patcher version unknown/invalid - pass --patcher-version x.y.z");
                m.Patcher = new PatcherInfo { Version = pver, Sha256 = psha, Size = new FileInfo(patcherExe).Length, Url = baseUrl + (stage is not null ? psha : "GmcPatcher.exe") };
                Console.WriteLine($"patcher {pver} listed ({psha[..12]})");
            }
            string? prev = Opt(o, "--previous");
            string? delList = Opt(o, "--delete-list");
            var extra = delList is null ? null : File.ReadAllLines(delList).Where(l => !l.TrimStart().StartsWith('#'));
            m.Delete = ManifestBuilder.ComputeDeleted(prev is null ? null : Load(prev), m, extra);
            File.WriteAllText(Req(o, "--out"), m.ToJson());
            if (signKey is not null)
            {
                // sign the exact bytes of the written file, then re-verify with the matching public key
                string sig = ManifestSigner.Sign(File.ReadAllBytes(Req(o, "--out")), signKey);
                File.WriteAllText(Req(o, "--out") + ".sig", sig);
                Console.WriteLine("signed -> " + Req(o, "--out") + ".sig");
            }
            Console.WriteLine($"{m.Files.Count} files, {m.Files.Sum(f => f.Size) / 1048576.0:0.0} MB, {m.Delete.Count} deletions -> {Req(o, "--out")}");
            return 0;
        }
        case "genkey":
        {
            // GmcManifest genkey --private-out <file> --public-out <file>   (keep the private key off the web host and out of any upload folder!)
            string priv = Req(o, "--private-out"), pub = Req(o, "--public-out");
            if (File.Exists(priv)) throw new ArgumentException(priv + " exists - refusing to overwrite a signing key");
            var (k, p) = ManifestSigner.GenerateKeyPair();
            File.WriteAllText(priv, k + Environment.NewLine);
            File.WriteAllText(pub, "# GMC manifest signing key (public part), embedded into GmcPatcher" + Environment.NewLine + p + Environment.NewLine);
            Console.WriteLine("key pair written: " + priv + " (SECRET) / " + pub);
            return 0;
        }
        case "exepatch":
        {
            string stock = Req(o, "--stock");
            byte[] s = File.ReadAllBytes(stock), t = File.ReadAllBytes(Req(o, "--patched"));
            var p = ExePatch.Create(Opt(o, "--name") ?? "yo_cm_client.exe", s, t);
            p.Apply(s);   // self-check
            byte[] data = p.Serialize();
            File.WriteAllBytes(Req(o, "--out"), data);
            Console.WriteLine($"{p.FileName}: {p.Chunks.Count} chunks, {data.Length} bytes; stock {p.StockSha256[..12]} -> patched {p.TargetSha256[..12]}");
            return 0;
        }
        case "diff":
        {
            var oldM = Load(Req(o, "--old"));
            var newM = Load(Req(o, "--new"));
            var oldMap = oldM.Files.ToDictionary(f => PathSafety.Normalize(f.Path), StringComparer.OrdinalIgnoreCase);
            foreach (var f in newM.Files)
            {
                string p = PathSafety.Normalize(f.Path);
                if (!oldMap.TryGetValue(p, out var of)) Console.WriteLine("+ " + p);
                else if (!Hashing.SameHash(of.Sha256, f.Sha256)) Console.WriteLine("~ " + p);
            }
            foreach (string d in ManifestBuilder.ComputeDeleted(oldM, newM)) Console.WriteLine("- " + d);
            return 0;
        }
        default:
            Console.WriteLine("""
                GmcManifest build    --source <modpack folder> --base-url <https://host/path/> --version <x.y.z> --out manifest.json
                                     [--previous <old manifest.json>] [--delete-list <file, one path per line>] [--keep-local <pattern>]...
                                     [--stage-dir <upload folder>]   content-addressed: files copied as <sha256>, url = base-url + sha256
                                     [--sign-key <private key file>]  sign (manifest.json.sig) and allow ALL file types incl. exe/dll/bat/js
                                     [--patcher-exe <GmcPatcher.exe> [--patcher-version x.y.z]]  patchers update themselves to this exe (needs --sign-key)
                GmcManifest genkey   --private-out <secret file> --public-out <public file>
                GmcManifest exepatch --stock <stock exe> --patched <patched exe> --out <name.gmcx> [--name yo_cm_client.exe]
                GmcManifest diff     --old <old manifest.json> --new <new manifest.json>
                """);
            return cmd.Length == 0 ? 0 : 2;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine("error: " + ex.Message);
    return 1;
}
