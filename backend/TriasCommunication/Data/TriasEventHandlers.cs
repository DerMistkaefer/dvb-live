namespace DerMistkaefer.DvbLive.TriasCommunication.Data
{
    /// <summary>
    /// Data Structure for all Trias Event Handler.
    /// </summary>
    public class TriasEventHandlers
    {
        /// <summary>
        /// Event Handler for an Trias Request that has finished.
        /// </summary>
        /// <param name="sender">Sender of this Event</param>
        /// <param name="e">Event Args</param>
        public delegate void RequestFinishedEventHandler(object sender, RequestFinishedEventArgs e);
    }
}
