using System;

namespace libwifimonitor
{
    public class CacheEntry
    {
        public string MAC { get; set; }
        public string SSID { get; set; }
        public int SeenTimes { get; set; }
        public int SignalStrength { get; set; }
        public int LastSignalStrength { get; set; }

        public DateTime LastSeen { get; set; }
    }
}