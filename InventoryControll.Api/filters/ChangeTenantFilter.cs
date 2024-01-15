using InventoryControll.DataDb;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace InventoryControll.Api.filters;

public class ChangeTenantFilter : IEndpointFilter
{
    /*
    private readonly ITenantService _tenantService;
    public ChangeTenantFilter(ITenantService tenantService) => _tenantService = tenantService;
    */
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        context.HttpContext.Request.Headers.TryGetValue("shop-name", out StringValues header);
        var shopNameHeader = header.FirstOrDefault();
        var service =  context.HttpContext.RequestServices.GetService<ITenantService>();
        service.SetTenant(shopNameHeader ?? "MySQL");
        var shopUnitOfWork = context.HttpContext.RequestServices.GetService<ShopUnitOfWork>();
        shopUnitOfWork.SetConnectionString(shopNameHeader);
        return next(context);
    }

}
