using System;
using System.Collections.Generic;
using System.Text;
using DerMistkaefer.DvbLive.TriasCommunication.Data;

namespace DerMistkaefer.DvbLive.TriasCommunication.Exceptions
{
    internal class StopEventException : Exception
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
