using System;
using System.Threading;
using System.Threading.Tasks;
using Deckify.Common;

namespace Deckify.Modules.AboutManager
{
    public class AutoUpdateChecker : IAutoUpdateChecker
    {
        private static readonly TimeSpan ThrottleInterval = TimeSpan.FromHours(24);

        private readonly IUpdateService _updateService;
        private readonly IUpdateNotificationService _notification;
        private readonly IAutoUpdateSettings _settings;

        public AutoUpdateChecker(IUpdateService updateService, IUpdateNotificationService notification, IAutoUpdateSettings settings)
        {
            _updateService = updateService;
            _notification = notification;
            _settings = settings;
        }

        public async Task RunStartupCheckAsync(CancellationToken ct = default)
        {
            try
            {
                if (!_settings.AutoCheckEnabled)
                    return;

                if (_settings.LastCheckUtc != default && DateTime.UtcNow - _settings.LastCheckUtc < ThrottleInterval)
                    return;

                var info = await _updateService.CheckForUpdatesAsync(ct).ConfigureAwait(false);
                _settings.LastCheckUtc = DateTime.UtcNow;
                _settings.Save();

                if (info.IsUpdateAvailable)
                {
                    // ClickOnce-native flow: just notify. The actual download happens
                    // when the user clicks Install in the toast — UpdateNotificationService
                    // calls UpdateService.ApplyPendingUpdateAsync which delegates to
                    // ApplicationDeployment.Update().
                    _notification.ShowUpdateAvailable(info.LatestVersion, info.ReleaseNotes);
                }
            }
            catch (OperationCanceledException)
            {
                // Add-in shutting down — nothing to do.
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "AutoUpdateChecker.RunStartupCheckAsync");
            }
        }
    }
}
