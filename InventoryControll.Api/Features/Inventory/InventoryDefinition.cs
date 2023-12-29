using Calabonga.AspNetCore.AppDefinitions;
using InventoryControll.Api.Features.Inventory.Models;
using InventoryControll.Api.filters;
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
        }
    }
}
