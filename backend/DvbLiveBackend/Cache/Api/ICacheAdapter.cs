using System.Collections;
using DerMistkaefer.DvbLive.Backend.Cache.Data;
using DerMistkaefer.DvbLive.TriasCommunication.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using DerMistkaefer.DvbLive.GetPublicTransportLines.Data;

namespace DerMistkaefer.DvbLive.Backend.Cache.Api
{
    public interface ICacheAdapter
    {
        bool NeedStopPointCached(string triasIdStopPoint);

        IEnumerable<string> GetStopPointIds();

        Task CacheStopPoint(LocationInformationStopResponse stopPoint);

        CachedTrip? TrGetTrip(StopEventResult stopEvent);

        CachedTrip AddTripCache(StopEventResult stopEvent);

        CachedTrip UpdateTripCache(StopEventResult stopEvent);

        IEnumerable<CachedStopPoint> GetAllStopPoints();

        /// <summary>
        /// Get all Cached Public Transport Lines.
        /// </summary>
        /// <returns>List with all Public Transport Lines</returns>
        IEnumerable<PublicTransportLine> GetAllLines();
    }
}
