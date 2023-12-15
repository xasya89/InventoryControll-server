using AutoMapper;
using InventoryControll.DataDb;
using InventoryControll.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryControll.Api.Features.Goods
{
    public class GetGoods
    {
        public class GetGoodsQuery : IRequest<Result>
        {
            public int? Skip {  get; set; }
            public int? Take { get; set; }
        }

        public class Result
        {
            public int Count { get; set; }
            public GoodResult[]? Goods { get; set; }
        }
        public class GoodResult
        {
            public Guid Uuid { get; set; }
            public string Name { get; set; }
            public SpecialTypes SpecialType { get; set; }
            public UnitType Unit { get; set; }
            public decimal Price { get; set; }
            public bool IsDeleted { get; set; }
            public IEnumerable<BarCodeResult> Barcodes { get; set; }
        }

        public class BarCodeResult
        {
            public string Code { get; set; }
        }

        public class Handler : IRequestHandler<GetGoodsQuery, Result>
        {
            private readonly ShopContext _context;
            private readonly IMapper _mapper;
            public Handler(ShopContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result> Handle(GetGoodsQuery request, CancellationToken cancellationToken)
            {
                var goodCount = await _context.Goods.CountAsync();
                var goods = await _context.Goods.Include(x => x.Barcodes)
                    .Skip(request.Skip ?? 0)
                    .Take(request.Take ?? 100)
                    .AsNoTracking().ToArrayAsync();
                var goodsResult = _mapper.Map<GoodResult[]?>(goods);
                return new Result { Count = goodCount, Goods = goodsResult };
            }
        }
    }
}
