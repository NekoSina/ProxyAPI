using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace libherst.Models
{
    public class WiFiNetworkName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong WiFiNetworkNameId{get;set;}
        public string SSID { get; set; }

        public override string ToString() => $"WiFi NetworkName #{WiFiNetworkNameId}{Environment.NewLine}SSID: {SSID}";
    }
}