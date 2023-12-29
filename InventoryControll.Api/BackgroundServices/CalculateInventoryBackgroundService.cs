

using InventoryControll.BizLogic.Services;
using InventoryControll.DataDb;
using InventoryControll.Domain;
using System.Threading;
using System.Threading.Channels;

namespace InventoryControll.Api.BackgroundServices;

public class CalculateInventoryBackgroundService : IHostedService
{
    public static Channel<Tuple<int, string>> NewInventoryChannel = Channel.CreateUnbounded<Tuple<int, string>>();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CalculateInventoryBackgroundService> _logger;

    public CalculateInventoryBackgroundService(IServiceProvider serviceProvider, ILogger<CalculateInventoryBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        getNewInventory();
    }

    public async void getNewInventory()
    {
        while (true)
        {
            (int stocktakingId, string shopDbName) = await NewInventoryChannel.Reader.ReadAsync();
            using var scope = _serviceProvider.CreateScope();
            
            var tenant = scope.ServiceProvider.GetRequiredService<ITenantService>();
            tenant.SetTenant(shopDbName);
            var unitOfWork = scope.ServiceProvider.GetRequiredService<ShopUnitOfWork>();
            var calculateBalance = scope.ServiceProvider.GetRequiredService<ICalculateBalance>();
            var transaction = unitOfWork.Connection.BeginTransaction();
            try
            {

                var stocktaking = await unitOfWork.Stocktakings.GetByGroup(stocktakingId);
                var balance = await calculateBalance.Calculate(stocktaking.Create);
                stocktaking.Create = DateOnly.FromDateTime(stocktaking.Start).ToDateTime(TimeOnly.MinValue);
                await unitOfWork.Stocktakings.Update(stocktaking);
                var positions = stocktaking.Groups.SelectMany(x => x.Goods).GroupBy(x=>x.GoodId)
                    .Select(x => new StocktakingSummary
                    {
                        StocktakingId = stocktakingId,
                        GoodId=x.Key,
                        CountFact = x.Sum(s=>s.CountFact),
                        CountDb=balance.Where(b=>b.Id==x.Key).FirstOrDefault()?.Count ?? 0,
                        Price = x.First().Price
                    });
                foreach(var pos in positions) 
                    await unitOfWork.Stocktakings.Add(pos);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("Calculate background stocktaking\n" + ex.Message);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
