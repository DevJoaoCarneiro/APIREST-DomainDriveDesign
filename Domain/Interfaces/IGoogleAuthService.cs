using Application.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserResult> ValidateGoogleTokenAsync(string idToken);
    }
}
