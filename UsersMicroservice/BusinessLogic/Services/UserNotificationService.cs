using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    internal class UserNotificationService : BaseService<UserNotification>, IUserNotificationService
    {
        public UserNotificationService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public override async Task CreateAsync(UserNotification entity)
        {
            await _unitOfWork.UserNotificationRepository.CreateAsync(entity);
        }

        public override async Task DeleteAsync(UserNotification entity)
        {
            await _unitOfWork.UserNotificationRepository.DeleteAsync(entity);
        }

        public override async Task UpdateAsync(UserNotification entity)
        {
            await _unitOfWork.UserNotificationRepository.UpdateAsync(entity);
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
