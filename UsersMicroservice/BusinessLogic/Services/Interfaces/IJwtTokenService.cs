using DataAccess.Entities;

namespace BusinessLogic.Services.Interfaces
{
    public interface IJwtTokenService
    {
        int TokenExpirationMinutes { get; }
        
        int RefreshTokenExpirationMinutess { get; }

        /// <summary>
        /// Generates a JWT token for the given user.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="name">The Name of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="role">The role of the user.</param>
        /// <returns>A JWT token as a string.</returns>
        string GenerateToken(Guid id, string name, string email, UserRoles role);

        /// <summary>
        /// Generates a random refresh token.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Generated refresh token</returns>
        RefreshToken GenerateRefreshToken(Guid userId);

        /// <summary>
        /// Checks the validity of the token
        /// </summary>
        /// <param name="refreshToken">Refresh token to validate</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>Validation result including the user Id of this token</returns>
        Task<(bool IsValid, Guid UserId)> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
