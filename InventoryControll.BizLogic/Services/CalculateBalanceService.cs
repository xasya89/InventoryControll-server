using Dapper;
using InventoryControll.DataDb;
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
    Task<IEnumerable<BalanceItem>> Calculate(DateTime? inventoryDateBefore = null);
}
public class CalculateBalanceService: ICalculateBalance
{
    private MySqlConnection _connection;
    public CalculateBalanceService(ShopUnitOfWork context) => _connection = context.Connection;

    public async Task<IEnumerable<BalanceItem>> Calculate(DateTime? inventoryDateBefore = null)
    {
        List<BalanceItem> result = (await _connection.QueryAsync<BalanceItem>($"SELECT id, Uuid, price FROM goods WHERE IsDeleted=0")).ToList();

        (DateTime? lastDateAct, IEnumerable<BalanceItem> inventoryBalance) = await getInventoryBalance(inventoryDateBefore);
        mergeBalancePlus(result, inventoryBalance);

        var arrivalItems = await getArrivals(lastDateAct);
        mergeBalancePlus(result, arrivalItems);

        var writeofItems = await getWriteofs(lastDateAct);
        mergeBalanceMinus(result, writeofItems);

        await deleteDoubleShiftPositions(lastDateAct);
        var shiftsItems = await getShifts(lastDateAct);
        mergeBalanceMinus(result, shiftsItems);

        return result;
    }

    private async Task<Tuple<DateTime?, IEnumerable<BalanceItem>>> getInventoryBalance(DateTime? inventoryDateBefore)
    {
        var lastDateId = await _connection.QueryFirstOrDefaultAsync<int?>($"SELECT MAX(id) FROM stocktakings WHERE isSuccess=1 AND Status=2");
        if(inventoryDateBefore != null)
            lastDateId = await _connection.QueryFirstOrDefaultAsync<int?>($"SELECT MAX(s.id) FROM stocktakings s WHERE s.Create<@DateBefore AND s.isSuccess=1 AND s.Status=2",
                new { DateBefore= inventoryDateBefore });
        if (lastDateId == null)
            return new Tuple<DateTime?, IEnumerable<BalanceItem>>(null, Enumerable.Empty<BalanceItem>());
        var inventoryDate = await _connection.QueryFirstOrDefaultAsync<DateTime?>($"SELECT s.Create FROM stocktakings s WHERE s.id={lastDateId}");
        var items = await _connection.QueryAsync<BalanceItem>($@"SELECT g.id, g.CountFact AS Count FROM stocktakinggroups gr INNER JOIN stocktakinggoods g ON gr.id=g.StockTakingGroupId
                    WHERE gr.StocktakingId={lastDateId}");
        return new Tuple<DateTime?, IEnumerable<BalanceItem>>(inventoryDate, items);
    }

    private async Task<IEnumerable<BalanceItem>> getArrivals(DateTime? start)=> await _connection.QueryAsync<BalanceItem>($@"SELECT g.GoodId AS id, SUM(g.Count) AS Count
                FROM arrivals a, arrivalgoods g WHERE a.id=g.ArrivalId AND a.DateArrival>=@Start
                GROUP BY g.GoodId", new { Start = start });

    private async Task<IEnumerable<BalanceItem>> getWriteofs(DateTime? start) => await _connection.QueryAsync<BalanceItem>($@"SELECT g.GoodId AS id, SUM(g.Count) AS Count
            FROM writeofs w, writeofgoods g WHERE w.id=g.WriteofId AND w.DateWriteof>=@Start
            GROUP BY g.GoodId", new { Start = start });

    private async Task deleteDoubleShiftPositions(DateTime? start)
    {
        IEnumerable<DoubleCheckSellItem> ids = await _connection.QueryAsync<DoubleCheckSellItem>(@"SELECT id, COUNT(*) c FROM checksells WHERE DateCreate>=@Start 
                GROUP BY DateCreate HAVING c>0", new { Start = start });
        while(ids.Any() )
        {
            await _connection.ExecuteAsync("DELETE FROM checksells WHERE id IN @Ids", new { Ids = ids });

            ids = await _connection.QueryAsync<DoubleCheckSellItem>(@"SELECT id, COUNT(*) c FROM checksells WHERE DateCreate>=@Start 
                GROUP BY DateCreate HAVING c>0", new { Start = start });
        };
    }

    private async Task<IEnumerable<BalanceItem>> getShifts(DateTime? start) => await _connection.QueryAsync<BalanceItem>($@"SELECT g.GoodId AS id, SUM(g.Count) AS Count
            FROM shifts s, checksells c, checkgoods g WHERE s.id=c.ShiftId AND c.id=g.CheckSellId AND s.Start>=@Start
            GROUP BY g.GoodId", new { Start = start });

    private void mergeBalancePlus(List<BalanceItem> original, IEnumerable<BalanceItem> dist)
    {
        foreach(var item in dist)
        {
            var item2 = original.Where(x => x.Id == item.Id).FirstOrDefault();
            if (item2 != null)
                item2.Count += item.Count;
        }
    }

    private void mergeBalanceMinus(List<BalanceItem> original, IEnumerable<BalanceItem> dist)
    {
        foreach (var item in dist)
        {
            var item2 = original.Where(x => x.Id == item.Id).FirstOrDefault();
            if (item2 != null)
                item2.Count -= item.Count;
        }
    }

    private class DoubleCheckSellItem
    {
        public int Id { get; set; }
    }
}
