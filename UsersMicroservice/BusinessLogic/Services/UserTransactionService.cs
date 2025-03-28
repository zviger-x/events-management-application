using AutoMapper;
using BusinessLogic.Exceptions;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Services
{
    public class UserTransactionService : BaseService<UserTransaction>, IUserTransactionService
    {
        public UserTransactionService(IUnitOfWork unitOfWork, IMapper mapper, IUserTransactionValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public override async Task CreateAsync(UserTransaction transaction, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(transaction, token);

            if (!await IsUserExistsAsync(transaction.UserId, token))
                throw new ServiceValidationException(
                    UserTransactionValidationErrorCodes.UserIdIsInvalid,
                    UserTransactionValidationMessages.UserIdIsInvalid,
                    nameof(transaction.UserId));

            #warning TODO: Нужно добавить проверку на наличие ивента (gRPC запрос в микросервису ивентов)

            transaction.Id = default;
            await _unitOfWork.UserTransactionRepository.CreateAsync(transaction, token);
        }

        public async Task<IEnumerable<UserTransaction>> GetByUserIdAsync(Guid id, CancellationToken token = default)
        {
            return await _unitOfWork.UserTransactionRepository.GetByUserIdAsync(id, token);
        }

        private async Task<bool> IsUserExistsAsync(Guid guid, CancellationToken token = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(guid);

            return user != null;
        }
    }
}
