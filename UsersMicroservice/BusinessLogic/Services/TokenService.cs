using BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtTokenConfig _jwtConfig;

        public TokenService(IOptions<JwtTokenConfig> config)
        {
            _jwtConfig = config.Value;
        }

        public int TokenExpirationMinutes => _jwtConfig.TokenExpirationMinutes;

        public int RefreshTokenExpirationMinutes => _jwtConfig.RefreshTokenExpirationMinutes;

        public string GenerateJwtToken(Guid id, string name, string email, UserRoles role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString()),

                // For Ocelot
                new Claim("scope", role.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfig.TokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string Token, DateTime Expires) GenerateRefreshToken()
        {
            using var rng = RandomNumberGenerator.Create();

            var randomBytes = new byte[64];
            rng.GetBytes(randomBytes);

            var token = Convert.ToBase64String(randomBytes);
            var expires = DateTime.UtcNow.AddMinutes(_jwtConfig.RefreshTokenExpirationMinutes);

            return new(token, expires);
        }
    }
}
