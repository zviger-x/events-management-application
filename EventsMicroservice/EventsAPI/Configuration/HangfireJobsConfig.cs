namespace EventsAPI.Configuration
{
    public class HangfireConfig
    {
        public string ConnectionString { get; set; }

        public JobConfig NotifyCompletedEventsJob { get; set; }
        public JobConfig NotifyUpcomingEventsJob { get; set; }

        public class JobConfig
        {
            public string Name { get; set; }
            public string Cron { get; set; }
        }
    }
}
