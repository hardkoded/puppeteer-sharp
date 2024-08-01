using System;

namespace PuppeteerSharp
{
    [Serializable]
    internal class TargetCrashedException : PuppeteerException
    {
        public TargetCrashedException()
        {
        }

        public TargetCrashedException(string message) : base(message)
        {
        }

        public TargetCrashedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
