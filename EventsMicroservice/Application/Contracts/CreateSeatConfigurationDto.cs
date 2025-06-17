namespace Application.Contracts
{
    public class CreateSeatConfigurationDto
    {
        public string Name { get; set; }
        public float DefaultPrice { get; set; }
        public List<int> Rows { get; set; }
    }
}
