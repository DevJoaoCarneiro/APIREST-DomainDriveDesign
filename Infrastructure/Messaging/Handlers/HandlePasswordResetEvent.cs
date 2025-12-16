using Application.Interfaces;
using Application.Request;
using Domain.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Messaging.Handlers
{
    public class HandlePasswordResetEvent : IHandlePasswordReset
    {
        IPasswordResetNotifier _notifier;

        public HandlePasswordResetEvent(IPasswordResetNotifier notifier)
        {
            _notifier = notifier;
        }

        public async Task HandleAsync(ResetRequestEventDTO requestDto)
        {
            var user = new User
            {
                Name = requestDto.UserName,
                Mail = requestDto.UserEmail
            };

            await _notifier.SendResetLinkAsync(user, requestDto.Token);
        }
    }
}
