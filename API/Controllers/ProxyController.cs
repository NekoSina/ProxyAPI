using System.Collections.Generic;
using System.Linq;
using HerstAPI.Database;
using HerstAPI.Repositories;
using HerstAPI.Services;
using libherst.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HerstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProxyController : ControllerBase
    {
        private readonly ProxyService _services;
        public ProxyController(HerstDbContext context)
        {
            _services = new ProxyService(new ProxyRepository(context));
        }

        [HttpGet]
        [Route("/api/proxy/test")]
        public IEnumerable<Proxy> GetProxies(int count)
        {
            return _services.GetProxiesToTest(count);
        }
        [HttpGet]
        [Route("/api/proxy")]
        public IEnumerable<Proxy> GetProxies(string region, string country, int hoursSinceTest,int score)
        {
            var proxies = _services.GetProxies(region, country,hoursSinceTest,score).Take(10);

            foreach(var proxy in proxies)
            {
                yield return proxy;
            }
        }
        [HttpPost]
        [Route("/api/proxy")]
        public IActionResult UploadFile(IFormFile file)
        {
            _services.ReadFile(file);
            
            return Ok();
        }

        [HttpPatch]
        [Route("/api/proxy")]
        public IActionResult UpdateProxy([FromBody] Proxy proxy)
        {
            _services.UpdateProxy(proxy);
            return Ok();
        }

        [HttpPut]
        [Route("/api/proxy")]
        public IActionResult AddProxy([FromBody] Proxy proxy)
        {
            _services.AddProxy(proxy);
            return Ok();
        }
        
        [HttpDelete]
        [Route("/api/proxy")]
        public IActionResult DeleteProxy(uint id)
        {
            _services.DeleteProxy(id);
            return Ok();
        }
    }
}
