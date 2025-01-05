// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplicationlaptop.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        public string StatusMessage { get; set; }

        public void OnGet()
        {
            StatusMessage = "Vui lòng kiểm tra email của bạn để tiếp tục đặt lại mật khẩu.";
        }
    }
}