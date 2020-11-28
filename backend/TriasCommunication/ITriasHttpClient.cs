using System.Threading.Tasks;

namespace DerMistkaefer.DvbLive.TriasCommunication
{
    /// <summary>
    /// Interface for an Trias Http Client
    /// </summary>
    internal interface ITriasHttpClient
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
        /// Base Call to the Trias-Api.
        /// </summary>
        /// <typeparam name="TType">Response Data Type for XmlDeserialisation</typeparam>
        /// <param name="requestPayload">Request Payload</param>
        /// <returns>Response Data for the Trias Request.</returns>
        Task<TType> BaseTriasCall<TType>(object requestPayload);
    }
}
