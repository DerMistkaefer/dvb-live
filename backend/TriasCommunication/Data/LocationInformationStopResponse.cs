using System;
using System.Collections.Generic;
using System.Text;

namespace DerMistkaefer.DvbLive.TriasCommunication.Data
{
    /// <summary>
    /// Response Structure for an Location Information Stop Request
    /// </summary>
    public class LocationInformationStopResponse
    {
        /// <summary>
        /// Id of this Stop Point
        /// </summary>
        public string IdStopPoint { get; set; } = "";

        /// <summary>
        /// Stop Point Name
        /// </summary>
        public string StopPointName { get; set; } = "";

        /// <summary>
        /// Longtitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Geo Latitude of the Stop
        /// </summary>
        public double Latitude { get; set; }
    }
}
