using GeoJSON.Net.Geometry;

namespace DerMistkaefer.DvbLive.Backend.ApiStructure.Output
{
    /// <summary>
    /// Api Output for an Vehicle Position
    /// </summary>
    public class VehiclePosition
    {
        /// <summary>
        /// Current Vehicle Position
        /// </summary>
        public Position Position { get; set; } = new Position(0, 0);
    }
}