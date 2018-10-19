using System;

namespace Automa.EntityComponents
{
    public class EntitiesException : Exception
    {
        public EntitiesException(string message) : base(message)
        {
        }

        public EntitiesException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}