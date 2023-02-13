using System.Runtime.InteropServices;
using Xunit;

namespace PuppeteerSharp.Tests.SingleFileDeployment
{
    internal class FactRunableOnWindowsAttribute : FactAttribute
    {
        public FactRunableOnWindowsAttribute()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            Skip = "Test will only run on windows.";
        }
    }
}
