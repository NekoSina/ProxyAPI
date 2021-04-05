using System;
using System.Linq;
using HerstAPI.Database;
using HerstAPI.Models;

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
            if (TryGetProxy(proxy.ID, out var oldProxy))
            {
                oldProxy.Score = proxy.Score;
                oldProxy.Working = proxy.Working;
                oldProxy.LastTest = proxy.LastTest;
                Save();
                return;
            }
            db.Proxies.Add(proxy);
            Save();
        }
        public bool TryAddProxy(Proxy proxy)
        {
            if (TryGetProxy(proxy.ID, out var oldProxy))
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