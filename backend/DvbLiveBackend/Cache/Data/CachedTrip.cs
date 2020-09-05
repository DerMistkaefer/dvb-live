using DerMistkaefer.DvbLive.TriasCommunication.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DerMistkaefer.DvbLive.Backend.Cache.Data
{
    /// <summary>
    /// Cached Element of an Trip
    /// </summary>
    public class CachedTrip
    {
        /// <summary>
        /// Refernece for the Operating Day of the journey.
        /// </summary>
        public DateTime OperatingDayRef { get; }

        /// <summary>
        /// Reference for the journey.
        /// </summary>
        public string JourneyRef { get; }

        /// <summary>
        /// Reference for the line.
        /// </summary>
        public string LineRef { get; }

        /// <summary>
        /// Name of the mode of the vehicle.
        /// </summary>
        public string ModeName { get; }

        /// <summary>
        /// Name of the Line.
        /// </summary>
        public string LineName { get; }

        /// <summary>
        /// Reference of the operator.
        /// </summary>
        public string OperatorRef { get; }

        /// <summary>
        /// Description of the Route.
        /// </summary>
        public string RouteDescription { get; }

        /// <summary>
        /// Reference of the Origin StopPoint from the Line/Route.
        /// </summary>
        public string OriginStopPointRef { get; }

        /// <summary>
        /// Reference of the Destination StopPoint from the Line/Route.
        /// </summary>
        public string DestinationStopPointRef { get; }

        /// <summary>
        /// Calls of the Stop Event.
        /// </summary>
        public IReadOnlyList<CachedTripStop> Stops { get; }

        internal CachedTrip(StopEventResult stopEvent)
        {
            OperatingDayRef = stopEvent.OperatingDayRef;
            JourneyRef = stopEvent.JourneyRef;
            LineRef = stopEvent.LineRef;
            ModeName = stopEvent.ModeName;
            LineName = stopEvent.LineName;
            OperatorRef = stopEvent.OperatorRef;
            RouteDescription = stopEvent.RouteDescription;
            OriginStopPointRef = stopEvent.OriginStopPointRef;
            DestinationStopPointRef = stopEvent.DestinationStopPointRef;
            Stops = stopEvent.Stops.Select(x => new CachedTripStop(x)).ToList();
        }

        /// <summary>
        /// Try to get an spezific Trip Stop of this Trip.
        /// </summary>
        /// <param name="call">StopEventCall that is use to find the <see cref="CachedTripStop"/>.</param>
        /// <returns>spezific Trip Stop null if not found</returns>
        public CachedTripStop? TryGetCachedStop(StopEventCall call)
        {
            return Stops.FirstOrDefault(x =>
                x.StopPointRef == call.StopPointRef && x.StopSeqNumber == call.StopSeqNumber);
        }
    }
}
