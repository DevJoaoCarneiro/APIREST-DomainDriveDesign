using Domain.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IMailRedirectService
    {
        Task SendResetAsync(string toEmail, string subject, string body);
    }
}
