using MediatR;

namespace InventoryControll.Api.Features.Shops
{
    public class GetShops
    {
        public class GetShopsRequest : IRequest<ShopResult[]>
        {

        }

        public class ShopResult
        {
            public string DbName { get; set; }
            public string Name { get; set; }
        }

        public class Handler : IRequestHandler<GetShopsRequest, ShopResult[]>
        {
            private readonly IConfiguration _configuration;
            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task<ShopResult[]> Handle(GetShopsRequest request, CancellationToken cancellationToken)
            {
                return _configuration.GetSection("Shops").Get<ShopResult[]>();
            }
        }
    }
}
