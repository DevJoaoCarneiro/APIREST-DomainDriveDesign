using Application.Request;
using Application.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> AuthenticateLogin(LoginRequestDTO loginRequestDTO);

        Task<RefreshTokenResponseDTO> RefreshToken(RefreshTokenRequestDTO refreshTokenRequestDTO);

        Task<AuthResponseDTO> LoginWithGoogleAsync(string idToken);
    }
}
