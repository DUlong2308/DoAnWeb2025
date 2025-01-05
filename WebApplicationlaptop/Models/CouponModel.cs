using System.ComponentModel.DataAnnotations;

namespace WebApplicationlaptop.Models
{
    public class CouponModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu tên coupon")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Yêu cầu tên mô tả")]
        public string Description { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateExpired { get; set; }

        [Required(ErrorMessage = "Yêu cầu số lượng coupon")]
        public int Quantity { get; set; }

        public int Status { get; set; }

      
        public decimal? PriceDecrease { get; set; }
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải nằm trong khoảng từ 0 đến 100.")]
        public double? DiscountPercentage { get; set; }
        public int DiscountType { get; set; }
    }
   
}


