using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    internal class UserNotificationService : BaseService<UserNotification>, IUserNotificationService
    {
        public UserNotificationService(IUnitOfWork unitOfWork, IBaseValidator<UserNotification> validator)
            : base(unitOfWork, validator)
        {
        }

        // Пока что я буду просто выбрасывать ошибки. Но в будущем будет выбрасывать DTO, хранящая ошибки,
        // которые будет принимать контроллер и возвращать в frontend
        public override async Task CreateAsync(UserNotification entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            // Это временно
            if (validationResult.Any())
                throw new Exception(string.Join(Environment.NewLine, validationResult.SelectMany(e => e.Value).ToList()));

            await _unitOfWork.UserNotificationRepository.CreateAsync(entity);
        }

        public override async Task UpdateAsync(UserNotification entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            // Это временно
            if (validationResult.Any())
                throw new Exception(string.Join(Environment.NewLine, validationResult.SelectMany(e => e.Value).ToList()));

            await _unitOfWork.UserNotificationRepository.UpdateAsync(entity);
        }

        public override async Task DeleteAsync(UserNotification entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            // Это временно
            if (validationResult.Any())
                throw new Exception(string.Join(Environment.NewLine, validationResult.SelectMany(e => e.Value).ToList()));

            await _unitOfWork.UserNotificationRepository.DeleteAsync(entity);
        }

        public override IQueryable<UserNotification> GetAll()
        {
            return _unitOfWork.UserNotificationRepository.GetAll();
        }

        public override async Task<UserNotification> GetByIdAsync(int id)
        {
            return await _unitOfWork.UserNotificationRepository.GetByIdAsync(id);
        }
    }
}
