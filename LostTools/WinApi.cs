using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace LostTools
{
    internal static class WinApi
    {
        const uint WM_KEYDOWN = 0x0100;
        const int HWND_TOPMOST = -1;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindowDC(IntPtr window);
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern uint GetPixel(IntPtr dc, int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int ReleaseDC(IntPtr window, IntPtr dc);
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        public static Color GetColorAt(int x, int y)
        {
            IntPtr desk = GetDesktopWindow();
            IntPtr dc = GetWindowDC(desk);
            int a = (int)GetPixel(dc, x, y);
            ReleaseDC(desk, dc);
            return Color.FromArgb(255, (a >> 0) & 0xff, (a >> 8) & 0xff, (a >> 16) & 0xff);
        }

        public static void SendKeyDown(IntPtr handle, int key)
        {
            PostMessage(handle, WM_KEYDOWN, key, 0);
        }

        public static void SetAlwaysOnTop(IntPtr handle, int x, int y, int width, int height)
        {
            SetWindowPos(handle, new IntPtr(HWND_TOPMOST), x, y, width, height, 0);
        }

        public static IntPtr GetCurrentWindow() => GetForegroundWindow();
    }
}
