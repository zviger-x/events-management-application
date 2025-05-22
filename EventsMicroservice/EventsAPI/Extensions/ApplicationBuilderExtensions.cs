using Hangfire;
using Infrastructure.BackgroundJobs.Interfaces;

namespace EventsAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHangfireRecurringJobs(this IApplicationBuilder app)
        {
            RecurringJob.AddOrUpdate<INotifyCompletedEventsJob>(
                "notify-completed-events-hourly-job",
                job => job.ExecuteAsync(default),
                Cron.Hourly());

            RecurringJob.AddOrUpdate<INotifyUpcomingEventsJob>(
                "notify-upcoming-events-daily-job",
                job => job.ExecuteAsync(default),
                Cron.Daily);

            return app;
        }
    }
}
