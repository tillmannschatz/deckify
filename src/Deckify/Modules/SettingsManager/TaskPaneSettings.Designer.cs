namespace Deckify
{
    partial class TaskPaneSettings
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TabControlSettings = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxMDPath = new System.Windows.Forms.TextBox();
            this.buttonFolderBrowser = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxDevMode = new System.Windows.Forms.CheckBox();
            this.checkBoxUsageStatistics = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoUpdates = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxDynamicUI = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoFocus = new System.Windows.Forms.CheckBox();
            this.comboBoxDefaultTab = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonLicenseSave = new System.Windows.Forms.Button();
            this.textBoxLicenseKey = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxLicenseEmail = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.folderBrowserDialogPluginPath = new System.Windows.Forms.FolderBrowserDialog();
            this.TabControlSettings.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControlSettings
            // 
            this.TabControlSettings.Controls.Add(this.tabPage1);
            this.TabControlSettings.Controls.Add(this.tabPage2);
            this.TabControlSettings.Location = new System.Drawing.Point(3, 3);
            this.TabControlSettings.Name = "TabControlSettings";
            this.TabControlSettings.SelectedIndex = 0;
            this.TabControlSettings.Size = new System.Drawing.Size(279, 345);
            this.TabControlSettings.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(271, 319);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxMDPath);
            this.groupBox2.Controls.Add(this.buttonFolderBrowser);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.checkBoxDevMode);
            this.groupBox2.Controls.Add(this.checkBoxUsageStatistics);
            this.groupBox2.Controls.Add(this.checkBoxAutoUpdates);
            this.groupBox2.Location = new System.Drawing.Point(6, 148);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(259, 166);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // textBoxMDPath
            // 
            this.textBoxMDPath.Enabled = false;
            this.textBoxMDPath.Location = new System.Drawing.Point(9, 123);
            this.textBoxMDPath.Name = "textBoxMDPath";
            this.textBoxMDPath.Size = new System.Drawing.Size(146, 20);
            this.textBoxMDPath.TabIndex = 10;
            // 
            // buttonFolderBrowser
            // 
            this.buttonFolderBrowser.Location = new System.Drawing.Point(158, 121);
            this.buttonFolderBrowser.Name = "buttonFolderBrowser";
            this.buttonFolderBrowser.Size = new System.Drawing.Size(34, 23);
            this.buttonFolderBrowser.TabIndex = 9;
            this.buttonFolderBrowser.Text = "...";
            this.buttonFolderBrowser.UseVisualStyleBackColor = true;
            this.buttonFolderBrowser.Click += new System.EventHandler(this.buttonFolderBrowser_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "deckify Path";
            // 
            // checkBoxDevMode
            // 
            this.checkBoxDevMode.AutoSize = true;
            this.checkBoxDevMode.Location = new System.Drawing.Point(9, 77);
            this.checkBoxDevMode.Name = "checkBoxDevMode";
            this.checkBoxDevMode.Size = new System.Drawing.Size(138, 17);
            this.checkBoxDevMode.TabIndex = 6;
            this.checkBoxDevMode.Text = "Use development mode";
            this.checkBoxDevMode.UseVisualStyleBackColor = true;
            this.checkBoxDevMode.CheckedChanged += new System.EventHandler(this.checkBoxDevMode_CheckedChanged);
            // 
            // checkBoxUsageStatistics
            // 
            this.checkBoxUsageStatistics.AutoSize = true;
            this.checkBoxUsageStatistics.Enabled = false;
            this.checkBoxUsageStatistics.Location = new System.Drawing.Point(9, 54);
            this.checkBoxUsageStatistics.Name = "checkBoxUsageStatistics";
            this.checkBoxUsageStatistics.Size = new System.Drawing.Size(183, 17);
            this.checkBoxUsageStatistics.TabIndex = 5;
            this.checkBoxUsageStatistics.Text = "Send anonymous usage statistics";
            this.checkBoxUsageStatistics.UseVisualStyleBackColor = true;
            this.checkBoxUsageStatistics.CheckedChanged += new System.EventHandler(this.checkBoxUsageStatistics_CheckedChanged);
            // 
            // checkBoxAutoUpdates
            // 
            this.checkBoxAutoUpdates.AutoSize = true;
            this.checkBoxAutoUpdates.Location = new System.Drawing.Point(9, 31);
            this.checkBoxAutoUpdates.Name = "checkBoxAutoUpdates";
            this.checkBoxAutoUpdates.Size = new System.Drawing.Size(178, 17);
            this.checkBoxAutoUpdates.TabIndex = 4;
            this.checkBoxAutoUpdates.Text = "Automatically Check for updates";
            this.checkBoxAutoUpdates.UseVisualStyleBackColor = true;
            this.checkBoxAutoUpdates.CheckedChanged += new System.EventHandler(this.checkBoxAutoUpdates_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxDynamicUI);
            this.groupBox1.Controls.Add(this.checkBoxAutoFocus);
            this.groupBox1.Controls.Add(this.comboBoxDefaultTab);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(259, 136);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PowerPoint";
            // 
            // checkBoxDynamicUI
            // 
            this.checkBoxDynamicUI.AutoSize = true;
            this.checkBoxDynamicUI.Checked = true;
            this.checkBoxDynamicUI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDynamicUI.Location = new System.Drawing.Point(9, 101);
            this.checkBoxDynamicUI.Name = "checkBoxDynamicUI";
            this.checkBoxDynamicUI.Size = new System.Drawing.Size(182, 17);
            this.checkBoxDynamicUI.TabIndex = 4;
            this.checkBoxDynamicUI.Text = "Selection dependent Dynamic UI";
            this.checkBoxDynamicUI.UseVisualStyleBackColor = true;
            this.checkBoxDynamicUI.CheckedChanged += new System.EventHandler(this.checkBoxDynamicUI_CheckedChanged);
            // 
            // checkBoxAutoFocus
            // 
            this.checkBoxAutoFocus.AutoSize = true;
            this.checkBoxAutoFocus.Checked = true;
            this.checkBoxAutoFocus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoFocus.Location = new System.Drawing.Point(9, 78);
            this.checkBoxAutoFocus.Name = "checkBoxAutoFocus";
            this.checkBoxAutoFocus.Size = new System.Drawing.Size(146, 17);
            this.checkBoxAutoFocus.TabIndex = 3;
            this.checkBoxAutoFocus.Text = "Autofocus on default Tab";
            this.checkBoxAutoFocus.UseVisualStyleBackColor = true;
            this.checkBoxAutoFocus.CheckedChanged += new System.EventHandler(this.checkBoxAutoFocus_CheckedChanged);
            // 
            // comboBoxDefaultTab
            // 
            this.comboBoxDefaultTab.Enabled = false;
            this.comboBoxDefaultTab.FormattingEnabled = true;
            this.comboBoxDefaultTab.Location = new System.Drawing.Point(9, 41);
            this.comboBoxDefaultTab.Name = "comboBoxDefaultTab";
            this.comboBoxDefaultTab.Size = new System.Drawing.Size(121, 21);
            this.comboBoxDefaultTab.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Default Tab";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.buttonLicenseSave);
            this.tabPage2.Controls.Add(this.textBoxLicenseKey);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.textBoxLicenseEmail);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(271, 319);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "License";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonLicenseSave
            // 
            this.buttonLicenseSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLicenseSave.Location = new System.Drawing.Point(163, 114);
            this.buttonLicenseSave.Name = "buttonLicenseSave";
            this.buttonLicenseSave.Size = new System.Drawing.Size(75, 23);
            this.buttonLicenseSave.TabIndex = 4;
            this.buttonLicenseSave.Text = "Save";
            this.buttonLicenseSave.UseVisualStyleBackColor = true;
            this.buttonLicenseSave.Click += new System.EventHandler(this.buttonLicenseSave_Click);
            // 
            // textBoxLicenseKey
            // 
            this.textBoxLicenseKey.Location = new System.Drawing.Point(9, 77);
            this.textBoxLicenseKey.Name = "textBoxLicenseKey";
            this.textBoxLicenseKey.Size = new System.Drawing.Size(229, 20);
            this.textBoxLicenseKey.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "License key";
            // 
            // textBoxLicenseEmail
            // 
            this.textBoxLicenseEmail.Location = new System.Drawing.Point(9, 29);
            this.textBoxLicenseEmail.Name = "textBoxLicenseEmail";
            this.textBoxLicenseEmail.Size = new System.Drawing.Size(229, 20);
            this.textBoxLicenseEmail.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Email";
            // 
            // TaskPaneSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TabControlSettings);
            this.Name = "TaskPaneSettings";
            this.Size = new System.Drawing.Size(604, 642);
            this.TabControlSettings.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TabControlSettings;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxMDPath;
        private System.Windows.Forms.Button buttonFolderBrowser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxDevMode;
        private System.Windows.Forms.CheckBox checkBoxUsageStatistics;
        private System.Windows.Forms.CheckBox checkBoxAutoUpdates;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxDynamicUI;
        private System.Windows.Forms.CheckBox checkBoxAutoFocus;
        private System.Windows.Forms.ComboBox comboBoxDefaultTab;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogPluginPath;
        private System.Windows.Forms.TextBox textBoxLicenseKey;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxLicenseEmail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonLicenseSave;
        private System.Windows.Forms.TabPage tabPage2;
    }
}
