using System;
using System.Collections.Generic;
using Cart.Domain.Entities;

namespace Cart.Domain.Responses.Cart
{
    public class CartItemResponse
    {
        public string CartItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LabelName { get; set; }
        public string Price { get; set; }
        public string PictureUri { get; set; }
        public string GenreDescription { get; set; }
        public string ArtistName { get; set; }
        public int Quantity { get; set; }
    }

    public class CartExtendedResponse
    {
        public string Id { get; set; }

        public IList<CartItemResponse> Items { get; set; }

        public CartUser User { get; set; }

        public DateTimeOffset ValidityDate { get; set; }
    }
}