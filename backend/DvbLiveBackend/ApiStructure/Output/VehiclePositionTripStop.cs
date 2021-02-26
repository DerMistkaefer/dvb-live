using System;

namespace DerMistkaefer.DvbLive.Backend.ApiStructure.Output
{
    /// <summary>
    /// Api Output for an Trip Stop of the Vehicle Position Output
    /// </summary>
    public class VehiclePositionTripStop
    {
        /// <summary>
        /// Id of the Trip Stop.
        /// </summary>
        public string IdStopPoint { get; set; } = "";
        
        /// <summary>
        /// Arrival Time on this Stop of an Trip. As Unix Timestamp.
        /// </summary>
        public int? ArrivalTime { get; set; }
        
        /// <summary>
        /// Departure Tie on this Stop of an Trip. As Unix Timestamp.
        /// </summary>
        public int? DepartureTime { get; set; }
    }
}