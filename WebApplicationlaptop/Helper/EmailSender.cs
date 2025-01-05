using Microsoft.AspNetCore.Identity.UI.Services;
using WebApplicationlaptop.Helper;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Use your SendMail class to send the email
        var result = SendMail.SendEmail(email, subject, htmlMessage);
        return Task.FromResult(result);
    }
}