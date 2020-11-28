using DerMistkaefer.DvbLive.TriasCommunication.Data;
using System;

namespace DerMistkaefer.DvbLive.Backend.Cache.Data
{
    /// <summary>
    /// Stop of an <see cref="CachedTrip"/>
    /// </summary>
    public class CachedTripStop
    {
        /// <summary>
        /// Reference for the stop point in the track.
        /// </summary>
        public string StopPointRef { get; }

        /// <summary>
        /// Name of the stop point.
        /// </summary>
        public string StopPointName { get; }

        /// <summary>
        /// Sequenz number of the stop point in the track.
        /// </summary>
        public int StopSeqNumber { get; }

        /// <summary>
        /// Planned Bay
        /// </summary>
        public string PlannedBay { get; }

        /// <summary>
        /// Arrival TimeTable Time
        /// </summary>
        public DateTime? ArrivalTimeTableTime { get; private set; }

        /// <summary>
        /// Arrival Estimated Time
        /// </summary>
        public DateTime? ArrivalEstimatedTime { get; private set; }

        /// <summary>
        /// Departure TimeTable Time
        /// </summary>
        public DateTime? DepartureTimeTableTime { get; private set; }

        /// <summary>
        /// Departure Estimated Time
        /// </summary>
        public DateTime? DepartureEstimatedTime { get; private set; }

        /// <summary>
        /// Typ of this call in the request.
        /// </summary>
        public CachedTripStopType Type { get; }

        internal CachedTripStop(StopEventCall call)
        {
            StopPointRef = call.StopPointRef;
            StopSeqNumber = call.StopSeqNumber;
            StopPointName = call.StopPointName;
            PlannedBay = call.PlannedBay;
            ArrivalTimeTableTime = call.ArrivalTimeTableTime;
            ArrivalEstimatedTime = call.ArrivalEstimatedTime;
            DepartureTimeTableTime = call.DepartureTimeTableTime;
            DepartureEstimatedTime = call.DepartureEstimatedTime;
            Type = (CachedTripStopType)call.Type;
        }

        /// <summary>
        /// Update the important data of an existent <see cref="CachedTripStop"/>
        /// </summary>
        /// <param name="call">StopEventCall that is use for the update.</param>
        public void UpdateTripStopData(StopEventCall call)
        {
            if (call is null)
            {
                throw new ArgumentNullException(nameof(call));
            }

            ArrivalTimeTableTime = call.ArrivalTimeTableTime;
            ArrivalEstimatedTime = call.ArrivalEstimatedTime;
            DepartureTimeTableTime = call.DepartureTimeTableTime;
            DepartureEstimatedTime = call.DepartureEstimatedTime;
        }
    }
}
