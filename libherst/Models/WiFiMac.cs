using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace libherst.Models
{
    public class WiFiMac
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong WiFiMacId{get;set;}
        public string MAC { get; set; }

        public override string ToString() => $"WiFi MAC #{WiFiMacId}{Environment.NewLine}MAC: {MAC}";
    }
}