﻿namespace WebApplicationlaptop.Models
{
    public class CartItemModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total
        { get { return Price * Quantity; } }
        public string Imange { get; set; }

        public CartItemModel()
        {
        }

        public CartItemModel(ProductModel product)
        {
            ProductId = product.Id;
            ProductName = product.Name;
            Price = product.Price;
            Quantity = 1;
            Imange = product.Imange;
        }
    }
}