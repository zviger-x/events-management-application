using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    internal class UserService : BaseService<User>, IUserService
    {
        public UserService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public override async Task CreateAsync(User entity)
        {
            await _unitOfWork.UserRepository.CreateAsync(entity);
        }

        public override async Task DeleteAsync(User entity)
        {
            await _unitOfWork.UserRepository.DeleteAsync(entity);
        }

        public override async Task UpdateAsync(User entity)
        {
            await _unitOfWork.UserRepository.UpdateAsync(entity);
        }

        public override IQueryable<User> GetAll()
        {
            return _unitOfWork.UserRepository.GetAll();
        }

        public override async Task<User> GetByIdAsync(int id)
        {
            return await _unitOfWork.UserRepository.GetByIdAsync(id);
        }
    }
}
