using Application.Interfaces;
using Application.Request;
using Application.Services;
using Domain.entities;
using Domain.Entities.Embeded;
using Domain.Repository;
using NSubstitute;

namespace Tests.Services
{
    public class AuthServiceTest
    {
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
        private readonly AuthService _service;

        public AuthServiceTest()
        {
            _service = new AuthService(_userRepository, _tokenService);
        }

        [Fact]
        public async void Should_Authenticate_User_Correctly_When_Credentials_Right()
        {

            var expectedPassword = "tim1234";
            var validHash = BCrypt.Net.BCrypt.HashPassword(expectedPassword);

            var loginRequest = new LoginRequestDTO
            {
                Mail = "tim_bernardes@gmail.com",
                Password = expectedPassword
            };


            var addressList = new Address("Street A", "City X", "State Y", "12345", "432");


            var user = new User
            (
                "Tim Bernardes",
                "tim_bernardes@gmail.com",
                validHash,
                addressList
            );

            _userRepository.GetByEmailAsync(loginRequest.Mail)
                .Returns(user);

            _tokenService.GenerateToken(user)
                .Returns("valid_token");


            var result = await _service.AuthenticateLogin(loginRequest);


            Assert.NotNull(result);
            Assert.Equal("Login successful", result.Message);
            Assert.Equal("Success", result.Status);
            Assert.Equal("valid_token", result.Token);

            _tokenService.Received(1).GenerateToken(user);
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


            var addressList = new Address("Street A", "City X", "State Y", "12345", "432");

            var user = new User
           (
               "Tim Bernardes",
               "tim_bernardes@gmail.com",
               validHash,
               addressList
           );

            _userRepository.GetByEmailAsync(loginRequest.Mail)
                .Returns(user);

            var result = await _service.AuthenticateLogin(loginRequest);

            Assert.Equal("Invalid password", result.Message);
            Assert.Equal("invalid_credentials", result.Status);
            Assert.True(string.IsNullOrEmpty(result.Token));

            _tokenService.DidNotReceive().GenerateToken(Arg.Any<User>());

        }
    }
}