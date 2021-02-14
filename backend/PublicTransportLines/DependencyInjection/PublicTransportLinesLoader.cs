using DerMistkaefer.DvbLive.GetPublicTransportLines.Germany;
using Microsoft.Extensions.DependencyInjection;

namespace DerMistkaefer.DvbLive.GetPublicTransportLines.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to add the Public Transport Lines library to the .NET
    /// dependency injection container.
    /// </summary>
    public static class PublicTransportLinesLoader
    {
        /// <summary>
        /// Adds the Public Transport Lines to the dependency injection container.
        /// </summary>
        /// <param name="services">Collection of services for building a dependency injection container.</param>
        public static void AddPublicTransportLines(this IServiceCollection services)
        {
            services.AddTransient<IPublicTransportLinesCollector, DresdenPublicTransportLinesCollector>();
        }
    }
}