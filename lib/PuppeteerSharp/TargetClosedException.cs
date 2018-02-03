using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    internal class TargetClosedException : Exception
    {
        public TargetClosedException(string message) : base(message)
        {
        }
    }
}