using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.DataDb;

public interface ITenantService
{
    string Tenant { get; }

    void SetTenant(string tenant);

    string[] GetTenants() => ["Shop1", "Shop2"];
}

public class TenantService : ITenantService
{
    private string _tenant;
    public string Tenant => _tenant;

    public void SetTenant(string tenant) => _tenant = tenant;
}