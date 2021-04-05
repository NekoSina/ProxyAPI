using System.ComponentModel.DataAnnotations;
using System;
namespace HerstAPI.Models
{
    public class Proxy
    {
        [Key]
        public ulong ID {get; set; }

        public string IP { get; set; }
        public ushort Port { get; set; }        
        public string City { get; set; }
        public string Country {get; set; }
        public string Region { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public DateTime LastTest { get; set; }
        public string AS { get; set; }
        public string ASN { get; set; }
        public string Domain { get; set; }
        public string ISP { get; set; }
        public bool KnownAsProxy => !string.IsNullOrEmpty(AS);
        public string ProxyType { get; set; }
        public string Threat { get; set; }
        public int Score {get;set;}
        public bool Working {get;set;}

        public override string ToString() => $"{IP}:{Port} - {Region}, {Country}, {City}";
    }
}