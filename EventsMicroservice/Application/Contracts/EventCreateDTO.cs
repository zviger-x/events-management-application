namespace Application.Contracts
{
    public class EventDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = default!;
        public DateTime PurchaseDeadline { get; set; }
        public byte[]? Image { get; set; }
    }
}
