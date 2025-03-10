using DataAccess.Entities.Interfaces;

namespace DataAccess.Entities
{
    public class UserNotification : IEntity
    {
        public int Id { get; set; }
        public required int UsertId { get; set; }
        public required string Message { get; set; }
        public required DateTime DateTime { get; set; }
        public required string Status { get; set; }
    }
}
