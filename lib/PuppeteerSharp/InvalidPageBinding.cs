using System;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    [Serializable]
    internal class InvalidPageBinding : Exception
    {
        public InvalidPageBinding()
        {
        }

        public InvalidPageBinding(string message) : base(message)
        {
        }

        public InvalidPageBinding(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidPageBinding(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}