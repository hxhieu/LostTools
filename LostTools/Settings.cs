namespace LostTools
{
    internal class Settings
    {
        internal class LifeBarSetting
        {
            public int StartX { get; set; } = 796;
            public int EndX { get; set; } = 1122;
            public int Y { get; set; } = 1265;
            public float CheckPct { get; set; } = 0.7f;
            public bool UntilFull { get; set; } = true;
        }

        public string ProcessName { get; set; } = "LOSTARK";
        public int CheckIntervalMs { get; set; } = 1000;
        public LifeBarSetting LifeBar { get; set; } = new LifeBarSetting();
        public int PotsKey { get; set; } = 112;
    }
}
