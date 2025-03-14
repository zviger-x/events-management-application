using BusinessLogic.Models;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

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
        public override async Task<Response> CreateAsync(UserNotification entity, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(entity, token);
            if (validationResult.IsValid)
                return Response.Fail(validationResult);

            entity.Id = default;
            await _unitOfWork.UserNotificationRepository.CreateAsync(entity, token);

            return Response.Success();
        }

        public override async Task<Response> UpdateAsync(UserNotification entity, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(entity, token);
            if (validationResult.IsValid)
                return Response.Fail(validationResult);

            await _unitOfWork.UserNotificationRepository.UpdateAsync(entity, token);

            return Response.Success();
        }

        public override async Task<Response> DeleteAsync(Guid id, CancellationToken token = default)
        {
            var notification = new UserNotification() { Id = id };
            await _unitOfWork.UserNotificationRepository.DeleteAsync(notification, token);
            return Response.Success();
        }

        public override Response<IEnumerable<UserNotification>> GetAll()
        {
            var collection = _unitOfWork.UserNotificationRepository.GetAll();
            return Response.Success(collection);
        }

        public override async Task<Response<UserNotification>> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var user = await _unitOfWork.UserNotificationRepository.GetByIdAsync(id, token);
            return Response.Success(user);
        }
    }
}
