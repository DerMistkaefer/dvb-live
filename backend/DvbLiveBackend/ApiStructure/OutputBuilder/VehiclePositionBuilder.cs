using DerMistkaefer.DvbLive.Backend.ApiStructure.Output;
using DerMistkaefer.DvbLive.Backend.Cache.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DerMistkaefer.DvbLive.Backend.ApiStructure.OutputBuilder
{
    /// <summary>
    /// Stop Point Api Output Builder / Converter Functions
    /// </summary>
    public static class VehiclePositionBuilder
    {
        /// <summary>
        /// Convert the Cache Data Structure to the Api Output Structure
        /// </summary>
        /// <param name="cache">cached stop point that should be converted</param>
        /// <returns>api output for the stop point</returns>
        public static VehiclePosition ConvertToApiOutput(this CachedTrip cache)
        {
            if (cache is null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            return new VehiclePosition
            {
                IdTrip = cache.JourneyRef,
                Stops = GetRelevantStops(cache.Stops).OrderBy(x => x.StopSeqNumber).ConvertToApiOutput()
            };
        }

        /// <summary>
        /// Convert the Cache Data Structure to the Api Output Structure
        /// </summary>
        /// <param name="cache">cached stop points that should be converted</param>
        /// <returns>api output for the stop points</returns>
        public static IEnumerable<VehiclePosition> ConvertToApiOutput(this IEnumerable<CachedTrip> cache)
            => cache.Select(x => x.ConvertToApiOutput());
        
        private static IEnumerable<CachedTripStop> GetRelevantStops(IEnumerable<CachedTripStop> stops)
        {
            var now = DateTime.UtcNow;
            var cachedTripStops = stops as CachedTripStop[] ?? stops.ToArray();
            var lastOldStop = cachedTripStops.LastOrDefault(x => (x.ArrivalCalculationTime ?? x.DepartureCalculationTime ?? now) <= now)?.StopSeqNumber ?? 0;
            
            return cachedTripStops.Where(x => x.StopSeqNumber > (lastOldStop - 1));
        }

        private static VehiclePositionTripStop ConvertToApiOutput(this CachedTripStop cache)
        {
            return new VehiclePositionTripStop
            {
                IdStopPoint = cache.TriasIdStopPoint,
                ArrivalTime = DateTimeToUnixTimeStamp(cache.ArrivalCalculationTime),
                DepartureTime = DateTimeToUnixTimeStamp(cache.DepartureCalculationTime)
            };
        }

        private static int? DateTimeToUnixTimeStamp(DateTime? dateTime)
            => (int?)dateTime?.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        private static IEnumerable<VehiclePositionTripStop> ConvertToApiOutput(this IEnumerable<CachedTripStop> cache)
            => cache.Select(x => x.ConvertToApiOutput());
    }
}
