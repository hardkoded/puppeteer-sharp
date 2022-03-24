using System;
using System.Runtime.Serialization;

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

        protected TargetCrashedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
