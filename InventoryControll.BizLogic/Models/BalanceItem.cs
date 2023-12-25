using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.Domain.NoEntityModels
{
    public class BalanceItem
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; }
        public string GoodName { get; set; }
        public decimal Price { get; set; }
        public decimal Count { get; set; }
    }
}
