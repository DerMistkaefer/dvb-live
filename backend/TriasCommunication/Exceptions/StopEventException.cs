using System;

namespace DerMistkaefer.DvbLive.TriasCommunication.Exceptions
{
    public class StopEventException : Exception
    {
        public StopEventException(string message) : base(message)
        {
        }

        public StopEventException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public StopEventException()
        {
        }
    }
}
