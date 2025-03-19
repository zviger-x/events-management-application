namespace BusinessLogic.Contracts
{
    #pragma warning disable CS8618
    public class UpdateUserDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
