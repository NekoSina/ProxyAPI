using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HerstAPI.Database;
using HerstAPI.Models;
using HerstAPI.Repositories;
using HerstAPI.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HerstAPI.Controllers
{
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WiFiController : ControllerBase
    {
        private WiFiService _service;
        private ILogger<WiFiController> _logger;
        public WiFiController(HerstDbContext context)
        {
            _service = new WiFiService(new WiFiRepository(context));
        }

        [HttpGet]
        [Route("/api/wifi/probe")]
        public IEnumerable<WiFiProbe> GetProbe(string mac, string ssid)
        {
            foreach(var probe in _service.GetProbes(mac, ssid))
                yield return probe;
        }
        [HttpPost]
        [Route("/api/wifi/probe")]
        public IActionResult UploadFile(IFormFile file)
        {
            _service.ReadFile(file);
            
            return Ok();
        }

        [HttpPut]
        [Route("/api/wifi/probe")]
        public IActionResult AddProbe([FromBody] WiFiProbe probe)
        {
            _service.AddProbe(probe);
            Debug.WriteLine($"Received a Probe!", probe);
            return Ok();
        }
        
        [HttpPut]
        [Route("/api/wifi/accesspoint")]
        public IActionResult AddAccessPoint([FromBody] WiFiAccessPoint ap)
        {
            _service.AddAccessPoint(ap);
            Debug.WriteLine($"Received an AccessPoint!", ap);
            return Ok();
        }
        
        // [HttpDelete]
        // [Route("/api/proxy")]
        // public IActionResult DeleteProxy(uint id)
        // {
        //     _services.DeleteProxy(id);
        //     return Ok();
        // }
    }
}
