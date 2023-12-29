using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.Domain
{
    public class StocktakingGood
    {
        public int Id { get; set; }
        public int StockTakingGroupId { get; set; }
        public int GoodId { get; set; }
        public decimal Count { get; set; }
        public decimal CountDB { get; set; } 
        public decimal CountFact { get; set; } 
        public decimal Price { get; set; }
    }
}
