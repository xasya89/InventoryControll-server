using AutoMapper;
using InventoryControll.DataDb;
using InventoryControll.Domain;
using MediatR;

namespace InventoryControll.Api.Features.Goods;

public class GetGroups
{
    public class Request : IRequest<IEnumerable<GoodGroup>>
    {

    }

    public class Handler : IRequestHandler<Request, IEnumerable<GoodGroup>>
    {
        private readonly ShopUnitOfWork _context;
        private readonly IMapper _mapper;
        public Handler(ShopUnitOfWork context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<GoodGroup>> Handle(Request request, CancellationToken cancellationToken) =>
            await _context.GoodGroups.Get();
    }
}
