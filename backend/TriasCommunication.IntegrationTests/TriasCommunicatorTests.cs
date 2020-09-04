using DerMistkaefer.DvbLive.TriasCommunication.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DerMistkaefer.DvbLive.TriasCommunication.IntegrationTests
{
    /// <summary>
    /// Trias Communicator - Integration Tests
    /// </summary>
    public class TriasCommunicatorTests
    {
        private readonly TriasCommunicator _communicator;

        /// <summary>
        /// Initalize the Trias Communicator
        /// </summary>
        public TriasCommunicatorTests()
        {
            _communicator = new TriasCommunicator(DefaultHttpClientFactory());
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
            await _communicator.TripRequest().ConfigureAwait(false);
        }

        [Fact]
        public async Task StopEventRequest_DresdenMainStation()
        {
            await _communicator.StopEventRequest("de:14612:28").ConfigureAwait(false);
        }

        private static IHttpClientFactory DefaultHttpClientFactory()
        {
            var serviceProvider = new ServiceCollection();
            serviceProvider.AddHttpClient();

            return serviceProvider.BuildServiceProvider().GetService<IHttpClientFactory>();
        }
    }
}
