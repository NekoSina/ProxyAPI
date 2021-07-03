using System;
using System.Linq;
using HerstAPI.Database;
using libherst.Models;

namespace HerstAPI.Repositories
{
    public class ProxyRepository
    {
        private readonly HerstDbContext db;
        public ProxyRepository(HerstDbContext context) => db = context;

        private bool TryGetProxy(ulong id, out Proxy proxy)
        {
            proxy = db.Proxies.Find(id);
            return proxy != null;
        }
        public IQueryable<Proxy> GetProxies(string region, string country)
        {
            return db.Proxies.Where(p => string.IsNullOrEmpty(region) || p.Region == region)
                             .Where(p => string.IsNullOrEmpty(country) || p.Country == country);
        }
        public void AddOrUpdateProxy(Proxy proxy)
        {
            if (TryGetProxy(proxy.Id, out var oldProxy))
            {
                oldProxy.Score = proxy.Score;
                oldProxy.Working = proxy.Working;
                oldProxy.LastTest = proxy.LastTest;
                oldProxy.Type = proxy.Type;
                Save();
                return;
            }
            db.Proxies.Add(proxy);
            Save();
        }
        public bool TryAddProxy(Proxy proxy)
        {
            if (TryGetProxy(proxy.Id, out var _))
                return false;
            db.Proxies.Add(proxy);
            Save(); return true;
        }
        public void DeleteProxy(uint id)
        {
            if (!TryGetProxy(id, out var p))
                return;

            db.Proxies.Remove(p);
            Save();
        }

        public void Save()
        {
            db.SaveChanges();
            Console.WriteLine("Saved!");
        }
    }
}