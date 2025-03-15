using DataAccess.Entities.Interfaces;

#pragma warning disable CS8618
namespace DataAccess.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public IEnumerable<UserNotification> Notifications { get; set; }
        public IEnumerable<UserTransaction> Transactions { get; set; }
    }
}
