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
        public Proxy GetProxy(int id)
        {
            
            Proxy proxy = _context.Proxies.Find(id);
            if(proxy == null)
                return null;
            else 
                return proxy;
        }
        public IQueryable GetProxies(string region, string country)
        {
            if (region == null && country == null)
            {
                var rowcount = _context.Proxies.Count();
                var firstid = _context.Proxies.First().ID;
                int randomid = firstid + Random.RandomNumber(rowcount);
                return _context.Proxies.Where(proxy => proxy.ID == randomid);
            }
            else 
            {
                return  _context.Proxies
                    .Where(proxy => (proxy.Region == region && proxy.Country == country));
            }
             
        }
        public void AddProxy(Proxy proxy)
        {
            if (ProxyExists(proxy.ID))
            {
                _context.SaveChanges();
            }
            else
            {
                _context.Proxies.Add(proxy);
                _context.SaveChanges();
            }
        }
        public void UpdateProxy(Proxy proxy)
        {
            if (ProxyExists(proxy.ID))
            {
                _context.Update(proxy);
                _context.SaveChanges();
            }
        }
        public void DeleteProxy(Proxy proxy)
        {
            _context.Remove(proxy);
        }
        public void Cleardb()
        {
            foreach (var proxy in _context.Proxies)
            {
                DeleteProxy(proxy);
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