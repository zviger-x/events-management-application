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

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Substring("Bearer ".Length);
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is missing.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await _authService.LogoutAsync(userId, token, cancellationToken);
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(string refreshToken, CancellationToken cancellationToken)
        {
            var token = await _authService.RefreshTokenAsync(refreshToken);
            if (token == null)
                return Unauthorized("Refresh token expired or invalid");

            return Ok(new { AccessToken = token });
        }
    }
}
