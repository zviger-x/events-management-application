using Serilog.Events;

namespace Shared.Configuration
{
    public class ELKConfig
    {
        public string LogstashUri { get; set; }
        public LogEventLevel MinimumLevel { get; set; }
    }
}