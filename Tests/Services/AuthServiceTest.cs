using Application.Interfaces;
using Application.Request;
using Application.Services;
using Domain.entities;
using Domain.Entities;
using Domain.Entities.Embeded;
using Domain.Interfaces;
using Domain.Repository;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Tests.Services
{
    public class AuthServiceTest
    {
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
        private readonly IIpAddressService _ipAddressService = Substitute.For<IIpAddressService>();
        private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        private readonly AuthService _service;

        public AuthServiceTest()
        {
            _service = new AuthService(_userRepository, _tokenService, _refreshTokenRepository, _ipAddressService);
        }

        [Fact]
        public async void Should_Authenticate_User_Correctly_When_Credentials_Right()
        {

            var expectedPassword = "tim1234";
            var validHash = BCrypt.Net.BCrypt.HashPassword(expectedPassword);
            var expectedIp = "127.0.0.1";
            var userId = Guid.NewGuid();

            var loginRequest = new LoginRequestDTO
            {
                Mail = "tim_bernardes@gmail.com",
                Password = expectedPassword
            };


            var addressList = new Address
            {
                Street = "Street A",
                City = "City X",
                State = "State Y",
                Number = "12345",
                ZipCode = "432"
            };


            var user = new User
            {
                Name = "Tim Bernardes",
                Mail = "tim_bernardes@gmail.com",
                PasswordHash = validHash,
                UserAddress = addressList
            };

            var createdRefreshToken = new RefreshToken
            {
                Token = "valid_token",
                UserId = userId,
                CreatedByIp = expectedIp,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };

            _ipAddressService.GetIpAddress()
                .Returns(expectedIp);

            _userRepository.GetByEmailAsync(loginRequest.Mail)
                .Returns(user);

            _tokenService.GenerateToken(user)
                .Returns("valid_token");

            _tokenService.GenerateRefreshToken(user.UserId, expectedIp)
                .Returns(createdRefreshToken);

            var result = await _service.AuthenticateLogin(loginRequest);


            Assert.NotNull(result);
            Assert.Equal("Login successful", result.Message);
            Assert.Equal("Success", result.Status);

            _tokenService.Received(1).GenerateToken(user);
            await _refreshTokenRepository.Received(1).AddAsync(createdRefreshToken);
            await _refreshTokenRepository.Received(1).SaveChangesAsync();
        }


        [Fact]
        public async void Should_Return_Invalid_Credentials_When_Password_Wrong()
        {

            var expectedPassword = "tim1234";
            var validHash = BCrypt.Net.BCrypt.HashPassword(expectedPassword);
            var loginRequest = new LoginRequestDTO
            {
                Mail = "tim_bernardes@gmail.com",
                Password = expectedPassword
            };


            _userRepository.GetByEmailAsync(loginRequest.Mail)
                .Returns((User)null);

            var result = await _service.AuthenticateLogin(loginRequest);


            Assert.Equal("User not found", result.Message);
            Assert.Equal("invalid_credentials", result.Status);
            Assert.Equal("", result.Token);


            _tokenService.DidNotReceive().GenerateToken(Arg.Any<User>());
        }

        [Fact]
        public async void Should_Return_Invalid_Password_When_Is_Wrong()
        {
            var expectedPassword = "tim1234";
            var wrongPassword = "wrong_password";
            var validHash = BCrypt.Net.BCrypt.HashPassword(expectedPassword);
            var loginRequest = new LoginRequestDTO
            {
                Mail = "tim_bernardes@gmail.com",
                Password = wrongPassword
            };


            var addressList = new Address
            {
                Street = "Street A",
                City = "City X",
                State = "State Y",
                Number = "12345",
                ZipCode = "432"
            };


            var user = new User
            {
                Name = "Tim Bernardes",
                Mail = "tim_bernardes@gmail.com",
                PasswordHash = validHash,
                UserAddress = addressList
            };

            _userRepository.GetByEmailAsync(loginRequest.Mail)
                .Returns(user);

            var result = await _service.AuthenticateLogin(loginRequest);

            Assert.Equal("Invalid password", result.Message);
            Assert.Equal("invalid_credentials", result.Status);
            Assert.True(string.IsNullOrEmpty(result.Token));

            _tokenService.DidNotReceive().GenerateToken(Arg.Any<User>());

        }

        [Fact]
        public async void Should_Return_Refresh_Token_When_No_Error()
        {
            var expectedIp = "127.0.0.1";
            var userId = Guid.NewGuid();
            var expectedPassword = "tim1234";
            var validHash = BCrypt.Net.BCrypt.HashPassword(expectedPassword);

            var user = new User
            {
                Name = "Tim Bernardes",
                Mail = "tim_bernardes@gmail.com",
                PasswordHash = validHash
            };

            var storedToken = new RefreshToken
            {
                Token = "token_valido",
                UserId = userId,
                User = user,
                Expires = DateTime.UtcNow.AddDays(1),
                Revoked = null
            };

            var requestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = "valid_token"
            };

            var newAccessToken = "novo_jwt_token";

            _ipAddressService.GetIpAddress().Returns(expectedIp);
            _refreshTokenRepository.GetByTokenAsync(requestDTO.RefreshToken).Returns(storedToken);

            _tokenService.GenerateToken(user).Returns(newAccessToken);
            _tokenService.GenerateRefreshToken(userId, expectedIp).Returns(storedToken);

            var result = await _service.RefreshToken(requestDTO);

            Assert.Equal("Token refreshed successfully", result.Message);
            Assert.Equal("Success", result.Status);
            Assert.NotNull(result.Data);
            Assert.Equal(newAccessToken, result.Data.AccessToken);

            await _refreshTokenRepository.Received(1).AddAsync(storedToken);
            await _refreshTokenRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async void Should_Return_Invalid_Token_When_Is_Null()
        {
            var invalidToken = "Invalid_Token";

            var requestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = invalidToken
            };

            _refreshTokenRepository.GetByTokenAsync(requestDTO.RefreshToken).Returns((RefreshToken)null);

            var result = await _service.RefreshToken(requestDTO);

            Assert.NotNull(result);

            Assert.Equal("Invalid refresh token", result.Message);
            Assert.Equal("invalid_token", result.Status);
        }

        [Fact]
        public async void Should_Return_Token_Expired_When_It_Is_Expired()
        {
            var validToken = "valid-token";

            var requestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = validToken
            };

            var expiredToken = new RefreshToken
            {
                Token = validToken,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            _refreshTokenRepository.GetByTokenAsync(requestDTO.RefreshToken).Returns(expiredToken);

            var result = await _service.RefreshToken(requestDTO);

            Assert.Equal("Refresh token has expired", result.Message);
            Assert.Equal("expired_token", result.Status);
        }

        [Fact]
        public async void Should_Return_Token_Revoked_When_It_Is_Revoked()
        {
            var validToken = "valid-token";

            var requestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = validToken
            };

            var refreshToken = new RefreshToken
            {
                UserId = Guid.NewGuid(),
                Token = validToken,
                Expires = DateTime.UtcNow.AddDays(1),
                Revoked = DateTime.UtcNow.AddDays(-1)
            };

            _refreshTokenRepository.GetAllActiveByUserIdAsync(refreshToken.UserId)
                .Returns(new List<RefreshToken>());
            _refreshTokenRepository.SaveChangesAsync()
                .Returns(Task.CompletedTask);
            _refreshTokenRepository.GetByTokenAsync(requestDTO.RefreshToken).Returns(refreshToken);

            var result = await _service.RefreshToken(requestDTO);

            Assert.Equal("Invalid token usage detected", result.Message);
            Assert.Equal("security_alert", result.Status);

            await _refreshTokenRepository.Received(1).GetAllActiveByUserIdAsync(refreshToken.UserId);
            await _refreshTokenRepository.Received(1).SaveChangesAsync();
            await _refreshTokenRepository.Received(1).GetByTokenAsync(requestDTO.RefreshToken);
        }

        [Fact]
        public async void Should_Return_Critical_Error_When_User_Null()
        {
            var validToken = "valid-token";

            var requestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = validToken
            };

            var refreshToken = new RefreshToken
            {
                UserId = Guid.NewGuid(),
                Token = validToken,
                Expires = DateTime.UtcNow.AddDays(1),
                Revoked = null,
                User = null
            };

            _refreshTokenRepository.GetByTokenAsync(requestDTO.RefreshToken).Returns(refreshToken);

            var result = await _service.RefreshToken(requestDTO);

            Assert.Equal("Critical Error", result.Message);
            Assert.Equal("not-found", result.Status);

            await _refreshTokenRepository.DidNotReceive().GetAllActiveByUserIdAsync(Arg.Any<Guid>());
            await _refreshTokenRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async void Should_Handle_Exception_During_Authentication_When_Error()
        {
            var validToken = "valid-token";

            var requestDTO = new RefreshTokenRequestDTO
            {
                RefreshToken = validToken
            };

            var refreshToken = new RefreshToken
            {
                UserId = Guid.NewGuid(),
                Token = validToken,
                Expires = DateTime.UtcNow.AddDays(1),
                Revoked = null,
                User = null
            };

            _refreshTokenRepository.GetByTokenAsync(requestDTO.RefreshToken).ThrowsAsync(new Exception("Error simulated"));

            var result = await _service.RefreshToken(requestDTO);

            Assert.Equal("Internal Error: Error simulated", result.Message);
            Assert.Equal("error", result.Status);
        }


    }
}