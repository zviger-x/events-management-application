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
    }
}
