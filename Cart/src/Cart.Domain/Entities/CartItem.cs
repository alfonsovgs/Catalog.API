using System;

namespace Cart.Domain.Entities
{
    public class CartItem
    {
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }

        public void IncreaseQuantity()
        {
            Quantity += 1;
        }

        public void DecreaseQuantity()
        {
            Quantity -= 1;
        }
    }
}