using BusinessLogic.Models;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

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
        public override async Task<Response> CreateAsync(User entity, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(entity, token);
            if (!validationResult.IsValid)
                return Response.Fail(validationResult);

            entity.Id = default;
            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.CreateAsync(entity, token);
            }, token);

            return Response.Success();
        }

        public override async Task<Response> UpdateAsync(User entity, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(entity, token);
            if (!validationResult.IsValid)
                return Response.Fail(validationResult);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(entity, token);
            }, token);

            return Response.Success();
        }

        public override async Task<Response> DeleteAsync(Guid id, CancellationToken token = default)
        {
            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                var user = new User() { Id = id };
                await _unitOfWork.UserRepository.DeleteAsync(user, token);
            }, token);

            return Response.Success();
        }

        public override Response<IEnumerable<User>> GetAll()
        {
            var collection = _unitOfWork.UserRepository.GetAll();
            return Response.Success(collection);
        }

        public override async Task<Response<User>> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id, token);
            if (user != null)
                user.Password = string.Empty;
            return Response.Success(user);
        }
    }
}
