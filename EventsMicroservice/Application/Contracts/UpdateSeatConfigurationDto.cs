namespace Application.Contracts
{
    public class UpdateSeatConfigurationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float DefaultPrice { get; set; }
        public List<int> Rows { get; set; }
    }
}
