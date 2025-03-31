namespace BusinessLogic.Contracts
{
    public class UpdateUserDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
    }
}
