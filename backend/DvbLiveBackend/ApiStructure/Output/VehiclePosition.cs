using System;
using System.Collections.Generic;

namespace DerMistkaefer.DvbLive.Backend.ApiStructure.Output
{
    /// <summary>
    /// Api Output for an Vehicle Position
    /// </summary>
    public class VehiclePosition
    {
        /// <summary>
        /// Unique Id of this Trip.
        /// </summary>
        public string IdTrip { get; set; } = "";

        /// <summary>
        /// Active Stops of this Trip.
        /// </summary>
        public IEnumerable<VehiclePositionTripStop> Stops { get; set; } = Array.Empty<VehiclePositionTripStop>();
    }
}