using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HerstAPI.Database;
using HerstAPI.Models;
using HerstAPI.Repositories;
using HerstAPI.Services;
using HerstAPI.Models.DTOs;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HerstAPI.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WiFiController : ControllerBase
    {
        private WiFiService _service;
        public WiFiController(HerstDbContext context) => _service = new WiFiService(new WiFiRepository(context));

        [HttpGet]
        [Route("/api/wifi/probe")]
        public IEnumerable<WiFiProbe> GetProbe(string mac, string ssid)
        {
            foreach (var probe in _service.GetProbes(mac, ssid))
                yield return probe;
        }
        [HttpPost]
        [Route("/api/wifi")]
        public IActionResult UploadFile(IFormFile file) => (Ok(_service.ReadFile(file)));

        [HttpPut]
        [Route("/api/wifi/probe")]
        public IActionResult AddProbe([FromBody] WiFiProbe probe)
        {
            Console.WriteLine($"[{DateTime.Now}] Received a Probe: " + probe);
            return Ok(_service.AddProbe(probe)); 
        }

        [HttpGet]
        [Route("/api/wifi/accesspoint")]
        public IEnumerable<WiFiAccessPointDto> GetAccessPoint(string mac, string ssid)
        {
            foreach (var ap in _service.GetAccessPoints(mac, ssid))
                yield return new WiFiAccessPointDto { Mac = ap.WiFiMac.MAC, Ssid = ap.WiFiNetworkName.SSID, LastSeen = ap.LastSeen };;
        }
        [HttpPut]
        [Route("/api/wifi/accesspoint")]
        public IActionResult AddAccessPoint([FromBody] WiFiAccessPoint ap)
        {
            Console.WriteLine($"[{DateTime.Now}] Received an AccessPoint: "+ ap);
            return Ok(_service.AddAccessPoint(ap));
        }
    }
}
