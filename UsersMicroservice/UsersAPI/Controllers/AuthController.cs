using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO, CancellationToken cancellationToken)
        {
            var token = await _authService.RegisterAsync(registerDTO, cancellationToken);

            return Ok(new { AccessToken = token.jwtToken, RefreshToken = token.refreshToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO, CancellationToken cancellationToken)
        {
            var token = await _authService.LoginAsync(loginDTO, cancellationToken);

            return Ok(new { AccessToken = token.jwtToken, RefreshToken = token.refreshToken });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            await _authService.LogoutAsync(cancellationToken);

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
