using System.ComponentModel.DataAnnotations;
namespace HerstAPI.Models
{
    public class UserInfo
    {
        [Key]
        public string Username {get; set; }
        public string Password {get; set; }
    }
}