using Application.Response;
using Domain.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ExternalServices
{
    public class GoogleAuthService : IGoogleAuthService
    {

        private readonly string _clienteId;

        public GoogleAuthService(IConfiguration _configuration)
        {
            _clienteId = _configuration["Authentication:Google:ClientId"];
        }

        public async Task<GoogleUserResult> ValidateGoogleTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _clienteId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return new GoogleUserResult
                {
                    Mail = payload.Email,
                    Name = payload.Name,
                    GoogleId = payload.Subject
                };
            }
            catch (InvalidJwtException)
            {
                return null;
            }
        }
    }
}
