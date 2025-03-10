using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    internal class UserTransactionService : BaseService<UserTransaction>, IUserTransactionService
    {
        public UserTransactionService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public override async Task CreateAsync(UserTransaction entity)
        {
            await _unitOfWork.UserTransactionRepository.CreateAsync(entity);
        }

        public override async Task DeleteAsync(UserTransaction entity)
        {
            await _unitOfWork.UserTransactionRepository.DeleteAsync(entity);
        }

        public override async Task UpdateAsync(UserTransaction entity)
        {
            await _unitOfWork.UserTransactionRepository.UpdateAsync(entity);
        }

        public override IQueryable<UserTransaction> GetAll()
        {
            return _unitOfWork.UserTransactionRepository.GetAll();
        }

        public override async Task<UserTransaction> GetByIdAsync(int id)
        {
            return await _unitOfWork.UserTransactionRepository.GetByIdAsync(id);
        }
    }
}
