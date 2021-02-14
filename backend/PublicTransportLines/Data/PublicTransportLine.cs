using GeoJSON.Net.Feature;

namespace DerMistkaefer.DvbLive.GetPublicTransportLines.Data
{
    /// <summary>
    /// DataStructure of an Public Transport Line.
    /// </summary>
    public class PublicTransportLine
    {
        /// <summary>
        /// Local regional title of the Line.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Description of the start point from this line.
        /// </summary>
        public string From { get; }

        /// <summary>
        /// Description of the end point from this line.
        /// </summary>
        public string To { get; }

        /// <summary>
        /// Geo Json Data of this Line.
        /// </summary>
        public Feature Line { get; }

        #region Additional Data
        
        /// <summary>
        /// Url of Line Change Data for this line.
        /// </summary>
        public string? UrlLineChange { get; set; }
        
        /// <summary>
        /// Information about the clocking of this line.
        /// </summary>
        public string? Clocking { get; set; }

        #endregion

        /// <summary>
        /// Create a new PublicTransportLine.
        /// </summary>
        /// <param name="title">Local regional title of the Line.</param>
        /// <param name="from">Description of the start point from this line.</param>
        /// <param name="to">Description of the end point from this line.</param>
        /// <param name="line">Geo Json Data of this Line.</param>
        public PublicTransportLine(string title, string from, string to, Feature line)
        {
            Title = title;
            From = from;
            To = to;
            Line = line;
        }
    }
}