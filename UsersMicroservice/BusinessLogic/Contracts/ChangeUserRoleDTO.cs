using DataAccess.Enums;

namespace BusinessLogic.Contracts
{
    public class ChangeUserRoleDTO
    {
        public Guid UserId { get; set; }
        public UserRoles Role { get; set; }
    }
}
