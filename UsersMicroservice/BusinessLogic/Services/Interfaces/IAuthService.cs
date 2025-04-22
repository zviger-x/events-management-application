using BusinessLogic.Contracts;

namespace BusinessLogic.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userRegister">The user registration data.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>Generated Jwt token and refresh token.</returns>
        Task<(string jwtToken, string refreshToken)> RegisterAsync(RegisterDTO userRegister, CancellationToken cancellationToken = default);

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <param name="userLogin">The user login data.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>Generated Jwt token and refresh token.</returns>
        Task<(string jwtToken, string refreshToken)> LoginAsync(LoginDTO userLogin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs out the user by his id.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task LogoutAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks the validity of the refresh token and returns a new Jwt token.
        /// </summary>
        /// <param name="refreshToken">Refresh token to check.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>Generated Jwt token.</returns>
        Task<(string jwtToken, string refreshToken)> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks the validity of the token
        /// </summary>
        /// <param name="refreshToken">Refresh token to validate</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>Validation result including the user Id of this token</returns>
        Task<(bool IsValid, Guid UserId)> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
