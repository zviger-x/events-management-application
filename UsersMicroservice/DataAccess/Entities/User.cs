using DataAccess.Entities.Interfaces;

namespace DataAccess.Entities
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
    }
}
