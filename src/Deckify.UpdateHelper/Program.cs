using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Deckify.UpdateHelper
{
    // Detached helper invoked by the deckify add-in when the user clicks "Restart now"
    // on the post-update toast. The add-in cannot kill its own host process and then
    // relaunch it, so it spawns this helper which:
    //   1) sends WM_CLOSE to the host PowerPoint window (graceful exit, save prompts)
    //   2) waits up to 5 minutes for the host to exit
    //   3) starts a fresh PowerPoint
    //
    // Authenticode-signed with the same cert as deckify-installer.exe so Defender's
    // ML heuristic doesn't flag the inline shell-out pattern (which is what tripped
    // the v0.1.46 false-positive).
    internal static class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        private const uint WM_CLOSE = 0x0010;

        private const int WaitTimeoutMs = 5 * 60 * 1000;

        private static int Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Log("Usage: Deckify.UpdateHelper.exe <pid> <powerpoint-exe-path>");
                    return 2;
                }

                if (!int.TryParse(args[0], out int pid))
                {
                    Log("Invalid PID: " + args[0]);
                    return 2;
                }

                string ppExe = args[1];
                if (!File.Exists(ppExe))
                {
                    Log("PowerPoint executable not found: " + ppExe);
                    return 2;
                }

                Process proc = null;
                try { proc = Process.GetProcessById(pid); }
                catch (ArgumentException) { /* already gone — fall through to relaunch */ }

                if (proc != null)
                {
                    IntPtr hwnd = proc.MainWindowHandle;
                    if (hwnd != IntPtr.Zero)
                    {
                        SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }

                    if (!proc.WaitForExit(WaitTimeoutMs))
                    {
                        // User likely cancelled a save prompt; abort the restart so
                        // we don't surprise them with a second instance.
                        Log("PowerPoint did not exit within 5 min; aborting restart.");
                        return 3;
                    }
                }

                // Brief settle so any Office cleanup finishes before the new instance starts.
                Thread.Sleep(500);
                Process.Start(ppExe);
                return 0;
            }
            catch (Exception ex)
            {
                Log("Helper failed: " + ex);
                return 1;
            }
        }

        private static void Log(string message)
        {
            try
            {
                string path = Path.Combine(Path.GetTempPath(), "deckify_Log.txt");
                File.AppendAllText(path,
                    "[" + DateTime.Now.ToString("O") + "] [UpdateHelper] " + message + Environment.NewLine);
            }
            catch
            {
                // Logging is best-effort; never block helper exit on it.
            }
        }
    }
}
