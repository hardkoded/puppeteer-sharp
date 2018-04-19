using System;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    [Serializable]
    public class NavigationException : PuppeteerException
    {
        public NavigationException()
        {
        }

        public NavigationException(string message) : base(message)
        {
        }

        public NavigationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NavigationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}