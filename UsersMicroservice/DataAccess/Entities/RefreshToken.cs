using DataAccess.Entities.Interfaces;
namespace DataAccess.Entities
{
    public class RefreshToken : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = default!;
        public DateTime Expires { get; set; }
    }
}
