using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsInput;
using static LostTools.Settings;

namespace LostTools
{
    class Program
    {

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr window);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(IntPtr dc, int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr window, IntPtr dc);
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        private static readonly KeyboardSimulator _key = new KeyboardSimulator(new InputSimulator());
        const uint WM_KEYDOWN = 0x0100;

        static void Main(string[] args)
        {
            Console.Title = $"Press any key to quit...";
            Console.SetWindowSize(40, 4);
            Console.SetBufferSize(40, 4);
            // TODO: Read from file
            var settings = new Settings();
            var target = Process.GetProcessesByName(settings.ProcessName).FirstOrDefault();
            if (target == null)
            {
                Console.WriteLine($"{settings.ProcessName} is not running.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Waiting for game window to be focused...");

            var startedPotting = false;

            while (!Console.KeyAvailable)
            {
                Thread.Sleep(settings.CheckIntervalMs);
                var activeHandle = GetForegroundWindow();
                //Ignore other windows to avoid spam/lock
                if (activeHandle != target.MainWindowHandle) continue;

                var lifeBarX = GetLifeBarX(settings.LifeBar, startedPotting);
                var hpBarCol = GetColorAt(lifeBarX, settings.LifeBar.Y);
                //Console.WriteLine(lifeBarX);
                //Console.WriteLine(settings.LifeBar.Y);
                //Console.WriteLine(hpBarCol.ToString());

                if (IsLowHealth(hpBarCol))
                {
                    Console.WriteLine("POTTING!!!");
                    startedPotting = true;
                    PostMessage(target.MainWindowHandle, WM_KEYDOWN, settings.PotsKey, 0);
                }
                else
                {
                    Console.WriteLine("Life is good...");
                    startedPotting = false;
                }
            }
        }

        private static Color GetColorAt(int x, int y)
        {
            IntPtr desk = GetDesktopWindow();
            IntPtr dc = GetWindowDC(desk);
            int a = (int)GetPixel(dc, x, y);
            ReleaseDC(desk, dc);
            return Color.FromArgb(255, (a >> 0) & 0xff, (a >> 8) & 0xff, (a >> 16) & 0xff);
        }

        private static int GetLifeBarX(LifeBarSetting setting, bool started)
        {
            var x = (int)(setting.StartX + (setting.EndX - setting.StartX) * setting.CheckPct);
            if (started && setting.UntilFull)
            {
                x = setting.EndX;
            }
            return x;
        }

        private static bool IsLowHealth(Color colour) =>
            // Really dimmed grey
            colour.R <= 10 && colour.R > 1
            && colour.G <= 10 && colour.G > 1
            && colour.B <= 10 && colour.B > 1
        ;
    }
}
