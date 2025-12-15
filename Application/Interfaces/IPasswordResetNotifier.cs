using Domain.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IPasswordResetNotifier
    {
        Task SendResetLinkAsync(User user, string token);
    }
}
