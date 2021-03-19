using System.ComponentModel.DataAnnotations;
namespace ProxyAPI.Models
{
    public class UserInfo
    {
        [Key]
        public string Username {get; set; }
        public string Password {get; set; }
    }
}