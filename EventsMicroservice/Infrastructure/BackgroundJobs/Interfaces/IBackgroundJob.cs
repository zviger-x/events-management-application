namespace Infrastructure.BackgroundJobs.Interfaces
{
    public interface IBackgroundJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
