using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplicationlaptop.Helper;

namespace WebApplicationlaptop.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
        }

        public void OnGet()
        {
            // Phương thức GET chỉ trả về trang quên mật khẩu mà không làm gì
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra tính hợp lệ của model
            if (!ModelState.IsValid)
            {
                return Page(); // Trả lại trang nếu model không hợp lệ
            }

            // Tìm người dùng với email nhập vào
            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                // Nếu không tìm thấy người dùng với email nhập vào, chuyển đến trang xác nhận (ForgotPasswordConfirmation)
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            // Tạo token reset mật khẩu cho người dùng
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(token))
            {
                // Nếu không tạo được token reset mật khẩu, hiển thị thông báo lỗi
                ModelState.AddModelError(string.Empty, "Không thể tạo token reset mật khẩu.");
                return Page();
            }

            // Tạo liên kết reset mật khẩu với email và token
            var resetLink = Url.Page(
                "/Account/ResetPassword",  // Trang reset mật khẩu
                pageHandler: null,
                values: new { email = Input.Email, token },
                protocol: Request.Scheme);  // Sử dụng protocol từ request hiện tại

            // Gửi email reset mật khẩu
            var emailSent = await SendMail.SendEmail12Async(Input.Email, "Reset Password", $"Click vào liên kết này để reset mật khẩu: {resetLink}");
            if (!emailSent)
            {
                // Nếu không gửi được email, hiển thị thông báo lỗi
                ModelState.AddModelError(string.Empty, "Không thể gửi email. Vui lòng thử lại sau.");
                return Page();
            }

            // Nếu gửi thành công, chuyển đến trang xác nhận
            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}