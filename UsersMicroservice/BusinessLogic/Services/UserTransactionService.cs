using BusinessLogic.Models;
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

        // Пока что я буду просто выбрасывать ошибки. Но в будущем будет выбрасываться коды ошибки,
        // которые будут поступать в frontend
        public override async Task<Response> CreateAsync(UserTransaction entity, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            if (validationResult.IsValid)
                return Response.Fail(validationResult);

            entity.Id = default;
            await _unitOfWork.UserTransactionRepository.CreateAsync(entity, token);

            return Response.Success();
        }

        public override async Task<Response> UpdateAsync(UserTransaction entity, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            if (validationResult.IsValid)
                return Response.Fail(validationResult);

            await _unitOfWork.UserTransactionRepository.UpdateAsync(entity, token);

            return Response.Success();
        }

        public override async Task<Response> DeleteAsync(Guid id, CancellationToken token = default)
        {
            var transaction = new UserTransaction() { Id = id };
            await _unitOfWork.UserTransactionRepository.DeleteAsync(transaction, token);
            return Response.Success();
        }

        public override Response<IEnumerable<UserTransaction>> GetAll()
        {
            var collection = _unitOfWork.UserTransactionRepository.GetAll();
            return Response.Success(collection);
        }

        public override async Task<Response<UserTransaction>> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var user = await _unitOfWork.UserTransactionRepository.GetByIdAsync(id, token);
            return Response.Success(user);
        }
    }
}
