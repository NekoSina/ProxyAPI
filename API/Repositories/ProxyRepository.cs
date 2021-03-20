using System;
using System.Linq;
using ProxyAPI.Database;
using ProxyAPI.Models;

namespace ProxyAPI.Repositories
{
    public class ProxyRepository
    {
        private readonly ProxyDbContext db;
        public ProxyRepository(ProxyDbContext context) => db = context;

        private bool TryGetProxy(ulong id, out Proxy proxy)
        {
            proxy = db.Proxies.Find(id);
            return proxy != null;
        }
        public IQueryable<Proxy> GetProxies(string region, string country, int hoursSinceTest) 
        {
            return db.Proxies.Where(p => string.IsNullOrEmpty(region) || p.Region == region)
                       .Where(p => string.IsNullOrEmpty(country) || p.Country == country)
                       .Where(p=> hoursSinceTest == 0 || p.LastTest.AddHours(hoursSinceTest) > DateTime.UtcNow);
        }
        public void AddOrUpdateProxy(Proxy proxy)
        {
            if (TryGetProxy(proxy.ID, out var oldProxy))
            {
                oldProxy.IP = proxy.IP;
                oldProxy.Port = proxy.Port;
                oldProxy.Region = proxy.Region;
                oldProxy.Country = proxy.Country;
                oldProxy.City = proxy.City;
                oldProxy.LastTest = proxy.LastTest;
                Save();
                return;
            }
            db.Proxies.Add(proxy);
            Save();
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