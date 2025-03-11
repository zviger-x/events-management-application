using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    internal class UserTransactionService : BaseService<UserTransaction>, IUserTransactionService
    {
        public UserTransactionService(IUnitOfWork unitOfWork, IBaseValidator<UserTransaction> validator)
            : base(unitOfWork, validator)
        {
        }

        // Пока что я буду просто выбрасывать ошибки. Но в будущем будет выбрасывать DTO, хранящая ошибки,
        // которые будет принимать контроллер и возвращать в frontend
        public override async Task CreateAsync(UserTransaction entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            // Это временно
            if (validationResult.IsValid)
                throw new Exception(string.Join(Environment.NewLine, validationResult.Errors.SelectMany(e => e.Value).ToList()));

            await _unitOfWork.UserTransactionRepository.CreateAsync(entity);
        }

        public override async Task UpdateAsync(UserTransaction entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            // Это временно
            if (validationResult.IsValid)
                throw new Exception(string.Join(Environment.NewLine, validationResult.Errors.SelectMany(e => e.Value).ToList()));

            await _unitOfWork.UserTransactionRepository.UpdateAsync(entity);
        }

        public override async Task DeleteAsync(UserTransaction entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            // Это временно
            if (validationResult.IsValid)
                throw new Exception(string.Join(Environment.NewLine, validationResult.Errors.SelectMany(e => e.Value).ToList()));

            await _unitOfWork.UserTransactionRepository.DeleteAsync(entity);
        }

        public override IQueryable<UserTransaction> GetAll()
        {
            return _unitOfWork.UserTransactionRepository.GetAll();
        }

        public override async Task<UserTransaction> GetByIdAsync(int id)
        {
            return await _unitOfWork.UserTransactionRepository.GetByIdAsync(id);
        }
    }
}
