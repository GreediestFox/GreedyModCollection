using System.Runtime.InteropServices;
using Gmc.Core;

namespace GmcPatcher;

/// <summary>Runs the credits check after the update. Only warns (log + message box); the game always starts.</summary>
static class CreditsPrompt
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "MessageBoxW")]
    static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    const uint MB_OK = 0x0, MB_ICONWARNING = 0x30, MB_TOPMOST = 0x40000;

    public static void Run(string root, ConsoleLog log)
    {
        if (CreditsCheck.IsValid(root)) { log.Info($"{CreditsCheck.FileName} (copyright and asset credits) found"); return; }
        log.Warn($"{CreditsCheck.FileName} (copyright and asset credits) is missing or invalid in {root}. The game will still start.");
        try
        {
            MessageBox(IntPtr.Zero,
                $"The file \"{CreditsCheck.FileName}\" (copyright and asset credits) is missing or invalid in the game folder.\n\n" +
                "The game will still start. Please start the game again with the patcher online, or reinstall the GreedyModCollection, to restore it.",
                "GreedyModCollection", MB_OK | MB_ICONWARNING | MB_TOPMOST);
        }
        catch (Exception) { /* no message box available: the log line above is enough */ }
    }
}
