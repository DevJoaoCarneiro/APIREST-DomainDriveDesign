using Api.controller;
using Application.Interfaces;
using Application.Request;
using Application.Response;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Tests.Controller
{
    public class AuthControllerTests
    {
        private readonly IAuthService _authService = Substitute.For<IAuthService>();
        private readonly AuthController _controller;
        public AuthControllerTests()
        {
            _controller = new AuthController(_authService);
        }

        [Fact]
        public async void Should_Authenticate_User_Correctly_Via_Controller()
        {
            var loginRequest = new LoginRequestDTO
            {
                Mail = "tim_bernardes@gmail.com",
                Password = "tim1234"
            };

            var result = new LoginResponseDTO
            {
                Message = "User authenticated successfully",
                Status = "Success",
                Token = "q2313"
            };

            _authService.AuthenticateLogin(Arg.Any<LoginRequestDTO>()).Returns(result);

            var response = await _controller.AuthenticateUser(loginRequest);
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnDto = Assert.IsType<LoginResponseDTO>(okResult.Value);


            Assert.Equal("Success", returnDto.Status);
            Assert.Equal("User authenticated successfully", returnDto.Message);
            Assert.Equal("q2313", returnDto.Token);

        }

        [Fact]
        public async void Should_Return_Bad_Request_When_Error()
        {
            var loginRequest = new LoginRequestDTO
            {
                Mail = "tim_bernardes@gmail.com",
                Password = "tim1234"
            };

            _authService
                .When(x => x.AuthenticateLogin(Arg.Any<LoginRequestDTO>()))
                .Do(x => throw new Exception("Db Error"));

            var response = await _controller.AuthenticateUser(loginRequest);

            var objectResult = Assert.IsType<ObjectResult>(response);

            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async void Should_Return_Invalid_Credentials_When_Password_Or_Mail_Is_Wrong()
        {
            var serviceResponse = new LoginResponseDTO
            {
                Message = "User not found",
                Status = "invalid_credentials"
            };

            _authService.AuthenticateLogin(Arg.Any<LoginRequestDTO>())
                .Returns(serviceResponse);

            var response = await _controller.AuthenticateUser(new LoginRequestDTO());

            var UnauthorizedRequestResult = Assert.IsType<UnauthorizedObjectResult>(response);
            var returnDto = Assert.IsType<LoginResponseDTO>(UnauthorizedRequestResult.Value);

            Assert.Equal(401, UnauthorizedRequestResult.StatusCode);
            Assert.Equal("invalid_credentials", returnDto.Status);
            Assert.Equal("User not found", returnDto.Message);
        }

        [Fact]
        public async void Should_Return_500_When_Error()
        {
            var serviceResponse = new LoginResponseDTO
            {
                Message = "Internal Error",
                Status = "error"
            };

            _authService.AuthenticateLogin(Arg.Any<LoginRequestDTO>())
                .Returns(serviceResponse);

            var response = await _controller.AuthenticateUser(new LoginRequestDTO());
            var objectResult = Assert.IsType<ObjectResult>(response);
            var returnDto = Assert.IsType<LoginResponseDTO>(objectResult.Value);

            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("error", returnDto.Status);
            Assert.Equal("Internal Error", returnDto.Message);
        }

        [Fact]
        public async Task RefreshToken_Should_Return_Ok_When_Service_Returns_Success()
        {
            var requestDTO = new RefreshTokenRequestDTO { RefreshToken = "valid-token" };
            var serviceResponse = new RefreshTokenResponseDTO
            {
                Status = "Success",
                Data = new DataToken { AccessToken = "new-access", RefreshToken = "new-refresh" }
            };

            _authService.RefreshToken(requestDTO).Returns(serviceResponse);

            var result = await _controller.RefreshToken(requestDTO);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(serviceResponse, okResult.Value);
        }

        [Theory]
        [InlineData("invalid_token")]
        [InlineData("expired_token")]
        [InlineData("security_alert")]
        public async Task RefreshToken_Should_Return_Unauthorized_When_Token_Is_Invalid_Or_Expired(string statusError)
        {
            var requestDTO = new RefreshTokenRequestDTO { RefreshToken = "bad-token" };
            var serviceResponse = new RefreshTokenResponseDTO
            {
                Status = statusError,
                Message = "Auth failed"
            };

            _authService.RefreshToken(requestDTO).Returns(serviceResponse);

            var result = await _controller.RefreshToken(requestDTO);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Equal(serviceResponse, unauthorizedResult.Value);
        }

        [Fact]
        public async Task RefreshToken_Should_Return_InternalServerError_When_Exception_Is_Thrown()
        {
            var requestDTO = new RefreshTokenRequestDTO { RefreshToken = "token" };


            _authService.RefreshToken(requestDTO).Throws(new Exception("Catastrophic Failure"));

            var result = await _controller.RefreshToken(requestDTO);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var value = objectResult.Value;
        }
    }
}
