namespace DerMistkaefer.DvbLive.Backend.Cache.Data
{
    /// <summary>
    /// Typ of an <see cref="CachedTripStop"/>.
    /// </summary>
    public enum CachedTripStopType
    {
        /// <summary>
        /// Previous call.
        /// </summary>
        Previous,

        /// <summary>
        /// Call of this Request.
        /// </summary>
        This,

        /// <summary>
        /// Onward call.
        /// </summary>
        Onward
    }
}
