using BusinessLogic.Contracts;

namespace BusinessLogic.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userRegister">The user registration data.</param>
        /// <returns>Generated Jwt token</returns>
        Task<string> RegisterAsync(RegisterDTO userRegister, CancellationToken cancellationToken);

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <param name="userLogin">The user login data.</param>
        /// <returns>Generated Jwt token</returns>
        Task<string> LoginAsync(LoginDTO userLogin, CancellationToken cancellationToken);
    }
}
