using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deckify
{
    public partial class TaskPaneSettings : UserControl
    {
        public TaskPaneSettings()
        {
            InitializeComponent();

            // Set Values for General - PowerPoint
            comboBoxDefaultTab.Text = "deckify";
            checkBoxAutoFocus.Checked = DeckifySettings.Default.AutoFocus;
            checkBoxDynamicUI.Checked = DeckifySettings.Default.DynamicUI;

            // Set Values for General - General
            checkBoxAutoUpdates.Checked = DeckifySettings.Default.AutoCheckUpdate;
            checkBoxUsageStatistics.Checked = DeckifySettings.Default.SendUsageStatistics;
            checkBoxDevMode.Checked = DeckifySettings.Default.DevMode;
            textBoxMDPath.Text = DeckifySettings.Default.MDPath;

            // Set Values for License
            if (DeckifySettings.Default.DevMode)
            {
                textBoxLicenseEmail.Visible = true;
                textBoxLicenseKey.Visible = true;
                buttonLicenseSave.Visible = true;

                textBoxLicenseEmail.Text = DeckifySettings.Default.LicenseKeyEmail;
                textBoxLicenseKey.Text = DeckifySettings.Default.LicenseKey;
            }
            else
            {
                textBoxLicenseEmail.Visible = false;
                textBoxLicenseKey.Visible = false;
                buttonLicenseSave.Visible = false;
            }
        }

        private void buttonLicenseSave_Click(object sender, EventArgs e)
        {
            DeckifySettings.Default.LicenseKeyEmail = textBoxLicenseEmail.Text;
            DeckifySettings.Default.LicenseKey = textBoxLicenseKey.Text;
            DeckifySettings.Default.Save();
        }

        private void buttonFolderBrowser_Click(object sender, EventArgs e)
        {
            // Create directory if it does not exist yet
            string[] paths = { Environment.GetFolderPath(Environment.SpecialFolder.Personal), "deckify" };
            Directory.CreateDirectory(Path.Combine(paths));
            this.folderBrowserDialogPluginPath.SelectedPath = Path.Combine(paths);
            this.folderBrowserDialogPluginPath.ShowNewFolderButton = true;
            DialogResult result = this.folderBrowserDialogPluginPath.ShowDialog();

            // handle user action (OK / Cancel)
            if (result == DialogResult.OK)
            {
                textBoxMDPath.Text = folderBrowserDialogPluginPath.SelectedPath;
                DeckifySettings.Default.MDPath = textBoxMDPath.Text;
                DeckifySettings.Default.Save();
            }
        }

        private void checkBoxAutoFocus_CheckedChanged(object sender, EventArgs e)
        {
            DeckifySettings.Default.AutoFocus = checkBoxAutoFocus.Checked;
            DeckifySettings.Default.Save();
            if (DeckifySettings.Default.AutoFocus) { Globals.ThisAddIn._ribbonUI.ActivateTab(Ribbon.ActiveTabId); } else { Globals.ThisAddIn._ribbonUI.ActivateTabMso("TabHome"); }
            if (DeckifySettings.Default.AutoFocus) { checkBoxDynamicUI.Enabled = true; } else { checkBoxDynamicUI.Enabled = false; }
        }

        private void checkBoxDynamicUI_CheckedChanged(object sender, EventArgs e)
        {
            DeckifySettings.Default.DynamicUI = checkBoxDynamicUI.Checked;
            DeckifySettings.Default.Save();
            Globals.ThisAddIn.DynamicGroupActivateDeactivate();
        }

        private void checkBoxAutoUpdates_CheckedChanged(object sender, EventArgs e)
        {
            DeckifySettings.Default.AutoCheckUpdate = checkBoxAutoUpdates.Checked;
            DeckifySettings.Default.Save();
        }

        private void checkBoxUsageStatistics_CheckedChanged(object sender, EventArgs e)
        {
            DeckifySettings.Default.SendUsageStatistics = checkBoxUsageStatistics.Checked;
            DeckifySettings.Default.Save();
        }

        private void checkBoxDevMode_CheckedChanged(object sender, EventArgs e)
        {
            DeckifySettings.Default.DevMode = checkBoxDevMode.Checked;
            DeckifySettings.Default.Save();
            Globals.ThisAddIn._ribbonUI.Invalidate();
        }
    }
}
