namespace Application.Contracts
{
    public class UpdateEventCommentDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime? EditedAt { get; } = DateTime.UtcNow;
    }
}
