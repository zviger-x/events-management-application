using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUpdateUserDTOValidator _updateUserValidator;
        private readonly IChangePasswordDTOValidator _changePasswordValidator;
        private readonly IPasswordHashingService _passwordHashingService;

        public UserService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserValidator validator,
            IUpdateUserDTOValidator updateUserValidator,
            IChangePasswordDTOValidator changePasswordValidator,
            IPasswordHashingService passwordHashingService)
            : base(unitOfWork, mapper, validator)
        {
            _updateUserValidator = updateUserValidator;
            _changePasswordValidator = changePasswordValidator;
            _passwordHashingService = passwordHashingService;
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
            var collection = await base.GetAllAsync(token);
            foreach (var user in collection)
                user.PasswordHash = string.Empty;

            return collection;
        }

        public override async Task<PagedCollection<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            var page = await base.GetPagedAsync(pageNumber, pageSize, token);
            foreach (var user in page.Items)
                user.PasswordHash = string.Empty;

            return page;
        }

        public override async Task<User> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var user = await base.GetByIdAsync(id, token);
            user.PasswordHash = string.Empty;

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
