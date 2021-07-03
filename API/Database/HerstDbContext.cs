using HerstAPI.Models;
using libherst.Models;
using Microsoft.EntityFrameworkCore;

namespace HerstAPI.Database
{
    public class HerstDbContext : DbContext
    {
        public DbSet<Proxy> Proxies { get; set; }
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<WiFiClient> WiFiClients { get; set; }
        public DbSet<WiFiMac> WiFiMacs { get; set; }
        public DbSet<WiFiProbe> WiFiProbes { get; set; }
        public DbSet<WiFiAccessPoint> WiFiAccessPoints { get; set; }
        public DbSet<WiFiNetworkName> WiFiNetworkNames { get; set; }

        internal HerstDbContext() { }
        public HerstDbContext(DbContextOptions<HerstDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite(Startup.ConnectionString);
    }
}

