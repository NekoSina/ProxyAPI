using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HerstAPI.Database;
using HerstAPI.Models;
using HerstAPI.Repositories;
using HerstAPI.Services;

namespace HerstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WiFiProbeController : ControllerBase
    {
        private WiFiProbeService _service;
        public WiFiProbeController(HerstDbContext context)
        {
            _service = new WiFiProbeService(new WiFiProbeRepository(context));
        }

        [HttpGet]
        [Route("/api/probe")]
        public IEnumerable<WiFiProbe> GetProbe(string mac, string ssid)
        {
            foreach(var probe in _service.GetProbes(mac, ssid))
                yield return probe;
        }
        [HttpPost]
        [Route("/api/probe")]
        public IActionResult UploadFile(IFormFile file)
        {
            _service.ReadFile(file);
            
            return Ok();
        }

        // [HttpPatch]
        // [Route("/api/proxy")]
        // public IActionResult UpdateProxy([FromBody] Proxy proxy)
        // {
        //     _services.UpdateProxy(proxy);
        //     return Ok();
        // }

        // [HttpPut]
        // [Route("/api/proxy")]
        // public IActionResult AddProxy([FromBody] Proxy proxy)
        // {
        //     _services.AddProxy(proxy);
        //     return Ok();
        // }
        
        // [HttpDelete]
        // [Route("/api/proxy")]
        // public IActionResult DeleteProxy(uint id)
        // {
        //     _services.DeleteProxy(id);
        //     return Ok();
        // }
    }
}
