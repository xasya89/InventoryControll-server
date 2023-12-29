namespace InventoryControll.Api.Features.Inventory.Models
{
    public class InventoryRequestModel
    {
        public Guid Uuid { get; set; }
        public string Start { get; set; }
        public decimal CashMoneyFact { get; set; }
        public IEnumerable<InventoryGroupRequestModel> Groups { get; set; }
    }
}
