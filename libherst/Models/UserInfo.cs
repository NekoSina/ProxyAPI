using System;
using System.ComponentModel.DataAnnotations;

namespace libherst.Models
{
    public class UserInfo
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }

        public override string ToString() => $"User: {Username}{Environment.NewLine}Pass: {Password}";
    }
}