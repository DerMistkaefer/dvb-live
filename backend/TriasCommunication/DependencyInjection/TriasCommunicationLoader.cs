using DerMistkaefer.DvbLive.TriasCommunication.Configuration;
using DerMistkaefer.DvbLive.TriasCommunication.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using DerMistkaefer.DvbLive.IPGeolocation.DependencyInjection;

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
            services.AddHttpClient(TriasConfiguration.HttpClientFactoryClientName)
                .ConfigurePrimaryHttpMessageHandler(BuildProxyHttpMessageHandler);
            services.AddSingleton<ITriasHttpClient, TriasHttpClient>();
            services.AddSingleton<ITriasCommunicator, TriasCommunicator>();
            services.AddHostedService<TorSharpProxyHostedService>();
            // Disable Logging for HttpClient.
            services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
            services.AddIpGeolocation(configuration);
        }

        private static HttpClientHandler BuildProxyHttpMessageHandler(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IOptions<TriasConfiguration>>();
            if (config is null)
            {
                throw new NullReferenceException($"The Configuration '{nameof(TriasConfiguration)}' could not be found.");
            }

            return new HttpClientHandler
            {
                Proxy = new WebProxy(new Uri($"http://localhost:{config.Value.TorSharpSettings.PrivoxySettings.Port}"))
            };
        }
    }
}
