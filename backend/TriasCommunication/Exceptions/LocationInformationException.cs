using System;

namespace DerMistkaefer.DvbLive.TriasCommunication.Exceptions
{
    public class LocationInformationException : Exception
    {
        public LocationInformationException(string message) : base(message)
        {
        }

        public LocationInformationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LocationInformationException()
        {
        }
    }
}
