using DataAccess.Entities.Interfaces;

namespace DataAccess.Entities
{
    public class UserNotification : IEntity
    {
        public int Id { get; set; }
        public int UsertId { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
    }
}
