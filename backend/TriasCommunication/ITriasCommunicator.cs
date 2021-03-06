using System;
using DerMistkaefer.DvbLive.TriasCommunication.Data;
using System.Threading.Tasks;

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
        public int TotalApiRequestsCount { get; }

        /// <summary>
        /// Size of Downloaded Bytes from the Trias-Api
        /// </summary>
        public long TotalDownloadedBytes { get; }

        /// <summary>
        /// Event Handler that will be triggered when a request is finished.
        /// </summary>
        public event EventHandler<RequestFinishedEventArgs>? RequestFinished;

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
