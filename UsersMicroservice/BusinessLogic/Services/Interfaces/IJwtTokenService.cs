using DataAccess.Entities;
using System.Security.Claims;

namespace BusinessLogic.Services.Interfaces
{
    public interface IJwtTokenService
    {
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
        /// <param name="userId">User id</param>
        /// <param name="refreshToken">Refresh token to validate</param>
        /// <returns>True if the token is valid, otherwise false.</returns>
        Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken);

        /// <summary>
        /// Retrieves the ClaimsPrincipal from an expired JWT token.
        /// This method is used to extract the user claims from a token that has expired.
        /// Note: The token's expiration time is not validated in this method.
        /// </summary>
        /// <param name="token">The expired JWT token.</param>
        /// <returns>Returns the ClaimsPrincipal extracted from the expired token if valid; otherwise, returns null.</returns>
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
