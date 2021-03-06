﻿using Cart.Domain.Responses.Cart;
using MediatR;
using System;

namespace Cart.Domain.Commands.Cart
{
    public class UpdateCartItemQuantityCommand : IRequest<CartExtendedResponse>
    {
        public Guid CartId { get; set; }

        public Guid CartItemId { get; set; }

        public bool IsAddOperation { get; set; }
    }
}