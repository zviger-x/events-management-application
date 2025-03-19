using DataAccess.Entities;

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
    }
}
