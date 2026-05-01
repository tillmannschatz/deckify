using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Office.Core;
using System.Runtime.InteropServices;
using System.Text;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using HWND = System.IntPtr;

namespace Deckify.Common
{
    public static class SystemUtilities
    {
        #region Get all open Windows
        /// <summary>Contains functionality to get all the open windows.</summary>
        public static class OpenWindowGetter
        {
            /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
            /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
            public static IDictionary<HWND, string> GetOpenWindows()
            {
                HWND shellWindow = GetShellWindow();
                Dictionary<HWND, string> windows = new Dictionary<HWND, string>();

                EnumWindows(delegate (HWND hWnd, int lParam)
                {
                    if (hWnd == shellWindow) return true;
                    if (!IsWindowVisible(hWnd)) return true;

                    int length = GetWindowTextLength(hWnd);
                    if (length == 0) return true;

                    StringBuilder builder = new StringBuilder(length);
                    GetWindowText(hWnd, builder, length + 1);

                    windows[hWnd] = builder.ToString();
                    return true;

                }, 0);

                return windows;
            }

            private delegate bool EnumWindowsProc(HWND hWnd, int lParam);

            [DllImport("USER32.DLL")]
            private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

            [DllImport("USER32.DLL")]
            private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

            [DllImport("USER32.DLL")]
            private static extern int GetWindowTextLength(HWND hWnd);

            [DllImport("USER32.DLL")]
            private static extern bool IsWindowVisible(HWND hWnd);

            [DllImport("USER32.DLL")]
            private static extern IntPtr GetShellWindow();
        }
        #endregion

        #region Open new PowerPoint Instance
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        #endregion

        #region Take a screenshot
        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        [DllImport("user32.dll")]
        private static extern IntPtr GetClientRect(IntPtr hWnd, ref Rect rect);
        [DllImport("user32.dll")]
        private static extern IntPtr ClientToScreen(IntPtr hWnd, ref System.Drawing.Point point);
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);
        public static Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new Rect();
            GetClientRect(handle, ref rect);

            var point = new System.Drawing.Point(0, 0);
            ClientToScreen(handle, ref point);

            var bounds = new Rectangle(point.X, point.Y, rect.Right, rect.Bottom);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result))
            {
                IntPtr dc = graphics.GetHdc();
                bool success = PrintWindow(handle, dc, 0);
                graphics.ReleaseHdc(dc);
            }

            return result;
        }
        #endregion


        #region Performance Helpers
        [DllImport("user32.dll")]
        private static extern bool LockWindowUpdate(IntPtr hWndLock);

        public static void RunBatchOperation(PowerPoint.Application app, Action action)
        {
            if (app == null)
            {
                action();
                return;
            }

            try
            {
                LockWindowUpdate((IntPtr)app.HWND);
                action();
            }
            finally
            {
                LockWindowUpdate(IntPtr.Zero);
            }
        }
        #endregion
    }
}
