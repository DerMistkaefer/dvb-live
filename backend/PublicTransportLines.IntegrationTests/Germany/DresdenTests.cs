using System.Net.Http;
using System.Threading.Tasks;
using DerMistkaefer.DvbLive.GetPublicTransportLines;
using DerMistkaefer.DvbLive.GetPublicTransportLines.Germany;
using FluentAssertions;
using Moq;
using Xunit;

namespace DerMistkaefer.DvbLive.PublicTransportLines.IntegrationTests.Germany
{
    /// <summary>
    /// Tests for <see cref="DresdenPublicTransportLinesCollector"/>
    /// </summary>
    public class DresdenTests
    {
        private readonly IPublicTransportLinesCollector _publicTransportLinesCollector;

        /// <summary>
        /// Initialize the <see cref="DresdenPublicTransportLinesCollector"/>
        /// </summary>
        public DresdenTests()
        {
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.CreateClient(string.Empty)).Returns(new HttpClient());
            _publicTransportLinesCollector = new DresdenPublicTransportLinesCollector(httpClientFactory.Object);
        }

        /// <summary>
        /// Test if GetPublicTransportLines works.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetPublicTransportLines()
        {
            var response = await _publicTransportLinesCollector.GetPublicTransportLines().ConfigureAwait(false);
            
            response.Should().HaveCountGreaterOrEqualTo(1);
            response.Should().Contain(predicate: x => x.From == "Kleinzschachwitz" && x.To == "Gorbitz" && x.Title == "Straßenbahnlinie 2", "One Correct Tram Line should be exist.");
            response.Should().Contain(predicate: x => x.From == "Dresden-Löbtau" && x.To == "Somsdorf/Pfaffengrund" && x.Title == "Buslinie A", "One Correct Bus Line should be exist.");
        }
    }
}