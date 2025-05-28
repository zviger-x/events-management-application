namespace Application.Contracts
{
    public class CreateEventDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Location { get; set; }
        public DateTimeOffset PurchaseDeadline { get; set; }
        public byte[] Image { get; set; }
        public Guid SeatConfigurationId { get; set; }
    }
}
