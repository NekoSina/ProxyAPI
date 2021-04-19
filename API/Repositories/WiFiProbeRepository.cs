using System;
using System.Linq;
using HerstAPI.Database;
using HerstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HerstAPI.Repositories
{
    public class WiFiRepository
    {
        private readonly HerstDbContext db;
        public WiFiRepository(HerstDbContext context) => db = context;

        public IQueryable<WiFiProbe> GetProbes(string mac, string ssid)
        {
            return db.WiFiProbes.Where(p => string.IsNullOrEmpty(mac) || p.WiFiMac.MAC == mac)
                                .Where(p => string.IsNullOrEmpty(ssid) || p.WiFiNetworkName.SSID == ssid)
                                .Include(p => p.WiFiMac)
                                .Include(p => p.WiFiNetworkName);
        }

        internal bool AddProbe(string mac, string ssid)
        {
            var client = GetOrCreateClient(mac);
            var probe = db.WiFiProbes.Include(p=> p.WiFiMac)
                                     .Include(p=> p.WiFiNetworkName)
                                     .Where(p=>p.WiFiMac == client.WiFiMac)
                                     .FirstOrDefault(p=> p.WiFiNetworkName.SSID == ssid);

            if (probe == null)
            {
                db.WiFiProbes.Add(new WiFiProbe
                {
                    WiFiMac = client.WiFiMac,
                    WiFiNetworkName = GetOrCreateNetworkName(ssid),
                    LastSeen = DateTime.UtcNow
                });
            }
            else
                probe.LastSeen = DateTime.UtcNow;

            db.SaveChanges();
            return probe == null;
        }
        internal bool AddAccessPoint(string mac, string ssid)
        {
            var network = GetOrCreateNetworkName(ssid);
            var wifiMac = GetOrCreateMac(mac);
            var accessPoint = db.WiFiAccessPoints.Include(p=>p.WiFiMac)
                                                 .Include(p=>p.WiFiNetworkName)
                                                 .Where(p=> p.WiFiMac == wifiMac)
                                                 .FirstOrDefault(p=>p.WiFiNetworkName == network);
            if(accessPoint == null)
            {                                   
                var clients = db.WiFiClients.Include(i=> i.AccessPoints)
                                            .Where(a=> a.AccessPoints
                                                .All(ap=>ap.WiFiNetworkName == network))
                                            .ToList();

                db.WiFiAccessPoints.Add(new WiFiAccessPoint
                {
                    LastSeen = DateTime.UtcNow,
                    WiFiMac = wifiMac,
                    WiFiNetworkName = network
                });
            }
            else
                accessPoint.LastSeen = DateTime.UtcNow;

            db.SaveChanges();
            return accessPoint == null;
        }
        internal bool AddAccessPoint(WiFiAccessPoint ap) => AddAccessPoint(ap.WiFiMac.MAC, ap.WiFiNetworkName.SSID);

        internal IQueryable<WiFiAccessPoint> GetAccessPoints(string mac, string ssid)
        {
            return db.WiFiAccessPoints.Include(p => p.WiFiMac)
                                        .Include(p => p.WiFiNetworkName)
                                        .Where(p => string.IsNullOrEmpty(mac) || p.WiFiMac.MAC == mac)
                                        .Where(p => string.IsNullOrEmpty(ssid) || p.WiFiNetworkName.SSID == ssid);
        }

        internal bool AddProbe(WiFiProbe probe) => AddProbe(probe.WiFiMac.MAC, probe.WiFiNetworkName.SSID);

        private WiFiClient GetOrCreateClient(string mac)
        {
            var client = db.WiFiClients.Include(p=>p.WiFiMac)
                                       .FirstOrDefault(w => w.WiFiMac.MAC == mac) ?? 
                                       new WiFiClient { WiFiMac = GetOrCreateMac(mac), LastSeen=DateTime.UtcNow};
            db.SaveChanges();
            return client;
        }
        private WiFiMac GetOrCreateMac(string mac) 
        {
            var wifiMac = db.WiFiMacs.FirstOrDefault(w => w.MAC == mac) ?? 
                                     new WiFiMac { MAC = mac };
            db.SaveChanges();
            return wifiMac;
        }
        private WiFiNetworkName GetOrCreateNetworkName(string ssid) 
        { 
            var network = db.WiFiNetworkNames.FirstOrDefault(n => n.SSID == ssid) ?? 
                                             new WiFiNetworkName { SSID = ssid };
            db.SaveChanges();
            return network;
        }
   }
}