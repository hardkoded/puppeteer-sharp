using System;

namespace PuppeteerSharp
{
    [Serializable]
    internal class BufferException : PuppeteerException
    {
        public BufferException()
        {
        }

        public BufferException(string message) : base(message)
        {
        }

        public BufferException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
