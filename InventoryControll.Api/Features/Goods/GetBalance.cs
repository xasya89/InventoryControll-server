using InventoryControll.DataDb;
using MediatR;

namespace InventoryControll.Api.Features.Goods
{
    public class GetBalance
    {
        public class GetBalanceQuery:IRequest<BalanceResult> { }
        public class BalanceResult
        {
            public Guid Uuid { get; set; }
            public decimal Balance { get; set; }
        }

        public class Handler : IRequestHandler<GetBalanceQuery, BalanceResult>
        {
            private readonly ShopContext _context;
            public Handler(ShopContext context)
            {
                _context = context;
            }
            public async Task<BalanceResult> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
