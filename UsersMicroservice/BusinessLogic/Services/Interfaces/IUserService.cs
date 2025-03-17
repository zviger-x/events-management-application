using BusinessLogic.Contracts;
using DataAccess.Entities;

namespace BusinessLogic.Services.Interfaces
{
    public interface IUserService : IService<User>
    {
        /// <summary>
        /// Updates the user's profile information.
        /// </summary>
        /// <param name="userUpdate">The data to update the user's profile.</param>
        Task UpdateUserProfile(UpdateUserDTO userUpdate);
    }
}
