using Application.Interfaces;
using Application.Request;
using Application.Response;
using Domain.Interfaces;
using Domain.Repository;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IIpAddressService _ipAddressService;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository, IIpAddressService ipAddressService, IGoogleAuthService googleAuthService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _ipAddressService = ipAddressService;
            _googleAuthService = googleAuthService;
        }



        public async Task<LoginResponseDTO> AuthenticateLogin(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var currentIp = _ipAddressService.GetIpAddress();
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
                var refreshToken = _tokenService.GenerateRefreshToken(result.UserId, currentIp);

                await _refreshTokenRepository.AddAsync(refreshToken);
                await _refreshTokenRepository.SaveChangesAsync();

                return new LoginResponseDTO
                {
                    Message = "Login successful",
                    Status = "Success",
                    Token = token,
                    RefreshToken = refreshToken.Token
                };
            }
            catch (Exception)
            {
                return new LoginResponseDTO
                {
                    Message = "Internal Error",
                    Status = "error",
                    Token = "",
                    RefreshToken = ""
                };
            }

        }

        public async Task<AuthResponseDTO> LoginWithGoogleAsync(string idToken)
        {

            try
            {
                var currentIp = _ipAddressService.GetIpAddress();
                var googleUser = await _googleAuthService.ValidateGoogleTokenAsync(idToken);

                if (googleUser == null)
                {
                    return new AuthResponseDTO
                    {
                        Message = "Invalid Google token",
                        Status = "invalid_token",
                        UserData = null
                    };
                }

                var user = await _userRepository.GetByEmailAsync(googleUser.Mail);

                if (user == null)
                {
                    return new AuthResponseDTO
                    {
                        Message = "User not found",
                        Status = "not_found",
                        UserData = new GoogleUserData
                        {
                            Payload = new UserPayload
                            {
                                GoogleId = googleUser.GoogleId,
                                Email = googleUser.Mail,
                                Name = googleUser.Name
                            }
                        }
                    };
                }

                var token = _tokenService.GenerateToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken(user.UserId, currentIp);

                await _refreshTokenRepository.AddAsync(refreshToken);
                await _refreshTokenRepository.SaveChangesAsync();

                return new AuthResponseDTO
                {
                    Message = "Login successful",
                    Status = "Success",
                    UserData = new GoogleUserData
                    {
                        AccessToken = token,
                        RefreshToken = refreshToken.Token,
                        ExpiresIn = refreshToken.Expires,
                        Payload = new UserPayload
                        {
                            GoogleId = user.UserId.ToString(),
                            Email = user.Mail,
                            Name = user.Name
                        }
                    }
                };
            }
            catch (Exception)
            {
                return new AuthResponseDTO
                {
                    Message = "Internal Error",
                    Status = "error",
                    UserData = null
                };
            }
        }

        public async Task<RefreshTokenResponseDTO> RefreshToken(RefreshTokenRequestDTO requestDTO)
        {
            try
            {
                var currentIp = _ipAddressService.GetIpAddress();
                var storedToken = await _refreshTokenRepository.GetByTokenAsync(requestDTO.RefreshToken);

                if (storedToken == null)
                {
                    return new RefreshTokenResponseDTO
                    {
                        Message = "Invalid refresh token",
                        Status = "invalid_token"
                    };
                }

                if (storedToken.Expires < DateTime.UtcNow)
                {
                    return new RefreshTokenResponseDTO
                    {
                        Message = "Refresh token has expired",
                        Status = "expired_token"
                    };
                }

                if (storedToken.Revoked != null)
                {
                    var allTokens = await _refreshTokenRepository.GetAllActiveByUserIdAsync(storedToken.UserId);
                    foreach (var token in allTokens)
                    {
                        token.Revoked = DateTime.UtcNow;
                        token.ReasonRevoked = "Compromised token";
                        token.RevokedByIp = currentIp;

                        await _refreshTokenRepository.UpdateAsync(token);
                    }

                    await _refreshTokenRepository.SaveChangesAsync();

                    return new RefreshTokenResponseDTO
                    {
                        Message = "Invalid token usage detected",
                        Status = "security_alert"
                    };
                }

                if (storedToken.User == null)
                {
                    return new RefreshTokenResponseDTO
                    {
                        Message = "Critical Error",
                        Status = "not-found"
                    };
                }

                var newAccessToken = _tokenService.GenerateToken(storedToken.User);
                var newRefreshToken = _tokenService.GenerateRefreshToken(storedToken.UserId, currentIp);

                storedToken.Revoked = DateTime.UtcNow;
                storedToken.ReplacedByToken = newRefreshToken.Token;
                storedToken.ReasonRevoked = "Replaced by new token";
                storedToken.RevokedByIp = currentIp;

                await _refreshTokenRepository.UpdateAsync(storedToken);
                await _refreshTokenRepository.AddAsync(newRefreshToken);
                await _refreshTokenRepository.SaveChangesAsync();

                return new RefreshTokenResponseDTO
                {
                    Message = "Token refreshed successfully",
                    Status = "Success",
                    Data = new DataToken
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken.Token
                    }
                };
            }
            catch (Exception ex)
            {
                return new RefreshTokenResponseDTO
                {
                    Message = "Internal Error: " + ex.Message,
                    Status = "error"
                };
            }

        }
        public async Task<ResetPasswordResponseDTO> RequestPasswordResetAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return new ResetPasswordResponseDTO
                    {
                        Message = "Email is required",
                        Status = "invalid_request",
                        Mail = ""
                    };
                }

                var user = await _userRepository.GetByEmailAsync(email);

                if (user == null)
                {
                    return new ResetPasswordResponseDTO
                    {
                        Status = "success",
                        Message = "Sending a confirmation mail for you",
                        Mail = ""
                    };
                }

                var token = Guid.NewGuid().ToString();

                user.PasswordResetToken = token;
                user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

                await _userRepository.UpdateAsync(user);


                return new ResetPasswordResponseDTO
                {
                    Status = "success",
                    Message = "Se o e-mail estiver cadastrado, enviaremos um link de recuperação.",
                    Mail = ""
                };
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
