namespace InventoryControll.Api.Features.Inventory.Models
{
    public class InventoryGroupRequestModel
    {
        public string Name { get; set; }
        public IEnumerable<InventoryGoodRequestModel> Goods { get; set; }
    }
}
