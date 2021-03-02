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
        public IQueryable<Proxy> GetProxies_Regional(string region)
        {
            var query = _context.Proxies
                .Where(proxy => proxy.Region == region);
            return query;
        }
        public Proxy GetProxy(int id)
        {
            Proxy proxy = _context.Proxies.Find(id);;
            if(proxy == null)
                return null;
            else 
                return proxy;
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
    }    
}