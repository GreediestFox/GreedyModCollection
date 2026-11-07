using Gmc.Core;

namespace GmcPatcher;

/// <summary>Appends to patcher.log next to the exe (rotated at 2 MB) and mirrors to the console.</summary>
sealed class ConsoleLog : IUpdateLog
{
    readonly string _file;
    int _len;
    DateTime _last;

    public ConsoleLog(string path)
    {
        _file = path;
        try
        {
            var fi = new FileInfo(path);
            if (fi.Exists && fi.Length > 2 * 1024 * 1024) File.Move(path, path + ".1", overwrite: true);
        }
        catch (Exception) { }
    }

    public void Info(string msg) => Write("INFO ", msg, ConsoleColor.Gray);
    public void Warn(string msg) => Write("WARN ", msg, ConsoleColor.Yellow);
    public void Error(string msg) => Write("ERROR", msg, ConsoleColor.Red);

    void Write(string level, string msg, ConsoleColor color)
    {
        ClearProgress();
        var old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ForegroundColor = old;
        try { File.AppendAllText(_file, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {level} {msg}{Environment.NewLine}"); }
        catch (Exception) { }
    }

    /// <summary>One-line console progress: [x/y] file  done/total.</summary>
    public void Progress(int index, int count, string file, long doneBytes, long totalBytes)
    {
        if (Console.IsOutputRedirected || (DateTime.UtcNow - _last).TotalMilliseconds < 100 && index != count) return;
        _last = DateTime.UtcNow;
        string name = file.Length > 48 ? "..." + file[^45..] : file;
        string line = totalBytes > 0
            ? $"[{index}/{count}] {name}  {Updater.FormatSize(doneBytes)} / {Updater.FormatSize(totalBytes)}"
            : $"[{index}/{count}] {name}";
        Console.Write("\r" + line.PadRight(_len));
        _len = line.Length;
    }

    public void ClearProgress()
    {
        if (_len == 0 || Console.IsOutputRedirected) return;
        Console.Write("\r" + new string(' ', _len) + "\r");
        _len = 0;
    }
}
