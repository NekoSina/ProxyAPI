 using Microsoft.EntityFrameworkCore;
 namespace ProxyAPI.Models
 {
     public class ProxyContext : DbContext
     {
         public static bool isMigration = true;
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=Proxy.db");
         public ProxyContext(DbContextOptions<ProxyContext> options)
             : base(options)
         {
         }
         public DbSet<Proxy> Proxies { get; set; }
     }
 }
 
