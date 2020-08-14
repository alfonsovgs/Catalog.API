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
    public class UpdateCartItemQuantity : IRequestHandler<UpdateCartItemQuantityCommand, CartExtendedResponse>
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICatalogService _catalogService;

        public UpdateCartItemQuantity(ICartRepository repository, IMapper mapper, ICatalogService catalogService)
        {
            _repository = repository;
            _mapper = mapper;
            _catalogService = catalogService;
        }

        public async Task<CartExtendedResponse> Handle(UpdateCartItemQuantityCommand request,
            CancellationToken cancellationToken)
        {
            var cartDetail = await _repository.GetAsync(request.CartId);

            if (request.IsAddOperation)
            {
                cartDetail.Items.FirstOrDefault(x => x.CartItemId == request.CartItemId)?.IncreaseQuantity();
            }
            else
            {
                cartDetail.Items.FirstOrDefault(x => x.CartItemId == request.CartItemId)?.DecreaseQuantity();
            }

            var cartItemList = cartDetail.Items.ToList();
            cartItemList.RemoveAll(x => x.Quantity <= 0);

            cartDetail.Items = cartItemList;
            await _repository.AddOrUpdateAsync(cartDetail);

            var response = _mapper.Map<CartExtendedResponse>(cartDetail);
            var tasks = response.Items
                .Select(async x => await _catalogService.EnrichCartItem(x, cancellationToken));

            response.Items = await Task.WhenAll(tasks);
            return response;
        }
    }
}