using Microsoft.EntityFrameworkCore;

namespace ProxyAPI.Models
{
    public class ProxyContext : DbContext
    {
        public ProxyContext(DbContextOptions<ProxyContext> options)
            : base(options)
        {
        }

        public DbSet<Proxy> Proxies { get; set; }
    }
}