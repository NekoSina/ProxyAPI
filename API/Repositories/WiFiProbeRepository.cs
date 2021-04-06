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
            var client = GetOrCreateClient(mac);

            var probe = db.WiFiProbes.FirstOrDefault(p => p.WiFiMac == client.WiFiMac);
            if (probe == null)
            {
                db.WiFiProbes.Add(new WiFiProbe
                {
                    WiFiMac = GetOrCreateMac(mac),
                    WiFiNetworkName = GetOrCreateNetworkName(ssid),
                    LastSeen = DateTime.Now
                });
            }
            else
                probe.LastSeen = DateTime.UtcNow;

            Save();
        }

        private WiFiClient GetOrCreateClient(string mac) => db.WiFiClients.FirstOrDefault(w => w.WiFiMac.MAC == mac) ?? new WiFiClient { WiFiMac = GetOrCreateMac(mac) };
        private WiFiMac GetOrCreateMac(string mac) => db.WiFiMacs.FirstOrDefault(w => w.MAC == mac) ?? new WiFiMac { MAC = mac };
        private WiFiNetworkName GetOrCreateNetworkName(string ssid) => db.WiFiNetworkNames.FirstOrDefault(n => n.SSID == ssid) ?? new WiFiNetworkName { SSID = ssid };

        internal void Save() => db.SaveChanges();
    }
}