using DataAccess.Entities.Interfaces;

namespace DataAccess.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public UserRoles Role { get; set; }
        public IEnumerable<UserNotification> Notifications { get; set; } = default!;
        public IEnumerable<UserTransaction> Transactions { get; set; } = default!;
    }
}
