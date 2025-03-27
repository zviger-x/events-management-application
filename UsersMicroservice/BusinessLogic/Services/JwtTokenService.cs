using BusinessLogic.Configuration;
using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _tokenExpirationMinutes;

        private readonly int _refreshTokenExpirationMinutes;

        public JwtTokenService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;

            var config = configuration.GetSection("Jwt").Get<JwtTokenConfig>();

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _secretKey = config.SecretKey;
            _issuer = config.Issuer;
            _audience = config.Audience;
            _tokenExpirationMinutes = config.TokenExpirationMinutes;

            _refreshTokenExpirationMinutes = config.RefreshTokenExpirationMinutes;
        }

        public int TokenExpirationMinutes => _tokenExpirationMinutes;

        public int RefreshTokenExpirationMinutess => _refreshTokenExpirationMinutes;

        public string GenerateToken(Guid id, string name, string email, UserRoles role)
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

        public async Task<(bool IsValid, Guid UserId)> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var storedToken = await _unitOfWork.RefreshTokenRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
            if (storedToken == null || storedToken.Token != refreshToken || storedToken.Expires < DateTime.UtcNow)
                return new (false, Guid.Empty);

            return new (true, storedToken.UserId);
        }
    }
}
