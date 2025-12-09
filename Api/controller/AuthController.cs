using Application.Interfaces;
using Application.Request;
using Microsoft.AspNetCore.Mvc;

namespace Api.controller
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticateUser([FromBody] LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var result = await _authService.AuthenticateLogin(loginRequestDTO);
                return result.Status switch
                {
                    "invalid_credentials" => Unauthorized(result),
                    "error" => StatusCode(500, result),
                    _ => Ok(result)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Internal server error",
                    detail = ex.Message
                });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO refreshTokenRequestDTO)
        {
            try
            {
                var result = await _authService.RefreshToken(refreshTokenRequestDTO);
                return result.Status switch
                {
                    "invalid_token" => Unauthorized(result),
                    "expired_token" => Unauthorized(result),
                    "security_alert" => Unauthorized(result),
                    "not-found" => StatusCode(404, result),
                    "error" => StatusCode(500, result),
                    _ => Ok(result)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Internal server error",
                    detail = ex.Message
                });
            }
        }
    }
}
