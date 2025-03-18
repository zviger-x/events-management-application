using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    public class UserTransactionService : BaseService<UserTransaction>, IUserTransactionService
    {
        public UserTransactionService(IUnitOfWork unitOfWork, IUserTransactionValidator validator)
            : base(unitOfWork, validator)
        {
        }
    }
}
