using BusinessLogic.Models;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    public class UserTransactionService : BaseService<UserTransaction>, IUserTransactionService
    {
        public UserTransactionService(IUnitOfWork unitOfWork, IUserTransactionValidator validator)
            : base(unitOfWork, validator)
        {
        }

        // Пока что я буду просто выбрасывать ошибки. Но в будущем будет выбрасываться коды ошибки,
        // которые будут поступать в frontend
        public override async Task<Response> CreateAsync(UserTransaction entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            if (validationResult.IsValid)
                return Response.Fail(validationResult);

            await _unitOfWork.UserTransactionRepository.CreateAsync(entity);

            return Response.Success();
        }

        public override async Task<Response> UpdateAsync(UserTransaction entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            if (validationResult.IsValid)
                return Response.Fail(validationResult);

            await _unitOfWork.UserTransactionRepository.UpdateAsync(entity);

            return Response.Success();
        }

        public override async Task<Response> DeleteAsync(UserTransaction entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            if (validationResult.IsValid)
                return Response.Fail(validationResult);

            await _unitOfWork.UserTransactionRepository.DeleteAsync(entity);

            return Response.Success();
        }

        public override Response<IQueryable<UserTransaction>> GetAll()
        {
            var collection = _unitOfWork.UserTransactionRepository.GetAll();
            return Response.Success(collection);
        }

        public override async Task<Response<UserTransaction>> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.UserTransactionRepository.GetByIdAsync(id);
            return Response.Success(user);
        }
    }
}
