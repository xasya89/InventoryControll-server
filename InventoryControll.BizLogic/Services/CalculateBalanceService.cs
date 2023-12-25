using InventoryControll.DataDb;
using InventoryControll.Domain.NoEntityModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.BizLogic.Services
{
    public class CalculateBalanceService
    {
        private ShopContext _context;
        public CalculateBalanceService(ShopContext context) => _context = context;

        public async Task<IEnumerable<BalanceItem>> Calculate()
        {
            List<BalanceItem> result = await _context.Database.SqlQuery<BalanceItem>($"SELECT id, Uuid, price FROM goods WHERE IsDeleted=0").ToListAsync();

            (DateTime? lastDateAct, IEnumerable<BalanceItem> inventoryBalance) = await getInventoryBalance();
            mergeBalancePlus(result, inventoryBalance);

            var arrivalItems = await getArrivals(lastDateAct);
            mergeBalancePlus(result, arrivalItems);



            return result;
        }

        private async Task<Tuple<DateTime?, IEnumerable<BalanceItem>>> getInventoryBalance()
        {
            var lastDateId = await _context.Database.SqlQuery<int?>($"SELECT MAX(id) FROM stocktakings WHERE isSuccess=1 AND Status=2")
                .FirstOrDefaultAsync();
            if (lastDateId == null)
                return new Tuple<DateTime?, IEnumerable<BalanceItem>>(null, Enumerable.Empty<BalanceItem>());
            var inventoryDate = await _context.Database.SqlQuery<DateTime?>($"SELECT Create FROM stocktakings WHERE id={lastDateId}").FirstAsync();
            var items = await _context.Database
                .SqlQuery<BalanceItem>($@"SELECT g.id, g.CountFact AS Count FROM stocktakinggroups gr INNER JOIN stocktakinggoods g ON gr.id=g.StockTakingGroupId
                    WHERE gr.StocktakingId={lastDateId}")
                .ToArrayAsync();
            return new Tuple<DateTime?, IEnumerable<BalanceItem>>(inventoryDate, items);
        }

        private async Task<IEnumerable<BalanceItem>> getArrivals(DateTime? start)
        {
            var dateSqlFormat = start?.ToString("dd.MM.yyyy HH:MM");// "01.01." + DateTime.Now.ToString("yyyy");
            return await _context.Database.SqlQuery<BalanceItem>($@"SELECT g.GoodId AS id, SUM(g.Count) AS Count
                FROM arrivals a, arrivalgoods g WHERE a.id=g.ArrivalId AND a.DateArrival>=str_to_date('{start}','%d.%m.%Y %H:%i')
                GROUP BY g.GoodId").ToListAsync();
        }
            

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
    }
}
