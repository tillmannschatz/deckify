namespace Deckify
{
    partial class TaskPaneAbout
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
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelInstalledVersion = new System.Windows.Forms.Label();
            this.labelLatestReleasedVersion = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelLastUpdateCheck = new System.Windows.Forms.Label();
            this.buttonCheckUpdate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabelMail = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelLatestVersionReleaseDate = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelLicenceKeyValidity = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.labelLicenceKeyHolder = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.labelLicenceKey = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUpdate.Location = new System.Drawing.Point(117, 115);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(56, 21);
            this.buttonUpdate.TabIndex = 0;
            this.buttonUpdate.Text = "Update";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Installed version: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Latest version: ";
            // 
            // labelInstalledVersion
            // 
            this.labelInstalledVersion.AutoSize = true;
            this.labelInstalledVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstalledVersion.Location = new System.Drawing.Point(114, 27);
            this.labelInstalledVersion.Name = "labelInstalledVersion";
            this.labelInstalledVersion.Size = new System.Drawing.Size(327, 13);
            this.labelInstalledVersion.TabIndex = 3;
            this.labelInstalledVersion.Text = "SET CURRENTLY INSTALLED VERSION PROGRAMMATICALLY";
            // 
            // labelLatestReleasedVersion
            // 
            this.labelLatestReleasedVersion.AutoSize = true;
            this.labelLatestReleasedVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLatestReleasedVersion.Location = new System.Drawing.Point(114, 50);
            this.labelLatestReleasedVersion.Name = "labelLatestReleasedVersion";
            this.labelLatestReleasedVersion.Size = new System.Drawing.Size(360, 13);
            this.labelLatestReleasedVersion.TabIndex = 4;
            this.labelLatestReleasedVersion.Text = "SET LATEST RELEASED VERSION AVAILABLE PROGRAMMATICALLY";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Last Update Check:";
            // 
            // labelLastUpdateCheck
            // 
            this.labelLastUpdateCheck.AutoSize = true;
            this.labelLastUpdateCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLastUpdateCheck.Location = new System.Drawing.Point(114, 95);
            this.labelLastUpdateCheck.Name = "labelLastUpdateCheck";
            this.labelLastUpdateCheck.Size = new System.Drawing.Size(261, 13);
            this.labelLastUpdateCheck.TabIndex = 6;
            this.labelLastUpdateCheck.Text = "SET LAST UPDATE CHECK PROGRAMMATICALLY";
            // 
            // buttonCheckUpdate
            // 
            this.buttonCheckUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCheckUpdate.Location = new System.Drawing.Point(9, 115);
            this.buttonCheckUpdate.Name = "buttonCheckUpdate";
            this.buttonCheckUpdate.Size = new System.Drawing.Size(99, 23);
            this.buttonCheckUpdate.TabIndex = 7;
            this.buttonCheckUpdate.Text = "Check Update";
            this.buttonCheckUpdate.UseVisualStyleBackColor = true;
            this.buttonCheckUpdate.Click += new System.EventHandler(this.buttonCheckUpdate_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(123, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Tillmann Schatz";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(120, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "effensify";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(120, 120);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(116, 17);
            this.label7.TabIndex = 13;
            this.label7.Text = "Mühldorfstraße 8";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(120, 137);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 17);
            this.label8.TabIndex = 14;
            this.label8.Text = "81671 München";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(123, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(132, 17);
            this.label9.TabIndex = 15;
            this.label9.Text = "+49 151 590 57899";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.linkLabelMail);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(277, 164);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Author";
            // 
            // linkLabelMail
            // 
            this.linkLabelMail.AutoSize = true;
            this.linkLabelMail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelMail.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.linkLabelMail.Location = new System.Drawing.Point(120, 59);
            this.linkLabelMail.Name = "linkLabelMail";
            this.linkLabelMail.Size = new System.Drawing.Size(65, 17);
            this.linkLabelMail.TabIndex = 15;
            this.linkLabelMail.TabStop = true;
            this.linkLabelMail.Text = "Email me";
            this.linkLabelMail.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.linkLabelMail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelMail_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 60F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.YellowGreen;
            this.label6.Location = new System.Drawing.Point(107, -55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 91);
            this.label6.TabIndex = 16;
            this.label6.Text = "_";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelLatestVersionReleaseDate);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.buttonUpdate);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.buttonCheckUpdate);
            this.groupBox2.Controls.Add(this.labelInstalledVersion);
            this.groupBox2.Controls.Add(this.labelLastUpdateCheck);
            this.groupBox2.Controls.Add(this.labelLatestReleasedVersion);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(3, 172);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(277, 144);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "deckify - PowerPoint Add-in";
            // 
            // labelLatestVersionReleaseDate
            // 
            this.labelLatestVersionReleaseDate.AutoSize = true;
            this.labelLatestVersionReleaseDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLatestVersionReleaseDate.Location = new System.Drawing.Point(114, 63);
            this.labelLatestVersionReleaseDate.Name = "labelLatestVersionReleaseDate";
            this.labelLatestVersionReleaseDate.Size = new System.Drawing.Size(211, 13);
            this.labelLatestVersionReleaseDate.TabIndex = 8;
            this.labelLatestVersionReleaseDate.Text = "SET RELEASE DATE PROGRAMATICALLY";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelLicenceKeyValidity);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.labelLicenceKeyHolder);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.labelLicenceKey);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(3, 322);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(277, 131);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "License";
            // 
            // labelLicenceKeyValidity
            // 
            this.labelLicenceKeyValidity.AutoSize = true;
            this.labelLicenceKeyValidity.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLicenceKeyValidity.Location = new System.Drawing.Point(6, 94);
            this.labelLicenceKeyValidity.Name = "labelLicenceKeyValidity";
            this.labelLicenceKeyValidity.Size = new System.Drawing.Size(268, 13);
            this.labelLicenceKeyValidity.TabIndex = 5;
            this.labelLicenceKeyValidity.Text = "SET LICENCE KEY VALIDITY PROGRAMMATICALLY";
            this.labelLicenceKeyValidity.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(6, 81);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 4;
            this.label15.Text = "Key Validity";
            // 
            // labelLicenceKeyHolder
            // 
            this.labelLicenceKeyHolder.AutoSize = true;
            this.labelLicenceKeyHolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLicenceKeyHolder.Location = new System.Drawing.Point(6, 62);
            this.labelLicenceKeyHolder.Name = "labelLicenceKeyHolder";
            this.labelLicenceKeyHolder.Size = new System.Drawing.Size(265, 13);
            this.labelLicenceKeyHolder.TabIndex = 3;
            this.labelLicenceKeyHolder.Text = "SET LICENCE KEY HOLDER PROGRAMMATICALLY";
            this.labelLicenceKeyHolder.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(6, 49);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(58, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Key Holder";
            // 
            // labelLicenceKey
            // 
            this.labelLicenceKey.AutoSize = true;
            this.labelLicenceKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLicenceKey.Location = new System.Drawing.Point(6, 29);
            this.labelLicenceKey.Name = "labelLicenceKey";
            this.labelLicenceKey.Size = new System.Drawing.Size(217, 13);
            this.labelLicenceKey.TabIndex = 1;
            this.labelLicenceKey.Text = "SET LICENCE KEY PROGRAMMATICALLY";
            this.labelLicenceKey.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(25, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Key";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Deckify.Properties.Resources.mews_logo;
            this.pictureBox2.Location = new System.Drawing.Point(18, 114);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(99, 37);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 12;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Deckify.Properties.Resources.author;
            this.pictureBox1.InitialImage = global::Deckify.Properties.Resources.author;
            this.pictureBox1.Location = new System.Drawing.Point(15, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(99, 66);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // TaskPaneAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Name = "TaskPaneAbout";
            this.Size = new System.Drawing.Size(723, 558);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelInstalledVersion;
        private System.Windows.Forms.Label labelLatestReleasedVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelLastUpdateCheck;
        private System.Windows.Forms.Button buttonCheckUpdate;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel linkLabelMail;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelLatestVersionReleaseDate;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelLicenceKeyValidity;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label labelLicenceKeyHolder;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label labelLicenceKey;
        private System.Windows.Forms.Label label10;
    }
}
