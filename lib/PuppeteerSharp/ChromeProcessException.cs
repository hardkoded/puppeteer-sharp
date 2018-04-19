using System;

namespace PuppeteerSharp
{
    public class ChromeProcessException : PuppeteerException
    {
        public ChromeProcessException(string message) : base(message)
        {
        }

        public ChromeProcessException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}