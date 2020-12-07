namespace DerMistkaefer.DvbLive.Backend.ApiStructure.Output
{
    /// <summary>
    /// Api Output for an Stop Point
    /// </summary>
    public class StopPoint
    {
        /// <summary>
        /// Unique Id for this Stop Point
        /// </summary>
        public string IdStopPoint { get; set; } = "";

        /// <summary>
        /// Stop Point Name
        /// </summary>
        public string StopPointName { get; set; } = "";

        /// <summary>
        /// Geo Longitude of the Stop
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Geo Latitude of the Stop
        /// </summary>
        public decimal Latitude { get; set; }
    }
}
