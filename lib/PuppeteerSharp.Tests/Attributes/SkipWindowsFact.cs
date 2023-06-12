using System.Runtime.InteropServices;
using Xunit;

namespace PuppeteerSharp.Tests.Attributes
{
    internal class SkipWindowsFact : FactAttribute
    {
        public SkipWindowsFact()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            Skip = "Test will not run on windows.";
        }
    }
}
