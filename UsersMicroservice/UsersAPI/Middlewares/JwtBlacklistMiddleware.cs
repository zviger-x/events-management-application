using BusinessLogic.Caching.Constants;
using BusinessLogic.Caching.Interfaces;
using BusinessLogic.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UsersAPI.Middlewares
{
    public class JwtBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<JwtTokenConfig> _jwtTokenConfig;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JwtBlacklistMiddleware> _logger;

        public JwtBlacklistMiddleware(RequestDelegate next,
            IOptions<JwtTokenConfig> jwtTokenConfig,
            IServiceProvider serviceProvider,
            ILogger<JwtBlacklistMiddleware> logger)
        {
            _next = next;
            _jwtTokenConfig = jwtTokenConfig;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null || !endpoint.Metadata.OfType<AuthorizeAttribute>().Any())
            {
                await _next(context);
                return;
            }

            foreach (var att in endpoint.Metadata.OfType<AuthorizeAttribute>())
                _logger.LogCritical(att.GetType().ToString());

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            var token = authHeader?.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                await _next(context);
                return;
            }

            var jwtConfig = _jwtTokenConfig.Value; 
            var tokenHandler = new JwtSecurityTokenHandler();
            var options = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };

            var result = await tokenHandler.ValidateTokenAsync(token, options);
            if (!result.IsValid)
            {
                await _next(context);
                return;
            }

            var claims = tokenHandler.ReadJwtToken(token).Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim?.Value, out var userId))
            {
                await _next(context);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            var blacklistedToken = await cacheService.GetAsync<string>(CacheKeys.UserJwtToken(userId));
            if (string.IsNullOrEmpty(blacklistedToken))
            {
                await _next(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token is revoked");
            return;
        }
    }
}
