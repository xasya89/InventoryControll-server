using InventoryControll.BizLogic.BizLogic;
using InventoryControll.DataDb;
using InventoryControll.Domain.NoEntityModels;
using MediatR;

namespace InventoryControll.Api.Features.Balance
{
    public class GetBalance
    {
        public class Request:IRequest<Response>
        {
            public DateTime? inventoryDateBefore { get; set; }
            public int Skip { get; set; }
            public int Take { get; set; }
        }

        public class Response
        {
            public int Count { get; set; }
            public IEnumerable<ResponseBalanceItem> Items { get; set; }
        }

        public class ResponseBalanceItem
        {
            public Guid Uuid { get; set; }
            public decimal Balance { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ITenantService _tenantService;
            private readonly ShopUnitOfWork _context;
            public Handler(ITenantService tenantService, ShopUnitOfWork context)
            {
                _tenantService = tenantService;
                _context = context;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var goodCount = await _context.Goods.CountWithoutDeleted();
                var balance = await new GetCalculateBalance(_context.Connection).GetBalance();
                return new Response { Count = goodCount, Items = balance.Skip(request.Skip).Take(request.Take)
                    .Select(x=>new ResponseBalanceItem { Uuid = x.Good.Uuid, Balance = x.Balance }).ToArray() };
            }
        }
    }
}
