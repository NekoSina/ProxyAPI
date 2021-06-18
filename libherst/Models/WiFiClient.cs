using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace libherst.Models
{
    public class WiFiClient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong WiFiClientId{get;set;}
        public WiFiMac WiFiMac {get;set;}
        public List<WiFiAccessPoint> AccessPoints {get;set;}
        public DateTime LastSeen { get; set; }

        public override string ToString() => $"WiFi Client #{WiFiClientId}{Environment.NewLine}MAC: {WiFiMac.MAC}{Environment.NewLine}Last Seen: {LastSeen}";
    }
}