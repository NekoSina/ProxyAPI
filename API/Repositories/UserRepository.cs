using System.Linq;
using HerstAPI.Database;
using HerstAPI.Models;

namespace HerstAPI.Repositories
{
    public class UserRepository
    {
        private readonly HerstDbContext db;
        public UserRepository(HerstDbContext context) => db = context;

        public UserInfo GetUser(string user, string pass) => db.Users.FirstOrDefault(u => u.Username == user && u.Password == pass);
    }
}