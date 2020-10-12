using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cart.Domain.Commands.Cart;
using Cart.Domain.Repositories;
using Cart.Domain.Responses.Cart;
using Cart.Domain.Services;
using MediatR;

namespace Cart.Domain.Handlers.Cart
{
    public class GetCartHandler : IRequestHandler<GetCartCommand, CartExtendedResponse>
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICatalogService _catalogService;

        public GetCartHandler(ICartRepository repository, IMapper mapper, ICatalogService catalogService)
        {
            _repository = repository;
            _mapper = mapper;
            _catalogService = catalogService;
        }

        public async Task<CartExtendedResponse> Handle(GetCartCommand request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAsync(request.Id);
            var extendedResponse = _mapper.Map<CartExtendedResponse>(result);

            var tasks = extendedResponse.Items
                .Select(async item => await _catalogService.EnrichCartItem(item, cancellationToken));

            extendedResponse.Items = await Task.WhenAll(tasks);
            return extendedResponse;
        }
    }
}