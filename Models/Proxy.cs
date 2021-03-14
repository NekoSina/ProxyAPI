using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System;
namespace ProxyAPI.Models
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
        public DateTime LastTest { get; set; }

        public override string ToString() => $"{IP}:{Port} - {Region}, {Country}, {City}";
    }
}