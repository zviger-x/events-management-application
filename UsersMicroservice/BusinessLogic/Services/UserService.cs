using AutoMapper;
using BusinessLogic.Caching.Constants;
using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;
using Shared.Caching.Services.Interfaces;
using Shared.Common;
using Shared.Exceptions.ServerExceptions;
using ValidationException = Shared.Exceptions.ServerExceptions.ValidationException;

namespace BusinessLogic.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IValidator<UpdateUserDto> _updateUserValidator;
        private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
        private readonly IValidator<ChangeUserRoleDto> _changeUserRoleValidator;
        private readonly IValidator<PageParameters> _pageParametersValidator;

        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ICacheService _cacheService;

        public UserService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<UpdateUserDto> updateUserValidator,
            IValidator<ChangePasswordDto> changePasswordValidator,
            IValidator<ChangeUserRoleDto> changeUserRoleValidator,
            IValidator<PageParameters> pageParametersValidator,
            IPasswordHashingService passwordHashingService,
            ICacheService cacheService)
            : base(unitOfWork, mapper)
        {
            _updateUserValidator = updateUserValidator;
            _changePasswordValidator = changePasswordValidator;
            _changeUserRoleValidator = changeUserRoleValidator;
            _pageParametersValidator = pageParametersValidator;

            _passwordHashingService = passwordHashingService;
            _cacheService = cacheService;
        }

        public async Task DeleteAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            var isCurrentUser = currentUserId == targetUserId;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                var entity = await _unitOfWork.UserRepository.GetByIdAsync(targetUserId, token);
                if (entity == null)
                    throw new NotFoundException("User is already deleted or not found.");

                await _unitOfWork.UserRepository.DeleteAsync(entity, token);
            }, cancellationToken);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken token = default)
        {
            var cachedUsers = await _cacheService.GetAsync<IEnumerable<UserDto>>(CacheKeys.AllUsers, token);
            if (cachedUsers != null)
                return cachedUsers;

            var users = await _unitOfWork.UserRepository.GetAllAsync(token);

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            await _cacheService.SetAsync(CacheKeys.AllUsers, userDtos, cancellationToken: token);

            return userDtos;
        }

        public async Task<PagedCollection<UserDto>> GetPagedAsync(PageParameters pageParameters, CancellationToken token = default)
        {
            await _pageParametersValidator.ValidateAndThrowAsync(pageParameters, token);

            var pageNumber = pageParameters.PageNumber;
            var pageSize = pageParameters.PageSize;

            var cachedUsers = await _cacheService.GetAsync<PagedCollection<UserDto>>(CacheKeys.PagedUsers(pageNumber, pageSize), token);
            if (cachedUsers != null)
                return cachedUsers;

            var pagedUsers = await _unitOfWork.UserRepository.GetPagedAsync(pageNumber, pageSize, token);

            var pagedUserDtos = _mapper.Map<PagedCollection<UserDto>>(pagedUsers);

            await _cacheService.SetAsync(CacheKeys.PagedUsers(pageNumber, pageSize), pagedUserDtos, cancellationToken: token);

            return pagedUserDtos;
        }

        public async Task<UserDto> GetByIdAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, CancellationToken token = default)
        {
            var isCurrentUser = currentUserId == targetUserId;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            var cachedUser = await _cacheService.GetAsync<UserDto>(CacheKeys.UserById(targetUserId), token);
            if (cachedUser != null)
                return cachedUser;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(targetUserId, token);
            if (user == null)
                throw new NotFoundException("User not found.");

            var userDto = _mapper.Map<UserDto>(user);

            await _cacheService.SetAsync(CacheKeys.UserById(targetUserId), userDto, cancellationToken: token);

            return userDto;
        }

        public async Task UpdateUserProfileAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, UpdateUserDto userUpdate, CancellationToken cancellationToken)
        {
            await _updateUserValidator.ValidateAndThrowAsync(userUpdate, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(targetUserId, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found");

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

        public async Task ChangePasswordAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken)
        {
            await _changePasswordValidator.ValidateAndThrowAsync(changePasswordDto, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(targetUserId, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found");

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
                throw new ParameterException("There is no user with this Id.");

            user.Role = changeUserRoleDto.Role;

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(user, token);
            }, cancellationToken);

            await _cacheService.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
        }

        public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserRepository.IsExistsAsync(userId, cancellationToken);
        }

        private bool IsCurrentPassword(User storedUser, ChangePasswordDto changePasswordDto, CancellationToken token = default)
        {
            if (storedUser == null)
                return false;

            var isPasswordValid = _passwordHashingService.VerifyPassword(changePasswordDto.CurrentPassword, storedUser.PasswordHash);

            return isPasswordValid;
        }

        public Task<bool> UserExists(Guid userId, CancellationToken cancellationToken = default)
        {
            var isUserExists = _unitOfWork.UserRepository.IsExists(userId, cancellationToken);

            return isUserExists;
        }
    }
}
