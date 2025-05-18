namespace Application.Contracts
{
    public class NotificationDto
    {
        public Guid InCacheId { get; init; } = Guid.NewGuid();

        public Guid UserId { get; init; }

        public string Message { get; init; } = string.Empty;

        public DateTime DateTime { get; } = DateTime.UtcNow;

        // Метаданные пока передаются только в signalR и не сохраняются на стороне БД
        // TODO: Сделать сохранение метаданных
        public Dictionary<string, string> Metadata { get; init; } = new();
    }
}
