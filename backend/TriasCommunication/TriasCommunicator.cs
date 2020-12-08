using DerMistkaefer.DvbLive.TriasCommunication.Data;
using DerMistkaefer.DvbLive.TriasCommunication.Exceptions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vdo.trias;

namespace DerMistkaefer.DvbLive.TriasCommunication
{
    /// <summary>
    /// Communicator to the Trias-Api.
    /// </summary>
    internal class TriasCommunicator : ITriasCommunicator
    {
        private readonly ITriasHttpClient _triasHttpClient;
        private readonly ILogger<TriasCommunicator> _logger;

        public TriasCommunicator(ITriasHttpClient triasHttpClient, ILogger<TriasCommunicator> logger)
        {
            _triasHttpClient = triasHttpClient;
            _triasHttpClient.RequestFinished += OnClientRequestFinished;
            _logger = logger;
        }

        /// <inheritdoc cref="ITriasCommunicator"/>
        public int TotalApiRequestsCount { get; private set; }

        /// <inheritdoc cref="ITriasCommunicator"/>
        public long TotalDownloadedBytes { get; private set; }

        /// <inheritdoc cref="ITriasCommunicator" />
        public event TriasEventHandlers.RequestFinishedEventHandler? RequestFinished;

        /// <inheritdoc cref="ITriasCommunicator" />
        public async Task<LocationInformationStopResponse> LocationInformationStopRequest(string idStopPoint)
        {
            var locationInformationRequest = new LocationInformationRequestStructure
            {
                Item = new LocationRefStructure { Item = new StopPointRefStructure1 { Value = idStopPoint } },
                Restrictions = new LocationParamStructure { Type = new[] { LocationTypeEnumeration.stop }, IncludePtModes = true }
            };

            var response = await _triasHttpClient.BaseTriasCall<LocationInformationResponseStructure>(locationInformationRequest).ConfigureAwait(false);

            var locationResult = response.LocationResult?.FirstOrDefault();
            if (response.LocationResult?.Length != 1 || locationResult == null || ((StopPointStructure)locationResult.Location.Item).StopPointRef.Value != idStopPoint)
            {
                var errorCodes = response.ErrorMessage?.SelectMany(x => x.Text).Select(x => x.Text) ?? new List<string>();
                throw new LocationInformationException($"No location could be found. {string.Join('-', errorCodes)}");
            }

            var stopPoint = (StopPointStructure)locationResult.Location.Item;

            return new LocationInformationStopResponse
            {
                IdStopPoint = stopPoint.StopPointRef.Value,
                StopPointName = stopPoint.StopPointName.FirstOrDefault(x => x.Language == "de")?.Text ?? "???",
                Latitude = locationResult.Location.GeoPosition.Latitude,
                Longitude = locationResult.Location.GeoPosition.Longitude
            };
        }

        public async Task TripRequest()
        {
            var tripRequest = new TripRequestStructure
            {

            };

            var response = await _triasHttpClient.BaseTriasCall<TripResponseStructure>(tripRequest).ConfigureAwait(false);
        }

        /// <inheritdoc cref="ITriasCommunicator" />
        public async Task<StopEventResponse> StopEventRequest(string idStopPoint)
        {
            var stopEventRequest = new StopEventRequestStructure
            {
                Location = new LocationContextStructure
                {
                    Item = new LocationRefStructure { Item = new StopPointRefStructure1 { Value = idStopPoint } },
                    DepArrTime = System.DateTime.Now
                },
                Params = new StopEventParamStructure
                {
                    TimeWindow = "5", // Include next 5 minutes of stops.
                    StopEventType = StopEventTypeEnumeration.both,
                    IncludePreviousCalls = true,
                    IncludeOnwardCalls = true,
                    IncludeRealtimeData = true,
                }
            };

            var response = await _triasHttpClient.BaseTriasCall<StopEventResponseStructure>(stopEventRequest).ConfigureAwait(false);

            if (!(response.ErrorMessage?.Length > 0))
            {
                return new StopEventResponse(response, idStopPoint);
            }

            if (response.ErrorMessage.First().Code == "-4030") // STOPEVENT_LOCATIONUNSERVED - Normal because not every stop point has trips in the next 5 minutes.
            {
                return new StopEventResponse(idStopPoint, new List<StopEventResult>());
            }

            var errorCodes = response.ErrorMessage?.SelectMany(x => x.Text).Select(x => x.Text) ?? new List<string>();
            var ex = new StopEventException($"No stop events could be collected. {string.Join('-', errorCodes)}");
            using (_logger.BeginScope(new Dictionary<string, object> { { "idStopPoint", idStopPoint }, { "response", response } }))
            {
                _logger.LogError(ex, "{idStopPoint} - No stop events could be collected.", idStopPoint);
            }
            return new StopEventResponse(idStopPoint, new List<StopEventResult>());
        }

        private void OnClientRequestFinished(object sender, RequestFinishedEventArgs e)
        {
            TotalApiRequestsCount++;
            TotalDownloadedBytes += e.DownloadedBytes;

            var handler = RequestFinished;
            handler?.Invoke(this, e);
        }
    }
}
