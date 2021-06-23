using WindowsInput.Native;

namespace LostTools
{
    internal class Settings
    {
        internal class LifeBarSetting
        {
            public int StartX { get; set; } = 840;
            public int EndX { get; set; } = 1140;
            public int Y { get; set; } = 1280;
            public float CheckPct { get; set; } = 0.6f;
            public bool UntilFull { get; set; } = true;
        }

        public string ProcessName { get; set; } = "LOSTARK";
        public int CheckIntervalMs { get; set; } = 1000;
        public LifeBarSetting LifeBar { get; set; } = new LifeBarSetting();
        public int PotsKey { get; set; } = (int)VirtualKeyCode.F1;
    }
}
