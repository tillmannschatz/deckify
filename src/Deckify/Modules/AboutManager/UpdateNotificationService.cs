using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Deckify.Common;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Deckify.Modules.AboutManager
{
    public class UpdateNotificationService : IUpdateNotificationService
    {
        private const string ActionKey = "action";
        private const string ActionInstall = "install";
        private const string ActionRestart = "restart";
        private const string ActionMigrate = "migrate";
        private const string HelperExeName = "Deckify.UpdateHelper.exe";

        private readonly IUpdateService _updateService;
        private bool _activationHandlerRegistered;
        private readonly object _registerLock = new object();

        public UpdateNotificationService(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        public void ShowUpdateAvailable(string version, string releaseNotes)
        {
            EnsureActivationHandler();
            try
            {
                new ToastContentBuilder()
                    .AddText("deckify update available")
                    .AddText($"Version {version} is ready to install.")
                    .AddButton(new ToastButton()
                        .SetContent("Install")
                        .AddArgument(ActionKey, ActionInstall))
                    .AddButton(new ToastButton()
                        .SetContent("Later")
                        .SetDismissActivation())
                    .Show();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ShowUpdateAvailable");
            }
        }

        public void ShowUpdateInstalling()
        {
            try
            {
                new ToastContentBuilder()
                    .AddText("deckify update installing")
                    .AddText("The installer is running in the background.")
                    .Show();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ShowUpdateInstalling");
            }
        }

        public void ShowUpdateInstalled()
        {
            EnsureActivationHandler();
            try
            {
                var builder = new ToastContentBuilder()
                    .AddText("deckify update installed")
                    .AddText("Restart PowerPoint to activate the new version.");

                // Only offer the restart button if the signed helper EXE actually
                // shipped alongside the add-in. Falls back to info-only toast in
                // dev / partial-payload scenarios.
                if (TryGetHelperPath(out _))
                {
                    builder
                        .AddButton(new ToastButton()
                            .SetContent("Restart now")
                            .AddArgument(ActionKey, ActionRestart))
                        .AddButton(new ToastButton()
                            .SetContent("Later")
                            .SetDismissActivation());
                }

                builder.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ShowUpdateInstalled");
            }
        }

        public void ShowUpdateError(string message)
        {
            try
            {
                new ToastContentBuilder()
                    .AddText("deckify update error")
                    .AddText(string.IsNullOrEmpty(message) ? "Unknown error." : message)
                    .Show();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ShowUpdateError");
            }
        }

        public void ShowChannelMigrationAvailable()
        {
            EnsureActivationHandler();
            try
            {
                new ToastContentBuilder()
                    .AddText("deckify update channel needs a one-time migration")
                    .AddText("Switch to in-app updates? PowerPoint will need a restart afterwards.")
                    .AddButton(new ToastButton()
                        .SetContent("Migrate now")
                        .AddArgument(ActionKey, ActionMigrate))
                    .AddButton(new ToastButton()
                        .SetContent("Later")
                        .SetDismissActivation())
                    .Show();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ShowChannelMigrationAvailable");
            }
        }

        // Subscribe to activation events lazily on first toast send. Subscribing in the
        // constructor would force ToastNotificationManagerCompat to do its AUMID +
        // Start-Menu-shortcut + COM-CLSID registration during DI build, which we want
        // deferred to a real toast send so any failure is logged in the right context.
        private void EnsureActivationHandler()
        {
            if (_activationHandlerRegistered) return;
            lock (_registerLock)
            {
                if (_activationHandlerRegistered) return;
                ToastNotificationManagerCompat.OnActivated += OnToastActivated;
                _activationHandlerRegistered = true;
            }
        }

        private void OnToastActivated(ToastNotificationActivatedEventArgsCompat args)
        {
            try
            {
                var arguments = ToastArguments.Parse(args.Argument);
                if (!arguments.TryGetValue(ActionKey, out string action)) return;

                if (string.Equals(action, ActionInstall, StringComparison.OrdinalIgnoreCase))
                {
                    HandleInstallAction();
                }
                else if (string.Equals(action, ActionRestart, StringComparison.OrdinalIgnoreCase))
                {
                    HandleRestartAction();
                }
                else if (string.Equals(action, ActionMigrate, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMigrateAction();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "OnToastActivated");
            }
        }

        private void HandleInstallAction()
        {
            // Run the install flow off the COM activation thread so we don't block the
            // toast notifier — and so a long-running installer doesn't hold the
            // activation callback open.
            _ = Task.Run(async () =>
            {
                try
                {
                    ShowUpdateInstalling();
                    bool success = await _updateService.ApplyPendingUpdateAsync().ConfigureAwait(false);
                    if (success)
                    {
                        ShowUpdateInstalled();
                    }
                    else
                    {
                        ShowUpdateError("Update could not be applied. See log file for details.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Toast install action");
                    ShowUpdateError(ex.Message);
                }
            });
        }

        private void HandleMigrateAction()
        {
            // Resolve migration service via Globals.ThisAddIn (service-locator) — adding
            // it as a constructor dependency would force a DI ordering constraint we
            // don't otherwise need. Migration is a rare one-time event.
            _ = Task.Run(async () =>
            {
                try
                {
                    var migration = Globals.ThisAddIn.ServiceProvider.GetService(typeof(IUpdateChannelMigration)) as IUpdateChannelMigration;
                    if (migration == null)
                    {
                        ShowUpdateError("Migration service not available.");
                        return;
                    }
                    bool ok = await migration.RunMigrationAsync().ConfigureAwait(false);
                    if (ok)
                    {
                        // Reuse the installed-toast: it has the Restart button which the
                        // user needs to click to actually load the new manifest URL.
                        ShowUpdateInstalled();
                    }
                    else
                    {
                        ShowUpdateError("Migration failed. See log file for details.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Toast migrate action");
                    ShowUpdateError(ex.Message);
                }
            });
        }

        private void HandleRestartAction()
        {
            try
            {
                if (!TryGetHelperPath(out string helperPath))
                {
                    ShowUpdateError("Restart helper is missing — close and reopen PowerPoint manually.");
                    return;
                }

                var pid = Process.GetCurrentProcess().Id;
                var ppExe = Process.GetCurrentProcess().MainModule.FileName;

                Process.Start(new ProcessStartInfo
                {
                    FileName = helperPath,
                    Arguments = pid + " \"" + ppExe + "\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "HandleRestartAction");
                ShowUpdateError("Could not start the restart helper.");
            }
        }

        private static bool TryGetHelperPath(out string path)
        {
            path = null;
            try
            {
                var dir = Path.GetDirectoryName(typeof(UpdateNotificationService).Assembly.Location);
                if (string.IsNullOrEmpty(dir)) return false;
                var candidate = Path.Combine(dir, HelperExeName);
                if (!File.Exists(candidate)) return false;
                path = candidate;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
