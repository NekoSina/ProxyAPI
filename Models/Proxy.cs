using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System;
namespace ProxyAPI.Models
{
    public class Proxy
    {
        public Proxy()
        {

        }
        public Proxy(string endpoint, string region)
        {
            Region = region;
            IP = endpoint.Split(':')[0];
            Port = ushort.Parse(endpoint.Split(':')[1]);
        }
        [Key]
        public int ID {get; set; }

        public string IP { get; set; }
        public ushort Port { get; set; }
        public string Region { get; set; }
        public string Country {get; set; }
        public DateTime LastTest { get; set; }
    }
}