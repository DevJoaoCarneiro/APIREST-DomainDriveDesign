using Application.Interfaces;
using Application.Request;
using Application.Response;
using Domain.Repository;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDTO> AuthenticateLogin(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var result = await _userRepository.GetByEmailAsync(loginRequestDTO.Mail);

                if (result == null)
                {
                    return new LoginResponseDTO
                    {
                        Message = "User not found",
                        Status = "invalid_credentials"
                    };
                }

                if (!BCrypt.Net.BCrypt.Verify(loginRequestDTO.Password, result.PasswordHash))
                {
                    return new LoginResponseDTO
                    {
                        Message = "Invalid password",
                        Status = "invalid_credentials"
                    };
                }

                var token = _tokenService.GenerateToken(result);

                return new LoginResponseDTO
                {
                    Message = "Login successful",
                    Status = "Success",
                    Token = token
                };
            }
            catch (Exception)
            {
                return new LoginResponseDTO
                {
                    Message = "Internal Error",
                    Status = "error",
                    Token = ""
                };
            }
            
        }
    }
}
