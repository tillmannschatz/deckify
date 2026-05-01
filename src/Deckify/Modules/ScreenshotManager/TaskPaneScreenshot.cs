using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.PowerPoint;

namespace Deckify
{
    public partial class TaskPaneScreenshot : UserControl
    {
        public TaskPaneScreenshot()
        {
            InitializeComponent();
            UpdateWindowSelector();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateWindowSelector();
        }
        private void listBoxWindowSelector_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void btnSetupRegion_Click(object sender, EventArgs e)
        {
            DefineScreenshotArea();
        }
        private void btnTakeScreenshot_Click(object sender, EventArgs e)
        {
            CaptureSelectedWindow();
        }
        private void btnTakeAreaScreenshot_Click(object sender, EventArgs e)
        {
            TakeScreenshotFromDefinedArea();
        }

        #region Functions to take a screenshot
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private void CaptureSelectedWindow()
        {
            // Ensure a window is selected in the ListBox
            if (listBoxWindowSelector.SelectedItem is WindowItem selectedItem)
            {
                // Retrieve the handle of the selected window
                IntPtr selectedWindowHandle = selectedItem.Handle;

                // Call the CaptureWindow function to take a screenshot of the selected window
                CaptureWindow(selectedWindowHandle);

                //MessageBox.Show($"Screenshot of window '{selectedItem.Title}' has been captured and added to PowerPoint.");
            }
            else
            {
                MessageBox.Show("Please select a window from the list before capturing.");
            }
        }

        public static void CaptureWindow(IntPtr windowHandle)
        {
            SetForegroundWindow(windowHandle);
            var rect = GetWindowRectangle(windowHandle);
            CaptureAndPaste(rect);
        }

        public static void CaptureArea(Rectangle area)
        {
            CaptureAndPaste(area);
        }

        private static void CaptureAndPaste(Rectangle rect)
        {
            using (Bitmap bmp = new Bitmap(rect.Width, rect.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rect.Location, System.Drawing.Point.Empty, rect.Size);
                }
                Clipboard.SetImage(bmp);
                AddScreenshotToPowerPoint();
            }
        }

        private static Rectangle GetWindowRectangle(IntPtr hWnd)
        {
            RECT rect;
            GetWindowRect(hWnd, out rect);
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private static void AddScreenshotToPowerPoint()
        {
            var pptApp = Globals.ThisAddIn.Application;
            var activePresentation = pptApp.ActivePresentation;

            // Add a new blank slide and paste the screenshot
            var slide = activePresentation.Slides.Add(activePresentation.Slides.Count + 1, PpSlideLayout.ppLayoutBlank);
            slide.Shapes.Paste();

            // Navigate to the newly added slide
            pptApp.ActiveWindow.View.GotoSlide(slide.SlideIndex);

            // Bring PowerPoint to the foreground
            var pptMainWindowHandle = new IntPtr(pptApp.HWND);
            SetForegroundWindow(pptMainWindowHandle);
        }
        #endregion

        #region Functions to handle area setup
        // Static variable to store the defined screenshot area
        private static Rectangle? definedArea = null;
        public static void DefineScreenshotArea()
        {
            // Allow user to define the area and save it
            definedArea = SelectAreaAcrossMonitors();
            if (definedArea != null)
            {
                MessageBox.Show($"Area defined: {definedArea.Value.Width}x{definedArea.Value.Height} at ({definedArea.Value.X},{definedArea.Value.Y})");
            }
        }
        public static void TakeScreenshotFromDefinedArea()
        {
            // Check if an area has been defined
            if (definedArea == null)
            {
                MessageBox.Show("Please define the screenshot area first.");
                return;
            }

            // Capture and paste the screenshot
            CaptureArea(definedArea.Value);
            //MessageBox.Show("Screenshot taken and added to PowerPoint.");
        }
        private static Rectangle SelectAreaAcrossMonitors()
        {
            // Calculate the combined bounds of all monitors
            var screenBounds = Screen.AllScreens
                .Select(screen => screen.Bounds)
                .Aggregate(Rectangle.Union);

            // Create an overlay form that spans all monitors
            Form overlay = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.Black,
                Opacity = 0.3,
                Bounds = screenBounds,
                TopMost = true
            };

            Rectangle selectedRectangle = Rectangle.Empty;
            System.Drawing.Point startPoint = System.Drawing.Point.Empty;

            overlay.MouseDown += (s, e) =>
            {
                startPoint = new System.Drawing.Point(e.X + overlay.Left, e.Y + overlay.Top);
            };

            overlay.MouseMove += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    var currentPoint = new System.Drawing.Point(e.X + overlay.Left, e.Y + overlay.Top);
                    selectedRectangle = new Rectangle(
                        Math.Min(startPoint.X, currentPoint.X),
                        Math.Min(startPoint.Y, currentPoint.Y),
                        Math.Abs(startPoint.X - currentPoint.X),
                        Math.Abs(startPoint.Y - currentPoint.Y));
                    overlay.Invalidate();
                }
            };

            overlay.MouseUp += (s, e) =>
            {
                overlay.Close();
            };

            overlay.Paint += (s, e) =>
            {
                if (!selectedRectangle.IsEmpty)
                {
                    using (Pen pen = new Pen(Color.Red, 2))
                    {
                        e.Graphics.DrawRectangle(pen, selectedRectangle);
                    }
                }
            };

            overlay.ShowDialog();
            return selectedRectangle;
        }
        #endregion

        #region Functions for Managing open Windows in Listbox
        public static Dictionary<IntPtr, string> GetOpenWindows()
        {
            var windows = new Dictionary<IntPtr, string>();
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    var sb = new StringBuilder(256);
                    GetWindowText(hWnd, sb, sb.Capacity);
                    var title = sb.ToString();
                    if (!string.IsNullOrEmpty(title))
                        windows[hWnd] = title;
                }
                return true; // Continue enumeration
            }, IntPtr.Zero);
            return windows;
        }
        public void UpdateWindowSelector()
        {
            // Get the Open windows
            var windows = GetOpenWindows();

            // Clear existing items in the ListBox
            listBoxWindowSelector.Items.Clear();

            // Add each window to the ListBox
            foreach (var window in windows)
            {
                listBoxWindowSelector.Items.Add(new WindowItem(window.Key, window.Value));
            }

            // Optionally set DisplayMember to show only the window titles
            listBoxWindowSelector.DisplayMember = "Title";
        }
        // Helper class to represent a window
        private class WindowItem
        {
            public IntPtr Handle { get; }
            public string Title { get; }

            public WindowItem(IntPtr handle, string title)
            {
                Handle = handle;
                Title = title;
            }

            public override string ToString()
            {
                return Title;
            }
        }
        #endregion

        #region DLL Imports for handling Windows
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        #endregion
    }
}
