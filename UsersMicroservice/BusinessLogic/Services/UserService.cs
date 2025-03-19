using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.Entities.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IUserValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public override async Task CreateAsync(User entity, CancellationToken token = default)
        {
            #warning использовать сервис Auth.Register()
            await _validator.ValidateAndThrowAsync(entity, token);

            entity.Id = default;
            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.CreateAsync(entity, token);
            }, token);
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

        public async Task UpdateUserProfileAsync(UpdateUserDTO userUpdate, CancellationToken cancellationToken)
        {
            #warning Сделать валидацию моделей

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userUpdate.Id);
            _mapper.Map(userUpdate, user);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(user, token);
            }, cancellationToken);
        }

        public Task ChangePasswordAsync(ChangePasswordDTO changePassword, CancellationToken cancellationToken)
        {
            #warning Сделать валидацию моделей
            #warning Сделать изменение пароля.
            return Task.FromException(new NotImplementedException());
        }
    }
}
