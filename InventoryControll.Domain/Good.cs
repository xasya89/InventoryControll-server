using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.Domain;

public class Good
{
    public int Id { get; set; }
    public Guid Uuid { get; set; }
    public string Name { get; set; }
    public SpecialTypes SpecialType { get; set; }
    public UnitType Unit { get; set; }
    public decimal Price { get; set; }
    public bool IsDeleted { get; set; }

    public IEnumerable<Barcode> Barcodes { get; set; }
    public IEnumerable<ArrivalGood> ArrivalGoods { get; set;}
    public IEnumerable<CheckGood> CheckGoods { get; set; }
}
