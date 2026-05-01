using System;
using System.Reflection;

namespace Deckify.Common
{
    public static class VersionHelpers
    {
        public static bool IsNewerVersion(string current, string latest)
        {
            if (string.IsNullOrEmpty(current) || string.IsNullOrEmpty(latest))
                return false;

            try
            {
                Version currentVersion = new Version(current);
                Version latestVersion = new Version(latest);
                return latestVersion > currentVersion;
            }
            catch
            {
                return false;
            }
        }

        public static string GetExecutingAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
