using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; } // Tên đầy đủ
    public string? Gender { get; set; } // Giới tính
    public string? Address { get; set; } // Địa chỉ
    public string? ProfilePictureUrl { get; set; } // Đường dẫn ảnh

    public string? GoogleId { get; set; }
}