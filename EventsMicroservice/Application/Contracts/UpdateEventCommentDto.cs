namespace Application.Contracts
{
    public class UpdateEventCommentDto
    {
        public string Text { get; set; }
        public DateTime? EditedAt { get; } = DateTime.UtcNow;
    }
}
