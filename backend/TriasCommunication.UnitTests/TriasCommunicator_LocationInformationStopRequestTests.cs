using DerMistkaefer.DvbLive.TriasCommunication.Configuration;
using DerMistkaefer.DvbLive.TriasCommunication.Data;
using DerMistkaefer.DvbLive.TriasCommunication.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DerMistkaefer.DvbLive.TriasCommunication.UnitTests
{
    /// <summary>
    /// Tests for the Trias Communicator - Function LocationInformationStopRequest
    /// </summary>
    public sealed class TriasCommunicator_LocationInformationStopRequestTests : IDisposable
    {
        private readonly MockHttpMessageHandler _mockHttpMessageHandler;
        private readonly TriasHttpClient _triasHttpClient;
        private readonly TriasCommunicator _communicator;

        /// <summary>
        /// Initalize the Trias Communicator
        /// </summary>
        public TriasCommunicator_LocationInformationStopRequestTests()
        {
            var mockConfig = new Mock<IOptions<TriasConfiguration>>();
            mockConfig.Setup(x => x.Value).Returns(new TriasConfiguration(new Uri("http://example.com")));
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(_mockHttpMessageHandler.ToHttpClient());

            _triasHttpClient = new TriasHttpClient(mockHttpClientFactory.Object, mockConfig.Object);
            _communicator = new TriasCommunicator(_triasHttpClient, NullLogger<TriasCommunicator>.Instance);
        }

        public void Dispose()
        {
            _mockHttpMessageHandler.Dispose();
            _triasHttpClient.Dispose();
        }

        [Fact]
        public async Task LocationInformationStopRequest_DresdenMainStation()
        {
            var requestBody = @"<?xml version=""1.0"" encoding=""utf-16""?>
            <Trias xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" version=""1.2"" xmlns=""http://www.vdv.de/trias"">
              <ServiceRequest>
                <RequestTimestamp xmlns=""http://www.siri.org.uk/siri"">2020-07-19T14:13:27.8106445+02:00</RequestTimestamp>
                <RequestorRef xmlns=""http://www.siri.org.uk/siri"">OpenService</RequestorRef>
                <RequestPayload>
                  <LocationInformationRequest>
                    <LocationRef>
                      <StopPointRef>de:14612:28</StopPointRef>
                    </LocationRef>
                    <Restrictions>
                      <Type>stop</Type>
                      <IncludePtModes>true</IncludePtModes>
                    </Restrictions>
                  </LocationInformationRequest>
                </RequestPayload>
              </ServiceRequest>
            </Trias>";
            var respondBody = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <Trias xmlns=""http://www.vdv.de/trias"" version=""1.2"">
                <ServiceDelivery>
                    <ResponseTimestamp xmlns=""http://www.siri.org.uk/siri"">2020-07-19T11:47:11Z</ResponseTimestamp>
                    <ProducerRef xmlns=""http://www.siri.org.uk/siri"">EFAController10.3.11.22-EFA1</ProducerRef>
                    <Status xmlns=""http://www.siri.org.uk/siri"">true</Status>
                    <MoreData>false</MoreData>
                    <Language>de</Language>
                    <CalcTime>58</CalcTime>
                    <DeliveryPayload>
                        <LocationInformationResponse>
                            <LocationResult>
                                <Location>
                                    <StopPoint>
                                        <StopPointRef>de:14612:28</StopPointRef>
                                        <StopPointName>
                                            <Text>Dresden, Hauptbahnhof</Text>
                                            <Language>de</Language>
                                        </StopPointName>
                                        <LocalityRef>14612000:1</LocalityRef>
                                        <WheelchairAccessible>false</WheelchairAccessible>
                                        <Lighting>false</Lighting>
                                        <Covered>false</Covered>
                                    </StopPoint>
                                    <LocationName>
                                        <Text>Dresden</Text>
                                        <Language>de</Language>
                                    </LocationName>
                                    <GeoPosition>
                                        <Longitude>13.73293</Longitude>
                                        <Latitude>51.03993</Latitude>
                                    </GeoPosition>
                                </Location>
                                <Complete>true</Complete>
                                <Probability>0</Probability>
                            </LocationResult>
                        </LocationInformationResponse>
                    </DeliveryPayload>
                </ServiceDelivery>
            </Trias>";
            _mockHttpMessageHandler.When("*")
                // TODO Request Content Matcher didn't function
                //.With(x => x.Content == new StringContent(requestBody, Encoding.UTF8, "text/xml"))
                .Respond("text/xml", respondBody);
            var response = await _communicator.LocationInformationStopRequest("de:14612:28").ConfigureAwait(false);

            var shouldResponse = new LocationInformationStopResponse
            {
                IdStopPoint = "de:14612:28",
                StopPointName = "Dresden, Hauptbahnhof",
                Longitude = 13.73293M,
                Latitude = 51.03993M
            };
            response.Should().BeEquivalentTo(shouldResponse);
        }

        [Fact]
        public async Task LocationInformationStopRequest_MultipleResponse()
        {
            var respondBody = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <Trias xmlns=""http://www.vdv.de/trias"" version=""1.2"">
                <ServiceDelivery>
                    <ResponseTimestamp xmlns=""http://www.siri.org.uk/siri"">2020-07-19T11:47:11Z</ResponseTimestamp>
                    <ProducerRef xmlns=""http://www.siri.org.uk/siri"">EFAController10.3.11.22-EFA1</ProducerRef>
                    <Status xmlns=""http://www.siri.org.uk/siri"">true</Status>
                    <MoreData>false</MoreData>
                    <Language>de</Language>
                    <CalcTime>58</CalcTime>
                    <DeliveryPayload>
                        <LocationInformationResponse>
                            <LocationResult>
                                <Location>
                                    <StopPoint>
                                        <StopPointRef>de:14612:28</StopPointRef>
                                        <StopPointName>
                                            <Text>Dresden, Hauptbahnhof</Text>
                                            <Language>de</Language>
                                        </StopPointName>
                                        <LocalityRef>14612000:1</LocalityRef>
                                        <WheelchairAccessible>false</WheelchairAccessible>
                                        <Lighting>false</Lighting>
                                        <Covered>false</Covered>
                                    </StopPoint>
                                    <LocationName>
                                        <Text>Dresden</Text>
                                        <Language>de</Language>
                                    </LocationName>
                                    <GeoPosition>
                                        <Longitude>13.73293</Longitude>
                                        <Latitude>51.03993</Latitude>
                                    </GeoPosition>
                                </Location>
                                <Complete>true</Complete>
                                <Probability>0</Probability>
                            </LocationResult>
                            <LocationResult>
                                <Location>
                                    <StopPoint>
                                        <StopPointRef>de:14612:22</StopPointRef>
                                        <StopPointName>
                                            <Text>Dresden, Maxstraﬂe (Bf Mitte)</Text>
                                            <Language>de</Language>
                                        </StopPointName>
                                        <LocalityRef>14612000:1</LocalityRef>
                                        <WheelchairAccessible>false</WheelchairAccessible>
                                        <Lighting>false</Lighting>
                                        <Covered>false</Covered>
                                    </StopPoint>
                                    <LocationName>
                                        <Text>Dresden</Text>
                                        <Language>de</Language>
                                    </LocationName>
                                    <GeoPosition>
                                        <Longitude>13.72663</Longitude>
                                        <Latitude>51.05694</Latitude>
                                    </GeoPosition>
                                </Location>
                                <Complete>true</Complete>
                                <Probability>1</Probability>
                            </LocationResult>
                        </LocationInformationResponse>
                    </DeliveryPayload>
                </ServiceDelivery>
            </Trias>";

            _mockHttpMessageHandler.When("*").Respond("text/xml", respondBody);

            Func<Task> act = async () => { await _communicator.LocationInformationStopRequest("de:14612:28").ConfigureAwait(false); };
            await act.Should().ThrowAsync<LocationInformationException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task LocationInformationStopRequest_WrongResponse()
        {
            var respondBody = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <Trias xmlns=""http://www.vdv.de/trias"" version=""1.2"">
                <ServiceDelivery>
                    <ResponseTimestamp xmlns=""http://www.siri.org.uk/siri"">2020-07-19T11:47:11Z</ResponseTimestamp>
                    <ProducerRef xmlns=""http://www.siri.org.uk/siri"">EFAController10.3.11.22-EFA1</ProducerRef>
                    <Status xmlns=""http://www.siri.org.uk/siri"">true</Status>
                    <MoreData>false</MoreData>
                    <Language>de</Language>
                    <CalcTime>58</CalcTime>
                    <DeliveryPayload>
                        <LocationInformationResponse>
                            <LocationResult>
                                <Location>
                                    <StopPoint>
                                        <StopPointRef>de:14612:14</StopPointRef>
                                        <StopPointName>
                                            <Text>Dresden, Hauptbahnhof</Text>
                                            <Language>de</Language>
                                        </StopPointName>
                                        <LocalityRef>14612000:1</LocalityRef>
                                        <WheelchairAccessible>false</WheelchairAccessible>
                                        <Lighting>false</Lighting>
                                        <Covered>false</Covered>
                                    </StopPoint>
                                    <LocationName>
                                        <Text>Dresden</Text>
                                        <Language>de</Language>
                                    </LocationName>
                                    <GeoPosition>
                                        <Longitude>13.73293</Longitude>
                                        <Latitude>51.03993</Latitude>
                                    </GeoPosition>
                                </Location>
                                <Complete>true</Complete>
                                <Probability>0</Probability>
                            </LocationResult>
                        </LocationInformationResponse>
                    </DeliveryPayload>
                </ServiceDelivery>
            </Trias>";

            _mockHttpMessageHandler.When("*").Respond("text/xml", respondBody);

            Func<Task> act = async () => { await _communicator.LocationInformationStopRequest("de:14612:28").ConfigureAwait(false); };
            await act.Should().ThrowAsync<LocationInformationException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task LocationInformationStopRequest_NoResponse()
        {
            var respondBody = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <Trias xmlns=""http://www.vdv.de/trias"" version=""1.2"">
                <ServiceDelivery>
                    <ResponseTimestamp xmlns=""http://www.siri.org.uk/siri"">2020-07-19T13:06:43Z</ResponseTimestamp>
                    <ProducerRef xmlns=""http://www.siri.org.uk/siri"">EFAController10.3.11.22-EFA1</ProducerRef>
                    <Status xmlns=""http://www.siri.org.uk/siri"">true</Status>
                    <MoreData>false</MoreData>
                    <Language>de</Language>
                    <CalcTime>33</CalcTime>
                    <DeliveryPayload>
                        <LocationInformationResponse>
                            <ErrorMessage>
                                <Code>-8020</Code>
                                <Text>
                                    <Text>LOCATION_NORESULTS</Text>
                                    <Language>de</Language>
                                </Text>
                            </ErrorMessage>
                        </LocationInformationResponse>
                    </DeliveryPayload>
                </ServiceDelivery>
            </Trias>";

            _mockHttpMessageHandler.When("*").Respond("text/xml", respondBody);

            Func<Task> act = async () => { await _communicator.LocationInformationStopRequest("de:14612:28").ConfigureAwait(false); };
            await act.Should().ThrowAsync<LocationInformationException>().ConfigureAwait(false);
        }
    }
}
