using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.Domain
{
    public class Stocktaking
    {
        public int Id { get; set; }
        public int Num { get; set; }
        public Guid Uuid { get; set; }
        public DateTime Create { get; set; }
        public DateTime Start { get; set; }
        public double CountDb { get; set; }
        public double CountFact { get; set; }
        public decimal SumDb { get; set; }
        public decimal SumFact { get; set; }
        public decimal CashMoneyFact { get; set; }
        public decimal CashMoneyDb { get; set; }

        public IEnumerable<StockTakingGroup> Groups { get; set; }
        public IEnumerable<StocktakingSummary> Summary { get; set; }
    }
}
