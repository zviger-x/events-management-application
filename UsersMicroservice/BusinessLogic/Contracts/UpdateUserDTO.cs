namespace BusinessLogic.Contracts
{
    public class UpdateUserDTO
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
