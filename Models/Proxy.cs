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
            EndPoint = endpoint;
            Region = region;
        }
        [Key]
        public int ID {get; set; }

        public string EndPoint { get; set; }
        public string Region { get; set; }
        public string Country {get; set; }
        //[DataType(DataType.Date)]
        //public DateTime ReleaseDate { get; set; }
    }
}