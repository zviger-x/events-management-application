namespace Application.Contracts
{
    public class CreateEventCommentDto
    {
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public DateTimeOffset CreationTime { get; } = DateTimeOffset.UtcNow;
    }
}
