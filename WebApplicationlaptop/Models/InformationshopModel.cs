using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationlaptop.Models
{
    public class InformationshopModel
    {
        public int Id { get; set; } // Khóa chính

        [Required(ErrorMessage = "Yêu cầu nhập tiêu đề website")]
        public string Name { get; set; } // Tên shop

        [Required(ErrorMessage = "Yêu cầu nhập bản đồ")]
        public string Map { get; set; } // Địa chỉ bản đồ (Google Map hoặc link nhúng)

        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; } // Email liên hệ

        [Required(ErrorMessage = "Yêu cầu nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } // Số điện thoại liên hệ

        [Required(ErrorMessage = "Yêu cầu nhập thông tin liên hệ")]
        public string Description { get; set; } // Mô tả shop

        public string? Logo { get; set; } // Đường dẫn hoặc tên file logo lưu trong cơ sở dữ liệu

        [NotMapped] // Không ánh xạ vào cơ sở dữ liệu
        public IFormFile? ImageUpload { get; set; } // Thuộc tính để upload file ảnh
    }
}
