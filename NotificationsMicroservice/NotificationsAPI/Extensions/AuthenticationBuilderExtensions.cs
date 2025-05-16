using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace NotificationsAPI.Extensions
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder ConfigureSignalRTokenHandling(this AuthenticationBuilder builder, params string[] hubPaths)
        {
            builder.Services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var previousHandler = options.Events?.OnMessageReceived;

                var events = options.Events ?? new JwtBearerEvents();

                events.OnMessageReceived = async context =>
                {
                    if (previousHandler != null)
                        await previousHandler(context);

                    if (!string.IsNullOrEmpty(context.Token))
                        return;

                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) &&
                        hubPaths.Any(h => path.StartsWithSegments(h, StringComparison.OrdinalIgnoreCase)))
                    {
                        context.Token = accessToken;
                    }
                };

                options.Events = events;
            });

            return builder;
        }
    }
}
