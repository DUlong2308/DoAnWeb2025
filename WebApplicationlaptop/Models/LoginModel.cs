using Microsoft.AspNetCore.Identity;

namespace WebApplicationlaptop.Models
{
    public class LoginModel : IdentityUser
    {
        public string Hovaten { get; set; }
    }
}