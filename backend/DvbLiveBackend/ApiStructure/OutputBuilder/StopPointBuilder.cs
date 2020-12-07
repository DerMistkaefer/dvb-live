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
    public static class StopPointBuilder
    {
        /// <summary>
        /// Convert the Cache Data Structure to the Api Output Structure
        /// </summary>
        /// <param name="cache">cached stop point that should be converted</param>
        /// <returns>api output for the stop point</returns>
        public static StopPoint ConvertToApiOutput(this CachedStopPoint cache)
        {
            if (cache is null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            return new StopPoint
            {
                IdStopPoint = cache.TriasIdStopPoint,
                Latitude = cache.Latitude,
                Longitude = cache.Longitude,
                StopPointName = cache.StopPointName
            };
        }

        /// <summary>
        /// Convert the Cache Data Structure to the Api Output Structure
        /// </summary>
        /// <param name="cache">cached stop points that should be converted</param>
        /// <returns>api output for the stop points</returns>
        public static IEnumerable<StopPoint> ConvertToApiOutput(this IEnumerable<CachedStopPoint> cache)
            => cache.Select(x => x.ConvertToApiOutput());
    }
}
