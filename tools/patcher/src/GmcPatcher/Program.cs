using System.Diagnostics;
using System.Text.Json;
using Gmc.Core;
using GmcPatcher;

// Steam launch option:  "C:\path\GmcPatcher.exe" %command%
// args[0] = the game exe Steam would have started, the rest = its arguments (forwarded unchanged).
// Without arguments the patcher only updates (needs "gameRoot" in the config).
// "--restore-exe" as the only argument puts the stock game exe back.

string exeDir = AppContext.BaseDirectory;
var log = new ConsoleLog(Path.Combine(exeDir, "patcher.log"));
try { Console.Title = "GMC Patcher"; } catch (Exception) { }
log.Info($"GMC Patcher {typeof(ConsoleLog).Assembly.GetName().Version?.ToString(3)} - {DateTime.Now:yyyy-MM-dd HH:mm}");

SelfUpdate.Cleanup();     // leftovers (.old/.new) of an earlier self-update
bool restoreOnly = args.Length == 1 && args[0] == "--restore-exe";
string[] gameCmd = restoreOnly ? Array.Empty<string>() : args;
int exitCode = 0;

PatcherConfig config = new();
string configFile = Path.Combine(exeDir, "gmc-patcher.json");
bool haveConfig = false;
try
{
    if (File.Exists(configFile))
    {
        config = JsonSerializer.Deserialize(File.ReadAllText(configFile), GmcJson.Default.PatcherConfig) ?? config;
        haveConfig = true;
    }
    else log.Warn($"No config file {configFile} - skipping the update");
}
catch (Exception ex) { log.Error($"Config file unreadable ({ex.Message}) - skipping the update"); }

try
{
    string? root = !string.IsNullOrWhiteSpace(config.GameRoot) ? config.GameRoot
                 : gameCmd.Length > 0 ? Path.GetDirectoryName(Path.GetFullPath(gameCmd[0])) : null;
    string? why = root is null ? "no game command and no gameRoot in the config"
                : !Directory.Exists(root) ? "folder does not exist: " + root
                : !string.IsNullOrEmpty(config.RequiredFile) && !File.Exists(Path.Combine(root, config.RequiredFile)) ? $"{config.RequiredFile} not found in {root}"
                : null;
    if (why is not null) log.Warn("Game folder not usable (" + why + ") - skipping the update");
    else if (restoreOnly) ExePatcher.Restore(root!, log);
    else
    {
        log.Info("Game folder: " + root);
        if (haveConfig)
        {
            try
            {
                var pins = config.ApplyExePatch ? ExePatcher.PinnedExecutables() : null;
                var updater = new Updater(config, root!, Path.Combine(exeDir, "gmc-hashcache.json"), log, null, pins, SelfUpdate.PublicKey());
                var manifest = await updater.FetchManifestAsync();
                // 1. the patcher itself first: a newer patcher in the signed manifest replaces this exe and restarts with the same arguments
                if (config.SelfUpdate && manifest.Patcher is { } np && SelfUpdate.IsNewer(np)
                    && Environment.GetEnvironmentVariable(SelfUpdate.EnvFlag) is null)
                {
                    int? code = await SelfUpdate.TryAsync(updater, np, args, log);
                    if (code is not null) return code.Value;
                }
                // 2. then the game files
                await updater.ApplyAsync(manifest);
                log.Info("Modpack is up to date");
            }
            catch (UpdateAbortedException ex) { log.Error("Update aborted: " + ex.Message); }
            catch (HttpRequestException ex) { log.Error("Update server not reachable: " + ex.Message); }
            catch (OperationCanceledException) { log.Error("Update server timed out"); }
            catch (Exception ex) { log.Error("Update failed: " + ex); }
        }
        if (config.ApplyExePatch) ExePatcher.ApplyAll(root!, log);
        // credits check: warning only, never blocks the game or changes any file
        try { CreditsPrompt.Run(root!, log); } catch (Exception ex) { log.Warn("Credits check failed: " + ex.Message); }
    }
}
catch (Exception ex) { log.Error("Unexpected error: " + ex); }

// Always start the game, whatever happened above.
if (gameCmd.Length > 0)
{
    try
    {
        var psi = new ProcessStartInfo(gameCmd[0])
        {
            UseShellExecute = false,
            WorkingDirectory = Environment.CurrentDirectory,     // keep the working directory Steam gave us
        };
        foreach (string a in gameCmd.Skip(1)) psi.ArgumentList.Add(a);
        log.Info("Starting the game: " + string.Join(' ', gameCmd.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)));
        using var game = Process.Start(psi);
        if (game is not null && config.WaitForGame) { game.WaitForExit(); exitCode = game.ExitCode; }
    }
    catch (Exception ex)
    {
        log.Error("Could not start the game: " + ex.Message);
        exitCode = 1;
    }
}
return exitCode;
