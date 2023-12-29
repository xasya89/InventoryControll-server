using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.Domain
{
    public class StocktakingSummary
    {
        public int Id { get; set; }
        public int StocktakingId { get; set; }
        public int GoodId { get; set; }
        public decimal CountDb { get; set; }
        public decimal CountFact { get; set; }
        public decimal Price { get; set; }
    }
}
