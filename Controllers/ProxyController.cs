using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProxyAPI.Models;

namespace ProxyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        //private readonly ProxyContext _context;
        private ProxyServices _services;
        public ProxyController(ProxyContext context)
        {
            _services = new ProxyServices(new ProxyRepository(context));
        }

        // GET: api/Proxy
        [HttpGet]
        public ActionResult<IEnumerable<Proxy>> GetProxies()
        {
            return  Ok();
        }

        [HttpGet]
        [Route("/api/randomproxy")]
        public ActionResult<Proxy> GetRandomProxy()
        {
            return _services.GetRandomProxy();
        }

        [HttpPost]
        [Route("/api/UploadFile")]
        public IActionResult UploadFile(IFormFile file)
        {
            _services.ReadFile(file);
            return Ok();
        }
    }
}
