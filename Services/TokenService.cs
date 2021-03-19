using ProxyAPI.Models;
using ProxyAPI.Repositories;

namespace ProxyAPI.Services
{
    public class TokenService
    {
        private UserRepository _userRepository;
        public TokenService(UserRepository repository) => _userRepository = repository;

        public bool TryAuthenticate(string user, string pass, out UserInfo userInfo)
        {
            userInfo = _userRepository.GetUser(user, pass);
            return user != null;
        }
    }
}