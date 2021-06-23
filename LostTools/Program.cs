using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using static LostTools.Settings;

namespace LostTools
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = $"Press any key to quit...";
            Console.SetWindowSize(60, 6);
            Console.SetBufferSize(60, 6);
            // TODO: Read from file
            var settings = new Settings();
            var target = Process.GetProcessesByName(settings.ProcessName).FirstOrDefault();
            if (target == null)
            {
                Log($"{settings.ProcessName} is not running.");
                Console.ReadKey();
                return;
            }

            Log($"Waiting for game window to be focused...");

            var startedPotting = false;

            while (!Console.KeyAvailable)
            {
                // Clamp the min interval
                Thread.Sleep(settings.CheckIntervalMs < 500 ? 500 : settings.CheckIntervalMs);
                var activeHandle = WinApi.GetForegroundWindow();
                //Ignore other windows to avoid spam/lock
                if (activeHandle != target.MainWindowHandle) continue;

                var lifeBarX = GetLifeBarX(settings.LifeBar, startedPotting);
                var hpBarCol = WinApi.GetColorAt(lifeBarX, settings.LifeBar.Y);
                //Console.WriteLine(lifeBarX);
                //Console.WriteLine(settings.LifeBar.Y);
                //Console.WriteLine(hpBarCol.ToString());

                if (IsLowHealth(hpBarCol))
                {
                    Log("POTTING!!!");
                    startedPotting = true;
                    WinApi.SendKeyDown(target.MainWindowHandle, settings.PotsKey);
                }
                else
                {
                    Log("Life is good...");
                    startedPotting = false;
                }
            }
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

        private static void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff]");
            var logMessage = $"{timestamp} {message}";
            Console.WriteLine(logMessage);
#if DEBUG
            Directory.CreateDirectory("logs");
            var logRoll = Path.Combine("logs", $"log_{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
            using (var stream = File.AppendText(logRoll))
            {
                stream.WriteLine(logMessage);
            }
#endif
        }
    }
}
