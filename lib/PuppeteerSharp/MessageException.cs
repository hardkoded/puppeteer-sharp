using System;

namespace PuppeteerSharp
{
    public class MessageException : PuppeteerException
    {
        public MessageException(string message) : base(message)
        {
        }

        public MessageException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}