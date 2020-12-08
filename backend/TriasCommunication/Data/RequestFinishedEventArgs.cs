using System;

namespace DerMistkaefer.DvbLive.TriasCommunication.Data
{
    /// <summary>
    /// Event Arguments for the Trias Request Finished Event
    /// </summary>
    public class RequestFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Size of Downloaded Bytes from the Trias-Api
        /// </summary>
        public long DownloadedBytes { get; }

        /// <summary>
        /// Initalize new Request Finished Event Args
        /// </summary>
        /// <param name="downloadedBytes">Downloaded Bytes from the Request</param>
        internal RequestFinishedEventArgs(long downloadedBytes)
        {
            DownloadedBytes = downloadedBytes;
        }
    }
}
