using System.Threading.Tasks;
using DerMistkaefer.DvbLive.TriasCommunication.Data;

namespace DerMistkaefer.DvbLive.TriasCommunication
{
    /// <summary>
    /// Interface for an Communicator to the Trias-Api
    /// </summary>
    public interface ITriasCommunicator
    {
        /// <summary>
        /// Count of Requests to the Trias-Api
        /// </summary>
        public int ApiRequestsCount { get; }

        /// <summary>
        /// Size of Downloaded Bytes from the Trias-Api
        /// </summary>
        public long DownloadedBytes { get; }

        /// <summary>
        /// Get Location Information from an StopPoint
        /// </summary>
        /// <param name="idStopPoint">StopPoint to get information</param>
        /// <returns>Location Information for the StopPoint</returns>
        Task<LocationInformationStopResponse> LocationInformationStopRequest(string idStopPoint);

        /// <summary>
        /// Get StopEvents on an stop point
        /// </summary>
        /// <param name="idStopPoint">StopPoint to get stop events</param>
        /// <returns>Stop Events for the Stop Point</returns>
        Task<StopEventResponse> StopEventRequest(string idStopPoint);
    }
}
