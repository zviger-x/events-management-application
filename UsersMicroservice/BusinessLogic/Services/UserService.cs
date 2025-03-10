using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    internal class UserService : BaseService<User>, IUserService
    {
        public UserService(IUnitOfWork unitOfWork, IBaseValidator<User> validator)
            : base(unitOfWork, validator)
        {
        }

        // Пока что я буду просто выбрасывать ошибки. Но в будущем будет выбрасывать DTO, хранящая ошибки,
        // которые будет принимать контроллер и возвращать в frontend
        public override async Task CreateAsync(User entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            // Это временно
            if (validationResult.Any())
                throw new Exception(string.Join(Environment.NewLine, validationResult.SelectMany(e => e.Value).ToList()));

            await _unitOfWork.InvokeWithTransactionAsync(async () =>
            {
                await _unitOfWork.UserRepository.CreateAsync(entity);
            });
        }

        public override async Task UpdateAsync(User entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            // Это временно
            if (validationResult.Any())
                throw new Exception(string.Join(Environment.NewLine, validationResult.SelectMany(e => e.Value).ToList()));

            await _unitOfWork.InvokeWithTransactionAsync(async () =>
            {
                await _unitOfWork.UserRepository.UpdateAsync(entity);
            });
        }

        public override async Task DeleteAsync(User entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            // Это временно
            if (validationResult.Any())
                throw new Exception(string.Join(Environment.NewLine, validationResult.SelectMany(e => e.Value).ToList()));

            await _unitOfWork.InvokeWithTransactionAsync(async () =>
            {
                await _unitOfWork.UserRepository.DeleteAsync(entity);
            });
        }

        public override IQueryable<User> GetAll()
        {
            return _unitOfWork.UserRepository.GetAll();
        }

        public override async Task<User> GetByIdAsync(int id)
        {
            return await _unitOfWork.UserRepository.GetByIdAsync(id);
        }
    }
}
