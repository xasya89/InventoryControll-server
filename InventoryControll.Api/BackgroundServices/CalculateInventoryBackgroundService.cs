

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
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var tenant = scope.ServiceProvider.GetRequiredService<ITenantService>();
                tenant.SetTenant(shopDbName);
                var calculateBalance = scope.ServiceProvider.GetRequiredService<ICalculateBalance>();
                await calculateBalance.Calculate(stocktakingId);
            }
            catch(Exception ex)
            {
                _logger.LogError("background stocktaking calc complite \n" + ex.Message);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
