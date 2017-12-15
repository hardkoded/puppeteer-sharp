using System;
namespace PuppeteerSharp
{
    public class ChromeProcessException : Exception
    {
        public ChromeProcessException(string message) : base(message)
        {
        }
    }
}
