using Microsoft.AspNetCore.Identity.UI.Services;
using System;

namespace ToDoApp.Services.RegisterEmailSender
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
