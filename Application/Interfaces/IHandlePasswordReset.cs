using Application.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IHandlePasswordReset
    {
        Task HandleAsync(ResetRequestEventDTO requestDto);
    }
}
