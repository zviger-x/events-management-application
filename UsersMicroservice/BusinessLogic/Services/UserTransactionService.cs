using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;
using Shared.Common;
using Shared.Exceptions.ServerExceptions;
using ValidationException = Shared.Exceptions.ServerExceptions.ValidationException;

namespace BusinessLogic.Services
{
    public class UserTransactionService : BaseService, IUserTransactionService
    {
        private readonly IValidator<CreateUserTransactionDto> _createUserTransactionDtoValidator;
        private readonly IValidator<UpdateUserTransactionDto> _updateUserTransactionDtoValidator;
        private readonly IValidator<PageParameters> _pageParametersValidator;

        public UserTransactionService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateUserTransactionDto> createUserTransactionDtoValidator,
            IValidator<UpdateUserTransactionDto> updateUserTransactionDtoValidator,
            IValidator<PageParameters> pageParametersValidator)
            : base(unitOfWork, mapper)
        {
            _createUserTransactionDtoValidator = createUserTransactionDtoValidator;
            _updateUserTransactionDtoValidator = updateUserTransactionDtoValidator;
            _pageParametersValidator = pageParametersValidator;
        }

        public async Task<Guid> CreateAsync(CreateUserTransactionDto transactionDto, CancellationToken token = default)
        {
            await _createUserTransactionDtoValidator.ValidateAndThrowAsync(transactionDto, token);

            if (!await _unitOfWork.UserRepository.IsExists(transactionDto.UserId, token))
                throw new ValidationException(
                    UserTransactionValidationErrorCodes.UserIdIsInvalid,
                    UserTransactionValidationMessages.UserIdIsInvalid,
                    nameof(transactionDto.UserId));

            // TODO: Нужно добавить проверку на наличие ивента (gRPC запрос в микросервису ивентов)

            var transaction = _mapper.Map<UserTransaction>(transactionDto);

            return await _unitOfWork.UserTransactionRepository.CreateAsync(transaction, token);
        }

        public async Task UpdateAsync(Guid id, UpdateUserTransactionDto transactionDto, CancellationToken token = default)
        {
            await _updateUserTransactionDtoValidator.ValidateAndThrowAsync(transactionDto, token);

            var storedEntity = await _unitOfWork.UserTransactionRepository.GetByIdAsync(id, token);
            if (storedEntity == null)
                throw new NotFoundException("Not found.");

            var transaction = _mapper.Map(transactionDto, storedEntity);

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

        public async Task<PagedCollection<UserTransaction>> GetPagedAsync(PageParameters pageParameters, CancellationToken token = default)
        {
            await _pageParametersValidator.ValidateAndThrowAsync(pageParameters, token);

            return await _unitOfWork.UserTransactionRepository.GetPagedAsync(pageParameters.PageNumber, pageParameters.PageSize, token);
        }

        public async Task<IEnumerable<UserTransaction>> GetByUserIdAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, CancellationToken token = default)
        {
            var isCurrentUser = currentUserId == targetUserId;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            return await _unitOfWork.UserTransactionRepository.GetByUserIdAsync(targetUserId, token);
        }
    }
}
