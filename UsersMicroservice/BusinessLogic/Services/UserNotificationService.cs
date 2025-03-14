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
        public override async Task<Response> CreateAsync(UserNotification entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            if (validationResult.IsValid)
                return Response.Fail(validationResult);

            await _unitOfWork.UserNotificationRepository.CreateAsync(entity);

            return Response.Success();
        }

        public override async Task<Response> UpdateAsync(UserNotification entity)
        {
            var validationResult = await _validator.ValidateAndThrowAsync(entity);
            if (validationResult.IsValid)
                return Response.Fail(validationResult);

            await _unitOfWork.UserNotificationRepository.UpdateAsync(entity);

            return Response.Success();
        }

        public override async Task<Response> DeleteAsync(Guid id)
        {
            var notification = new UserNotification() { Id = id };
            await _unitOfWork.UserNotificationRepository.DeleteAsync(notification);
            return Response.Success();
        }

        public override Response<IEnumerable<UserNotification>> GetAll()
        {
            var collection = _unitOfWork.UserNotificationRepository.GetAll();
            return Response.Success(collection);
        }

        public override async Task<Response<UserNotification>> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserNotificationRepository.GetByIdAsync(id);
            return Response.Success(user);
        }
    }
}
