using System;
using System.Linq;
using ProxyAPI.Database;
using ProxyAPI.Models;

namespace ProxyAPI.Repositories
{
    public class ProxyRepository
    {
        private readonly ProxyDbContext _context;
        public ProxyRepository(ProxyDbContext context)
        {
            _context = context;
        }
        private bool TryGetProxy(ulong id, out Proxy proxy)
        {
            proxy = _context.Proxies.Find(id);
            return proxy != null;
        }
        public Proxy GetProxy(int idx) => _context.Proxies.Find(idx);
        public IQueryable<Proxy> GetProxies(string region, string country)
        {
            return _context.Proxies.Where(p => string.IsNullOrEmpty(region) || p.Region == region)
                                   .Where(p => string.IsNullOrEmpty(country) || p.Country == country);
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
            }
            else
            {
                _context.Proxies.Add(proxy);
            }
        }
        public void DeleteProxy(int id)
        {
            Proxy p = GetProxy(id);
            _context.Proxies.Remove(p);
        }
        public void Cleardb()
        {
            foreach (var proxy in _context.Proxies)
            {
                _context.Proxies.Remove(proxy);
            }
            _context.SaveChanges();
        }
        public int GetDBLength()
        {
            return _context.Proxies.Count();
        }
        public Proxy GetFirstProxy()
        {
            return _context.Proxies.First();
        }

        public void Save() 
        {
            Console.WriteLine("Saving...");
            _context.SaveChanges();
            Console.WriteLine("Saved!");
        }
    }
}