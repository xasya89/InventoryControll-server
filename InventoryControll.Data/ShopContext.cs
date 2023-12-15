using InventoryControll.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InventoryControll.Data
{
    public class ShopContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantService _tenantService;
        
        public DbSet<Good> Goods { get; set; }

        public ShopContext( DbContextOptions<ShopContext> opts, IConfiguration config, ITenantService service) 
        {
            _tenantService = service;
            _configuration = config;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var tenant = _tenantService.Tenant;
            var connectionStr = _configuration.GetConnectionString(tenant);
            optionsBuilder.UseMySql(connectionStr, Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.30-mysql"));
        }
    }
}
