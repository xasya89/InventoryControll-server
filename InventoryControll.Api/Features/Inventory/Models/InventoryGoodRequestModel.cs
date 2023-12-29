namespace InventoryControll.Api.Features.Inventory.Models
{
    public class InventoryGoodRequestModel
    {
        public Guid Uuid { get; set; }
        public decimal Price { get; set; }
        public decimal Count { get; set; }
    }
}
