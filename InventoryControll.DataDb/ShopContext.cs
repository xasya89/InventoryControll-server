using System;
using System.Collections.Generic;
using InventoryControll.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InventoryControll.DataDb;

public partial class ShopContext : DbContext
{
    public DbSet<Good> Goods { get; set; }
    public DbSet<Barcode> Barcodes { get; set; }
    public DbSet<Arrival> Arrivals { get; set; }
    public DbSet<ArrivalGood> ArrivalGoods { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<CheckSell> CheckSells { get; set; }
    public DbSet<CheckGood> CheckGoods { get; set; }
    public ShopContext()
    {
    }

    private readonly ITenantService _tenantService;
    private readonly IConfiguration _configuration;

    public ShopContext(DbContextOptions<ShopContext> options, ITenantService tenant, IConfiguration configuration)
        : base(options)
    {
        _tenantService = tenant;
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var name = _tenantService.Tenant;
        string conStr = _configuration.GetConnectionString(name);
        optionsBuilder.UseMySql(conStr, Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.29-mysql"));
    }
        //=> optionsBuilder.UseMySql("server=localhost;database=shop;uid=root;pwd=kt38hmapq", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.29-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }

}
