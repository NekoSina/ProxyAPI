using System;
using System.Collections.Generic;

namespace HerstAPI.Models.DTOs
{
    public class WiFiAccessPointDto
    {
        public string Mac {get;set;}
        public string Ssid {get;set;}
        public List<string> Clients {get;set;}
        public DateTime LastSeen {get;set;}
    }
}