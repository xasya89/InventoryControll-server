using Calabonga.AspNetCore.AppDefinitions;
using InventoryControll.Api.filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryControll.Api.Features.Goods;

public class GoodsDefinition:AppDefinition
{
    public override void ConfigureApplication(WebApplication app)
    {
        app.MapGet("/api-inventory/goods", async ([FromQuery] int? skip, [FromQuery] int? take, [FromServices] IMediator mediator) =>
         await mediator.Send(new GetGoods.GetGoodsQuery { Take = take, Skip = skip }))
            .AddEndpointFilter<ChangeTenantFilter>();

        app.MapGet("/api-inventory/goodgroups", async ([FromServices] IMediator mediator) =>
            await mediator.Send(new GetGroups.Request()))
            .AddEndpointFilter<ChangeTenantFilter>();
    }
}
