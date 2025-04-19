namespace Application.Contracts
{
    public class CreateEventCommentDto
    {
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public DateTime CreationTime { get; } = DateTime.UtcNow;
    }
}
