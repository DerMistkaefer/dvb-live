using DerMistkaefer.DvbLive.TriasCommunication.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using System;
using DerMistkaefer.DvbLive.IPGeolocation.DependencyInjection;
using DerMistkaefer.DvbLive.ProxyHttp.FreeProxySharp;

namespace DerMistkaefer.DvbLive.TriasCommunication.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to add the TriasCommunication library to the .NET Core
    /// dependency injection container.
    /// </summary>
    public static class TriasCommunicationLoader
    {
        /// <summary>
        /// Adds the TriasCommunicator to the dependency injection container.
        /// </summary>
        /// <param name="services">Collection of services for building a dependency injection container.</param>
        /// <param name="configuration">Configuration</param>
        public static void AddTriasCommunication(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            services.Configure<TriasConfiguration>(configuration.GetSection("TriasCommunication"));
            services.AddHttpClient();
            services.AddProxyHttpFreeProxySharp();
            services.AddSingleton<ITriasHttpClient, TriasHttpClient>();
            services.AddSingleton<ITriasCommunicator, TriasCommunicator>();
            // Disable Logging for HttpClient.
            services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
            services.AddIpGeolocation(configuration);
        }
    }
}
