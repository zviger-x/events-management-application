using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO, CancellationToken cancellationToken)
        {
            var token = await _authService.RegisterAsync(registerDTO, cancellationToken);
            return Ok(new { AccessToken = token.jwtToken, RefreshToken = token.refreshToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO, CancellationToken cancellationToken)
        {
            var token = await _authService.LoginAsync(loginDTO, cancellationToken);
            return Ok(new { AccessToken = token.jwtToken, RefreshToken = token.refreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(string refreshToken, CancellationToken cancellationToken)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null)
                return Unauthorized("Authorization header is missing");

            var headerToken = authHeader.Substring("Bearer ".Length);
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(headerToken);

            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID not found in token");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid User ID format");

            var token = await _authService.RefreshTokenAsync(userId, refreshToken);
            if (token == null)
                return Unauthorized("Refresh token expired or invalid");

            return Ok(new { AccessToken = token });
        }
    }
}
