using DerMistkaefer.DvbLive.TriasCommunication.Data;
using DerMistkaefer.DvbLive.TriasCommunication.IntegrationTests.LibrarySetup;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace DerMistkaefer.DvbLive.TriasCommunication.IntegrationTests
{
    /// <summary>
    /// Trias Communicator - Integration Tests
    /// </summary>
    [Collection("")]
    public class TriasCommunicatorTests
    {
        private readonly ITriasCommunicator _communicator;

        /// <summary>
        /// Initialize the Trias Communicator
        /// </summary>
        public TriasCommunicatorTests(TestFixture testFixture)
        {
            var serviceProvider = testFixture.ServiceProvider;
            _communicator = serviceProvider.GetService<ITriasCommunicator>()!;
        }

        [Fact]
        public async Task LocationInformationStopRequest_DresdenMainStation()
        {
            var response = await _communicator.LocationInformationStopRequest("de:14612:28").ConfigureAwait(false);

            var shouldResponse = new LocationInformationStopResponse
            {
                IdStopPoint = "de:14612:28",
                StopPointName = "Dresden, Dresden Hauptbahnhof",
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


    }
}
