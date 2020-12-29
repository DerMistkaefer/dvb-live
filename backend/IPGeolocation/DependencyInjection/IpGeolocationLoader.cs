using System;
using DerMistkaefer.DvbLive.IPGeolocation.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DerMistkaefer.DvbLive.IPGeolocation.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to add the IPGeolocation library to the .NET
    /// dependency injection container.
    /// </summary>
    public static class IpGeolocationLoader
    {
        /// <summary>
        /// Adds the IPGeolocation to the dependency injection container.
        /// </summary>
        /// <param name="services">Collection of services for building a dependency injection container.</param>
        /// <param name="configuration">Configuration</param>
        public static void AddIpGeolocation(this IServiceCollection services, IConfiguration configuration)
        {          
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            services.Configure<IpGeolocationConfiguration>(configuration.GetSection("IPGeolocation"));
            services.AddSingleton<IIpGeolocation, IpStackGeolocation>();
        }
    }
}