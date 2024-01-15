using Dapper;
using InventoryControll.BizLogic.BizLogic;
using InventoryControll.DataDb;
using InventoryControll.Domain;
using InventoryControll.Domain.NoEntityModels;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.BizLogic.Services;


public interface ICalculateBalance
{
    Task<IEnumerable<BalanceItem>> GetBalanceOld(DateTime? inventoryDateBefore = null);
    Task Calculate(int stocktakingId);
}
public class CalculateBalanceService: ICalculateBalance
{
    private ShopUnitOfWork _context;
    public CalculateBalanceService(ShopUnitOfWork context) => _context = context;

    public async Task<IEnumerable<BalanceItem>> GetBalanceOld(DateTime? inventoryDateBefore = null) =>
        await new GetCalculateBalance(_context.Connection).GetBalance(inventoryDateBefore);

    public async Task Calculate(int stocktakingId)
    {
        var transaction = _context.Connection.BeginTransaction();
        try
        {
            var stocktaking = await _context.Stocktakings.GetByGroup(stocktakingId);
            stocktaking.Create = DateOnly.FromDateTime(stocktaking.Start).ToDateTime(TimeOnly.MinValue);
            await _calculate(stocktaking);
            await _context.Stocktakings.Update(stocktaking);
            await _context.Stocktakings.Complite(stocktaking.Id);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new Exception(ex.Message, ex);
        }
    }

    private async Task _calculate(Stocktaking stocktaking)
    {
        int stocktakingId = stocktaking.Id;
        var balance = await new GetCalculateBalance(_context.Connection).GetBalance(stocktaking.Create);
        var positions = stocktaking.Groups.SelectMany(x => x.Goods).GroupBy(x => x.GoodId)
                    .Select(x => new StocktakingSummary
                    {
                        StocktakingId = stocktakingId,
                        GoodId = x.Key,
                        CountFact = x.Sum(s => s.CountFact),
                        CountDb = balance.Where(b => b.Good.Id == x.Key).FirstOrDefault()?.Balance ?? 0,
                        Price = x.First().Price
                    }).ToList();

        var zeroPositions = from b in balance
                            join p in positions on b.Good.Id equals p.GoodId into t
                            from subject in t.DefaultIfEmpty()
                            where subject is null
                            select new StocktakingSummary
                            {
                                StocktakingId = stocktakingId,
                                GoodId = b.Good.Id,
                                CountFact = 0,
                                CountDb = b.Balance,
                                Price = b.Good.Price
                            };
        positions.AddRange(zeroPositions.ToList());
        foreach (var pos in positions)
            await _context.Stocktakings.Add(pos);

        stocktaking.SumFact = positions.Sum(x => x.CountFact * x.Price);
        stocktaking.SumDb = positions.Sum(x => x.CountDb * x.Price);
        stocktaking.CountDb = positions.Sum(x => x.CountDb);
        stocktaking.CountFact = positions.Sum(x => x.CountFact);
    }
}
