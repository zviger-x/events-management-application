using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.Exceptions;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;
using ValidationException = BusinessLogic.Exceptions.ValidationException;

namespace BusinessLogic.Services
{
    public class UserNotificationService : BaseService, IUserNotificationService
    {
        private readonly IValidator<CreateUserNotificationDto> _createUserNotificationDtoValidator;
        private readonly IValidator<UpdateUserNotificationDto> _updateUserNotificationDtoValidator;
        private readonly IValidator<PageParameters> _pageParametersValidator;

        public UserNotificationService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateUserNotificationDto> createUserNotificationDtoValidator,
            IValidator<UpdateUserNotificationDto> updateUserNotificationDtoValidator,
            IValidator<PageParameters> pageParametersValidator)
            : base(unitOfWork, mapper)
        {
            _createUserNotificationDtoValidator = createUserNotificationDtoValidator;
            _updateUserNotificationDtoValidator = updateUserNotificationDtoValidator;
            _pageParametersValidator = pageParametersValidator;
        }

        public async Task<Guid> CreateAsync(CreateUserNotificationDto notificationDto, CancellationToken token = default)
        {
            await _createUserNotificationDtoValidator.ValidateAndThrowAsync(notificationDto, token);

            if (!await _unitOfWork.UserRepository.IsExists(notificationDto.UserId, token))
                throw new ValidationException(
                    UserNotificationValidationErrorCodes.UserIdIsInvalid,
                    UserNotificationValidationMessages.UserIdIsInvalid,
                    nameof(notificationDto.UserId));

            var notification = _mapper.Map<UserNotification>(notificationDto);

            return await _unitOfWork.UserNotificationRepository.CreateAsync(notification, token);
        }

        public async Task UpdateAsync(Guid id, UpdateUserNotificationDto notificationDto, CancellationToken token = default)
        {
            await _updateUserNotificationDtoValidator.ValidateAndThrowAsync(notificationDto, token);

            var storedEntity = await _unitOfWork.UserNotificationRepository.GetByIdAsync(id, token);
            if (storedEntity == null)
                throw new NotFoundException("Not found.");

            var notification = _mapper.Map(notificationDto, storedEntity);

            await _unitOfWork.UserNotificationRepository.UpdateAsync(notification, token);
        }

        public async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            var entity = await _unitOfWork.UserNotificationRepository.GetByIdAsync(id, token);
            if (entity == null)
                return;

            await _unitOfWork.UserNotificationRepository.DeleteAsync(entity, token);
        }

        public async Task<IEnumerable<UserNotification>> GetAllAsync(CancellationToken token = default)
        {
            return await _unitOfWork.UserNotificationRepository.GetAllAsync(token);
        }

        public async Task<UserNotification> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var entity = await _unitOfWork.UserNotificationRepository.GetByIdAsync(id, token);

            return entity ?? throw new NotFoundException("Not found");
        }

        public async Task<PagedCollection<UserNotification>> GetPagedAsync(PageParameters pageParameters, CancellationToken token = default)
        {
            await _pageParametersValidator.ValidateAndThrowAsync(pageParameters, token);

            return await _unitOfWork.UserNotificationRepository.GetPagedAsync(pageParameters.PageNumber, pageParameters.PageSize, token);
        }

        public async Task<IEnumerable<UserNotification>> GetByUserIdAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, CancellationToken token = default)
        {
            var isCurrentUser = currentUserId == targetUserId;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            return await _unitOfWork.UserNotificationRepository.GetByUserIdAsync(targetUserId, token);
        }
    }
}
