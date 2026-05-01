using System;
using System.Deployment.Application;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Deckify.Common;
using Deckify.Properties;

namespace Deckify.Modules.AboutManager
{
    public class UpdateService : IUpdateService
    {
        private static readonly HttpClient _httpClient = CreateHttpClient();

        private string ApiUrl => Settings.Default.GitHubRepository;

        private static HttpClient CreateHttpClient()
        {
            // GitHub API requires TLS 1.2+; net48 default is OS-level which is usually fine,
            // but make it explicit so a stale machine policy can't pin us to TLS 1.0.
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("deckify-Updater/1.0");
            return client;
        }

        public async Task<UpdateInfo> CheckForUpdatesAsync(CancellationToken ct = default)
        {
            var info = new UpdateInfo
            {
                IsNetworkDeployed = ApplicationDeployment.IsNetworkDeployed
            };

            // GitHub release tag is the source of truth for "latest published version" —
            // /publish drives both gh-pages and the GitHub release in one workflow, so they
            // are always in sync. We deliberately do NOT trust ClickOnce's UpdateAvailable
            // here: it compares server-to-cache, not running-to-latest, so a silently
            // pre-downloaded but not-yet-activated update reports UpdateAvailable=false
            // while the user is still on the older running version.
            string runningVersion = ClickOnceVersionProvider.GetInstalledVersion();

            try
            {
                var response = await _httpClient.GetAsync(ApiUrl, ct).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var json = JObject.Parse(jsonResponse);

                    info.LatestVersion = json["tag_name"].ToString().TrimStart('v');
                    info.ReleaseDate = DateTime.Parse(json["published_at"].ToString());
                    info.ReleaseNotes = json["body"]?.ToString() ?? string.Empty;
                    info.IsUpdateAvailable = !string.IsNullOrEmpty(runningVersion)
                        && VersionHelpers.IsNewerVersion(runningVersion, info.LatestVersion);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GitHub release poll");
            }

            return info;
        }

        public async Task<bool> ApplyPendingUpdateAsync(CancellationToken ct = default)
        {
            // Only ClickOnce-deployed installs can update themselves. Dev / debug builds
            // (loaded from bin/Debug via debug_setup.ps1) report IsNetworkDeployed=false
            // and have no update channel.
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                Logger.LogInfo("ApplyPendingUpdateAsync: not network-deployed; skipping ClickOnce update.");
                return false;
            }

            try
            {
                var deployment = ApplicationDeployment.CurrentDeployment;
                DeploymentProgressChangedEventHandler progressHandler = (s, e) =>
                    Logger.LogInfo($"ClickOnce update: {e.ProgressPercentage}% ({e.BytesCompleted}/{e.BytesTotal})");
                deployment.UpdateProgressChanged += progressHandler;
                try
                {
                    // Update() is synchronous and returns true if a new version was
                    // downloaded + staged. False means no update was available (despite
                    // GitHub saying otherwise — usually means HKCU\…\Manifest URL points
                    // somewhere stale; UpdateChannelMigration fixes that on first launch).
                    bool updated = await Task.Run(() => deployment.Update(), ct).ConfigureAwait(false);
                    return updated;
                }
                finally
                {
                    deployment.UpdateProgressChanged -= progressHandler;
                }
            }
            catch (DeploymentDownloadException ex)
            {
                Logger.LogError(ex, "ClickOnce update download");
                return false;
            }
            catch (InvalidDeploymentException ex)
            {
                Logger.LogError(ex, "ClickOnce update — invalid deployment");
                return false;
            }
            catch (TrustNotGrantedException ex)
            {
                Logger.LogError(ex, "ClickOnce update — trust not granted");
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ApplyPendingUpdateAsync");
                return false;
            }
        }
    }
}
