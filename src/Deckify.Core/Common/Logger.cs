using System;
using System.Diagnostics;
using System.IO;

namespace Deckify.Common
{
    public static class Logger
    {
        public static void LogError(Exception ex, string context = "")
        {
            try
            {
                string message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {context} - {ex.Message}\n{ex.StackTrace}";
                
                // Write to Debug Output (visible in Visual Studio)
                Debug.WriteLine(message);

                // Ideally, write to a log file in %TEMP% for production debugging
                string logPath = Path.Combine(Path.GetTempPath(), LogFileName);
                File.AppendAllText(logPath, message + Environment.NewLine + "--------------------------------------------------" + Environment.NewLine);
            }
            catch
            {
                // Fail silently if logging fails to avoid recursive crashes
            }
        }

        public const string LogFileName = "deckify_Log.txt";

        public static void LogInfo(string message)
        {
             try
            {
                string logMsg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: {message}";
                Debug.WriteLine(logMsg);
                string logPath = Path.Combine(Path.GetTempPath(), LogFileName);
                File.AppendAllText(logPath, logMsg + Environment.NewLine);
            }
             catch { }
        }
    }
}
