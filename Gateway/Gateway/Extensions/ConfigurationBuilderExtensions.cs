using Newtonsoft.Json.Linq;
using System.Text;

namespace Gateway.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Loads all JSON files from the specified directory, merges them into a single routes array,
        /// and overrides the existing "Routes" section in the Ocelot configuration with the merged array.
        /// </summary>
        /// <param name="builder">The configuration builder to which the updated configuration will be added.</param>
        /// <param name="directory">The path to the directory containing the route JSON files.</param>
        /// <returns>The updated <see cref="IConfigurationBuilder"/> with the overridden Routes section.</returns>
        public static IConfigurationBuilder OverrideOcelotRoutesFromDirectory(
            this IConfigurationBuilder builder,
            string directory)
        {
            var mergedRoutes = new JArray();
            var files = Directory.GetFiles(directory, "*.json");

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var routesArray = JArray.Parse(json);

                mergedRoutes.Merge(routesArray);
            }

            var finalRoutes = new JObject
            {
                ["Routes"] = mergedRoutes
            };

            var jsonString = finalRoutes.ToString();
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            using var jsonStream = new MemoryStream(jsonBytes);

            builder.AddJsonStream(jsonStream);

            return builder;
        }
    }
}
