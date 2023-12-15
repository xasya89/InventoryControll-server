using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.Domain
{
    public class Arrival
    {
        public int Id { get; set; }
        public DateTime DateArrival { get; set; }
        public IEnumerable<ArrivalGood> ArrivalGoods { get; set; }
    }
    public class ArrivalGood
    {
        public int Id { get; set; }
        public int ArrivalId { get; set; }
        public Arrival Arrival { get; set; }
        public int GoodId { get; set; }
        public Good Good { get; set; }
        public decimal Count { get; set; }
        public decimal Price { get; set; }
        public decimal PriceSell { get; set; }
    }
}
