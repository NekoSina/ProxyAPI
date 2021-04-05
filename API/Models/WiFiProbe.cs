using System;
using System.ComponentModel.DataAnnotations;

namespace HerstAPI.Models
{
    public class WiFiProbe
    {
        [Key]
        public string MAC {get; set; }
        public string SSID {get; set; }
        public DateTime LastSeen {get;set;}
    }
}