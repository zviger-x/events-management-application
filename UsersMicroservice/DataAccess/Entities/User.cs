using DataAccess.Entities.Interfaces;
using DataAccess.Enums;

namespace DataAccess.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRoles Role { get; set; }
        public IEnumerable<UserNotification> Notifications { get; set; }
        public IEnumerable<UserTransaction> Transactions { get; set; }
    }
}
