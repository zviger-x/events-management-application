using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Binds a configuration section to a strongly-typed configuration class, 
        /// registers it with the service collection, and returns the configured instance.
        /// </summary>
        /// <typeparam name="TConfig">The type of the configuration class to bind to.</typeparam>
        /// <param name="services">The service collection to register the configuration with.</param>
        /// <param name="configuration">The application's configuration root.</param>
        /// <param name="sectionName">The name of the configuration section to bind.</param>
        /// <returns>The registered configuration instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the specified configuration section is not found or the configuration object could not be created.
        /// </exception>
        public static TConfig ConfigureAndReceive<TConfig>(this IServiceCollection services, IConfiguration configuration, string sectionName)
            where TConfig : class, new()
        {
            var section = configuration.GetSection(sectionName);

            if (!section.Exists())
                throw new InvalidOperationException($"Configuration section '{sectionName}' not found.");

            var config = section.Get<TConfig>();
            if (config == null)
                throw new InvalidOperationException($"Failed to create configuration object for section '{sectionName}'.");

            services.Configure<TConfig>(section);
            return config;
        }
    }
}
