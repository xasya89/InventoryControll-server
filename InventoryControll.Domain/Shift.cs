using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InventoryControll.Domain
{
    public class Shift
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime? Stop { get; set; }
        public decimal SumAll { get; set; }
        public decimal SumNoElectron { get; set; }
        public decimal SumElectron { get; set; }
        public decimal SumSell { get; set; }
        public decimal SumReturnCash { get; set; }
        public decimal SumReturnElectron { get; set; }
        public IEnumerable<CheckSell> CheckSells { get; set; }
    }

    public class CheckSell
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public Shift Shift { get; set; }
        public DateTime DateCreate { get; set; } = DateTime.Now;
        public TypeSell TypeSell { get; set; } = TypeSell.Sell;
        public bool IsElectron { get; set; }
        public decimal SumCash { get; set; }
        public decimal SumElectron { get; set; }
        public decimal Sum { get; set; }
        public decimal SumDiscont { get; set; }
        public decimal SumAll { get; set; }
        public IEnumerable<CheckGood> CheckGoods { get; set; }
        
    }

    public class CheckGood
    {
        public int Id { get; set; }
        public decimal Count { get; set; }
        public decimal Price { get; set; }
        public int GoodId { get; set; }
        public Good Good { get; set; }
        public int CheckSellId { get; set; }
        public CheckSell CheckSell { get; set; }
    }

    public enum TypeSell
    {
        Sell = 0,
        Return = 1
    }
}
