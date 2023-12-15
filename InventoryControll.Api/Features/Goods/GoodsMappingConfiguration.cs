using AutoMapper;
using InventoryControll.Domain;

namespace InventoryControll.Api.Features.Goods
{
    public class GoodsMappingConfiguration:Profile
    {
        public GoodsMappingConfiguration()
        {
            CreateMap<Good, GetGoods.GoodResult>();
            CreateMap<Barcode, GetGoods.BarCodeResult>();
        }
    }
}
