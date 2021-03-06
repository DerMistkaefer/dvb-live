using DerMistkaefer.DvbLive.Backend.Cache.Api;
using DerMistkaefer.DvbLive.Backend.Cache.Data;
using DerMistkaefer.DvbLive.TriasCommunication;
using DerMistkaefer.DvbLive.TriasCommunication.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DerMistkaefer.DvbLive.Backend.HostedServices
{
    /// <summary>
    /// Hosted Service for the Observation and Logging of Trips
    /// </summary>
    public sealed class TripLogger : IHostedService, IDisposable
    {
        private readonly ITriasCommunicator _triasCommunicator;
        private readonly ICacheAdapter _cacheAdapter;
        private readonly ILogger<TripLogger> _logger;
        private Timer? _timer;

        private int _runApiRequestCount = 0;
        private long _runDownloadedBytes = 0;
        private List<string> _runStopPointsProcessed = new List<string>();

        /// <summary>
        /// Load all dependencies in the Hosted Service.
        /// </summary>
        /// <param name="triasCommunicator">Trias Communicator service</param>
        /// <param name="cacheAdapter">Adapter for the Cache</param>
        /// <param name="logger">Service to log events</param>
        public TripLogger(
            ITriasCommunicator triasCommunicator,
            ICacheAdapter cacheAdapter,
            ILogger<TripLogger> logger
            )
        {
            _triasCommunicator = triasCommunicator;
            _triasCommunicator.RequestFinished += OnTriasCommunicatorRequestFinished;
            _cacheAdapter = cacheAdapter;
            _logger = logger;
        }

        /// <inheritdoc cref="IHostedService"/>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Trip Logger Service starting");
            _timer = new Timer(DoLogging, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        /// <inheritdoc cref="IHostedService"/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Trip Logger Service stopping");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <inheritdoc cref="IDisposable"/>
        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void DoLogging(object? state)
        {
            try
            {
                DoLoggingAsync().Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Class}", nameof(TripLogger));
            }
        }

        private async Task DoLoggingAsync()
        {
            _runApiRequestCount = 0;
            _runDownloadedBytes = 0;
            _runStopPointsProcessed = new List<string>();

            var stopWatch = Stopwatch.StartNew();

            await GetInitStopPoint().ConfigureAwait(false);
            ObserveTripsFromStopPoints();

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            _logger.LogInformation("Logging - RunTime {ElapsedTime}", elapsedTime);
            PrintTriasCommunicatorUsage(ts.TotalSeconds);
        }

        private void PrintTriasCommunicatorUsage(double totalSeconds)
        {
            var apiRequestsCountRun = _runApiRequestCount;
            var downloadedKbRun = _runDownloadedBytes / 1000;
            _logger.LogInformation($"TriasCommunicator - Requests Run: {apiRequestsCountRun} - Time: {totalSeconds}s - {apiRequestsCountRun / totalSeconds} r/s - {downloadedKbRun} KB");
            var apiRequestsCountTotal = _triasCommunicator.TotalApiRequestsCount;
            var downloadedKbTotal = _triasCommunicator.TotalDownloadedBytes / 1000;
            _logger.LogInformation($"TriasCommunicator - Requests Total: {apiRequestsCountTotal} - {downloadedKbTotal} KB");
        }

        private void OnTriasCommunicatorRequestFinished(object sender, RequestFinishedEventArgs e)
        {
            _runApiRequestCount++;
            _runDownloadedBytes += e.DownloadedBytes;
        }

        private async Task GetInitStopPoint()
        {
            // Collect Dresden Main Station
            await CollectStopPoint("de:14612:28").ConfigureAwait(false);
        }

        private async Task CollectStopPoint(string triasIdStopPoint)
        {
            if (_cacheAdapter.NeedStopPointCached(triasIdStopPoint))
            {
                //_logger.LogDebug("{function} - {triasIdStopPoint}", nameof(CollectStopPoint), triasIdStopPoint);

                var data = await _triasCommunicator.LocationInformationStopRequest(triasIdStopPoint).ConfigureAwait(false);
                await _cacheAdapter.CacheStopPoint(data).ConfigureAwait(false);

                //_logger.LogDebug("{function} - {triasIdStopPoint} - {name} ✔", nameof(CollectStopPoint), triasIdStopPoint, data.StopPointName);
            }
        }

        private void ObserveTripsFromStopPoints()
        {
            var key = 0;
            var queryStopPoints = GetQueryStopPointsObserve();
            while (queryStopPoints.Count != 0)
            {
                var tasks = new List<Task>();
                foreach (var triasIdStopPoint in queryStopPoints)
                {
                    tasks.Add(ObserveTripsFromStopPoint(key, triasIdStopPoint));
                    key++;
                }
                Task.WhenAll(tasks).Wait();

                queryStopPoints = GetQueryStopPointsObserve();
            }
        }

        private List<string> GetQueryStopPointsObserve()
        {
            var cacheStopPointsNextRun = _cacheAdapter.GetStopPointIds().Where(x => !_runStopPointsProcessed.Contains(x)).ToList();
            _runStopPointsProcessed.AddRange(cacheStopPointsNextRun);

            return cacheStopPointsNextRun;
        }

        private async Task ObserveTripsFromStopPoint(int key, string triasIdStopPoint)
        {
            //_logger.LogDebug("#############################################");
            //_logger.LogDebug("#############################################");
            //_logger.LogDebug("Next Stop: {key} - {triasIdStopPoint}", key, triasIdStopPoint);
            //_logger.LogDebug("Next Stop: {key} - {triasIdStopPoint}", key, triasIdStopPoint);
            //_logger.LogDebug("Next Stop: {key} - {triasIdStopPoint}", key, triasIdStopPoint);
            //_logger.LogDebug("#############################################");
            //_logger.LogDebug("#############################################");
            //PrintTriasCommunicatorUssage(1);

            try
            {
                var data = await _triasCommunicator.StopEventRequest(triasIdStopPoint).ConfigureAwait(false);

                var collectStopEventTasks = data.StopEvents
                    .Select(stopEvent => CollectStopEvent(triasIdStopPoint, stopEvent)).ToList();
                await Task.WhenAll(collectStopEventTasks).ConfigureAwait(false);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                _logger.LogInformation("Trias ServiceUnavailable - {TriasIdStopPoint}", triasIdStopPoint);
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(new Dictionary<string, object> { { "key", key }, { "triasIdStopPoint", triasIdStopPoint } }))
                {
                    _logger.LogError(ex, "Error in {Class} - {Function}", nameof(TripLogger), nameof(ObserveTripsFromStopPoint));
                }
            }
        }

        private async Task CollectStopEvent(string triasIdStopPoint, StopEventResult stopEvent)
        {
            //_logger.LogDebug("{function} - {triasIdStopPoint} - {journey}", nameof(CollectStopEvent), triasIdStopPoint, stopEvent.JourneyRef);
            if (stopEvent.OperatorRef != "voe:16") // IF Operator == DVB
            {
                //_logger.LogDebug("{function} - {triasIdStopPoint} - {journey} ❌ - Operator not DVB", nameof(CollectStopEvent), triasIdStopPoint, stopEvent.JourneyRef);
                return;
            }

            var cachedTrip = _cacheAdapter.TrGetTrip(stopEvent);
            if (cachedTrip == null)
            {
                cachedTrip = _cacheAdapter.AddTripCache(stopEvent);
                await CollectTripStopPoints(cachedTrip).ConfigureAwait(false);
            }
            else
            {
                foreach (var stop in stopEvent.Stops)
                {
                    var cacheStop = cachedTrip.TryGetCachedStop(stop);
                    if (cacheStop == null)
                    {
                        cachedTrip = null; // Reinit Complete Trip
                        break;
                    }
                    cacheStop.UpdateTripStopData(stop);
                }
                if (cachedTrip == null)
                {
                    cachedTrip = _cacheAdapter.UpdateTripCache(stopEvent);
                    await CollectTripStopPoints(cachedTrip).ConfigureAwait(false);
                }
            }

            //_logger.LogDebug("{function} - {triasIdStopPoint} - {journey} ✔", nameof(CollectStopEvent), triasIdStopPoint, stopEvent.JourneyRef);
        }

        private async Task CollectTripStopPoints(CachedTrip cachedTrip)
        {
            var collectStopTasks = new List<Task>
            {
                CollectStopPoint(StopPointRefTriasIdStopPoint(cachedTrip.OriginStopPointRef)),
                CollectStopPoint(StopPointRefTriasIdStopPoint(cachedTrip.DestinationStopPointRef))
            };
            collectStopTasks.AddRange(cachedTrip.Stops.Select(stop => CollectStopPoint(StopPointRefTriasIdStopPoint(stop.StopPointRef))));
            await Task.WhenAll(collectStopTasks).ConfigureAwait(false);
        }

        private static string StopPointRefTriasIdStopPoint(string stopPointRef)
        {
            var split = stopPointRef.Split(':');

            return string.Join(':', split.Take(3));
        }
    }
}
