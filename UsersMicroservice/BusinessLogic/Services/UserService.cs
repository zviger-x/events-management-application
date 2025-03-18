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
