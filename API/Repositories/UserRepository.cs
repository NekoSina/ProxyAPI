using System.Linq;
using ProxyAPI.Database;
using ProxyAPI.Models;

namespace ProxyAPI.Repositories
{
    public class UserRepository
    {
        private readonly ProxyDbContext db;
        public UserRepository(ProxyDbContext context) => db = context;

        public UserInfo GetUser(string user, string pass) => db.Users.FirstOrDefault(u => u.Username == user && u.Password == pass);
    }
}