namespace Deckify
{
    partial class TaskPaneScreenshot
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
            this.listBoxWindowSelector = new System.Windows.Forms.ListBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblOpenWindows = new System.Windows.Forms.Label();
            this.btnSetupRegion = new System.Windows.Forms.Button();
            this.btnTakeWindowScreenshot = new System.Windows.Forms.Button();
            this.btnTakeAreaScreenshot = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxWindowSelector
            // 
            this.listBoxWindowSelector.FormattingEnabled = true;
            this.listBoxWindowSelector.HorizontalScrollbar = true;
            this.listBoxWindowSelector.Location = new System.Drawing.Point(13, 94);
            this.listBoxWindowSelector.Name = "listBoxWindowSelector";
            this.listBoxWindowSelector.Size = new System.Drawing.Size(240, 485);
            this.listBoxWindowSelector.TabIndex = 0;
            this.listBoxWindowSelector.SelectedIndexChanged += new System.EventHandler(this.listBoxWindowSelector_SelectedIndexChanged);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(13, 52);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(240, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh Window List";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblOpenWindows
            // 
            this.lblOpenWindows.AutoSize = true;
            this.lblOpenWindows.Location = new System.Drawing.Point(10, 78);
            this.lblOpenWindows.Name = "lblOpenWindows";
            this.lblOpenWindows.Size = new System.Drawing.Size(223, 13);
            this.lblOpenWindows.TabIndex = 2;
            this.lblOpenWindows.Text = "Select the Window to take a screenshot from ";
            // 
            // btnSetupRegion
            // 
            this.btnSetupRegion.Location = new System.Drawing.Point(13, 651);
            this.btnSetupRegion.Name = "btnSetupRegion";
            this.btnSetupRegion.Size = new System.Drawing.Size(240, 23);
            this.btnSetupRegion.TabIndex = 3;
            this.btnSetupRegion.Text = "Setup Screenshot area";
            this.btnSetupRegion.UseVisualStyleBackColor = true;
            this.btnSetupRegion.Click += new System.EventHandler(this.btnSetupRegion_Click);
            // 
            // btnTakeWindowScreenshot
            // 
            this.btnTakeWindowScreenshot.Location = new System.Drawing.Point(13, 23);
            this.btnTakeWindowScreenshot.Name = "btnTakeWindowScreenshot";
            this.btnTakeWindowScreenshot.Size = new System.Drawing.Size(240, 23);
            this.btnTakeWindowScreenshot.TabIndex = 4;
            this.btnTakeWindowScreenshot.Text = "Take screenshot from selected window";
            this.btnTakeWindowScreenshot.UseVisualStyleBackColor = true;
            this.btnTakeWindowScreenshot.Click += new System.EventHandler(this.btnTakeScreenshot_Click);
            // 
            // btnTakeAreaScreenshot
            // 
            this.btnTakeAreaScreenshot.Location = new System.Drawing.Point(13, 622);
            this.btnTakeAreaScreenshot.Name = "btnTakeAreaScreenshot";
            this.btnTakeAreaScreenshot.Size = new System.Drawing.Size(240, 23);
            this.btnTakeAreaScreenshot.TabIndex = 5;
            this.btnTakeAreaScreenshot.Text = "Take screenshot from selected area";
            this.btnTakeAreaScreenshot.UseVisualStyleBackColor = true;
            this.btnTakeAreaScreenshot.Click += new System.EventHandler(this.btnTakeAreaScreenshot_Click);
            // 
            // TaskPaneScreenshot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnTakeAreaScreenshot);
            this.Controls.Add(this.btnTakeWindowScreenshot);
            this.Controls.Add(this.btnSetupRegion);
            this.Controls.Add(this.lblOpenWindows);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.listBoxWindowSelector);
            this.Name = "TaskPaneScreenshot";
            this.Size = new System.Drawing.Size(359, 677);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxWindowSelector;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblOpenWindows;
        private System.Windows.Forms.Button btnSetupRegion;
        private System.Windows.Forms.Button btnTakeWindowScreenshot;
        private System.Windows.Forms.Button btnTakeAreaScreenshot;
    }
}
