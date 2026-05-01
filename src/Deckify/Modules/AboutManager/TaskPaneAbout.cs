using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Reflection;

// Modules for the update Check
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Deckify.Modules.AboutManager;
using Microsoft.Extensions.DependencyInjection;

namespace Deckify
{
    public partial class TaskPaneAbout : UserControl
    {

        public TaskPaneAbout()
        {
            InitializeComponent();

            // Set Labels for Installed Version, latest Version (incl. Release date) and last Update
            labelInstalledVersion.Text = ClickOnceVersionProvider.GetInstalledVersion();
            labelLatestReleasedVersion.Text = Properties.Settings.Default.LatestVersion;
            labelLastUpdateCheck.Text = Properties.Settings.Default.LastUpdateCheck.ToString("yyyy/MM/dd - HH:mm", DateTimeFormatInfo.InvariantInfo);
            labelLatestVersionReleaseDate.Text = Properties.Settings.Default.LatestVersionReleaseDate.ToString("yyyy/MM/dd - HH:mm", DateTimeFormatInfo.InvariantInfo);

            // Set Labels for Licence Key, Holder & Validity
            if (DeckifySettings.Default.DevMode)
            {
                labelLicenceKey.Visible = true;
                labelLicenceKeyHolder.Visible = true;
                labelLicenceKeyValidity.Visible = true;

                labelLicenceKey.Text = DeckifySettings.Default.LicenseKey;
                labelLicenceKeyHolder.Text = DeckifySettings.Default.LicenseKeyHolder;
                switch (DeckifySettings.Default.LicenseKeyValidity)
                {
                    case true:
                        labelLicenceKeyValidity.Text = "Valid";
                        return;
                    case false:
                        labelLicenceKeyValidity.Text = "Not valid";
                        return;
                }
            }
            else
            {
                labelLicenceKey.Visible = false;
                labelLicenceKeyHolder.Visible = false;
                labelLicenceKeyValidity.Visible = false;
            }
        }

        private async void buttonCheckUpdate_Click(object sender, EventArgs e)
        {
            var updateService = Globals.ThisAddIn.ServiceProvider.GetService<IUpdateService>();
            var notification = Globals.ThisAddIn.ServiceProvider.GetService<IUpdateNotificationService>();
            if (updateService == null || notification == null) return;

            SetUpdateButtonsEnabled(false);
            try
            {
                var info = await updateService.CheckForUpdatesAsync();

                Properties.Settings.Default.LastUpdateCheck = DateTime.Now;
                if (!string.IsNullOrEmpty(info.LatestVersion))
                    Properties.Settings.Default.LatestVersion = info.LatestVersion;
                if (info.ReleaseDate != default(DateTime))
                    Properties.Settings.Default.LatestVersionReleaseDate = info.ReleaseDate;
                Properties.Settings.Default.Save();

                labelLastUpdateCheck.Text = Properties.Settings.Default.LastUpdateCheck.ToString("yyyy/MM/dd - HH:mm", DateTimeFormatInfo.InvariantInfo);
                if (!string.IsNullOrEmpty(info.LatestVersion))
                    UpdateVersionLabel(info.LatestVersion);
                if (info.ReleaseDate != default(DateTime))
                    UpdateReleaseDateLabel(info.ReleaseDate.ToString("yyyy/MM/dd - HH:mm", DateTimeFormatInfo.InvariantInfo));

                if (!info.IsUpdateAvailable)
                {
                    MessageBox.Show("You are using the latest version.", "No Updates Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Update available — show the toast. The toast's Install button hands
                // off to UpdateService.ApplyPendingUpdateAsync which uses ClickOnce-native
                // ApplicationDeployment.Update() to download + stage the new version.
                notification.ShowUpdateAvailable(info.LatestVersion, info.ReleaseNotes);
            }
            catch (Exception ex)
            {
                Deckify.Common.Logger.LogError(ex, "buttonCheckUpdate_Click");
                MessageBox.Show($"Update check failed:\n\n{ex.Message}\n\nSee log file for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUpdateButtonsEnabled(true);
            }
        }

        private void linkLabelMail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Send mail with Outlook
            string mailBody = "<br><br><br><br>---<br>Please do not modify this Information:<br>------------------------------------------------------------------------------------------------------------<br>Installed Version: " + ClickOnceVersionProvider.GetInstalledVersion() + "<br>Licence Key: " + DeckifySettings.Default.LicenseKey + "<br>------------------------------------------------------------------------------------------------------------<br><br>";
            string mailTo = "tillmannschatz@gmail.com";
            string mailSubject = "[deckify] PLEASE ADD PROPPER MESSAGE SUBJECT";

            var emailService = Globals.ThisAddIn.ServiceProvider.GetService<Deckify.Modules.Services.IEmailService>();
            emailService.CreateMail(mailTo: mailTo, mailSubject: mailSubject, mailBody: mailBody);
        }

        private async void buttonUpdate_Click(object sender, EventArgs e)
        {
            var updateService = Globals.ThisAddIn.ServiceProvider.GetService<IUpdateService>();
            var notification = Globals.ThisAddIn.ServiceProvider.GetService<IUpdateNotificationService>();
            if (updateService == null || notification == null) return;

            SetUpdateButtonsEnabled(false);
            try
            {
                var info = await updateService.CheckForUpdatesAsync();
                if (!info.IsUpdateAvailable)
                {
                    MessageBox.Show("You are already on the latest version.", "No Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Surface the toast — Install click triggers ClickOnce-native update via
                // UpdateService.ApplyPendingUpdateAsync, which calls ApplicationDeployment.Update().
                // No installer.exe download — Defender's per-build false-positive risk eliminated.
                notification.ShowUpdateAvailable(info.LatestVersion, info.ReleaseNotes);
            }
            catch (Exception ex)
            {
                Deckify.Common.Logger.LogError(ex, "buttonUpdate_Click");
                MessageBox.Show($"Update failed:\n\n{ex.Message}\n\nSee log file for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUpdateButtonsEnabled(true);
            }
        }

        private void SetUpdateButtonsEnabled(bool enabled)
        {
            buttonCheckUpdate.Enabled = enabled;
            buttonUpdate.Enabled = enabled;
        }

        public void UpdateVersionLabel(string text)
        {
            if (labelLatestReleasedVersion.InvokeRequired)
            {
                labelLatestReleasedVersion.Invoke((MethodInvoker)delegate
                {
                    labelLatestReleasedVersion.Text = text;
                });
            }
            else
            {
                labelLatestReleasedVersion.Text = text;
            }
        }
        public void UpdateReleaseDateLabel(string text)
        {
            if (labelLatestVersionReleaseDate.InvokeRequired)
            {
                labelLatestVersionReleaseDate.Invoke((MethodInvoker)delegate
                {
                    labelLatestVersionReleaseDate.Text = text;
                });
            }
            else
            {
                labelLatestVersionReleaseDate.Text = text;
            }
        }

    }
}
