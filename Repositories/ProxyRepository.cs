using System;
using System.Linq;
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
        private bool ProxyExists(ulong id) => _context.Proxies.Any(p => p.ID == id);
        public Proxy GetProxy(int idx)
        {
            Proxy proxy = _context.Proxies.Find(idx);
            if(proxy == null)
                return null;
            else 
                return proxy;
        }
        public IQueryable GetProxies(string region, string country)
        {
           var proxies = _context.Proxies.Where(p=> string.IsNullOrEmpty(region) || p.Region == region)
                                         .Where(p=> string.IsNullOrEmpty(country) || p.Country == country);
           return proxies;
        }
        public void AddOrUpdateProxy(Proxy proxy)
        {
            if (ProxyExists(proxy.ID))
                _context.Update(proxy);
            else
                _context.Proxies.Add(proxy);
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

        public void Save() => _context.SaveChanges();
    }    
}