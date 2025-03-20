using BusinessLogic.Contracts;

namespace BusinessLogic.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userRegister">The user registration data.</param>
        /// <returns>Generated Jwt token and refresh token.</returns>
        Task<(string jwtToken, string refreshToken)> RegisterAsync(RegisterDTO userRegister, CancellationToken cancellationToken = default);

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <param name="userLogin">The user login data.</param>
        /// <returns>Generated Jwt token and refresh token.</returns>
        Task<(string jwtToken, string refreshToken)> LoginAsync(LoginDTO userLogin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks the validity of the refresh token and returns a new Jwt token.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="refreshToken">Refresh token to check.</param>
        /// <returns>Generated Jwt token.</returns>
        Task<string> RefreshTokenAsync(Guid id, string refreshToken, CancellationToken cancellationToken = default);
    }
}
