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

namespace BusinessLogic.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUpdateUserDTOValidator _updateUserValidator;
        private readonly IChangePasswordDTOValidator _changePasswordValidator;
        private readonly IChangeUserRoleDTOValidator _changeUserRoleValidator;

        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserValidator validator,
            IUpdateUserDTOValidator updateUserValidator,
            IChangePasswordDTOValidator changePasswordValidator,
            IChangeUserRoleDTOValidator changeUserRoleValidator,
            IPasswordHashingService passwordHashingService,
            ICacheService cacheService,
            ILogger<UserService> logger)
            : base(unitOfWork, mapper, validator)
        {
            _updateUserValidator = updateUserValidator;
            _changePasswordValidator = changePasswordValidator;
            _changeUserRoleValidator = changeUserRoleValidator;

            _passwordHashingService = passwordHashingService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public override Task CreateAsync(User entity, CancellationToken token = default)
        {
            _logger.LogWarning("The deprecated Create method was used. This method is disabled because it does not have the ability to hash the password and generate tokens.");
            throw new InvalidOperationException("Please use the Register method from AuthService instead. This method cannot hash the password and return Jwt token.");
        }

        public override async Task UpdateAsync(User entity, CancellationToken token = default)
        {
            _logger.LogWarning("The deprecated Update method was used. The User model was mapped to UpdateUserDTO and the UpdateUserProfileAsync method was called. Please use the UpdateUserProfileAsync method instead.");

            var updateDTO = _mapper.Map<UpdateUserDTO>(entity);

            await UpdateUserProfileAsync(updateDTO, token);
        }

        public override async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await base.DeleteAsync(id, token);
            }, token);
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
            var cachedUser = await _cacheService.GetAsync<User>(CacheKeys.UserById(id), token);
            if (cachedUser != null)
            {
                cachedUser.PasswordHash = string.Empty;
                return cachedUser;
            }

            var user = await base.GetByIdAsync(id, token);
            if (user == null)
                return null;

            user.PasswordHash = string.Empty;
            await _cacheService.SetAsync(CacheKeys.UserById(id), user, token);

            return user;
        }

        public async Task UpdateUserProfileAsync(UpdateUserDTO userUpdate, CancellationToken cancellationToken)
        {
            await _updateUserValidator.ValidateAndThrowAsync(userUpdate, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userUpdate.Id, cancellationToken);
            if (user == null)
                throw new ArgumentException("There is no user with this Id.");

            _mapper.Map(userUpdate, user);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(user, token);
            }, cancellationToken);

            await _cacheService.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
        }

        public async Task ChangePasswordAsync(ChangePasswordDTO changePasswordDto, CancellationToken cancellationToken)
        {
            await _changePasswordValidator.ValidateAndThrowAsync(changePasswordDto, cancellationToken);

            if (!await IsCurrentPassword(changePasswordDto, cancellationToken))
                throw new ServiceValidationException(
                    ChangePasswordValidationErrorCodes.CurrentPasswordIsInvalid,
                    ChangePasswordValidationMessages.CurrentPasswordIsInvalid,
                    nameof(changePasswordDto.CurrentPassword));

            var user = await _unitOfWork.UserRepository.GetByIdAsync(changePasswordDto.Id, cancellationToken);
            if (user == null)
                throw new ArgumentException("There is no user with this Id.");

            user.PasswordHash = _passwordHashingService.HashPassword(changePasswordDto.NewPassword);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(user, token);
            }, cancellationToken);
        }

        public async Task ChangeUserRoleAsync(ChangeUserRoleDTO changeUserRole, CancellationToken cancellationToken = default)
        {
            await _changeUserRoleValidator.ValidateAndThrowAsync(changeUserRole, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(changeUserRole.Id, cancellationToken);
            if (user == null)
                throw new ArgumentException("There is no user with this Id.");

            user.Role = changeUserRole.Role;

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(user, token);
            }, cancellationToken);

            await _cacheService.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
        }

        private async Task<bool> IsCurrentPassword(ChangePasswordDTO dto, CancellationToken token = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(dto.Id, token);

            if (user == null || !_passwordHashingService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                return false;

            return true;
        }
    }
}
