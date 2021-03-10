using System.Linq;
using ProxyAPI.Models;
using System.Collections.Generic;

namespace ProxyAPI
{
    public class ProxyRepository
    {   
        private readonly ProxyContext _context;
        public ProxyRepository(ProxyContext context)
        {
            _context = context;
        }
        private bool ProxyExists(int id)
        {
            return _context.Proxies.Any(p => p.ID == id);
        }
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
        public void AddProxy(Proxy proxy)
        {
            if (ProxyExists(proxy.ID))
                _context.Update(proxy);
            else
                _context.Proxies.Add(proxy);
            _context.SaveChanges();
        }
        public void UpdateProxy(Proxy proxy)
        {
            if (ProxyExists(proxy.ID))
            {
                _context.Update(proxy);
                _context.SaveChanges();
            }
        }
        public void DeleteProxy(int id)
        {
            Proxy p = GetProxy(id);
            _context.Proxies.Remove(p);
            _context.SaveChanges();
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
    }    
}