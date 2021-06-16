using System;
using System.Collections.Generic;
using System.IO;
using HerstAPI.Models;
using HerstAPI.Models.DTOs;
using HerstAPI.Repositories;
using Microsoft.AspNetCore.Http;

namespace HerstAPI.Services
{
    public class WiFiService
    {
        private readonly WiFiRepository _repo;
        public WiFiService(WiFiRepository repo) => _repo = repo;

        internal WiFiUploadResponseDto ReadFile(IFormFile file)
        {
            int totalCount =0,validCount = 0, newCount = 0, beaconCount = 0, probeCount = 0;
            List<string> invalidEntries = new();
            using var stream = new StreamReader(file.OpenReadStream());

            while (!stream.EndOfStream)
            {
                var line = stream.ReadLine();
                totalCount++;

                try
                {
                    var parts = line.Split(",", StringSplitOptions.TrimEntries);

                    if (parts.Length != 4)
                    {
                        invalidEntries.Add(line);
                        throw new FormatException();
                    }

                    var timestamp = parts[0];
                    var type = parts[1];
                    var mac = parts[2];
                    var ssid = parts[3];
                    validCount++;
                    Console.WriteLine($"Got {type}: {mac} - {ssid}");

                    switch (type.ToLowerInvariant())
                    {
                        case "beacon":
                            beaconCount++;
                            if (_repo.AddAccessPoint(mac, ssid))
                                newCount++;
                            break;
                        case "probe":
                            probeCount++;
                            if (_repo.AddProbe(mac, ssid))
                                newCount++;
                            break;
                    }
                }
                catch { Console.WriteLine($"Failed to parse: {line}"); }
            }
            return new WiFiUploadResponseDto { TotalLineCount = totalCount,ValidEntries = validCount, NewEntries = newCount, Beacons = beaconCount, Probes = probeCount, InvalidEntries = invalidEntries };
        }

        internal IEnumerable<WiFiProbe> GetProbes(string mac, string ssid) => _repo.GetProbes(mac, ssid);
        internal bool AddAccessPoint(WiFiAccessPoint ap) => _repo.AddAccessPoint(ap);

        internal bool AddProbe(WiFiProbe probe) => _repo.AddProbe(probe);

        internal IEnumerable<WiFiAccessPoint> GetAccessPoints(string mac, string ssid) => _repo.GetAccessPoints(mac, ssid);
    }
}