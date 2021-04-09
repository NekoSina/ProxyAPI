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
                                .Include(p => p.Client)
                                .Include(p => p.WiFiMac)
                                .Include(p => p.WiFiNetworkName)
                                .Include(p => p.Client.AccessPoints);
        }

        internal void AddProbe(string mac, string ssid)
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
                    Client = client,
                    WiFiMac = client.WiFiMac,
                    WiFiNetworkName = GetOrCreateNetworkName(ssid),
                    LastSeen = DateTime.UtcNow
                });
            }
            else
                probe.LastSeen = DateTime.UtcNow;

            db.SaveChanges();
        }

        internal void AddAccessPoint(WiFiAccessPoint ap)
        {
            var network = GetOrCreateNetworkName(ap.WiFiNetworkName.SSID);
            var mac = GetOrCreateMac(ap.WiFiMac.MAC);
            var accessPoint = db.WiFiAccessPoints.Include(p=>p.WiFiMac)
                                                 .Include(p=>p.WiFiNetworkName)
                                                 .Where(p=> p.WiFiMac == mac)
                                                 .FirstOrDefault(p=>p.WiFiNetworkName == network);
            if(accessPoint == null)
            {                                   
                db.WiFiAccessPoints.Add(new WiFiAccessPoint
                {
                    LastSeen = DateTime.UtcNow,
                    WiFiMac = mac,
                    WiFiNetworkName = network
                });
            }
            else
                accessPoint.LastSeen = DateTime.UtcNow;

            db.SaveChanges();
        }

        internal void AddProbe(WiFiProbe probe) => AddProbe(probe.WiFiMac.MAC, probe.WiFiNetworkName.SSID);

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