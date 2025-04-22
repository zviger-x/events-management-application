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
    public class UserNotificationService : BaseService, IUserNotificationService
    {
        private readonly IValidator<UserNotification> _validator;
        private readonly ICurrentUserService _currentUserService;

        public UserNotificationService(IUnitOfWork unitOfWork, IMapper mapper, IUserNotificationValidator validator, ICurrentUserService currentUserService)
            : base(unitOfWork, mapper)
        {
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> CreateAsync(UserNotification notification, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(notification, token);

            if (!await IsUserExistsAsync(notification.UserId, token))
                throw new ValidationException(
                    UserNotificationValidationErrorCodes.UserIdIsInvalid,
                    UserNotificationValidationMessages.UserIdIsInvalid,
                    nameof(notification.UserId));

            notification.Id = default;
            return await _unitOfWork.UserNotificationRepository.CreateAsync(notification, token);
        }

        public async Task UpdateAsync(Guid id, UserNotification notification, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(notification, token);

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

        public async Task<PagedCollection<UserNotification>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
#warning TODO: Use PageParameters dto and validator!

            if (pageNumber < 1)
                throw new ParameterException(nameof(pageNumber));

            if (pageSize < 1)
                throw new ParameterException(nameof(pageSize));

            return await _unitOfWork.UserNotificationRepository.GetPagedAsync(pageNumber, pageSize, token);
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

        private async Task<bool> IsUserExistsAsync(Guid guid, CancellationToken token = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(guid, token);

            return user != null;
        }
    }
}
