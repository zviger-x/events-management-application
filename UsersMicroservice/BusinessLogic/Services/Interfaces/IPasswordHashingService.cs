namespace BusinessLogic.Services.Interfaces
{
    public interface IPasswordHashingService
    {
        /// <summary>
        /// Hashes the provided password.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>A securely hashed password as a string, including the generated salt.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies whether the provided password matches the stored hashed password.
        /// The stored hashed password contains both the hash and the salt, which are used for comparison.
        /// </summary>
        /// <param name="password">The password entered by the user during authentication.</param>
        /// <param name="hashedPassword">The stored hashed password to compare.</param>
        /// <returns>True if the provided password matches the stored hashed password, otherwise false.</returns>
        bool VerifyPassword(string password, string hashedPassword);
    }
}
