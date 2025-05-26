using Hangfire.Dashboard;

namespace Shared.Hangfire.Filters
{
    public class HangfireAllowAllAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
