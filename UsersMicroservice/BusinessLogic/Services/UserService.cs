using AutoMapper;
using BusinessLogic.Caching.Constants;
using BusinessLogic.Caching.Interfaces;
using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
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
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserValidator validator,
            IUpdateUserDTOValidator updateUserValidator,
            IChangePasswordDTOValidator changePasswordValidator,
            IPasswordHashingService passwordHashingService,
            ICacheService cacheService,
            ILogger<UserService> logger)
            : base(unitOfWork, mapper, validator)
        {
            _updateUserValidator = updateUserValidator;
            _changePasswordValidator = changePasswordValidator;
            _passwordHashingService = passwordHashingService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public override Task CreateAsync(User entity, CancellationToken token = default)
        {
            throw new InvalidOperationException("Please use the Register method from AuthService instead. This method cannot hash the password and return Jwt token");
            // await _validator.ValidateAndThrowAsync(entity, token);
            // 
            // entity.Id = default;
            // await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            // {
            //     await _unitOfWork.UserRepository.CreateAsync(entity, token);
            // }, token);
        }

        public override async Task UpdateAsync(User entity, CancellationToken token = default)
        {
            var updateDTO = _mapper.Map<UpdateUserDTO>(entity);

            await UpdateUserProfileAsync(updateDTO, token);
        }

        public override async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                var user = new User() { Id = id };
                await _unitOfWork.UserRepository.DeleteAsync(user, token);
            }, token);
        }

        public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken token = default)
        {
            var cachedUsers = await _cacheService.GetAsync<List<User>>(CacheKeys.AllUsers);
            if (cachedUsers != null)
                return cachedUsers;

            var users = await base.GetAllAsync(token);
            foreach (var user in users)
                user.PasswordHash = string.Empty;

            await _cacheService.SetAsync(CacheKeys.AllUsers, users);

            return users;
        }

        public override async Task<PagedCollection<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            var cachedUsers = await _cacheService.GetAsync<PagedCollection<User>>(CacheKeys.PagedUsers(pageNumber, pageSize));
            if (cachedUsers != null)
                return cachedUsers;

            var users = await base.GetPagedAsync(pageNumber, pageSize, token);
            foreach (var user in users.Items)
                user.PasswordHash = string.Empty;

            await _cacheService.SetAsync(CacheKeys.PagedUsers(pageNumber, pageSize), users);

            return users;
        }

        public override async Task<User> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var cachedUser = await _cacheService.GetAsync<User>(CacheKeys.UserById(id));
            if (cachedUser != null)
            {
                cachedUser.PasswordHash = string.Empty;
                return cachedUser;
            }

            var user = await base.GetByIdAsync(id, token);
            if (user == null)
                return null;

            user.PasswordHash = string.Empty;
            await _cacheService.SetAsync(CacheKeys.UserById(id), user);

            return user;
        }

        public async Task UpdateUserProfileAsync(UpdateUserDTO userUpdate, CancellationToken cancellationToken)
        {
            await _updateUserValidator.ValidateAndThrowAsync(userUpdate, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userUpdate.Id, cancellationToken);
            _mapper.Map(userUpdate, user);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(user, token);
            }, cancellationToken);

            await _cacheService.RemoveAsync(CacheKeys.UserById(user.Id));
        }

        public async Task ChangePasswordAsync(ChangePasswordDTO changePassword, CancellationToken cancellationToken)
        {
            await _changePasswordValidator.ValidateAndThrowAsync(changePassword, cancellationToken);

            #warning Сделать валидацию текущего пароля

            var user = await _unitOfWork.UserRepository.GetByIdAsync(changePassword.Id, cancellationToken);
            user.PasswordHash = _passwordHashingService.HashPassword(changePassword.NewPassword);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(user, token);
            }, cancellationToken);
        }
    }
}
