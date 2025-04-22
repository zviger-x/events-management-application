using BusinessLogic.Configuration;
using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _tokenExpirationMinutes;

        private readonly int _refreshTokenExpirationMinutes;

        public TokenService(IOptions<JwtTokenConfig> config)
        {
            var cfg = config.Value;

            _secretKey = cfg.SecretKey;
            _issuer = cfg.Issuer;
            _audience = cfg.Audience;
            _tokenExpirationMinutes = cfg.TokenExpirationMinutes;

            _refreshTokenExpirationMinutes = cfg.RefreshTokenExpirationMinutes;
        }

        public int TokenExpirationMinutes => _tokenExpirationMinutes;

        public int RefreshTokenExpirationMinutess => _refreshTokenExpirationMinutes;

        public string GenerateJwtToken(Guid id, string name, string email, UserRoles role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_tokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(Guid userId)
        {
            using var rng = RandomNumberGenerator.Create();

            var randomBytes = new byte[64];
            rng.GetBytes(randomBytes);

            var token = Convert.ToBase64String(randomBytes);
            var refreshToken = new RefreshToken()
            {
                UserId = userId,
                Token = token,
                Expires = DateTime.Now.AddMinutes(_refreshTokenExpirationMinutes)
            };

            return refreshToken;
        }
    }
}
