using Dapper;
using InventoryControll.Domain;
using InventoryControll.Domain.NoEntityModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.BizLogic.BizLogic
{
    public class GetCalculateBalance
    {
        private readonly MySqlConnection _connection;
        public GetCalculateBalance(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<BalanceItem>> GetBalance(DateTime? inventoryDateBefore = null)
        {
            List<BalanceItem> result = (await _connection.QueryAsync<Good>($"SELECT * FROM goods WHERE IsDeleted=0"))
                .Select(x=>new BalanceItem { Good = x}).ToList();

            (DateTime? lastDateAct, IEnumerable<GoodIdAndCount> inventoryBalance) = await getInventoryBalance(inventoryDateBefore);
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

        private async Task<Tuple<DateTime?, IEnumerable<GoodIdAndCount>>> getInventoryBalance(DateTime? inventoryDateBefore)
        {
            var lastDateId = await _connection.QueryFirstOrDefaultAsync<int?>($"SELECT MAX(id) FROM stocktakings WHERE isSuccess=1 AND Status=2");
            if (inventoryDateBefore != null)
                lastDateId = await _connection.QueryFirstOrDefaultAsync<int?>($"SELECT MAX(s.id) FROM stocktakings s WHERE s.Create<@DateBefore AND s.isSuccess=1 AND s.Status=2",
                    new { DateBefore = inventoryDateBefore });
            if (lastDateId == null)
                return new Tuple<DateTime?, IEnumerable<GoodIdAndCount>>(null, Enumerable.Empty<GoodIdAndCount>());
            var inventoryDate = await _connection.QueryFirstOrDefaultAsync<DateTime?>($"SELECT s.Create FROM stocktakings s WHERE s.id={lastDateId}");
            var items = await _connection.QueryAsync<GoodIdAndCount>($@"SELECT g.id, g.CountFact AS Count FROM stocktakinggroups gr INNER JOIN stocktakinggoods g ON gr.id=g.StockTakingGroupId
                    WHERE gr.StocktakingId={lastDateId}");
            return new Tuple<DateTime?, IEnumerable<GoodIdAndCount>>(inventoryDate, items);
        }

        private async Task<IEnumerable<GoodIdAndCount>> getArrivals(DateTime? start) => await _connection.QueryAsync<GoodIdAndCount>($@"SELECT g.GoodId AS id, SUM(g.Count) AS Count
                FROM arrivals a, arrivalgoods g WHERE a.id=g.ArrivalId AND a.DateArrival>=@Start
                GROUP BY g.GoodId", new { Start = start });

        private async Task<IEnumerable<GoodIdAndCount>> getWriteofs(DateTime? start) => await _connection.QueryAsync<GoodIdAndCount>($@"SELECT g.GoodId AS id, SUM(g.Count) AS Count
            FROM writeofs w, writeofgoods g WHERE w.id=g.WriteofId AND w.DateWriteof>=@Start
            GROUP BY g.GoodId", new { Start = start });

        private async Task deleteDoubleShiftPositions(DateTime? start)
        {
            IEnumerable<DoubleCheckSellItem> ids = await _connection.QueryAsync<DoubleCheckSellItem>(@"SELECT id, COUNT(*) c FROM checksells WHERE DateCreate>=@Start 
                GROUP BY DateCreate HAVING c>1", new { Start = start });
            while (ids.Any())
            {
                await _connection.ExecuteAsync("DELETE FROM checksells WHERE id IN @Ids", new { Ids = ids.Select(x=>x.Id) });

                ids = await _connection.QueryAsync<DoubleCheckSellItem>(@"SELECT id, COUNT(*) c FROM checksells WHERE DateCreate>=@Start 
                GROUP BY DateCreate HAVING c>1", new { Start = start });
            };
        }

        private async Task<IEnumerable<GoodIdAndCount>> getShifts(DateTime? start) => await _connection.QueryAsync<GoodIdAndCount>($@"SELECT g.GoodId AS id, SUM(g.Count) AS Count
            FROM shifts s, checksells c, checkgoods g WHERE s.id=c.ShiftId AND c.id=g.CheckSellId AND s.Start>=@Start
            GROUP BY g.GoodId", new { Start = start });

        private void mergeBalancePlus(List<BalanceItem> original, IEnumerable<GoodIdAndCount> dist)
        {
            foreach (var item in dist)
            {
                var item2 = original.Where(x => x.Good.Id == item.Id).FirstOrDefault();
                if (item2 != null)
                    item2.Balance += item.Count;
            }
        }

        private void mergeBalanceMinus(List<BalanceItem> original, IEnumerable<GoodIdAndCount> dist)
        {
            foreach (var item in dist)
            {
                var item2 = original.Where(x => x.Good.Id == item.Id).FirstOrDefault();
                if (item2 != null)
                    item2.Balance -= item.Count;
            }
        }

        private class DoubleCheckSellItem
        {
            public int Id { get; set; }
        }

        private class GoodIdAndCount
        {
            public int Id { get; set; }
            public decimal Count { get; set; }
        }
    }
}
