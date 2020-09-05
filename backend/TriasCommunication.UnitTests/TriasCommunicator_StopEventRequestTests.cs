using DerMistkaefer.DvbLive.TriasCommunication.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using DateTime = System.DateTime;

namespace DerMistkaefer.DvbLive.TriasCommunication.UnitTests
{
    /// <summary>
    /// Tests for the Trias Communicator - Function StopEventRequest
    /// </summary>
    public sealed class TriasCommunicator_StopEventRequestTests : IDisposable
    {
        private readonly MockHttpMessageHandler _mockHttpMessageHandler;
        private readonly TriasCommunicator _communicator;

        /// <summary>
        /// Initalize the Trias Communicator
        /// </summary>
        public TriasCommunicator_StopEventRequestTests()
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(_mockHttpMessageHandler.ToHttpClient());

            _communicator = new TriasCommunicator(mockHttpClientFactory.Object, NullLogger<TriasCommunicator>.Instance);
        }

        public void Dispose()
        {
            _mockHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task StopEventRequest_DresdenMainStation()
        {
            var respondBody = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <Trias xmlns=""http://www.vdv.de/trias"" version=""1.2"">
                <ServiceDelivery>
                    <ResponseTimestamp xmlns=""http://www.siri.org.uk/siri"">2020-07-19T13:06:43Z</ResponseTimestamp>
                    <ProducerRef xmlns=""http://www.siri.org.uk/siri"">EFAController10.3.12.4-EFA1</ProducerRef>
                    <Status xmlns=""http://www.siri.org.uk/siri"">true</Status>
                    <MoreData>false</MoreData>
                    <Language>de</Language>
                    <CalcTime>135</CalcTime>
                    <DeliveryPayload>
                        <StopEventResponse>
                            <StopEventResponseContext>
                                <Situations>
                                    <PtSituation>
                                        <CreationTime xmlns=""http://www.siri.org.uk/siri"">2020-08-31T08:16:00Z</CreationTime>
                                        <ParticipantRef xmlns=""http://www.siri.org.uk/siri""></ParticipantRef>
                                        <SituationNumber xmlns=""http://www.siri.org.uk/siri"">9792</SituationNumber>
                                        <Version xmlns=""http://www.siri.org.uk/siri"">115640162</Version>
                                        <Source xmlns=""http://www.siri.org.uk/siri"">
                                            <SourceType>other</SourceType>
                                        </Source>
                                        <Progress xmlns=""http://www.siri.org.uk/siri"">open</Progress>
                                        <ValidityPeriod xmlns=""http://www.siri.org.uk/siri"">
                                            <StartTime>2020-08-30T22:00:00Z</StartTime>
                                            <EndTime>2500-12-30T23:00:00Z</EndTime>
                                        </ValidityPeriod>
                                        <UnknownReason xmlns=""http://www.siri.org.uk/siri"">unknown</UnknownReason>
                                        <Priority xmlns=""http://www.siri.org.uk/siri"">3</Priority>
                                        <Audience xmlns=""http://www.siri.org.uk/siri"">public</Audience>
                                        <ScopeType xmlns=""http://www.siri.org.uk/siri"">line</ScopeType>
                                        <Planned xmlns=""http://www.siri.org.uk/siri"">false</Planned>
                                        <Language xmlns=""http://www.siri.org.uk/siri""></Language>
                                        <Summary xmlns=""http://www.siri.org.uk/siri"" overridden=""true"">Linienaenderung</Summary>
                                        <Description xmlns=""http://www.siri.org.uk/siri"" overridden=""true"">Dresden - Änderungen zum Schuljahresbeginn 2020/2021</Description>
                                        <Detail xmlns=""http://www.siri.org.uk/siri"" overridden=""true"">&lt;div style=""DISPLAY: none""&gt;Änderungen Schuljahresbeginn&lt;/div&gt; &lt;h2&gt;Beschreibung&lt;/h2&gt; &lt;p&gt;Ab dem 31. August 2020 fahren fast alle Linien wieder nach Standardfahrplan inklusive der Zusatzfahrten im Schülerverkehr. &lt;br&gt;&lt;br&gt;&lt;strong&gt;Bitte beachten Sie:&lt;/strong&gt;&lt;/p&gt; &lt;p&gt;&lt;b&gt;Buslinie 61&lt;/b&gt;: &lt;br&gt;Montag bis Freitag wieder nach Standardfahrplan; Samstag weiterhin nur im 15-Minuten-Takt.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 74&lt;/b&gt;:&lt;br&gt;Zusatzfahrten zwischen Jägerpark und Waldschlösschen im 30-Minuten-Takt.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 75&lt;/b&gt;:&lt;br&gt; Schulfahrt ab Cossebaude verkehrt nur bis Ockerwitzer Straße, anschließend weiter nach Omsewitz.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 78&lt;/b&gt;:&lt;font color=""#ff0000""&gt;&lt;strong&gt; &lt;font color=""#ff0000""&gt;neu&lt;/font&gt;&lt;/strong&gt; &lt;/font&gt;&lt;br&gt;vom Bahnhof Klotzsche zum Gewerbegebiet Airportpark, &lt;br&gt;Montag bis Freitag im 30-Minuten-Takt&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 80&lt;/b&gt;:&lt;br&gt; neue Schulfahrt von Ockerwitzer Straße nach Omsewitz (als Line 75).&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 81&lt;/b&gt;:&lt;br&gt; jetzt samstags, sonn- und feiertags im 30-Minuten-Takt.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 88&lt;/b&gt;:&lt;font color=""#ff0000""&gt;&lt;strong&gt; &lt;font color=""#ff0000""&gt;neu&lt;/font&gt;&lt;/strong&gt;&lt;/font&gt;&lt;br&gt;Verlängerung von Kauscha nach Goppeln.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 90&lt;/b&gt;: &lt;br&gt;Zusatzfahrten im Schülerverkehr von Dölzschen nach Löbtau.&lt;br&gt;&lt;br&gt;&lt;b&gt;Straßenbahnlinie 11 und Ersatzverkehr EV11&lt;/b&gt;:&lt;br&gt; verkehren Montag bis Freitag in der Hautpverkehrszeit im 8-Minuten-Takt.&lt;br&gt;&lt;br&gt;Die Zusatzfahrten der Straßenbahnlinien E7 und E12 entfallen weiterhin.&lt;/p&gt;&lt;h2&gt;Haltestellenanpassungen&lt;/h2&gt; &lt;ul&gt; &lt;li&gt;Neue Haltestelle &lt;b&gt;Gewerbegebiet Airportpark&lt;/b&gt; für die Buslinie 78. &lt;/li&gt;&lt;li&gt;Neue Haltestelle &lt;b&gt;An der Bartlake&lt;/b&gt; zwischen Wilschdorfer Landstraße und Rähnitzer Allee für die Buslinie 80.&lt;/li&gt;&lt;li&gt;Neue Haltestelle &lt;b&gt;Alnpeckstraße&lt;/b&gt; zwischen Fritz-Meinhardt-Straße und Altnickern für die Buslinie 88.&lt;/li&gt;&lt;/ul&gt; inEFA=""true"";MeldungTyp=""Information""</Detail>
                                    </PtSituation>
                                </Situations>
                            </StopEventResponseContext>
                            <StopEventResult>
                                <ResultId>ID-9E2FD9B0-9B9A-4676-A01C-B7C83F65DDA7</ResultId>
                                <StopEvent>
                                    <PreviousCall>
                                        <CallAtStop>
                                            <StopPointRef>de:14612:253:2:3</StopPointRef>
                                            <StopPointName>
                                                <Text>Dresden Wilder Mann</Text>
                                                <Language>de</Language>
                                            </StopPointName>
                                            <PlannedBay>
                                                <Text>3</Text>
                                                <Language>de</Language>
                                            </PlannedBay>
                                            <ServiceDeparture>
                                                <TimetabledTime>2020-08-31T13:07:00Z</TimetabledTime>
                                            </ServiceDeparture>
                                            <StopSeqNumber>1</StopSeqNumber>
                                            <DemandStop>false</DemandStop>
                                            <UnplannedStop>false</UnplannedStop>
                                            <NotServicedStop>false</NotServicedStop>
                                            <NoBoardingAtStop>false</NoBoardingAtStop>
                                            <NoAlightingAtStop>false</NoAlightingAtStop>
                                        </CallAtStop>
                                    </PreviousCall>
                                    <ThisCall>
                                        <CallAtStop>
                                            <StopPointRef>de:14612:28:2:4</StopPointRef>
                                            <StopPointName>
                                                <Text>Dresden Hauptbahnhof</Text>
                                                <Language>de</Language>
                                            </StopPointName>
                                            <PlannedBay>
                                                <Text>4</Text>
                                                <Language>de</Language>
                                            </PlannedBay>
                                            <ServiceArrival>
                                                <TimetabledTime>2020-08-31T13:31:00Z</TimetabledTime>
                                                <EstimatedTime>2020-08-31T13:35:00Z</EstimatedTime>
                                            </ServiceArrival>
                                            <ServiceDeparture>
                                                <TimetabledTime>2020-08-31T13:33:00Z</TimetabledTime>
                                                <EstimatedTime>2020-08-31T13:36:00Z</EstimatedTime>
                                            </ServiceDeparture>
                                            <StopSeqNumber>2</StopSeqNumber>
                                            <DemandStop>false</DemandStop>
                                            <UnplannedStop>false</UnplannedStop>
                                            <NotServicedStop>false</NotServicedStop>
                                            <NoBoardingAtStop>false</NoBoardingAtStop>
                                            <NoAlightingAtStop>false</NoAlightingAtStop>
                                        </CallAtStop>
                                    </ThisCall>
                                    <OnwardCall>
                                        <CallAtStop>
                                            <StopPointRef>de:14612:165:1:1</StopPointRef>
                                            <StopPointName>
                                                <Text>Dresden Coschütz</Text>
                                                <Language>de</Language>
                                            </StopPointName>
                                            <PlannedBay>
                                                <Text>1</Text>
                                                <Language>de</Language>
                                            </PlannedBay>
                                            <ServiceArrival>
                                                <TimetabledTime>2020-08-31T13:45:00Z</TimetabledTime>
                                            </ServiceArrival>
                                            <StopSeqNumber>3</StopSeqNumber>
                                            <DemandStop>false</DemandStop>
                                            <UnplannedStop>false</UnplannedStop>
                                            <NotServicedStop>false</NotServicedStop>
                                            <NoBoardingAtStop>false</NoBoardingAtStop>
                                            <NoAlightingAtStop>false</NoAlightingAtStop>
                                        </CallAtStop>
                                    </OnwardCall>
                                    <Service>
                                        <OperatingDayRef>2020-08-31T</OperatingDayRef>
                                        <JourneyRef>voe:11008::H:j20:397</JourneyRef>
                                        <ServiceSection>
                                            <LineRef>voe:11008::H</LineRef>
                                            <DirectionRef>outward</DirectionRef>
                                            <Mode>
                                                <PtMode>tram</PtMode>
                                                <TramSubmode>cityTram</TramSubmode>
                                                <Name>
                                                    <Text>Straßenbahn</Text>
                                                    <Language>de</Language>
                                                </Name>
                                            </Mode>
                                            <PublishedLineName>
                                                <Text>8</Text>
                                                <Language>de</Language>
                                            </PublishedLineName>
                                            <OperatorRef>voe:16</OperatorRef>
                                            <RouteDescription>
                                                <Text>Hellerau - Südvorstadt</Text>
                                                <Language>de</Language>
                                            </RouteDescription>
                                        </ServiceSection>
                                        <OriginStopPointRef>de:14612:275:1:1</OriginStopPointRef>
                                        <OriginText>
                                            <Text>Dresden Hellerau Kiefernweg</Text>
                                            <Language>de</Language>
                                        </OriginText>
                                        <DestinationStopPointRef>de:14612:133:1:2</DestinationStopPointRef>
                                        <DestinationText>
                                            <Text>Südvorstadt</Text>
                                            <Language>de</Language>
                                        </DestinationText>
                                        <Unplanned>false</Unplanned>
                                        <Cancelled>false</Cancelled>
                                        <Deviation>false</Deviation>
                                        <SituationFullRef>
                                            <ParticipantRef xmlns=""http://www.siri.org.uk/siri""></ParticipantRef>
                                            <SituationNumber xmlns=""http://www.siri.org.uk/siri"">9792</SituationNumber>
                                        </SituationFullRef>
                                    </Service>
                                </StopEvent>
                            </StopEventResult>
                        </StopEventResponse>
                    </DeliveryPayload>
                </ServiceDelivery>
            </Trias>";

            _mockHttpMessageHandler.When("*").Respond("text/xml", respondBody);

            var response = await _communicator.StopEventRequest("de:14612:28").ConfigureAwait(false);

            var shouldStop1 = new StopEventCall("de:14612:253:2:3", "Dresden Wilder Mann", 1, "3",
                null, null,
                new DateTime(2020, 08, 31, 13, 07, 0, DateTimeKind.Utc), null, CallType.Previous);
            var shouldStop2 = new StopEventCall("de:14612:28:2:4", "Dresden Hauptbahnhof", 2, "4",
                new DateTime(2020, 08, 31, 13, 31, 0, DateTimeKind.Utc), new DateTime(2020, 08, 31, 13, 35, 0, DateTimeKind.Utc),
                new DateTime(2020, 08, 31, 13, 33, 0, DateTimeKind.Utc), new DateTime(2020, 08, 31, 13, 36, 0, DateTimeKind.Utc), CallType.This);
            var shouldStop3 = new StopEventCall("de:14612:165:1:1", "Dresden Coschütz", 3, "1",
                new DateTime(2020, 08, 31, 13, 45, 0, DateTimeKind.Utc), null,
                null, null, CallType.Onward);
            var shouldStops = new List<StopEventCall> { shouldStop1, shouldStop2, shouldStop3 };
            var shouldEvents = new StopEventResult(new DateTime(2020, 08, 31), "voe:11008::H:j20:397", "voe:11008::H",
                "Straßenbahn", "8", "voe:16", "Hellerau - Südvorstadt",
                "de:14612:275:1:1", "de:14612:133:1:2", shouldStops);
            var shouldResponse = new StopEventResponse("de:14612:28", new List<StopEventResult> { shouldEvents });
            response.Should().BeEquivalentTo(shouldResponse);
        }

        [Fact]
        public async Task StopEventRequest_ErrorResponse()
        {
            var respondBody = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <Trias xmlns=""http://www.vdv.de/trias"" version=""1.2"">
                    <ServiceDelivery>
                        <ResponseTimestamp xmlns=""http://www.siri.org.uk/siri"">2020-09-03T09:14:53Z</ResponseTimestamp>
                        <ProducerRef xmlns=""http://www.siri.org.uk/siri"">EFAController10.3.13.9-EFA1</ProducerRef>
                        <Status xmlns=""http://www.siri.org.uk/siri"">true</Status>
                        <MoreData>false</MoreData>
                        <Language>de</Language>
                        <CalcTime>521</CalcTime>
                        <DeliveryPayload>
                            <StopEventResponse>
                                <ErrorMessage>
                                    <Code>-4030</Code>
                                    <Text>
                                        <Text>STOPEVENT_LOCATIONUNSERVED</Text>
                                        <Language>de</Language>
                                    </Text>
                                </ErrorMessage>
                                <StopEventResponseContext>
                                    <Situations></Situations>
                                </StopEventResponseContext>
                            </StopEventResponse>
                        </DeliveryPayload>
                    </ServiceDelivery>
                </Trias>";

            _mockHttpMessageHandler.When("*").Respond("text/xml", respondBody);

            var response = await _communicator.StopEventRequest("de:14612:281456").ConfigureAwait(false);

            var shouldResponse = new StopEventResponse("de:14612:281456", new List<StopEventResult>());
            response.Should().BeEquivalentTo(shouldResponse);
        }

        [Fact]
        public async Task StopEventRequest_OnlyThisCall()
        {
            var respondBody = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <Trias xmlns=""http://www.vdv.de/trias"" version=""1.2"">
                <ServiceDelivery>
                    <ResponseTimestamp xmlns=""http://www.siri.org.uk/siri"">2020-07-19T13:06:43Z</ResponseTimestamp>
                    <ProducerRef xmlns=""http://www.siri.org.uk/siri"">EFAController10.3.12.4-EFA1</ProducerRef>
                    <Status xmlns=""http://www.siri.org.uk/siri"">true</Status>
                    <MoreData>false</MoreData>
                    <Language>de</Language>
                    <CalcTime>135</CalcTime>
                    <DeliveryPayload>
                        <StopEventResponse>
                            <StopEventResponseContext>
                                <Situations>
                                    <PtSituation>
                                        <CreationTime xmlns=""http://www.siri.org.uk/siri"">2020-08-31T08:16:00Z</CreationTime>
                                        <ParticipantRef xmlns=""http://www.siri.org.uk/siri""></ParticipantRef>
                                        <SituationNumber xmlns=""http://www.siri.org.uk/siri"">9792</SituationNumber>
                                        <Version xmlns=""http://www.siri.org.uk/siri"">115640162</Version>
                                        <Source xmlns=""http://www.siri.org.uk/siri"">
                                            <SourceType>other</SourceType>
                                        </Source>
                                        <Progress xmlns=""http://www.siri.org.uk/siri"">open</Progress>
                                        <ValidityPeriod xmlns=""http://www.siri.org.uk/siri"">
                                            <StartTime>2020-08-30T22:00:00Z</StartTime>
                                            <EndTime>2500-12-30T23:00:00Z</EndTime>
                                        </ValidityPeriod>
                                        <UnknownReason xmlns=""http://www.siri.org.uk/siri"">unknown</UnknownReason>
                                        <Priority xmlns=""http://www.siri.org.uk/siri"">3</Priority>
                                        <Audience xmlns=""http://www.siri.org.uk/siri"">public</Audience>
                                        <ScopeType xmlns=""http://www.siri.org.uk/siri"">line</ScopeType>
                                        <Planned xmlns=""http://www.siri.org.uk/siri"">false</Planned>
                                        <Language xmlns=""http://www.siri.org.uk/siri""></Language>
                                        <Summary xmlns=""http://www.siri.org.uk/siri"" overridden=""true"">Linienaenderung</Summary>
                                        <Description xmlns=""http://www.siri.org.uk/siri"" overridden=""true"">Dresden - Änderungen zum Schuljahresbeginn 2020/2021</Description>
                                        <Detail xmlns=""http://www.siri.org.uk/siri"" overridden=""true"">&lt;div style=""DISPLAY: none""&gt;Änderungen Schuljahresbeginn&lt;/div&gt; &lt;h2&gt;Beschreibung&lt;/h2&gt; &lt;p&gt;Ab dem 31. August 2020 fahren fast alle Linien wieder nach Standardfahrplan inklusive der Zusatzfahrten im Schülerverkehr. &lt;br&gt;&lt;br&gt;&lt;strong&gt;Bitte beachten Sie:&lt;/strong&gt;&lt;/p&gt; &lt;p&gt;&lt;b&gt;Buslinie 61&lt;/b&gt;: &lt;br&gt;Montag bis Freitag wieder nach Standardfahrplan; Samstag weiterhin nur im 15-Minuten-Takt.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 74&lt;/b&gt;:&lt;br&gt;Zusatzfahrten zwischen Jägerpark und Waldschlösschen im 30-Minuten-Takt.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 75&lt;/b&gt;:&lt;br&gt; Schulfahrt ab Cossebaude verkehrt nur bis Ockerwitzer Straße, anschließend weiter nach Omsewitz.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 78&lt;/b&gt;:&lt;font color=""#ff0000""&gt;&lt;strong&gt; &lt;font color=""#ff0000""&gt;neu&lt;/font&gt;&lt;/strong&gt; &lt;/font&gt;&lt;br&gt;vom Bahnhof Klotzsche zum Gewerbegebiet Airportpark, &lt;br&gt;Montag bis Freitag im 30-Minuten-Takt&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 80&lt;/b&gt;:&lt;br&gt; neue Schulfahrt von Ockerwitzer Straße nach Omsewitz (als Line 75).&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 81&lt;/b&gt;:&lt;br&gt; jetzt samstags, sonn- und feiertags im 30-Minuten-Takt.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 88&lt;/b&gt;:&lt;font color=""#ff0000""&gt;&lt;strong&gt; &lt;font color=""#ff0000""&gt;neu&lt;/font&gt;&lt;/strong&gt;&lt;/font&gt;&lt;br&gt;Verlängerung von Kauscha nach Goppeln.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 90&lt;/b&gt;: &lt;br&gt;Zusatzfahrten im Schülerverkehr von Dölzschen nach Löbtau.&lt;br&gt;&lt;br&gt;&lt;b&gt;Straßenbahnlinie 11 und Ersatzverkehr EV11&lt;/b&gt;:&lt;br&gt; verkehren Montag bis Freitag in der Hautpverkehrszeit im 8-Minuten-Takt.&lt;br&gt;&lt;br&gt;Die Zusatzfahrten der Straßenbahnlinien E7 und E12 entfallen weiterhin.&lt;/p&gt;&lt;h2&gt;Haltestellenanpassungen&lt;/h2&gt; &lt;ul&gt; &lt;li&gt;Neue Haltestelle &lt;b&gt;Gewerbegebiet Airportpark&lt;/b&gt; für die Buslinie 78. &lt;/li&gt;&lt;li&gt;Neue Haltestelle &lt;b&gt;An der Bartlake&lt;/b&gt; zwischen Wilschdorfer Landstraße und Rähnitzer Allee für die Buslinie 80.&lt;/li&gt;&lt;li&gt;Neue Haltestelle &lt;b&gt;Alnpeckstraße&lt;/b&gt; zwischen Fritz-Meinhardt-Straße und Altnickern für die Buslinie 88.&lt;/li&gt;&lt;/ul&gt; inEFA=""true"";MeldungTyp=""Information""</Detail>
                                    </PtSituation>
                                </Situations>
                            </StopEventResponseContext>
                            <StopEventResult>
                                <ResultId>ID-9E2FD9B0-9B9A-4676-A01C-B7C83F65DDA7</ResultId>
                                <StopEvent>
                                    <ThisCall>
                                        <CallAtStop>
                                            <StopPointRef>de:14612:28:2:4</StopPointRef>
                                            <StopPointName>
                                                <Text>Dresden Hauptbahnhof</Text>
                                                <Language>de</Language>
                                            </StopPointName>
                                            <PlannedBay>
                                                <Text>4</Text>
                                                <Language>de</Language>
                                            </PlannedBay>
                                            <ServiceArrival>
                                                <TimetabledTime>2020-08-31T13:31:00Z</TimetabledTime>
                                                <EstimatedTime>2020-08-31T13:35:00Z</EstimatedTime>
                                            </ServiceArrival>
                                            <ServiceDeparture>
                                                <TimetabledTime>2020-08-31T13:33:00Z</TimetabledTime>
                                                <EstimatedTime>2020-08-31T13:36:00Z</EstimatedTime>
                                            </ServiceDeparture>
                                            <StopSeqNumber>2</StopSeqNumber>
                                            <DemandStop>false</DemandStop>
                                            <UnplannedStop>false</UnplannedStop>
                                            <NotServicedStop>false</NotServicedStop>
                                            <NoBoardingAtStop>false</NoBoardingAtStop>
                                            <NoAlightingAtStop>false</NoAlightingAtStop>
                                        </CallAtStop>
                                    </ThisCall>
                                    <Service>
                                        <OperatingDayRef>2020-08-31T</OperatingDayRef>
                                        <JourneyRef>voe:11008::H:j20:397</JourneyRef>
                                        <ServiceSection>
                                            <LineRef>voe:11008::H</LineRef>
                                            <DirectionRef>outward</DirectionRef>
                                            <Mode>
                                                <PtMode>tram</PtMode>
                                                <TramSubmode>cityTram</TramSubmode>
                                                <Name>
                                                    <Text>Straßenbahn</Text>
                                                    <Language>de</Language>
                                                </Name>
                                            </Mode>
                                            <PublishedLineName>
                                                <Text>8</Text>
                                                <Language>de</Language>
                                            </PublishedLineName>
                                            <OperatorRef>voe:16</OperatorRef>
                                            <RouteDescription>
                                                <Text>Hellerau - Südvorstadt</Text>
                                                <Language>de</Language>
                                            </RouteDescription>
                                        </ServiceSection>
                                        <OriginStopPointRef>de:14612:275:1:1</OriginStopPointRef>
                                        <OriginText>
                                            <Text>Dresden Hellerau Kiefernweg</Text>
                                            <Language>de</Language>
                                        </OriginText>
                                        <DestinationStopPointRef>de:14612:133:1:2</DestinationStopPointRef>
                                        <DestinationText>
                                            <Text>Südvorstadt</Text>
                                            <Language>de</Language>
                                        </DestinationText>
                                        <Unplanned>false</Unplanned>
                                        <Cancelled>false</Cancelled>
                                        <Deviation>false</Deviation>
                                        <SituationFullRef>
                                            <ParticipantRef xmlns=""http://www.siri.org.uk/siri""></ParticipantRef>
                                            <SituationNumber xmlns=""http://www.siri.org.uk/siri"">9792</SituationNumber>
                                        </SituationFullRef>
                                    </Service>
                                </StopEvent>
                            </StopEventResult>
                        </StopEventResponse>
                    </DeliveryPayload>
                </ServiceDelivery>
            </Trias>";

            _mockHttpMessageHandler.When("*").Respond("text/xml", respondBody);

            await _communicator.StopEventRequest("de:14612:281456").ConfigureAwait(false);
        }

        [Fact]
        public async Task StopEventRequest_PlannedBayMissing()
        {
            var respondBody = @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <Trias xmlns=""http://www.vdv.de/trias"" version=""1.2"">
                <ServiceDelivery>
                    <ResponseTimestamp xmlns=""http://www.siri.org.uk/siri"">2020-07-19T13:06:43Z</ResponseTimestamp>
                    <ProducerRef xmlns=""http://www.siri.org.uk/siri"">EFAController10.3.12.4-EFA1</ProducerRef>
                    <Status xmlns=""http://www.siri.org.uk/siri"">true</Status>
                    <MoreData>false</MoreData>
                    <Language>de</Language>
                    <CalcTime>135</CalcTime>
                    <DeliveryPayload>
                        <StopEventResponse>
                            <StopEventResponseContext>
                                <Situations>
                                    <PtSituation>
                                        <CreationTime xmlns=""http://www.siri.org.uk/siri"">2020-08-31T08:16:00Z</CreationTime>
                                        <ParticipantRef xmlns=""http://www.siri.org.uk/siri""></ParticipantRef>
                                        <SituationNumber xmlns=""http://www.siri.org.uk/siri"">9792</SituationNumber>
                                        <Version xmlns=""http://www.siri.org.uk/siri"">115640162</Version>
                                        <Source xmlns=""http://www.siri.org.uk/siri"">
                                            <SourceType>other</SourceType>
                                        </Source>
                                        <Progress xmlns=""http://www.siri.org.uk/siri"">open</Progress>
                                        <ValidityPeriod xmlns=""http://www.siri.org.uk/siri"">
                                            <StartTime>2020-08-30T22:00:00Z</StartTime>
                                            <EndTime>2500-12-30T23:00:00Z</EndTime>
                                        </ValidityPeriod>
                                        <UnknownReason xmlns=""http://www.siri.org.uk/siri"">unknown</UnknownReason>
                                        <Priority xmlns=""http://www.siri.org.uk/siri"">3</Priority>
                                        <Audience xmlns=""http://www.siri.org.uk/siri"">public</Audience>
                                        <ScopeType xmlns=""http://www.siri.org.uk/siri"">line</ScopeType>
                                        <Planned xmlns=""http://www.siri.org.uk/siri"">false</Planned>
                                        <Language xmlns=""http://www.siri.org.uk/siri""></Language>
                                        <Summary xmlns=""http://www.siri.org.uk/siri"" overridden=""true"">Linienaenderung</Summary>
                                        <Description xmlns=""http://www.siri.org.uk/siri"" overridden=""true"">Dresden - Änderungen zum Schuljahresbeginn 2020/2021</Description>
                                        <Detail xmlns=""http://www.siri.org.uk/siri"" overridden=""true"">&lt;div style=""DISPLAY: none""&gt;Änderungen Schuljahresbeginn&lt;/div&gt; &lt;h2&gt;Beschreibung&lt;/h2&gt; &lt;p&gt;Ab dem 31. August 2020 fahren fast alle Linien wieder nach Standardfahrplan inklusive der Zusatzfahrten im Schülerverkehr. &lt;br&gt;&lt;br&gt;&lt;strong&gt;Bitte beachten Sie:&lt;/strong&gt;&lt;/p&gt; &lt;p&gt;&lt;b&gt;Buslinie 61&lt;/b&gt;: &lt;br&gt;Montag bis Freitag wieder nach Standardfahrplan; Samstag weiterhin nur im 15-Minuten-Takt.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 74&lt;/b&gt;:&lt;br&gt;Zusatzfahrten zwischen Jägerpark und Waldschlösschen im 30-Minuten-Takt.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 75&lt;/b&gt;:&lt;br&gt; Schulfahrt ab Cossebaude verkehrt nur bis Ockerwitzer Straße, anschließend weiter nach Omsewitz.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 78&lt;/b&gt;:&lt;font color=""#ff0000""&gt;&lt;strong&gt; &lt;font color=""#ff0000""&gt;neu&lt;/font&gt;&lt;/strong&gt; &lt;/font&gt;&lt;br&gt;vom Bahnhof Klotzsche zum Gewerbegebiet Airportpark, &lt;br&gt;Montag bis Freitag im 30-Minuten-Takt&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 80&lt;/b&gt;:&lt;br&gt; neue Schulfahrt von Ockerwitzer Straße nach Omsewitz (als Line 75).&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 81&lt;/b&gt;:&lt;br&gt; jetzt samstags, sonn- und feiertags im 30-Minuten-Takt.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 88&lt;/b&gt;:&lt;font color=""#ff0000""&gt;&lt;strong&gt; &lt;font color=""#ff0000""&gt;neu&lt;/font&gt;&lt;/strong&gt;&lt;/font&gt;&lt;br&gt;Verlängerung von Kauscha nach Goppeln.&lt;br&gt;&lt;br&gt;&lt;b&gt;Buslinie 90&lt;/b&gt;: &lt;br&gt;Zusatzfahrten im Schülerverkehr von Dölzschen nach Löbtau.&lt;br&gt;&lt;br&gt;&lt;b&gt;Straßenbahnlinie 11 und Ersatzverkehr EV11&lt;/b&gt;:&lt;br&gt; verkehren Montag bis Freitag in der Hautpverkehrszeit im 8-Minuten-Takt.&lt;br&gt;&lt;br&gt;Die Zusatzfahrten der Straßenbahnlinien E7 und E12 entfallen weiterhin.&lt;/p&gt;&lt;h2&gt;Haltestellenanpassungen&lt;/h2&gt; &lt;ul&gt; &lt;li&gt;Neue Haltestelle &lt;b&gt;Gewerbegebiet Airportpark&lt;/b&gt; für die Buslinie 78. &lt;/li&gt;&lt;li&gt;Neue Haltestelle &lt;b&gt;An der Bartlake&lt;/b&gt; zwischen Wilschdorfer Landstraße und Rähnitzer Allee für die Buslinie 80.&lt;/li&gt;&lt;li&gt;Neue Haltestelle &lt;b&gt;Alnpeckstraße&lt;/b&gt; zwischen Fritz-Meinhardt-Straße und Altnickern für die Buslinie 88.&lt;/li&gt;&lt;/ul&gt; inEFA=""true"";MeldungTyp=""Information""</Detail>
                                    </PtSituation>
                                </Situations>
                            </StopEventResponseContext>
                            <StopEventResult>
                                <ResultId>ID-9E2FD9B0-9B9A-4676-A01C-B7C83F65DDA7</ResultId>
                                <StopEvent>
                                    <PreviousCall>
                                        <CallAtStop>
                                            <StopPointRef>de:14612:253:2:3</StopPointRef>
                                            <StopPointName>
                                                <Text>Dresden Wilder Mann</Text>
                                                <Language>de</Language>
                                            </StopPointName>
                                            <ServiceDeparture>
                                                <TimetabledTime>2020-08-31T13:07:00Z</TimetabledTime>
                                            </ServiceDeparture>
                                            <StopSeqNumber>1</StopSeqNumber>
                                            <DemandStop>false</DemandStop>
                                            <UnplannedStop>false</UnplannedStop>
                                            <NotServicedStop>false</NotServicedStop>
                                            <NoBoardingAtStop>false</NoBoardingAtStop>
                                            <NoAlightingAtStop>false</NoAlightingAtStop>
                                        </CallAtStop>
                                    </PreviousCall>
                                    <ThisCall>
                                        <CallAtStop>
                                            <StopPointRef>de:14612:28:2:4</StopPointRef>
                                            <StopPointName>
                                                <Text>Dresden Hauptbahnhof</Text>
                                                <Language>de</Language>
                                            </StopPointName>
                                            <ServiceArrival>
                                                <TimetabledTime>2020-08-31T13:31:00Z</TimetabledTime>
                                                <EstimatedTime>2020-08-31T13:35:00Z</EstimatedTime>
                                            </ServiceArrival>
                                            <ServiceDeparture>
                                                <TimetabledTime>2020-08-31T13:33:00Z</TimetabledTime>
                                                <EstimatedTime>2020-08-31T13:36:00Z</EstimatedTime>
                                            </ServiceDeparture>
                                            <StopSeqNumber>2</StopSeqNumber>
                                            <DemandStop>false</DemandStop>
                                            <UnplannedStop>false</UnplannedStop>
                                            <NotServicedStop>false</NotServicedStop>
                                            <NoBoardingAtStop>false</NoBoardingAtStop>
                                            <NoAlightingAtStop>false</NoAlightingAtStop>
                                        </CallAtStop>
                                    </ThisCall>
                                    <OnwardCall>
                                        <CallAtStop>
                                            <StopPointRef>de:14612:165:1:1</StopPointRef>
                                            <StopPointName>
                                                <Text>Dresden Coschütz</Text>
                                                <Language>de</Language>
                                            </StopPointName>
                                            <ServiceArrival>
                                                <TimetabledTime>2020-08-31T13:45:00Z</TimetabledTime>
                                            </ServiceArrival>
                                            <StopSeqNumber>3</StopSeqNumber>
                                            <DemandStop>false</DemandStop>
                                            <UnplannedStop>false</UnplannedStop>
                                            <NotServicedStop>false</NotServicedStop>
                                            <NoBoardingAtStop>false</NoBoardingAtStop>
                                            <NoAlightingAtStop>false</NoAlightingAtStop>
                                        </CallAtStop>
                                    </OnwardCall>
                                    <Service>
                                        <OperatingDayRef>2020-08-31T</OperatingDayRef>
                                        <JourneyRef>voe:11008::H:j20:397</JourneyRef>
                                        <ServiceSection>
                                            <LineRef>voe:11008::H</LineRef>
                                            <DirectionRef>outward</DirectionRef>
                                            <Mode>
                                                <PtMode>tram</PtMode>
                                                <TramSubmode>cityTram</TramSubmode>
                                                <Name>
                                                    <Text>Straßenbahn</Text>
                                                    <Language>de</Language>
                                                </Name>
                                            </Mode>
                                            <PublishedLineName>
                                                <Text>8</Text>
                                                <Language>de</Language>
                                            </PublishedLineName>
                                            <OperatorRef>voe:16</OperatorRef>
                                            <RouteDescription>
                                                <Text>Hellerau - Südvorstadt</Text>
                                                <Language>de</Language>
                                            </RouteDescription>
                                        </ServiceSection>
                                        <OriginStopPointRef>de:14612:275:1:1</OriginStopPointRef>
                                        <OriginText>
                                            <Text>Dresden Hellerau Kiefernweg</Text>
                                            <Language>de</Language>
                                        </OriginText>
                                        <DestinationStopPointRef>de:14612:133:1:2</DestinationStopPointRef>
                                        <DestinationText>
                                            <Text>Südvorstadt</Text>
                                            <Language>de</Language>
                                        </DestinationText>
                                        <Unplanned>false</Unplanned>
                                        <Cancelled>false</Cancelled>
                                        <Deviation>false</Deviation>
                                        <SituationFullRef>
                                            <ParticipantRef xmlns=""http://www.siri.org.uk/siri""></ParticipantRef>
                                            <SituationNumber xmlns=""http://www.siri.org.uk/siri"">9792</SituationNumber>
                                        </SituationFullRef>
                                    </Service>
                                </StopEvent>
                            </StopEventResult>
                        </StopEventResponse>
                    </DeliveryPayload>
                </ServiceDelivery>
            </Trias>";

            _mockHttpMessageHandler.When("*").Respond("text/xml", respondBody);

            await _communicator.StopEventRequest("de:14612:281456").ConfigureAwait(false);
        }
    }
}
