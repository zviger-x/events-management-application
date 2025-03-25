using AutoMapper;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    public class UserTransactionService : BaseService<UserTransaction>, IUserTransactionService
    {
        public UserTransactionService(IUnitOfWork unitOfWork, IMapper mapper, IUserTransactionValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<IEnumerable<UserTransaction>> GetByUserIdAsync(Guid id, CancellationToken token = default)
        {
            return await _unitOfWork.UserTransactionRepository.GetByUserIdAsync(id, token);
        }
    }
}
