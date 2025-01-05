namespace WebApplicationlaptop.Models.ViewModel
{
    public class CartItemViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal ShippingCost { get; set; }

        public string CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }  // Thêm thuộc tính này
        public decimal DiscountPercentage { get; set; }  // Thêm thuộc tính phần trăm giảm giá
        public decimal DiscountFixedAmount { get; set; }

    }
}