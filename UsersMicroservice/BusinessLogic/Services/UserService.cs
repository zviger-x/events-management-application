using AutoMapper;
using BusinessLogic.Caching.Constants;
using BusinessLogic.Caching.Interfaces;
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
    public class UserService : BaseService, IUserService
    {
        private readonly IValidator<UpdateUserDto> _updateUserValidator;
        private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
        private readonly IValidator<ChangeUserRoleDto> _changeUserRoleValidator;
        private readonly IValidator<PageParameters> _pageParametersValidator;

        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICacheService _cacheService;

        public UserService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<UpdateUserDto> updateUserValidator,
            IValidator<ChangePasswordDto> changePasswordValidator,
            IValidator<ChangeUserRoleDto> changeUserRoleValidator,
            IValidator<PageParameters> pageParametersValidator,
            IPasswordHashingService passwordHashingService,
            ICurrentUserService currentUserService,
            ICacheService cacheService)
            : base(unitOfWork, mapper)
        {
            _updateUserValidator = updateUserValidator;
            _changePasswordValidator = changePasswordValidator;
            _changeUserRoleValidator = changeUserRoleValidator;
            _pageParametersValidator = pageParametersValidator;

            _passwordHashingService = passwordHashingService;
            _currentUserService = currentUserService;
            _cacheService = cacheService;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserIdOrThrow();
            var isAdmin = _currentUserService.IsAdminOrThrow();
            var isCurrentUser = currentUserId == id;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                var entity = await _unitOfWork.UserRepository.GetByIdAsync(id, token);
                if (entity == null)
                    return;

                await _unitOfWork.UserRepository.DeleteAsync(entity, token);
            }, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken token = default)
        {
            var cachedUsers = await _cacheService.GetAsync<List<User>>(CacheKeys.AllUsers, token);
            if (cachedUsers != null)
                return cachedUsers;

            var users = await _unitOfWork.UserRepository.GetAllAsync(token);
            foreach (var user in users)
                user.PasswordHash = string.Empty;

            await _cacheService.SetAsync(CacheKeys.AllUsers, users, token);

            return users;
        }

        public async Task<PagedCollection<User>> GetPagedAsync(PageParameters pageParameters, CancellationToken token = default)
        {
            await _pageParametersValidator.ValidateAndThrowAsync(pageParameters, token);

            var pageNumber = pageParameters.PageNumber;
            var pageSize = pageParameters.PageSize;

            var cachedUsers = await _cacheService.GetAsync<PagedCollection<User>>(CacheKeys.PagedUsers(pageNumber, pageSize), token);
            if (cachedUsers != null)
                return cachedUsers;

            var users = await _unitOfWork.UserRepository.GetPagedAsync(pageNumber, pageSize, token);
            foreach (var user in users.Items)
                user.PasswordHash = string.Empty;

            await _cacheService.SetAsync(CacheKeys.PagedUsers(pageNumber, pageSize), users, token);

            return users;
        }

        public async Task<User> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var currentUserId = _currentUserService.GetUserIdOrThrow();
            var isAdmin = _currentUserService.IsAdminOrThrow();
            var isCurrentUser = currentUserId == id;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            var cachedUser = await _cacheService.GetAsync<User>(CacheKeys.UserById(id), token);
            if (cachedUser != null)
                return cachedUser;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(id, token);
            if (user == null)
                throw new NotFoundException("User not found.");

            user.PasswordHash = string.Empty;
            await _cacheService.SetAsync(CacheKeys.UserById(id), user, token);

            return user;
        }

        public async Task UpdateUserProfileAsync(Guid userId, UpdateUserDto userUpdate, CancellationToken cancellationToken)
        {
            await _updateUserValidator.ValidateAndThrowAsync(userUpdate, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found");

            var currentUserId = _currentUserService.GetUserIdOrThrow();
            var isAdmin = _currentUserService.IsAdminOrThrow();
            var isCurrentUser = currentUserId == user.Id;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            _mapper.Map(userUpdate, user);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(user, token);
            }, cancellationToken);

            await _cacheService.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken)
        {
            await _changePasswordValidator.ValidateAndThrowAsync(changePasswordDto, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found");

            var currentUserId = _currentUserService.GetUserIdOrThrow();
            var isAdmin = _currentUserService.IsAdminOrThrow();
            var isCurrentUser = currentUserId == user.Id;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            if (!IsCurrentPassword(user, changePasswordDto, cancellationToken))
                throw new ValidationException(
                    ChangePasswordValidationErrorCodes.CurrentPasswordIsInvalid,
                    ChangePasswordValidationMessages.CurrentPasswordIsInvalid,
                    nameof(changePasswordDto.CurrentPassword));

            user.PasswordHash = _passwordHashingService.HashPassword(changePasswordDto.NewPassword);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(user, token);
            }, cancellationToken);
        }

        public async Task ChangeUserRoleAsync(Guid userId, ChangeUserRoleDto changeUserRoleDto, CancellationToken cancellationToken = default)
        {
            await _changeUserRoleValidator.ValidateAndThrowAsync(changeUserRoleDto, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new ArgumentException("There is no user with this Id.");

            user.Role = changeUserRoleDto.Role;

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(user, token);
            }, cancellationToken);

            await _cacheService.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
        }

        private bool IsCurrentPassword(User storedUser, ChangePasswordDto changePasswordDto, CancellationToken token = default)
        {
            if (storedUser == null)
                return false;

            var isPasswordValid = _passwordHashingService.VerifyPassword(changePasswordDto.CurrentPassword, storedUser.PasswordHash);

            return isPasswordValid;
        }
    }
}
