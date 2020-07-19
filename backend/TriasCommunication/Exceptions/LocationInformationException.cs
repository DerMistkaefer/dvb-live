using System;
using System.Collections.Generic;
using System.Text;

namespace DerMistkaefer.DvbLive.TriasCommunication.Exceptions
{
    internal class LocationInformationException : Exception
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
