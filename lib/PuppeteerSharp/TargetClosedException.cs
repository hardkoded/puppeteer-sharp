using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    internal class TargetClosedException : PuppeteerException
    {
        public TargetClosedException(string message) : base(message)
        {
        }
    }
}