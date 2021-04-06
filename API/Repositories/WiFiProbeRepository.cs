using System;
using System.Linq;
using HerstAPI.Database;
using HerstAPI.Models;

namespace HerstAPI.Repositories
{
    public class WiFiProbeRepository
    {
        private readonly HerstDbContext db;
        public WiFiProbeRepository(HerstDbContext context) => db = context;

        public IQueryable<WiFiProbe> GetProbes(string mac, string ssid)
        {
            return db.WiFiProbes.Where(p => string.IsNullOrEmpty(mac) || p.WiFiMac.MAC == mac)
                                .Where(p => string.IsNullOrEmpty(ssid) || p.WiFiNetworkName.SSID == ssid);
        }

        internal void AddProbe(string mac, string ssid)
        {
            var old = db.WiFiMacs.FirstOrDefault(w => w.MAC == mac);
            if (old == null)
                old = new WiFiMac { MAC = mac };

            var probe = db.WiFiProbes.FirstOrDefault(p => p.WiFiMac == old);
            if (probe != null)
                probe.LastSeen = DateTime.UtcNow;
            else
            {
                db.WiFiProbes.Add(new WiFiProbe 
                { 
                    WiFiMac = new WiFiMac { MAC = mac },
                    WiFiNetworkName = new WiFiNetworkName { SSID = ssid }, 
                    LastSeen = DateTime.Now 
                });
            }
            Save();
        }

        internal void Save() => db.SaveChanges();
    }
}