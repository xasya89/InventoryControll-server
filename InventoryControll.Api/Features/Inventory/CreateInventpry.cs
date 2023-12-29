using InventoryControll.Api.BackgroundServices;
using InventoryControll.Api.Features.Inventory.Models;
using InventoryControll.DataDb;
using InventoryControll.Domain;
using MediatR;
using MySql.Data.MySqlClient;

namespace InventoryControll.Api.Features.Inventory
{
    public class CreateInventory
    {
        public class Request: IRequest<Result>
        {
            public InventoryRequestModel Inventory { get; set; }
        }
        public class Result
        {
            public string Message { get; set; } = "ok";
        }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly ITenantService _tenantService;
            private readonly ShopUnitOfWork _context;
            public Handler(ITenantService tenantService, ShopUnitOfWork context)
            {
                _tenantService = tenantService;
                _context = context;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var uuids = request.Inventory.Groups.SelectMany(x=>x.Goods).Select(x=>x.Uuid).ToList();
                var goodsDb = await _context.Goods.GetAll();
                using MySqlTransaction transaction = _context.Connection.BeginTransaction();
                try
                {

                    var start = DateTime.ParseExact(request.Inventory.Start, "yyyy-MM-dd'T'HH:mm:ss", null);
                    var goodSum = request.Inventory.Groups.SelectMany(x => x.Goods).Sum(x => x.Price * x.Count);
                    var stocktaking = new Stocktaking
                    {
                        Uuid = request.Inventory.Uuid,
                        Create = DateOnly.FromDateTime(start).ToDateTime(TimeOnly.MinValue),
                        Start = start,
                        SumFact = goodSum,
                        CashMoneyFact = request.Inventory.CashMoneyFact
                    };
                    var stocktakingId = await _context.Stocktakings.Add(stocktaking);
                    foreach (var group in request.Inventory.Groups)
                    {
                        var newGroup = new StockTakingGroup { Name = group.Name, StocktakingId = stocktakingId };
                        var groupId = await _context.Stocktakings.Add(newGroup);
                        foreach (var good in group.Goods)
                        {
                            int goodId = goodsDb.Where(x => x.Uuid == good.Uuid).First().Id;
                            await _context.Stocktakings.Add(new StocktakingGood
                            {
                                StockTakingGroupId = groupId,
                                GoodId = goodId,
                                CountFact = good.Count,
                                Price = good.Price
                            });
                        }
                    }

                    transaction.Commit();

                    await CalculateInventoryBackgroundService.NewInventoryChannel.Writer.WriteAsync(new Tuple<int, string>(stocktakingId, _tenantService.Tenant));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Ошибка сохранения инвенторизации", ex);
                }
                return new Result { };
            }
        }
    }
}
