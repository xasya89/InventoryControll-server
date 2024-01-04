using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.Domain.NoEntityModels
{
    public class BalanceItem
    {
        public Good Good { get; set; }
        public decimal Balance { get; set; }
    }
}
