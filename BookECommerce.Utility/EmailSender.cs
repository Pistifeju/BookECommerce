
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BookECommerce.Utility;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // TODO: Implement this method for sending email to the user
        return Task.CompletedTask;
    }
}