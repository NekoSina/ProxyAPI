using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HerstAPI.Models
{
    public class WiFiProbe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong WiFiProbeId{get;set;}
        public WiFiMac WiFiMac {get; set; }
        public WiFiNetworkName WiFiNetworkName {get; set; }
        public DateTime LastSeen { get; set; }
    }

    public class WiFiNetworkName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong WiFiNetworkNameId{get;set;}
        public string SSID { get; set; }
    }

    public class WiFiMac
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong WiFiMacId{get;set;}
        public string MAC { get; set; }
    }
    public class WiFiClient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong WiFiClientId{get;set;}
        public WiFiMac WiFiMac {get;set;}
        public List<WiFiAccessPoint> AccessPoints {get;set;}
        public DateTime LastSeen { get; set; }
    }
    public class WiFiAccessPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong WiFiAccessPointId{get;set;}
        public WiFiMac WiFiMac {get;set;}
        public WiFiNetworkName WiFiNetworkName {get;set;}
        public DateTime LastSeen { get; set; }
    }
}