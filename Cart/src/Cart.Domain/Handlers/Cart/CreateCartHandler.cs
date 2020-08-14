using AutoMapper;
using Cart.Domain.Commands.Cart;
using Cart.Domain.Entities;
using Cart.Domain.Repositories;
using Cart.Domain.Responses.Cart;
using Cart.Domain.Services;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cart.Domain.Handlers.Cart
{
    public class CreateCartHandler : IRequestHandler<CreateCartCommand, CartExtendedResponse>
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICatalogService _catalogService;

        public CreateCartHandler(ICartRepository repository, IMapper mapper, ICatalogService catalogService)
        {
            _repository = repository;
            _mapper = mapper;
            _catalogService = catalogService;
        }

        public async Task<CartExtendedResponse> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            var entity = new CartSession
            {
                Id = Guid.NewGuid().ToString(),
                Items = request.ItemsIds.Select(item => new CartItem
                {
                    CartItemId = new Guid(item),
                    Quantity = 1,
                }).ToList(),
                User = new CartUser
                {
                    Email = request.UserEmail,
                },
                ValidityDate = DateTimeOffset.Now.AddMonths(2),
            };

            var result = await _repository.AddOrUpdateAsync(entity);
            var response = _mapper.Map<CartExtendedResponse>(result);
            
            var tasks = response.Items
                .Select(async x => await _catalogService.EnrichCartItem(x, cancellationToken));

            response.Items = await Task.WhenAll(tasks);
            return response;
        }
    }
}