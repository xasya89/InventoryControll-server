using Calabonga.AspNetCore.AppDefinitions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryControll.Api.Features.Shops
{
    public class ShopDefinition:AppDefinition
    {
        public override void ConfigureApplication(WebApplication app)
        {
            app.MapGet("/api/shops", async ([FromServices] IMediator mediator) =>
            {
                return await mediator.Send(new GetShops.GetShopsRequest());
            });
        }
    }
}
