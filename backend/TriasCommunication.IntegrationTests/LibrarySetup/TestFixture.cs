using DerMistkaefer.DvbLive.TriasCommunication.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DerMistkaefer.DvbLive.TriasCommunication.IntegrationTests.LibrarySetup
{
    /// <summary>
    /// Fixture that Build the Serivce Provider for all Integration Tests
    /// </summary>
    public class TestFixture
    {
        /// <inheritdoc cref="IServiceProvider"/>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Build the Service Provider for all Integration Tests
        /// </summary>
        public TestFixture()
        {
            ServiceProvider = BuildServiceProvider();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var config = new ConfigurationBuilder();
            config.AddJsonFile("trias-settings.json");
            var configuration = config.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTriasCommunication(configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var hostedServices = serviceProvider.GetServices<IHostedService>();
            foreach (var hostedService in hostedServices)
            {
                Task.Run(() => hostedService.StartAsync(CancellationToken.None));
            }

            return serviceProvider;
        }
    }
}
