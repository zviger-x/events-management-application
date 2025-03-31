using DataAccess.Entities;

namespace BusinessLogic.Contracts
{
    public class ChangeUserRoleDTO
    {
        public Guid Id { get; set; }
        public UserRoles Role { get; set; }
    }
}
