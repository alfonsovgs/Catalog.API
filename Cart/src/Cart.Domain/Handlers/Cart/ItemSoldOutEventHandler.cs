using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cart.Domain.Entities;
using Cart.Domain.Events;
using Cart.Domain.Repositories;
using MediatR;

namespace Cart.Domain.Handlers.Cart
{
    public class ItemSoldOutEventHandler : IRequestHandler<ItemSoldOutEvent>
    {
        private readonly ICartRepository _cartRepository;

        public ItemSoldOutEventHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Unit> Handle(ItemSoldOutEvent request, CancellationToken cancellationToken)
        {
            var cartIds = _cartRepository.GetCarts().ToList();

            var tasks = cartIds.Select(async x =>
            {
                var cart = await _cartRepository.GetAsync(new Guid(x));
                await RemoveItemsInCart(request.Id, cart);
            });

            await Task.WhenAll(tasks);
            return Unit.Value;
        }

        private async Task RemoveItemsInCart(string itemToRemove, CartSession cartSession)
        {
            if (string.IsNullOrEmpty(itemToRemove)) return;

            var toDelete = cartSession
                ?.Items
                ?.Where(x => x.CartItemId.ToString() == itemToRemove)
                .ToList();

            if (toDelete?.Any() ?? true) return;

            foreach (var item in toDelete)
                cartSession.Items?.Remove(item);

            await _cartRepository.AddOrUpdateAsync(cartSession);
        }
    }
}
