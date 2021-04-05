using System;
using System.Collections.Generic;
using System.IO;
using HerstAPI.Models;
using HerstAPI.Repositories;
using Microsoft.AspNetCore.Http;

namespace HerstAPI.Services
{
    public class WiFiProbeService
    {
        private WiFiProbeRepository _repo;
        public WiFiProbeService(WiFiProbeRepository repo) => _repo = repo;
        internal IEnumerable<WiFiProbe> GetProbes(string mac, string ssid) => _repo.GetProbes(mac, ssid);

        internal void ReadFile(IFormFile file)
        {
            using var stream = new StreamReader(file.OpenReadStream());

            while (!stream.EndOfStream)
            {
                var line = stream.ReadLine();
                
                if (string.IsNullOrWhiteSpace(line) || string.IsNullOrEmpty(line))
                    continue;

                if (line.Contains('('))
                    line = line.Substring(0, line.IndexOf("(") - 1) + line.Substring(line.IndexOf(")") + 1);
                
                line = line.Replace(" -> ", "@");
                line = line.Replace(" looking for ", "@");

                try
                {
                    var parts = line.Split("@", StringSplitOptions.RemoveEmptyEntries);
                    var mac = parts[0].Trim();
                    var ssid = parts[1].Trim();

                    Console.WriteLine($"MAC: {mac}, SSID: {ssid} ## {line} ##");
                    _repo.AddProbe(mac, ssid);
                }
                catch { Console.WriteLine($"Failed to parse: {line}"); }
            }
            _repo.Save();
        }
    }
}