using System.Collections.Generic;
using System.Threading.Tasks;
using DerMistkaefer.DvbLive.GetPublicTransportLines.Data;

namespace DerMistkaefer.DvbLive.GetPublicTransportLines
{
    /// <summary>
    /// Interface for an collector that collects all public transport lines (bus, subway, tram, ...) for an specify region. 
    /// </summary>
    public interface IPublicTransportLinesCollector
    {
        /// <summary>
        /// Get all public transport lines (bus, subway, tram, ...)
        /// </summary>
        /// <returns>list with public transport lines</returns>
        public Task<IEnumerable<PublicTransportLine>> GetPublicTransportLines();
    }
}