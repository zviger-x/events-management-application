using Bogus;
using BusinessLogic.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Tests.UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly Faker _faker;
        private readonly JwtTokenConfig _config;
        private readonly TokenService _service;

        public TokenServiceTests()
        {
            _faker = new Faker();

            _config = new JwtTokenConfig
            {
                SecretKey = "secretkey256supersecretkey256key",
                Issuer = _faker.Random.Word(),
                Audience = _faker.Random.Word(),
                TokenExpirationMinutes = 60,
                RefreshTokenExpirationMinutes = 4320
            };

            var options = Options.Create(_config);
            _service = new TokenService(options);
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnValidJwt()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var name = _faker.Name.FirstName();
            var email = _faker.Internet.Email();
            var role = UserRoles.User;

            // Act
            var token = _service.GenerateJwtToken(userId, name, email, role);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config.Issuer,
                ValidAudience = _config.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey)),
                ClockSkew = TimeSpan.Zero
            }, out _);

            principal.Identity.Should().NotBeNull();
            principal.Identity.IsAuthenticated.Should().BeTrue();

            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value.Should().Be(userId.ToString());
            principal.FindFirst(ClaimTypes.Name)?.Value.Should().Be(name);
            principal.FindFirst(ClaimTypes.Email)?.Value.Should().Be(email);
            principal.FindFirst(ClaimTypes.Role)?.Value.Should().Be(role.ToString());
            principal.FindFirst("scope")?.Value.Should().Be(role.ToString());
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnValidBase64StringAndExpiration()
        {
            // Act
            var (token, expires) = _service.GenerateRefreshToken();

            // Assert
            token.Should().NotBeNullOrWhiteSpace();

            var act = () => Convert.FromBase64String(token);
            act.Should().NotThrow();

            expires.Should()
                .BeAfter(DateTime.UtcNow).And
                .BeBefore(DateTime.UtcNow.AddMinutes(_config.RefreshTokenExpirationMinutes + 1));
        }

        [Fact]
        public void TokenExpirationMinutes_ShouldReturnConfiguredValue()
        {
            // Act & Assert
            _service.TokenExpirationMinutes.Should().Be(_config.TokenExpirationMinutes);
        }

        [Fact]
        public void RefreshTokenExpirationMinutes_ShouldReturnConfiguredValue()
        {
            // Act & Assert
            _service.RefreshTokenExpirationMinutes.Should().Be(_config.RefreshTokenExpirationMinutes);
        }

        [Fact]
        public void GenerateJwtToken_ShouldThrowException_WhenSecretKeyIsTooShort()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var name = _faker.Name.FirstName();
            var email = _faker.Internet.Email();
            var role = UserRoles.Admin;

            var shortKeyConfig = new JwtTokenConfig
            {
                SecretKey = "short",
                Issuer = _faker.Random.Word(),
                Audience = _faker.Random.Word(),
                TokenExpirationMinutes = _config.TokenExpirationMinutes,
                RefreshTokenExpirationMinutes = _config.RefreshTokenExpirationMinutes
            };

            var service = new TokenService(Options.Create(shortKeyConfig));

            // Act
            var act = () => service.GenerateJwtToken(userId, name, email, role);

            // Assert
            act.Should().Throw<ArgumentException>();
        }
    }
}
