using AutoMapper;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    public class UserNotificationService : BaseService<UserNotification>, IUserNotificationService
    {
        public UserNotificationService(IUnitOfWork unitOfWork, IMapper mapper, IUserNotificationValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<IEnumerable<UserNotification>> GetByUserIdAsync(Guid id, CancellationToken token = default)
        {
            return await _unitOfWork.UserNotificationRepository.GetByUserIdAsync(id, token);
        }
    }
}
