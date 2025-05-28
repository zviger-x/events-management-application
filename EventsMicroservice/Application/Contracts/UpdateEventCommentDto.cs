namespace Application.Contracts
{
    public class UpdateEventCommentDto
    {
        public string Text { get; set; }
        public DateTimeOffset? EditedAt { get; } = DateTimeOffset.UtcNow;
    }
}
