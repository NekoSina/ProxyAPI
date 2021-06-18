using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HerstAPI.Database;
using HerstAPI.Repositories;
using HerstAPI.Services;
using libherst.Models;

namespace HerstAPI.Controllers
{
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        public TokenService _tokenService;

        public TokenController(IConfiguration config)
        {
            _configuration = config;
            _tokenService = new TokenService(new UserRepository(new HerstDbContext()));
        }

        [HttpPost]
        [Route("api/auth/token")]
        public ActionResult Post(UserInfo _userData)
        {
            if (_userData != null && _userData.Username != null && _userData.Password != null)
            {
                if(_tokenService.TryAuthenticate(_userData.Username, _userData.Password, out var user))
                {
                    //create claims details based on the user information
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Username", user.Username),
                    new Claim("Password", user.Password)
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Secret"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken("Trbl", "sec.her.st",claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}