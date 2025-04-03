namespace Application.Contracts
{
    public class CreateEventDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public DateTime PurchaseDeadline { get; set; }
        public byte[] Image { get; set; }
        public Guid SeatConfigurationId { get; set; }
    }
}
