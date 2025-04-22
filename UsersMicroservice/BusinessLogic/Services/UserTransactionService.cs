using AutoMapper;
using BusinessLogic.Exceptions;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;
using ValidationException = BusinessLogic.Exceptions.ValidationException;

namespace BusinessLogic.Services
{
    public class UserTransactionService : BaseService, IUserTransactionService
    {
        private readonly IValidator<UserTransaction> _validator;
        private readonly ICurrentUserService _currentUserService;

        public UserTransactionService(IUnitOfWork unitOfWork, IMapper mapper, IUserTransactionValidator validator, ICurrentUserService currentUserService)
            : base(unitOfWork, mapper)
        {
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> CreateAsync(UserTransaction transaction, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(transaction, token);

            if (!await IsUserExistsAsync(transaction.UserId, token))
                throw new ValidationException(
                    UserTransactionValidationErrorCodes.UserIdIsInvalid,
                    UserTransactionValidationMessages.UserIdIsInvalid,
                    nameof(transaction.UserId));

            // TODO: Нужно добавить проверку на наличие ивента (gRPC запрос в микросервису ивентов)

            transaction.Id = default;
            return await _unitOfWork.UserTransactionRepository.CreateAsync(transaction, token);
        }

        public async Task UpdateAsync(Guid id, UserTransaction transaction, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(transaction, token);

            var storedEntity = await _unitOfWork.UserTransactionRepository.GetByIdAsync(id, token);
            if (storedEntity == null)
                throw new NotFoundException("Not found.");

            transaction.Id = id;

            await _unitOfWork.UserTransactionRepository.UpdateAsync(transaction, token);
        }

        public async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            var entity = await _unitOfWork.UserTransactionRepository.GetByIdAsync(id, token);
            if (entity == null)
                return;

            await _unitOfWork.UserTransactionRepository.DeleteAsync(entity, token);
        }

        public async Task<IEnumerable<UserTransaction>> GetAllAsync(CancellationToken token = default)
        {
            return await _unitOfWork.UserTransactionRepository.GetAllAsync(token);
        }

        public async Task<UserTransaction> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var entity = await _unitOfWork.UserTransactionRepository.GetByIdAsync(id, token);

            return entity ?? throw new NotFoundException("Not found");
        }

        public async Task<PagedCollection<UserTransaction>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
#warning TODO: Use PageParameters dto and validator!

            if (pageNumber < 1)
                throw new ParameterException(nameof(pageNumber));

            if (pageSize < 1)
                throw new ParameterException(nameof(pageSize));

            return await _unitOfWork.UserTransactionRepository.GetPagedAsync(pageNumber, pageSize, token);
        }

        public async Task<IEnumerable<UserTransaction>> GetByUserIdAsync(Guid id, CancellationToken token = default)
        {
            var currentUserId = _currentUserService.GetUserIdOrThrow();
            var isAdmin = _currentUserService.IsAdminOrThrow();
            var isCurrentUser = currentUserId == id;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            return await _unitOfWork.UserTransactionRepository.GetByUserIdAsync(id, token);
        }

        private async Task<bool> IsUserExistsAsync(Guid guid, CancellationToken token = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(guid, token);

            return user != null;
        }
    }
}
