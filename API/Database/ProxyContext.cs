using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProxyAPI.Models;

namespace ProxyAPI.Database
 {
     public class ProxyDbContext : DbContext
     {
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=Proxy.db");
        public ProxyDbContext(DbContextOptions<ProxyDbContext> options) : base(options) { }
        internal ProxyDbContext(){}
        public DbSet<Proxy> Proxies { get; set; }
        public DbSet<UserInfo> Users { get; set; }
     }
 }
 
