using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.Domain
{
    public class StockTakingGroup
    {
        public int Id { get; set; }
        public int StocktakingId { get; set; }
        public string Name { get; set; }
        public IEnumerable<StocktakingGood> Goods { get; set; }
    }
}
