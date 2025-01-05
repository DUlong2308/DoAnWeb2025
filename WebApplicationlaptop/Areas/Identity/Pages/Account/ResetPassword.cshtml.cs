using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplicationlaptop.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        // Inject UserManager vào constructor
        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Token { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public IActionResult OnGet(string email, string token)
        {
            // Kiểm tra token hợp lệ thông qua UserManager
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Email hoặc token không hợp lệ.");
            }

            Email = email;
            Token = token;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra mật khẩu và xác nhận mật khẩu có trùng khớp không
            if (Password != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu không khớp!";
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
                return Page();
            }

            // Đặt lại mật khẩu
            var resetResult = await _userManager.ResetPasswordAsync(user, Token, Password);

            if (resetResult.Succeeded)
            {
                TempData["SuccessMessage"] = "Mật khẩu của bạn đã được thay đổi thành công!";
                return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach (var error in resetResult.Errors)
                {
                    TempData["ErrorMessage"] = error.Description;
                }
                return Page();
            }
        }
    }
}