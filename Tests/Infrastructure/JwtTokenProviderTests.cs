using Domain.entities;
using Infrastructure.Provider;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Tests.Infrastructure
{
    public class JwtTokenProviderTests
    {
        private readonly IConfiguration _configuration;
        private readonly JwtTokenProvider _jwtProvider;
        private readonly string _secretKey = "minha-chave-secreta-super-poderosa-que-tem-pelo-menos-32-bytes";

        public JwtTokenProviderTests()
        {
            _configuration = Substitute.For<IConfiguration>();

            _configuration["Jwt:Secret"].Returns(_secretKey);

            _jwtProvider = new JwtTokenProvider(_configuration);
        }

        [Fact]
        public void GenerateToken_Should_Create_Valid_Token_With_Correct_Claims()
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = "Teste User",
                Mail = "teste@email.com"
            };

            var tokenString = _jwtProvider.GenerateToken(user);

            Assert.False(string.IsNullOrEmpty(tokenString));

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            Assert.Equal(user.UserId.ToString(), token.Claims.First(c => c.Type == "nameid").Value);
            Assert.Equal(user.Name, token.Claims.First(c => c.Type == "unique_name").Value);
            Assert.Equal(user.Mail, token.Claims.First(c => c.Type == "email").Value);

            Assert.Equal("HS256", token.Header.Alg);

            var expectedExpiration = DateTime.UtcNow.AddHours(8);
            var expirationDiff = (token.ValidTo - expectedExpiration).TotalSeconds;

            Assert.True(Math.Abs(expirationDiff) < 5, $"Expiração incorreta. Esperado ~{expectedExpiration}, recebido {token.ValidTo}");
        }

        [Fact]
        public void GenerateRefreshToken_Should_Return_Valid_Object()
        {
            var userId = Guid.NewGuid();
            var ip = "127.0.0.1";

            var refreshToken = _jwtProvider.GenerateRefreshToken(userId, ip);

            Assert.NotNull(refreshToken);
            Assert.False(string.IsNullOrEmpty(refreshToken.Token));
            Assert.Equal(userId, refreshToken.UserId);
            Assert.Equal(ip, refreshToken.CreatedByIp);

            var expectedExpiration = DateTime.UtcNow.AddDays(7);
            var expirationDiff = (refreshToken.Expires - expectedExpiration).TotalSeconds;
            Assert.True(Math.Abs(expirationDiff) < 5, "Data de expiração do Refresh Token incorreta");

            var createdDiff = (refreshToken.Created - DateTime.UtcNow).TotalSeconds;
            Assert.True(Math.Abs(createdDiff) < 5, "Data de criação incorreta");
        }
    }
}
