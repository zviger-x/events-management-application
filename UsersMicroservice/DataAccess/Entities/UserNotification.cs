using DataAccess.Entities.Interfaces;

#pragma warning disable CS8618
namespace DataAccess.Entities
{
    public class UserNotification : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
    }
}
