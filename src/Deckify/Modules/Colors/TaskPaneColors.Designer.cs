using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Deckify.Modules.Colors
{
    partial class TaskPaneColors
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.groupBoxAdd = new System.Windows.Forms.GroupBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnPickColor = new System.Windows.Forms.Button();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblPreview = new System.Windows.Forms.Label();
            this.lvColors = new System.Windows.Forms.ListView();
            this.cmbGroup = new System.Windows.Forms.ComboBox();
            this.groupBoxApply = new System.Windows.Forms.GroupBox();
            this.btnApplyText = new System.Windows.Forms.Button();
            this.btnApplyOutline = new System.Windows.Forms.Button();
            this.btnApplyFill = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.groupBoxAdd.SuspendLayout();
            this.groupBoxApply.SuspendLayout();
            this.SuspendLayout();
            this.txtHex = new System.Windows.Forms.TextBox();
            this.btnEyedropper = new System.Windows.Forms.Button();
            // 
            // groupBoxAdd
            // 
            this.groupBoxAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAdd.Controls.Add(this.btnEyedropper);
            this.groupBoxAdd.Controls.Add(this.txtHex);
            this.groupBoxAdd.Controls.Add(this.lblPreview);
            this.groupBoxAdd.Controls.Add(this.btnAdd);
            this.groupBoxAdd.Controls.Add(this.btnPickColor);
            this.groupBoxAdd.Controls.Add(this.txtName);
            this.groupBoxAdd.Controls.Add(this.cmbGroup);
            this.groupBoxAdd.Location = new System.Drawing.Point(3, 3);
            this.groupBoxAdd.Name = "groupBoxAdd";
            this.groupBoxAdd.Size = new System.Drawing.Size(294, 135);
            this.groupBoxAdd.TabIndex = 0;
            this.groupBoxAdd.TabStop = false;
            this.groupBoxAdd.Text = "Add New Color";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(6, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(282, 20);
            this.txtName.TabIndex = 0;
            this.txtName.Text = "Color Name";
            // 
            // cmbGroup
            // 
            this.cmbGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbGroup.Location = new System.Drawing.Point(6, 45);
            this.cmbGroup.Name = "cmbGroup";
            this.cmbGroup.Size = new System.Drawing.Size(282, 21);
            this.cmbGroup.TabIndex = 1;
            this.cmbGroup.Text = "Default";
            // 
            // btnPickColor
            // 
            this.btnPickColor.Location = new System.Drawing.Point(6, 75);
            this.btnPickColor.Name = "btnPickColor";
            this.btnPickColor.Size = new System.Drawing.Size(75, 23);
            this.btnPickColor.TabIndex = 2;
            this.btnPickColor.Text = "Pick Color...";
            this.btnPickColor.UseVisualStyleBackColor = true;
            this.btnPickColor.Click += new System.EventHandler(this.btnPickColor_Click);
            // 
            // lblPreview
            // 
            this.lblPreview.BackColor = System.Drawing.Color.Gray;
            this.lblPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPreview.Location = new System.Drawing.Point(87, 75);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(23, 23);
            this.lblPreview.TabIndex = 3;
            // 
            // txtHex
            // 
            this.txtHex.Location = new System.Drawing.Point(116, 77);
            this.txtHex.Name = "txtHex";
            this.txtHex.Size = new System.Drawing.Size(80, 20);
            this.txtHex.TabIndex = 4;
            this.txtHex.Text = "#808080";
            this.txtHex.TextChanged += new System.EventHandler(this.txtHex_TextChanged);
            // 
            // btnEyedropper
            // 
            this.btnEyedropper.Location = new System.Drawing.Point(6, 104);
            this.btnEyedropper.Name = "btnEyedropper";
            this.btnEyedropper.Size = new System.Drawing.Size(75, 23);
            this.btnEyedropper.TabIndex = 5;
            this.btnEyedropper.Text = "Eyedropper";
            this.btnEyedropper.UseVisualStyleBackColor = true;
            this.btnEyedropper.Click += new System.EventHandler(this.btnEyedropper_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(213, 75);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 52);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lvColors
            // 
            this.lvColors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvColors.Location = new System.Drawing.Point(3, 144);
            this.lvColors.Name = "lvColors";
            this.lvColors.Size = new System.Drawing.Size(294, 216);
            this.lvColors.TabIndex = 7;
            this.lvColors.UseCompatibleStateImageBehavior = false;
            this.lvColors.View = System.Windows.Forms.View.Tile;
            this.lvColors.SelectedIndexChanged += new System.EventHandler(this.lvColors_SelectedIndexChanged);
            // 
            // groupBoxApply
            // 
            this.groupBoxApply.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxApply.Controls.Add(this.btnApplyText);
            this.groupBoxApply.Controls.Add(this.btnApplyOutline);
            this.groupBoxApply.Controls.Add(this.btnApplyFill);
            this.groupBoxApply.Location = new System.Drawing.Point(3, 395);
            this.groupBoxApply.Name = "groupBoxApply";
            this.groupBoxApply.Size = new System.Drawing.Size(294, 105);
            this.groupBoxApply.TabIndex = 8;
            this.groupBoxApply.TabStop = false;
            this.groupBoxApply.Text = "Apply to Selection";
            // 
            // btnApplyFill
            // 
            this.btnApplyFill.Location = new System.Drawing.Point(6, 19);
            this.btnApplyFill.Name = "btnApplyFill";
            this.btnApplyFill.Size = new System.Drawing.Size(282, 23);
            this.btnApplyFill.TabIndex = 0;
            this.btnApplyFill.Text = "Set Fill Color";
            this.btnApplyFill.UseVisualStyleBackColor = true;
            this.btnApplyFill.Click += new System.EventHandler(this.btnApplyFill_Click);
            // 
            // btnApplyOutline
            // 
            this.btnApplyOutline.Location = new System.Drawing.Point(6, 48);
            this.btnApplyOutline.Name = "btnApplyOutline";
            this.btnApplyOutline.Size = new System.Drawing.Size(282, 23);
            this.btnApplyOutline.TabIndex = 1;
            this.btnApplyOutline.Text = "Set Outline Color";
            this.btnApplyOutline.UseVisualStyleBackColor = true;
            this.btnApplyOutline.Click += new System.EventHandler(this.btnApplyOutline_Click);
            // 
            // btnApplyText
            // 
            this.btnApplyText.Location = new System.Drawing.Point(6, 77);
            this.btnApplyText.Name = "btnApplyText";
            this.btnApplyText.Size = new System.Drawing.Size(282, 23);
            this.btnApplyText.TabIndex = 2;
            this.btnApplyText.Text = "Set Text Color";
            this.btnApplyText.UseVisualStyleBackColor = true;
            this.btnApplyText.Click += new System.EventHandler(this.btnApplyText_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(3, 366);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(294, 23);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Text = "Delete Selected";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // TaskPaneColors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.groupBoxApply);
            this.Controls.Add(this.lvColors);
            this.Controls.Add(this.groupBoxAdd);
            this.Name = "TaskPaneColors";
            this.Size = new System.Drawing.Size(300, 503);
            this.groupBoxAdd.ResumeLayout(false);
            this.groupBoxAdd.PerformLayout();
            this.groupBoxApply.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxAdd;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnPickColor;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.ListView lvColors;
        private System.Windows.Forms.GroupBox groupBoxApply;
        private System.Windows.Forms.Button btnApplyText;
        private System.Windows.Forms.Button btnApplyOutline;
        private System.Windows.Forms.Button btnApplyFill;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.TextBox txtHex;
        private System.Windows.Forms.Button btnEyedropper;
        private System.Windows.Forms.ComboBox cmbGroup;
    }
}
