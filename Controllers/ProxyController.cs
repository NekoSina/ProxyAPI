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
        [Route("/api/getproxies")]
        public IQueryable GetProxies(string region, string country, int latency)
        {
            return _services.GetProxies(region, country, latency);
        }
        [HttpGet]
        [Route("/api/randomproxy")]
        public ActionResult<Proxy> GetRandomProxy()
        {
            return _services.GetRandomProxy();;
        }
        [HttpGet]
        [Route("/api/firstproxy")]
        public ActionResult<Proxy> GetFirstProxy()
        {
            return _services.GetFirstProxy();
        }
        [HttpPost]
        [Route("/api/uploadfile")]
        public IActionResult UploadFile(IFormFile file)
        {
            _services.ReadFile(file);
            return Ok();
        }
        [HttpDelete]
        [Route("/api/delete")]
        public IActionResult ClearDB()
        {
            _services.Cleardb();
            return Ok();
        }
    }
}
