using Calabonga.AspNetCore.AppDefinitions;
using InventoryControll.Api.BackgroundServices;
using InventoryControll.Api.Features.Inventory.Models;
using InventoryControll.Api.filters;
using InventoryControll.BizLogic.Services;
using InventoryControll.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryControll.Api.Features.Inventory
{
    public class InventoryDefinition:AppDefinition
    {
        public override void ConfigureApplication(WebApplication app)
        {
            app.MapPost("/api-inventory/inventory", async ([FromBody] InventoryRequestModel model, [FromServices] IMediator service) =>
            {
                await service.Send(new CreateInventory.Request { Inventory = model });
            })
                .AddEndpointFilter<ChangeTenantFilter>();

            app.MapGet("/api-inventory/recalc-summary/{stocktakingId}", async (int stocktakingId, [FromServices] ICalculateBalance recalcService) =>
                await recalcService.Calculate(stocktakingId))
                .AddEndpointFilter<ChangeTenantFilter>();
        }
    }
}
