// AmIAlive - keeps the LiF dedicated server running.
//
//   AmIAlive.exe            watchdog loop (single instance): if the server exe is not running and no restart is in progress,
//                           start Start_LiF_Server_VPS.bat (hidden). After a start it stays quiet for 120 s.
//   AmIAlive.exe restart    planned restart: marks "restart in progress", closes the running server (graceful close, then kill),
//                           starts it again, waits until it is up. The watchdog does nothing while the mark exists.
//   AmIAlive.exe status     prints what it sees.
//   AmIAlive.exe stop       planned stop: marks "stopped on purpose" (the watchdog leaves the server alone), closes the server.
//   AmIAlive.exe start      removes the "stopped" mark and starts the server.
//
// 120 seconds after it started the server, AmIAlive also closes the console window of Start_LiF_Server_VPS.bat (the cmd.exe that
// runs it) - the server itself keeps running (it was started detached). Every cmd.exe running the bat that is older than 120 s is closed.
//
// Settings (optional) in AmIAlive.ini next to the exe:  process=ddctd_cm_yo_server  bat=Start_LiF_Server_VPS.bat  grace=120  interval=30
//   dryrun=1  only logs what it would do.
// Log: AmIAlive.log in the same folder.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;

static class AmIAlive
{
    static string Dir = (Environment.GetEnvironmentVariable("AMIALIVE_DIR") ?? AppDomain.CurrentDomain.BaseDirectory).TrimEnd('\\');
    static string ProcName = "ddctd_cm_yo_server";
    static string BatName = "Start_LiF_Server_VPS.bat";
    static int GraceSeconds = 120;
    static int IntervalSeconds = 30;
    static bool DryRun = false;

    static string RestartMark { get { return Path.Combine(Dir, "AmIAlive.restart"); } }
    static string StopMark { get { return Path.Combine(Dir, "AmIAlive.stopped"); } }
    static string LogFile { get { return Path.Combine(Dir, "AmIAlive.log"); } }

    static void Log(string msg)
    {
        string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + msg;
        Console.WriteLine(line);
        try
        {
            if (File.Exists(LogFile) && new FileInfo(LogFile).Length > 1000000) File.Move(LogFile, LogFile + ".old");
            File.AppendAllText(LogFile, line + Environment.NewLine);
        }
        catch { }
    }

    static void LoadConfig()
    {
        string ini = Path.Combine(Dir, "AmIAlive.ini");
        if (!File.Exists(ini)) return;
        foreach (string raw in File.ReadAllLines(ini))
        {
            string l = raw.Trim();
            if (l.Length == 0 || l.StartsWith("#") || l.StartsWith(";")) continue;
            int i = l.IndexOf('=');
            if (i < 1) continue;
            string k = l.Substring(0, i).Trim().ToLowerInvariant(), v = l.Substring(i + 1).Trim();
            int n;
            if (k == "process") ProcName = v;
            else if (k == "bat") BatName = v;
            else if (k == "grace" && int.TryParse(v, out n)) GraceSeconds = n;
            else if (k == "interval" && int.TryParse(v, out n)) IntervalSeconds = n;
            else if (k == "dryrun") DryRun = v == "1";
        }
    }

    static Process[] ServerProcs() { return Process.GetProcessesByName(ProcName); }

    static bool RestartInProgress()
    {
        try
        {
            if (!File.Exists(RestartMark)) return false;
            // a mark older than 10 minutes is stale (a restart that died half way)
            if ((DateTime.UtcNow - File.GetLastWriteTimeUtc(RestartMark)).TotalMinutes > 10)
            {
                File.Delete(RestartMark);
                Log("stale restart mark removed");
                return false;
            }
            return true;
        }
        catch { return false; }
    }

    static void StartServer(string why)
    {
        string bat = Path.Combine(Dir, BatName);
        if (!File.Exists(bat)) { Log("ERROR: " + bat + " not found - cannot start the server"); return; }
        Log("starting the server (" + why + ")");
        if (DryRun) { Log("[dryrun] would run: cmd /c \"" + bat + "\" auto"); return; }
        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c \"\"" + bat + "\" auto\"");
        psi.WorkingDirectory = Dir;
        psi.CreateNoWindow = true;
        psi.UseShellExecute = false;
        Process.Start(psi);
    }

    static void CloseServer()
    {
        Process[] ps = ServerProcs();
        foreach (Process p in ps)
        {
            Log("closing server process " + p.Id);
            if (DryRun) continue;
            try { p.CloseMainWindow(); } catch { }
        }
        if (DryRun) return;
        DateTime until = DateTime.Now.AddSeconds(20);
        while (DateTime.Now < until && ServerProcs().Length > 0) Thread.Sleep(500);
        foreach (Process p in ServerProcs())
        {
            Log("server process " + p.Id + " did not close, killing it");
            try { p.Kill(); p.WaitForExit(10000); } catch (Exception e) { Log("kill failed: " + e.Message); }
        }
    }

    // close the cmd.exe windows that run the start bat and are older than the grace time
    static void CloseBatWindows()
    {
        try
        {
            using (ManagementObjectSearcher s = new ManagementObjectSearcher("SELECT ProcessId, CommandLine, CreationDate FROM Win32_Process WHERE Name = 'cmd.exe'"))
            {
                foreach (ManagementObject o in s.Get())
                {
                    string cl = (o["CommandLine"] as string) ?? "";
                    if (cl.IndexOf(BatName, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    DateTime created = ManagementDateTimeConverter.ToDateTime((string)o["CreationDate"]);
                    if ((DateTime.Now - created).TotalSeconds < GraceSeconds) continue;
                    int pid = Convert.ToInt32(o["ProcessId"]);
                    Log("closing the start-bat window (cmd.exe " + pid + ", open for " + (int)(DateTime.Now - created).TotalSeconds + " s)");
                    if (DryRun) continue;
                    try { Process.GetProcessById(pid).Kill(); } catch (Exception e) { Log("could not close it: " + e.Message); }
                }
            }
        }
        catch (Exception e) { Log("bat window check failed: " + e.Message); }
    }

    static int Watchdog()
    {
        bool created;
        using (Mutex m = new Mutex(true, "Global\\AmIAlive_" + Dir.Replace('\\', '_').Replace(':', '_'), out created))
        {
            if (!created) { Log("another AmIAlive is already running in this folder - exiting"); return 0; }
            Log("AmIAlive started (folder " + Dir + ", process " + ProcName + ", grace " + GraceSeconds + " s, interval " + IntervalSeconds + " s" + (DryRun ? ", DRY RUN" : "") + ")");
            DateTime graceUntil = DateTime.MinValue;
            Queue<DateTime> starts = new Queue<DateTime>();
            DateTime pausedUntil = DateTime.MinValue;
            while (true)
            {
                try
                {
                    CloseBatWindows();
                    bool running = ServerProcs().Length > 0;
                    if (!running && DateTime.Now >= graceUntil && DateTime.Now >= pausedUntil)
                    {
                        if (File.Exists(StopMark)) { /* stopped on purpose */ }
                        else if (RestartInProgress()) { /* a planned restart is running */ }
                        else
                        {
                            while (starts.Count > 0 && (DateTime.Now - starts.Peek()).TotalMinutes > 10) starts.Dequeue();
                            if (starts.Count >= 5)
                            {
                                Log("the server died 5 times within 10 minutes - pausing restarts for 15 minutes (check the server log)");
                                pausedUntil = DateTime.Now.AddMinutes(15);
                                starts.Clear();
                            }
                            else
                            {
                                Log("server exe is not running");
                                StartServer("watchdog");
                                starts.Enqueue(DateTime.Now);
                                graceUntil = DateTime.Now.AddSeconds(GraceSeconds);
                            }
                        }
                    }
                }
                catch (Exception e) { Log("loop error: " + e.Message); }
                Thread.Sleep(IntervalSeconds * 1000);
            }
        }
    }

    static int Restart()
    {
        File.WriteAllText(RestartMark, DateTime.UtcNow.ToString("o"));
        try
        {
            if (File.Exists(StopMark)) File.Delete(StopMark);
            Log("planned restart: closing the server");
            CloseServer();
            Thread.Sleep(3000);
            File.WriteAllText(RestartMark, DateTime.UtcNow.ToString("o"));
            StartServer("planned restart");
            // wait until the new process shows up (the bat starts MariaDB first, so give it time)
            DateTime until = DateTime.Now.AddSeconds(GraceSeconds);
            while (DateTime.Now < until && ServerProcs().Length == 0 && !DryRun) Thread.Sleep(1000);
            Log(ServerProcs().Length > 0 ? "planned restart done: the server process is up" : (DryRun ? "[dryrun] done" : "WARNING: the server process did not appear within " + GraceSeconds + " s"));
        }
        finally
        {
            try { File.Delete(RestartMark); } catch { }
        }
        return 0;
    }

    static int Main(string[] args)
    {
        LoadConfig();
        string cmd = args.Length > 0 ? args[0].ToLowerInvariant() : "";
        if (cmd == "restart") return Restart();
        if (cmd == "stop")
        {
            File.WriteAllText(StopMark, DateTime.UtcNow.ToString("o"));
            Log("planned stop: the watchdog leaves the server alone until 'AmIAlive.exe start'");
            CloseServer();
            return 0;
        }
        if (cmd == "start")
        {
            try { File.Delete(StopMark); } catch { }
            if (ServerProcs().Length == 0) StartServer("manual start"); else Log("the server is already running");
            return 0;
        }
        if (cmd == "status")
        {
            Process[] ps = ServerProcs();
            Console.WriteLine("folder:           " + Dir);
            Console.WriteLine("server process:   " + (ps.Length > 0 ? "running (pid " + ps[0].Id + ", since " + ps[0].StartTime + ")" : "NOT running"));
            Console.WriteLine("restart mark:     " + (File.Exists(RestartMark) ? "yes (restart in progress)" : "no"));
            Console.WriteLine("stopped on purpose: " + (File.Exists(StopMark) ? "yes" : "no"));
            return 0;
        }
        return Watchdog();
    }
}
