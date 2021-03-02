using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace ProxyAPI.Models
{
    public class Proxy
    {
        public Proxy()
        {

        }
        public Proxy(string endpoint)
        {
            EndPoint = endpoint;
        }
        [Key]
        public int ID {get; set; }

        public string EndPoint { get; set; }
        public string Region { get; set; }
        public string Country {get; set; }
    }
}