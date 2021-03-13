 using Microsoft.EntityFrameworkCore;
 namespace ProxyAPI.Models
 {
     public class ProxyDbContext : DbContext
     {
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=Proxy.db");
         public ProxyDbContext(DbContextOptions<ProxyDbContext> options) : base(options) { }
         public DbSet<Proxy> Proxies { get; set; }
     }
 }
 
