using System;
using System.Runtime.Serialization;

namespace Core.Library.Exceptions
{
    [Serializable]
    public class ErrorPageFoundException : Exception
    {
        public ErrorPageFoundException()
        {
        }

        public ErrorPageFoundException(string message) : base(message)
        {
        }

        public ErrorPageFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ErrorPageFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}