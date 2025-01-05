﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplicationlaptop.Responsibilty.Validation;

namespace WebApplicationlaptop.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
        public string Name { get; set; }

        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả sản phẩm")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        public decimal Price { get; set; }
       
        [NotMapped]  // NotMapped để không lưu vào cơ sở dữ liệu
        public decimal CostPrice
        {
            get
            {
                return Price * 0.8M; // Tính giá nhập bằng 80% giá bán
            }
        }

        public string Slug { get; set; }

        public int BrandId { get; set; }
        public int CategoryId { get; set; }

        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }

        public string Imange { get; set; }

        [FileExtension(new string[] { ".jpg", ".png", ".jpeg", ".gif" })]
        [NotMapped]
        public IFormFile ImageUpload { get; set; }

        public bool IsFeatured { get; set; }

        public int Quantity { get; set; }

        public int Sold { get; set; }

        public List<ReviewModel> Reviews { get; set; }
    }
}