using System.Text;
using System.Text.Json.Nodes;

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
            var mergedRoutes = new JsonArray();
            var files = Directory.GetFiles(directory, "*.json");

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var routesArray = JsonNode.Parse(json)?.AsArray();
                if (routesArray != null)
                {
                    foreach (var route in routesArray)
                    {
                        if (route == null)
                            continue;

                        var routeCopy = JsonNode.Parse(route.ToJsonString());
                        mergedRoutes.Add(routeCopy);
                    }
                }
            }

            var finalObject = new JsonObject { ["Routes"] = mergedRoutes };
            var jsonString = finalObject.ToJsonString();
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            using var jsonStream = new MemoryStream(jsonBytes);
            builder.AddJsonStream(jsonStream);

            return builder;
        }
    }
}
