using Calabonga.AspNetCore.AppDefinitions;
using InventoryControll.Api.filters;
using InventoryControll.BizLogic.BizLogic;
using InventoryControll.DataDb;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryControll.Api.Features.Balance
{
    public class BalanceDefinitions: AppDefinition
    {
        public override void ConfigureApplication(WebApplication app)
        {
            app.MapGet("/api-inventory/balance", async ([FromQuery] int skip, [FromQuery] int take, [FromServices] IMediator mediator) => 
                await mediator.Send(new GetBalance.Request { Skip = skip, Take = take}))
                .AddEndpointFilter<ChangeTenantFilter>();
        }
    }
}
