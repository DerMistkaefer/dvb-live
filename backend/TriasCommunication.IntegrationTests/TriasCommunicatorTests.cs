using DerMistkaefer.DvbLive.TriasCommunication.Data;
using DerMistkaefer.DvbLive.TriasCommunication.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DerMistkaefer.DvbLive.TriasCommunication.IntegrationTests
{
    /// <summary>
    /// Trias Communicator - Integration Tests
    /// </summary>
    public class TriasCommunicatorTests
    {
        private readonly ITriasCommunicator _communicator;

        /// <summary>
        /// Initalize the Trias Communicator
        /// </summary>
        public TriasCommunicatorTests()
        {
            var serviceProvider = BuildServiceProvider();
            _communicator = serviceProvider.GetService<ITriasCommunicator>()!;
        }

        [Fact]
        public async Task LocationInformationStopRequest_DresdenMainStation()
        {
            var response = await _communicator.LocationInformationStopRequest("de:14612:28").ConfigureAwait(false);

            var shouldResponse = new LocationInformationStopResponse()
            {
                IdStopPoint = "de:14612:28",
                StopPointName = "Dresden, Hauptbahnhof",
                Longitude = 13.73293M,
                Latitude = 51.03993M
            };
            response.Should().BeEquivalentTo(shouldResponse);
        }

        [Fact]
        public async Task TripRequest()
        {
            //await _communicator.TripRequest().ConfigureAwait(false);
        }

        [Fact]
        public async Task StopEventRequest_DresdenMainStation()
        {
            await _communicator.StopEventRequest("de:14612:28").ConfigureAwait(false);
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
