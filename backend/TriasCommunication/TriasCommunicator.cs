using DerMistkaefer.DvbLive.TriasCommunication.Data;
using DerMistkaefer.DvbLive.TriasCommunication.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using vdo.trias;

namespace DerMistkaefer.DvbLive.TriasCommunication
{
    /// <summary>
    /// Communicator to the Trias-Api
    /// </summary>
    internal class TriasCommunicator : ITriasCommunicator
    {
        public int ApiRequestsCount { get; private set; }
        public long DownloadedBytes { get; private set; }

        private readonly ILogger<TriasCommunicator> _logger;
        private readonly HttpClient _httpClient;

        public TriasCommunicator(IHttpClientFactory httpClientFactory, ILogger<TriasCommunicator> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://efa.vvo-online.de:8080/std3/trias");
        }

        /// <inheritdoc cref="ITriasCommunicator" />
        public async Task<LocationInformationStopResponse> LocationInformationStopRequest(string idStopPoint)
        {
            var locationInformationRequest = new LocationInformationRequestStructure
            {
                Item = new LocationRefStructure() { Item = new StopPointRefStructure1 { Value = idStopPoint } },
                Restrictions = new LocationParamStructure() { Type = new[] { LocationTypeEnumeration.stop }, IncludePtModes = true }
            };

            var response = await BaseTriasCall<LocationInformationResponseStructure>(locationInformationRequest).ConfigureAwait(false);

            var locationResult = response.LocationResult?.FirstOrDefault();
            if (response.LocationResult?.Length != 1 || locationResult == null || ((StopPointStructure)locationResult.Location.Item).StopPointRef.Value != idStopPoint)
            {
                var errorCodes = response.ErrorMessage?.SelectMany(x => x.Text).Select(x => x.Text) ?? new List<string>();
                throw new LocationInformationException($"No location could be found. {string.Join('-', errorCodes)}");
            }

            var stopPoint = (StopPointStructure)locationResult.Location.Item;

            return new LocationInformationStopResponse()
            {
                IdStopPoint = stopPoint.StopPointRef.Value,
                StopPointName = stopPoint.StopPointName.FirstOrDefault(x => x.Language == "de")?.Text ?? "???",
                Latitude = locationResult.Location.GeoPosition.Latitude,
                Longitude = locationResult.Location.GeoPosition.Longitude
            };
        }

        public async Task TripRequest()
        {
            var tripRequest = new TripRequestStructure()
            {

            };

            var response = await BaseTriasCall<TripResponseStructure>(tripRequest).ConfigureAwait(false);
        }

        /// <inheritdoc cref="ITriasCommunicator" />
        public async Task<StopEventResponse> StopEventRequest(string idStopPoint)
        {
            var stopEventRequest = new StopEventRequestStructure
            {
                Location = new LocationContextStructure()
                {
                    Item = new LocationRefStructure { Item = new StopPointRefStructure1() { Value = idStopPoint } },
                    DepArrTime = System.DateTime.Now
                },
                Params = new StopEventParamStructure()
                {
                    StopEventType = StopEventTypeEnumeration.both,
                    IncludePreviousCalls = true,
                    IncludeOnwardCalls = true,
                    IncludeRealtimeData = true,
                }
            };

            var response = await BaseTriasCall<StopEventResponseStructure>(stopEventRequest).ConfigureAwait(false);

            if (response.ErrorMessage?.Length > 0)
            {
                var errorCodes = response.ErrorMessage?.SelectMany(x => x.Text).Select(x => x.Text) ?? new List<string>();
                var ex = new StopEventException($"No stop events could be collected. {string.Join('-', errorCodes)}");
                using (_logger.BeginScope(new Dictionary<string, object> { { "idStopPoint", idStopPoint }, { "response", response } }))
                {
                    _logger.LogError(ex, "{idStopPoint} - No stop events could be collected.", idStopPoint);
                }
                return new StopEventResponse(idStopPoint, new List<StopEventResult>());
            }

            return new StopEventResponse(response, idStopPoint);
        }

        private async Task<TType> BaseTriasCall<TType>(object requestPayload)
        {
            ApiRequestsCount++;
            var serviceRequest = new ServiceRequestStructure1
            {
                RequestTimestamp = System.DateTime.Now,
                RequestorRef = new ParticipantRefStructure() { Value = "OpenService" },
                RequestPayload = new RequestPayloadStructure() { Item = requestPayload }
            };
            var trias = new Trias { Item = serviceRequest };
            var text = XmlSerialisation(trias);

            using var content = new StringContent(text, Encoding.UTF8, "text/xml");
            var response = await _httpClient.PostAsync("", content).ConfigureAwait(false);
            DownloadedBytes += response.Content?.Headers?.ContentLength ?? 0;
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content!.ReadAsStreamAsync().ConfigureAwait(false);
            var responseTrias = XmlDeserialisation<Trias>(responseStream);
            var serviceDelievery = (ServiceDeliveryStructure1)responseTrias.Item;
            DeliveryPayloadStructure delevieryPayload = serviceDelievery.DeliveryPayload;

            return (TType)delevieryPayload.Item;
        }

        private static string XmlSerialisation(object data)
        {
            var xmlSerializer = new XmlSerializer(data.GetType());
            using StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, data);

            return textWriter.ToString();
        }

        private static TType XmlDeserialisation<TType>(Stream data)
        {
            var xmlSerializer = new XmlSerializer(typeof(TType));

            return (TType)xmlSerializer.Deserialize(data);
        }
    }
}
