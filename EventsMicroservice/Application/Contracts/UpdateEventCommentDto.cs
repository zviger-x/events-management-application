namespace Application.Contracts
{
    public class UpdateEventCommentDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public DateTime? EditedAt { get; } = DateTime.UtcNow;
    }
}
