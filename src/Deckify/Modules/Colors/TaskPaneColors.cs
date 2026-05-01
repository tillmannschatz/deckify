using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Linq;
using Deckify.Common;

namespace Deckify.Modules.Colors
{
    public partial class TaskPaneColors : UserControl
    {
        private Color _selectedColor = Color.Gray;
        private IPaletteService _paletteService;

        public TaskPaneColors()
        {
            InitializeComponent();
            
            // Configure ListView
            lvColors.View = View.LargeIcon;
            lvColors.LargeImageList = new ImageList();
            lvColors.LargeImageList.ImageSize = new Size(32, 32); 
            lvColors.ShowGroups = true;

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                // Delay loading service until runtime to avoid designer errors
                this.Load += TaskPaneColors_Load;
            }
        }

        private void TaskPaneColors_Load(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.ServiceProvider != null)
            {
                _paletteService = Globals.ThisAddIn.ServiceProvider.GetService<IPaletteService>();
                RefreshList();
            }
        }

        private void RefreshList()
        {
            lvColors.Items.Clear();
            lvColors.Groups.Clear();
            if (lvColors.LargeImageList != null) lvColors.LargeImageList.Images.Clear();
            
            if (_paletteService == null) return;

            // Gather Unique Groups
            var colors = _paletteService.GetPalette();
            var groups = colors.Select(c => c.Group ?? "Default").Distinct().OrderBy(g => g).ToList();

            // Create ListViewGroups
            foreach (var g in groups)
            {
                lvColors.Groups.Add(new ListViewGroup(g, g)); // Key, Header
            }

            // Populate ComboBox
            cmbGroup.Items.Clear();
            cmbGroup.Items.AddRange(groups.ToArray());
            if (!cmbGroup.Items.Contains("Default")) cmbGroup.Items.Add("Default");

            foreach (var color in colors)
            {
                // Create Swatch
                try 
                {
                    Color c = ColorTranslator.FromHtml(color.HexCode);
                    Bitmap bmp = CreateSolidColorBitmap(c, 32, 32);
                    lvColors.LargeImageList.Images.Add(color.HexCode, bmp); // Key is Hex
                }
                catch
                {
                    // Fallback
                    lvColors.LargeImageList.Images.Add(color.HexCode, CreateSolidColorBitmap(Color.Gray, 32, 32));
                }

                var item = new ListViewItem(color.Name);
                item.Tag = color; // Store model in Tag
                item.ImageKey = color.HexCode; // Link to ImageList
                
                string groupName = color.Group ?? "Default";
                item.Group = lvColors.Groups[groupName];
                
                lvColors.Items.Add(item);
            }
        }

        private Bitmap CreateSolidColorBitmap(Color color, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillRectangle(brush, 0, 0, width, height);
                }
                g.DrawRectangle(Pens.Black, 0, 0, width - 1, height - 1);
            }
            return bmp;
        }

        private void btnPickColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                UpdateSelectedColor(colorDialog1.Color);
            }
        }

        private void UpdateSelectedColor(Color color)
        {
            _selectedColor = color;
            lblPreview.BackColor = color;
            
            // Update Hex Text without triggering event (remove handler temporarily or check focus)
            // safer to just update and handle re-entry in event
            string hex = ColorTranslator.ToHtml(color);
            if (txtHex.Text != hex)
            {
               txtHex.Text = hex;
            }
        }

        private void txtHex_TextChanged(object sender, EventArgs e)
        {
            string text = txtHex.Text;
            if (!text.StartsWith("#")) text = "#" + text;

            try
            {
                Color c = ColorTranslator.FromHtml(text);
                // Valid color
                _selectedColor = c;
                lblPreview.BackColor = c;
                txtHex.ForeColor = Color.Black;
            }
            catch
            {
                // Invalid
                txtHex.ForeColor = Color.Red;
            }
        }

        private void btnEyedropper_Click(object sender, EventArgs e)
        {
            using (var overlay = new EyedropperOverlay())
            {
                if (overlay.ShowDialog() == DialogResult.OK)
                {
                    UpdateSelectedColor(overlay.SelectedColor);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a name.");
                return;
            }

            string group = cmbGroup.Text.Trim();
            if (string.IsNullOrEmpty(group)) group = "Default";

            string hex = ColorTranslator.ToHtml(_selectedColor);
            var newColor = new ColorModel(txtName.Text, hex, group);
            _paletteService.AddColor(newColor);
            RefreshList();
            txtName.Text = "Color Name";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvColors.SelectedItems.Count > 0)
            {
                var selectedItem = lvColors.SelectedItems[0];
                if (selectedItem.Tag is ColorModel model)
                {
                    _paletteService.RemoveColor(model);
                    RefreshList();
                }
            }
        }

        private void lvColors_SelectedIndexChanged(object sender, EventArgs e)
        {
             if (lvColors.SelectedItems.Count > 0)
             {
                 var selectedItem = lvColors.SelectedItems[0];
                 if (selectedItem.Tag is ColorModel selected)
                 {
                     try 
                     {
                        Color c = ColorTranslator.FromHtml(selected.HexCode);
                        UpdateSelectedColor(c);
                        txtName.Text = selected.Name;
                        cmbGroup.Text = selected.Group ?? "Default";
                     }
                     catch {}
                 }
             }
        }

        #region Application Logic
        
        private int GetColorInt(ColorModel model)
        {
            Color c = ColorTranslator.FromHtml(model.HexCode);
            return ColorTranslator.ToOle(c);
        }

        private void ApplyColor(Action<PowerPoint.ShapeRange, int> applyAction)
        {
            if (lvColors.SelectedItems.Count == 0)
            {
                // If no list item selected, try using the current "New Color" preview?
                // Standard behavior is to apply the *selected* color from list.
                // But maybe user wants to apply the color they just picked without adding?
                // Let's stick to list selection for now to match prompt "select color from list".
                // Or: If list has no selection, use _selectedColor? Let's assume list selection is required.
                MessageBox.Show("Please select a color from the list.");
                return;
            }
            
            var selectedModel = lvColors.SelectedItems[0].Tag as ColorModel;
            if (selectedModel == null) return;

            int colorOle = GetColorInt(selectedModel);

            ErrorHandler.SafeExecute(() =>
            {
                var selection = Globals.ThisAddIn.Application.ActiveWindow.Selection;
                if (selection.Type == PowerPoint.PpSelectionType.ppSelectionShapes || 
                    selection.Type == PowerPoint.PpSelectionType.ppSelectionText)
                {
                     // Use SystemUtilities to batch updates (disable screen updating)
                     SystemUtilities.RunBatchOperation(Globals.ThisAddIn.Application, () => 
                     {
                         if(selection.ShapeRange.Count > 0)
                         {
                             applyAction(selection.ShapeRange, colorOle);
                         }
                     });
                }
            }, "Apply Color");
        }


        private void btnApplyFill_Click(object sender, EventArgs e)
        {
            ApplyColor((shapes, color) => 
            {
                shapes.Fill.ForeColor.RGB = color;
                shapes.Fill.Visible = MsoTriState.msoTrue;
            });
        }

        private void btnApplyOutline_Click(object sender, EventArgs e)
        {
             ApplyColor((shapes, color) => 
            {
                shapes.Line.ForeColor.RGB = color;
                shapes.Line.Visible = MsoTriState.msoTrue;
            });
        }

        private void btnApplyText_Click(object sender, EventArgs e)
        {
             ApplyColor((shapes, color) => 
            {
                // Applying to ShapeRange text sets it for all text in shape
                 if (shapes.HasTextFrame == MsoTriState.msoTrue)
                 {
                     shapes.TextFrame.TextRange.Font.Color.RGB = color;
                 }
            });
        }
        #endregion

        // Inner class for Eyedropper Overlay
        private class EyedropperOverlay : Form
        {
            public Color SelectedColor { get; private set; }
            private Bitmap _screenCapture;

            public EyedropperOverlay()
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = true;
                this.Cursor = Cursors.Cross;
                this.ShowInTaskbar = false;
                this.DoubleBuffered = true;

                CaptureScreen();
            }

            private void CaptureScreen()
            {
                // Capture entire virtual screen
                Rectangle bounds = SystemInformation.VirtualScreen;
                _screenCapture = new Bitmap(bounds.Width, bounds.Height);
                using (Graphics g = Graphics.FromImage(_screenCapture))
                {
                    g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);
                }
                this.BackgroundImage = _screenCapture;
                // Offset form if VirtualScreen doesn't start at 0,0
                this.Location = bounds.Location; 
                this.Size = bounds.Size; // Ensure size matches
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                // Get color at mouse position relative to image
                // Form is fullscreen, so e.Location is screen location (relative to form top-left)
                // If generic multi-monitor setup, Form (0,0) is VirtualScreen (0,0)
                if (_screenCapture != null && e.X >= 0 && e.X < _screenCapture.Width && e.Y >= 0 && e.Y < _screenCapture.Height)
                {
                    // Optional: Show a magnified tooltip?
                }
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                {
                    if (_screenCapture != null && e.X >= 0 && e.X < _screenCapture.Width && e.Y >= 0 && e.Y < _screenCapture.Height)
                    {
                        SelectedColor = _screenCapture.GetPixel(e.X, e.Y);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);
                if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _screenCapture?.Dispose();
                }
                base.Dispose(disposing);
            }
        }
    }
}
