using DerMistkaefer.DvbLive.Backend.Cache.Api;
using DerMistkaefer.DvbLive.Backend.Cache.Data;
using DerMistkaefer.DvbLive.Backend.Cache.Exceptions;
using DerMistkaefer.DvbLive.Backend.Database.Api;
using DerMistkaefer.DvbLive.Backend.Database.Entity;
using DerMistkaefer.DvbLive.TriasCommunication.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DerMistkaefer.DvbLive.GetPublicTransportLines;
using DerMistkaefer.DvbLive.GetPublicTransportLines.Data;

namespace DerMistkaefer.DvbLive.Backend.Cache
{
    internal class CacheAdapter : ICacheAdapter
    {
        private readonly ILogger<CacheAdapter> _logger;
        private readonly IDatabaseAdapter _databaseAdapter;
        private readonly IPublicTransportLinesCollector _publicTransportLinesCollector;
        private ConcurrentDictionary<string, bool> _existsStopPoints;
        private ConcurrentDictionary<string, CachedStopPoint> _stopPointCache;
        private readonly ConcurrentDictionary<string, bool> _existsTrips;
        private readonly ConcurrentDictionary<string, CachedTrip> _tripCache;
        private PublicTransportLine[] _linesCache;

        public CacheAdapter(ILogger<CacheAdapter> logger, IDatabaseAdapter databaseAdapter, IPublicTransportLinesCollector publicTransportLinesCollector)
        {
            _logger = logger;
            _databaseAdapter = databaseAdapter;
            _publicTransportLinesCollector = publicTransportLinesCollector;
            _existsStopPoints = new ConcurrentDictionary<string, bool>();
            _existsTrips = new ConcurrentDictionary<string, bool>();
            _stopPointCache = new ConcurrentDictionary<string, CachedStopPoint>();
            _tripCache = new ConcurrentDictionary<string, CachedTrip>();
            _linesCache = Array.Empty<PublicTransportLine>();
            LoadCacheFromStorage().Wait();
        }

        private async Task LoadCacheFromStorage()
        {
            var data = await _databaseAdapter.GetAllStopPoints().ConfigureAwait(false);
            var istStopPoints = data.Select(x => new KeyValuePair<string, bool>(x.TriasIdStopPoint, true));
            _existsStopPoints = new ConcurrentDictionary<string, bool>(istStopPoints);
            var istStopPointCache = data.Select(x => new KeyValuePair<string, CachedStopPoint>(x.TriasIdStopPoint, new CachedStopPoint
            {
                TriasIdStopPoint = x.TriasIdStopPoint,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                StopPointName = x.StopPointName
            }));
            _stopPointCache = new ConcurrentDictionary<string, CachedStopPoint>(istStopPointCache);
            var dataLines = await _publicTransportLinesCollector.GetPublicTransportLines().ConfigureAwait(false);
            _linesCache = dataLines.ToArray();
        }

        /// <inheritdoc cref="ICacheAdapter"/>
        public bool NeedStopPointCached(string triasIdStopPoint)
        {
            return _existsStopPoints.TryAdd(triasIdStopPoint, false);
        }

        /// <inheritdoc cref="ICacheAdapter"/>
        public IEnumerable<string> GetStopPointIds()
        {
            return _existsStopPoints.Select(x => x.Key);
        }

        /// <inheritdoc cref="ICacheAdapter"/>
        public Task CacheStopPoint(LocationInformationStopResponse stopPoint)
        {
            var cachedStopPoint = new CachedStopPoint
            {
                TriasIdStopPoint = stopPoint.IdStopPoint,
                StopPointName = stopPoint.StopPointName,
                Longitude = stopPoint.Longitude,
                Latitude = stopPoint.Latitude
            };

            return CacheStopPoint(cachedStopPoint);
        }

        private async Task CacheStopPoint(CachedStopPoint stopPoint)
        {
            _stopPointCache.TryAdd(stopPoint.TriasIdStopPoint, stopPoint);
            var entity = new StopPoints
            {
                TriasIdStopPoint = stopPoint.TriasIdStopPoint,
                StopPointName = stopPoint.StopPointName,
                Longitude = stopPoint.Longitude,
                Latitude = stopPoint.Latitude
            };

            await _databaseAdapter.InsertStopPoint(entity).ConfigureAwait(false);
        }

        /// <inheritdoc cref="ICacheAdapter"/>
        public CachedTrip? TrGetTrip(StopEventResult stopEvent)
        {
            var uniqueTripKey = $"{stopEvent.OperatingDayRef}_{stopEvent.JourneyRef}";
            if (!_existsTrips.TryAdd(uniqueTripKey, true))
            {
                return WhileGetTripCache(stopEvent.OperatingDayRef, stopEvent.JourneyRef);
            }

            return null;
        }

        /// <inheritdoc cref="ICacheAdapter"/>
        public CachedTrip AddTripCache(StopEventResult stopEvent)
        {
            var cacheTrip = new CachedTrip(stopEvent);
            var uniqueTripKey = $"{cacheTrip.OperatingDayRef}_{cacheTrip.JourneyRef}";
            if (!_tripCache.TryAdd(uniqueTripKey, cacheTrip))
            {
                throw new CacheAlreadyExistsException($"Trip Cache - {uniqueTripKey} - Allredy exist.");
            }

            return cacheTrip;
        }

        /// <inheritdoc cref="ICacheAdapter"/>
        public CachedTrip UpdateTripCache(StopEventResult stopEvent)
        {
            var cacheTrip = new CachedTrip(stopEvent);
            var uniqueTripKey = $"{cacheTrip.OperatingDayRef}_{cacheTrip.JourneyRef}";
            if (!_tripCache.TryGetValue(uniqueTripKey, out var existCacheTrip))
            {
                throw new CacheAlreadyExistsException($"Trip Cache - {uniqueTripKey} - Did not exist.");
            };
            if (!_tripCache.TryUpdate(uniqueTripKey, cacheTrip, existCacheTrip))
            {
                throw new CacheAlreadyExistsException($"Trip Cache - {uniqueTripKey} - Update failed exist.");
            };

            return cacheTrip;
        }

        /// <inheritdoc cref="ICacheAdapter"/>
        public IEnumerable<CachedStopPoint> GetAllStopPoints()
            => _stopPointCache.Values;

        /// <inheritdoc cref="ICacheAdapter"/>
        public IEnumerable<PublicTransportLine> GetAllLines()
            => _linesCache;

        /// <inheritdoc cref="ICacheAdapter"/>
        public IEnumerable<CachedTrip> GetAllActiveTrips() 
            => _tripCache.Values;

        /// <inheritdoc cref="ICacheAdapter"/>
        public CachedStopPoint GetStopPointById(string triasIdStopPoint)
            => _stopPointCache[triasIdStopPoint];

        private CachedTrip? GetTripCache(DateTime operatingDayRef, string journeyRef)
        {
            return _tripCache.Values.FirstOrDefault(x => x.OperatingDayRef == operatingDayRef && x.JourneyRef == journeyRef);
        }

        private CachedTrip WhileGetTripCache(DateTime operatingDayRef, string journeyRef)
        {
            var cancelCounter = 0;
            var output = GetTripCache(operatingDayRef, journeyRef);
            while (output == null)
            {
                if (cancelCounter++ > 10)
                {
                    throw new NotImplementedException("Cache could not be created.");
                }
                _logger.LogDebug("{function} - Wait that cache will be exist. {operatingDayRef} - {journeyRef}", nameof(WhileGetTripCache), operatingDayRef, journeyRef);
                Thread.Sleep(5);
                output = GetTripCache(operatingDayRef, journeyRef);
            }

            return output;
        }
    }
}
