using Application.Helpers;
using Application.Interfaces;
using Domain.entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class MailNotificationService : IPasswordResetNotifier
    {
        private readonly IConfiguration _configuration;
        private readonly IMailRedirectService _mailRedirectService;

        public MailNotificationService(IConfiguration configuration, IMailRedirectService mailRedirectService)
        {
            _configuration = configuration;
            _mailRedirectService = mailRedirectService;
        }

        public async Task SendResetLinkAsync(User user, string token)
        {
            var frontendUrl = _configuration["ClientSettings:Url"] ?? "http://localhost:5173";

            var link = $"{frontendUrl}/reset-password?token={token}";

            var emailBody = EmailTemplateBuilder.BuildPasswordResetEmail(user.Name, token, link);

            await _mailRedirectService.SendResetAsync(user.Mail, "Redefinição de Senha", emailBody);
        }
    }
}
