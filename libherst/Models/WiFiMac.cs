using System;

namespace libherst.Models
{
    public class WiFiMac
    {
        public ulong WiFiMacId{get;set;}
        public string MAC { get; set; }

        public override string ToString() => $"WiFi MAC #{WiFiMacId}{Environment.NewLine}MAC: {MAC}";
    }
}