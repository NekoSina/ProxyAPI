using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace libherst.Models
{
    public class WiFiProbe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong WiFiProbeId{get;set;}
        public WiFiMac WiFiMac {get; set; }
        public WiFiNetworkName WiFiNetworkName {get; set; }
        public DateTime LastSeen { get; set; }

        public WiFiClient Client {get;set;}

        public override string ToString() => $"Probe #{WiFiProbeId}{Environment.NewLine}MAC: {WiFiMac.MAC}{Environment.NewLine}SSID: {WiFiNetworkName.SSID}{Environment.NewLine}Last Seen: {LastSeen}";
    }
}