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
            return _context.Proxies.Any(e => e.ID == id);
        }
        public Proxy GetProxy(int id)
        {
            
            Proxy proxy = _context.Proxies.Find(id);;
            if(proxy == null)
                return null;
            else 
                return proxy;
        }
        public IQueryable GetProxies(string region, string country)
        {
            var query = _context.Proxies
                .Where(proxy => (proxy.Region == region && proxy.Country == country));
            return query;
        }
        public void AddProxy(Proxy proxy)
        {
            if (!ProxyExists(proxy.ID))
            {
                _context.Proxies.Add(proxy);
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