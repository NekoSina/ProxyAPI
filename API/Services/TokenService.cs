using HerstAPI.Models;
using HerstAPI.Repositories;

namespace HerstAPI.Services
{
    public class TokenService
    {
        private UserRepository _userRepository;
        public TokenService(UserRepository repository) => _userRepository = repository;

        public bool TryAuthenticate(string user, string pass, out UserInfo userInfo)
        {
            userInfo = _userRepository.GetUser(user, pass);
            return userInfo != null;
        }
    }
}