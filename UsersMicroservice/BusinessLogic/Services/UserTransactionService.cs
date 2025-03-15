using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Services
{
    public class UserTransactionService : BaseService<UserTransaction>, IUserTransactionService
    {
        public UserTransactionService(IUnitOfWork unitOfWork, IUserTransactionValidator validator)
            : base(unitOfWork, validator)
        {
        }

        // Пока что я буду просто выбрасывать ошибки. Но в будущем будет выбрасываться коды ошибки,
        // которые будут поступать в frontend
        public override async Task CreateAsync(UserTransaction entity, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(entity, token);

            entity.Id = default;
            await _unitOfWork.UserTransactionRepository.CreateAsync(entity, token);
        }

        public override async Task UpdateAsync(UserTransaction entity, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(entity, token);

            await _unitOfWork.UserTransactionRepository.UpdateAsync(entity, token);
        }

        public override async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            var transaction = new UserTransaction() { Id = id };
            await _unitOfWork.UserTransactionRepository.DeleteAsync(transaction, token);
        }

        public override IEnumerable<UserTransaction> GetAll()
        {
            return _unitOfWork.UserTransactionRepository.GetAll();
        }

        public override async Task<UserTransaction> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _unitOfWork.UserTransactionRepository.GetByIdAsync(id, token);
        }
    }
}
