using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Services
{
    public class UserNotificationService : BaseService<UserNotification>, IUserNotificationService
    {
        public UserNotificationService(IUnitOfWork unitOfWork, IUserNotificationValidator validator)
            : base(unitOfWork, validator)
        {
        }

        // Пока что я буду просто выбрасывать ошибки. Но в будущем будет выбрасываться коды ошибки,
        // которые будут поступать в frontend
        public override async Task CreateAsync(UserNotification entity, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(entity, token);

            entity.Id = default;
            await _unitOfWork.UserNotificationRepository.CreateAsync(entity, token);
        }

        public override async Task UpdateAsync(UserNotification entity, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(entity, token);

            await _unitOfWork.UserNotificationRepository.UpdateAsync(entity, token);
        }

        public override async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            var notification = new UserNotification() { Id = id };
            await _unitOfWork.UserNotificationRepository.DeleteAsync(notification, token);
        }

        public override IEnumerable<UserNotification> GetAll()
        {
            return _unitOfWork.UserNotificationRepository.GetAll();
        }

        public override async Task<UserNotification> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _unitOfWork.UserNotificationRepository.GetByIdAsync(id, token);
        }
    }
}
