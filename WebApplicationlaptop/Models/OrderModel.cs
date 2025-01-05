using System.ComponentModel.DataAnnotations;

namespace WebApplicationlaptop.Models
{
    public class OrderModel
    {
        public int Id { get; set; }

        [Required]
        public string OrderCode { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public int Status { get; set; }

        public string FullName  { get; set; }  // Thêm thuộc tính này

        public string? PaymentMethod { get; set; }

        public string? CouponCode { get; set; }

        public List<OrderDetail> OrderDetail { get; set; }

        public decimal? ShippingCost { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal TotalAmount
        {
            get
            {
                // Tính tổng doanh thu = (số lượng * giá) của các sản phẩm trong OrderDetail + phí vận chuyển
                return OrderDetail?.Sum(od => od.Quantity * od.Price) ?? 0 + (ShippingCost ?? 0);
            }
        }
    }
}