using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
        {
            var token = await _authService.RegisterAsync(registerDto, cancellationToken);

            return Ok(new { AccessToken = token.jwtToken, RefreshToken = token.refreshToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            var token = await _authService.LoginAsync(loginDto, cancellationToken);

            return Ok(new { AccessToken = token.jwtToken, RefreshToken = token.refreshToken });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var currentUserId = User.GetUserIdOrThrow();

            await _authService.LogoutAsync(currentUserId, cancellationToken);

            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, CancellationToken cancellationToken)
        {
            var token = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);

            return Ok(new { AccessToken = token.jwtToken, RefreshToken = token.refreshToken });
        }
    }
}
