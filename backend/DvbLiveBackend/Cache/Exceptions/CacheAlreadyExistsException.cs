using System;

namespace DerMistkaefer.DvbLive.Backend.Cache.Exceptions
{
    public class CacheAlreadyExistsException : Exception
    {
        public CacheAlreadyExistsException(string message) : base(message)
        {
        }

        public CacheAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CacheAlreadyExistsException()
        {
        }
    }
}
