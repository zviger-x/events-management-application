namespace Application.Clients
{
    public interface IUserClient
    {
        /// <summary>
        /// Checks if a user exists by their unique identifier.
        /// </summary>
        /// <param name="userId">The GUID of the user to check.</param>
        /// <param name="cancellationToken">Cancellation token if needed.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
