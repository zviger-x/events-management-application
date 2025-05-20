using Hangfire;
using Infrastructure.BackgroundJobs.Interfaces;

namespace EventsAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHangfireRecurringJobs(this IApplicationBuilder app)
        {
            // Запускаем задачи по расписанию
            RecurringJob.AddOrUpdate<INotifyCompletedEventsJob>(
                "notify-completed-events-hourly-job",
                job => job.ExecuteAsync(default),
                Cron.Hourly()); // каждый час

            RecurringJob.AddOrUpdate<INotifyUpcomingEventsJob>(
                "notify-upcoming-events-daily-job",
                job => job.ExecuteAsync(default),
                Cron.Daily); // каждый день

            return app;
        }
    }
}
