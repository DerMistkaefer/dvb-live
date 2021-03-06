using System;
using DerMistkaefer.DvbLive.TriasCommunication.Data;
using System.Threading.Tasks;

namespace DerMistkaefer.DvbLive.TriasCommunication
{
    /// <summary>
    /// Interface for an Trias Http Client
    /// </summary>
    internal interface ITriasHttpClient
    {
        /// <summary>
        /// Event Handler that will be triggered when a request is finished.
        /// </summary>
        public event EventHandler<RequestFinishedEventArgs>? RequestFinished;

        /// <summary>
        /// Base Call to the Trias-Api.
        /// </summary>
        /// <typeparam name="TType">Response Data Type for XmlDeserialisation</typeparam>
        /// <param name="requestPayload">Request Payload</param>
        /// <returns>Response Data for the Trias Request.</returns>
        Task<TType> BaseTriasCall<TType>(object requestPayload);
    }
}
