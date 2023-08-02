using System.Runtime.InteropServices;

namespace PuppeteerSharp.Tests.Attributes
{
    internal class SkipNonWindowsFact : FactAttribute
    {
        public SkipNonWindowsFact()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            Skip = "Test will only run on windows.";
        }
    }
}
