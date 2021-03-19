using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProxyAPI.Database;
using ProxyAPI.Models;
using ProxyAPI.Repositories;
using ProxyAPI.Services;

namespace ProxyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProxyController : ControllerBase
    {
        private ProxyService _services;
        public ProxyController(ProxyDbContext context)
        {
            _services = new ProxyService(new ProxyRepository(context));
        }

        // GET: api/Proxy
        [HttpGet]
        [Route("/api/proxy")]
        public Proxy GetRandomProxy(string region, string country)
        {
            return _services.GetRandomProxy(region, country);
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
