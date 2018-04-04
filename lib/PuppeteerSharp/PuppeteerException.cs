using System;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    [Serializable]
    public class PuppeteerException : Exception
    {
        public PuppeteerException()
        {
        }

        public PuppeteerException(string message) : base(message)
        {
        }

        public PuppeteerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PuppeteerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
