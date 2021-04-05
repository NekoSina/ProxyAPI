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
            return db.WiFiProbes.Where(p => string.IsNullOrEmpty(mac) || p.MAC == mac)
                                .Where(p => string.IsNullOrEmpty(ssid) || p.SSID == ssid);
        }

        internal void AddProbe(string mac, string ssid)
        {
            var old = db.WiFiProbes.Find(mac);
            if(old != null)
                old.LastSeen = DateTime.Now;
            else
                db.WiFiProbes.Add(new WiFiProbe{ MAC = mac,SSID =ssid, LastSeen = DateTime.Now} );
        }

        internal void Save() => db.SaveChanges();
    }
}