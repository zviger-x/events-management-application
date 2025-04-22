using AutoMapper;
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
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<UserNotification> _notificationValidator;
        private readonly IValidator<PageParameters> _pageParametersValidator;

        public UserNotificationService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<UserNotification> notificationValidatorvalidator,
            IValidator<PageParameters> pageParametersValidator,
            ICurrentUserService currentUserService)
            : base(unitOfWork, mapper)
        {
            _notificationValidator = notificationValidatorvalidator;
            _pageParametersValidator = pageParametersValidator;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> CreateAsync(UserNotification notification, CancellationToken token = default)
        {
            await _notificationValidator.ValidateAndThrowAsync(notification, token);

            if (!await _unitOfWork.UserRepository.IsExists(notification.UserId, token))
                throw new ValidationException(
                    UserNotificationValidationErrorCodes.UserIdIsInvalid,
                    UserNotificationValidationMessages.UserIdIsInvalid,
                    nameof(notification.UserId));

            notification.Id = default;
            return await _unitOfWork.UserNotificationRepository.CreateAsync(notification, token);
        }

        public async Task UpdateAsync(Guid id, UserNotification notification, CancellationToken token = default)
        {
            await _notificationValidator.ValidateAndThrowAsync(notification, token);

            var storedEntity = await _unitOfWork.UserNotificationRepository.GetByIdAsync(id, token);
            if (storedEntity == null)
                throw new NotFoundException("Not found.");

            notification.Id = id;

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

        public async Task<IEnumerable<UserNotification>> GetByUserIdAsync(Guid id, CancellationToken token = default)
        {
            var currentUserId = _currentUserService.GetUserIdOrThrow();
            var isAdmin = _currentUserService.IsAdminOrThrow();
            var isCurrentUser = currentUserId == id;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            return await _unitOfWork.UserNotificationRepository.GetByUserIdAsync(id, token);
        }
    }
}
