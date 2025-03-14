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
        public override async Task<Response> CreateAsync(User entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            if (!validationResult.IsValid)
                return Response.Fail(validationResult);

            await _unitOfWork.InvokeWithTransactionAsync(async () =>
            {
                await _unitOfWork.UserRepository.CreateAsync(entity);
            });

            return Response.Success();
        }

        public override async Task<Response> UpdateAsync(User entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            if (!validationResult.IsValid)
                return Response.Fail(validationResult);

            await _unitOfWork.InvokeWithTransactionAsync(async () =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(entity);
            });

            return Response.Success();
        }

        public override async Task<Response> DeleteAsync(Guid id)
        {
            await _unitOfWork.InvokeWithTransactionAsync(async () =>
            {
                var user = new User() { Id = id };
                await _unitOfWork.UserRepository.DeleteAsync(user);
            });

            return Response.Success();
        }

        public override Response<IEnumerable<User>> GetAll()
        {
            var collection = _unitOfWork.UserRepository.GetAll();
            return Response.Success(collection);
        }

        public override async Task<Response<User>> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            user.Password = string.Empty;
            return Response.Success(user);
        }
    }
}
