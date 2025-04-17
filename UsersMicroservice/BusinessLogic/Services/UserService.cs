using AutoMapper;
using BusinessLogic.Caching.Constants;
using BusinessLogic.Caching.Interfaces;
using BusinessLogic.Contracts;
using BusinessLogic.Exceptions;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ValidationException = BusinessLogic.Exceptions.ValidationException;

namespace BusinessLogic.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUpdateUserDTOValidator _updateUserValidator;
        private readonly IChangePasswordDTOValidator _changePasswordValidator;
        private readonly IChangeUserRoleDTOValidator _changeUserRoleValidator;

        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserValidator validator,
            IUpdateUserDTOValidator updateUserValidator,
            IChangePasswordDTOValidator changePasswordValidator,
            IChangeUserRoleDTOValidator changeUserRoleValidator,
            IPasswordHashingService passwordHashingService,
            ICurrentUserService currentUserService,
            ICacheService cacheService,
            ILogger<UserService> logger)
            : base(unitOfWork, mapper, validator)
        {
            _updateUserValidator = updateUserValidator;
            _changePasswordValidator = changePasswordValidator;
            _changeUserRoleValidator = changeUserRoleValidator;

            _passwordHashingService = passwordHashingService;
            _currentUserService = currentUserService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserIdOrThrow();
            var isAdmin = _currentUserService.IsAdminOrThrow();
            var isCurrentUser = currentUserId == id;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await base.DeleteAsync(id, cancellationToken);
            }, cancellationToken);
        }

        public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken token = default)
        {
            var cachedUsers = await _cacheService.GetAsync<List<User>>(CacheKeys.AllUsers, token);
            if (cachedUsers != null)
                return cachedUsers;

            var users = await base.GetAllAsync(token);
            foreach (var user in users)
                user.PasswordHash = string.Empty;

            await _cacheService.SetAsync(CacheKeys.AllUsers, users, token);

            return users;
        }

        public override async Task<PagedCollection<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            var cachedUsers = await _cacheService.GetAsync<PagedCollection<User>>(CacheKeys.PagedUsers(pageNumber, pageSize), token);
            if (cachedUsers != null)
                return cachedUsers;

            var users = await base.GetPagedAsync(pageNumber, pageSize, token);
            foreach (var user in users.Items)
                user.PasswordHash = string.Empty;

            await _cacheService.SetAsync(CacheKeys.PagedUsers(pageNumber, pageSize), users, token);

            return users;
        }

        public override async Task<User> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var currentUserId = _currentUserService.GetUserIdOrThrow();
            var isAdmin = _currentUserService.IsAdminOrThrow();
            var isCurrentUser = currentUserId == id;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            var cachedUser = await _cacheService.GetAsync<User>(CacheKeys.UserById(id), token);
            if (cachedUser != null)
                return cachedUser;

            var user = await base.GetByIdAsync(id, token);
            if (user == null)
                throw new NotFoundException("User not found.");

            user.PasswordHash = string.Empty;
            await _cacheService.SetAsync(CacheKeys.UserById(id), user, token);

            return user;
        }

        public async Task UpdateUserProfileAsync(Guid userRouteId, UpdateUserDTO userUpdate, CancellationToken cancellationToken)
        {
            await _updateUserValidator.ValidateAndThrowAsync(userUpdate, cancellationToken);

            if (userRouteId != userUpdate.UserId)
                throw new ParameterException("You are not allowed to change this.");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userUpdate.UserId, cancellationToken);
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

        public async Task ChangePasswordAsync(Guid userRouteId, ChangePasswordDTO changePasswordDto, CancellationToken cancellationToken)
        {
            await _changePasswordValidator.ValidateAndThrowAsync(changePasswordDto, cancellationToken);

            if (userRouteId != changePasswordDto.UserId)
                throw new ParameterException("You are not allowed to change this.");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(changePasswordDto.UserId, cancellationToken);
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

        public async Task ChangeUserRoleAsync(Guid userRouteId, ChangeUserRoleDTO changeUserRole, CancellationToken cancellationToken = default)
        {
            await _changeUserRoleValidator.ValidateAndThrowAsync(changeUserRole, cancellationToken);

            if (userRouteId != changeUserRole.UserId)
                throw new ParameterException("You are not allowed to change this.");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(changeUserRole.UserId, cancellationToken);
            if (user == null)
                throw new ArgumentException("There is no user with this Id.");

            user.Role = changeUserRole.Role;

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(user, token);
            }, cancellationToken);

            await _cacheService.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
        }

        private bool IsCurrentPassword(User storedUser, ChangePasswordDTO dto, CancellationToken token = default)
        {
            if (storedUser == null || !_passwordHashingService.VerifyPassword(dto.CurrentPassword, storedUser.PasswordHash))
                return false;

            return true;
        }
    }
}
