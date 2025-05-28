using EventsAPI.Configuration;
using Hangfire;
using Infrastructure.BackgroundJobs.Interfaces;

namespace EventsAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHangfireRecurringJobs(this IApplicationBuilder app, HangfireConfig config)
        {
            RecurringJob.AddOrUpdate<INotifyCompletedEventsJob>(
                config.NotifyCompletedEventsJob.Name,
                job => job.ExecuteAsync(default),
                config.NotifyCompletedEventsJob.Cron);

            RecurringJob.AddOrUpdate<INotifyUpcomingEventsJob>(
                config.NotifyUpcomingEventsJob.Name,
                job => job.ExecuteAsync(default),
                config.NotifyUpcomingEventsJob.Cron);

            return app;
        }
    }
}
