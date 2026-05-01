using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using Deckify.Common;

namespace Deckify.Modules.AboutManager
{
    public class UpdateChannelMigration : IUpdateChannelMigration
    {
        // VSTO add-in registration lives here per the manifest specification.
        // Manifest value is read by ClickOnce at update time; if it's file://, the
        // ApplicationDeployment.Update() call polls a dead local path and reports
        // no-update-available even when gh-pages has a newer version.
        private const string AddinRegistryPath = @"Software\Microsoft\Office\PowerPoint\Addins\Deckify";
        private const string ClickOnceManifestUrl = "https://tillmannschatz.github.io/deckify/Deckify.vsto";

        public bool NeedsMigration()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(AddinRegistryPath))
                {
                    if (key == null) return false;
                    var manifest = key.GetValue("Manifest") as string;
                    if (string.IsNullOrEmpty(manifest)) return false;
                    return manifest.StartsWith("file:", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "UpdateChannelMigration.NeedsMigration");
                return false;
            }
        }

        public async Task<bool> RunMigrationAsync()
        {
            try
            {
                var vstoInstaller = ResolveVSTOInstallerPath();
                if (string.IsNullOrEmpty(vstoInstaller))
                {
                    Logger.LogError(new FileNotFoundException("VSTOInstaller.exe not found"),
                        "UpdateChannelMigration.RunMigrationAsync");
                    return false;
                }

                // VSTOInstaller is Microsoft-signed — Defender lets it through. /i installs
                // (or repoints if already installed). Synchronous wait so we can report
                // success/failure to the toast.
                var psi = new ProcessStartInfo
                {
                    FileName = vstoInstaller,
                    Arguments = "/i " + ClickOnceManifestUrl,
                    UseShellExecute = false,
                    CreateNoWindow = false
                };
                var process = Process.Start(psi);
                if (process == null) return false;

                await Task.Run(() => process.WaitForExit()).ConfigureAwait(false);

                if (process.ExitCode != 0)
                {
                    Logger.LogInfo("VSTOInstaller exited with code " + process.ExitCode);
                    return false;
                }

                // Verify the registry got repointed. VSTOInstaller forks async on some
                // Office versions; the actual write may lag the process exit slightly.
                // Allow a couple seconds before checking.
                await Task.Delay(1500).ConfigureAwait(false);
                return !NeedsMigration();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "UpdateChannelMigration.RunMigrationAsync");
                return false;
            }
        }

        private static string ResolveVSTOInstallerPath()
        {
            // Try both bitnesses — the add-in's process bitness matches Office's,
            // CommonProgramFiles resolves to the matching root, but we double-check
            // both just in case Office is mismatched against the add-in (rare).
            var candidates = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                             "Microsoft Shared", "VSTO", "10.0", "VSTOInstaller.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86),
                             "Microsoft Shared", "VSTO", "10.0", "VSTOInstaller.exe")
            };
            return candidates.FirstOrDefault(File.Exists);
        }
    }
}
