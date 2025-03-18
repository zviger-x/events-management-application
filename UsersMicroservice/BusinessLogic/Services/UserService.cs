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
        public UserService(IUnitOfWork unitOfWork, IUserValidator validator)
            : base(unitOfWork, validator)
        {
        }

        // Пока что я буду просто выбрасывать ошибки. Но в будущем будет выбрасываться коды ошибки,
        // которые будут поступать в frontend
        public override async Task CreateAsync(User entity, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(entity, token);

            entity.Id = default;
            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.CreateAsync(entity, token);
            }, token);
        }

        public override async Task UpdateAsync(User entity, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(entity, token);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(entity, token);
            }, token);
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
            return await _unitOfWork.UserRepository.GetAllAsync(token);
        }

        public override async Task<PagedCollection<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _unitOfWork.UserRepository.GetPagedAsync(pageNumber, pageSize, token);
        }

        public override async Task<User> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id, token);
            if (user != null)
                user.Password = string.Empty;

            return user;
        }

        public Task UpdateUserProfileAsync(UpdateUserDTO userUpdate, CancellationToken cancellationToken)
        {
            #warning Сделать валидацию моделей
            #warning Сделать маппинг и обновление профиля.
            return Task.FromException(new NotImplementedException());
        }

        public Task ChangePasswordAsync(ChangePasswordDTO changePassword, CancellationToken cancellationToken)
        {
            #warning Сделать валидацию моделей
            #warning Сделать изменение пароля.
            return Task.FromException(new NotImplementedException());
        }
    }
}
