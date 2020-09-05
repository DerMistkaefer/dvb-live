namespace DerMistkaefer.DvbLive.Backend.Database.Entity
{
    /// <summary>
    /// Entity for an Stop Point
    /// </summary>
    public class StopPoints
    {
        /// <summary>
        /// Internal Database Id of this Stop Point
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Trias Id of this Stop Point
        /// </summary>
        public string TriasIdStopPoint { get; set; } = "";

        /// <summary>
        /// Stop Point Name
        /// </summary>
        public string StopPointName { get; set; } = "";

        /// <summary>
        /// Longtitude
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Geo Latitude of the Stop
        /// </summary>
        public decimal Latitude { get; set; }
    }
}
