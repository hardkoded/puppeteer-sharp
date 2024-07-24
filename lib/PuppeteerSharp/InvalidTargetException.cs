using System;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    [Serializable]
    internal class InvalidTargetException : PuppeteerException
    {
        public InvalidTargetException()
        {
        }

        public InvalidTargetException(string message) : base(message)
        {
        }

        public InvalidTargetException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
